using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;

namespace LogRecord
{
    /// <summary>
    /// Log記錄
    /// </summary>
    public class fn_Log
    {
        /// <summary>
        /// 寫入Log
        /// </summary>
        /// <param name="ProcType">類別(20)</param>
        /// <param name="ProcAction">處理動作(50)</param>
        /// <param name="ProcDesc">處理描述(255)</param>
        /// <param name="CreateWho">處理者(Guid)</param>
        /// <returns>bool</returns>
        public static bool Log_Rec(string ProcType, string ProcAction, string ProcDesc, string CreateWho)
        {
            //[初始化]
            string ErrMsg = "";
            try
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[SQL] - 宣告New ID
                    SBSql.AppendLine(" Declare @Log_ID AS INT ");
                    //[SQL] - 寫入Log
                    SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_Record) ");
                    SBSql.AppendLine(" INSERT INTO Log_Record( ");
                    SBSql.AppendLine("  Log_ID, Proc_Time, Proc_Type, Proc_Action, Proc_Desc, Create_Who");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Log_ID, GETDATE(), @ProcType, @ProcAction, @ProcDesc, @CreateWho");
                    SBSql.AppendLine(" )");
                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("ProcType", ProcType);
                    cmd.Parameters.AddWithValue("ProcAction", ProcAction);
                    cmd.Parameters.AddWithValue("ProcDesc", ProcDesc);
                    cmd.Parameters.AddWithValue("CreateWho", CreateWho);

                    //[執行SQL]
                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }

        }

        /// <summary>
        /// 寫入AD處理Log
        /// </summary>
        /// <param name="ProcType">類別 (User / Group)</param>
        /// <param name="ProcAction">處理動作</param>
        /// <param name="ProcAccount">處理的帳戶</param>
        /// <param name="ProcDesc">處理描述</param>
        /// <param name="CreateWho">處理者</param>
        /// <returns>bool</returns>
        public static bool Log_AD(string ProcType, string ProcAction, string ProcAccount
            , string ProcDesc, string CreateWho)
        {
            //[初始化]
            string ErrMsg = "";
            try
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[SQL] - 宣告New ID
                    SBSql.AppendLine(" Declare @Log_ID AS INT ");
                    //[SQL] - 寫入Log
                    SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_AD) ");
                    SBSql.AppendLine(" INSERT INTO Log_AD( ");
                    SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Log_ID, @ProcType, @ProcAction, @ProcAccount, @ProcDesc, @CreateWho");
                    SBSql.AppendLine(" )");
                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("ProcType", ProcType);
                    cmd.Parameters.AddWithValue("ProcAction", ProcAction);
                    cmd.Parameters.AddWithValue("ProcAccount", ProcAccount);
                    cmd.Parameters.AddWithValue("ProcDesc", ProcDesc);
                    cmd.Parameters.AddWithValue("CreateWho", CreateWho);

                    //[執行SQL]
                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }

        }

        /// <summary>
        /// 寫入AD處理Log
        /// </summary>
        /// <param name="ProcType">類別 (User / Group)</param>
        /// <param name="ProcAction">處理動作</param>
        /// <param name="ProcAccount">處理的帳戶</param>
        /// <param name="ProcDesc">處理描述</param>
        /// <param name="CreateWho">處理者</param>
        /// <param name="iProgID_Old">原權限ID</param>
        /// <param name="iProgID_New">新權限ID</param>
        /// <param name="AD_Guid">處理的帳戶GUID</param>
        /// <returns>bool</returns>
        public static bool Log_AD_withAuth(string ProcType, string ProcAction, string ProcAccount
            , string ProcDesc, string CreateWho
            , List<string> iProgID_Old, List<string> iProgID_New, string AD_Guid)
        {
            //[初始化]
            string ErrMsg = "";
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[SQL] - 宣告New ID
                    SBSql.AppendLine(" Declare @Log_ID AS INT ");
                    //[SQL] - 寫入Log主檔
                    SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_AD) ");
                    SBSql.AppendLine(" INSERT INTO Log_AD( ");
                    SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Log_ID, @ProcType, @ProcAction, @ProcAccount, @ProcDesc, @CreateWho");
                    SBSql.AppendLine(" ); ");

                    //[SQL] - 判斷來源Log
                    string AuthTable = "";
                    switch (ProcType)
                    {
                        case "User":
                            AuthTable = "Log_User_Profile_Rel_Program";
                            break;

                        case "Group":
                            AuthTable = "Log_User_Group_Rel_Program";
                            break;
                    }
                    if (string.IsNullOrEmpty(AuthTable) == false)
                    {
                        //[SQL] - 寫入Log權限檔(原權限)
                        if (iProgID_Old != null)
                        {
                            for (int i = 0; i < iProgID_Old.Count; i++)
                            {
                                SBSql.AppendLine(string.Format(
                                    " INSERT INTO {0} (Log_ID, LogType, Guid, Prog_ID) VALUES (@Log_ID, 'Old', '{1}', {2});"
                                    , AuthTable, AD_Guid, iProgID_Old[i].ToString()));
                            }
                        }

                        //[SQL] - 寫入Log權限檔(新權限)
                        if (iProgID_New != null)
                        {
                            for (int i = 0; i < iProgID_New.Count; i++)
                            {
                                SBSql.AppendLine(string.Format(
                                    " INSERT INTO {0} (Log_ID, LogType, Guid, Prog_ID) VALUES (@Log_ID, 'New', '{1}', {2});"
                                    , AuthTable, AD_Guid, iProgID_New[i].ToString()));
                            }
                        }
                    }

                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("ProcType", ProcType);
                    cmd.Parameters.AddWithValue("ProcAction", ProcAction);
                    cmd.Parameters.AddWithValue("ProcAccount", ProcAccount);
                    cmd.Parameters.AddWithValue("ProcDesc", ProcDesc);
                    cmd.Parameters.AddWithValue("CreateWho", CreateWho);

                    //[執行SQL]
                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }

        }

      
    }
}
