using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_Invoice.Aisino.Controllers;

public partial class mySZInvoice_List : SecurityIn
{
    public string ErrMsg = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1010", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"]
                        , HttpUtility.UrlEncode(ErrMsg))
                        , true);
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
                    this.filter_Status.SelectedValue = Req_Status;
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
        SZ_InvoiceRepository _data = new SZ_InvoiceRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----

        //固定條件
        search.Add((int)mySearch.DataType, "1");

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
            search.Add((int)mySearch.Type, Req_Status);

            PageParam.Add("st=" + Server.UrlEncode(Req_Status));
        }

        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search);


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
            //Clear
            CustomExtension.setCookie("EF_SZInvoice", "", -1);
        }
        else
        {
            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false
                , false, CustomExtension.myStyle.Goole);

            ////重新整理頁面Url
            //string reSetPage = "{0}?Page={1}{2}".FormatThis(
            //    PageUrl
            //    , pageIndex
            //    , "&" + string.Join("&", PageParam.ToArray()));

            ////暫存頁面Url, 給其他頁使用
            //CustomExtension.setCookie("EF_SZInvoice", Server.UrlEncode(reSetPage), 1);
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料:中繼檔是否已回寫發票號
                int Get_Rel = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "IsRel"));
                //取得資料:是否已寫入中繼檔
                string Get_IsInsert = DataBinder.Eval(dataItem.DataItem, "IsInsert").ToString();
                //取得資料:是否已執行回寫ERP
                string Get_IsUpdate = DataBinder.Eval(dataItem.DataItem, "IsUpdate").ToString();

                bool showBtn_Edit = false;
                bool showBtn_Update = false;
                string stName = "";

                /*
                   判斷順序: IsInsert -> IsUpdate -> IsRel
                 */
                if (Get_IsInsert.Equals("N"))
                {
                    showBtn_Edit = true;
                    stName = "未完成轉入";
                }
                else
                {
                    if (Get_IsUpdate.Equals("N"))
                    {
                        if (Get_Rel == 0)
                        {
                            stName = "待產生發票";
                        }
                        else
                        {
                            stName = "發票已產生,請按下「更新ERP」";
                            showBtn_Update = true;
                        }
                    }
                    else
                    {
                        stName = "已完成";
                    }
                }


                //顯示狀態
                Literal lt_InvStatus = (Literal)e.Item.FindControl("lt_InvStatus");
                lt_InvStatus.Text = stName;

                //ERP更新鈕
                PlaceHolder ph_Update = (PlaceHolder)e.Item.FindControl("ph_Update");
                ph_Update.Visible = showBtn_Update;

                //編輯
                PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                ph_Edit.Visible = showBtn_Edit;

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //取得Key值
                string Get_ID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;


                //----- 刪除 -----
                SZ_InvoiceRepository _data = new SZ_InvoiceRepository();

                if (!_data.Delete(Get_ID))
                {
                    CustomExtension.AlertMsg("刪除失敗!", "");
                    return;
                }

                Response.Redirect(PageUrl + "?page=" + Req_PageIdx);

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - ItemCommand！");
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

        url.Append("{0}?Page=1".FormatThis(PageUrl));


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
        }

        //[查詢條件] - 狀態
        if (this.filter_Status.SelectedIndex > 0)
        {
            url.Append("&st=" + Server.UrlEncode(this.filter_Status.SelectedValue));
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
    /// 取得傳遞參數 - St
    /// </summary>
    public string Req_Status
    {
        get
        {
            String data = Request.QueryString["st"];
            return (CustomExtension.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? data.Trim() : "";
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
            return "{0}mySZInvoice/List.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    #endregion
}