<%@ WebHandler Language="C#" Class="GetHtml_Supplier" %>

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using PKLib_Data.Controllers;
using PKLib_Data.Assets;
using PKLib_Method.Methods;

public class GetHtml_Supplier : IHttpHandler
{

    /// <summary>
    /// 取得供應商關聯列表
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //sleep 1 sec.
        System.Threading.Thread.Sleep(1000);
        
        //[接收參數] 查詢字串
        string _id = context.Request["DataID"];

        if (string.IsNullOrEmpty(_id))
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("Fail..");
            return;
        }


        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetRelList(_id);

        //----- 資料整理:顯示Html -----
        StringBuilder html = new StringBuilder();
        int row = 0;

        html.Append("<table class=\"ui very basic collapsing celled table\">");
        //html.Append("<thead><tr><th class=\"grey-text text-darken-1\">公司別</th><th class=\"grey-text text-darken-1\">代號</th><th class=\"grey-text text-darken-1\">名稱</th></tr></thead>");
        html.Append("<tbody>");

        foreach (var item in query)
        {
            html.Append("<tr>");
            html.Append("<td>{0}</td><td>{1}</td><td>{2}</td>".FormatThis(
                item.Corp_Name
                , item.ERP_SupID
                , item.ERP_SupName
                ));
            html.Append("</tr>");

            row++;
        }

        html.Append("</tbody>");
        html.Append("</table>");

        //若無資料
        if (row.Equals(0))
        {
            html.Clear();
            html.Append("<p class=\"red-text\">尚未設定關聯, <a href=\"{0}\">請前往設定</a></p>".FormatThis(
                System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"] + "myDataInfo/SupplierEdit.aspx?DataID=" + _id
                ));
        }

        //release
        _data = null;

        //輸出Html
        context.Response.ContentType = "text/html";
        context.Response.Write(html.ToString());


    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}