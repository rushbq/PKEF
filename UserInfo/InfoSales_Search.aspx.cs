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

public partial class InfoSales_Search : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 業務資料維護
                if (fn_CheckAuth.CheckAuth_User("410", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[權限判斷] - 取得區域別
                if (Param_AreaCode == null)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[取得/檢查參數] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, Param_DeptID, true, false, Param_AreaCode, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - Keyword
                if (fn_Extensions.String_字數(Request.QueryString["Keyword"], "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = Request.QueryString["Keyword"].ToString().Trim();
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
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "UserInfo/InfoSales_Search.aspx?func=set";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string Dept = this.ddl_Dept.SelectedValue;
            string Keyword = fn_stringFormat.Filter_Html(this.tb_Keyword.Text.Trim());

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT Prof.Display_Name, Prof.Account_Name, Prof.ERP_LoginID, Prof.ERP_UserID ");
            SBSql.AppendLine("      , Prof.Email, Prof.NickName, Prof.Tel, Prof.Tel_Ext, Prof.Mobile");
            SBSql.AppendLine("      , Prof.IM_Skype, Prof.IM_QQ, Prof.IM_Line");
            SBSql.AppendLine("      , Dept.DeptName");
            SBSql.AppendLine("      , Shipping.SName");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Shipping.Sort, Dept.DeptID, Prof.Account_Name) AS RowRank ");
            SBSql.AppendLine("    FROM User_Profile Prof ");
            SBSql.AppendLine("    INNER JOIN User_Dept Dept ON Prof.DeptID = Dept.DeptID");
            SBSql.AppendLine("    INNER JOIN Shipping ON Dept.Area = Shipping.SID");
            SBSql.AppendLine("    WHERE (Prof.Display = 'Y') ");

            #region "查詢條件"
            //[查詢條件] - 部門
            if (false == string.IsNullOrEmpty(Param_DeptID))
            {
                SBSql.Append(" AND (Dept.DeptID = @Dept) ");

                cmd.Parameters.AddWithValue("Dept", Dept);

                this.ViewState["Page_LinkStr"] += "&Dept=" + Server.UrlEncode(Dept);
            }

            //[查詢條件] - 人員關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Prof.Account_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.Display_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.ERP_LoginID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.ERP_UserID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.NickName LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }

            //[查詢條件] - 區域別
            if (Param_AreaCode != null)
            {
                SBSql.Append(" AND (Dept.Area IN ({0}))".FormatThis(fn_Extensions.GetSQLParam(Param_AreaCode, "Area")));

                for (int row = 0; row < Param_AreaCode.Count; row++)
                {
                    cmd.Parameters.AddWithValue("Area{0}".FormatThis(row), Param_AreaCode[row].ToString());
                }
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
            SBSql.AppendLine(" FROM User_Profile Prof ");
            SBSql.AppendLine("  INNER JOIN User_Dept Dept ON Prof.DeptID = Dept.DeptID");
            SBSql.AppendLine("  INNER JOIN Shipping ON Dept.Area = Shipping.SID");
            SBSql.AppendLine(" WHERE (Prof.Display = 'Y') ");

            #region "查詢條件"
            //[查詢條件] - 部門
            if (false == string.IsNullOrEmpty(Param_DeptID))
            {
                SBSql.Append(" AND (Dept.DeptID = @Dept) ");

                cmdTotalCnt.Parameters.AddWithValue("Dept", Dept);
            }

            //[查詢條件] - 人員關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Prof.Account_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.Display_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.ERP_LoginID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("   OR (Prof.ERP_UserID LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }

            //[查詢條件] - 區域別
            if (Param_AreaCode != null)
            {

                SBSql.Append(" AND (Dept.Area IN ({0}))".FormatThis(fn_Extensions.GetSQLParam(Param_AreaCode, "Area")));

                for (int row = 0; row < Param_AreaCode.Count; row++)
                {
                    cmdTotalCnt.Parameters.AddWithValue("Area{0}".FormatThis(row), Param_AreaCode[row].ToString());
                }
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
                
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 取得資料列表！");
        }
        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
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
            SBUrl.Append("InfoSales_Search.aspx?func=set");

            //[查詢條件] - 部門
            if (this.ddl_Dept.SelectedIndex > 0)
            {
                SBUrl.Append("&DeptID=" + Server.UrlEncode(this.ddl_Dept.SelectedValue));
            }
            //[查詢條件] - 關鍵字
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text));
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
    /// 部門代號
    /// </summary>
    private string _Param_DeptID;
    public string Param_DeptID
    {
        get
        {
            string deptID = "";

            //若deptID為空，則判斷是否有request
            return (string.IsNullOrEmpty(deptID)) ?
                string.IsNullOrEmpty(Request.QueryString["DeptID"]) ? "" : Request.QueryString["DeptID"].ToString()
                : deptID;
        }
        set
        {
            this._Param_DeptID = value;
        }
    }

    private List<string> _Param_AreaCode;
    public List<string> Param_AreaCode
    {
        get
        {
            string ErrMsg;
            return fn_Extensions.GetAreaCode("411#412#413", fn_Params.UserGuid, out ErrMsg);
        }
        set
        {
            this._Param_AreaCode = value;
        }
    }
    #endregion
}