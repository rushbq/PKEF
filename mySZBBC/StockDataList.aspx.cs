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
using SZ_BBC.Controllers;
using SZ_BBC.Models;

public partial class mySZBBC_StockDataList : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("851", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                #region --Request參數--

                //[取得/檢查參數] - setDate
                if (!string.IsNullOrWhiteSpace(Req_setDate))
                {
                    this.filter_setDate.Text = Req_setDate;
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrWhiteSpace(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
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
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Date
        if (!string.IsNullOrWhiteSpace(Req_setDate))
        {
            search.Add("setDate", Req_setDate.ToDateString("yyyy/MM/dd"));

            PageParam.Add("setDate=" + Server.UrlEncode(Req_setDate));
        }

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrWhiteSpace(Req_Keyword))
        {
            search.Add("Keyword", Req_Keyword);

            PageParam.Add("k=" + Server.UrlEncode(Req_Keyword));
        }
        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetStockData(search, out ErrMsg);


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
            //CustomExtension.setCookie("EF_StockData", "", -1);
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
            //CustomExtension.setCookie("EF_StockData", Server.UrlEncode(reSetPage), 1);


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
    protected void lbtn_Excel_Click(object sender, EventArgs e)
    {
        //Params
        string _setDate = this.filter_setDate.Text;
        string _keyword = filter_Keyword.Text.Trim();

        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        DataTable DT = new DataTable();

        //----- 原始資料:條件篩選 -----
        #region >> 條件篩選 <<

        //[查詢條件] - Date
        if (!string.IsNullOrWhiteSpace(_setDate))
        {
            search.Add("setDate", _setDate.ToDateString("yyyy/MM/dd"));
        }
        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(_keyword))
        {
            search.Add("Keyword", _keyword);
        }
        #endregion

        //----- 方法:取得資料 -----
        var query = _data.GetStockData(search, out ErrMsg)
            .Select(fld => new {
                ModelNo = fld.ModelNo,
                EC1_SKU = fld.EC1_SKU,
                EC1_StockNum = fld.EC1_StockNum,
                EC2_SKU = fld.EC2_SKU,
                EC2_StockNum = fld.EC2_StockNum,
                EC3_SKU = fld.EC3_SKU,
                EC3_StockNum = fld.EC3_StockNum,
                EC4_SKU = fld.EC4_SKU,
                EC4_StockNum = fld.EC4_StockNum,
                StockQty_B01 = fld.StockQty_B01,
                PreSell_B01 = fld.PreSell_B01,
                PreIN_B01 = fld.PreIN_B01
            });

        if (query.Count() == 0)
        {
            CustomExtension.AlertMsg("查無資料,請重新確認條件.", "");
            return;
        }
         

        //將IQueryable轉成DataTable
        DataTable myDT = CustomExtension.LINQToDataTable(query);

        if (myDT.Rows.Count > 0)
        {
            //重新命名欄位標頭
            myDT.Columns["ModelNo"].ColumnName = "品號";
            myDT.Columns["EC1_SKU"].ColumnName = "京東POP-SKU";
            myDT.Columns["EC1_StockNum"].ColumnName = "京東POP-庫存";
            myDT.Columns["EC2_SKU"].ColumnName = "天貓-SKU";
            myDT.Columns["EC2_StockNum"].ColumnName = "天貓-庫存";
            myDT.Columns["EC3_SKU"].ColumnName = "唯品會-SKU";
            myDT.Columns["EC3_StockNum"].ColumnName = "唯品會-庫存";
            myDT.Columns["EC4_SKU"].ColumnName = "京東廠送-SKU";
            myDT.Columns["EC4_StockNum"].ColumnName = "京東廠送-庫存";
            myDT.Columns["StockQty_B01"].ColumnName = "上海倉-庫存";
            myDT.Columns["PreSell_B01"].ColumnName = "預計銷";
            myDT.Columns["PreIN_B01"].ColumnName = "預計進";
          
        }

        //release
        query = null;

        //匯出Excel
        CustomExtension.ExportExcel(
            myDT
            , "Dataoutput-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
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
        string _setDate = filter_setDate.Text;
        string _keyword = filter_Keyword.Text.Trim();

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?page=1".FormatThis(thisPage));

        //[查詢條件] - Date
        if (!string.IsNullOrWhiteSpace(_setDate))
        {
            url.Append("&setDate=" + Server.UrlEncode(_setDate));
        }

        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(_keyword))
        {
            url.Append("&k=" + Server.UrlEncode(_keyword));
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
        return "{0}mySZBBC/StockDataList.aspx".FormatThis(
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
    /// 取得傳遞參數 - setDate
    /// 預設今日
    /// </summary>
    public string Req_setDate
    {
        get
        {
            String _data = Request.QueryString["setDate"];
            string dt = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : dt;
        }
        set
        {
            this._Req_setDate = value;
        }
    }
    private string _Req_setDate;



    /// <summary>
    /// 取得傳遞參數 - keyword
    /// </summary>
    public string Req_Keyword
    {
        get
        {
            String _data = Request.QueryString["k"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "30", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    private string _Req_Keyword;


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