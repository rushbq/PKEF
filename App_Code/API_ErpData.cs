using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using Newtonsoft.Json;
using ExtensionMethods;

/// <summary>
/// BBC處理
/// </summary>
[WebService(Namespace = "http://www.prokits.com.tw/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下一行。
//[System.Web.Script.Services.ScriptService]
public class API_ERPData : System.Web.Services.WebService
{
    public string ErrMsg;


    /// <summary>
    /// 取得資料 - Step3(內業BBC)
    /// </summary>
    /// <param name="custID"></param>
    /// <param name="DBS"></param>
    /// <param name="valModelNo"></param>
    /// <param name="valQty"></param>
    /// <param name="SWID"></param>
    [WebMethod(enableSession: true)]
    public void GetData(string custID, string DBS, string valModelNo, string valQty, string SWID)
    {
        StringBuilder html = new StringBuilder();

        //-- 取得價格 --
        DataTable myDTPrice = GetPrice(custID, DBS, valModelNo, valQty, out ErrMsg);
        if (myDTPrice == null)
        {
            this.Context.Response.Write("fail:{0}".FormatThis(ErrMsg));
            return;
        }

        //-- 取得庫存 --
        DataTable myDTStock = GetStock(DBS, SWID, valModelNo, out ErrMsg);
        if (myDTStock == null)
        {
            this.Context.Response.Write("fail:{0}".FormatThis(ErrMsg));
            return;
        }

        //-- 合併Datatable --
        DataTable dtResult = new DataTable();
        dtResult.Columns.Add("ModelNo", typeof(string));
        dtResult.Columns.Add("Model_Name_zh_TW", typeof(string));
        dtResult.Columns.Add("UnitPrice", typeof(double));
        dtResult.Columns.Add("SpQty", typeof(int));
        dtResult.Columns.Add("SpQtyPrice", typeof(double));
        dtResult.Columns.Add("BuyQty", typeof(int));
        dtResult.Columns.Add("StockNum", typeof(int));

        var result = from dataRows1 in myDTPrice.AsEnumerable()
                     join dataRows2 in myDTStock.AsEnumerable()
                     on dataRows1.Field<string>("ModelNo") equals dataRows2.Field<string>("ModelNo") into myMix
                     from r in myMix.DefaultIfEmpty()
                     select dtResult.LoadDataRow(new object[]
             {
                dataRows1.Field<string>("ModelNo"),
                dataRows1.Field<string>("Model_Name_zh_TW"),
                dataRows1.Field<double>("UnitPrice"),
                dataRows1.Field<int>("SpQty"),
                dataRows1.Field<double>("SpQtyPrice"),
                dataRows1.Field<int>("BuyQty"),
                r == null ? 0 : r.Field<int>("StockNum")                
             }, false);

        result.CopyToDataTable();

        //顯示提示資訊
        string[] strAry_Items = Regex.Split(valModelNo, @"\,{1}");
        int inputNum = strAry_Items.Count();
        int outputNum = dtResult.Rows.Count;

        html.AppendLine(
            "<li class=\"list-group-item list-group-item-warning\"><h4><i class=\"fa fa-shopping-bag\"></i>&nbsp;購買清單&nbsp;<small>輸入品項共 <span class=\"label label-success\">{0}</span> 筆, 輸出品項共 <span class=\"label label-primary\">{1}</span> 筆</small></h4></li>"
            .FormatThis(inputNum, outputNum));


        //顯示列表明細, 庫存為 0 時背景變色
        double cntTotal = 0;
        for (int row = 0; row < dtResult.Rows.Count; row++)
        {
            string ModelNo = dtResult.Rows[row]["ModelNo"].ToString().Trim();
            string ModelName = dtResult.Rows[row]["Model_Name_zh_TW"].ToString();
            double SellPrice = Convert.ToDouble(dtResult.Rows[row]["SpQtyPrice"]);
            double BuyQty = Convert.ToDouble(dtResult.Rows[row]["BuyQty"]);
            int StockNum = Convert.ToInt32(dtResult.Rows[row]["StockNum"]);
            double SubTotal = SellPrice * BuyQty;

            html.AppendLine(" <li class=\"list-group-item {0}\">".FormatThis(StockNum == 0 ? "list-group-item-danger" : ""));
            html.Append(" <table class=\"table\">");
            html.Append("     <tbody>");
            html.Append("         <tr>");
            //- 品名 -
            html.Append("             <td><h4>{0}</h4></td>".FormatThis(ModelName));
            //- 單價 -
            html.Append("             <td class=\"text-right\" style=\"width: 15%\">$ <span>{0}</span></td>".FormatThis(fn_stringFormat.C_format(SellPrice.ToString())));
            //- 數量 -
            html.Append("             <td class=\"text-center\" style=\"width: 15%\">{0}</td>".FormatThis(BuyQty));
            //- 小計 -
            html.Append("             <td class=\"text-right\" style=\"width: 20%\"><strong>$ <span>{0}</span></strong></td>".FormatThis(fn_stringFormat.C_format(SubTotal.ToString())));
            html.Append("         </tr>");
            html.Append("         <tr>");
            //- 品號 -
            html.Append("             <td><label class=\"label label-warning\">{0}</label></td>".FormatThis(ModelNo));
            //- 庫存 -
            html.Append("             <td colspan=\"2\" class=\"text-right\"><abbr title=\"目前庫存\">庫存：{0}</abbr></td>".FormatThis(StockNum));
            html.Append("             <td align=\"right\">{0}</td>".FormatThis(StockNum == 0 ? "<i class=\"fa fa-bug\"></i>" : ""));
            html.Append("         </tr>");
            html.Append("     </tbody>");
            html.Append(" </table>");

            html.AppendLine("</li>");

            //Count Total
            cntTotal += SubTotal;
        }

        //Total
        html.AppendLine("<li class=\"list-group-item list-group-item-danger text-right\">Total：<strong>$ <span>{0}</span></strong></li>".FormatThis(fn_stringFormat.C_format(cntTotal.ToString())));

        //輸出html
        this.Context.Response.Write(html.ToString());
    }


    /// <summary>
    /// 建立訂單 - 取得ERP資料
    /// </summary>
    /// <param name="custID"></param>
    /// <param name="DBS"></param>
    /// <param name="valModelNo"></param>
    /// <param name="valQty"></param>
    /// <param name="SWID"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    public DataTable GetData(string custID, string DBS, string valModelNo, string valQty, string SWID, out string ErrMsg)
    {
        //-- 取得價格 --
        DataTable myDTPrice = GetPrice(custID, DBS, valModelNo, valQty, out ErrMsg);
        if (myDTPrice == null)
        {
            return null;
        }

        //-- 取得庫存 --
        DataTable myDTStock = GetStock(DBS, SWID, valModelNo, out ErrMsg);
        if (myDTStock == null)
        {
            return null;
        }

        //-- 合併Datatable --
        DataTable dtResult = new DataTable();
        dtResult.Columns.Add("ModelNo", typeof(string));
        dtResult.Columns.Add("Model_Name_zh_TW", typeof(string));
        dtResult.Columns.Add("Currency", typeof(string));
        dtResult.Columns.Add("UnitPrice", typeof(double));
        dtResult.Columns.Add("SpQty", typeof(int));
        dtResult.Columns.Add("SpQtyPrice", typeof(double));
        dtResult.Columns.Add("BuyQty", typeof(int));
        dtResult.Columns.Add("inMOQ", typeof(int));
        dtResult.Columns.Add("StockNum", typeof(int));

        var result = from dataRows1 in myDTPrice.AsEnumerable()
                     join dataRows2 in myDTStock.AsEnumerable()
                     on dataRows1.Field<string>("ModelNo") equals dataRows2.Field<string>("ModelNo") into myMix
                     from r in myMix.DefaultIfEmpty()
                     select dtResult.LoadDataRow(new object[]
             {
                dataRows1.Field<string>("ModelNo"),
                dataRows1.Field<string>("Model_Name_zh_TW"),
                dataRows1.Field<string>("Currency"),
                dataRows1.Field<double>("UnitPrice"),
                dataRows1.Field<int>("SpQty"),
                dataRows1.Field<double>("SpQtyPrice"),
                dataRows1.Field<int?>("BuyQty"),
                dataRows1.Field<int>("inMOQ"),
                r == null ? 0 : r.Field<int>("StockNum")
             }, false);

        result.CopyToDataTable();


        return dtResult;
    }


    /// <summary>
    ///  取得報價
    /// </summary>
    /// <param name="custID">客戶代號</param>
    /// <param name="DBS">資料庫</param>
    /// <param name="valModelNo">品號</param>
    /// <param name="valQty">輸入數量</param>
    /// <returns></returns>
    public DataTable GetPrice(string custID, string DBS, string valModelNo, string valQty, out string ErrMsg)
    {
        //檢查是否有空值
        if (string.IsNullOrEmpty(custID) || string.IsNullOrEmpty(valModelNo) || string.IsNullOrEmpty(valQty))
        {
            ErrMsg = "資料未提供齊全";
            return null;
        }

        //判斷是否為空
        if (string.IsNullOrEmpty(valModelNo))
        {
            ErrMsg = "無法取得產品清單";
            return null;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(valModelNo, @"\,{1}");
        string[] strAry_Qty = Regex.Split(valQty, @"\,{1}");

        //宣告暫存清單
        List<TempParam_Item> ITempList = new List<TempParam_Item>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam_Item(strAry_ID[row], Convert.ToInt16(strAry_Qty[row])));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID,
                        Qty = el.tmp_Qty
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID,
                        Qty = gp.Key.Qty
                    };


        #region -- 取得價格 --
        //設定參數
        ArrayList aryCustID = new ArrayList();
        aryCustID.Add(custID);

        ArrayList aryDBS = new ArrayList();
        aryDBS.Add(DBS);

        ArrayList aryModelNo = new ArrayList();
        ArrayList aryQty = new ArrayList();

        foreach (var item in query)
        {
            //品號
            aryModelNo.Add(item.ID);

            //數量
            aryQty.Add(item.Qty);
        }

        //取得價格資料 (DataTable)
        DataTable DT_Price = GetQuotePrice(aryCustID, aryModelNo, aryDBS, aryQty, out ErrMsg);
        if (DT_Price == null)
        {
            //無法取得價格
            ErrMsg = "無法取得價格";
            return null;
        }

        return DT_Price;

        #endregion

    }

    /// <summary>
    /// 取得庫存 - 多筆
    /// </summary>
    /// <param name="DBS"></param>
    /// <param name="SWID"></param>
    /// <param name="valModelNo"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private DataTable GetStock(string DBS, string SWID, string valModelNo, out string ErrMsg)
    {
        //判斷是否為空
        if (string.IsNullOrEmpty(valModelNo))
        {
            ErrMsg = "無法取得產品清單";
            return null;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(valModelNo, @"\,{1}");

        //宣告暫存清單
        List<TempParam_Item> ITempList = new List<TempParam_Item>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam_Item(strAry_ID[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID
                    };


        #region -- 取得庫存 --
        //庫別
        string StockType = GetParamData("EDI_庫別", SWID);

        //設定參數
        ArrayList aryStockType = new ArrayList();
        aryStockType.Add(StockType);

        ArrayList aryDBS = new ArrayList();
        aryDBS.Add(DBS);

        ArrayList aryModelNo = new ArrayList();

        /* 20180723 - 移除使用webservice, 網路不穩會發生無法預料的問題 */
        //宣告WebService
        //API_GetERPData.ws_GetERPData ws_GetData = new API_GetERPData.ws_GetERPData();

        foreach (var item in query)
        {
            //品號
            aryModelNo.Add(item.ID);
        }

        //取得庫存資料 (DataTable)
        //DataTable DT_Stock = ws_GetData.GetStock(aryStockType.ToArray(), aryModelNo.ToArray(), aryDBS.ToArray(), TokenID, out ErrMsg);
        DataTable DT_Stock = GetStock(aryStockType, aryModelNo, aryDBS, out ErrMsg);
        if (DT_Stock == null)
        {
            ErrMsg = "無法取得庫存";
            return null;
        }

        return DT_Stock;

        #endregion
    }


    /// <summary>
    /// 取得庫存 - 單筆
    /// </summary>
    /// <param name="DBS"></param>
    /// <param name="SWID"></param>
    /// <param name="ModelNo"></param>
    /// <returns></returns>
    [WebMethod(enableSession: true)]
    public void GetStockNum(string DBS, string SWID, string ModelNo)
    {
        string stockNum = "-";

        //判斷是否為空
        if (string.IsNullOrEmpty(ModelNo))
        {
            this.Context.Response.Write(stockNum);
            return;
        }


        #region -- 取得庫存 --
        //庫別
        string StockType = GetParamData("EDI_庫別", SWID);

        //設定參數
        ArrayList aryStockType = new ArrayList();
        aryStockType.Add(StockType);

        ArrayList aryDBS = new ArrayList();
        aryDBS.Add(DBS);

        ArrayList aryModelNo = new ArrayList();
        aryModelNo.Add(ModelNo);


        //宣告WebService
        //API_GetERPData.ws_GetERPData ws_GetData = new API_GetERPData.ws_GetERPData();


        //取得庫存資料 (DataTable)
        DataTable DT_Stock = GetStock(aryStockType, aryModelNo, aryDBS, out ErrMsg);
        if (DT_Stock == null)
        {
            this.Context.Response.Write(stockNum);
            return;
        }

        //OK
        stockNum = DT_Stock.Rows[0]["StockNum"].ToString();
        this.Context.Response.Write(stockNum);

        #endregion
    }


    /// <summary>
    /// 取得公用檔參數
    /// </summary>
    /// <param name="valKind"></param>
    /// <param name="valName"></param>
    /// <returns></returns>
    private string GetParamData(string valKind, string valName)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT Param_Value");
            SBSql.AppendLine(" FROM Param_Public WITH (NOLOCK)");
            SBSql.AppendLine(" WHERE (Param_Kind = @Param_Kind) AND (Param_Name = @Param_Name)");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Param_Kind", valKind);
            cmd.Parameters.AddWithValue("Param_Name", valName);

            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                return DT.Rows[0]["Param_Value"].ToString(); ;
            }
        }
    }


    #region -- StoredProcedure --

    /// <summary>
    /// 從ERP取得報價價格, 使用StoreProcedure
    /// 此函式可輸入多筆品號及數量，並回傳已過濾的資料
    /// </summary>
    /// <param name="aryCustID">ERP客戶編號</param>
    /// <param name="aryModelNo">品號</param>
    /// <param name="aryDBS">客戶DBS</param>
    /// <param name="aryQty">數量</param>
    /// <returns></returns>
    public DataTable GetQuotePrice(ArrayList aryCustID, ArrayList aryModelNo, ArrayList aryDBS, ArrayList aryQty
        , out string ErrMsg)
    {
        try
        {
            //[判斷參數] - 判斷是否有傳入值
            if (aryModelNo.Count == 0)
            {
                if (aryCustID.Count == 0 || aryDBS.Count == 0)
                {
                    ErrMsg = "「ERP客戶編號」或「品號」或「客戶DBS」未傳入資料";
                    return null;
                }
            }
            //[判斷參數] - 判斷傳入值是否全部為空
            if (aryCustID.Count == 0 && aryModelNo.Count == 0 && aryDBS.Count == 0)
            {
                ErrMsg = "資料傳遞錯誤";
                return null;
            }

            //Array轉換字串
            string strCustID = (aryCustID.Count > 0) ? string.Join(",", aryCustID.ToArray()) : "";
            string strModelNo = (aryModelNo.Count > 0) ? string.Join(",", aryModelNo.ToArray()) : "";
            string strDBS = (aryDBS.Count > 0) ? string.Join(",", aryDBS.ToArray()) : "";
            string strQty = (aryQty.Count > 0) ? string.Join(",", aryQty.ToArray()) : "";

            //查詢StoreProcedure (myPrc_GetQuotePrice_v1)
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = 60;
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "myPrc_GetFilterQuotePrice";
                cmd.Parameters.AddWithValue("CustIDs", string.IsNullOrEmpty(strCustID) ? DBNull.Value : (Object)strCustID);
                cmd.Parameters.AddWithValue("ModelNos", string.IsNullOrEmpty(strModelNo) ? DBNull.Value : (Object)strModelNo);
                cmd.Parameters.AddWithValue("Qty", string.IsNullOrEmpty(strQty.ToString()) ? DBNull.Value : (Object)strQty);
                cmd.Parameters.AddWithValue("DBS", string.IsNullOrEmpty(strDBS) ? DBNull.Value : (Object)strDBS);
                //取得回傳值, 輸出參數
                SqlParameter Msg = cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 200);
                Msg.Direction = ParameterDirection.Output;

                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "查無資料";
                        return null;
                    }

                    //SQL回傳訊息
                    ErrMsg = Msg.Value.ToString();

                    //回傳資料集
                    return DT;
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return null;
        }
    }


    /// <summary>
    /// 從ERP取得庫存可用量, 使用StoreProcedure
    /// myPrc_GetStock
    /// </summary>
    /// <param name="aryStockType">庫別</param>
    /// <param name="aryModelNo">品號</param>
    /// <param name="aryDBS">DBS</param>
    /// <returns></returns>
    /// <remarks>
    /// output欄位：
    /// DBS varchar(10) NOT NULL			--[資料來源DBS]
    ///	, ModelNo nvarchar(40) NOT NULL		--[品號]
    ///	, StockTypes nvarchar(5) NOT NULL	--[庫別]
    ///	, StockNum int NOT NULL				--[庫存可用量]
    /// </remarks>
    public DataTable GetStock(ArrayList aryStockType, ArrayList aryModelNo, ArrayList aryDBS
      , out string ErrMsg)
    {
        try
        {
            //[判斷參數] - 判斷是否有傳入值
            if (aryDBS.Count == 0)
            {
                ErrMsg = "「DBS」未傳入資料";
                return null;
            }
            //[判斷參數] - 判斷傳入值是否全部為空
            if (aryStockType.Count == 0 && aryModelNo.Count == 0 && aryDBS.Count == 0)
            {
                ErrMsg = "資料傳遞錯誤";
                return null;
            }

            //Array轉換字串
            string strStockType = (aryStockType.Count > 0) ? string.Join(",", aryStockType.ToArray()) : "";
            string strModelNo = (aryModelNo.Count > 0) ? string.Join(",", aryModelNo.ToArray()) : "";
            string strDBS = (aryDBS.Count > 0) ? string.Join(",", aryDBS.ToArray()) : "";

            //查詢StoreProcedure (myPrc_GetStock)
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "myPrc_GetStock";
                cmd.Parameters.AddWithValue("StockTypes", string.IsNullOrEmpty(strStockType) ? DBNull.Value : (Object)strStockType);
                cmd.Parameters.AddWithValue("ModelNos", string.IsNullOrEmpty(strModelNo) ? DBNull.Value : (Object)strModelNo);
                cmd.Parameters.AddWithValue("DBS", string.IsNullOrEmpty(strDBS) ? DBNull.Value : (Object)strDBS);
                //取得回傳值, 輸出參數
                SqlParameter Msg = cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 200);
                Msg.Direction = ParameterDirection.Output;

                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "查無庫存資料";
                        return null;
                    }

                    //SQL回傳訊息
                    ErrMsg = Msg.Value.ToString();

                    //回傳資料集
                    return DT;
                }
            }
        }
        catch (Exception)
        {
            ErrMsg = "系統發生錯誤 - 無法取得庫存可用量";
            return null;
        }
    }



    /// <summary>
    /// 從ERP取得庫存資訊, 使用StoreProcedure
    /// </summary>
    /// <param name="aryStockType">庫別</param>
    /// <param name="aryModelNo">品號</param>
    /// <param name="aryDBS">DBS</param>
    /// <returns></returns>
    /// <remarks>
    /// output欄位：
    ///  DBS varchar(10) NOT NULL			--[資料來源DBS]
    /// 	, ModelNo nvarchar(40) NOT NULL		--[品號]
    /// 	, StockTypes nvarchar(5) NOT NULL	--[庫別]
    /// 	, INV_Num int NOT NULL				--[庫存]
    /// 	, INV_PreOut int NOT NULL			--[預計銷]
    /// 	, INV_Safe int NOT NULL				--[安全存量]
    /// 	, INV_PreIn int NOT NULL			--[預計進]
    /// 	, StockNum int NOT NULL				--[庫存可用量]
    /// </remarks>
    public DataTable GetStockInfo(ArrayList aryStockType, ArrayList aryModelNo, ArrayList aryDBS
       , out string ErrMsg)
    {
        try
        {
            //[判斷參數] - 判斷是否有傳入值
            if (aryDBS.Count == 0)
            {
                ErrMsg = "「DBS」未傳入資料";
                return null;
            }
            //[判斷參數] - 判斷傳入值是否全部為空
            if (aryStockType.Count == 0 && aryModelNo.Count == 0 && aryDBS.Count == 0)
            {
                ErrMsg = "資料傳遞錯誤";
                return null;
            }

            //Array轉換字串
            string strStockType = (aryStockType.Count > 0) ? string.Join(",", aryStockType.ToArray()) : "";
            string strModelNo = (aryModelNo.Count > 0) ? string.Join(",", aryModelNo.ToArray()) : "";
            string strDBS = (aryDBS.Count > 0) ? string.Join(",", aryDBS.ToArray()) : "";

            //查詢StoreProcedure (myPrc_GetStock)
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "myPrc_GetStockInfo";
                cmd.Parameters.AddWithValue("StockTypes", string.IsNullOrEmpty(strStockType) ? DBNull.Value : (Object)strStockType);
                cmd.Parameters.AddWithValue("ModelNos", string.IsNullOrEmpty(strModelNo) ? DBNull.Value : (Object)strModelNo);
                cmd.Parameters.AddWithValue("DBS", string.IsNullOrEmpty(strDBS) ? DBNull.Value : (Object)strDBS);
                //取得回傳值, 輸出參數
                SqlParameter Msg = cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 200);
                Msg.Direction = ParameterDirection.Output;

                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "查無庫存資料";
                        return null;
                    }

                    //SQL回傳訊息
                    ErrMsg = Msg.Value.ToString();

                    //回傳資料集
                    return DT;
                }
            }
        }
        catch (Exception)
        {
            ErrMsg = "系統發生錯誤 - 無法取得庫存資訊";
            return null;
        }
    }


    /// <summary>
    /// 新增資料至EDI (多筆)
    /// </summary>
    /// <param name="myDS">EDI欄位Table</param>
    /// <param name="TestMode">是否為測試模式</param>
    /// <param name="ErrMsg"></param>
    /// <returns>bool</returns>
    /// <remarks>
    /// 適用:
    /// TWBBC / SZBBC / ToyBBC
    /// 歷程:
    /// 20191203, 新增XA034(自訂序號)
    /// </remarks>
    public bool Insert(DataSet myDS, string TestMode, out string ErrMsg)
    {
        try
        {
            //[判斷參數] - 判斷是否有傳入值
            if (myDS == null)
            {
                ErrMsg = "EDI欄位資料為空";
                return false;
            }

            //將DataSet 轉 DataTable
            DataTable myData = myDS.Tables["MyEDITable"];

            #region ** EDI資料新增 **
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                for (int row = 0; row < myData.Rows.Count; row++)
                {
                    sbSQL.Append("INSERT INTO EDIXA( ");
                    sbSQL.Append("XA001, XA003, XA004, XA005, XA006, XA007, XA017");
                    sbSQL.Append(", XA002, XA008, XA009, XA010");
                    sbSQL.Append(", XA011, XA012, XA013, XA014, XA015, XA016, XA018");
                    sbSQL.Append(", XA020, XA021, XA022, XA023, XA024, XA025, XA026");
                    sbSQL.Append(", XA027, XA028, XA031, XA032, XA033, XA034");
                    sbSQL.Append(", CREATE_DATE");
                    sbSQL.Append(" ) VALUES ( ");
                    sbSQL.Append("@XA001_{0}, @XA003, NULL, NULL, @XA006_{0}, @XA007, @XA017".FormatThis(row));
                    sbSQL.Append(", @XA002_{0}, @XA008_{0}, @XA009_{0}, @XA010_{0}".FormatThis(row));
                    sbSQL.Append(", @XA011_{0}, @XA012_{0}, @XA013_{0}, @XA014_{0}, @XA015_{0}, @XA016_{0}, 0".FormatThis(row));
                    sbSQL.Append(", @XA020_{0}, @XA021_{0}, @XA022_{0}, @XA023_{0}, @XA024_{0}, @XA025_{0}, @XA026_{0}".FormatThis(row));
                    sbSQL.Append(", @XA027_{0}, @XA028_{0}, @XA031_{0}, @XA032_{0}, @XA033_{0}, @XA034_{0}".FormatThis(row));
                    sbSQL.AppendLine(", @CREATE_DATE);".FormatThis(row));

                    cmd.Parameters.AddWithValue("XA001_{0}".FormatThis(row), myData.Rows[row]["XA001"].ToString());
                    cmd.Parameters.AddWithValue("XA002_{0}".FormatThis(row), myData.Rows[row]["XA002"].ToString());
                    cmd.Parameters.AddWithValue("XA006_{0}".FormatThis(row), myData.Rows[row]["XA006"].ToString());
                    cmd.Parameters.AddWithValue("XA008_{0}".FormatThis(row), myData.Rows[row]["XA008"].ToString());
                    cmd.Parameters.AddWithValue("XA009_{0}".FormatThis(row), myData.Rows[row]["XA009"].ToString());
                    cmd.Parameters.AddWithValue("XA010_{0}".FormatThis(row), myData.Rows[row]["XA010"].ToString());
                    cmd.Parameters.AddWithValue("XA011_{0}".FormatThis(row), myData.Rows[row]["XA011"].ToString());
                    cmd.Parameters.AddWithValue("XA012_{0}".FormatThis(row), myData.Rows[row]["XA012"].ToString());
                    cmd.Parameters.AddWithValue("XA013_{0}".FormatThis(row), myData.Rows[row]["XA013"].ToString());
                    cmd.Parameters.AddWithValue("XA014_{0}".FormatThis(row), myData.Rows[row]["XA014"].ToString());
                    cmd.Parameters.AddWithValue("XA015_{0}".FormatThis(row), myData.Rows[row]["XA015"].ToString());
                    cmd.Parameters.AddWithValue("XA016_{0}".FormatThis(row), myData.Rows[row]["XA016"].ToString());

                    cmd.Parameters.AddWithValue("XA020_{0}".FormatThis(row), myData.Rows[row]["XA020"].ToString());
                    cmd.Parameters.AddWithValue("XA021_{0}".FormatThis(row), myData.Rows[row]["XA021"].ToString());
                    cmd.Parameters.AddWithValue("XA022_{0}".FormatThis(row), myData.Rows[row]["XA022"].ToString());
                    cmd.Parameters.AddWithValue("XA023_{0}".FormatThis(row), myData.Rows[row]["XA023"].ToString());
                    cmd.Parameters.AddWithValue("XA024_{0}".FormatThis(row), myData.Rows[row]["XA024"].ToString());
                    cmd.Parameters.AddWithValue("XA025_{0}".FormatThis(row), myData.Rows[row]["XA025"].ToString());
                    cmd.Parameters.AddWithValue("XA026_{0}".FormatThis(row), myData.Rows[row]["XA026"].ToString());
                    cmd.Parameters.AddWithValue("XA027_{0}".FormatThis(row), myData.Rows[row]["XA027"].ToString());
                    cmd.Parameters.AddWithValue("XA028_{0}".FormatThis(row), myData.Rows[row]["XA028"].ToString());
                    cmd.Parameters.AddWithValue("XA031_{0}".FormatThis(row), myData.Rows[row]["XA031"].ToString());
                    cmd.Parameters.AddWithValue("XA032_{0}".FormatThis(row), myData.Rows[row]["XA032"].ToString());
                    cmd.Parameters.AddWithValue("XA033_{0}".FormatThis(row), myData.Rows[row]["XA033"].ToString());

                    string mySID = myData.Rows[row]["XA034"].ToString();
                    cmd.Parameters.AddWithValue("XA034_{0}".FormatThis(row), string.IsNullOrWhiteSpace(mySID) ? "9999" : mySID);
                }

                //[SQL] - SQL Source
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.AddWithValue("XA003", myData.Rows[0]["XA003"].ToString());
                cmd.Parameters.AddWithValue("XA007", myData.Rows[0]["XA007"].ToString());
                cmd.Parameters.AddWithValue("XA017", myData.Rows[0]["XA017"].ToString());
                cmd.Parameters.AddWithValue("CREATE_DATE", DateTime.Now.ToString().ToDateString("yyyyMMdd"));

                //[SQL] - 判斷是否為測試模式, 執行SQL
                if (TestMode.Equals("Y"))
                {
                    return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
                }
                else
                {
                    return dbConn.ExecuteSql(cmd, dbConn.DBS.ERP_DSCSYS, out ErrMsg);
                }
            }

            #endregion
        }
        catch (Exception ex)
        {
            ErrMsg = "系統發生錯誤 - 資料無法寫入EDI(3)(WS);" + ex.Message.ToString();
            return false;
        }
    }


    /// <summary>
    /// 新增資料至EDI (多筆)
    /// </summary>
    /// <param name="myDS">EDI欄位Table</param>
    /// <param name="ErrMsg"></param>
    /// <returns>bool</returns>
    /// <remarks>
    /// 適用:EF報價匯入
    /// 
    /// </remarks>
    public bool InsertQuote(DataSet myDS, out string ErrMsg)
    {
        try
        {
            //[判斷參數] - 判斷是否有傳入值
            if (myDS == null)
            {
                ErrMsg = "EDI欄位資料為空";
                return false;
            }

            //將DataSet 轉 DataTable
            DataTable myData = myDS.Tables["MyEDIQuoteTable"];

            #region ** EDI資料新增 **
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                for (int row = 0; row < myData.Rows.Count; row++)
                {
                    sbSQL.Append("INSERT INTO EDIXA( ");
                    sbSQL.Append("XA001, XA003, XA004, XA005, XA006, XA007, XA017");
                    sbSQL.Append(", XA002, XA008, XA009, XA010");
                    sbSQL.Append(", XA011, XA012, XA013, XA014, XA015, XA016, XA018");
                    sbSQL.Append(", XA020, XA021, XA022, XA023, XA024, XA025, XA026");
                    sbSQL.Append(", XA027, XA028, XA031, XA032, XA033, XA034");
                    sbSQL.Append(", CREATE_DATE");
                    sbSQL.Append(" ) VALUES ( ");
                    sbSQL.Append("@XA001_{0}, @XA003, @XA004_{0}, NULL, @XA006_{0}, @XA007, @XA017".FormatThis(row));
                    sbSQL.Append(", @XA002_{0}, @XA008_{0}, @XA009_{0}, @XA010_{0}".FormatThis(row));
                    sbSQL.Append(", @XA011_{0}, @XA012_{0}, @XA013_{0}, @XA014_{0}, @XA015_{0}, @XA016_{0}, 0".FormatThis(row));
                    sbSQL.Append(", @XA020_{0}, @XA021_{0}, @XA022_{0}, @XA023_{0}, @XA024_{0}, @XA025_{0}, @XA026_{0}".FormatThis(row));
                    sbSQL.Append(", @XA027_{0}, @XA028_{0}, @XA031_{0}, @XA032_{0}, @XA033_{0}, @XA034_{0}".FormatThis(row));
                    sbSQL.AppendLine(", @CREATE_DATE);".FormatThis(row));

                    cmd.Parameters.AddWithValue("XA001_{0}".FormatThis(row), myData.Rows[row]["XA001"].ToString());
                    cmd.Parameters.AddWithValue("XA002_{0}".FormatThis(row), myData.Rows[row]["XA002"].ToString());
                    cmd.Parameters.AddWithValue("XA004_{0}".FormatThis(row), myData.Rows[row]["XA004"].ToString());
                    cmd.Parameters.AddWithValue("XA006_{0}".FormatThis(row), myData.Rows[row]["XA006"].ToString());
                    cmd.Parameters.AddWithValue("XA008_{0}".FormatThis(row), myData.Rows[row]["XA008"].ToString());
                    cmd.Parameters.AddWithValue("XA009_{0}".FormatThis(row), myData.Rows[row]["XA009"].ToString());
                    cmd.Parameters.AddWithValue("XA010_{0}".FormatThis(row), myData.Rows[row]["XA010"].ToString());
                    cmd.Parameters.AddWithValue("XA011_{0}".FormatThis(row), myData.Rows[row]["XA011"].ToString());
                    cmd.Parameters.AddWithValue("XA012_{0}".FormatThis(row), myData.Rows[row]["XA012"].ToString());
                    cmd.Parameters.AddWithValue("XA013_{0}".FormatThis(row), myData.Rows[row]["XA013"].ToString());
                    cmd.Parameters.AddWithValue("XA014_{0}".FormatThis(row), myData.Rows[row]["XA014"].ToString());
                    cmd.Parameters.AddWithValue("XA015_{0}".FormatThis(row), myData.Rows[row]["XA015"].ToString());
                    cmd.Parameters.AddWithValue("XA016_{0}".FormatThis(row), myData.Rows[row]["XA016"].ToString());

                    cmd.Parameters.AddWithValue("XA020_{0}".FormatThis(row), myData.Rows[row]["XA020"].ToString());
                    cmd.Parameters.AddWithValue("XA021_{0}".FormatThis(row), myData.Rows[row]["XA021"].ToString());
                    cmd.Parameters.AddWithValue("XA022_{0}".FormatThis(row), myData.Rows[row]["XA022"].ToString());
                    cmd.Parameters.AddWithValue("XA023_{0}".FormatThis(row), myData.Rows[row]["XA023"].ToString());
                    cmd.Parameters.AddWithValue("XA024_{0}".FormatThis(row), myData.Rows[row]["XA024"].ToString());
                    cmd.Parameters.AddWithValue("XA025_{0}".FormatThis(row), myData.Rows[row]["XA025"].ToString());
                    cmd.Parameters.AddWithValue("XA026_{0}".FormatThis(row), myData.Rows[row]["XA026"].ToString());
                    cmd.Parameters.AddWithValue("XA027_{0}".FormatThis(row), myData.Rows[row]["XA027"].ToString());
                    cmd.Parameters.AddWithValue("XA028_{0}".FormatThis(row), myData.Rows[row]["XA028"].ToString());
                    cmd.Parameters.AddWithValue("XA031_{0}".FormatThis(row), myData.Rows[row]["XA031"].ToString());
                    cmd.Parameters.AddWithValue("XA032_{0}".FormatThis(row), myData.Rows[row]["XA032"].ToString());
                    cmd.Parameters.AddWithValue("XA033_{0}".FormatThis(row), myData.Rows[row]["XA033"].ToString());

                    string mySID = myData.Rows[row]["XA034"].ToString();
                    cmd.Parameters.AddWithValue("XA034_{0}".FormatThis(row), string.IsNullOrWhiteSpace(mySID) ? "9999" : mySID);
                }

                //[SQL] - SQL Source
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.AddWithValue("XA003", myData.Rows[0]["XA003"].ToString());
                cmd.Parameters.AddWithValue("XA007", myData.Rows[0]["XA007"].ToString());
                cmd.Parameters.AddWithValue("XA017", myData.Rows[0]["XA017"].ToString());
                cmd.Parameters.AddWithValue("CREATE_DATE", DateTime.Now.ToString().ToDateString("yyyyMMdd"));

                //[SQL] - 執行SQL
                return dbConn.ExecuteSql(cmd, dbConn.DBS.ERP_DSCSYS, out ErrMsg);
            }

            #endregion
        }
        catch (Exception ex)
        {
            ErrMsg = "系統發生錯誤 - 資料無法寫入EDI(3)(WS);" + ex.Message.ToString();
            return false;
        }
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 系統通用Token
    /// </summary>
    //private string _TokenID;
    //public string TokenID
    //{
    //    get
    //    {
    //        return System.Web.Configuration.WebConfigurationManager.AppSettings["API_TokenID"];
    //    }
    //    set
    //    {
    //        this._TokenID = value;
    //    }
    //}

    #endregion

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam_Item
    {
        /// <summary>
        /// [參數] - 編號
        /// </summary>
        private string _tmp_ID;
        public string tmp_ID
        {
            get { return this._tmp_ID; }
            set { this._tmp_ID = value; }
        }

        /// <summary>
        /// [參數] - 數量
        /// </summary>
        private int _tmp_Qty;
        public int tmp_Qty
        {
            get { return this._tmp_Qty; }
            set { this._tmp_Qty = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        public TempParam_Item(string tmp_ID)
        {
            this._tmp_ID = tmp_ID;
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        /// <param name="tmp_Qty">數量</param>
        public TempParam_Item(string tmp_ID, int tmp_Qty)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Qty = tmp_Qty;
        }
    }
}
