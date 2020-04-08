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

public partial class Msg_Search : SecurityIn
{
    //回覆權限
    public bool ReplyAuth = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 網站訊息
                if (fn_CheckAuth.CheckAuth_User("610", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[取得/檢查參數] - 訊息類別
                if (fn_Extensions.Menu_InqClass(this.ddl_Req_Class, Request.QueryString["ReqClass"], true, out ErrMsg) == false)
                {
                    this.ddl_Req_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 處理狀態
                if (fn_Extensions.Menu_InqStatus(this.ddl_Status, Request.QueryString["ReqStatus"], true, out ErrMsg) == false)
                {
                    this.ddl_Status.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 區域
                if (fn_Extensions.Menu_Area(this.ddl_Area, Request.QueryString["ReqArea"], true, out ErrMsg) == false)
                {
                    this.ddl_Area.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - StartDate
                String StartDate = Request.QueryString["StartDate"];
                if (fn_Extensions.String_字數(StartDate, "1", "10", out ErrMsg) && StartDate.IsDate())
                {
                    this.tb_StartDate.Text = fn_stringFormat.Filter_Html(StartDate.ToString().Trim());
                }
                //[取得/檢查參數] - EndDate
                String EndDate = Request.QueryString["EndDate"];
                if (fn_Extensions.String_字數(EndDate, "1", "10", out ErrMsg) && StartDate.IsDate())
                {
                    this.tb_EndDate.Text = fn_stringFormat.Filter_Html(EndDate.ToString().Trim());
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
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "myMarket/Msg_Search.aspx?t=" + Param_Type;

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string StartDate = fn_stringFormat.Filter_Html(this.tb_StartDate.Text);
            string EndDate = fn_stringFormat.Filter_Html(this.tb_EndDate.Text);
            string Keyword = fn_stringFormat.Filter_Html(this.tb_Keyword.Text.Trim()).Replace("-", "");
            string Req_Class = fn_stringFormat.Filter_Html(this.ddl_Req_Class.SelectedValue);   //問題類別
            string Req_Status = fn_stringFormat.Filter_Html(this.ddl_Status.SelectedValue);   //處理狀態
            string Req_Area = fn_stringFormat.Filter_Html(this.ddl_Area.SelectedValue);   //Area

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Cls.Class_Name, Base.InquiryID, LEFT(Base.Message, 30) AS ClientMsg, Base.Status, St.Class_Name AS StName, Base.TraceID ");
            SBSql.AppendLine("      , Base.Create_Time ");
            //回覆人、時間
            SBSql.AppendLine("      , (SELECT Account_Name + ' <br>(' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid IN ( ");
            SBSql.AppendLine("          SELECT TOP 1 Reply.Create_Who FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC");
            SBSql.AppendLine("       )");
            SBSql.AppendLine("      )) AS Reply_Name");
            SBSql.AppendLine("      , (SELECT TOP 1 Reply.Create_Time FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS Reply_Time");
            SBSql.AppendLine("      , (SELECT TOP 1 AreaName FROM PKSYS.dbo.Param_Area WHERE (Param_Area.AreaCode = Base.AreaCode) AND (UPPER(LangCode) = 'ZH-TW')) AS AreaName ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY St.Sort ASC, Base.Create_Time DESC) AS RowRank ");
            SBSql.AppendLine("    FROM Inquiry Base ");
            SBSql.AppendLine("      INNER JOIN Inquiry_Class Cls ON Base.Class_ID = Cls.Class_ID AND Cls.LangCode = 'zh-TW' ");
            SBSql.AppendLine("      INNER JOIN Inquiry_Status St ON Base.Status = St.Class_ID ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));

                this.ViewState["Page_LinkStr"] += "&StartDate=" + Server.UrlEncode(StartDate);
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));

                this.ViewState["Page_LinkStr"] += "&EndDate=" + Server.UrlEncode(EndDate);
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.Message LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.TraceID LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }
            //[查詢條件] - 類別
            if (false == string.IsNullOrEmpty(Req_Class))
            {
                SBSql.Append(" AND (Base.Class_ID = @ReqClass)");

                cmd.Parameters.AddWithValue("ReqClass", Req_Class);

                this.ViewState["Page_LinkStr"] += "&ReqClass=" + Server.UrlEncode(Req_Class);
            }
            //[查詢條件] - 狀態
            if (false == string.IsNullOrEmpty(Req_Status))
            {
                SBSql.Append(" AND (Base.Status = @ReqStatus)");

                cmd.Parameters.AddWithValue("ReqStatus", Req_Status);

                this.ViewState["Page_LinkStr"] += "&ReqStatus=" + Server.UrlEncode(Req_Status);
            }
            //[查詢條件] - Area
            if (false == string.IsNullOrEmpty(Req_Area))
            {
                SBSql.Append(" AND (Base.AreaCode = @ReqArea)");

                cmd.Parameters.AddWithValue("ReqArea", Req_Area);

                this.ViewState["Page_LinkStr"] += "&ReqArea=" + Server.UrlEncode(Req_Area);
            }

            //判斷Tab
            switch (Param_Type)
            {
                case "1":
                    SBSql.Append(" AND (Base.Status <> 4)");
                    break;

                default:
                    SBSql.Append(" AND (Base.Status = 4)");
                    break;
            }
            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Inquiry Base ");
            SBSql.AppendLine("   INNER JOIN Inquiry_Class Cls ON Base.Class_ID = Cls.Class_ID AND Cls.LangCode = 'zh-TW' ");
            SBSql.AppendLine("   INNER JOIN Inquiry_Status St ON Base.Status = St.Class_ID ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmdTotalCnt.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmdTotalCnt.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.Message LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.TraceID LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }
            //[查詢條件] - 類別
            if (false == string.IsNullOrEmpty(Req_Class))
            {
                SBSql.Append(" AND (Base.Class_ID = @ReqClass)");

                cmdTotalCnt.Parameters.AddWithValue("ReqClass", Req_Class);
            }
            //[查詢條件] - 狀態
            if (false == string.IsNullOrEmpty(Req_Status))
            {
                SBSql.Append(" AND (Base.Status = @ReqStatus)");

                cmdTotalCnt.Parameters.AddWithValue("ReqStatus", Req_Status);
            }
            //[查詢條件] - Area
            if (false == string.IsNullOrEmpty(Req_Area))
            {
                SBSql.Append(" AND (Base.AreaCode = @ReqArea)");

                cmdTotalCnt.Parameters.AddWithValue("ReqArea", Req_Area);
            }

            //判斷Tab
            switch (Param_Type)
            {
                case "1":
                    SBSql.Append(" AND (Base.Status <> 4)");
                    break;

                default:
                    SBSql.Append(" AND (Base.Status = 4)");
                    break;
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, dbConn.DBS.PKWeb, out TotalCnt, out ErrMsg))
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

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - ItemCommand！", "");
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                #region >> 狀態 <<
                //取得狀態
                string helpStatus = DataBinder.Eval(dataItem.DataItem, "Status").ToString();
                bool showEdit, showView;

                //判斷狀態, 給予不同的顏色
                Label lb_Status = (Label)e.Item.FindControl("lb_Status");
                switch (helpStatus)
                {
                    case "1":
                        lb_Status.CssClass = "label label-info";
                        showEdit = true;
                        showView = false;

                        break;

                    case "2":
                        lb_Status.CssClass = "label label-warning";
                        showEdit = true;
                        showView = false;

                        break;

                    case "3":
                        lb_Status.CssClass = "label label-success";
                        showEdit = true;
                        showView = true;

                        break;

                    case "4":
                        lb_Status.CssClass = "label label-default";
                        showEdit = false;
                        showView = true;

                        break;

                    case "5":
                        lb_Status.CssClass = "label label-default";
                        showEdit = false;
                        showView = true;

                        break;

                    default:
                        lb_Status.CssClass = "label label-success";
                        showEdit = false;
                        showView = true;

                        break;
                }

                //設定狀態
                lb_Status.Text = DataBinder.Eval(dataItem.DataItem, "StName").ToString();

                PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                PlaceHolder ph_View = (PlaceHolder)e.Item.FindControl("ph_View");
                ph_Edit.Visible = showEdit;
                ph_View.Visible = showView;
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
            SBUrl.Append("Msg_Search.aspx?t=" + Param_Type);

            //[查詢條件] - 開始日期
            if (string.IsNullOrEmpty(this.tb_StartDate.Text) == false)
            {
                SBUrl.Append("&StartDate=" + Server.UrlEncode(this.tb_StartDate.Text));
            }
            //[查詢條件] - 結束日期
            if (string.IsNullOrEmpty(this.tb_EndDate.Text) == false)
            {
                SBUrl.Append("&EndDate=" + Server.UrlEncode(this.tb_EndDate.Text));
            }
            //[查詢條件] - 關鍵字
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Filter_Html(this.tb_Keyword.Text)));
            }
            //[查詢條件] - 問題類別
            if (this.ddl_Req_Class.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqClass=" + Server.UrlEncode(this.ddl_Req_Class.SelectedValue));
            }
            //[查詢條件] - 處理狀態
            if (this.ddl_Status.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqStatus=" + Server.UrlEncode(this.ddl_Status.SelectedValue));
            }
            //[查詢條件] - Area
            if (this.ddl_Area.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqArea=" + Server.UrlEncode(this.ddl_Area.SelectedValue));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - Tab
    /// </summary>
    private string _Param_Type;
    public string Param_Type
    {
        get
        {
            String DataID = Request.QueryString["t"];

            //判斷處理狀態, 若為 4-垃圾訊息, 則自動將type轉為2
            if (this.ddl_Status.SelectedIndex > 0)
            {
                if (this.ddl_Status.SelectedValue.Equals("4"))
                {
                    DataID = "2";
                }
                else
                {
                    DataID = "1";
                }
            }

            return string.IsNullOrEmpty(DataID) ? "1" : DataID.ToString();
        }
        set
        {
            this._Param_Type = value;
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
    #endregion

}