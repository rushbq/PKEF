<%@ WebHandler Language="C#" Class="GetData_Customer" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PKLib_Data.Controllers;
using PKLib_Data.Assets;

public class GetData_Customer : IHttpHandler
{

    /// <summary>
    /// 取得客戶資料(Ajax)
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string searchVal = context.Request["keyword"];
        string doBlock = context.Request["doBlock"];


        //----- 宣告:資料參數 -----
        CustomersRepository _data = new CustomersRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(searchVal))
        {
            search.Add((int)Common.CustSearch.Keyword, searchVal);
        }
        //開票專用條件限制
        if (!string.IsNullOrEmpty(doBlock))
        {
            search.Add((int)Common.CustSearch.Block, doBlock);
        }

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetCustomers(search);


        //----- 資料整理:顯示筆數 -----
        var data = query
            .Select(fld =>
             new
             {
                 ID = fld.CustID,
                 Label = "(" + fld.Corp_Name + ") " + fld.CustName,
                 SalesID = fld.SalesID
             })
            .Take(100);


        //----- 資料整理:序列化 ----- 
        string jdata = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

        /*
         * [回傳格式] - Json
         * data：資料
         */

        //輸出Json
        context.Response.ContentType = "application/json";
        context.Response.Write(jdata);


    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}