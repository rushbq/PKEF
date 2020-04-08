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

public partial class SupplierDelivery_sh : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[初始化]
                string ErrMsg;
                ////[權限判斷] - 符號資料庫
                //if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
                //{
                //    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                //    return;
                //}

                //[取得資料] - 採購人員
                Get_Employee();

                //[取得/檢查參數] - 採購人員
                if (fn_Extensions.String_字數(Request.QueryString["Employee"], "1", "10", out ErrMsg))
                {
                    this.ddl_Employee.SelectedIndex = this.ddl_Employee.Items.IndexOf(
                               this.ddl_Employee.Items.FindByValue(Request.QueryString["Employee"].ToString().Trim())
                               );
                }

                //[取得/檢查參數] - 供應商代號
                if (fn_Extensions.String_字數(Request.QueryString["BgSupID"], "1", "10", out ErrMsg))
                {
                    this.tb_BgSupID.Text = fn_stringFormat.Filter_Html(Request.QueryString["BgSupID"].ToString().Trim());
                }
                if (fn_Extensions.String_字數(Request.QueryString["EdSupID"], "1", "10", out ErrMsg))
                {
                    this.tb_EdSupID.Text = fn_stringFormat.Filter_Html(Request.QueryString["EdSupID"].ToString().Trim());
                }

                //[取得/檢查參數] - 關鍵字
                if (fn_Extensions.String_字數(Request.QueryString["Keyword"], "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = fn_stringFormat.Filter_Html(Request.QueryString["Keyword"].ToString().Trim());
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
        string ErrMsg = "";
        try
        {
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "Purchasing/SupplierDelivery_sh.aspx?func=sd";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string BgSupID = this.tb_BgSupID.Text;
            string EdSupID = this.tb_EdSupID.Text;
            string Keyword = this.tb_Keyword.Text;
            string Employee = this.ddl_Employee.SelectedValue;

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      PURMA.MA001, PURMA.MA002, Sup.SupDelDay, CMSMV.MV002 ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY PURMA.MA001) AS RowRank ");
            SBSql.AppendLine("    FROM [SHPK2].dbo.PURMA ");
            SBSql.AppendLine("      LEFT JOIN [SHPK2].dbo.SupplierA Sup ON PURMA.MA001 = Sup.SupCode ");
            SBSql.AppendLine("      LEFT JOIN [SHPK2].dbo.CMSMV ON PURMA.MA047 = CMSMV.MV001 ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 廠商代號
            if (false == string.IsNullOrEmpty(BgSupID) && false == string.IsNullOrEmpty(EdSupID))
            {
                //兩欄皆有值，查詢區間資料
                SBSql.Append(" AND (");
                SBSql.Append("  UPPER(PURMA.MA001) BETWEEN UPPER(@BgSupID) AND UPPER(@EdSupID) ");
                SBSql.Append(" )");

                cmd.Parameters.AddWithValue("BgSupID", BgSupID);
                cmd.Parameters.AddWithValue("EdSupID", EdSupID);

                this.ViewState["Page_LinkStr"] += "&BgSupID=" + Server.UrlEncode(BgSupID) + "&EdSupID=" + Server.UrlEncode(EdSupID);
            }
            else
            {
                if (false == string.IsNullOrEmpty(BgSupID))
                {
                    SBSql.Append(" AND (UPPER(PURMA.MA001) = UPPER(@BgSupID))");
                    cmd.Parameters.AddWithValue("BgSupID", BgSupID);

                    this.ViewState["Page_LinkStr"] += "&BgSupID=" + Server.UrlEncode(BgSupID);
                }
                if (false == string.IsNullOrEmpty(EdSupID))
                {
                    SBSql.Append(" AND (UPPER(PURMA.MA001) = UPPER(@EdSupID))");
                    cmd.Parameters.AddWithValue("EdSupID", EdSupID);

                    this.ViewState["Page_LinkStr"] += "&EdSupID=" + Server.UrlEncode(EdSupID);
                }
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(PURMA.MA001) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(PURMA.MA002) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }

            //[查詢條件] - 採購人員
            if (this.ddl_Employee.SelectedIndex > 0)
            {
                SBSql.Append(" AND (CMSMV.MV001 = @Employee) ");
                cmd.Parameters.AddWithValue("Employee", Employee);

                this.ViewState["Page_LinkStr"] += "&Employee=" + Server.UrlEncode(Employee);
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
            SBSql.AppendLine(" FROM [SHPK2].dbo.PURMA ");
            SBSql.AppendLine("  LEFT JOIN [SHPK2].dbo.SupplierA Sup ON PURMA.MA001 = Sup.SupCode ");
            SBSql.AppendLine("  LEFT JOIN [SHPK2].dbo.CMSMV ON PURMA.MA047 = CMSMV.MV001 ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 廠商代號
            if (false == string.IsNullOrEmpty(BgSupID) && false == string.IsNullOrEmpty(EdSupID))
            {
                //兩欄皆有值，查詢區間資料
                SBSql.Append(" AND (");
                SBSql.Append("  UPPER(PURMA.MA001) BETWEEN UPPER(@BgSupID) AND UPPER(@EdSupID) ");
                SBSql.Append(" )");

                cmdTotalCnt.Parameters.AddWithValue("BgSupID", BgSupID);
                cmdTotalCnt.Parameters.AddWithValue("EdSupID", EdSupID);
            }
            else
            {
                if (false == string.IsNullOrEmpty(BgSupID))
                {
                    SBSql.Append(" AND (UPPER(PURMA.MA001) = UPPER(@BgSupID))");
                    cmdTotalCnt.Parameters.AddWithValue("BgSupID", BgSupID);
                }
                if (false == string.IsNullOrEmpty(EdSupID))
                {
                    SBSql.Append(" AND (UPPER(PURMA.MA001) = UPPER(@EdSupID))");
                    cmdTotalCnt.Parameters.AddWithValue("EdSupID", EdSupID);
                }
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(PURMA.MA001) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(PURMA.MA002) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }

            //[查詢條件] - 採購人員
            if (this.ddl_Employee.SelectedIndex > 0)
            {
                SBSql.Append(" AND (CMSMV.MV001 = @Employee) ");
                cmdTotalCnt.Parameters.AddWithValue("Employee", Employee);
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();

            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, dbConn.DBS.ERP_Ana, out TotalCnt, out ErrMsg))
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


    /// <summary>
    /// 取得採購人員
    /// </summary>
    private void Get_Employee()
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT RTRIM(Acc.MV001) AS MV001, RTRIM(Acc.MV002) AS MV002 ");
                SBSql.AppendLine(" FROM [DSCSYS].[dbo].DSCMA INNER JOIN [SHPK2].dbo.CMSMV AS Acc ");
                SBSql.AppendLine("  ON DSCMA.MA002 = Acc.MV002 ");
                SBSql.AppendLine(" WHERE (DSCMA.MA005 = '') AND (Acc.MV004 = '151') ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.ERP_Ana, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {

                        return;
                    }

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        this.ddl_Employee.Items.Add(new ListItem(
                            DT.Rows[row]["MV002"].ToString(), DT.Rows[row]["MV001"].ToString()
                                ));
                    }

                    this.ddl_Employee.Items.Insert(0, new ListItem("-- 所有資料 --", ""));
                    this.ddl_Employee.SelectedIndex = 0;
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("無法取得採購人員名單");
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
            SBUrl.Append("SupplierDelivery_sh.aspx?func=sd");

            //[查詢條件] - 廠商代號
            if (false == string.IsNullOrEmpty(this.tb_BgSupID.Text))
            {
                SBUrl.Append("&BgSupID=" + Server.UrlEncode(this.tb_BgSupID.Text.Trim()));
            }
            if (false == string.IsNullOrEmpty(this.tb_EdSupID.Text))
            {
                SBUrl.Append("&EdSupID=" + Server.UrlEncode(this.tb_EdSupID.Text.Trim()));
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text.Trim()));
            }

            //[查詢條件] - 採購人員
            if (this.ddl_Employee.SelectedIndex > 0)
            {
                SBUrl.Append("&Employee=" + Server.UrlEncode(this.ddl_Employee.SelectedValue));
            }
            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    //匯出
    protected void lbtn_Export_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("SupplierDelivery_sh_Excel.aspx?func=sd");

            //[查詢條件] - 廠商代號
            if (false == string.IsNullOrEmpty(this.tb_BgSupID.Text))
            {
                SBUrl.Append("&BgSupID=" + Server.UrlEncode(this.tb_BgSupID.Text.Trim()));
            }
            if (false == string.IsNullOrEmpty(this.tb_EdSupID.Text))
            {
                SBUrl.Append("&EdSupID=" + Server.UrlEncode(this.tb_EdSupID.Text.Trim()));
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text.Trim()));
            }

            //[查詢條件] - 採購人員
            if (this.ddl_Employee.SelectedIndex > 0)
            {
                SBUrl.Append("&Employee=" + Server.UrlEncode(this.ddl_Employee.SelectedValue));
            }

            //執行
            Response.Redirect(SBUrl.ToString(), false);
            //fn_Extensions.JsAlert("", "script:window.open(\"" + SBUrl.ToString()  + "\",\"_blank\");");

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 匯出！", "");
        }
    }

    #endregion

}