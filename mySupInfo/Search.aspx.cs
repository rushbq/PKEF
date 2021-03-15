using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;

public partial class mySupInfo_Search : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 檢查必要參數 **
                if (string.IsNullOrEmpty(Req_ds))
                {
                    Response.Redirect(Application["WebUrl"] + "Default.aspx");
                    return;
                }

                //[權限判斷] Start
                /* 
                 * 使用公司別代號，判斷對應的MENU ID
                 */
                bool isPass = false;
                switch (Req_ds)
                {
                    case "3":
                        //上海寶工
                        isPass = fn_CheckAuth.CheckAuth_User("446", out ErrMsg);
                        break;


                    case "2":
                        //深圳寶工
                        isPass = fn_CheckAuth.CheckAuth_User("447", out ErrMsg);
                        break;

                    default:
                        isPass = fn_CheckAuth.CheckAuth_User("445", out ErrMsg);
                        break;
                }

                if (!isPass)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"], HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //[權限判斷] End


                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
                }


                //取得公司別
                string _corpName = GetCorpName(Req_ds);
                this.lt_CorpName.Text = _corpName;
                Page.Title += " | " + _corpName;


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
    /// 取得公司別參數資料
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private String GetCorpName(string id)
    {
        //----- 宣告:資料參數 -----
        ParamsRepository _data = new ParamsRepository();

        Dictionary<int, string> search = new Dictionary<int, string>();
        search.Add((int)Common.mySearch.DataID, id);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetCorpList(search).FirstOrDefault();

        return query.Corp_Name;

    }


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
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得參數] - 公司別
        search.Add((int)Common.mySearch.Corp, Req_ds);
        PageParam.Add("ds=" + Server.UrlEncode(Req_ds));

        //[取得參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)Common.mySearch.Keyword, Req_Keyword);

            PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
        }

        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetERPDataList(search);


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
            Session.Remove("BackListUrl");
        }
        else
        {
            string getPager = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false
                , false, CustomExtension.myStyle.Goole);

            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = getPager;

            Literal lt_TopPager = (Literal)this.lvDataList.FindControl("lt_TopPager");
            lt_TopPager.Text = getPager;

            //重新整理頁面Url
            string thisPage = "{0}?Page={1}{2}".FormatThis(
                PageUrl
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));


            //暫存頁面Url, 給其他頁使用
            Session["BackListUrl"] = thisPage;
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                ////取得資料
                //string Get_IsSend = DataBinder.Eval(dataItem.DataItem, "IsSend").ToString();

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


        url.Append("{0}?ds={1}&Page=1".FormatThis(PageUrl, Req_ds));


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
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
    /// 取得傳遞參數 - ds, 公司別UID
    /// </summary>
    public string Req_ds
    {
        get
        {
            String data = Request.QueryString["ds"];
            return (CustomExtension.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_ds = value;
        }
    }
    private string _Req_ds;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}mySupInfo/Search.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    #endregion

}