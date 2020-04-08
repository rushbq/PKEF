<%@ WebHandler Language="C#" Class="Ashx_UpdateData" %>

using System;
using System.Web;
using SZ_Invoice.Aisino.Models;
using SZ_Invoice.Aisino.Controllers;

/// <summary>
/// [紙本發票][電商紙本發票],資料儲存 - AJAX
/// 1.列表頁直接轉入
/// 2.ERP發票號碼更新(List.aspx)
/// </summary>
public class Ashx_UpdateData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        System.Threading.Thread.Sleep(1000);

        string ErrMsg;

        //[接收參數]
        string _DataID = context.Request["DataID"];
        string _Who = context.Request["Who"];
        string _Type = context.Request["type"];
        string _invtype = context.Request["invtype"];

        context.Response.ContentType = "text/html";

        //----- 宣告:資料參數 -----
        SZ_InvoiceRepository _data = new SZ_InvoiceRepository();

        if (_Type.Equals("U"))
        {
            //----- 方法:更新資料 -----
            if (_data.Update(_DataID, _Who, out ErrMsg))
            {
                context.Response.Write("success");
            }
            else
            {
                context.Response.Write("fail..." + ErrMsg);
            }
        }
        else
        {
            //----- 方法:新增資料 -----
            if (_data.CreateImportData(_DataID, _Who, _invtype, out ErrMsg))
            {
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