using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;


public partial class Top : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //try
            //{
            //    string ErrMsg = "";
            //    //[產生選單] - 語系
            //    if (fn_Extensions.LangMenu(this.ddl_Language, out ErrMsg) == false)
            //    {
            //        this.ddl_Language.Items.Clear();
            //    }
            //    //[判斷&取得參數] - 語系
            //    this.ddl_Language.SelectedValue = fn_Language.PKEF_Lang;
            //}
            //catch (Exception)
            //{
            //    string js = "alert('語系選單產生失敗！');";
            //    ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
            //    return;
            //}
        }

    }

    //[變更語系]
    //protected void ddl_Language_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (string.IsNullOrEmpty(ddl_Language.SelectedValue) == false)
    //    {
    //        //新增語系Cookies
    //        Response.Cookies.Remove("PKEF_Lang");
    //        Response.Cookies.Add(new HttpCookie("PKEF_Lang", ddl_Language.SelectedValue));
    //        Response.Cookies["PKEF_Lang"].Expires = DateTime.Now.AddYears(1);
    //    }

    //    string js = "top.location.replace('Default.aspx');";
    //    ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
    //    return;
    //}

    /// <summary>
    /// 登出 
    /// </summary>
    protected void lbtn_Logout_Click(object sender, EventArgs e)
    {
        //清除Cookie
        if (Request.Cookies["PKEF_UserSID"] != null)
        {
            HttpCookie myCookie = new HttpCookie("PKEF_UserSID");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
        }

        //清除Session
        Session.Clear();
        Session.Abandon();

        string js = "top.location.replace('Login.aspx');";
        ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
        return;
    }

}