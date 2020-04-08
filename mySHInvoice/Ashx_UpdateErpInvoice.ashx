<%@ WebHandler Language="C#" Class="Ashx_UpdateErpInvoice" %>

using System;
using System.Web;
using SH_Invoice.Models;
using SH_Invoice.Controllers;
using PKLib_Method.Methods;
using LogRecord;

/// <summary>
/// 資料儲存 - AJAX
/// ERP發票號碼更新 - 手動填入發票號碼(ErpInvStep2.aspx)
/// </summary>
public class Ashx_UpdateErpInvoice : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        System.Threading.Thread.Sleep(1000);

        string ErrMsg;

        //[接收參數]
        string _DataID = context.Request["DataID"];
        string _Who = context.Request["Who"];
        string _DataVal = context.Request["DataVal"];
        string _InvDate = context.Request["InvDate"];
        string _Type = context.Request["type"];

        context.Response.ContentType = "text/html";

        //----- 宣告:資料參數 -----
        SH_InvoiceRepository _data = new SH_InvoiceRepository();

        if (_Type.Equals("U"))
        {
            //----- 方法:更新資料 -----
            if (_data.UpdateInvNo(_DataID, _Who, _DataVal, _InvDate, out ErrMsg))
            {
                fn_Log.Log_Rec("上海開票", "手動開票-Update", "更新發票號至ERP,結帳單號:{0},發票號碼:{1}".FormatThis(_DataID, _DataVal), _Who);

                context.Response.Write("success");
            }
            else
            {
                context.Response.Write("fail..." + ErrMsg);
            }
        }
        else
        {
            //----- 方法:復原資料 -----
            if (_data.UpdateInvNo(_DataID, _Who, "", "", out ErrMsg))
            {
                fn_Log.Log_Rec("上海開票", "手動開票-Restore", "從ERP清空發票號,結帳單號:{0}".FormatThis(_DataID), _Who);

                context.Response.Write("success");
            }
            else
            {
                context.Response.Write("fail..." + ErrMsg);
            }
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