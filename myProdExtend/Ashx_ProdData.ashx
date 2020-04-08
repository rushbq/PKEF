<%@ WebHandler Language="C#" Class="Ashx_ProdData" %>

using System;
using System.Web;
using ProdExt.Models;
using ProdExt.Controllers;

/// <summary>
/// 資料儲存 - AJAX
/// </summary>
public class Ashx_ProdData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string ErrMsg;

        //[接收參數]
        string _DataID = context.Request["DataID"];
        string _SafeQty_SZEC = context.Request["SafeQty_SZEC"];
        string _SafeQty_A01 = context.Request["SafeQty_A01"];
        string _SafeQty_B01 = context.Request["SafeQty_B01"];
        string _Who = context.Request["Who"];

        //----- 宣告:資料參數 -----
        ProdExtRepository _data = new ProdExtRepository();


        //----- 設定:資料欄位 -----
        var data = new ItemData
        {
            Model_No = _DataID,
            SafeQty_SZEC = Convert.ToInt32(_SafeQty_SZEC),
            SafeQty_A01 = Convert.ToInt32(_SafeQty_A01),
            SafeQty_B01 = Convert.ToInt32(_SafeQty_B01),
            Create_Who = _Who,
            Update_Who = _Who
        };

        //----- 方法:新增資料 -----
        context.Response.ContentType = "text/html";
        if (_data.SetInvoice(data, out ErrMsg))
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