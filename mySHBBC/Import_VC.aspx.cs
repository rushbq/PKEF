using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using LinqToExcel;
using PKLib_Method.Methods;
using SH_BBC.Controllers;
using SH_BBC.Models;

/*
 * [VC專用上傳]
 * 說明:
 * 原始檔為各家客戶在同一SHEET,且使用簡稱表示,需從DB對應抓取ERP代號
 * 系統拆解後產生各客戶的EXCEL (TraceID/abc.xlsx),並建立好基本資料(原Step1的功能)
 * 完成後導回列表頁(Mall=3)
 */

public partial class mySHBBC_Import_VC : SecurityIn
{
    public string ErrMsg;

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("861", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
            }


        }
        catch (Exception)
        {

            throw;
        }
    }



    #region -- 按鈕事件 --

    /// <summary>
    /// 執行
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //[Ste1] 取得上傳後的新檔名
        string getFileName = do_FileUpload(Request.Files, out ErrMsg);

        //取得完整路徑(原始檔案)
        string filePath = @"{0}{1}/{2}".FormatThis(
            System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"]
            , UploadFolder()
            , getFileName);

        //Check
        if (string.IsNullOrWhiteSpace(getFileName))
        {
            lt_Msg.Text = "抓不到檔案.." + ErrMsg;
            ph_Message.Visible = true;
            return;
        }


        //[Step2] 讀檔, 拆分資料, 並取得各客戶的對應編號
        List<TempDT> getExcelDT = do_FixExcel(filePath, out ErrMsg);
        if (getExcelDT == null || getExcelDT.Count == 0)
        {
            //查無資料, 刪除原始檔
            _ftp.FTP_DelFile(UploadFolder(), getFileName);

            lt_Msg.Text = "拆分資料失敗.." + ErrMsg;
            ph_Message.Visible = true;
            return;
        }


        //[Step3] 取得TraceID, 上傳新檔案至FTP, 產生各自的基本資料
        int errCnt = 0;
        for (int row = 0; row < getExcelDT.Count; row++)
        {
            string getTraceID = NewTraceID();
            string getCustID = getExcelDT[row].CustID;
            DataTable getDT = getExcelDT[row].myDT; //New Excel內容
            string getFolder = NewUploadFolder(getTraceID); //Ftp Folder
            string getNewFileName = "{0}.xlsx".FormatThis(getCustID);

            //產生Excel,並轉成byte
            byte[] myByteExcel = CustomExtension.ExcelToByte(getDT, false);

            //判斷Ftp資料夾, 不存在則建立
            _ftp.FTP_CheckFolder(getFolder);

            //上傳至FTP
            if (!_ftp.FTP_doUploadWithByte(myByteExcel, getFolder, getNewFileName))
            {
                errCnt++;
                ErrMsg += "{0}上傳失敗..<br/>".FormatThis(getCustID);
            }


            //建立基本資料
            #region -- 資料處理 --

            //----- 宣告:資料參數 -----
            SHBBCRepository _data = new SHBBCRepository();

            //----- 設定:資料欄位 -----
            string guid = CustomExtension.GetGuid();

            var data = new ImportData
            {
                Data_ID = new Guid(guid),
                TraceID = getTraceID,
                MallID = 3, //VC(固定值)
                CustID = getCustID,
                SalesID = "", //改為直接由SQL取得
                Data_Type = 1, //1:未出貨訂單
                Upload_File = getNewFileName,
                Create_Who = fn_Params.UserGuid
            };

            //----- 方法:建立資料 -----      
            if (!_data.Create(data))
            {
                errCnt++;
                ErrMsg += "{0}資料建立失敗..<br/>".FormatThis(getCustID);

                //刪除檔案
                _ftp.FTP_DelFolder(getFolder);
            }

            _data = null;

            #endregion
        }


        //[Step999] 刪除原始檔
        _ftp.FTP_DelFile(UploadFolder(), getFileName);

        //Show error
        if (errCnt > 0)
        {
            lt_Msg.Text = ErrMsg;
            ph_Message.Visible = true;
            return;
        }


        //[Finish] 導至列表頁
        Response.Redirect(Page_SearchUrl);
    }

    #endregion


    #region -- 資料編輯 Start --

    /// <summary>
    /// 檔案上傳 - Step1
    /// </summary>
    /// <param name="hfc">來源檔案</param>
    /// <param name="ErrMsg"></param>
    /// <returns>檔名</returns>
    private string do_FileUpload(HttpFileCollection hfc, out string ErrMsg)
    {
        try
        {
            #region -- 檔案處理 --

            //宣告
            List<IOTempParam> ITempList = new List<IOTempParam>();
            Random rnd = new Random();
            ErrMsg = "";

            //--- 檔案檢查 ---
            for (int idx = 0; idx <= hfc.Count - 1; idx++)
            {
                //取得個別檔案
                HttpPostedFile hpf = hfc[idx];

                if (hpf.ContentLength > FileSizeLimit)
                {
                    //[提示]
                    ErrMsg = "檔案大小超出限制, 每個檔案大小限制為 {0} MB".FormatThis(FileSizeLimit);
                    return "";
                }

                if (hpf.ContentLength > 0)
                {
                    //取得原始檔名
                    string OrgFileName = Path.GetFileName(hpf.FileName);
                    //取得副檔名
                    string FileExt = Path.GetExtension(OrgFileName).ToLower();
                    if (false == CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                    {
                        //[提示]
                        ErrMsg = "檔案副檔名不符規定, 僅可上傳副檔名為 {0}".FormatThis(FileExtLimit.Replace("|", ", "));
                        return "";
                    }
                }
            }


            //--- 檔案暫存List ---
            for (int idx = 0; idx <= hfc.Count - 1; idx++)
            {
                //取得個別檔案
                HttpPostedFile hpf = hfc[idx];

                if (hpf.ContentLength > 0)
                {
                    //取得原始檔名
                    string OrgFileName = Path.GetFileName(hpf.FileName);
                    //取得副檔名
                    string FileExt = Path.GetExtension(OrgFileName).ToLower();

                    //設定檔名, 重新命名
                    string myFullFile = String.Format(@"{0:yyMMddHHmmssfff}{1}{2}"
                        , DateTime.Now
                        , rnd.Next(0, 99)
                        , FileExt);


                    //判斷副檔名, 未符合規格的檔案不上傳
                    if (CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                    {
                        //設定暫存-檔案
                        ITempList.Add(new IOTempParam(myFullFile, OrgFileName, hpf));
                    }
                }
            }

            #endregion


            #region -- 儲存檔案 --

            if (ITempList.Count == 0)
            {
                ErrMsg = "未上傳檔案!";
                return "";
            }
            else
            {
                int errCnt = 0;
                //判斷資料夾, 不存在則建立
                _ftp.FTP_CheckFolder(UploadFolder());

                //暫存檔案List
                for (int row = 0; row < ITempList.Count; row++)
                {
                    //取得個別檔案
                    HttpPostedFile hpf = ITempList[row].Param_hpf;

                    //執行上傳
                    if (false == _ftp.FTP_doUpload(hpf, UploadFolder(), ITempList[row].Param_FileName))
                    {
                        errCnt++;
                    }
                }

                if (errCnt > 0)
                {
                    ErrMsg = "檔案上傳失敗, 失敗筆數為 {0} 筆, 請重新整理後再上傳!".FormatThis(errCnt);
                    return "";
                }

                //取得檔名
                return ITempList[0].Param_FileName;
            }

            #endregion
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return "";
        }

    }


    /// <summary>
    /// 解析Excel,回傳分解後的資料 - Step2
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private List<TempDT> do_FixExcel(string filePath, out string ErrMsg)
    {
        try
        {
            //宣告
            ErrMsg = "";
            List<TempDT> listDT = new List<TempDT>();

            //使用LinqToExcel讀取Excel檔案
            var excelFile = new ExcelQueryFactory(filePath);

            //Excel 轉 List
            var queryXls = excelFile.Worksheet(0).ToList();

            //將指定欄位GroupBy
            var groups = queryXls
                .GroupBy(gp => new
                {
                    id = gp[7].Cast<string>()
                })
                .Select(fld => new
                {
                    place = fld.Key.id
                });
            if (groups.Count() == 0)
            {
                ErrMsg = "查無資料";
                return null;
            }

            //取得客戶對應(BBC_RefTable)(filter:VC)
            SHBBCRepository _data = new SHBBCRepository();
            var custs = _data.GetRefCust(null, out ErrMsg)
                .Where(fld => fld.DispType.Equals("VC"))
                .Select(fld => new
                {
                    place = fld.Platform_ID,
                    custID = fld.ERP_ID
                });
            _data = null;

            foreach (var gp in groups)
            {
                //欄位:分配機構(客戶)
                string place = gp.place;

                if (!string.IsNullOrWhiteSpace(place))
                {
                    //比對客戶編號, 對應不到的客戶不顯示
                    var cust = custs.Where(c => c.place.Equals(place)).FirstOrDefault();
                    string custID = cust == null ? "" : cust.custID;

                    if (!string.IsNullOrWhiteSpace(custID))
                    {
                        //取得各客戶資料,命名表頭欄位(*** 重要:若Excel欄位有增減,要調整此處 ***) 
                        var newDT = queryXls
                                .Where(c => c[7].Cast<string>().Equals(place))
                                .Select(fld => new
                                {
                                    fld1 = fld[0],
                                    fld2 = fld[1],
                                    fld3 = fld[2],
                                    fld4 = fld[3],
                                    fld5 = fld[4],
                                    fld6 = fld[5],
                                    fld7 = fld[6],
                                    fld8 = fld[7],
                                    fld9 = fld[8],
                                    fld10 = fld[9],
                                    fld11 = fld[10],
                                    fld12 = fld[11],
                                    fld13 = fld[12],
                                    fld14 = fld[13],
                                    fld15 = fld[14],
                                    fld16 = fld[15],
                                    fld17 = fld[16],
                                    fld18 = fld[17],
                                    fld19 = fld[18],
                                    fld20 = fld[19],
                                    fld21 = fld[20],
                                    fld22 = fld[21],
                                    fld23 = fld[22],
                                    fld24 = fld[23]
                                });

                        //轉成DataTable
                        DataTable myDT = CustomExtension.LINQToDataTable(newDT);

                        //暫存DT
                        listDT.Add(new TempDT(custID, myDT));

                    }

                }

            }

            return listDT;
        }
        catch (Exception ex)
        {
            ErrMsg = "請先檢查Excel格式...".FormatThis(ex.Message.ToString());
            return null;
        }
    }

    #endregion -- 資料編輯 End --


    #region -- 參數設定 --

    /// <summary>
    /// TraceID
    /// </summary>
    /// <returns></returns>
    private string NewTraceID()
    {
        //產生TraceID
        long ts = Cryptograph.GetCurrentTime();

        Random rnd = new Random();
        int myRnd = rnd.Next(1, 99);

        return "{0}{1}".FormatThis(ts, myRnd);
    }

    /// <summary>
    /// 設定參數 - 列表頁Url (Mall=3)
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = "{0}mySHBBC/ImportList.aspx?Mall=3".FormatThis(fn_Params.WebUrl);

            return tempUrl;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

    #endregion


    #region -- 上傳參數 --
    /// <summary>
    /// 上傳目錄(原始檔暫存)
    /// </summary>
    /// <returns></returns>
    private string UploadFolder()
    {
        return "{0}SHBBC/VCGroup/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
    }

    /// <summary>
    /// 上傳目錄(各自的目錄)
    /// </summary>
    /// <param name="traceId">追蹤編號</param>
    /// <returns></returns>
    private string NewUploadFolder(string traceId)
    {
        return "{0}SHBBC/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceId);
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "xls|xlsx";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 限制上傳的檔案大小(1MB = 1024000), 5MB
    /// </summary>
    private int _FileSizeLimit;
    public int FileSizeLimit
    {
        get
        {
            return 5120000;
        }
        set
        {
            this._FileSizeLimit = value;
        }
    }

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class IOTempParam
    {
        /// <summary>
        /// [參數] - 檔名
        /// </summary>
        private string _Param_FileName;
        public string Param_FileName
        {
            get { return this._Param_FileName; }
            set { this._Param_FileName = value; }
        }

        /// <summary>
        /// [參數] -原始檔名
        /// </summary>
        private string _Param_OrgFileName;
        public string Param_OrgFileName
        {
            get { return this._Param_OrgFileName; }
            set { this._Param_OrgFileName = value; }
        }


        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_FileName">系統檔名</param>
        /// <param name="Param_OrgFileName">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public IOTempParam(string Param_FileName, string Param_OrgFileName, HttpPostedFile Param_hpf)
        {
            this._Param_FileName = Param_FileName;
            this._Param_OrgFileName = Param_OrgFileName;
            this._Param_hpf = Param_hpf;
        }

    }

    /// <summary>
    /// 暫存DT
    /// </summary>
    public class TempDT
    {
        /// <summary>
        /// [參數] - CustID
        /// </summary>
        private string _CustID;
        public string CustID
        {
            get { return this._CustID; }
            set { this._CustID = value; }
        }

        /// <summary>
        /// [參數] - 資料表
        /// </summary>
        private DataTable _myDT;
        public DataTable myDT
        {
            get { return this._myDT; }
            set { this._myDT = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="CustID">客編</param>
        /// <param name="myDT">DataTable</param>
        public TempDT(string CustID, DataTable myDT)
        {
            this._CustID = CustID;
            this._myDT = myDT;
        }

    }

    #endregion
}