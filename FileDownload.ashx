<%@ WebHandler Language="C#" Class="FileDownload" %>

using System;
using System.Web;

public class FileDownload : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //[取得參數] - 原始檔名
            string OrgFileName = fn_stringFormat.clearFileName(HttpContext.Current.Request["OrgiName"].Trim());
            OrgFileName = Convert.ToChar(34) + System.Web.HttpUtility.UrlEncode(OrgFileName, System.Text.Encoding.UTF8) + Convert.ToChar(34);

            //[取得參數] - 完整檔案路徑
            string FullFilePath = Cryptograph.Decrypt(HttpContext.Current.Request["FilePath"].Trim());

            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("content-disposition", "attachment;filename=" + OrgFileName);
            //輸出檔案
            context.Response.WriteFile(FullFilePath, true);
            context.Response.End();
        }
        catch (Exception)
        {
            throw;
        }

    }

    public bool IsReusable { get { return false; } }

}