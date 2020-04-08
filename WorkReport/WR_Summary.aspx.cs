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

public partial class WR_Summary : SecurityIn
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

                //[取得/檢查參數] - SearchDate
                String SearchDate = Request.QueryString["SearchDate"];
                if (fn_Extensions.String_字數(SearchDate, "1", "10", out ErrMsg) && SearchDate.IsDate())
                {
                    this.tb_ShowDT.Text = fn_stringFormat.Filter_Html(SearchDate.ToString().Trim());
                }
                else
                {
                    this.tb_ShowDT.Text = DateTime.Now.AddDays(-1).ToShortDateString().ToDateString("yyyy/MM/dd");
                }

                //[帶出資料]
                LookupDataList();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
            return;
        }
    }

    #region -- 資料取得 --
    private void LookupDataList()
    {
        string ErrMsg;
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //取得參數
                string searchDate = this.tb_ShowDT.Text;
                this.lb_SearchDate.Text = searchDate.ToDateString("yyyy-MM-dd");

                //[SQL] - 資料查詢
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine("SELECT Dept.DeptID, Dept.Area, Dept.DeptName, COUNT(*) AS Total ");
                //計數 - 未完成
                SBSql.AppendLine("    , (SELECT COUNT(Task_ID) FROM TTD_Task Sub INNER JOIN TTD_Class SubCls ON Sub.Class_ID = SubCls.Class_ID");
                SBSql.AppendLine("        WHERE (Sub.Complete_Time IS NULL) AND (SubCls.DeptID = Dept.DeptID)");

                #region "查詢條件"
                if (Check_TTDAuth(fn_Params.UserAccount, out ErrMsg))
                {
                    SBSql.Append(" AND (Sub.Create_Who IN ( ");
                    SBSql.Append("  SELECT Auth.View_Account FROM TTD_ViewAuth Auth WHERE Auth.Account_Name = @Param_Who ");
                    SBSql.Append(" ))");
                }
                else
                {
                    SBSql.Append(" AND (Sub.Create_Who = @Param_Who) ");
                }
                //[查詢條件] - 開始日期
                if (false == string.IsNullOrEmpty(searchDate))
                {
                    SBSql.Append(" AND (Sub.Create_Time >= @StartDate) ");
                }
                //[查詢條件] - 結束日期
                if (false == string.IsNullOrEmpty(searchDate))
                {
                    SBSql.Append(" AND (Sub.Create_Time <= @EndDate) ");
                }
                #endregion

                SBSql.AppendLine("    ) AS unCompleted");
                //計數 - 已完成
                SBSql.AppendLine("    , (SELECT COUNT(Task_ID) FROM TTD_Task Sub INNER JOIN TTD_Class SubCls ON Sub.Class_ID = SubCls.Class_ID");
                SBSql.AppendLine("        WHERE (Sub.Complete_Time IS NOT NULL) AND (SubCls.DeptID = Dept.DeptID)");

                #region "查詢條件"
                if (Check_TTDAuth(fn_Params.UserAccount, out ErrMsg))
                {
                    SBSql.Append(" AND (Sub.Create_Who IN ( ");
                    SBSql.Append("  SELECT Auth.View_Account FROM TTD_ViewAuth Auth WHERE Auth.Account_Name = @Param_Who ");
                    SBSql.Append(" ))");
                }
                else
                {
                    SBSql.Append(" AND (Sub.Create_Who = @Param_Who) ");
                }
                //[查詢條件] - 開始日期
                if (false == string.IsNullOrEmpty(searchDate))
                {
                    SBSql.Append(" AND (Sub.Create_Time >= @StartDate) ");
                }
                //[查詢條件] - 結束日期
                if (false == string.IsNullOrEmpty(searchDate))
                {
                    SBSql.Append(" AND (Sub.Create_Time <= @EndDate) ");
                }
                #endregion

                SBSql.AppendLine("    ) AS Completed");
                SBSql.AppendLine(" FROM TTD_Task Task INNER JOIN TTD_Class Cls ON Task.Class_ID = Cls.Class_ID");
                SBSql.AppendLine("   INNER JOIN PKSYS.dbo.User_Dept Dept ON Cls.DeptID = Dept.DeptID");
                SBSql.AppendLine(" WHERE (1=1) ");

                #region "查詢條件"
                if (Check_TTDAuth(fn_Params.UserAccount, out ErrMsg))
                {
                    SBSql.Append(" AND (Task.Create_Who IN ( ");
                    SBSql.Append("  SELECT Auth.View_Account FROM TTD_ViewAuth Auth WHERE Auth.Account_Name = @Param_Who ");
                    SBSql.Append(" ))");
                }
                else
                {
                    SBSql.Append(" AND (Task.Create_Who = @Param_Who) ");
                }
                //[查詢條件] - 開始日期
                if (false == string.IsNullOrEmpty(searchDate))
                {
                    SBSql.Append(" AND (Task.Create_Time >= @StartDate) ");
                }
                //[查詢條件] - 結束日期
                if (false == string.IsNullOrEmpty(searchDate))
                {
                    SBSql.Append(" AND (Task.Create_Time <= @EndDate) ");
                }
                #endregion

                SBSql.AppendLine(" GROUP BY Dept.DeptID, Dept.Area, Dept.DeptName ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Who", fn_Params.UserAccount);
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", searchDate));
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", searchDate));
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 取得資料列表！", "");
        }
    }

    /// <summary>
    /// 判斷讀取權限
    /// </summary>
    /// <param name="UserID">工號</param>
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

    #endregion

    #region -- 前端頁面控制 --
    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("WR_Summary.aspx?func=summary");

            //[查詢條件] - SearchDate
            if (string.IsNullOrEmpty(this.tb_ShowDT.Text) == false)
            {
                SBUrl.Append("&SearchDate=" + Server.UrlEncode(this.tb_ShowDT.Text));
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
    /// 7 日內明細
    /// </summary>
    protected void btn_Sh1_Click(object sender, EventArgs e)
    {
        try
        {
            string StartDate = DateTime.Now.AddDays(-7).ToShortDateString().ToDateString("yyyy/MM/dd");
            string EndDate = DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd");
            Response.Redirect(string.Format("WR_Search.aspx?StartDate={0}&EndDate={1}"
                , Server.UrlEncode(StartDate)
                , Server.UrlEncode(EndDate)
                ));
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 7 日內明細！", "");
            return;
        }

    }

    /// <summary>
    /// 30 日內明細
    /// </summary>
    protected void btn_Sh2_Click(object sender, EventArgs e)
    {
        try
        {
            string StartDate = DateTime.Now.AddDays(-30).ToShortDateString().ToDateString("yyyy/MM/dd");
            string EndDate = DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd");
            Response.Redirect(string.Format("WR_Search.aspx?StartDate={0}&EndDate={1}"
                , Server.UrlEncode(StartDate)
                , Server.UrlEncode(EndDate)
                ));
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 30 日內明細！", "");
            return;
        }
    }
    #endregion

}