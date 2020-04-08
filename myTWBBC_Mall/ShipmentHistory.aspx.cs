using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using TWBBC_Mall.Controllers;
using TWBBC_Mall.Models;

/// <summary>
/// 出貨明細表-匯出記錄
/// </summary>
public partial class myTWBBC_Mall_ShipmentHistory : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1232", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                #region --Request參數--

                //[取得/檢查參數] - Req_sDate
                if (!string.IsNullOrWhiteSpace(Req_sDate))
                {
                    this.filter_sDate.Text = Req_sDate;
                }
                //[取得/檢查參數] - Req_eDate
                if (!string.IsNullOrWhiteSpace(Req_eDate))
                {
                    this.filter_eDate.Text = Req_eDate;
                }

                //[取得/檢查參數] - Req_Cust
                if (!string.IsNullOrWhiteSpace(Req_Cust))
                {
                    this.filter_Cust.Text = Req_Cust;
                }

                #endregion


                //Get Data
                LookupDataList(Req_PageIdx);

            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --

    /// <summary>
    /// 取得資料
    /// </summary>
    /// <param name="pageIndex"></param>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:分頁參數 -----
        int RecordsPerPage = 20;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數,for pager

        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Date
        if (!string.IsNullOrWhiteSpace(Req_sDate))
        {
            search.Add("sDate", Req_sDate.ToDateString("yyyy/MM/dd 00:00"));

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        if (!string.IsNullOrWhiteSpace(Req_eDate))
        {
            search.Add("eDate", Req_eDate.ToDateString("yyyy/MM/dd 23:59"));

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }

        //[取得/檢查參數] - Cust
        if (!string.IsNullOrWhiteSpace(Req_Cust))
        {
            search.Add("CustID", Req_Cust);

            PageParam.Add("Cust=" + Server.UrlEncode(Req_Cust));
        }

        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetShipmentHistory(search, out ErrMsg);


        //----- 資料整理:取得總筆數 -----
        TotalRow = query.Count();

        //----- 資料整理:頁數判斷 -----
        if (pageIndex > ((TotalRow / RecordsPerPage) + ((TotalRow % RecordsPerPage) > 0 ? 1 : 0)) && TotalRow > 0)
        {
            StartRow = 0;
            pageIndex = 1;
        }

        //----- 資料整理:選取每頁顯示筆數 -----
        var data = query.Skip(StartRow).Take(RecordsPerPage);

        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = data;
        this.lvDataList.DataBind();


        //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
        if (query.Count() == 0)
        {
            this.ph_EmptyData.Visible = true;
            this.ph_Data.Visible = false;

        }
        else
        {
            this.ph_EmptyData.Visible = false;
            this.ph_Data.Visible = true;

            //分頁
            string getPager = CustomExtension.Pagination(TotalRow, RecordsPerPage, pageIndex, 5
                , thisPage, PageParam, false, true);

            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = getPager;

        }

    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        //取得Key值
        string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
        //檔案命名用欄位
        string Get_SeqNo = ((HiddenField)e.Item.FindControl("hf_SeqNo")).Value;
        string Get_CustID = ((HiddenField)e.Item.FindControl("hf_CustID")).Value;

        //do export
        doExport(Get_CustID, Get_SeqNo, Get_DataID);
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料
                string CustID = (string)DataBinder.Eval(dataItem.DataItem, "CustID");
                ((PlaceHolder)e.Item.FindControl("ph_ShowCust")).Visible = !string.IsNullOrWhiteSpace(CustID);
                ((PlaceHolder)e.Item.FindControl("ph_NoCust")).Visible = string.IsNullOrWhiteSpace(CustID);

            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// [按鈕] - 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        //執行查詢
        Response.Redirect(filterUrl(), false);
    }


    /// <summary>
    /// [按鈕] - 匯出
    /// </summary>
    private void doExport(string custid, string seqNo, string id)
    {
        {
            //----- 宣告:資料參數 -----
            TWBBCMallRepository _data = new TWBBCMallRepository();
            Dictionary<string, string> search = new Dictionary<string, string>();
            DataTable DT = new DataTable();

            //----- 原始資料:條件篩選 -----
            //固定條件
            search.Add("DataID", id);

            //----- 方法:取得資料 -----
            var query = _data.GetShipmentHistoryDetail(search, out ErrMsg)
                .Select(fld => new
                {
                    OrderID = fld.OrderID,
                    RowRank = fld.RowRank,
                    ModelNo = fld.ModelNo,
                    MallName = fld.MallName,
                    CustName = fld.CustName,
                    BuyCnt = fld.BuyCnt,
                    Erp_SO_ID = fld.Erp_SO_ID,
                    ShipNo = fld.ShipNo,
                    ShipWho = fld.ShipWho,
                    ShipAddr = fld.ShipAddr,
                    ShipTel = fld.ShipTel,
                    TotalPrice = fld.TotalPrice
                });

            if (query.Count() == 0)
            {
                CustomExtension.AlertMsg("查無資料,請重新確認條件.", "");
                return;
            }


            /* [開始匯出] */
            //將IQueryable轉成DataTable
            DataTable myDT = CustomExtension.LINQToDataTable(query);

            if (myDT.Rows.Count > 0)
            {
                //重新命名欄位標頭
                myDT.Columns["OrderID"].ColumnName = "商城單號";
                myDT.Columns["RowRank"].ColumnName = "序號";
                myDT.Columns["MallName"].ColumnName = "商城";
                myDT.Columns["CustName"].ColumnName = "客戶";
                myDT.Columns["ModelNo"].ColumnName = "品號";
                myDT.Columns["BuyCnt"].ColumnName = "數量";
                myDT.Columns["Erp_SO_ID"].ColumnName = "銷貨單號";
                myDT.Columns["ShipNo"].ColumnName = "物流單號";
                myDT.Columns["ShipWho"].ColumnName = "收貨人";
                myDT.Columns["ShipAddr"].ColumnName = "收貨地址";
                myDT.Columns["ShipTel"].ColumnName = "收貨電話";
                myDT.Columns["TotalPrice"].ColumnName = "總金額";
            }

            //release
            query = null;

            //匯出Excel
            CustomExtension.ExportExcel(
                myDT
                , "Dataoutput-{0}.xlsx".FormatThis(DateTime.Now.ToString().ToDateString("yyyyMMddhhmm"))
                , false);

        }
    }


    #endregion


    #region -- 附加功能 --

    /// <summary>
    /// 含查詢條件的完整網址(新查詢)
    /// </summary>
    /// <returns></returns>
    public string filterUrl()
    {
        //Params
        string _sDate = this.filter_sDate.Text;
        string _eDate = this.filter_eDate.Text;
        string _Cust = this.filter_Cust.Text;

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?page=1".FormatThis(thisPage));

        //[查詢條件] - Date
        if (!string.IsNullOrWhiteSpace(_sDate))
        {
            url.Append("&sDate=" + Server.UrlEncode(_sDate));
        }
        if (!string.IsNullOrWhiteSpace(_eDate))
        {
            url.Append("&eDate=" + Server.UrlEncode(_eDate));
        }

        //[查詢條件] - Cust
        if (!string.IsNullOrWhiteSpace(_Cust))
        {
            url.Append("&Cust=" + Server.UrlEncode(_Cust));
        }

        return url.ToString();
    }


    #endregion


    #region -- 網址參數 --


    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC_Mall/ShipmentHistory.aspx".FormatThis(
            fn_Params.WebUrl);
    }

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    public int Req_PageIdx
    {
        get
        {
            int data = Request.QueryString["Page"] == null ? 1 : Convert.ToInt32(Request.QueryString["Page"]);
            return data;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }
    private int _Req_PageIdx;


    /// <summary>
    /// 取得傳遞參數 - sDate
    /// 預設1日內
    /// </summary>
    public string Req_sDate
    {
        get
        {
            String _data = Request.QueryString["sDate"];
            string dt = DateTime.Now.AddDays(-1).ToString().ToDateString("yyyy/MM/dd");
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : dt;
        }
        set
        {
            this._Req_sDate = value;
        }
    }
    private string _Req_sDate;


    /// <summary>
    /// 取得傳遞參數 - eDate
    /// </summary>
    public string Req_eDate
    {
        get
        {
            String _data = Request.QueryString["eDate"];
            string dt = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : dt;
        }
        set
        {
            this._Req_eDate = value;
        }
    }
    private string _Req_eDate;

    /// <summary>
    /// 取得傳遞參數 - Cust
    /// </summary>
    public string Req_Cust
    {
        get
        {
            String _data = Request.QueryString["Cust"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "20", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Cust = value;
        }
    }
    private string _Req_Cust;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            return "{0}myTWBBC_Mall/ShipmentList.aspx".FormatThis(fn_Params.WebUrl);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    #endregion
}