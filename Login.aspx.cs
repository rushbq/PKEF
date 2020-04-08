using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void lbtn_Login_Click(object sender, EventArgs e)
    {
        try
        {
            //檢查驗證碼
            if (this.tb_VerifyCode.Text.ToUpper().Equals(Session["ImgCheckCode"]) == false)
            {
                string js = "alert('「驗證碼」輸入錯誤！');";
                ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
                return;
            }

            #region ***AD帳戶驗證***
            //[設定參數] - LDAP路徑
            string adPath = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_Path"];
            //[設定參數] - 網域名稱
            string domainName = "prokits";

            LdapAuthentication adAuth = new LdapAuthentication(adPath);

            //[取得參數] - 使用者帳密
            string UserName = this.tb_UserID.Text.Trim();
            string UserPwd = this.tb_UserPwd.Text.Trim();
            //[AD驗證]
            if (true == adAuth.IsAuthenticated(domainName, UserName, UserPwd))
            {
                string SID = adAuth.GetSID;
                if (string.IsNullOrEmpty(SID))
                {
                    string js = "alert('登入失敗 - 請確認帳號或密碼是否正確！');";
                    ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
                    return;
                }

                //[暫存參數] - 新增Cookie, 存入SID(設定過期時間為 4 小時)
                Response.Cookies.Add(new HttpCookie("PKEF_UserSID", SID));
                Response.Cookies["PKEF_UserSID"].Expires = DateTime.Now.AddHours(4);

                //登入成功，導向首頁
                Response.Redirect("Default.aspx");

            }
            else
            {
                string js = "alert('登入失敗 - 請確認帳號或密碼是否正確！');";
                ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
                return;
            }
            #endregion
        }

        catch (Exception ex)
        {
            this.lt_ErrMsg.Text = "<p style=\"color: Red\">* 錯誤訊息：" + ex.Message.ToString() + "</p>";
            string js = "alert('登入失敗！');";
            ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
            return;
        }
    }

    /// <summary>
    /// ValidCode
    /// </summary>
    private string _Param_ValidCode;
    public string Param_ValidCode
    {
        get;
        private set;
    }
}