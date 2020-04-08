<%@ WebHandler Language="C#" Class="Ashx_BBCTempData" %>

using System;
using System.Web;
using System.IO;
using System.Linq;
using TWBBC_Mall.Models;
using TWBBC_Mall.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

/// <summary>
/// BBC 匯入Step2-1, Excel資料整理
/// 欄位:產品編號, 開票資料
/// </summary>
public class Ashx_BBCTempData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //wait 1 sec.
        System.Threading.Thread.Sleep(1000);

        //Response ContentType
        context.Response.ContentType = "text/plain";
        string json = string.Empty;
        string ErrMsg = "";

        //Read Request Json
        using (var reader = new StreamReader(context.Request.InputStream))
        {
            json = reader.ReadToEnd();
        }

        //Convert json to JArray
        JArray jArry = JsonConvert.DeserializeObject<JArray>(json);


        //Get Data
        List<RefColumn> dataItems = ((JArray)jArry).Select(fld => new RefColumn
        {
            Parent_ID = new Guid(fld["ParentID"].ToString()),  //單頭編號
            OrderID = (string)fld["OrderID"],    //平台單號
            Data_ID = (int)fld["DataID"].ToObject<int>(),  //單身資料編號(轉型態使用ToObject<int>)
            ProdID = (string)fld["ProdID"],  //產品編號
            BuyCnt = (int)fld["BuyCnt"].ToObject<int>()
        }).ToList();


        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();

        //----- 方法:更新資料 -----
        if (_data.Update_TempDT(dataItems, out ErrMsg))
        {
            context.Response.Write("success");
        }
        else
        {
            context.Response.Write("fail...");
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