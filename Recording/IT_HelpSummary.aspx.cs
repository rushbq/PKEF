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
using LogRecord;

public partial class IT_HelpSummary : SecurityIn
{
  
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 資訊需求統計
                if (fn_CheckAuth.CheckAuth_User("9813", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
               
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
            return;
        }
    }


}