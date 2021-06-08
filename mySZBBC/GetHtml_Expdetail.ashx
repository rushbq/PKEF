<%@ WebHandler Language="C#" Class="GetHtml_SOdetail" %>

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using Invoice.SH.Controllers;
using PKLib_Method.Methods;

public class GetHtml_SOdetail : IHttpHandler
{
    /// <summary>
    /// 取得銷貨明細
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string id = context.Request["id"];

        if (string.IsNullOrEmpty(id))
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("Load fail..");
            return;
        }


        //----- 宣告:資料參數 -----
        //InvoiceRepository _data = new InvoiceRepository();
        //Dictionary<int, string> search = new Dictionary<int, string>();
        //search.Add((int)mySearch.DataID, id);

        ////----- 原始資料:取得所有資料 -----
        //var query = _data.GetDataList_withExport(search);

        ////----- 資料整理:顯示Html -----
        StringBuilder html = new StringBuilder();
        //int row = 0;

        //html.Append("<table class=\"bordered\">");
        //html.Append("<tr>");
        //html.Append(" <td class=\"grey lighten-5\">客戶單號</td>");
        //html.Append(" <td class=\"grey lighten-5\">客戶暱稱</td>");
        //html.Append(" <td class=\"grey lighten-5 center-align\">銷貨單號</td>");
        //html.Append(" <td class=\"grey lighten-5\">客戶代號/客戶名稱</td>");
        //html.Append(" <td class=\"grey lighten-5 center-align\">銷貨日期</td>");
        //html.Append(" <td class=\"grey lighten-5 right-align\">開票金額</td>");
        //html.Append(" <td class=\"grey lighten-5 center-align\">發票抬頭</td>");
        //html.Append(" <td class=\"grey lighten-5 center-align\">發票類型</td>");
        //html.Append("</tr>");

        //foreach (var item in query)
        //{
        //    html.AppendLine("<tr>");
        //    html.Append("<td>{0}</td>".FormatThis(item.OrderID));
        //    html.Append("<td>{0}</td>".FormatThis(item.NickName));
        //    html.Append("<td class=\"center-align\">{0}-{1}</td>".FormatThis(item.SO_FID, item.SO_SID));
        //    html.Append("<td>{0}<br/>{1}</td>".FormatThis(item.CustID, item.CustName));
        //    html.Append("<td class=\"center-align\">{0}</td>".FormatThis(item.SO_Date));
        //    html.Append("<td class=\"right-align\">{0}</td>".FormatThis(item.TotalPrice));
        //    html.Append("<td class=\"center-align\">{0}</td>".FormatThis(item.InvTitle));
        //    html.Append("<td class=\"center-align\">{0}</td>".FormatThis(fn_Desc.BBC.GetInvTypeName(item.InvType)));
        //    html.AppendLine("</tr>");

        //    row++;
        //}

        //html.Append("</table>");

        ////若無資料
        //if (row.Equals(0))
        //{
        //    html.Clear();
        //    html.Append("<p class=\"red-text\">查無資料....</p>");
        //}

        //query = null;

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