﻿<%@ WebHandler Language="C#" Class="GetFullPriceData_OverSales" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GetFullPriceData_OverSales : IHttpHandler
{
    /// <summary>
    /// jQuery DataTable 取得資料(Ajax)
    /// </summary>
    /// <remarks>
    /// 版本:1.10.11
    /// 版本若更改, 需注意參數名稱是否有變動
    /// </remarks>
    public void ProcessRequest(HttpContext context)
    {
        string ErrMsg;
        
        // 接收參數
        var reqID = context.Request["DataID"].ToString();

        //查詢StoreProcedure (myPrc_GetCustFullPrice_OverSales)
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "myPrc_GetCustFullPrice_OverSales";
            cmd.Parameters.AddWithValue("CustID", reqID);
            cmd.Parameters.AddWithValue("ProdClass", DBNull.Value);
            cmd.CommandTimeout = 90;
            
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
            {
                //序列化DT
                string data = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.Indented);
                
                //將Json加到data的屬性下
                JObject json = new JObject();
                json.Add(new JProperty("data", JsonConvert.DeserializeObject<JArray>(data)));

                /*
                 * [回傳格式] - Json
                 * recordsTotal：篩選前的總資料數 (serverside模式)
                 * recordsFiltered：篩選後的總資料數 (serverside模式)
                 * data：該分頁所需要的資料
                 */

                //輸出Json
                context.Response.ContentType = "application/json";
                context.Response.Write(json);
            }
        }
        
    }

 
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }


}