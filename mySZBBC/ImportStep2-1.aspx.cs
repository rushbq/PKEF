using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_BBC.Controllers;
using SZ_BBC.Models;

/// <summary>
/// 此頁僅供 POP(1)/ 天貓(2) 使用
/// </summary>
public partial class mySZBBC_PromoConfig_Edit : SecurityIn
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
                if (fn_CheckAuth.CheckAuth_User("810", out ErrMsg) == false)
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
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                FileName = fld.Upload_File,
                MallID = fld.MallID,
                MallName = fld.MallName,
                DataType = fld.Data_Type,
                DataTypeName = fld.Data_TypeName

            }).FirstOrDefault();
        if (query == null)
        {
            CustomExtension.AlertMsg("無法取得資料", "{0}ImportStep2.aspx?dataID={1}".FormatThis(FuncPath(), Req_DataID));
            return;
        }

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string CustID = query.CustID;
        string CustName = query.CustName;

        this.lt_TraceID.Text = TraceID;
        this.lt_CustName.Text = "{0} ({1})".FormatThis(CustName, CustID);
        this.lt_MallName.Text = query.MallName;
        this.lt_TypeName.Text = query.DataTypeName;

        //hidden field
        this.hf_MallID.Value = query.MallID.ToString();
        this.hf_CustID.Value = CustID;
        this.hf_Type.Value = query.DataType.ToString();
        this.hf_TraceID.Value = TraceID;

        query = null;


        //載入暫存清單
        LookupData_Detail();

    }


    #endregion


    #region -- 資料顯示:資料清單 --

    /// <summary>
    /// 預先載入暫存檔單身
    /// </summary>
    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

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

    /// <summary>
    /// 發票明細輸入欄-預設值
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_number"></param>
    /// <param name="_shipWho"></param>
    /// <returns></returns>
    protected string defRemarkValue(string _title, string _number, string _shipWho)
    {
        return "单位名称:{0}&#10;纳税人识别号:{1}&#10;地址:{2}&#10;电话:{3}&#10;开户银行:{4}&#10;开户账号:{5}&#10;收票人姓名:{6}".FormatThis(
                _title
                , _number
                , ""
                , ""
                , ""
                , ""
                , _shipWho
            );
    }

    #endregion


    #region -- 按鈕事件 --
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        string mallID = this.hf_MallID.Value;
        string dataType = this.hf_Type.Value;
        string traceID = this.hf_TraceID.Value;

        //建立基本資料參數, 用途:資料篩選條件
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID,
            MallID = Convert.ToInt32(mallID),
            Update_Who = fn_Params.UserGuid,
            Data_Type = Convert.ToDecimal(dataType)
        };

        #region -- 資料Check.1:品號 --

        if (!_data.Check_Step1(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.1:品號。\\nERP品號檢查失敗(Step2-1), 檢查客戶品號對應是否重複";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }
        #endregion


        #region -- 資料Check.2:訂單編號 --

        if (!_data.Check_Step2(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.2:訂單編號。\\n訂單編號檢查失敗, 狀態碼未更新(Step2-1)";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }

        #endregion


        #region -- 資料Check:活動 --

        if (!_data.Check_Promo(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "Check:促銷活動。\\n資料判斷時出了問題(Step2-1)";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }

        #endregion


        #region -- 資料Check.3:價格 --

        if (!_data.Check_Step3(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.3:價格。\\nERP價格檢查失敗, 狀態碼及價格未更新(Step2-1)";
            _data.Create_Log(baseData, Msg + ErrMsg, out ErrMsg);

            CustomExtension.AlertMsg(Msg, thisPage);
            return;
        }

        #endregion


        //Next Page
        Response.Redirect("{0}mySZBBC/ImportStep3.aspx?dataID={1}".FormatThis(fn_Params.WebUrl, Req_DataID));
    }

    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}mySZBBC/".FormatThis(
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
            string data = Request.QueryString["dataID"];

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
            return "{0}ImportStep2-1.aspx?dataID={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._thisPage = value;
        }
    }

    #endregion

}