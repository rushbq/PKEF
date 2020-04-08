using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;

public partial class WorkReport_WR_Write_Ajax : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[驗證] - MD5是否相同
                if (Request.Form["ValidCode"] == null)
                {
                    Response.Write("驗證失敗!");
                    return;
                }
                if (!Request.Form["ValidCode"].Equals(ValidCode))
                {
                    Response.Write("驗證失敗!");
                    return;
                }

                //[取得callback資料]
                if (string.IsNullOrEmpty(Request["taskID"]) || string.IsNullOrEmpty(Request["isChecked"]))
                {
                    Response.Write("error");
                    return;
                }
                //[取得參數值]
                string taskID = Request["taskID"].ToString();   //工作編號
                string isChecked = Request["isChecked"].ToString().ToUpper();     //是否為選取完成(Y/N)
                string ErrMsg;

                //[SQL] - 資料新增/更新
                using (SqlCommand cmd = new SqlCommand())
                {
                    //[清除參數]
                    cmd.Parameters.Clear();

                    StringBuilder SBSql = new StringBuilder();

                    switch (isChecked) { 
                        case "Y":
                            SBSql.AppendLine(" UPDATE TTD_Task SET Update_Time = GETDATE(), Complete_Time = GETDATE() ");
                            SBSql.AppendLine(" WHERE (Task_ID = @Task_ID) AND (Create_Who = @Create_Who) ");
                            break;

                        case "N":
                            SBSql.AppendLine(" UPDATE TTD_Task SET Update_Time = GETDATE(), Complete_Time = NULL ");
                            SBSql.AppendLine(" WHERE (Task_ID = @Task_ID) AND (Create_Who = @Create_Who) ");
                            break;
                    }
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Task_ID", taskID);
                    cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserAccount);
                    if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                    {
                        Response.Write("error:" + ErrMsg);
                        return;
                    }
                    else
                    {
                        Response.Write("OK" + isChecked);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("error:" + ex.Message.ToString());
                return;
            }
        }
    }


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