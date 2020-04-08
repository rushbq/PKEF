<%@ WebHandler Language="C#" Class="GetData_Users" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using PKLib_Data.Controllers;
using PKLib_Data.Assets;

public class GetData_Users : IHttpHandler
{

    /// <summary>
    /// 取得人員資料(Ajax)
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string searchVal = context.Request["keyword"];


        //----- 宣告:資料參數 -----
        UsersRepository _product = new UsersRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(searchVal))
        {
            search.Add((int)Common.UserSearch.Keyword, searchVal);
        }


        //----- 原始資料:取得所有資料 -----
        var query = _product.GetUsers(search, null);


        //----- 資料整理:顯示筆數 -----
        var _data = query
            .Select(fld =>
             new
             {
                 ID = fld.ProfID,
                 Label = fld.ProfName,
                 CategoryID = fld.DeptID,
                 Category = fld.DeptName
             })
            .Take(100);


        //----- 資料整理:序列化 ----- 
        string jdata = JsonConvert.SerializeObject(_data, Formatting.Indented);

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