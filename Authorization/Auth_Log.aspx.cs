using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Specialized;

public partial class Auth_Log : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";
       
            //[權限判斷] - 異動記錄
            if (fn_CheckAuth.CheckAuth_User("9905", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[取得/檢查參數] - page(頁數)
            int page = 1;
            if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
            //[取得/檢查參數] - BgDate(開始日期)
            if (Request.QueryString["BgDate"].IsDate())
            {
                this.tb_BgDate.Text = Request.QueryString["BgDate"].ToString().Trim();
            }
            //[取得/檢查參數] - EdDate(結束日期)
            if (Request.QueryString["EdDate"].IsDate())
            {
                this.tb_EdDate.Text = Request.QueryString["EdDate"].ToString().Trim();
            }
            //[取得/檢查參數] - Keyword
            if (fn_Extensions.String_字數(Request.QueryString["Keyword"], "1", "50", out ErrMsg))
            {
                this.tb_Keyword.Text = Request.QueryString["Keyword"].ToString().Trim();
            }
            //[取得/檢查參數] - ProcType(類別)
            if (fn_Extensions.String_字數(Request.QueryString["ProcType"], "1", "10", out ErrMsg))
            {
                this.ddl_ProcType.SelectedValue = Request.QueryString["ProcType"].ToString();
            }


            //[帶出資料]
            LookupDataList(page);
        }

    }

    #region "資料取得"
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="page">頁數</param>
    private void LookupDataList(int page)
    {
        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        string ErrMsg = "";
        try
        {
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = "Auth_Log.aspx?func=Log";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢(跨資料庫:PKSYS, PKEF)
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("  SELECT ");
            SBSql.AppendLine("      Log_AD.Log_ID, Log_AD.Proc_Time, Log_AD.Proc_Type, Log_AD.Proc_Action, Log_AD.Proc_Account, Log_AD.Proc_Desc ");
            SBSql.AppendLine("      , (Case When Log_AD.Create_Who = 'System' Then Log_AD.Create_Who Else User_Profile.Display_Name End) AS Create_Name ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Log_AD.Proc_Time DESC) AS RowRank");
            //[SQL] - 判斷是否有權限記錄
            SBSql.AppendLine("      , (SELECT COUNT(*) FROM Log_User_Profile_Rel_Program Rel WHERE (Rel.Log_ID = Log_AD.Log_ID)) AS ProfileCnt ");
            SBSql.AppendLine("      , (SELECT COUNT(*) FROM Log_User_Group_Rel_Program Rel WHERE (Rel.Log_ID = Log_AD.Log_ID)) AS GroupCnt ");
            SBSql.AppendLine("  FROM Log_AD LEFT JOIN PKSYS.dbo.User_Profile ON Log_AD.Create_Who = User_Profile.Account_Name ");
            SBSql.AppendLine("  WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - BgDate(開始日期)
            if (string.IsNullOrEmpty(this.tb_BgDate.Text) == false)
            {
                SBSql.Append(" AND (Log_AD.Proc_Time >= @BgDate)");
                cmd.Parameters.AddWithValue("BgDate", this.tb_BgDate.Text.ToDateString("yyyy/MM/dd 00:00:00"));

                this.ViewState["Page_LinkStr"] += "&BgDate=" + Server.UrlEncode(this.tb_BgDate.Text);
            }
            //[查詢條件] - EdDate(結束日期)
            if (string.IsNullOrEmpty(this.tb_EdDate.Text) == false)
            {
                SBSql.Append(" AND (Log_AD.Proc_Time <= @EdDate)");
                cmd.Parameters.AddWithValue("EdDate", this.tb_EdDate.Text.ToDateString("yyyy/MM/dd 23:59:59"));

                this.ViewState["Page_LinkStr"] += "&EdDate=" + Server.UrlEncode(this.tb_EdDate.Text);
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBSql.Append(" AND (Log_AD.Proc_Account LIKE '%' + @Param_Keyword + '%') ");
                cmd.Parameters.AddWithValue("Param_Keyword", this.tb_Keyword.Text);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text);
            }
            //[查詢條件] - ProcType(類別)
            if (this.ddl_ProcType.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Log_AD.Proc_Type = @ProcType) ");
                cmd.Parameters.AddWithValue("ProcType", this.ddl_ProcType.SelectedValue);

                this.ViewState["Page_LinkStr"] += "&ProcType=" + Server.UrlEncode(this.ddl_ProcType.SelectedValue);
            }
            #endregion

            SBSql.AppendLine("       ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Log_AD LEFT JOIN PKSYS.dbo.User_Profile ON Log_AD.Create_Who = User_Profile.Account_Name ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - BgDate(開始日期)
            if (string.IsNullOrEmpty(this.tb_BgDate.Text) == false)
            {
                SBSql.Append(" AND (Log_AD.Proc_Time >= @BgDate)");
                cmdTotalCnt.Parameters.AddWithValue("BgDate", this.tb_BgDate.Text.ToDateString("yyyy/MM/dd 00:00:00"));
            }
            //[查詢條件] - EdDate(結束日期)
            if (string.IsNullOrEmpty(this.tb_EdDate.Text) == false)
            {
                SBSql.Append(" AND (Log_AD.Proc_Time <= @EdDate)");
                cmdTotalCnt.Parameters.AddWithValue("EdDate", this.tb_EdDate.Text.ToDateString("yyyy/MM/dd 23:59:59"));
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBSql.Append(" AND (Log_AD.Proc_Account LIKE '%' + @Param_Keyword + '%') ");
                cmdTotalCnt.Parameters.AddWithValue("Param_Keyword", this.tb_Keyword.Text);
            }

            //[查詢條件] - ProcType(類別)
            if (this.ddl_ProcType.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Log_AD.Proc_Type = @ProcType) ");
                cmdTotalCnt.Parameters.AddWithValue("ProcType", this.ddl_ProcType.SelectedValue);
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
                //暫存目前Url
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"];
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", ""); 
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
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            //取得處理類型
            string Proc_Type = DataBinder.Eval(dataItem.DataItem, "Proc_Type").ToString().Trim();
            //處理類型 - 中文名稱
            Literal lt_Proc_Type = (Literal)e.Item.FindControl("lt_Proc_Type");
            switch (Proc_Type.ToUpper())
            {
                case "USER":
                    lt_Proc_Type.Text = "使用者";
                    break;

                case "GROUP":
                    lt_Proc_Type.Text = "群組";
                    break;
            }

            //檢視連結
            Literal lt_View = (Literal)e.Item.FindControl("lt_View");
            if ((int)DataBinder.Eval(dataItem.DataItem, "ProfileCnt") == 0
                && (int)DataBinder.Eval(dataItem.DataItem, "GroupCnt") == 0)
            {
                lt_View.Text = "";
            }
            else
            {
                lt_View.Text = string.Format("<a href=\"Auth_Log_View.aspx?LogID={0}\" class=\"ViewBox\">檢視</a>"
                    , HttpUtility.UrlEncode(Cryptograph.Encrypt(DataBinder.Eval(dataItem.DataItem, "Log_ID").ToString())));
            }
        }
    }

    #endregion

    #region "前端頁面控制"
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
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Auth_Log.aspx?func=Log");

            //[查詢條件] - BgDate(開始日期)
            if (string.IsNullOrEmpty(this.tb_BgDate.Text) == false)
                SBUrl.Append("&BgDate=" + Server.UrlEncode(this.tb_BgDate.Text));
            //[查詢條件] - EdDate(結束日期)
            if (string.IsNullOrEmpty(this.tb_EdDate.Text) == false)
                SBUrl.Append("&EdDate=" + Server.UrlEncode(this.tb_EdDate.Text));
            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text));
            //[查詢條件] - ProcType(類別)
            if (this.ddl_ProcType.SelectedIndex > 0)
                SBUrl.Append("&ProcType=" + Server.UrlEncode(this.ddl_ProcType.SelectedValue));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    #endregion

}
