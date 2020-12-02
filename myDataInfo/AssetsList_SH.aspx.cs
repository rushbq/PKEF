using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Data.Models;
using CustomController;
using ExtensionUI;
using System.Data;

public partial class myDataInfo_AssetsList_SH : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("541", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                //Get Dept (init)
                Get_DeptList(this.filter_Dept, true);


                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
                }

                //[取得/檢查參數] - DeptID
                if (!string.IsNullOrEmpty(Req_DeptID))
                {
                    this.filter_Dept.SelectedIndex = this.filter_Dept.Items.IndexOf(this.filter_Dept.Items.FindByValue(Req_DeptID));
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
        int RecordsPerPage = 10;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數

        //----- 宣告:資料參數 -----
        EquipmentRepository _data = new EquipmentRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)Common.mySearch.Keyword, Req_Keyword);

            PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
        }

        //[取得參數] - DeptID
        if (!string.IsNullOrEmpty(Req_DeptID))
        {
            search.Add((int)Common.mySearch.DeptID, Req_DeptID);

            PageParam.Add("DeptID=" + Server.UrlEncode(Req_DeptID));
        }

        //[取得參數] - sDate
        if (!string.IsNullOrEmpty(Req_sDate))
        {
            search.Add((int)Common.mySearch.StartDate, Req_sDate);

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        //[取得參數] - eDate
        if (!string.IsNullOrEmpty(Req_eDate))
        {
            search.Add((int)Common.mySearch.EndDate, Req_eDate);

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }
        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList_SH(search);


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


    /// <summary>
    /// 取得部門列表
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="showRoot"></param>
    private void Get_DeptList(DropDownListGP menu, bool showRoot)
    {
        //----- 宣告:資料參數 -----
        DeptsRepository _data = new DeptsRepository();
        menu.Items.Clear();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDepts(null, Common.DeptArea.SH);

        //index 0
        if (showRoot)
        {
            menu.Items.Add(new ListItem("- 所有資料 -", ""));
        }

        //Item list
        foreach (var item in query)
        {
            //判斷GP_Rank, 若為第一項, 則輸出群組名稱
            if (item.GP_Rank.Equals(1))
            {
                menu.AddItemGroup(item.GroupName);
            }

            //Item Name  
            menu.Items.Add(new ListItem("{0} ({1})".FormatThis(item.DeptName, item.DeptID), item.DeptID));
        }

        //Release
        query = null;
    }


    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 關鍵字快查
    /// </summary>
    protected void btn_KeySearch_Click(object sender, EventArgs e)
    {
        doSearch(this.filter_Keyword.Text);
    }


    /// <summary>
    /// 執行查詢
    /// </summary>
    /// <param name="keyword"></param>
    private void doSearch(string keyword)
    {
        StringBuilder url = new StringBuilder();

        url.Append("{0}?Page=1".FormatThis(PageUrl));


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
        }

        //[查詢條件] - Dept
        if (this.filter_Dept.SelectedIndex > 0)
        {
            url.Append("&DeptID=" + Server.UrlEncode(this.filter_Dept.SelectedValue));
        }

        //[查詢條件] - sDate
        if (!string.IsNullOrEmpty(this.filter_sDate.Text))
        {
            url.Append("&sDate=" + Server.UrlEncode(this.filter_sDate.Text));
        }
        //[查詢條件] - eDate
        if (!string.IsNullOrEmpty(this.filter_eDate.Text))
        {
            url.Append("&eDate=" + Server.UrlEncode(this.filter_eDate.Text));
        }

        //執行轉頁
        Response.Redirect(url.ToString(), false);
    }


    /// <summary>
    /// 重置條件
    /// </summary>
    protected void lbtn_Reset_Click(object sender, EventArgs e)
    {
        Response.Redirect(PageUrl);
    }


    /// <summary>
    /// 匯出Excel
    /// </summary>
    protected void lbtn_Export_Click(object sender, EventArgs e)
    {

        //----- 宣告:資料參數 -----
        EquipmentRepository _data = new EquipmentRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)Common.mySearch.Keyword, Req_Keyword);
        }

        //[取得參數] - DeptID
        if (!string.IsNullOrEmpty(Req_DeptID))
        {
            search.Add((int)Common.mySearch.DeptID, Req_DeptID);
        }

        //[取得參數] - sDate
        if (!string.IsNullOrEmpty(Req_sDate))
        {
            search.Add((int)Common.mySearch.StartDate, Req_sDate);
        }
        //[取得參數] - eDate
        if (!string.IsNullOrEmpty(Req_eDate))
        {
            search.Add((int)Common.mySearch.EndDate, Req_eDate);
        }
        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList_SH(search);

        //將IQueryable轉成DataTable
        DataTable myDT = fn_CustomUI.LINQToDataTable(query);

        //重新命名欄位標頭
        myDT.Columns["ID"].ColumnName = "資產編號";
        myDT.Columns["Name"].ColumnName = "資產名稱";
        myDT.Columns["Spec"].ColumnName = "資產規格";
        myDT.Columns["Who"].ColumnName = "保管人";
        myDT.Columns["GetItemDate"].ColumnName = "取得日期";
        myDT.Columns["GetItemMoney"].ColumnName = "取得金額";

        //release
        query = null;

        //匯出Excel
        fn_CustomUI.ExportExcel(
            myDT
            , "資產設備明細-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
            , false);
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
            return (CustomExtension.String_資料長度Byte(data, "1", "10", out ErrMsg)) ? data.Trim() : "";
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
            return (CustomExtension.String_資料長度Byte(data, "1", "50", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_eDate = value;
        }
    }
    private string _Req_eDate;


    /// <summary>
    /// 取得傳遞參數 - DeptID
    /// </summary>
    public string Req_DeptID
    {
        get
        {
            String data = Request.QueryString["DeptID"];
            return (CustomExtension.String_資料長度Byte(data, "1", "5", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_DeptID = value;
        }
    }
    private string _Req_DeptID;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}myDataInfo/AssetsList_SH.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    #endregion

}