<%@ WebHandler Language="C#" Class="Ashx_FtpFileDownload" %>

using System;
using System.Web;
using ExtensionMethods;

public class Ashx_FtpFileDownload : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //取得參數
            string dwFolder = HttpContext.Current.Request["dwFolder"].Trim();
            string realFile = HttpContext.Current.Request["realFile"].Trim();
            string dwFileName = HttpContext.Current.Request["dwFileName"].Trim();

            //ftp路徑 + folder + 檔名
            fn_FTP.FTP_doDownload("{0}{1}/{2}".FormatThis(Ftp_Url, dwFolder, realFile), dwFileName);

            ////輸出檔案
            //context.Response.BinaryWrite(xfile);
            //context.Response.End();
        }
        catch (Exception)
        {
            throw new Exception("處理失敗");
        }

    }

    public bool IsReusable { get { return false; } }

    /// <summary>
    /// FTP路徑
    /// </summary>
    private string _Ftp_Url;
    public string Ftp_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"];
        }
        set
        {
            this._Ftp_Url = value;
        }
    }

}