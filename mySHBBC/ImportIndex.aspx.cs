using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SH_BBC.Controllers;

public partial class mySHBBC_ImportIndex : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("861", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //get remark list
                LookupData_Remark();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// 銷退代號
    /// </summary>
    private void LookupData_Remark()
    {
        //----- 宣告:資料參數 -----
        SHBBCRepository _data = new SHBBCRepository();

        //----- 原始資料:取得基本資料 -----
        var query = _data.GetRemarkList(out ErrMsg);
        
        //----- 資料整理:繫結 ----- 
        this.lv_RemarkList.DataSource = query;
        this.lv_RemarkList.DataBind();
        
        //release
        query = null;
        _data = null;

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
        string url = "{0}mySHBBC/ImportStep1.aspx?ts={1}&type=1".FormatThis(
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
        string url = "{0}mySHBBC/ImportStep1.aspx?ts={1}&type=2".FormatThis(
             fn_Params.WebUrl
             , Cryptograph.MD5Encrypt(NewTraceID(), System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"])
            );

        Response.Redirect(url);
    }

}