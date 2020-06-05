using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ERP_PriceData.Controllers;

public partial class myErpPriceData_View : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();
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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();
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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();

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
            string tempUrl = CustomExtension.getCookie("ErpPriceData");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx?dbs=" + Req_DBS : Server.UrlDecode(tempUrl);
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
            return "{0}ErpPriceData/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["Ftp_RefUrl"] + System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }
    #endregion

}