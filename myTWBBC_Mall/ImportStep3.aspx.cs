using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using TWBBC_Mall.Controllers;
using TWBBC_Mall.Models;

/// <summary>
/// 有自訂客戶品號的商城使用
/// 資料關聯:PKSYS.refCOPMG
/// 20191218:目前未有自訂客戶品號的商城,故此功能無法測試.
/// </summary>
public partial class myTWBBC_Mall_ImportStep3 : SecurityIn
{
    public string ErrMsg;
    public IQueryable<RefColumn> DTitems;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1231", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    CustomExtension.AlertMsg("來源參數錯誤", "{0}ImportList.aspx".FormatThis(FuncPath()));
                    return;
                }

                //載入資料
                LookupData();

            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示:基本資料 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search,out ErrMsg).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                MallID = fld.MallID,
                MallName = fld.MallName

            }).FirstOrDefault();
        if (query == null)
        {
            CustomExtension.AlertMsg("無法取得資料", "{0}ImportStep2.aspx?id={1}".FormatThis(FuncPath(), Req_DataID));
            return;
        }

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string CustID = query.CustID;
        string CustName = query.CustName;

        this.lb_TraceID.Text = TraceID;
        this.lb_Cust.Text = "{0} ({1})".FormatThis(CustName, CustID);
        this.lb_Mall.Text = query.MallName;

        //hidden field
        this.hf_MallID.Value = query.MallID.ToString();
        this.hf_CustID.Value = CustID;
        this.hf_TraceID.Value = TraceID;

        query = null;


        //載入暫存清單
        LookupData_Detail();

    }


    #endregion


    #region -- 資料顯示:資料清單 --

    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();

        //取得詳細品項,置入DTitems,讓ItemDataBound取用
        DTitems = _data.GetTempDTItems(Req_DataID, out ErrMsg);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetTempDTGroup(Req_DataID, out ErrMsg);

        //----- 資料整理:繫結 ----- 
        this.lvDetailList.DataSource = query;
        this.lvDetailList.DataBind();
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料
                string Get_OrderID = DataBinder.Eval(dataItem.DataItem, "OrderID").ToString();

                //取得控制項
                Literal lt_Items = (Literal)e.Item.FindControl("lt_Items");

                /*
                 * 顯示暫存檔單身資料,以OrderID為篩選
                 */
                if (DTitems != null)
                {
                    StringBuilder html = new StringBuilder();
                    var _items = DTitems.Where(c => c.OrderID.Equals(Get_OrderID));
                    foreach (var item in _items)
                    {
                        html.Append("<tr>");
                        html.Append("<td><input id=\"prodID-{0}\" data-id=\"{0}\" class=\"prodID-{2}\" type=\"text\" maxlength=\"40\" value=\"{1}\" /></td>"
                            .FormatThis(item.Data_ID, item.ProdID, item.OrderID));
                        html.Append("<td>" + item.ProdSpec + "</td>");
                        html.Append("<td>" + item.ProdName + "</td>");
                        html.Append("<td><input id=\"buyCnt-{0}\" data-id=\"{0}\" class=\"buyCnt-{2}\" type=\"number\" min=\"1\" maxlength=\"40\" value=\"{1}\" /></td>"
                            .FormatThis(item.Data_ID, item.BuyCnt, item.OrderID));
                        html.Append("</tr>");
                    }

                    lt_Items.Text = html.ToString();
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message.ToString());
        }
    }
    
    #endregion


    #region -- 按鈕事件 --
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();
        string mallID = this.hf_MallID.Value;
        string dataType = this.hf_Type.Value;
        string traceID = this.hf_TraceID.Value;

        //建立基本資料參數, 用途:資料篩選條件
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID,
            MallID = Convert.ToInt32(mallID),
            Update_Who = fn_Params.UserGuid
        };

        #region -- 資料Check.1:品號 --

        if (!_data.Check_Step1(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.1:品號。\\nERP品號檢查失敗(Step3), 檢查ERP客戶品號是否重複";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }
        #endregion


        #region -- 資料Check.2:訂單編號 --

        if (!_data.Check_Step2(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.2:訂單編號。\\n訂單編號檢查失敗, 狀態碼未更新(Step3).";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }

        #endregion


        #region -- 資料Check:活動 --

        if (!_data.Check_Promo(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "Check:促銷活動。\\n資料判斷時出了問題(Step3)";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }

        #endregion


        #region -- 資料Check.3:價格 --

        if (!_data.Check_Step3(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.3:價格。\\nERP價格檢查失敗, 狀態碼及價格未更新(Step3).";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }

        #endregion
        
        //Update Status
        if (!_data.Update_Status(Req_DataID, 12, out ErrMsg))
        {
            //[Log]
            string Msg = "狀態更新失敗(Step3)..." + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }


        //Next Page
        Response.Redirect("{0}myTWBBC_Mall/ImportStep4.aspx?id={1}".FormatThis(fn_Params.WebUrl, Req_DataID));
    }

    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC_Mall/".FormatThis(
            fn_Params.WebUrl);
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
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}ImportStep3.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._thisPage = value;
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
            string tempUrl = CustomExtension.getCookie("EF_TWBBC_Mall");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }
    #endregion

}