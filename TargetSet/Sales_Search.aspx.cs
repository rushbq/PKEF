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

public partial class Sales_Search : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 業務目標
                if (fn_CheckAuth.CheckAuth_User("110", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //無:管理者權限，有:一般權限
                if (fn_CheckAuth.CheckAuth_User("111", out ErrMsg) == false
                    && fn_CheckAuth.CheckAuth_User("112", out ErrMsg))
                {
                    this.ddl_Dept.Enabled = false;
                }

                //[取得/檢查參數] - 出貨地
                if (fn_Extensions.Menu_ShipFrom(this.ddl_ShipFrom, Request.QueryString["ShipFrom"], true, out ErrMsg) == false)
                {
                    this.ddl_ShipFrom.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, Param_DeptID, true, false, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[帶出選單] - 人員
                if (fn_Extensions.Menu_Employee(this.ddl_Employee, Request.QueryString["StaffID"], true, Param_DeptID, false, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[帶出選單] - 年
                if (fn_Extensions.Menu_Year(this.ddl_Year, Request.QueryString["SetYear"], true, 1, out ErrMsg) == false)
                {
                    this.ddl_Year.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //[帶出資料]
                LookupDataList(page);

                //[權限判斷] - 編輯 (程式置放於 LookupDataList 之後)
                #region "--細項權限判斷--"
                if (fn_CheckAuth.CheckAuth_User("111", out ErrMsg) || fn_CheckAuth.CheckAuth_User("112", out ErrMsg))
                {
                    //[取得物件] - 判斷資料列表
                    if (this.lvDataList.Items.Count > 0)
                    {
                        //[取得物件] - 新增鈕
                        this.ph_Edit.Visible = true;
                        for (int i = 0; i < lvDataList.Items.Count; i++)
                        {
                            //[取得物件] - 編輯區
                            PlaceHolder ph_Edit = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_Edit"));
                            //[取得物件] - 空閒區
                            PlaceHolder ph_block = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_block"));
                            ph_Edit.Visible = true;
                            ph_block.Visible = false;
                        }
                    }
                }
                else
                {
                    //[取得物件] - 新增鈕
                    this.ph_Edit.Visible = false;
                    //[取得物件] - 判斷資料列表
                    if (this.lvDataList.Items.Count > 0)
                    {
                        for (int i = 0; i < lvDataList.Items.Count; i++)
                        {
                            //[取得物件] - 編輯區
                            PlaceHolder ph_Edit = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_Edit"));
                            //[取得物件] - 空閒區
                            PlaceHolder ph_block = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_block"));
                            ph_Edit.Visible = false;
                            ph_block.Visible = true;
                        }
                    }
                }
                #endregion
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
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "TargetSet/Sales_Search.aspx?func=set";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string ShipFrom = this.ddl_ShipFrom.SelectedValue;
            string Dept = this.ddl_Dept.SelectedValue;
            string SetYear = this.ddl_Year.SelectedValue;
            string StaffID = this.ddl_Employee.SelectedValue;

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢 (跨資料庫 PKEF, PKSYS)
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Shipping.SName, User_Dept.DeptName, TarData.ShipFrom, Staff.Display_Name ");
            SBSql.AppendLine("      , TarData.DeptID, TarData.SetYear, TarData.StaffID ");
            SBSql.AppendLine("      , SUM(TarData.Amount_NTD) AS Amount_NTD, SUM(TarData.Amount_USD) AS Amount_USD, SUM(TarData.Amount_RMB) AS Amount_RMB ");
            SBSql.AppendLine("      , SUM(TarData.OrdAmount_NTD) AS OrdAmount_NTD, SUM(TarData.OrdAmount_USD) AS OrdAmount_USD, SUM(TarData.OrdAmount_RMB) AS OrdAmount_RMB ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY TarData.ShipFrom, TarData.DeptID, TarData.SetYear DESC) AS RowRank ");
            SBSql.AppendLine("    FROM Target_Sales TarData ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.Shipping ON TarData.ShipFrom = Shipping.SID ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Dept ON TarData.DeptID = User_Dept.DeptID ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Profile Staff ON TarData.StaffID = Staff.Account_Name ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 出貨地
            if (false == string.IsNullOrEmpty(ShipFrom))
            {
                SBSql.Append(" AND (TarData.ShipFrom = @ShipFrom) ");
                cmd.Parameters.AddWithValue("ShipFrom", ShipFrom);

                this.ViewState["Page_LinkStr"] += "&ShipFrom=" + Server.UrlEncode(ShipFrom);
            }
            //[查詢條件] - 部門
            if (false == string.IsNullOrEmpty(Dept))
            {
                SBSql.Append(" AND (TarData.DeptID = @Dept) ");
                cmd.Parameters.AddWithValue("Dept", Dept);

                this.ViewState["Page_LinkStr"] += "&Dept=" + Server.UrlEncode(Dept);
            }
            //[查詢條件] - 年份
            if (false == string.IsNullOrEmpty(SetYear))
            {
                SBSql.Append(" AND (TarData.SetYear = @SetYear) ");
                cmd.Parameters.AddWithValue("SetYear", SetYear);

                this.ViewState["Page_LinkStr"] += "&SetYear=" + Server.UrlEncode(SetYear);
            }
            //[查詢條件] - 人員
            if (false == string.IsNullOrEmpty(StaffID))
            {
                SBSql.Append(" AND (TarData.StaffID = @StaffID) ");
                cmd.Parameters.AddWithValue("StaffID", StaffID);

                this.ViewState["Page_LinkStr"] += "&StaffID=" + Server.UrlEncode(StaffID);
            }

            #endregion

            SBSql.AppendLine("    GROUP BY Shipping.SName, User_Dept.DeptName, TarData.ShipFrom, Staff.Display_Name ");
            SBSql.AppendLine("      , TarData.DeptID, TarData.SetYear, TarData.StaffID ");
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
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("  SELECT TarData.ShipFrom, TarData.DeptID, TarData.SetYear, TarData.StaffID ");
            SBSql.AppendLine("  FROM Target_Sales TarData ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.Shipping ON TarData.ShipFrom = Shipping.SID ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Dept ON TarData.DeptID = User_Dept.DeptID ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.User_Profile AS Staff ON TarData.StaffID = Staff.Account_Name ");
            SBSql.AppendLine("  WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 出貨地
            if (false == string.IsNullOrEmpty(ShipFrom))
            {
                SBSql.Append("  AND (TarData.ShipFrom = @ShipFrom) ");
                cmdTotalCnt.Parameters.AddWithValue("ShipFrom", ShipFrom);
            }
            //[查詢條件] - 部門
            if (false == string.IsNullOrEmpty(Dept))
            {
                SBSql.Append("  AND (TarData.DeptID = @Dept) ");
                cmdTotalCnt.Parameters.AddWithValue("Dept", Dept);
            }
            //[查詢條件] - 年份
            if (false == string.IsNullOrEmpty(SetYear))
            {
                SBSql.Append(" AND (TarData.SetYear = @SetYear) ");
                cmdTotalCnt.Parameters.AddWithValue("SetYear", SetYear);
            }
            //[查詢條件] - 人員
            if (false == string.IsNullOrEmpty(StaffID))
            {
                SBSql.Append(" AND (TarData.StaffID = @StaffID) ");
                cmdTotalCnt.Parameters.AddWithValue("StaffID", StaffID);
            }
            #endregion

            SBSql.AppendLine("  GROUP BY TarData.ShipFrom, TarData.DeptID, TarData.SetYear, TarData.StaffID ");
            SBSql.AppendLine(" ) AS TblCnt ");
            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, dbConn.DBS.EFLocal, out TotalCnt, out ErrMsg))
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
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;
                    StringBuilder SBSql = new StringBuilder();

                    //取得Key值
                    string Get_ShipFrom = ((HiddenField)e.Item.FindControl("hf_ShipFrom")).Value;
                    string Get_DeptID = ((HiddenField)e.Item.FindControl("hf_DeptID")).Value;
                    string Get_StaffID = ((HiddenField)e.Item.FindControl("hf_StaffID")).Value;
                    string Get_Year = ((HiddenField)e.Item.FindControl("hf_SetYear")).Value;

                    //[SQL] - 刪除資料
                    SBSql.Clear();
                    SBSql.AppendLine(" DELETE FROM Target_Sales WHERE (ShipFrom = @ShipFrom) AND (DeptID = @DeptID) AND (StaffID = @StaffID) AND (SetYear = @SetYear) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("ShipFrom", Get_ShipFrom);
                    cmd.Parameters.AddWithValue("DeptID", Get_DeptID);
                    cmd.Parameters.AddWithValue("StaffID", Get_StaffID);
                    cmd.Parameters.AddWithValue("SetYear", Get_Year);
                    if (dbConn.ExecuteSql(cmd, dbConn.DBS.EFLocal, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料刪除失敗！", "");
                    }
                    else
                    {
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
            SBUrl.Append("Sales_Search.aspx?func=set");

            //[查詢條件] - 出貨地
            if (this.ddl_ShipFrom.SelectedIndex > 0)
            {
                SBUrl.Append("&ShipFrom=" + Server.UrlEncode(this.ddl_ShipFrom.SelectedValue));
            }
            //[查詢條件] - 部門
            if (this.ddl_Dept.SelectedIndex > 0)
            {
                SBUrl.Append("&DeptID=" + Server.UrlEncode(this.ddl_Dept.SelectedValue));
            }
            //[查詢條件] - 年
            if (this.ddl_Year.SelectedIndex > 0)
            {
                SBUrl.Append("&SetYear=" + Server.UrlEncode(this.ddl_Year.SelectedValue));
            }
            //[查詢條件] - 人員
            if (this.ddl_Employee.SelectedIndex > 0)
            {
                SBUrl.Append("&StaffID=" + Server.UrlEncode(this.ddl_Employee.SelectedValue));
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
            /* 判斷權限 
             *   管理者權限(111):帶出所有部門
             *   一般權限(112):帶出所屬部門
             */
            string ErrMsg;
            string deptID = "";

            //無:管理者權限，有:一般權限
            if (fn_CheckAuth.CheckAuth_User("111", out ErrMsg) == false
                && fn_CheckAuth.CheckAuth_User("112", out ErrMsg))
            {
                //取得目前使用者的部門
                deptID = ADService.getDepartmentFromGUID(fn_Params.UserGuid);
            }

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
    #endregion
}