using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 各類參數
/// </summary>
public class fn_Params
{
    /// <summary>
    /// 網站名稱
    /// </summary>
    public static string WebName
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebName"];
        }
        set
        {
            _WebName = value;
        }
    }
    private static string _WebName;


    /// <summary>
    /// 網站網址
    /// </summary>
    public static string WebUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        }
        set
        {
            _WebUrl = value;
        }
    }
    private static string _WebUrl;


    /// <summary>
    /// CDN網址
    /// </summary>
    public static string CDNUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDN_Url"];
        }
        set
        {
            _CDNUrl = value;
        }
    }
    private static string _CDNUrl;

    /// <summary>
    /// Ref網址
    /// </summary>
    public static string RefUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"];
        }
        set
        {
            _RefUrl = value;
        }
    }
    private static string _RefUrl;


    /// <summary>
    /// Deskey
    /// </summary>
    public static string DesKey
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
        }
        private set
        {
            _DesKey = value;
        }
    }
    private static string _DesKey;

    /// <summary>
    /// 系統寄件者
    /// </summary>
    public static string SysMail_Sender
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];
        }
        set
        {
            _SysMail_Sender = value;
        }
    }
    private static string _SysMail_Sender;

    /// <summary>
    /// AD Guid
    /// </summary>
    public static string UserGuid
    {
        get
        {
            return UnobtrusiveSession.Session["Login_GUID"].ToString();
        }
        private set
        {
            _UserGuid = value;
        }
    }
    private static string _UserGuid;


    /// <summary>
    /// 工號
    /// </summary>
    public static string UserAccount
    {
        get
        {
            return UnobtrusiveSession.Session["Login_UserID"].ToString();
        }
        private set
        {
            _UserAccount = value;
        }
    }
    private static string _UserAccount;


    /// <summary>
    /// AD Display Name
    /// </summary>
    public static string UserAccountName
    {
        get
        {
            return UnobtrusiveSession.Session["Login_UserName"].ToString();
        }
        private set
        {
            _UserAccountName = value;
        }
    }
    private static string _UserAccountName;



    /// <summary>
    /// FTP_Username
    /// </summary>
    public static string ftp_Username
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Username"];
        }
        set
        {
            _ftp_Username = value;
        }
    }
    private static string _ftp_Username;

    /// <summary>
    /// Password
    /// </summary>
    public static string ftp_Password
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Password"];
        }
        set
        {
            _ftp_Password = value;
        }
    }
    private static string _ftp_Password;

    /// <summary>
    /// ServerUrl
    /// </summary>
    public static string ftp_ServerUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"];
        }
        set
        {
            _ftp_ServerUrl = value;
        }
    }
    private static string _ftp_ServerUrl;
}