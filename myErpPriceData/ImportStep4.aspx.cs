using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PKLib_Method.Methods;
using ERP_PriceData.Controllers;

public partial class myErpPriceData_ImportStep4 : SecurityIn
{
    public string ErrMsg;

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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();
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
            string _dataID = query.Data_ID.ToString();
            string _traceID = query.TraceID;
            string _dbs = query.DBS;
            string _custID = query.CustID;
            string _custName = query.CustName;
            string _orderType = query.OrderType;
            string _orderNo = query.OrderNo;
            string _validDate = query.ValidDate;
            string _fileName = query.Upload_File;
            Int16 _status = query.Status;

            //release
            query = null;


            //填入表單欄位
            lb_TraceID.Text = _traceID;
            lb_DBS.Text = fn_Desc.PubAll.AreaDesc(_dbs);
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_OrderType.Text = _orderType;
            lb_OrderNo.Text = _orderNo;
            lb_validDate.Text = _validDate;


        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            //release
            _data = null;
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
    #endregion

}