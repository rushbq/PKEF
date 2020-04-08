using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

/// <summary>
/// 權限處理
/// </summary>
/// <remarks>
/// 判斷個人權限是否存在
///   -否:前往群組權限
///   -是:
///     1.判斷帳戶是否停用
///     2.判斷權限編號是否正常設定
/// </remarks>
public class fn_CheckAuth
{
    #region -- 權限檢查 --
    /// <summary>
    /// 權限檢查
    /// </summary>
    /// <param name="authProgID">欲判斷的權限編號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    /// <remarks>
    /// 先判斷是否有個人權限, 若沒有才檢查群組權限
    /// </remarks>
    public static bool CheckAuth_User(string authProgID, out string ErrMsg)
    {
        try
        {
            //取得個人Guid
            string tmpGuid = fn_Params.UserGuid;
            if (string.IsNullOrEmpty(tmpGuid))
            {
                ErrMsg = "無法取得個人參數，請聯絡系統管理員!";
                return false;
            }
            //取得個人帳號
            string tmpAccount = fn_Params.UserAccount;
            if (string.IsNullOrEmpty(tmpAccount))
            {
                ErrMsg = "無法取得個人參數，請聯絡系統管理員!";
                return false;
            }

            //判斷是否有個人權限
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Guid, Prog_ID ");
                sbSQL.AppendLine(" FROM User_Profile_Rel_Program WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Prog_ID = @Prog_ID) AND (Guid = @Guid) ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.AddWithValue("Prog_ID", authProgID);
                cmd.Parameters.AddWithValue("Guid", tmpGuid);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //未建立個人權限，前往取得部門權限
                        //return CheckAuth_Group(authProgID, out ErrMsg);
                        return false;
                    }
                    else
                    {
                        ErrMsg = "";
                        return true;

                    }
                }
            }

        }
        catch (Exception)
        {
            ErrMsg = "權限判斷發生錯誤，請聯絡系統管理員!";
            return false;
        }
    }

    ///// <summary>
    ///// [權限判斷] - 群組
    ///// </summary>
    ///// <param name="tmpGuid">所屬群組的GUID</param>
    ///// <param name="authProgID">欲判斷的權限編號</param>
    ///// <param name="ErrMsg">錯誤訊息</param>
    ///// <returns>bool</returns>
    //private static bool CheckAuth_Group(string authProgID, out string ErrMsg)
    //{
    //    try
    //    {
    //        //取得所屬群組Guid
    //        ArrayList tmpGuid = (ArrayList)HttpContext.Current.Session["Login_UserGroups"];
    //        if (tmpGuid == null)
    //        {
    //            ErrMsg = "無法取得所屬群組，請聯絡系統管理員!";
    //            return false;
    //        }


    //        using (SqlCommand cmd = new SqlCommand())
    //        {
    //            StringBuilder sbSQL = new StringBuilder();
    //            cmd.Parameters.Clear();

    //            //[SQL] - 資料查詢
    //            sbSQL.AppendLine(" SELECT Guid, Prog_ID ");
    //            sbSQL.AppendLine(" FROM User_Group_Rel_Program WITH (NOLOCK) ");
    //            sbSQL.AppendLine(" WHERE (Prog_ID = @Prog_ID) ");

    //            #region >>群組參數組合<<
    //            //[SQL] - 暫存參數
    //            string tempParam = "";
    //            for (int row = 0; row < tmpGuid.Count; row++)
    //            {
    //                if (string.IsNullOrEmpty(tempParam) == false) { tempParam += ","; }
    //                tempParam += "@ParamTmp" + row;
    //            }
    //            //[SQL] - 代入暫存參數
    //            sbSQL.AppendLine(" AND (Guid IN (" + tempParam + "))");
    //            for (int row = 0; row < tmpGuid.Count; row++)
    //            {
    //                cmd.Parameters.AddWithValue("ParamTmp" + row, tmpGuid[row]);
    //            }
    //            #endregion

    //            //[SQL] - Command
    //            cmd.CommandText = sbSQL.ToString();
    //            cmd.Parameters.AddWithValue("Prog_ID", authProgID);

    //            //取得資料
    //            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
    //            {
    //                if (DT.Rows.Count == 0)
    //                {
    //                    ErrMsg = "所屬群組權限不足!";
    //                    return false;
    //                }
    //                else
    //                {
    //                    ErrMsg = "";
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        ErrMsg = "權限判斷發生錯誤，請聯絡系統管理員!";
    //        return false;
    //    }
    //}
    #endregion

}