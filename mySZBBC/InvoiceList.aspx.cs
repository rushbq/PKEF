using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomController;
using ExtensionUI;
using Invoice.SH.Controllers;
using PKLib_Method.Methods;

public partial class InvoiceList : SecurityIn
{
    public string ErrMsg = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("840", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
                }

                //[取得/檢查參數] - Status
                if (!string.IsNullOrEmpty(Req_Status))
                {
                    this.filter_Status.SelectedIndex = this.filter_Status.Items.IndexOf(this.filter_Status.Items.FindByValue(Req_Status));
                }


                //[取得/檢查參數] - sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    this.filter_sDate.Text = Req_sDate;
                }

                //[取得/檢查參數] - eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    this.filter_eDate.Text = Req_eDate;
                }

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
        ArrayList PageParam = new ArrayList();  //條件參數

        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)mySearch.Keyword, Req_Keyword);

            PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
        }

        //[取得參數] - Status
        if (!string.IsNullOrEmpty(Req_Status))
        {
            search.Add((int)mySearch.Status, Req_Status);

            PageParam.Add("Status=" + Server.UrlEncode(Req_Status));
        }

        //[取得參數] - sDate
        if (!string.IsNullOrEmpty(Req_sDate))
        {
            search.Add((int)mySearch.StartDate, Req_sDate);

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        //[取得參數] - eDate
        if (!string.IsNullOrEmpty(Req_eDate))
        {
            search.Add((int)mySearch.EndDate, Req_eDate);

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }
        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search, out ErrMsg);


        //----- 資料整理:取得總筆數 -----
        TotalRow = query.Count();

        //----- 資料整理:頁數判斷 -----
        if (pageIndex > TotalRow && TotalRow > 0)
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
            //Session.Remove("BackListUrl");
        }
        else
        {
            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false
                , false, CustomExtension.myStyle.Goole);


            ////重新整理頁面Url
            //string thisPage = "{0}?Page={1}{2}".FormatThis(
            //    PageUrl
            //    , pageIndex
            //    , "&" + string.Join("&", PageParam.ToArray()));


            ////暫存頁面Url, 給其他頁使用
            //Session["BackListUrl"] = thisPage;
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料:狀態
                string Get_Status = DataBinder.Eval(dataItem.DataItem, "InvStatus").ToString();
                string Get_SerialNo = DataBinder.Eval(dataItem.DataItem, "SerialNo").ToString();
                string Get_InvTitle = DataBinder.Eval(dataItem.DataItem, "InvTitle").ToString();

                //取得控制項
                string editWord = Get_Status.Equals("3") ? "檢視" :
                    string.IsNullOrEmpty(Get_InvTitle) ? "編輯" : Get_InvTitle;

                Literal lt_Edit = (Literal)e.Item.FindControl("lt_Edit");
                lt_Edit.Text = "<a href=\"#!\" id=\"btn-OpenInvoice-{0}\" class=\"btn-OpenInvoice\" data-id=\"{0}\">{1}</a>".FormatThis(Get_SerialNo, editWord);

                PlaceHolder ph_InvSave = (PlaceHolder)e.Item.FindControl("ph_InvSave");
                ph_InvSave.Visible = !Get_Status.Equals("3");

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 關鍵字快查
    /// </summary>
    protected void btn_KeySearch_Click(object sender, EventArgs e)
    {
        doSearch();
    }


    /// <summary>
    /// 執行查詢
    /// </summary>
    /// <param name="keyword"></param>
    private void doSearch()
    {
        StringBuilder url = new StringBuilder();
        string keyword = this.filter_Keyword.Text;
        string sDate = this.filter_sDate.Text;
        string eDate = this.filter_eDate.Text;
        string status = this.filter_Status.SelectedValue;


        url.Append("{0}?Page=1".FormatThis(PageUrl));


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
        }

        //[查詢條件] - status
        url.Append("&status=" + Server.UrlEncode(status));

        //[查詢條件] - sDate
        if (!string.IsNullOrEmpty(sDate))
        {
            url.Append("&sDate=" + Server.UrlEncode(sDate));
        }
        //[查詢條件] - eDate
        if (!string.IsNullOrEmpty(eDate))
        {
            url.Append("&eDate=" + Server.UrlEncode(eDate));
        }

        //執行轉頁
        Response.Redirect(url.ToString(), false);
    }



    #endregion


    #region -- 參數設定 --
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
    /// 取得傳遞參數 - Keyword
    /// </summary>
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["Keyword"];
            return (CustomExtension.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? Keyword.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    private string _Req_Keyword;


    /// <summary>
    /// 取得傳遞參數 - sDate
    /// </summary>
    public string Req_sDate
    {
        get
        {
            String data = Request.QueryString["sDate"];
            return string.IsNullOrEmpty(data) ? DateTime.Today.ToString().ToDateString("yyyyMMdd") : data;
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
            String data = Request.QueryString["eDate"];
            return string.IsNullOrEmpty(data) ? DateTime.Today.ToString().ToDateString("yyyyMMdd") : data;
        }
        set
        {
            this._Req_eDate = value;
        }
    }
    private string _Req_eDate;


    /// <summary>
    /// 取得傳遞參數 - Status
    /// </summary>
    public string Req_Status
    {
        get
        {
            String data = Request.QueryString["Status"];
            return (CustomExtension.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? data.Trim() : "1";
        }
        set
        {
            this._Req_Status = value;
        }
    }
    private string _Req_Status;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}mySZBBC/InvoiceList.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    #endregion

}