using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;

public partial class mySZBBC_ImportIndex : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("810", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    private string NewTraceID()
    {
        //產生TraceID
        long ts = Cryptograph.GetCurrentTime();

        Random rnd = new Random();
        int myRnd = rnd.Next(1, 99);

        return "{0}{1}".FormatThis(ts, myRnd);
    }

    /// <summary>
    /// 訂單
    /// </summary>
    protected void lbtn_link1_Click(object sender, EventArgs e)
    {
        string url = "{0}mySZBBC/ImportStep1.aspx?ts={1}&type=1".FormatThis(
             fn_Params.WebUrl
             , Cryptograph.MD5Encrypt(NewTraceID(), System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"])
            );

        Response.Redirect(url);
    }


    /// <summary>
    /// 退貨單
    /// </summary>
    protected void lbtn_link2_Click(object sender, EventArgs e)
    {
        string url = "{0}mySZBBC/ImportStep1.aspx?ts={1}&type=2".FormatThis(
             fn_Params.WebUrl
             , Cryptograph.MD5Encrypt(NewTraceID(), System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"])
            );

        Response.Redirect(url);
    }
}