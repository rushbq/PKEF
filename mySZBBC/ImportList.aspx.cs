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

public partial class mySZBBC_ImportList : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("820", out ErrMsg) == false)
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
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        search.Add((int)mySearch.DateType, "1");

        //[取得/檢查參數] - Date
        if (!string.IsNullOrWhiteSpace(Req_sDate))
        {
            search.Add((int)mySearch.StartDate, Req_sDate);

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        if (!string.IsNullOrWhiteSpace(Req_eDate))
        {
            search.Add((int)mySearch.EndDate, Req_eDate);

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrWhiteSpace(Req_Keyword))
        {
            search.Add((int)mySearch.Keyword, Req_Keyword);

            PageParam.Add("k=" + Server.UrlEncode(Req_Keyword));
        }

        //[取得/檢查參數] - Mall
        if (!string.IsNullOrWhiteSpace(Req_Mall))
        {
            search.Add((int)mySearch.Mall, Req_Mall);

            PageParam.Add("Mall=" + Server.UrlEncode(Req_Mall));
        }
        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search);


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
            CustomExtension.setCookie("EF_SZBBC", "", -1);
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
                , (PageParam.Count == 0 ? "" : "&") + string.Join("&", PageParam.ToArray()));

            //暫存頁面Url, 給其他頁使用
            CustomExtension.setCookie("EF_SZBBC", Server.UrlEncode(reSetPage), 1);


        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料:狀態/Log記錄筆數
                string Get_Status = DataBinder.Eval(dataItem.DataItem, "Status").ToString();
                int Get_LogCnt = Convert.ToInt16(DataBinder.Eval(dataItem.DataItem, "LogCnt"));

                //取得控制項
                PlaceHolder ph_KeepGo = (PlaceHolder)e.Item.FindControl("ph_KeepGo");
                Literal lt_LogCnt = (Literal)e.Item.FindControl("lt_LogCnt");

                //判斷狀態
                switch (Get_Status)
                {
                    case "10":
                    case "11":
                    case "12":
                        ph_KeepGo.Visible = true;

                        break;

                    default:
                        ph_KeepGo.Visible = false;

                        break;
                }

                //判斷是否有錯誤記錄
                lt_LogCnt.Visible = (!Get_Status.Equals("13") && Get_LogCnt > 0);

            }
        }
        catch (Exception)
        {

            throw;
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
        SZBBCRepository _data = new SZBBCRepository();

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


    public string GetStatusColor(string value)
    {
        switch (value)
        {
            case "10":
            case "11":
            case "12":
                return "orange";

            default:
                return "grey";
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


    /// <summary>
    /// 取得繼續匯入網址
    /// </summary>
    /// <param name="status"></param>
    /// <param name="dataID"></param>
    /// <returns></returns>
    public string keepGoUrl(int status, string dataID)
    {
        switch (status)
        {
            case 12:
                return "{0}mySZBBC/ImportStep4.aspx?dataID={1}".FormatThis(fn_Params.WebUrl, dataID);

            default:
                return "{0}mySZBBC/ImportStep2.aspx?dataID={1}".FormatThis(fn_Params.WebUrl, dataID);
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
        return "{0}mySZBBC/ImportList.aspx".FormatThis(
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
    /// 預設15 days內
    /// </summary>
    public string Req_sDate
    {
        get
        {
            String _data = Request.QueryString["sDate"];
            string dt = DateTime.Now.AddDays(-15).ToShortDateString().ToDateString("yyyy/MM/dd");
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