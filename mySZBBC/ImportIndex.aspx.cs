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

    /// <summary>
    /// 設定參數 - 產生TraceID
    /// </summary>
    public string TraceID
    {
        get
        {
            //產生TraceID
            long ts = Cryptograph.GetCurrentTime();

            Random rnd = new Random();
            int myRnd = rnd.Next(1, 99);

            return Cryptograph.MD5Encrypt("{0}{1}".FormatThis(ts, myRnd), System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"]);
        }
        set
        {
            this._TraceID = value;
        }
    }
    private string _TraceID;

}