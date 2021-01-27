<%@ WebHandler Language="C#" Class="GetMenuList" %>

using System;
using System.Web;
using System.Linq;
using MISData.Controllers;
using Newtonsoft.Json;
using PKLib_Method.Methods;

public class GetMenuList : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string search_Year = context.Request["y"];
        string search_DB = context.Request["db"];
        string search_sm = context.Request["sm"];
        string search_sd = context.Request["sd"];
        string search_em = context.Request["em"];
        string search_ed = context.Request["ed"];


        //----- 宣告:資料參數 -----
        MISRepository _data = new MISRepository();
        string ErrMsg;


        //----- 原始資料:取得所有資料 -----
        string _sDate = (new DateTime(Convert.ToInt16(search_Year), Convert.ToInt16(search_sm), Convert.ToInt16(search_sd))).ToShortDateString().ToDateString("yyyy/MM/dd");
        string _eDate = (new DateTime(Convert.ToInt16(search_Year), Convert.ToInt16(search_em), Convert.ToInt16(search_ed))).ToShortDateString().ToDateString("yyyy/MM/dd");

        var query = _data.Get_WebClickList(search_DB, search_Year, _sDate, _eDate, out ErrMsg);


        //----- 資料整理:顯示欄位 -----
        /*
         * 使用jsTree 樹狀選單元件
         * 將資料整理成 jsTree json格式
         */
        var data = query
            .Select(fld =>
             new
             {
                 id = fld.MenuID,
                 parentID = fld.ParentID,
                 label = fld.MenuName,
                 selected = false,
                 open = true,
                 cnt = fld.ClickCnt,
                 remark = fld.Remark
             });


        //----- 資料整理:序列化 ----- 
        string jdata = JsonConvert.SerializeObject(data, Formatting.Indented);

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