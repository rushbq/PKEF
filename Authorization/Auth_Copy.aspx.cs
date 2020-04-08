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
using System.Collections;
using LogRecord;

public partial class Auth_Copy : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";

            //[權限判斷] - 權限複製
            if (fn_CheckAuth.CheckAuth_User("9904", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }
        }
    }

    #region --按鈕區--
    //群組權限複製
    protected void btn_CopyGroup_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            //暫存參數
            this.lb_Group_Name_from.Text = this.tb_Group_Name_from.Text;
            this.lb_Group_Name_to.Text = this.tb_Group_Name_to.Text;

            //[判斷參數] - 群組
            if (string.IsNullOrEmpty(this.tb_Group_ID_from.Text) || string.IsNullOrEmpty(this.tb_Group_ID_to.Text))
            {
                fn_Extensions.JsAlert("群組選擇不正確！", "");
                return;
            }
            if (this.tb_Group_ID_from.Text.Trim() == this.tb_Group_ID_to.Text.Trim())
            {
                fn_Extensions.JsAlert("複製的群組不可相同！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 執行SQL
                StringBuilder SBSql = new StringBuilder();

                #region "Log處理"
                //[暫存參數] - 原權限ID
                List<string> iProgID_Old = new List<string>();

                //[SQL] - (LOG) 取得原權限ID
                SBSql.Clear();
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Group_Rel_Program WHERE (Guid = @To_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("To_Guid", this.tb_Group_ID_to.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        iProgID_Old.Add(DT.Rows[i]["Prog_ID"].ToString());
                    }
                }

                //[暫存參數] - 新權限ID
                List<string> iProgID_New = new List<string>();
                //[SQL] - (LOG) 取得新權限ID
                SBSql.Clear();
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Group_Rel_Program WHERE (Guid = @From_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("From_Guid", this.tb_Group_ID_from.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        iProgID_New.Add(DT.Rows[i]["Prog_ID"].ToString());
                    }
                }
                #endregion

                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();
                SBSql.Clear();
                //[SQL] - 刪除目標權限
                SBSql.AppendLine(" DELETE FROM User_Group_Rel_Program WHERE (Guid = @To_Guid); ");
                //[SQL] - 複製來源權限
                SBSql.AppendLine(" INSERT INTO User_Group_Rel_Program (Guid, Prog_ID) ");
                SBSql.AppendLine(" SELECT @To_Guid, Prog_ID ");
                SBSql.AppendLine(" FROM User_Group_Rel_Program ");
                SBSql.AppendLine(" WHERE (Guid = @From_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("To_Guid", this.tb_Group_ID_to.Text);
                cmd.Parameters.AddWithValue("From_Guid", this.tb_Group_ID_from.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    this.lt_Result_Group.Text = "群組權限複製失敗";
                    return;
                }
                else
                {
                    //寫入Log
                    fn_Log.Log_AD_withAuth("Group", "複製權限", this.lb_Group_Name_from.Text
                        , "複製 " + this.lb_Group_Name_from.Text + " 的使用權限給 " + this.lb_Group_Name_to.Text
                        , fn_Params.UserAccount
                        , iProgID_Old, iProgID_New, this.tb_Group_ID_to.Text);

                    this.lt_Result_Group.Text = string.Format(
                        "群組權限複製成功，<a href=\"{0}\">前往檢視詳細權限</a>"
                        , "Auth_SetGroup.aspx?GroupID=" + HttpUtility.UrlEncode(this.tb_Group_ID_to.Text));
                    return;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 群組權限複製！", "");
        }
    }

    //使用者權限複製
    protected void btn_CopyProfile_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            //暫存參數
            this.lb_Profile_Name_from.Text = this.tb_Profile_Name_from.Text;
            this.lb_Profile_Name_to.Text = this.tb_Profile_Name_to.Text;

            //[判斷參數] - 使用者
            if (string.IsNullOrEmpty(this.tb_Profile_ID_from.Text) || string.IsNullOrEmpty(this.tb_Profile_ID_to.Text))
            {
                fn_Extensions.JsAlert("使用者選擇不正確！", "");
                return;
            }
            if (this.tb_Profile_ID_from.Text.Trim() == this.tb_Profile_ID_to.Text.Trim())
            {
                fn_Extensions.JsAlert("複製的使用者不可相同！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 執行SQL
                StringBuilder SBSql = new StringBuilder();

                #region "Log處理"
                //[暫存參數] - 原權限ID
                List<string> iProgID_Old = new List<string>();

                //[SQL] - (LOG) 取得原權限ID
                SBSql.Clear();
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Profile_Rel_Program WHERE (Guid = @To_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("To_Guid", this.tb_Profile_ID_to.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        iProgID_Old.Add(DT.Rows[i]["Prog_ID"].ToString());
                    }
                }

                //[暫存參數] - 新權限ID
                List<string> iProgID_New = new List<string>();
                //[SQL] - (LOG) 取得新權限ID
                SBSql.Clear();
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Profile_Rel_Program WHERE (Guid = @From_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("From_Guid", this.tb_Profile_ID_from.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        iProgID_New.Add(DT.Rows[i]["Prog_ID"].ToString());
                    }
                }
                #endregion

                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();
                SBSql.Clear();
                //[SQL] - 刪除目標權限
                SBSql.AppendLine(" DELETE FROM User_Profile_Rel_Program WHERE (Guid = @To_Guid); ");
                //[SQL] - 複製來源權限
                SBSql.AppendLine(" INSERT INTO User_Profile_Rel_Program (Guid, Prog_ID) ");
                SBSql.AppendLine(" SELECT @To_Guid, Prog_ID ");
                SBSql.AppendLine(" FROM User_Profile_Rel_Program ");
                SBSql.AppendLine(" WHERE (Guid = @From_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("To_Guid", this.tb_Profile_ID_to.Text);
                cmd.Parameters.AddWithValue("From_Guid", this.tb_Profile_ID_from.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    this.lt_Result_Profile.Text = "使用者權限複製失敗";
                    return;
                }
                else
                {
                    //寫入Log
                    fn_Log.Log_AD_withAuth("User", "複製權限", this.lb_Profile_Name_to.Text
                        , "複製 " + this.lb_Profile_Name_from.Text + " 的使用權限給 " + this.lb_Profile_Name_to.Text
                        , fn_Params.UserAccount
                        , iProgID_Old, iProgID_New, this.tb_Profile_ID_to.Text);

                    this.lt_Result_Profile.Text = string.Format(
                        "使用者權限複製成功，<a href=\"{0}\">前往檢視詳細權限</a>"
                        , "Auth_SetUser.aspx?ProfileID=" + HttpUtility.UrlEncode(this.tb_Profile_ID_to.Text));
                    return;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 使用者權限複製！", "");
        }
    }

    #endregion
}
