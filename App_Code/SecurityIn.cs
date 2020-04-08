using System;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Security.Principal;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Text;
using System.Data;

/// <summary>
/// [建立]
///  功能:Session檢查
///  日期:2012-07-23
///  人員:Clyde
///  說明:
///   檢查Session是否過期，重新取得登入資訊
/// </summary>
public class SecurityIn : System.Web.UI.Page
{
    protected override void OnLoad(System.EventArgs e)
    {
        try
        {
            //[檢查參數] Session是否已過期
            if (UnobtrusiveSession.Session["Login_UserID"] == null)
            {
                //清除Session
                Session.Clear();

                if ((Request.Cookies["PKEF_UserSID"] == null))
                {
                    //AD登入驗證 - 網域電腦登入後自動驗證
                    CheckAD_Auto();
                }
                else
                {
                    //AD登入驗證 - 手動輸入帳密
                    CheckAD_Input(Request.Cookies["PKEF_UserSID"].Value.ToString());
                }


                base.OnLoad(e);
            }
            else
            {
                base.OnLoad(e);
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message.ToString());
            return;
        }

    }

    /// <summary>
    /// AD登入驗證 - 網域電腦登入後自動驗證
    /// </summary>
    private void CheckAD_Auto()
    {
        //取得登入相關資訊
        IPrincipal userPrincipal = HttpContext.Current.User;
        WindowsIdentity windowsId = userPrincipal.Identity as WindowsIdentity;
        if (windowsId == null)
        {
            //找不到此SID, 導向登入錯誤頁
            Response.Write(ErrPage("請先登入網域"));
            return;
        }
        else
        {
            SecurityIdentifier sid = windowsId.User;
            //取得屬性值(Sid / DisplayName / AccountName / Guid / 帳戶類型)
            StringCollection listAttr = ADService.getAttributesFromSID(sid.Value);
            if (listAttr == null)
            {
                //找不到此SID, 導向登入錯誤頁
                Response.Write(ErrPage("帳號未建立或未登入網域"));
                return;
            }
            else
            {
                //取得登入名稱
                UnobtrusiveSession.Session["Login_UserName"] = listAttr[1];
                //取得登入帳號
                UnobtrusiveSession.Session["Login_UserID"] = listAttr[2];
                //取得AD GUID
                UnobtrusiveSession.Session["Login_GUID"] = listAttr[3];
                ////取得登入者所屬群組(ArrayList)
                //Guid objectGuid = new Guid(listAttr[3]);
                //ArrayList aryGroup = ADService.getGroupGUIDFromGUID(objectGuid);
                //HttpContext.Current.Session["Login_UserGroups"] = aryGroup;
            }
        }
    }

    /// <summary>
    /// AD登入驗證 - 手動輸入帳密
    /// </summary>
    private void CheckAD_Input(string SID)
    {
        //取得登入相關資訊
        if (string.IsNullOrEmpty(SID))
        {
            //找不到此SID, 導向登入錯誤頁
            Response.Write(ErrPage("請先登入網域"));
            return;
        }
        else
        {
            //取得屬性值(Sid / DisplayName / AccountName / Guid / 帳戶類型)
            StringCollection listAttr = ADService.getAttributesFromSID(SID);
            if (listAttr == null)
            {
                //找不到此SID, 導向登入錯誤頁
                Response.Write(ErrPage("帳號未建立或未登入網域"));
                return;
            }
            else
            {
                //取得登入名稱
                HttpContext.Current.Session["Login_UserName"] = listAttr[1];
                //取得登入帳號
                HttpContext.Current.Session["Login_UserID"] = listAttr[2];
                //取得AD GUID
                HttpContext.Current.Session["Login_GUID"] = listAttr[3];
                //取得登入者所屬群組(ArrayList)
                Guid objectGuid = new Guid(listAttr[3]);
                ArrayList aryGroup = ADService.getGroupGUIDFromGUID(objectGuid);
                HttpContext.Current.Session["Login_UserGroups"] = aryGroup;

                //取得部門參數
                Get_DeptAttr(listAttr[3]);
            }
        }
    }

    /// <summary>
    /// 取得 & 設定部門參數
    /// </summary>
    /// <param name="LoginGuid">AD Guid</param>
    private void Get_DeptAttr(string LoginGuid)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT User_Dept.Area ");
                SBSql.AppendLine(" FROM User_Profile INNER JOIN User_Dept ON User_Profile.DeptID = User_Dept.DeptID ");
                SBSql.AppendLine(" WHERE (User_Profile.Guid = @LoginGuid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("LoginGuid", LoginGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        HttpContext.Current.Session["Login_Area"] = null;
                        return;
                    }
                    //取得登入地區
                    HttpContext.Current.Session["Login_Area"] = DT.Rows[0]["Area"].ToString();
                }
            }
        }
        catch (Exception)
        {

            throw new Exception("取得部門參數失敗");
        }
    }

    string ErrPage(string ErrMsg)
    {
        return string.Format("<script>parent.location.href='{0}Login_Fail.aspx?ErrMsg={1}';</script>"
                             , System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"]
                             , Server.UrlEncode(ErrMsg));
    }
}