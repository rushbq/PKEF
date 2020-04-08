<%@ WebHandler Language="C#" Class="GetData_RefCopmg" %>

using System.Web;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using SZ_BBC.Controllers;
using SZ_BBC.Models;

public class GetData_RefCopmg : IHttpHandler
{
    /// <summary>
    /// 取得RefCOPMG資料(Ajax)
    /// 使用Semantic UI的Dropdown Search UI
    /// </summary>
    /// <remarks>
    /// 使用:
    /// mySZBBC/ImportStep2-1.aspx
    /// mySZBBC_Toy/ImportStep2-1.aspx
    /// </remarks>
    public void ProcessRequest(HttpContext context)
    {
        //wait 1 sec.
        System.Threading.Thread.Sleep(1000);
        
        //[接收參數] 查詢字串
        string _searchVal = context.Request["q"];
        string _cust = context.Request["cust"];
        string _mall = context.Request["mall"];
        string ErrMsg = "";

        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();


        //----- 原始資料:條件篩選 -----
        //[取得/檢查參數] - Cust
        if (!string.IsNullOrWhiteSpace(_cust))
        {
            search.Add("Cust", _cust);
        }

        //[取得/檢查參數] - Mall
        if (!string.IsNullOrWhiteSpace(_mall))
        {
            search.Add("Mall", _mall);
        }

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrWhiteSpace(_searchVal))
        {
            search.Add("Keyword", _searchVal);
        }

        //----- 原始資料:取得所有資料 -----
        var results = _data.GetRefModelList(search, 1, out ErrMsg)
            .Select(fld =>
                new
                {
                    ID = fld.Data_ID,
                    Label = fld.ModelNo + " (" + fld.CustModelNo + ") " + fld.CustSpec
                }).Take(20);


        var data = new { results };


        //----- 資料整理:序列化 ----- 
        string jdata = JsonConvert.SerializeObject(data, Formatting.None);

        /*
         * [回傳格式] - Json
         * results：資料
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