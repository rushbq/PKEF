using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using TW_BBC.Controllers;

public partial class myTWBBC_View : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //[權限判斷]
            if (fn_CheckAuth.CheckAuth_User("1216", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //判斷編號是否為空
            if (string.IsNullOrWhiteSpace(Req_DataID))
            {
                CustomExtension.AlertMsg("編號空白,即將返回列表頁.", Page_SearchUrl);
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


    #region -- 資料顯示:基本資料 --

    /// <summary>
    /// 取得基本資料
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
            var query = _data.GetOne(search, out ErrMsg);

            //----- 資料整理:繫結 ----- 
            this.lv_BaseData.DataSource = query;
            this.lv_BaseData.DataBind();


            //載入其他明細資料
            if (query.Count() > 0)
            {
                string traceID = query.FirstOrDefault().TraceID;

                //Error Log
                LookupData_Detail(Req_DataID);

                //EDI轉入記錄
                LookupData_EDILog(traceID);

                //ERP 訂單
                LookupData_ERPOrderData(traceID);

                //ERP 銷貨單
                LookupData_ERPSalesData(traceID);
            }


            //release
            query = null;
        }
        catch (Exception ex)
        {
            ph_ErrMessage.Visible = true;
            lt_ShowMsg.Text = ex.Message.ToString();
            return;
        }
        finally
        {
            //Release
            _data = null;
        }

    }

    protected void lv_BaseData_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料:錯誤訊息
                string _errMessage = DataBinder.Eval(dataItem.DataItem, "ErrMessage").ToString();


                //欄位顯示:Error
                PlaceHolder ph_ErrLog = (PlaceHolder)e.Item.FindControl("ph_ErrLog");
                ph_ErrLog.Visible = !string.IsNullOrWhiteSpace(_errMessage);


            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion


    /// <summary>
    /// 單身資料
    /// </summary>
    /// <param name="_parentID"></param>
    private void LookupData_Detail(string _parentID)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.GetDetailList(_parentID, null, out ErrMsg);

            //----- 資料整理:繫結 ----- 
            lv_DetailList.DataSource = query;
            lv_DetailList.DataBind();

            //Show Error
            if (!string.IsNullOrWhiteSpace(ErrMsg))
            {
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = ErrMsg;
                return;
            }

        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "載入單身資料時發生錯誤;" + ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            lt_ShowMsg.Text = msg;
            return;
        }

        finally
        {
            _data = null;
        }
    }


    /// <summary>
    /// EDI Log
    /// </summary>
    /// <param name="traceID"></param>
    private void LookupData_EDILog(string traceID)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();

        //----- 原始資料:取得資料 -----
        var query = _data.GetEDILog(traceID, out ErrMsg);

        if (query != null)
        {
            //----- 資料整理:繫結 ----- 
            this.lv_EdiLog.DataSource = query;
            this.lv_EdiLog.DataBind();
        }

        //release
        query = null;
        _data = null;
    }

    /// <summary>
    /// ERP訂單
    /// </summary>
    /// <param name="traceID"></param>
    private void LookupData_ERPOrderData(string traceID)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> _search = new Dictionary<string, string>();

        _search.Add("TraceID", traceID);

        //----- (TW)原始資料:取得資料 -----
        var data_tw = _data.GetERPData_Order("TW", _search, out ErrMsg);
        if (data_tw != null)
        {
            //----- 資料整理:繫結 ----- 
            lv_OrderList_tw1.DataSource = data_tw;
            lv_OrderList_tw1.DataBind();

            //count
            lt_CntTW_1.Text = data_tw.Count().ToString();
        }

        //----- (SH)原始資料:取得資料 -----
        var data_sh = _data.GetERPData_Order("SH", _search, out ErrMsg);
        if (data_sh != null)
        {
            //----- 資料整理:繫結 ----- 
            lv_OrderList_sh1.DataSource = data_sh;
            lv_OrderList_sh1.DataBind();

            //count
            lt_CntSH_1.Text = data_sh.Count().ToString();
        }

        //release
        data_tw = null;
        data_sh = null;
        _data = null;
    }


    /// <summary>
    /// ERP銷貨單
    /// </summary>
    /// <param name="traceID"></param>
    private void LookupData_ERPSalesData(string traceID)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> _search = new Dictionary<string, string>();

        _search.Add("TraceID", traceID);

        //----- (TW)原始資料:取得資料 -----
        var data_tw = _data.GetERPData_Sales("TW", _search, out ErrMsg);
        if (data_tw != null)
        {
            //----- 資料整理:繫結 ----- 
            lv_SalesOrderList_tw2.DataSource = data_tw;
            lv_SalesOrderList_tw2.DataBind();

            //count
            lt_CntTW_2.Text = data_tw.Count().ToString();
        }

        //----- (SH)原始資料:取得資料 -----
        var data_sh = _data.GetERPData_Sales("SH", _search, out ErrMsg);
        if (data_sh != null)
        {
            //----- 資料整理:繫結 ----- 
            lv_SalesOrderList_sh2.DataSource = data_sh;
            lv_SalesOrderList_sh2.DataBind();

            //count
            lt_CntSH_2.Text = data_sh.Count().ToString();
        }

        //release
        data_tw = null;
        data_sh = null;
        _data = null;
    }


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC".FormatThis(fn_Params.WebUrl);
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


    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}TWBBC/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["Ftp_RefUrl"] + System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }
    #endregion

}