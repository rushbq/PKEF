﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using PKLib_Method.Methods;
using ERP_PriceData.Controllers;
using ERP_PriceData.Models;
using System.Web.UI.WebControls;

public partial class myErpPriceData_ImportStep1 : SecurityIn
{
    public string ErrMsg;

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region --權限--
                //[權限判斷] Start
                /* 
                 * 使用公司別代號，判斷對應的MENU ID
                 */
                bool isPass = false;

                switch (Req_DBS)
                {
                    case "TW":
                        isPass = fn_CheckAuth.CheckAuth_User("416", out ErrMsg);
                        break;

                    case "SH":
                        isPass = fn_CheckAuth.CheckAuth_User("417", out ErrMsg);
                        break;
                }

                if (!isPass)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                #endregion

                //Get TraceID
                string _traceID = NewTraceID();
                lb_TraceID.Text = _traceID;
                hf_TraceID.Value = _traceID;

                //DBS
                lb_DBS.Text = fn_Desc.PubAll.AreaDesc(Req_DBS);

                //單別
                Get_TypeList(ddl_OrderType, "請選擇單別", "");

            }
            catch (Exception)
            {

                throw;
            }

        }
    }

    #region -- 按鈕事件 --
    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        string errTxt = "";
        string _cust = val_Cust.Text;
        string _orderType = val_OrderType.Text;
        string _validDate = tb_validDate.Text;
        string _way = val_Way.Text;
        string _orderNo = tb_OrderNo.Text;

        //必填檢查
        if (string.IsNullOrWhiteSpace(_cust) || string.IsNullOrWhiteSpace(_orderType)
            || string.IsNullOrWhiteSpace(_validDate))
        {
            errTxt += "===請檢查以下欄位===\\n";
            errTxt += "客戶\\n";
            errTxt += "ERP單別\\n";
            errTxt += "生效日\\n";
        }

        //依編碼方式檢查單號
        if (_way.Equals("4") && string.IsNullOrWhiteSpace(_orderNo))
        {
            errTxt += "單號空白\\n";
        }

        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        //Check file
        HttpPostedFile hpf = fu_File.PostedFile;

        if (hpf.ContentLength == 0)
        {
            CustomExtension.AlertMsg("請確認檔案已選擇!!", "");
            return;
        }

        //資料處理
        string[] myData = Add_Data();

        //取得回傳參數
        string DataID = myData[0];
        string ProcCode = myData[1];
        string Message = myData[2];

        //判斷是否處理成功
        if (!ProcCode.Equals("200"))
        {
            lt_ShowMsg.Text = Message;
            ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //導至下一步
            Response.Redirect("{0}/ImportStep2.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), DataID, Req_DBS));
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

        //取得欄位資料
        string myTraceID = hf_TraceID.Value;
        string _cust = val_Cust.Text;
        string _orderType = val_OrderType.Text;
        string _validDate = tb_validDate.Text;
        string _orderNo = tb_OrderNo.Text;


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
                string OrgFileName = System.IO.Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = System.IO.Path.GetExtension(OrgFileName).ToLower();
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
                string OrgFileName = System.IO.Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = System.IO.Path.GetExtension(OrgFileName).ToLower();

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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();


        //----- 設定:資料欄位 -----
        string guid = CustomExtension.GetGuid();

        var data = new ImportData
        {
            Data_ID = new Guid(guid),
            TraceID = myTraceID,
            CustID = _cust,
            OrderType = _orderType,
            OrderNo = _orderNo,
            DBS = Req_DBS,
            ValidDate = _validDate,
            Upload_File = ITempList[0].Param_FileName,
            Create_Who = fn_Params.UserGuid
        };

        //----- 方法:建立資料 -----      
        if (!_data.Create(data, out ErrMsg))
        {
            //刪除檔案
            _ftp.FTP_DelFolder(UploadFolder(myTraceID));

            //顯示錯誤
            Message = "資料建立失敗<br/>({0})".FormatThis(ErrMsg);
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


    #region -- 附加功能 --
    /// <summary>
    /// New TraceID
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
    /// 取得單別
    /// </summary>
    /// <param name="ddl"></param>
    /// <param name="rootName"></param>
    /// <param name="inputValue"></param>
    private void Get_TypeList(DropDownList ddl, string rootName, string inputValue)
    {

        //----- 宣告:資料參數 -----
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetPriceTypeID(Req_DBS.ToUpper());

        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(
                "{0} - {1}".FormatThis(item.ID, item.Label)
                //選擇值為ID + Type, 以#區隔, Type用來判斷是否要填單號(4 -> 手動填寫單號)
                , "{0}#{1}".FormatThis(item.ID, item.NoType)));
        }

        //被選擇值
        if (!string.IsNullOrWhiteSpace(inputValue))
        {
            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(inputValue));
        }

        query = null;
    }

    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myErpPriceData".FormatThis(fn_Params.WebUrl);
    }

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 設定參數 - DBS
    /// </summary>
    private string _Req_DBS;
    public string Req_DBS
    {
        get
        {
            string data = Request.QueryString["dbs"];

            return string.IsNullOrWhiteSpace(data) ? "TW" : data.ToString();
        }
        set
        {
            this._Req_DBS = value;
        }
    }


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("ErpPriceData");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx?dbs=" + Req_DBS : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }

    #endregion


    #region -- 上傳參數 --
    /// <summary>
    /// 上傳目錄(+TraceID)
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder(string traceID)
    {
        return "{0}ErpPriceData/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceID);
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "xlsx";
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