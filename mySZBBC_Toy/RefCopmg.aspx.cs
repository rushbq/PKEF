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
using SZBBC_Toy.Controllers;
using SZBBC_Toy.Models;

public partial class mySZBBC_RefCopmg : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("920", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                #region --Request參數--

                //[取得/檢查參數] - Cust
                if (!string.IsNullOrWhiteSpace(Req_Cust))
                {
                    this.filter_Cust.Text = Req_Cust;
                    this.lb_Cust.Text = Server.UrlDecode(Req_CustName);
                    this.filter_CustName.Text = Server.UrlDecode(Req_CustName);
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
                if (Req_Search.Equals("Y"))
                {
                    LookupDataList(Req_PageIdx, 1);
                }

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
    private void LookupDataList(int pageIndex, int sort)
    {
        //----- 宣告:分頁參數 -----
        int RecordsPerPage = 20;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數,for pager

        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<
        PageParam.Add("s=Y");

        //[取得/檢查參數] - Cust
        if (!string.IsNullOrWhiteSpace(Req_Cust))
        {
            search.Add("Cust", Req_Cust);

            PageParam.Add("Cust=" + Server.UrlEncode(Req_Cust));
            PageParam.Add("cn=" + Server.UrlEncode(Req_CustName));
        }

        //[取得/檢查參數] - Mall
        if (!string.IsNullOrWhiteSpace(Req_Mall))
        {
            search.Add("Mall", Req_Mall);

            PageParam.Add("Mall=" + Server.UrlEncode(Req_Mall));
        }

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrWhiteSpace(Req_Keyword))
        {
            search.Add("Keyword", Req_Keyword);

            PageParam.Add("k=" + Server.UrlEncode(Req_Keyword));
        }
        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetRefModelList(search, sort, out ErrMsg);


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

        //Add section
        ph_Add.Visible = true;

        //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
        if (query.Count() == 0)
        {
            ph_EmptyData.Visible = true;
            ph_Data.Visible = false;
            ph_Pager.Visible = false;

        }
        else
        {
            ph_EmptyData.Visible = false;
            ph_Data.Visible = true;
            ph_Pager.Visible = true;

            //分頁
            string getPager = CustomExtension.Pagination(TotalRow, RecordsPerPage, pageIndex, 5
                , thisPage, PageParam, false, true);

            //Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = getPager;

        }

    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        //取得Key值
        string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();

        //----- 方法:刪除資料 -----
        if (false == _data.Delete_RefModel(Get_DataID, out ErrMsg))
        {
            CustomExtension.AlertMsg("刪除失敗", "");
            return;
        }
        else
        {
            //導向本頁(帶參數)
            Response.Redirect(filterUrl());
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        //try
        //{
        //    if (e.Item.ItemType == ListViewItemType.DataItem)
        //    {
        //        ListViewDataItem dataItem = (ListViewDataItem)e.Item;

        //        //取得資料:單身數
        //        Int32 _Cnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ChildCnt"));


        //    }
        //}
        //catch (Exception)
        //{
        //    throw;
        //}
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
        SZBBCToyRepository _data = new SZBBCToyRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls);


        //----- 資料整理 -----
        ddl.Items.Clear();

        //if (!string.IsNullOrEmpty(rootName))
        //{
        //    ddl.Items.Add(new ListItem(rootName, ""));
        //}

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
        //Check
        string _Cust = this.filter_Cust.Text;
        if (string.IsNullOrWhiteSpace(_Cust))
        {
            CustomExtension.AlertMsg("請填入正確的客戶", "");
            return;
        }

        //執行查詢
        Response.Redirect(filterUrl(), false);
    }

    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            //Get input
            string _modelNo = val_ModelNo.Text;
            string _custModel = tb_CustModelNo.Text;
            string _custSpec = tb_CustSpec.Text;

            //Check input
            if (string.IsNullOrWhiteSpace(_modelNo))
            {
                CustomExtension.AlertMsg("請填入正確的品號", "");
                return;
            }
            if (string.IsNullOrWhiteSpace(_custModel))
            {
                CustomExtension.AlertMsg("請填入客戶品號", "");
                return;
            }

            //Add Data
            //----- 宣告:資料參數 -----
            SZBBCToyRepository _data = new SZBBCToyRepository();
            RefModel inst = new RefModel();

            inst.MallID = Convert.ToInt32(Req_Mall);
            inst.CustID = Req_Cust;
            inst.ModelNo = _modelNo;
            inst.CustModelNo = _custModel;
            inst.CustSpec = _custSpec;


            //----- 方法:新增資料 -----
            if (false == _data.Create_RefModel(inst, out ErrMsg))
            {
                CustomExtension.AlertMsg("新增失敗", "");
                return;
            }

            //ReBind
            LookupDataList(1, 2);
        }
        catch (Exception)
        {

            throw;
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
        string _Cust = this.filter_Cust.Text;
        string _CustName = this.filter_CustName.Text;
        string _Mall = this.filter_Mall.SelectedValue;
        string _Keyword = this.filter_Keyword.Text;

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?s=Y&page=1".FormatThis(thisPage));

        //[查詢條件] - Cust
        if (!string.IsNullOrWhiteSpace(_Cust))
        {
            url.Append("&Cust=" + Server.UrlEncode(_Cust));
            url.Append("&cn=" + Server.UrlEncode(_CustName));
        }

        //[查詢條件] - Mall
        if (!string.IsNullOrWhiteSpace(_Mall))
        {
            url.Append("&Mall=" + Server.UrlEncode(_Mall));
        }

        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(_Keyword))
        {
            url.Append("&k=" + Server.UrlEncode(_Keyword));
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
        return "{0}mySZBBC_Toy/".FormatThis(
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
    /// 取得傳遞參數 - doSearch
    /// </summary>
    public string Req_Search
    {
        get
        {
            String _data = Request.QueryString["s"];
            return string.IsNullOrWhiteSpace(_data) ? "N" : _data;
        }
        set
        {
            this._Req_Search = value;
        }
    }
    private string _Req_Search;


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
    /// 取得傳遞參數 - CustName
    /// </summary>
    public string Req_CustName
    {
        get
        {
            String _data = Request.QueryString["cn"];
            return string.IsNullOrWhiteSpace(_data) ? "" : _data;
        }
        set
        {
            this._Req_CustName = value;
        }
    }
    private string _Req_CustName;

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
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}RefCopmg.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;

    #endregion

}