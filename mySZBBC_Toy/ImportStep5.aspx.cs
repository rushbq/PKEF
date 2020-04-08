using System;
using System.Web;


public partial class mySZBBC_ImportStep5 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("910", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //失敗或成功
                if (Req_Status.Equals("200"))
                {
                    this.ph_Message.Visible = false;
                    this.ph_Content.Visible = true;
                    return;
                }
                else
                {
                    this.ph_Message.Visible = true;
                    this.ph_Content.Visible = false;
                    return;
                }

            }


        }
        catch (Exception)
        {

            throw;
        }
    }



    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            string data = Request.QueryString["dataID"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;

    /// <summary>
    /// 設定參數 - 取得匯入狀態
    /// </summary>
    public string Req_Status
    {
        get
        {
            string data = Request.QueryString["st"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Status = value;
        }
    }
    private string _Req_Status;
    #endregion

}