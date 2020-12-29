using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;
using LogRecord;

public partial class Cust_Search : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷] - 客戶基本資料
                if (fn_CheckAuth.CheckAuth_User("430", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[取得/檢查參數] - 地區別
                if (fn_Extensions.Menu_ERP_Param(this.ddl_Area, Request.QueryString["ReqArea"], "3", true, out ErrMsg) == false)
                {
                    this.ddl_Area.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 國家別
                if (fn_Extensions.Menu_ERP_Param(this.ddl_Country, Request.QueryString["ReqCountry"], "4", true, out ErrMsg) == false)
                {
                    this.ddl_Country.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - Keyword
                String Keyword = Request.QueryString["Keyword"];
                if (fn_Extensions.String_字數(Keyword, "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = Keyword.Trim();
                }

                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //[帶出資料]
                LookupDataList(page);

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="page">頁數</param>
    private void LookupDataList(int page)
    {
        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        string ErrMsg;
        try
        {
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "CustInfo/Cust_Search.aspx?t=" + Param_Type;

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 10;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string Keyword = fn_stringFormat.Filter_Html(this.tb_Keyword.Text.Trim());
            string Req_Area = fn_stringFormat.Filter_Html(this.ddl_Area.SelectedValue);   //地區別
            string Req_Country = fn_stringFormat.Filter_Html(this.ddl_Country.SelectedValue);   //國家別

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      RTRIM(Base.MA001) CustID, RTRIM(Base.MA003) CustName");
            SBSql.AppendLine("      , RTRIM(myArea.MR003) AreaName, RTRIM(myCountry.MR003) CountryName");
            SBSql.AppendLine("      , Prof.Account_Name RepSalesID, Prof.Display_Name RepSales");
            SBSql.AppendLine("      , Corp.Corp_Name");
            SBSql.AppendLine("      , Base.*");
            SBSql.AppendLine("      , (SELECT SWID FROM Customer_Data WHERE (Cust_ERPID = Base.MA001)) AS mySWID");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.MA001 ASC) AS RowRank");
            SBSql.AppendLine("    FROM Customer Base ");
            SBSql.AppendLine("      INNER JOIN Param_Corp Corp ON UPPER(Base.DBC) = UPPER(Corp.Corp_ID)");
            SBSql.AppendLine("      LEFT JOIN [prokit2].dbo.CMSMR myArea ON myArea.MR001 = 3 AND myArea.MR002 = Base.MA018 COLLATE Chinese_Taiwan_Stroke_BIN");
            SBSql.AppendLine("      LEFT JOIN [prokit2].dbo.CMSMR myCountry ON myCountry.MR001 = 4 AND myCountry.MR002 = Base.MA019 COLLATE Chinese_Taiwan_Stroke_BIN");
            SBSql.AppendLine("      LEFT JOIN User_Profile Prof ON Prof.ERP_UserID = Base.MA016");
            SBSql.AppendLine("    WHERE (Base.DBC = Base.DBS) ");

            #region "判斷權限"

            switch (Param_Type)
            {
                case "1":
                    //TW
                    SBSql.Append(" AND (Corp.Corp_UID IN (1,4,5,6))");

                    break;


                case "3":
                    //SH
                    SBSql.Append(" AND (Corp.Corp_UID IN (3))");

                    break;


                default:
                    SBSql.Append(" AND (Corp.Corp_UID IN (2))");

                    break;
            }

            #endregion


            #region "查詢條件"

            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.MA001 LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.MA002 LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.MA003 LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }
            //[查詢條件] - Area
            if (false == string.IsNullOrEmpty(Req_Area))
            {
                SBSql.Append(" AND (Base.MA018 = @ReqArea)");

                cmd.Parameters.AddWithValue("ReqArea", Req_Area);

                this.ViewState["Page_LinkStr"] += "&ReqArea=" + Server.UrlEncode(Req_Area);
            }
            //[查詢條件] - Country
            if (false == string.IsNullOrEmpty(Req_Country))
            {
                SBSql.Append(" AND (Base.MA019 = @ReqCountry)");

                cmd.Parameters.AddWithValue("ReqCountry", Req_Country);

                this.ViewState["Page_LinkStr"] += "&ReqCountry=" + Server.UrlEncode(Req_Country);
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            /*------------------------------ 我是分隔線 ------------------------------*/

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine("    FROM Customer Base ");
            SBSql.AppendLine("      INNER JOIN Param_Corp Corp ON UPPER(Base.DBC) = UPPER(Corp.Corp_ID)");
            SBSql.AppendLine("      LEFT JOIN [prokit2].dbo.CMSMR myArea ON myArea.MR001 = 3 AND myArea.MR002 = Base.MA018 COLLATE Chinese_Taiwan_Stroke_BIN");
            SBSql.AppendLine("      LEFT JOIN [prokit2].dbo.CMSMR myCountry ON myCountry.MR001 = 4 AND myCountry.MR002 = Base.MA019 COLLATE Chinese_Taiwan_Stroke_BIN");
            SBSql.AppendLine("      LEFT JOIN User_Profile Prof ON UPPER(Prof.ERP_UserID) = UPPER(Base.MA016)");
            SBSql.AppendLine("    WHERE (Base.DBC = Base.DBS) ");

            #region "判斷權限"

            switch (Param_Type)
            {
                case "1":
                    //TW
                    SBSql.Append(" AND (Corp.Corp_UID IN (1,4,5,6))");

                    break;


                case "3":
                    //SH
                    SBSql.Append(" AND (Corp.Corp_UID IN (3))");

                    break;


                default:
                    SBSql.Append(" AND (Corp.Corp_UID IN (2))");

                    break;
            }

            #endregion


            #region "查詢條件"

            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.MA001 LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.MA002 LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.MA003 LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }
            //[查詢條件] - Area
            if (false == string.IsNullOrEmpty(Req_Area))
            {
                SBSql.Append(" AND (Base.MA018 = @ReqArea)");

                cmdTotalCnt.Parameters.AddWithValue("ReqArea", Req_Area);
            }
            //[查詢條件] - Country
            if (false == string.IsNullOrEmpty(Req_Country))
            {
                SBSql.Append(" AND (Base.MA019 = @ReqCountry)");

                cmdTotalCnt.Parameters.AddWithValue("ReqCountry", Req_Country);
            }

            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, dbConn.DBS.PKSYS, out TotalCnt, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    //判斷是否為頁碼過大, 帶往最後一頁
                    if (TotalCnt > 0 & BgItem > TotalCnt)
                    {
                        if (TotalCnt % PageSize == 0)
                        {
                            TotalPage = Convert.ToInt16(TotalCnt / PageSize);
                        }
                        else
                        {
                            TotalPage = Convert.ToInt16(Math.Floor((double)TotalCnt / PageSize)) + 1;
                        }
                        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + TotalPage);
                    }
                    else
                    {
                        //隱藏分頁
                        this.pl_Page.Visible = false;
                    }
                }
                else
                {
                    #region "分頁控制"
                    //計算總頁數
                    if (TotalCnt % PageSize == 0)
                    {
                        TotalPage = Convert.ToInt16(TotalCnt / PageSize);
                    }
                    else
                    {
                        TotalPage = Convert.ToInt16(Math.Floor((double)TotalCnt / PageSize)) + 1;
                    }
                    //判斷頁數
                    if (page < 1)
                        page = 1;
                    if (page > TotalPage)
                        page = TotalPage;
                    //一次n頁的頁碼
                    int PageTen = 0;
                    if (page % PageRoll == 0)
                        PageTen = page;
                    else
                        PageTen = (Convert.ToInt16(Math.Floor((double)page / PageRoll)) + 1) * PageRoll;
                    //帶入頁數資料
                    int FirstItem = (page - 1) * PageSize + 1;
                    int LastItem = FirstItem + (PageSize - 1);
                    if (LastItem > TotalCnt)
                        LastItem = TotalCnt;
                    //填入頁數資料
                    int i = 0;
                    for (i = 1; i <= TotalPage; i++)
                    {
                        this.ddl_Page_List.Items.Insert(i - 1, Convert.ToString(i));
                        this.ddl_Page_List.Items[i - 1].Value = Convert.ToString(i);
                    }
                    this.ddl_Page_List.SelectedValue = Convert.ToString(page); //頁碼下拉選單
                    this.lt_TotalPage.Text = Convert.ToString(TotalPage);  // n 頁
                    this.lt_Page_DataCntInfo.Text = "第 " + FirstItem + " - " + LastItem + " 筆，共 " + TotalCnt + " 筆";

                    //[分頁] - 顯示頁碼
                    StringBuilder sb = new StringBuilder();

                    //[頁碼] - 第一頁
                    if (page >= 2)
                    {
                        sb.AppendFormat("<a href=\"{0}&page=1\" class=\"PagePre\">第一頁</a>", this.ViewState["Page_LinkStr"]);
                    }
                    //[頁碼] - 數字頁碼
                    sb.AppendLine("<div class=\"Pages\">");

                    for (i = PageTen - (PageRoll - 1); i <= PageTen; i++)
                    {
                        if (i > TotalPage)
                            break;
                        if (i == page)
                        {
                            sb.AppendFormat("<span>{0}</span>", i);
                        }
                        else
                        {
                            sb.AppendFormat("<a href=\"{0}&page={1}\">{1}</a>", this.ViewState["Page_LinkStr"], i);
                        }
                    }
                    sb.AppendLine("</div>");

                    //[頁碼] - 最後1頁
                    if (page < TotalPage)
                    {
                        sb.AppendFormat("<a href=\"{0}&page={1}\" class=\"PageNext\">{2}</a>", this.ViewState["Page_LinkStr"], TotalPage, "最後一頁");
                    }
                    //顯示分頁
                    this.pl_Page.Visible = true;
                    this.lt_Page_Link.Text = sb.ToString();

                    #endregion
                }

                //暫存目前頁碼 
                this.ViewState["page"] = page;
                //暫存目前Url
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"] + "&page=" + page;

                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 取得資料列表！", "");
        }
        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得負責業務
                string salesID = DataBinder.Eval(dataItem.DataItem, "RepSalesID").ToString();
                //取得目前登入工號
                string userID = fn_Params.UserAccount;

                PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                ph_Edit.Visible = false;

                #region "判斷權限"

                switch (Param_Type)
                {
                    case "1":
                        //TW判斷編輯權限
                        if (editAll_TW)
                        {
                            ph_Edit.Visible = true;
                        }
                        else if (editSelf_TW)
                        {
                            if (salesID.Equals(userID))
                            {
                                ph_Edit.Visible = true;
                            }
                        }
                        break;

                    case "3":
                        //SH
                        if (editAll_SH)
                        {
                            ph_Edit.Visible = true;
                        }
                        else if (editSelf_SH)
                        {
                            if (salesID.Equals(userID))
                            {
                                ph_Edit.Visible = true;
                            }
                        }
                        break;

                    default:
                        //SZ判斷編輯權限
                        if (editAll_SZ)
                        {
                            ph_Edit.Visible = true;
                        }
                        else if (editSelf_SZ)
                        {
                            if (salesID.Equals(userID))
                            {
                                ph_Edit.Visible = true;
                            }
                        }

                        break;
                }

                #endregion

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }
    #endregion

    #region -- 前端頁面控制 --
    //分頁跳轉
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    //搜尋
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Cust_Search.aspx?t=" + Param_Type);

            //[查詢條件] - 關鍵字
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Filter_Html(this.tb_Keyword.Text)));
            }
            //[查詢條件] - Area
            if (this.ddl_Area.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqArea=" + Server.UrlEncode(this.ddl_Area.SelectedValue));
            }
            //[查詢條件] - Country
            if (this.ddl_Country.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqCountry=" + Server.UrlEncode(this.ddl_Country.SelectedValue));
            }
            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    /// <summary>
    /// 產生Tab (依權限判斷)
    /// </summary>
    /// <returns></returns>
    public string Html_Tabs()
    {
        StringBuilder html = new StringBuilder();

        html.Append("<ul>");

        //台灣寶工
        if (Tab_TW)
        {
            html.Append("<li class=\"{0}\"><a href=\"Cust_Search.aspx?t=1\">台灣寶工</a></li>".FormatThis(
                    (Param_Type.Equals("1")) ? "TabAc" : ""
                ));
        }

        //上海寶工
        if (Tab_SH)
        {
            html.Append("<li class=\"{0}\"><a href=\"Cust_Search.aspx?t=3\">上海寶工</a></li>".FormatThis(
                    (Param_Type.Equals("3")) ? "TabAc" : ""
                ));
        }

        //深圳寶工
        //if (Tab_SZ)
        //{
        //    html.Append("<li class=\"{0}\"><a href=\"Cust_Search.aspx?t=2\">深圳寶工</a></li>".FormatThis(
        //            (Param_Type.Equals("2")) ? "TabAc" : ""
        //        ));
        //}

        html.Append("</ul>");

        return html.ToString();
    }
    #endregion

    #region -- 參數設定 --

    private bool _editAll_TW;
    public bool editAll_TW
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("434", out ErrMsg);
        }
        set
        {
            this._editAll_TW = value;
        }
    }

    private bool _editSelf_TW;
    public bool editSelf_TW
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("433", out ErrMsg);
        }
        set
        {
            this._editSelf_TW = value;
        }
    }


    private bool _editAll_SH;
    public bool editAll_SH
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("454", out ErrMsg);
        }
        set
        {
            this._editAll_SH = value;
        }
    }

    private bool _editSelf_SH;
    public bool editSelf_SH
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("453", out ErrMsg);
        }
        set
        {
            this._editSelf_SH = value;
        }
    }


    private bool _editAll_SZ;
    public bool editAll_SZ
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("438", out ErrMsg);
        }
        set
        {
            this._editAll_SZ = value;
        }
    }

    private bool _editSelf_SZ;
    public bool editSelf_SZ
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("437", out ErrMsg);
        }
        set
        {
            this._editSelf_SZ = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - Tab
    /// </summary>
    private string _Param_Type;
    public string Param_Type
    {
        get
        {
            string _id = Request.QueryString["t"];
            string _checkID = _id;

            //若為空值,依權限帶預設值
            if (string.IsNullOrWhiteSpace(_id) || _id.Equals("0"))
            {
                if (Tab_TW)
                {
                    _checkID = "1";
                }
                else if (Tab_SZ)
                {
                    _checkID = "2";
                }
                else if (Tab_SH)
                {
                    _checkID = "3";
                }
            }
            else
            {
                //判斷來源type是否有對應的權限
                switch (_id)
                {
                    case "2":
                        _checkID = (Tab_SZ) ? _id : "0";
                        break;


                    case "3":
                        _checkID = (Tab_SH) ? _id : "0";
                        break;

                    default:
                        _checkID = (Tab_TW) ? _id : "0";
                        break;

                }
            }

            return _checkID;
            
        }
        set
        {
            this._Param_Type = value;
        }
    }

    /// <summary>
    /// 取得頁籤權限 - TW
    /// </summary>
    private bool _Tab_TW;
    public bool Tab_TW
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("432", out ErrMsg);
        }
        set
        {
            this._Tab_TW = value;
        }
    }


    /// <summary>
    /// 取得頁籤權限 - 深圳
    /// </summary>
    private bool _Tab_SZ;
    public bool Tab_SZ
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("436", out ErrMsg);
        }
        set
        {
            this._Tab_SZ = value;
        }
    }


    /// <summary>
    /// 取得頁籤權限 - 上海
    /// </summary>
    private bool _Tab_SH;
    public bool Tab_SH
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("452", out ErrMsg);
        }
        set
        {
            this._Tab_SH = value;
        }
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    protected string DesKey
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
        }
        set
        {
            this._DesKey = value;
        }
    }

    /// <summary>
    /// 取得Session ID
    /// </summary>
    private string _mySID;
    protected string mySID
    {
        get
        {
            return Session.SessionID;
        }
        set
        {
            this._mySID = value;
        }
    }
    #endregion

}