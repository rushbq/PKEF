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
using System.Web.UI.HtmlControls;

public partial class WR_Write : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 填寫日誌
                if (fn_CheckAuth.CheckAuth_User("320", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[帶出資料]
                LookupDataList();

                //帶出日期
                this.lb_showToday.Text = string.Format("{0} ({1})"
                    , DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd")
                    , DateTime.Now.ToString("ddd", new System.Globalization.CultureInfo("zh-tw")).Right(1)
                    );
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
    /// 副程式 - 取得資料列表 
    /// </summary>
    private void LookupDataList()
    {
        string ErrMsg;
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 資料查詢 (帶出自己的清單)
                StringBuilder SBSql = new StringBuilder();

                //--未完成事項(不含今日)
                SBSql.AppendLine("SELECT Tbl.* FROM ( ");
                SBSql.AppendLine("  SELECT Task.Task_ID, Task.Task_Name, Task.Remark, Task.Create_Time, Task.Complete_Time ");
                SBSql.AppendLine("      , Cls.Class_Name, 'Past' AS TaskType, 1 AS Sort ");
                SBSql.AppendLine("  FROM TTD_Task Task INNER JOIN TTD_Class Cls ON Task.Class_ID = Cls.Class_ID ");
                SBSql.AppendLine("  WHERE (Task.Create_Who = @Create_Who) AND (Task.Create_Time < @BgTime) AND (Task.Complete_Time IS NULL) ");
                SBSql.AppendLine("  UNION ALL ");
                //--今日所有事項
                SBSql.AppendLine("  SELECT Task.Task_ID, Task.Task_Name, Task.Remark, Task.Create_Time, Task.Complete_Time ");
                SBSql.AppendLine("      , Cls.Class_Name, 'Now' AS TaskType, 2 AS Sort ");
                SBSql.AppendLine("  FROM TTD_Task Task INNER JOIN TTD_Class Cls ON Task.Class_ID = Cls.Class_ID ");
                SBSql.AppendLine("  WHERE (Task.Create_Who = @Create_Who) AND (Task.Create_Time >= @BgTime) AND (Task.Create_Time <= @EdTime) ");
                SBSql.AppendLine("  UNION ALL ");
                //--未來所有事項
                SBSql.AppendLine("  SELECT Task.Task_ID, Task.Task_Name, Task.Remark, Task.Create_Time, Task.Complete_Time ");
                SBSql.AppendLine("      , Cls.Class_Name, 'Future' AS TaskType, 3 AS Sort ");
                SBSql.AppendLine("  FROM TTD_Task Task INNER JOIN TTD_Class Cls ON Task.Class_ID = Cls.Class_ID ");
                SBSql.AppendLine("  WHERE (Task.Create_Who = @Create_Who) AND (Task.Create_Time > @EdTime) ");
                SBSql.AppendLine(" ) AS Tbl ");
                SBSql.AppendLine(" ORDER BY Tbl.Sort, Tbl.Complete_Time, Tbl.Create_Time ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserAccount);
                cmd.Parameters.AddWithValue("BgTime", DateTime.Now.ToString().ToDateString("yyyy/MM/dd 00:00:00"));
                cmd.Parameters.AddWithValue("EdTime", DateTime.Now.ToString().ToDateString("yyyy/MM/dd 23:59:59"));
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

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //Checkbox 新增屬性
            CheckBox cb_Item = (CheckBox)e.Item.FindControl("cb_Confirm");
            cb_Item.InputAttributes["value"] = DataBinder.Eval(dataItem.DataItem, "Task_ID").ToString();

            //符號控制項
            Label lb_MsgIcon = (Label)e.Item.FindControl("lb_MsgIcon");

            //判斷是否為過去的項目
            if (DataBinder.Eval(dataItem.DataItem, "TaskType").ToString().Equals("Past"))
            {
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trItem");
                trRow.Attributes["class"] = "TrOrange";

                //顯示日期
                Label lb_pastTime = (Label)e.Item.FindControl("lb_pastTime");
                lb_pastTime.Text = " - " + DataBinder.Eval(dataItem.DataItem, "Create_Time").ToString().ToDateString("MM/dd");

                //顯示符號
                lb_MsgIcon.Text = "<span class=\"JQ-ui-state-error\"><span class=\"JQ-ui-icon ui-icon-notice\"></span></span>";
            }

            //判斷是否為未來的項目
            if (DataBinder.Eval(dataItem.DataItem, "TaskType").ToString().Equals("Future"))
            {
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trItem");
                trRow.Attributes["class"] = "TrBlue";

                //顯示日期
                Label lb_pastTime = (Label)e.Item.FindControl("lb_pastTime");
                lb_pastTime.Text = " - " + DataBinder.Eval(dataItem.DataItem, "Create_Time").ToString().ToDateString("MM/dd");

                //顯示符號
                lb_MsgIcon.Text = "<span class=\"JQ-ui-state-default\"><span class=\"JQ-ui-icon ui-icon-clock\"></span></span>";

                //鎖定核選方塊
                cb_Item.Enabled = false;
            }

            //判斷今日事項是否已完成
            if (!string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "Complete_Time").ToString()))
            {
                HtmlTableRow trRow = (HtmlTableRow)e.Item.FindControl("trItem");
                trRow.Attributes["class"] = "TrYellow";
                trRow.Attributes["style"] = "text-decoration:line-through";

                //將checkbox勾選
                cb_Item.Checked = true;

                //顯示符號
                lb_MsgIcon.Text = "<span class=\"JQ-ui-state-highlight\"><span class=\"JQ-ui-icon ui-icon-check\"></span></span>";
            }
        }
    }
    #endregion

    /// <summary>
    /// 產生MD5驗証碼
    /// SessionID + 登入帳號 + 自訂字串
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get { return Cryptograph.MD5(Session.SessionID + Session["Login_UserID"] + System.Web.Configuration.WebConfigurationManager.AppSettings["ValidCode_Pwd"]); }
        private set { this._ValidCode = value; }
    }
}