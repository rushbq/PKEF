<%@ WebHandler Language="C#" Class="Ashx_BBCTempNewItem" %>

using System;
using System.Web;
using System.IO;
using System.Linq;
using SZ_BBC.Models;
using SZ_BBC.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

/// <summary>
/// BBC匯入Step2-1, Excel資料整理
/// 手動新增商品
/// </summary>
public class Ashx_BBCTempNewItem : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //wait 1 sec.
        System.Threading.Thread.Sleep(1000);

        //[接收參數]
        string _parentID = context.Request["parentID"];
        string _orderID = context.Request["orderID"];
        string _itemID = context.Request["itemID"];
        
        //Response ContentType
        context.Response.ContentType = "text/plain";
        string ErrMsg = "";


        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        //----- 方法:更新資料 -----
        if (_data.Create_TempNewItem(_parentID, _orderID, Convert.ToInt32(_itemID) ,out ErrMsg))
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