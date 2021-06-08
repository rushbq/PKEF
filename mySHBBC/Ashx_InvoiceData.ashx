<%@ WebHandler Language="C#" Class="Ashx_InvoiceData" %>

using System;
using System.Web;
using Invoice.SH.Controllers;
using Invoice.Models;

/// <summary>
/// 發票維護資料儲存 - AJAX
/// </summary>
public class Ashx_InvoiceData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string ErrMsg;

        //[接收參數]
        string _OrderID = context.Request["OrderID"];
        string _TraceID = context.Request["TraceID"];
        string _InvTitle = context.Request["InvTitle"];
        string _InvType = context.Request["InvType"];
        string _InvNumber = context.Request["InvNumber"];
        string _InvAddrInfo = context.Request["InvAddrInfo"];
        string _InvBankInfo = context.Request["InvBankInfo"];
        string _InvMessage = context.Request["InvMessage"];
        string _InvRemark = context.Request["InvRemark"];
        string _Who = context.Request["Who"];

        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();


        //----- 設定:資料欄位 -----
        var data = new InvoiceData
        {
            OrderID = _OrderID,
            TraceID = _TraceID,
            InvTitle = _InvTitle,
            InvType = _InvType,
            InvNumber = _InvNumber,
            InvAddrInfo = _InvAddrInfo,
            InvBankInfo = _InvBankInfo,
            InvMessage = _InvMessage,
            InvRemark = _InvRemark,
            Create_Who = _Who,
            Update_Who = _Who
        };

        //----- 方法:新增資料 -----
        context.Response.ContentType = "text/html";
        if (_data.SetInvoice_SH(data, out ErrMsg))
        {
            context.Response.Write("success");
        }
        else
        {
            context.Response.Write("fail..." + ErrMsg);
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