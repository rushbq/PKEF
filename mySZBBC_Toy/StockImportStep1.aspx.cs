using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZBBC_Toy.Controllers;
using SZBBC_Toy.Models;

/*
 * [Step1]
 */

public partial class mySZBBC_Toy_StockImportStep1 : SecurityIn
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
                if (fn_CheckAuth.CheckAuth_User("922", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                //Get Class
                Get_ClassList(myClass.mall, this.ddl_Mall, "選擇商城");

            }


        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料讀取 --

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
        var query = _data.GetClassList(cls).Where(fld => fld.IsStock.Equals("Y"));


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
            Response.Redirect("{0}mySZBBC_Toy/StockImportStep2.aspx?dataID={1}".FormatThis(
                fn_Params.WebUrl
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
                Message = "檔案上傳失敗, 失敗筆數為 {0} 筆, 請重新整理後再上傳!".FormatThis(errCnt);
                return new string[] { DataID, ProcCode, Message };
            }
        }

        #endregion


        #region -- 資料處理 --

        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        bool IsExists = false;
        string guid = "";
        string oldFile = "";


        /** 判斷是否已新增過(條件:MallID, StockDate) **/
        //取得參數
        string _stockDate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
        string _mallID = ddl_Mall.SelectedValue;

        //新增條件
        search.Add("MallID", _mallID);
        search.Add("StockDate", _stockDate);

        //判斷
        var check = _data.GetStockImportData(search, out ErrMsg).FirstOrDefault();
        if (check == null)
        {
            guid = CustomExtension.GetGuid();
        }
        else
        {
            IsExists = true;
            guid = check.Data_ID.ToString();
            oldFile = check.Upload_File;
        }

        //將資料裝入容器
        var data = new StockImportData
        {
            Data_ID = new Guid(guid),
            MallID = Convert.ToInt32(_mallID),
            CustID = this.Cust_ID_Val.Text.Trim(),
            StockDate = _stockDate,
            Upload_File = ITempList[0].Param_FileName,
            Create_Who = fn_Params.UserGuid,
            Update_Who = fn_Params.UserGuid
        };

        if (IsExists)
        {
            //刪除舊檔案
            _ftp.FTP_DelFile(UploadFolder(), oldFile);

            //----- 方法:更新資料 -----     
            if (!_data.Update_StockData(data, out ErrMsg))
            {
                Message = "資料建立失敗.." + ErrMsg;
                ProcCode = "999";
                return new string[] { DataID, ProcCode, Message };
            }
            else
            {
                DataID = guid;
                ProcCode = "200";
                return new string[] { DataID, ProcCode, Message };
            }
        }
        else
        {
            //----- 方法:建立資料 -----      
            if (!_data.Create_StockData(data, out ErrMsg))
            {
                Message = "資料建立失敗.." + ErrMsg;
                ProcCode = "999";
                return new string[] { DataID, ProcCode, Message };
            }
            else
            {
                DataID = guid;
                ProcCode = "200";
                return new string[] { DataID, ProcCode, Message };
            }

        }


        #endregion
    }


    #endregion -- 資料編輯 End --


    #region -- 上傳參數 --
    /// <summary>
    /// 上傳目錄
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder()
    {
        return "{0}SZBBC_Toy/ECStock/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
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