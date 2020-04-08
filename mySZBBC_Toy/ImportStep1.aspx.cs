using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZBBC_Toy.Controllers;
using SZBBC_Toy.Models;

/*
 * [Step1]
 * 判斷TraceID是否有資料(PageLoad)
 *  :有資料 -> 取得DataID -> 前往Step2
 *  :無資料 -> 「按下一步」 -> 新增資料 -> 上傳檔案 -> 取得DataID -> 前往Step2
 */

public partial class mySZBBC_ImportStep1 : SecurityIn
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
                if (fn_CheckAuth.CheckAuth_User("910", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷參數是否為空
                Check_Params();

                //Get Class
                Get_ClassList(myClass.mall, this.ddl_Mall, "選擇商城");

                //判斷TraceID是否已新增資料
                string myDataID = Check_Data();
                if (!string.IsNullOrEmpty(myDataID))
                {
                    //自動導至下一步
                    Response.Redirect("{0}mySZBBC_Toy/ImportStep2.aspx?dataID={1}".FormatThis(
                        Application["WebUrl"]
                        , myDataID));
                    return;
                }
            }


        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料讀取 --

    /// <summary>
    /// 判斷參數是否為空
    /// </summary>
    private void Check_Params()
    {
        //判斷TraceID , Type不可為空
        if (string.IsNullOrEmpty(Req_TraceID) || string.IsNullOrEmpty(Req_Type))
        {
            this.ph_Message.Visible = true;
            this.ph_Content.Visible = false;
            this.ph_Buttons.Visible = false;
        }
        else
        {
            this.ph_Message.Visible = false;
            this.ph_Content.Visible = true;
            this.ph_Buttons.Visible = true;
        }
    }

    /// <summary>
    /// 取得類別資料 
    /// </summary>
    /// <param name="cls">類別參數</param>
    /// <param name="ddl">下拉選單object</param>
    /// <param name="rootName">第一選項顯示名稱</param>
    private void Get_ClassList(myClass cls, DropDownList ddl, string rootName)
    {
        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls);


        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        query = null;
    }


    /// <summary>
    /// 資料判斷
    /// </summary>
    /// <returns></returns>
    private string Check_Data()
    {
        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.TraceID, Req_TraceID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search)
            .Select(fld => new
            {
                myID = fld.Data_ID.ToString()
            })
            .FirstOrDefault();

        //----- 資料整理:回傳編號 -----
        return query == null ? "" : query.myID;
    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //資料處理
        string[] myData = Add_Data();

        //取得回傳參數
        string DataID = myData[0];
        string ProcCode = myData[1];
        string Message = myData[2];

        //判斷是否處理成功
        if (!ProcCode.Equals("200"))
        {
            this.lt_UploadMessage.Text = Message;
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導至下一步
            Response.Redirect("{0}mySZBBC_Toy/ImportStep2.aspx?dataID={1}".FormatThis(
                Application["WebUrl"]
                , DataID));
            return;
        }

    }

    #endregion


    #region -- 資料編輯 Start --

    /// <summary>
    /// 資料新增
    /// </summary>
    /// <returns></returns>
    private string[] Add_Data()
    {
        //回傳參數初始化
        string DataID = "";
        string ProcCode = "0";
        string Message = "";



        //----- 判斷:TraceID是否重複
        string myTraceID;
        if (string.IsNullOrEmpty(Check_Data()))
        {
            myTraceID = Req_TraceID;
        }
        else
        {
            myTraceID = NewTraceID();
        }

        #region -- 檔案處理 --

        //宣告
        List<IOTempParam> ITempList = new List<IOTempParam>();
        Random rnd = new Random();


        //取得上傳檔案集合
        HttpFileCollection hfc = Request.Files;


        //--- 檔案檢查 ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > FileSizeLimit)
            {
                //[提示]
                Message = "檔案大小超出限制, 每個檔案大小限制為 {0} MB".FormatThis(FileSizeLimit);
                return new string[] { DataID, ProcCode, Message };
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
                    Message = "檔案副檔名不符規定, 僅可上傳副檔名為 {0}".FormatThis(FileExtLimit.Replace("|", ", "));
                    return new string[] { DataID, ProcCode, Message };
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

        if (ITempList.Count > 0)
        {
            int errCnt = 0;
            //判斷資料夾, 不存在則建立
            _ftp.FTP_CheckFolder(UploadFolder(myTraceID));

            //暫存檔案List
            for (int row = 0; row < ITempList.Count; row++)
            {
                //取得個別檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;

                //執行上傳
                if (false == _ftp.FTP_doUpload(hpf, UploadFolder(myTraceID), ITempList[row].Param_FileName))
                {
                    errCnt++;
                }
            }

            if (errCnt > 0)
            {
                Message = "檔案上傳失敗, 失敗筆數為 {0} 筆, 請重新整理後再上傳!".FormatThis(errCnt);
                return new string[] { DataID, ProcCode, Message };
            }
        }

        #endregion


        #region -- 資料處理 --

        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();


        //----- 設定:資料欄位 -----
        string guid = CustomExtension.GetGuid();

        var data = new ImportData
        {
            Data_ID = new Guid(guid),
            TraceID = myTraceID,
            MallID = Convert.ToInt32(this.ddl_Mall.SelectedValue),
            CustID = this.Cust_ID_Val.Text.Trim(),
            SalesID = this.Sales_ID_Val.Text.Trim(),
            Data_Type = Convert.ToDecimal(Req_Type),
            Upload_File = ITempList[0].Param_FileName,
            Create_Who = fn_Params.UserGuid
        };


        //----- 方法:建立資料 -----      
        if (!_data.Create(data))
        {
            Message = "資料建立失敗";
            return new string[] { DataID, ProcCode, Message };
        }
        else
        {
            DataID = guid;
            ProcCode = "200";
            return new string[] { DataID, ProcCode, Message };
        }


        #endregion
    }


    #endregion -- 資料編輯 End --


    #region -- 參數設定 --

    /// <summary>
    /// 當TraceID重複時, 重新產生
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
    /// 設定參數 - 取得TraceID
    /// </summary>
    public string Req_TraceID
    {
        get
        {
            string data = Request.QueryString["ts"];

            return string.IsNullOrEmpty(data) ? "" : Cryptograph.MD5Decrypt(data, System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"]);
        }
        set
        {
            this._Req_TraceID = value;
        }
    }
    private string _Req_TraceID;

    /// <summary>
    /// 設定參數 - 取得type
    /// </summary>
    public string Req_Type
    {
        get
        {
            string data = Request.QueryString["type"];

            return string.IsNullOrEmpty(data) ? "" : data;
        }
        set
        {
            this._Req_Type = value;
        }
    }
    private string _Req_Type;



    #endregion


    #region -- 上傳參數 --
    /// <summary>
    /// 上傳目錄(+TraceID)
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder(string traceID)
    {
        return "{0}SZBBC_Toy/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceID);
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
    #endregion
}