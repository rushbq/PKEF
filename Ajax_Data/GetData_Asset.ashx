<%@ WebHandler Language="C#" Class="GetData_Asset" %>

using System.Web;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using PKLib_Data.Controllers;
using PKLib_Data.Assets;

public class GetData_Asset : IHttpHandler
{

    /// <summary>
    /// 取得資產清單(MIS)
    /// 使用Semantic UI的Search UI(type=category)
    /// </summary>
    /// <remarks>
    /// 資料來源
    /// PKLibrary-Data
    /// </remarks>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string searchVal = context.Request["q"];
        string dbs = context.Request["dbs"];

        //----- 宣告:資料參數 -----
        EquipmentRepository _data = new EquipmentRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        
        //固定參數(不使用MIS指定條件)
        search.Add((int)Common.mySearch.IsMIS, "N");

        //關鍵字
        if (!string.IsNullOrEmpty(searchVal))
        {
            search.Add((int)Common.mySearch.Keyword, searchVal);
        }

        //----- 原始資料:取得所有資料 -----
        IQueryable results = null;

        if (dbs.Equals("TW"))
        {
            results = _data.GetDataList(search)
                .Select(fld =>
                    new
                    {
                        ID = fld.ID,
                        Label = fld.Name + " (" + fld.Spec + ")",
                        Category = fld.SupName
                    }).Take(50);
        }
        else
        {
            results = _data.GetDataList_SH(search)
                .Select(fld =>
                    new
                    {
                        ID = fld.ID,
                        Label = fld.Name + " (" + fld.Spec + ")",
                        Category = fld.SupName
                    }).Take(50);
        }


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