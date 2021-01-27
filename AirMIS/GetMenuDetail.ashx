<%@ WebHandler Language="C#" Class="GetMenuDetail" %>

using System;
using System.Web;
using System.Linq;
using MISData.Controllers;

public class GetMenuDetail : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string search_Year = context.Request["y"];
        string search_DB = context.Request["db"];
        string search_ID = context.Request["id"];


        //----- 宣告:資料參數 -----
        MISRepository _data = new MISRepository();
        string ErrMsg;


        //----- 原始資料:取得所有資料 -----
        System.Data.DataTable query = _data.GetDT_Click(search_ID, search_DB, search_Year, out ErrMsg);


        //----- 資料整理:顯示欄位 -----
        System.Text.StringBuilder html = new System.Text.StringBuilder();

        
        for (int row = 0; row < query.Rows.Count; row++)
        {
            //*** 填入Html ***
            html.Append("<tr>");
            html.Append("<td style=\"text-align:center\">" + query.Rows[row]["tot"] + "</td>");
            html.Append("<td style=\"text-align:center\">" + query.Rows[row]["Display_Name"] + "</td>");
            html.Append("</tr>");
        }

        //output
        context.Response.Write(html.ToString());
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}