<%@ WebHandler Language="C#" Class="GetHtml_SOdetail" %>

using System;
using System.Web;
using System.Text;
using Invoice.Controllers;
using PKLib_Method.Methods;

public class GetHtml_SOdetail : IHttpHandler
{
    /// <summary>
    /// 發票維護資料-取得銷貨明細
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string fid = context.Request["fid"];
        string sid = context.Request["sid"];

        if (string.IsNullOrEmpty(fid) || string.IsNullOrEmpty(sid))
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("Load fail..");
            return;
        }


        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDetailList_SH(fid, sid);

        //----- 資料整理:顯示Html -----
        StringBuilder html = new StringBuilder();
        int row = 0;

        html.Append("<table class=\"bordered\">");
        html.Append("<tr>");
        html.Append(" <td class=\"grey lighten-5\">品號</td>");
        html.Append(" <td class=\"grey lighten-5 right-align\">數量</td>");
        html.Append(" <td class=\"grey lighten-5 right-align\">金額</td>");
        html.Append(" <td class=\"grey lighten-5 right-align\">稅額</td>");
        html.Append(" <td class=\"grey lighten-5\">&nbsp;</td>");
        html.Append("</tr>");

        foreach (var item in query)
        {
            //執行更新的按鈕-N/Y
            //string btnUpdate = "<a href=\"#!\" class=\"btn waves-effect waves-light green lighten-1 btn-UpdateDetail btn-{0}{1}{2}\" data-rel=\"{0}{1}{2}\" data-fid=\"{0}\" data-sid=\"{1}\" data-no=\"{2}\" data-value=\"N\" title=\"設為「不開發票」\" style=\"display:{3}\"><i class=\"material-icons\">highlight_off</i></a>"
            //    .FormatThis(item.SO_FID, item.SO_SID, item.SO_No, item.IsInvoice.Equals("Y") ? "inline-block" : "none");
            //btnUpdate += "<a href=\"#!\" class=\"btn waves-effect waves-light grey darkten-1 btn-UpdateDetail btn-{0}{1}{2}\" data-rel=\"{0}{1}{2}\" data-fid=\"{0}\" data-sid=\"{1}\" data-no=\"{2}\" data-value=\"Y\" title=\"還原為「要開發票」\" style=\"display:{3}\"><i class=\"material-icons\">undo</i></a>"
            //    .FormatThis(item.SO_FID, item.SO_SID, item.SO_No, item.IsInvoice.Equals("Y") ? "none" : "inline-block");

            //Html
            html.AppendLine("<tr>");
            html.Append("<td>{0}</td><td class=\"right-align\">{1}</td><td class=\"right-align\">{2}</td><td class=\"right-align\">{3}</td><td class=\"right-align\">{4}</td>"
                .FormatThis(
                    item.ModelNo
                    , item.Qty
                    , item.Price
                    , item.TaxPrice
                    , ""
                ));
            html.AppendLine("</tr>");

            row++;
        }

        html.Append("</table>");

        //若無資料
        if (row.Equals(0))
        {
            html.Clear();
            html.Append("<p class=\"red-text\">查無資料....</p>");
        }

        query = null;

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