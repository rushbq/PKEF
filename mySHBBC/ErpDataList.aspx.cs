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
using SH_BBC.Controllers;
using SH_BBC.Models;

public partial class mySHBBC_ErpDataList : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("866", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                #region --Request參數--

                //[取得/檢查參數] - sDate
                if (!string.IsNullOrWhiteSpace(Req_sDate))
                {
                    filter_sDate.Text = Req_sDate;
                }
                //[取得/檢查參數] - eDate
                if (!string.IsNullOrWhiteSpace(Req_eDate))
                {
                    filter_eDate.Text = Req_eDate;
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrWhiteSpace(Req_Keyword))
                {
                    filter_Keyword.Text = Req_Keyword;
                }

                //Get Class
                Get_ClassList(myClass.mall, filter_Mall, "所有商城", Req_Mall);

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
        SHBBCRepository _data = new SHBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Date
        if (!string.IsNullOrWhiteSpace(Req_sDate))
        {
            search.Add("sDate", Req_sDate.ToDateString("yyyy/MM/dd"));

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        if (!string.IsNullOrWhiteSpace(Req_eDate))
        {
            search.Add("eDate", Req_eDate.ToDateString("yyyy/MM/dd"));

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrWhiteSpace(Req_Keyword))
        {
            search.Add("Keyword", Req_Keyword);

            PageParam.Add("k=" + Server.UrlEncode(Req_Keyword));
        }

        //[取得/檢查參數] - Mall
        if (!string.IsNullOrWhiteSpace(Req_Mall))
        {
            search.Add("Mall", Req_Mall);

            PageParam.Add("Mall=" + Server.UrlEncode(Req_Mall));
        }
        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetERPDataList(search, out ErrMsg);


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

            //Clear
            //CustomExtension.setCookie("EF_Shipment", "", -1);
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

            //重新整理頁面Url
            string reSetPage = "{0}?Page={1}{2}".FormatThis(
                thisPage
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));

            //暫存頁面Url, 給其他頁使用
            //CustomExtension.setCookie("EF_Shipment", Server.UrlEncode(reSetPage), 1);


        }

    }


    /// <summary>
    /// 取得類別
    /// </summary>
    /// <param name="cls"></param>
    /// <param name="ddl"></param>
    /// <param name="rootName"></param>
    /// <param name="inputValue"></param>
    private void Get_ClassList(myClass cls, DropDownList ddl, string rootName, string inputValue)
    {
        //----- 宣告:資料參數 -----
        SHBBCRepository _data = new SHBBCRepository();

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

        //被選擇值
        if (!string.IsNullOrWhiteSpace(inputValue))
        {
            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(inputValue));
        }

        query = null;
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
    protected void lbtn_Excel_Click(object sender, EventArgs e)
    {
        //Params
        string _sDate = this.filter_sDate.Text;
        string _eDate = this.filter_eDate.Text;
        string _Keyword = this.filter_Keyword.Text;
        string _Mall = this.filter_Mall.SelectedValue;

        //----- 宣告:資料參數 -----
        SHBBCRepository _data = new SHBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----
        #region >> 條件篩選 <<

        //[查詢條件] - Date
        if (!string.IsNullOrWhiteSpace(_sDate))
        {
            search.Add("sDate", _sDate.ToDateString("yyyy/MM/dd"));
        }
        if (!string.IsNullOrWhiteSpace(_eDate))
        {
            search.Add("eDate", _eDate.ToDateString("yyyy/MM/dd"));
        }

        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(_Keyword))
        {
            search.Add("Keyword", _Keyword);
        }

        //[查詢條件] - Mall
        if (!string.IsNullOrWhiteSpace(_Mall))
        {
            search.Add("Mall", _Mall);
        }

        #endregion

        //----- 方法:取得資料 -----
        var query = _data.GetERPDataList(search, out ErrMsg);

        if (query.Count() == 0)
        {
            CustomExtension.AlertMsg("查無資料,請重新確認條件.", "");
            return;
        }


        /* [開始匯出] */
        var _newDT = query
            .Select(fld => new
            {
                OrderID = fld.OrderID,
                ShipNo = fld.ShipmentNo,
                ShipTel = fld.ShipTel,
                MallName = fld.MallName,
                Erp_PO_ID = "{0}-{1}".FormatThis(fld.TC001, fld.TC002),
                Erp_SO_ID = "{0}-{1}".FormatThis(fld.TH001, fld.TH002),
                Erp_AR_ID = "{0}-{1}".FormatThis(fld.TA001, fld.TA002),
                TotalPrice = fld.TotalPrice
            });

        //將IQueryable轉成DataTable
        DataTable myDT = CustomExtension.LINQToDataTable(_newDT);

        if (myDT.Rows.Count > 0)
        {
            //重新命名欄位標頭
            myDT.Columns["OrderID"].ColumnName = "平台單號";
            myDT.Columns["ShipNo"].ColumnName = "物流單號";
            myDT.Columns["ShipTel"].ColumnName = "電話";
            myDT.Columns["MallName"].ColumnName = "商城";
            myDT.Columns["Erp_PO_ID"].ColumnName = "訂單單號";
            myDT.Columns["Erp_SO_ID"].ColumnName = "銷貨單號";
            myDT.Columns["Erp_AR_ID"].ColumnName = "結帳單號";
            myDT.Columns["TotalPrice"].ColumnName = "金額";

        }

        //release
        query = null;

        //匯出Excel
        CustomExtension.ExportExcel(
            myDT
            , "DataOutput-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
            , false);

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
        string _Keyword = this.filter_Keyword.Text;
        string _Mall = this.filter_Mall.SelectedValue;

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

        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(_Keyword))
        {
            url.Append("&k=" + Server.UrlEncode(_Keyword));
        }

        //[查詢條件] - Mall
        if (!string.IsNullOrWhiteSpace(_Mall))
        {
            url.Append("&Mall=" + Server.UrlEncode(_Mall));
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
        return "{0}mySHBBC/ErpDataList.aspx".FormatThis(
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
            string dt = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
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
    /// 取得傳遞參數 - Keyword
    /// </summary>
    public string Req_Keyword
    {
        get
        {
            String _data = Request.QueryString["k"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "20", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    private string _Req_Keyword;

    /// <summary>
    /// 取得傳遞參數 - Mall
    /// </summary>
    public string Req_Mall
    {
        get
        {
            String _data = Request.QueryString["Mall"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "3", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Mall = value;
        }
    }
    private string _Req_Mall;


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

    #endregion
}