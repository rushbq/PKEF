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
using ExtensionUI;

public partial class IT_HelpSearch : SecurityIn
{
    //回覆權限
    public bool ReplyAuth = false;
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;
               
                //[權限判斷] - 資訊需求登記
                if (fn_CheckAuth.CheckAuth_User("520", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //取得回覆權限
                ReplyAuth = fn_CheckAuth.CheckAuth_User("521", out ErrMsg);
                this.btn_Export.Visible = ReplyAuth;

                //[取得/檢查參數] - 需求部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, Request.QueryString["Dept"], true, false, new List<string> { "TW", "SH", "SZ" }, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 問題類型
                if (fn_Extensions.Menu_ITClass(this.ddl_Req_Class, Request.QueryString["ReqClass"], true, new List<string> { "登記問題類型" }, out ErrMsg) == false)
                {
                    this.ddl_Req_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 處理狀態
                if (fn_Extensions.Menu_ITClass(this.ddl_Help_Status, Request.QueryString["HelpStatus"], true, new List<string> { "處理狀態" }, out ErrMsg) == false)
                {
                    this.ddl_Help_Status.Items.Insert(0, new ListItem("選單產生失敗", ""));
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
            fn_Extensions.JsAlert("系統發生錯誤！", "");
            return;
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
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "Recording/IT_HelpSearch.aspx?func=sh";

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
            string Dept = fn_stringFormat.Filter_Html(this.ddl_Dept.SelectedValue);     //需求部門
            string Req_Class = fn_stringFormat.Filter_Html(this.ddl_Req_Class.SelectedValue);   //問題類別
            string Help_Status = fn_stringFormat.Filter_Html(this.ddl_Help_Status.SelectedValue);   //處理狀態

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Main.TraceID, Main.Help_Subject, Main.Create_Time, Main.Reply_Date, Main.onTop, ISNULL(LEN(Reply_Content), 0) Reply_Cnt, Main.Help_Status, Main.Req_Who ");
            SBSql.AppendLine("      , ReqClass.Class_Name AS HClass, HelpStatus.Class_Name AS HStatus ");
            SBSql.AppendLine("      , Prof.Account_Name, Prof.Display_Name, Dept.DeptName ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Main.onTop DESC, HelpStatus.Sort ASC, Main.Create_Time DESC) AS RowRank ");
            SBSql.AppendLine("    FROM IT_Help Main ");
            SBSql.AppendLine("      INNER JOIN IT_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID ");
            SBSql.AppendLine("      INNER JOIN IT_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Profile Prof ON Main.Req_Who = Prof.Account_Name ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Dept Dept ON Main.Req_Dept = Dept.DeptID ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Main.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));

                this.ViewState["Page_LinkStr"] += "&StartDate=" + Server.UrlEncode(StartDate);
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Main.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));

                this.ViewState["Page_LinkStr"] += "&EndDate=" + Server.UrlEncode(EndDate);
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Main.TraceID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Main.Help_Subject LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Main.Req_Who LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }
            //[查詢條件] - 需求部門
            if (false == string.IsNullOrEmpty(Dept))
            {
                SBSql.Append(" AND (Main.Req_Dept = @Dept)");

                cmd.Parameters.AddWithValue("Dept", Dept);

                this.ViewState["Page_LinkStr"] += "&Dept=" + Server.UrlEncode(Dept);
            }
            //[查詢條件] - 問題類別
            if (false == string.IsNullOrEmpty(Req_Class))
            {
                SBSql.Append(" AND (Main.Req_Class = @ReqClass)");

                cmd.Parameters.AddWithValue("ReqClass", Req_Class);

                this.ViewState["Page_LinkStr"] += "&ReqClass=" + Server.UrlEncode(Req_Class);
            }
            //[查詢條件] - 處理狀態
            if (false == string.IsNullOrEmpty(Help_Status))
            {
                SBSql.Append(" AND (Main.Help_Status = @HelpStatus)");

                cmd.Parameters.AddWithValue("HelpStatus", Help_Status);

                this.ViewState["Page_LinkStr"] += "&HelpStatus=" + Server.UrlEncode(Help_Status);
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
            SBSql.AppendLine(" FROM IT_Help Main ");
            SBSql.AppendLine("  INNER JOIN IT_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID ");
            SBSql.AppendLine("  INNER JOIN IT_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Create_Time >= @StartDate) ");
                cmdTotalCnt.Parameters.AddWithValue("StartDate", StartDate);
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Create_Time <= @EndDate) ");
                cmdTotalCnt.Parameters.AddWithValue("EndDate", EndDate);
            }
            //[查詢條件] - 關鍵字(TraceID, 主旨, 需求者工號)
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Main.TraceID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Main.Help_Subject LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Main.Req_Who LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }
            //[查詢條件] - 需求部門
            if (false == string.IsNullOrEmpty(Dept))
            {
                SBSql.Append(" AND (Main.Req_Dept = @Dept)");

                cmdTotalCnt.Parameters.AddWithValue("Dept", Dept);
            }
            //[查詢條件] - 問題類別
            if (false == string.IsNullOrEmpty(Req_Class))
            {
                SBSql.Append(" AND (Main.Req_Class = @ReqClass)");

                cmdTotalCnt.Parameters.AddWithValue("ReqClass", Req_Class);
            }
            //[查詢條件] - 處理狀態
            if (false == string.IsNullOrEmpty(Help_Status))
            {
                SBSql.Append(" AND (Main.Help_Status = @HelpStatus)");

                cmdTotalCnt.Parameters.AddWithValue("HelpStatus", Help_Status);
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalCnt, out ErrMsg))
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
                string reSetPage = this.ViewState["Page_LinkStr"] + "&page=" + page;

                //暫存頁面Url, 給其他頁使用
                PKLib_Method.Methods.CustomExtension.setCookie("EF_ITHelp", Server.UrlEncode(reSetPage), 1);

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
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;
                    StringBuilder SBSql = new StringBuilder();

                    //取得Key值
                    string Get_ID = ((HiddenField)e.Item.FindControl("hf_TraceID")).Value;

                    //[SQL] - 取得已上傳的檔案
                    SBSql.AppendLine(" SELECT AttachFile");
                    SBSql.AppendLine(" FROM IT_Help_Attach WHERE (TraceID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", Get_ID);
                    //[暫存參數] - 檔案名稱
                    List<string> ListFiles = new List<string>();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //暫存檔案名稱
                            ListFiles.Add(DT.Rows[row]["AttachFile"].ToString());
                        }
                    }


                    //[SQL] - 刪除資料
                    SBSql.Clear();
                    SBSql.AppendLine(" DELETE FROM IT_Help_Attach WHERE (TraceID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM IT_Help_CC WHERE (TraceID = @Param_ID) ");
                    SBSql.AppendLine(" DELETE FROM IT_Help WHERE (TraceID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", Get_ID);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料刪除失敗！", "");
                    }
                    else
                    {
                        //刪除檔案
                        for (int idx = 0; idx < ListFiles.Count; idx++)
                        {
                            IOManage.DelFile(Param_FileFolder, ListFiles[idx]);
                        }

                        fn_Extensions.JsAlert("資料刪除成功！", this.ViewState["Page_LinkStr"] + "&page=" + this.ViewState["page"]);
                    }
                }
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

                //onTop
                string getTop = DataBinder.Eval(dataItem.DataItem, "onTop").ToString();
                if (getTop.Equals("Y"))
                {
                    Literal lt_onTop = (Literal)e.Item.FindControl("lt_onTop");
                    lt_onTop.Text = "<span class=\"glyphicon glyphicon-pushpin\"></span>";
                }


                #region >> 狀態 <<
                //取得狀態
                string helpStatus = DataBinder.Eval(dataItem.DataItem, "Help_Status").ToString();
                int Reply_Cnt = Convert.ToInt16(DataBinder.Eval(dataItem.DataItem, "Reply_Cnt"));

                //判斷狀態, 給予不同的顏色
                Label lb_Status = (Label)e.Item.FindControl("lb_Status");
                switch (helpStatus)
                {
                    case "9":
                        lb_Status.CssClass = "label label-info";
                        break;

                    case "10":
                        lb_Status.CssClass = "label label-warning";
                        break;

                    default:
                        lb_Status.CssClass = "label label-success";
                        break;
                }

                //設定狀態
                if (Reply_Cnt > 0 && helpStatus.Equals("10"))
                {
                    lb_Status.Text = "{0} ,{1}".FormatThis(DataBinder.Eval(dataItem.DataItem, "HStatus").ToString(), "已回覆");
                }
                else
                {
                    lb_Status.Text = DataBinder.Eval(dataItem.DataItem, "HStatus").ToString();
                }

                #endregion


                #region >> 權限 <<
                //編輯按鈕
                Literal lt_Edit = (Literal)e.Item.FindControl("lt_Edit");
                if (ReplyAuth)
                {
                    lt_Edit.Text = "回覆";
                }
                else
                {
                    lt_Edit.Text = "編輯";
                }

                //刪除按鈕
                LinkButton lbtn_Delete = (LinkButton)e.Item.FindControl("lbtn_Delete");
                PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                PlaceHolder ph_None = (PlaceHolder)e.Item.FindControl("ph_None");
                //判斷可刪除狀態, 待處理&是自己的單 ; 有權限的人員
                if (ReplyAuth)
                //if ((helpStatus.Equals("9") && DataBinder.Eval(dataItem.DataItem, "Req_Who").Equals(fn_Params.UserAccount))
                //    || ReplyAuth)
                {
                    lbtn_Delete.Visible = true;
                    ph_Edit.Visible = true;
                    ph_None.Visible = false;
                }
                else
                {
                    lbtn_Delete.Visible = false;
                    ph_Edit.Visible = false;
                    ph_None.Visible = true;
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
            SBUrl.Append("IT_HelpSearch.aspx?func=sh");

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
            //[查詢條件] - 需求部門
            if (this.ddl_Dept.SelectedIndex > 0)
            {
                SBUrl.Append("&Dept=" + Server.UrlEncode(this.ddl_Dept.SelectedValue));
            }
            //[查詢條件] - 問題類別
            if (this.ddl_Req_Class.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqClass=" + Server.UrlEncode(this.ddl_Req_Class.SelectedValue));
            }
            //[查詢條件] - 處理狀態
            if (this.ddl_Help_Status.SelectedIndex > 0)
            {
                SBUrl.Append("&HelpStatus=" + Server.UrlEncode(this.ddl_Help_Status.SelectedValue));
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
    /// 匯出
    /// </summary>
    protected void btn_Export_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[取得參數]
                string StartDate = fn_stringFormat.Filter_Html(this.tb_StartDate.Text);
                string EndDate = fn_stringFormat.Filter_Html(this.tb_EndDate.Text);
                string Keyword = fn_stringFormat.Filter_Html(this.tb_Keyword.Text.Trim()).Replace("-", "");
                string Dept = fn_stringFormat.Filter_Html(this.ddl_Dept.SelectedValue);     //需求部門
                string Req_Class = fn_stringFormat.Filter_Html(this.ddl_Req_Class.SelectedValue);   //問題類別
                string Help_Status = fn_stringFormat.Filter_Html(this.ddl_Help_Status.SelectedValue);   //處理狀態

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.Append(" SELECT");
                SBSql.Append("  YEAR(Main.Create_Time) AS '年' , MONTH(Main.Create_Time) AS '月', DAY(Main.Create_Time) AS '日'");
                SBSql.Append("  , Main.Help_Subject AS '主旨', ReqClass.Class_Name AS '問題類別', HelpStatus.Class_Name AS '處理狀態'");
                SBSql.Append("  , Main.Help_Content AS '需求說明', ISNULL(Main.Reply_Hours, 0) AS '處理工時' ");
                SBSql.Append("  , (Prof.Account_Name + ' (' + Prof.Display_Name + ')') AS '需求者'");
                SBSql.Append("  , (RepProf.Account_Name + ' (' + RepProf.Display_Name + ')') AS '回覆者'");
                SBSql.Append("  , Main.TraceID AS '追蹤編號'");
                SBSql.Append(" FROM IT_Help Main");
                SBSql.Append("  INNER JOIN IT_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID");
                SBSql.Append("  INNER JOIN IT_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID");
                SBSql.Append("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Main.Req_Who = Prof.Account_Name");
                SBSql.Append("  LEFT JOIN PKSYS.dbo.User_Profile RepProf ON Main.Update_Who = RepProf.Account_Name");
                SBSql.Append(" WHERE (1 = 1) ");

                #region "查詢條件"
                //[查詢條件] - 開始日期
                if (false == string.IsNullOrEmpty(StartDate))
                {
                    SBSql.Append(" AND (Main.Create_Time >= @StartDate) ");
                    cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
                }
                //[查詢條件] - 結束日期
                if (false == string.IsNullOrEmpty(EndDate))
                {
                    SBSql.Append(" AND (Main.Create_Time <= @EndDate) ");
                    cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
                }
                //[查詢條件] - 關鍵字
                if (false == string.IsNullOrEmpty(Keyword))
                {
                    SBSql.Append(" AND ( ");
                    SBSql.Append("      (Main.TraceID LIKE '%' + @Keyword + '%') ");
                    SBSql.Append("   OR (Main.Help_Subject LIKE '%' + @Keyword + '%') ");
                    SBSql.Append("   OR (Main.Req_Who LIKE '%' + @Keyword + '%') ");
                    SBSql.Append(" ) ");

                    cmd.Parameters.AddWithValue("Keyword", Keyword);
                }
                //[查詢條件] - 需求部門
                if (false == string.IsNullOrEmpty(Dept))
                {
                    SBSql.Append(" AND (Main.Req_Dept = @Dept)");

                    cmd.Parameters.AddWithValue("Dept", Dept);
                }
                //[查詢條件] - 問題類別
                if (false == string.IsNullOrEmpty(Req_Class))
                {
                    SBSql.Append(" AND (Main.Req_Class = @ReqClass)");

                    cmd.Parameters.AddWithValue("ReqClass", Req_Class);
                }
                //[查詢條件] - 處理狀態
                if (false == string.IsNullOrEmpty(Help_Status))
                {
                    SBSql.Append(" AND (Main.Help_Status = @HelpStatus)");

                    cmd.Parameters.AddWithValue("HelpStatus", Help_Status);
                }
                #endregion
                SBSql.AppendLine(" ORDER BY Main.Create_Time");

                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料", "");
                        return;
                    }
                    //匯出Excel
                    fn_CustomUI.ExportExcel(
                        DT
                        , "{0}-資訊需求明細.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd")));
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"PKEF\IT_Help\";
        }
        set
        {
            this._Param_FileFolder = value;
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