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
using CustomController;

public partial class WR_Search : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 日誌查詢
                if (fn_CheckAuth.CheckAuth_User("330", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //[權限判斷] - 日誌查詢讀取權限
                if (Check_TTDAuth(fn_Params.UserAccount, out ErrMsg) == false)
                {
                    //隱藏部門/人員選單
                    this.ddl_Dept.Visible = false;
                    this.ddl_Employee.Visible = false;
                    this.ph_Menu.Visible = false;

                    //顯示類別選單
                    this.ph_ClassMenu.Visible = true;
                }

                //[取得/檢查參數] - StartDate
                String StartDate = Request.QueryString["StartDate"];
                if (fn_Extensions.String_字數(StartDate, "1", "10", out ErrMsg) && StartDate.IsDate())
                {
                    this.tb_StartDate.Text = fn_stringFormat.Filter_Html(StartDate.ToString().Trim());
                }
                else
                {
                    this.tb_StartDate.Text = DateTime.Now.AddDays(-7).ToShortDateString().ToDateString("yyyy/MM/dd");
                }

                //[取得/檢查參數] - EndDate
                String EndDate = Request.QueryString["EndDate"];
                if (fn_Extensions.String_字數(EndDate, "1", "10", out ErrMsg) && StartDate.IsDate())
                {
                    this.tb_EndDate.Text = fn_stringFormat.Filter_Html(EndDate.ToString().Trim());
                }
                else
                {
                    this.tb_EndDate.Text = DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd");
                }

                //[帶出選單] - 部門
                String DeptID = Request.QueryString["DeptID"];
                if (MyMenu_Dept(this.ddl_Dept, DeptID, true, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[帶出選單] - 人員
                String Employee = Request.QueryString["Employee"];
                if (MyMenu_ADUser(this.ddl_Employee, Employee, true, DeptID, out ErrMsg) == false)
                {
                    this.ddl_Employee.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 狀態
                String IsDone = Request.QueryString["IsDone"];
                if (fn_Extensions.String_字數(IsDone, "1", "1", out ErrMsg))
                {
                    this.ddl_IsDone.SelectedValue = IsDone;
                }

                //[取得/檢查參數] - 類別
                String ClassID = Request.QueryString["ClassID"];
                if (fn_Extensions.Menu_TTDClass(this.ddl_Class, ClassID, true, Param_DeptID, out ErrMsg) == false)
                {
                    this.ddl_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - Keyword
                String Keyword = Request.QueryString["Keyword"];
                if (fn_Extensions.String_字數(Keyword, "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = fn_stringFormat.Filter_Html(Keyword.Trim());
                }

                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //[取得/檢查參數] - SortName(排序欄位)
                this.lb_SortName.Text = GetSortDesc(SortName);

                //[取得/檢查參數] - Sortby(排序方式)
                String Sortby = Request.QueryString["Sortby"];
                if (fn_Extensions.String_字數(Sortby, "1", "5", out ErrMsg))
                {
                    this.ddl_Sortby.SelectedValue = Sortby.ToUpper();
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

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string StartDate = this.tb_StartDate.Text;
            string EndDate = this.tb_EndDate.Text;
            string DeptID = this.ddl_Dept.SelectedValue;
            string Employee = this.ddl_Employee.SelectedValue;
            string IsDone = this.ddl_IsDone.SelectedValue;
            string Sortby = this.ddl_Sortby.SelectedValue.ToUpper();
            string ClassID = this.ddl_Class.SelectedValue;
            string Keyword = this.tb_Keyword.Text;

            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = string.Format("{0}WorkReport/WR_Search.aspx?SortName={1}&Sortby={2}"
                , Application["WebUrl"]
                , Server.UrlEncode(SortName)
                , Server.UrlEncode(Sortby));

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢 (跨資料庫 PKEF, PKSYS)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Task.Task_Name, Task.Create_Time, Task.Complete_Time, Task.Remark ");
            SBSql.AppendLine("      , Cls.Class_Name, User_Dept.DeptName, User_Profile.Display_Name ");

            //排序方式
            switch (Sortby) {
                case "ASC":
                    Sortby = "ASC";
                    break;

                case "DESC":
                    Sortby = "DESC";
                    break;

                default:
                    Sortby = "ASC";
                    break;
            }

            //排序欄位
            if (false == string.IsNullOrEmpty(SortName))
            {
                SBSql.Append(" , ROW_NUMBER() OVER (ORDER BY ");
                switch (SortName.ToUpper())
                {
                    case "CLASS":   //依類別
                        SBSql.Append(" Cls.Class_Name ");
                        break;

                    case "CREATETIME":  //依建立時間
                        SBSql.Append(" Task.Create_Time ");
                        break;

                    case "COMPLETETIME":    //依完成時間
                        SBSql.Append(" Task.Complete_Time ");
                        break;

                    case "CREATEWHO":   //依建立者
                        SBSql.Append(" User_Profile.Display_Name ");
                        break;
                }
                SBSql.Append(string.Format(" {0}) AS RowRank ", Sortby));
            }
            else
            {
                //預設
                SBSql.Append(string.Format(" , ROW_NUMBER() OVER (ORDER BY Task.Create_Time {0}, User_Dept.DeptID, User_Profile.Account_Name) AS RowRank "
                    , Sortby));
            }

            SBSql.AppendLine("    FROM TTD_Task Task INNER JOIN TTD_Class Cls ON Task.Class_ID = Cls.Class_ID ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Dept ON Cls.DeptID = User_Dept.DeptID");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Profile ON Task.Create_Who = User_Profile.Account_Name");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Task.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));

                this.ViewState["Page_LinkStr"] += "&StartDate=" + Server.UrlEncode(StartDate);
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Task.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));

                this.ViewState["Page_LinkStr"] += "&EndDate=" + Server.UrlEncode(EndDate);
            }

            //判斷權限, 無權限帶自己
            if (Check_TTDAuth(fn_Params.UserAccount, out ErrMsg))
            {
                //[查詢條件] - 部門 (依權限)
                if (false == string.IsNullOrEmpty(DeptID))
                {
                    SBSql.Append(" AND (Cls.DeptID = @DeptID) ");
                    cmd.Parameters.AddWithValue("DeptID", DeptID);

                    this.ViewState["Page_LinkStr"] += "&DeptID=" + Server.UrlEncode(DeptID);
                }
                //[查詢條件] - 人員
                if (false == string.IsNullOrEmpty(Employee))
                {
                    SBSql.Append(" AND (Task.Create_Who = @Employee)");
                    cmd.Parameters.AddWithValue("Employee", Employee);

                    this.ViewState["Page_LinkStr"] += "&Employee=" + Server.UrlEncode(Employee);
                }

                SBSql.Append(" AND (Task.Create_Who IN ( ");
                SBSql.Append("  SELECT Auth.View_Account FROM TTD_ViewAuth Auth WHERE Auth.Account_Name = @Param_Who ");
                SBSql.Append(" ))");
                cmd.Parameters.AddWithValue("Param_Who", fn_Params.UserAccount);
            }
            else
            {
                SBSql.Append(" AND (Task.Create_Who = @Employee)");
                cmd.Parameters.AddWithValue("Employee", fn_Params.UserAccount);
            }

            //[查詢條件] - 狀態
            if (false == string.IsNullOrEmpty(IsDone))
            {
                switch (IsDone.ToUpper())
                {
                    case "Y":
                        SBSql.Append(" AND (Task.Complete_Time IS NOT NULL)");
                        break;

                    case "N":
                        SBSql.Append(" AND (Task.Complete_Time IS NULL)");
                        break;
                }

                this.ViewState["Page_LinkStr"] += "&IsDone=" + Server.UrlEncode(IsDone);
            }

            //[查詢條件] - 類別
            if (false == string.IsNullOrEmpty(ClassID))
            {
                SBSql.Append(" AND (Task.Class_ID = @ClassID)");
                cmd.Parameters.AddWithValue("ClassID", ClassID);

                this.ViewState["Page_LinkStr"] += "&ClassID=" + Server.UrlEncode(ClassID);
            }

            //[查詢條件] - Keyword
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Task.Task_Name LIKE '%' + @Param_Keyword + '%') ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Param_Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
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
            SBSql.AppendLine(" FROM TTD_Task Task INNER JOIN TTD_Class Cls ON Task.Class_ID = Cls.Class_ID ");
            SBSql.AppendLine("   INNER JOIN PKSYS.dbo.User_Dept ON Cls.DeptID = User_Dept.DeptID");
            SBSql.AppendLine("   INNER JOIN PKSYS.dbo.User_Profile ON Task.Create_Who = User_Profile.Account_Name");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Task.Create_Time >= @StartDate) ");
                cmdTotalCnt.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Task.Create_Time <= @EndDate) ");
                cmdTotalCnt.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }
            //判斷權限, 無權限帶自己
            if (Check_TTDAuth(fn_Params.UserAccount, out ErrMsg))
            {
                //[查詢條件] - 部門 (依權限)
                if (false == string.IsNullOrEmpty(DeptID))
                {
                    SBSql.Append(" AND (Cls.DeptID = @DeptID) ");
                    cmdTotalCnt.Parameters.AddWithValue("DeptID", DeptID);
                }
                //[查詢條件] - 人員
                if (false == string.IsNullOrEmpty(Employee))
                {
                    SBSql.Append(" AND (Task.Create_Who = @Employee)");
                    cmdTotalCnt.Parameters.AddWithValue("Employee", Employee);
                }

                SBSql.Append(" AND (Task.Create_Who IN ( ");
                SBSql.Append("  SELECT Auth.View_Account FROM TTD_ViewAuth Auth WHERE Auth.Account_Name = @Param_Who ");
                SBSql.Append(" ))");
                cmdTotalCnt.Parameters.AddWithValue("Param_Who", fn_Params.UserAccount);
            }
            else
            {
                SBSql.Append(" AND (Task.Create_Who = @Employee)");
                cmdTotalCnt.Parameters.AddWithValue("Employee", fn_Params.UserAccount);
            }

            //[查詢條件] - 狀態
            if (false == string.IsNullOrEmpty(IsDone))
            {
                switch (IsDone.ToUpper())
                {
                    case "Y":
                        SBSql.Append(" AND (Task.Complete_Time IS NOT NULL)");
                        break;

                    case "N":
                        SBSql.Append(" AND (Task.Complete_Time IS NULL)");
                        break;
                }
            }

            //[查詢條件] - 類別
            if (false == string.IsNullOrEmpty(ClassID))
            {
                SBSql.Append(" AND (Task.Class_ID = @ClassID)");
                cmdTotalCnt.Parameters.AddWithValue("ClassID", ClassID);
            }

            //[查詢條件] - Keyword
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Task.Task_Name LIKE '%' + @Param_Keyword + '%') ");
                SBSql.Append(" ) ");
                cmdTotalCnt.Parameters.AddWithValue("Param_Keyword", Keyword);
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
    /// 選單 - 部門 (依TTD權限設定)
    /// </summary>
    /// <param name="setMenu">控制項</param>
    /// <param name="inputValue">輸入值</param>
    /// <param name="showRoot">是否顯示索引文字</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns></returns>
    private bool MyMenu_Dept(DropDownListGP setMenu, string inputValue, bool showRoot, out string ErrMsg)
    {
        ErrMsg = "";
        setMenu.Items.Clear();
        try
        {
            //[取得資料] - 部門資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Shipping.SName, User_Dept.DeptID, User_Dept.DeptName ");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Shipping.SID ORDER BY Shipping.Sort, User_Dept.Sort ASC) AS GP_Rank");
                SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                SBSql.AppendLine("   INNER JOIN Shipping WITH (NOLOCK) ON User_Dept.Area = Shipping.SID ");
                SBSql.AppendLine(" WHERE (User_Dept.Display = 'Y') AND (User_Dept.DeptID IN ( ");
                SBSql.AppendLine("    SELECT Prof.DeptID");
                SBSql.AppendLine("    FROM PKEF.dbo.TTD_ViewAuth Auth INNER JOIN User_Profile Prof ON Auth.View_Account = Prof.Account_Name");
                SBSql.AppendLine("    WHERE Auth.Account_Name = @Param_Who");
                SBSql.AppendLine("    GROUP BY Prof.DeptID");
                SBSql.AppendLine(" ))");
                SBSql.AppendLine(" ORDER BY Shipping.Sort, User_Dept.Sort ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Who", fn_Params.UserAccount);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //判斷GP_Rank, 若為第一項，則輸出群組名稱
                        if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                        {
                            setMenu.AddItemGroup(DT.Rows[row]["SName"].ToString());

                        }

                        setMenu.Items.Add(new ListItem(DT.Rows[row]["DeptName"].ToString()
                                     , DT.Rows[row]["DeptID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (false == string.IsNullOrEmpty(inputValue))
                    {
                        setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                    }
                    //判斷是否要顯示索引文字
                    if (showRoot)
                    {
                        setMenu.Items.Insert(0, new ListItem("-- 選擇部門 --", ""));
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 選單 - AD人員 (依TTD權限設定)
    /// </summary>
    /// <param name="setMenu">控制項</param>
    /// <param name="inputValue">輸入值</param>
    /// <param name="showRoot">是否顯示索引文字</param>
    /// <param name="filterDept">篩選-部門</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns></returns>
    private bool MyMenu_ADUser(DropDownListGP setMenu, string inputValue, bool showRoot, string filterDept, out string ErrMsg)
    {
        ErrMsg = "";
        setMenu.Items.Clear();
        try
        {
            //[取得資料] - 人員資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Staff.Account_Name, Staff.Display_Name, User_Dept.DeptID, User_Dept.DeptName ");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY User_Dept.DeptID ORDER BY User_Dept.Sort, Staff.Account_Name ASC) AS GP_Rank ");
                SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                SBSql.AppendLine("  INNER JOIN User_Profile Staff WITH (NOLOCK) ON User_Dept.DeptID = Staff.DeptID");
                SBSql.AppendLine(" WHERE (User_Dept.Display = 'Y') AND (Staff.Display = 'Y') AND (Staff.Account_Name IN ( ");
                SBSql.AppendLine("    SELECT Prof.Account_Name");
                SBSql.AppendLine("    FROM PKEF.dbo.TTD_ViewAuth Auth INNER JOIN User_Profile Prof ON Auth.View_Account = Prof.Account_Name");
                SBSql.AppendLine("    WHERE Auth.Account_Name = @Param_Who");
                SBSql.AppendLine(" ))");
                //判斷部門
                if (false == string.IsNullOrEmpty(filterDept))
                {
                    SBSql.AppendLine(" AND (User_Dept.DeptID = @DeptID) ");

                    cmd.Parameters.AddWithValue("DeptID", filterDept);
                }
                SBSql.AppendLine(" ORDER BY User_Dept.Sort ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Who", fn_Params.UserAccount);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //判斷GP_Rank, 若為第一項，則輸出群組名稱
                        if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                        {
                            setMenu.AddItemGroup(DT.Rows[row]["DeptName"].ToString());

                        }

                        setMenu.Items.Add(new ListItem(DT.Rows[row]["Display_Name"].ToString()
                                     , DT.Rows[row]["Account_Name"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (false == string.IsNullOrEmpty(inputValue))
                    {
                        setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                    }
                    //判斷是否要顯示索引文字
                    if (showRoot)
                    {
                        setMenu.Items.Insert(0, new ListItem("-- 選擇人員 --", ""));
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 判斷讀取權限, 顯示部門/人員選單
    /// </summary>
    /// <param name="UserID"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool Check_TTDAuth(string UserID, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Account_Name ");
                SBSql.AppendLine(" FROM TTD_ViewAuth WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Account_Name = @Param_Who) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Who", UserID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 排序欄位說明
    /// </summary>
    /// <param name="SortName">排序欄位參數</param>
    /// <returns></returns>
    private string GetSortDesc(string SortName)
    {
        switch (SortName.ToUpper())
        {
            case "CLASS":
                return "類別";

            case "CREATETIME":
                return "建立時間";

            case "COMPLETETIME":
                return "完成時間";

            case "CREATEWHO":
                return "建立者";

            default:
                return "建立時間";
        }
    }

    #endregion

    #region -- 前端頁面控制 --
    /// <summary>
    /// 分頁跳轉
    /// </summary>
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        SortSearch("CreateTime");
    }

    /// <summary>
    /// 切換部門
    /// </summary>
    protected void ddl_Dept_SelectedIndexChanged(object sender, EventArgs e)
    {
        string ErrMsg;
        if (this.ddl_Dept.SelectedIndex > 0)
        {
            //[帶出選單] - 人員
            if (MyMenu_ADUser(this.ddl_Employee, "", true, this.ddl_Dept.SelectedValue, out ErrMsg) == false)
            {
                this.ddl_Employee.Items.Insert(0, new ListItem("選單產生失敗", ""));
            }
        }
    }
    #endregion

    #region --排序搜尋--
    protected void btn_sClass(object sender, EventArgs e)
    {
        SortSearch("Class");
    }

    protected void btn_sCreateTime(object sender, EventArgs e)
    {
        SortSearch("CreateTime");
    }

    protected void btn_sCompTime(object sender, EventArgs e)
    {
        SortSearch("CompleteTime");
    }

    protected void btn_sCreateWho(object sender, EventArgs e)
    {
        SortSearch("CreateWho");
    }

    private void SortSearch(string sortName)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("WR_Search.aspx?func=set");

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
            //[查詢條件] - 部門
            if (this.ddl_Dept.SelectedIndex > 0)
            {
                SBUrl.Append("&DeptID=" + Server.UrlEncode(this.ddl_Dept.SelectedValue));
            }
            //[查詢條件] - 人員
            if (this.ddl_Employee.SelectedIndex > 0)
            {
                SBUrl.Append("&Employee=" + Server.UrlEncode(this.ddl_Employee.SelectedValue));
            }
            //[查詢條件] - 狀態
            if (this.ddl_IsDone.SelectedIndex > 0)
            {
                SBUrl.Append("&IsDone=" + Server.UrlEncode(this.ddl_IsDone.SelectedValue));
            }
            //[查詢條件] - 類別
            if (this.ddl_Class.SelectedIndex > 0)
            {
                SBUrl.Append("&ClassID=" + Server.UrlEncode(this.ddl_Class.SelectedValue));
            }
            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text.Trim()));
            }

            //[查詢條件] - 排序方式
            SBUrl.Append("&Sortby=" + Server.UrlEncode(this.ddl_Sortby.SelectedValue));

            //[查詢條件] - 排序欄位
            SBUrl.Append("&SortName=" + Server.UrlEncode(sortName));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }
    #endregion

    #region --參數設定--
    /// <summary>
    /// 排序欄位
    /// </summary>
    private string _SortName;
    public string SortName
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["SortName"]) ? "CreateTime" : fn_stringFormat.Filter_Html(Request.QueryString["SortName"].ToString());
        }
        set
        {
            this._SortName = value;
        }
    }

    /// <summary>
    /// 我的部門代號
    /// </summary>
    private string _Param_DeptID;
    public string Param_DeptID
    {
        get
        {
            //取得目前使用者的部門
            return ADService.getDepartmentFromGUID(fn_Params.UserGuid);
        }
        set
        {
            this._Param_DeptID = value;
        }
    }
    #endregion
       
}