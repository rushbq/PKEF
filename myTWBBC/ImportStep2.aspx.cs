using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToExcel;
using PKLib_Method.Methods;
using TW_BBC.Controllers;
using TW_BBC.Models;

public partial class myTWBBC_ImportStep2 : SecurityIn
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
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1210", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                //取得基本資料
                LookupData();

            }
            catch (Exception)
            {

                throw;
            }

        }
    }


    #region -- 資料讀取 --
    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            //----- 原始資料:條件篩選 -----
            search.Add("DataID", Req_DataID);

            //----- 原始資料:取得所有資料 -----
            var query = _data.GetOne(search, out ErrMsg).FirstOrDefault();
            if (query == null)
            {
                CustomExtension.AlertMsg("查無資料", Page_SearchUrl);
                return;
            }

            //----- 資料整理:填入資料 -----
            string _traceID = query.TraceID;
            string _dataType = query.Data_Type;
            string _dataTypeName = query.Data_TypeName;
            string _custID = query.CustID;
            string _custName = query.CustName;
            string _orderType = query.OrderType;
            string _currency = query.Currency;
            string _fileName = query.Upload_File;
            Int16 _status = query.Status;

            //release
            query = null;

            /*
             * 判斷狀態, 導至指定頁面
             */
            switch (_status)
            {
                case 13:
                case 99:
                    //完成,作廢(離開此頁)
                    Response.Redirect(Page_SearchUrl);
                    break;

                default:
                    //不採取動作
                    break;
            }

            //設定檔案完整路徑
            string filePath = @"{0}{1}{2}".FormatThis(
                System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"]
                , UploadFolder(_traceID).Replace("/", "\\")
                , _fileName);

            //填入表單欄位
            lb_TraceID.Text = _traceID;
            hf_TraceID.Value = _traceID;
            lb_DataType.Text = _dataTypeName;
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_OrderType.Text = _orderType;
            lb_Currency.Text = _currency;
            //完整路徑
            hf_FullFileName.Value = filePath;

            //----- [元件][LinqToExcel] - 取得工作表 -----
            Set_SheetMenu(filePath);
        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "載入單頭資料時發生錯誤;" + ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            lt_ShowMsg.Text = msg;
            return;
        }
        finally
        {
            //release
            _data = null;
        }
    }


    /// <summary>
    /// 產生工作表選單
    /// </summary>
    /// <param name="filePath"></param>
    private void Set_SheetMenu(string filePath)
    {
        try
        {
            //查詢Excel
            var excelFile = new ExcelQueryFactory(filePath);

            //取得Excel 頁籤
            var data = excelFile.GetWorksheetNames();

            this.ddl_Sheet.Items.Clear();
            this.ddl_Sheet.Items.Add(new ListItem("選擇要匯入的工作表", ""));

            foreach (var item in data)
            {
                this.ddl_Sheet.Items.Add(new ListItem(item.ToString(), item.ToString()));
            }
        }
        catch (Exception ex)
        {
            lt_ShowMsg.Text = ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            ph_WorkBtns.Visible = false;
        }


    }
    #endregion


    #region -- 按鈕事件 --
    /// <summary>
    /// 重新上傳:刪除資料及檔案
    /// </summary>
    protected void lbtn_ReNew_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        try
        {
            //----- 方法:刪除資料 -----
            if (false == _data.Delete(Req_DataID, out ErrMsg))
            {
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = "重新上傳操作失敗";
                return;
            }
            else
            {
                //刪除整個Folder檔案
                _ftp.FTP_DelFolder(UploadFolder(hf_TraceID.Value));

                //導向至Step1
                Response.Redirect(prevPage);
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        #region -- [Check] 欄位檢查 --

        //declare
        string errTxt = "";
        string _sheet = ddl_Sheet.SelectedValue;

        //Check Sheet
        if (string.IsNullOrWhiteSpace(_sheet))
        {
            errTxt += "[檢查] 請選擇工作表";
        }

        //show alert
        if (!string.IsNullOrWhiteSpace(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, thisPage);
            return;
        }

        #endregion

        //宣告
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        try
        {
            #region -- [Get] 取得基本資料 --

            //----- 原始資料:條件篩選 -----
            search.Add("DataID", Req_DataID);

            //----- 原始資料:取得所有資料 -----
            var query = _data.GetOne(search, out ErrMsg).FirstOrDefault();

            //----- 資料整理:填入資料 -----
            string _dataID = query.Data_ID.ToString();
            string _traceID = query.TraceID;
            string _dataType = query.Data_Type;
            string _custID = query.CustID;

            //release
            query = null;

            #endregion

            //[清空Log]
            _data.Update_ClearLog(_dataID, out ErrMsg);


            #region -- 建立單身資料 --

            //[Excel] - 取得輸入參數
            string _sheetName = ddl_Sheet.SelectedValue; //工作表名
            string _filePath = hf_FullFileName.Value; //完整路徑(抓Excel資料用)

            //填入基本資料Inst
            var baseData = new ImportData
            {
                Data_ID = new Guid(_dataID),
                TraceID = _traceID,
                Data_Type = _dataType, //*** [特殊處理] 單身Shipfrom預設值 & 若為Prod,則Shipfrom更新為產品中心的出貨地 ***
                Sheet_Name = _sheetName,
                Update_Who = fn_Params.UserGuid
            };

            //填入單身資料Inst - 取得Excel資料欄位
            var query_Xls = _data.Get_ExcelData(_filePath, _sheetName);


            //寫入單身資料, 更新單頭欄位
            if (!_data.CreateDetail(baseData, query_Xls, out ErrMsg))
            {
                string msg = "(Step2)單身資料填入失敗;" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job1] 檢查客戶品號 --

            if (!_data.CheckJob1(_dataID, _custID, _dataType, out ErrMsg))
            {
                string msg = "(Step2)檢查客戶品號...[Job1];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job2] 檢查寶工品號 --

            if (!_data.CheckJob2(_dataID, _dataType, "", out ErrMsg))
            {
                string msg = "(Step2)檢查寶工品號...[Job2];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job3] 更新公司別/資料庫名 --

            if (!_data.CheckJob3(_dataID, "", out ErrMsg))
            {
                string msg = "(Step2)更新公司別/資料庫名...[Job3];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job4] 更新產品其他欄位 --

            if (!_data.CheckJob4(_dataID, "", out ErrMsg))
            {
                string msg = "(Step2)更新產品其他欄位...[Job4];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job5] 取得價格 --

            if (!_data.CheckJob5(_dataID, _custID, "", out ErrMsg))
            {
                string msg = "(Step2)取得價格...[Job5];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            //[清空Log]
            _data.Update_ClearLog(_dataID, out ErrMsg);

            //導至下一步
            Response.Redirect(nextPage);


        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }


    /// <summary>
    /// 選擇工作表, 產生預覽資料 - onChange
    /// </summary>
    protected void ddl_Sheet_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.ddl_Sheet.SelectedIndex > 0 && !string.IsNullOrEmpty(this.hf_FullFileName.Value))
        {
            //取得參數
            string filePath = hf_FullFileName.Value;
            string sheetName = ddl_Sheet.SelectedValue;

            //取得資料
            TWBBCRepository _data = new TWBBCRepository();
            try
            {
                var data = _data.Get_ExcelData(filePath, sheetName);

                lvViewList.DataSource = data;
                lvViewList.DataBind();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _data = null;
            }

        }
    }
    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC".FormatThis(fn_Params.WebUrl);
    }


    /// <summary>
    /// 上傳目錄(+TraceID)
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder(string traceID)
    {
        return "{0}TWBBC/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceID);
    }

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 設定參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            string data = Request.QueryString["id"];

            return string.IsNullOrWhiteSpace(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 上一頁網址
    /// </summary>
    private string _prevPage;
    public string prevPage
    {
        get
        {
            return "{0}/ImportStep1.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._prevPage = value;
        }
    }

    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}/ImportStep2.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._thisPage = value;
        }
    }

    /// <summary>
    /// 下一頁網址
    /// </summary>
    private string _nextPage;
    public string nextPage
    {
        get
        {
            return "{0}/ImportStep3.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._nextPage = value;
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
            string tempUrl = CustomExtension.getCookie("TWBBC");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }
    #endregion

}