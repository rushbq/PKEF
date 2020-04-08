<%@ WebHandler Language="C#" Class="Ashx_InvoiceData" %>

using System;
using System.Web;
using Invoice.Controllers;
using Invoice.Models;

/// <summary>
/// 指定品項是否開發票 - 發票維護頁AJAX
/// </summary>
public class Ashx_InvoiceData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //[接收參數]
        string _FID = context.Request["FID"];
        string _SID = context.Request["SID"];
        string _SNO = context.Request["SNO"];
        string _Val = context.Request["Val"];

        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();


        //----- 方法:更新資料 -----
        context.Response.ContentType = "text/html";
        if (_data.Update_Status(_FID, _SID, _SNO, _Val))
        {
            context.Response.Write("success");
        }
        else
        {
            context.Response.Write("fail...");
        }


        _data = null;

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}