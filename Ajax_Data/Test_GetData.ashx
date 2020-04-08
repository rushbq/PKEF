<%@ WebHandler Language="C#" Class="Test_GetData" %>

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

public class Test_GetData : IHttpHandler
{
    /// <summary>
    /// jQuery DataTable 取得資料
    /// </summary>
    /// <remarks>
    /// 版本:1.10.11
    /// 版本若更改, 需注意參數名稱是否有變動
    /// </remarks>
    public void ProcessRequest(HttpContext context)
    {
        // 接收參數
        //var reqLength = int.Parse(context.Request["length"]);       //每頁資料數
        //var reqStart = int.Parse(context.Request["start"]);    //該頁第一筆資料是全部資料的第幾筆，注意是從0開始。
        //var reqSearchKey = context.Request["search[value]"] == null ? "" : context.Request["search[value]"].ToString();    //查詢文字
        //var reqOrderByColumn = context.Request["order[0][column]"] == null ? "" : context.Request["order[0][column]"].ToString();  //排序欄位
        //var reqOrderByDir = context.Request["order[0][dir]"] == null ? "" : context.Request["order[0][dir]"].ToString();   //排序方式
        //var reqDraw = context.Request["draw"] == null ? "1" : context.Request["draw"].ToString();   //頁數
        string ErrMsg;

        //查詢StoreProcedure (myPrc_GetCustFullPrice)
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "myPrc_GetCustFullPrice";
            cmd.Parameters.AddWithValue("CustID", "1131002");

            //取得回傳值, 輸出參數
            SqlParameter Msg = cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 200);
            Msg.Direction = ParameterDirection.Output;

            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
            {
                //序列化DT
                string data = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.Indented);

                //將Json加到data的屬性下
                JObject json = new JObject();
                json.Add(new JProperty("data", JsonConvert.DeserializeObject<JArray>(data)));


                //var result = new
                //{
                //    recordsTotal = TotalCnt,
                //    recordsFiltered = TotalCnt,
                //    data = DT.AsEnumerable()
                //        .Select(el => new[]
                //    {
                //        el.Field<string>("Account_Name"),
                //        el.Field<string>("Display_Name") 
                //    })
                //};
                /*
                 * [回傳格式] - Json
                 * recordsTotal：篩選前的總資料數
                 * recordsFiltered：篩選後的總資料數
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