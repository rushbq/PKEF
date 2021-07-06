using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinqToExcel;
using PKLib_Method.Methods;
using TWBBC_Mall.Models;

/*
 * [TW BBC Mall匯入]
 * 主要資料庫:prokit2
 */
namespace TWBBC_Mall.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        TraceID = 3,
        Status = 4,
        DateType = 5,  //要放在sDate, eDate之前
        StartDate = 6,
        EndDate = 7,
        Mall = 8
    }


    /// <summary>
    /// [BBC] 類別參數
    /// </summary>
    /// <remarks>
    /// 1:商城, 2:狀態
    /// </remarks>
    public enum myClass : int
    {
        mall = 1,
        status = 2
    }


    public class TWBBCMallRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        #region >> BBC匯入 <<
        /// <summary>
        /// [BBC] 取得匯入資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<ImportData> GetDataList(out string ErrMsg)
        {
            return GetDataList(null, out ErrMsg);
        }


        /// <summary>
        /// [BBC] 取得匯入資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ImportData> GetDataList(Dictionary<int, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ImportData> dataList = new List<ImportData>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, out ErrMsg))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new ImportData
                    {
                        Data_ID = item.Field<Guid>("Data_ID"),
                        TraceID = item.Field<string>("TraceID"),
                        MallID = item.Field<int>("MallID"),
                        CustID = item.Field<string>("CustID"),
                        Status = item.Field<Int16>("Status"),
                        Upload_File = item.Field<string>("Upload_File"),
                        Upload_ShipFile = item.Field<string>("Upload_ShipFile"),
                        Sheet_Name = item.Field<string>("Sheet_Name"),
                        Sheet_ShipName = item.Field<string>("Sheet_ShipName"),
                        MallName = item.Field<string>("MallName"),
                        CustName = item.Field<string>("CustName"),
                        StatusName = item.Field<string>("StatusName"),
                        LogCnt = item.Field<int>("LogCnt"),

                        Import_Time = item.Field<DateTime?>("Import_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                        Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                        Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),

                        Create_Who = item.Field<string>("Create_Who"),
                        Update_Who = item.Field<string>("Update_Who"),
                        Create_Name = item.Field<string>("Create_Name"),
                        Update_Name = item.Field<string>("Update_Name")

                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得類別
        /// </summary>
        /// <param name="cls">type</param>
        /// <returns></returns>
        public IQueryable<RefClass> GetClassList(myClass cls)
        {
            //----- 宣告 -----
            List<RefClass> dataList = new List<RefClass>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData_Status(cls))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new RefClass
                    {
                        ID = item.Field<int>("ID"),
                        Label = item.Field<string>("Label")
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 取得匯入資料 - 單身
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetDetailList(string parentID)
        {
            //----- 宣告 -----
            List<RefColumn> dataList = new List<RefColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Data_ID, OrderID");
                sql.AppendLine(" , ProdID, BuyCnt, BuyPrice, TotalPrice, IsPass, ISNULL(doWhat, '') doWhat");
                sql.AppendLine(" , ERP_ModelNo, ERP_Price, Currency");
                sql.AppendLine(" , ShipWho, ShipAddr, ShipTel, IsGift");
                sql.AppendLine(" , ISNULL(PromoID, '') AS RelPromoID, ISNULL(PromoName, '') AS RelPromoName");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData_DT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY OrderID, Data_ID");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefColumn
                        {
                            Data_ID = item.Field<int>("Data_ID"),
                            OrderID = item.Field<string>("OrderID"),
                            ProdID = item.Field<string>("ProdID"),
                            BuyCnt = item.Field<int>("BuyCnt"),
                            BuyPrice = item.Field<double>("BuyPrice"),
                            TotalPrice = item.Field<double>("TotalPrice"),
                            IsPass = item.Field<string>("IsPass"),
                            doWhat = item.Field<string>("doWhat"),
                            ERP_ModelNo = item.Field<string>("ERP_ModelNo"),
                            ERP_Price = item.Field<double>("ERP_Price"),
                            Currency = item.Field<string>("Currency"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            IsGift = item.Field<string>("IsGift"),
                            PromoID = item.Field<string>("RelPromoID"),
                            PromoName = item.Field<string>("RelPromoName")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 取得匯入暫存資料 - 群組化單頭
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetTempDTGroup(string parentID, out string ErrMsg)
        {
            //----- 宣告 -----
            List<RefColumn> dataList = new List<RefColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT OrderID, TotalPrice, ShipWho, ShipAddr, ShipTel");
                sql.AppendLine(" , BuyRemark, SellRemark");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData_TempDT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (ShipWho <> '') AND (Parent_ID = @ParentID)");
                sql.AppendLine(" GROUP BY OrderID, TotalPrice, ShipWho, ShipAddr, ShipTel, BuyRemark, SellRemark");
                sql.AppendLine(" ORDER BY OrderID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefColumn
                        {
                            OrderID = item.Field<string>("OrderID"),
                            TotalPrice = item.Field<double>("TotalPrice"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            BuyRemark = item.Field<string>("BuyRemark"),
                            SellRemark = item.Field<string>("SellRemark")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 取得匯入暫存資料 - 所有品項
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetTempDTItems(string parentID, out string ErrMsg)
        {
            //----- 宣告 -----
            List<RefColumn> dataList = new List<RefColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Data_ID, OrderID, ProdID, ProdSpec, ProdName, BuyCnt");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData_TempDT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY OrderID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefColumn
                        {
                            Data_ID = item.Field<Int32>("Data_ID"),
                            OrderID = item.Field<string>("OrderID"),
                            ProdID = item.Field<string>("ProdID"),
                            ProdSpec = item.Field<string>("ProdSpec"),
                            ProdName = item.Field<string>("ProdName"),
                            BuyCnt = item.Field<Int32>("BuyCnt")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 取得匯入Log資料
        /// </summary>
        /// <param name="dataID">ID</param>
        /// <returns>IQueryable<RefLog></returns>
        public IQueryable<RefLog> GetLogList(string dataID)
        {
            //----- 宣告 -----
            List<RefLog> dataList = new List<RefLog>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT *");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData_Log WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Data_ID = @Data_ID)");
                sql.AppendLine(" ORDER BY Create_Time DESC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefLog
                        {
                            Log_ID = item.Field<Int64>("Log_ID"),
                            TraceID = item.Field<string>("TraceID"),
                            Log_Desc = item.Field<string>("Log_Desc"),
                            Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 反查ERP訂單
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ERPOrderData> GetERPData_Order(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ERPOrderData> dataList = new List<ERPOrderData>();
            StringBuilder sql = new StringBuilder();
            string _dbName = "prokit2";

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("  COPTC.TC001, COPTC.TC002, COPTC.TC003, COPTD.TD004, COPTD.TD005, COPTD.TD007, ROUND(COPTD.TD008, 0) TD008");
                sql.AppendLine(" FROM [{0}].dbo.COPTC WITH(NOLOCK) INNER JOIN [{0}].dbo.COPTD WITH(NOLOCK)".FormatThis(_dbName));
                sql.AppendLine("  ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y')");

                /* Search */
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    //查詢內容
                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "TraceID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (COPTC.TC202 = @TraceID)");

                                    cmd.Parameters.AddWithValue("TraceID", item.Value);
                                }

                                break;
                        }
                    }
                }

                sql.AppendLine(" ORDER BY COPTC.TC001, COPTC.TC002, COPTD.TD004");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ERPOrderData
                        {
                            TC001 = item.Field<string>("TC001"),
                            TC002 = item.Field<string>("TC002"),
                            TD004 = item.Field<string>("TD004"),    //品號
                            TD005 = item.Field<string>("TD005"),    //品名
                            TD007 = item.Field<string>("TD007"),    //庫別
                            TD008 = Convert.ToInt16(item.Field<Decimal?>("TD008")),  //數量
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 反查ERP銷貨單
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ERPOrderData> GetERPData_Sales(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ERPOrderData> dataList = new List<ERPOrderData>();
            StringBuilder sql = new StringBuilder();
            string _dbName = "prokit2";

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----

                sql.AppendLine(" SELECT");
                //--訂單資訊
                sql.AppendLine(" COPTC.TC001, COPTC.TC002, COPTC.TC003, COPTD.TD004, COPTD.TD005, COPTD.TD007, ROUND(COPTD.TD008, 0) TD008");
                //--銷貨單資訊
                sql.AppendLine(" , COPTH.TH001, COPTH.TH002, COPTH.TH004, COPTH.TH005, COPTH.TH007, ROUND(COPTH.TH008, 0) TH008");
                sql.AppendLine(" FROM [{0}].dbo.COPTC WITH(NOLOCK)".FormatThis(_dbName));
                sql.AppendLine("  INNER JOIN [{0}].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002".FormatThis(_dbName));
                sql.AppendLine("  INNER JOIN [{0}].dbo.COPTH WITH(NOLOCK) ON COPTH.TH014 = COPTC.TC001 AND COPTH.TH015 = COPTC.TC002 AND COPTH.TH016 = COPTD.TD003".FormatThis(_dbName));
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y')");

                /* Search */
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    //查詢內容
                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "TraceID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (COPTC.TC202 = @TraceID)");

                                    cmd.Parameters.AddWithValue("TraceID", item.Value);
                                }

                                break;
                        }
                    }
                }

                sql.AppendLine(" ORDER BY COPTC.TC001, COPTC.TC002, COPTD.TD004");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ERPOrderData
                        {
                            TC001 = item.Field<string>("TC001"),
                            TC002 = item.Field<string>("TC002"),
                            TD004 = item.Field<string>("TD004"),    //品號
                            TD005 = item.Field<string>("TD005"),    //品名
                            TD007 = item.Field<string>("TD007"),    //庫別
                            TD008 = Convert.ToInt16(item.Field<Decimal?>("TD008")),  //數量

                            TH001 = item.Field<string>("TH001"),
                            TH002 = item.Field<string>("TH002"),
                            TH004 = item.Field<string>("TH004"),    //品號
                            TH005 = item.Field<string>("TH005"),    //品名
                            TH007 = item.Field<string>("TH007"),    //庫別
                            TH008 = Convert.ToInt16(item.Field<Decimal?>("TH008"))   //數量
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] EDI轉入排程LOG
        /// </summary>
        /// <param name="traceID">平台追蹤編號</param>
        /// <returns></returns>
        public IQueryable<EDILog> GetEDILog(string traceID, out string ErrMsg)
        {
            //----- 宣告 -----
            List<EDILog> dataList = new List<EDILog>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(XA006) OrderID, RTRIM(XA011) ModelNo, XA019 Why");
                sql.AppendLine(" FROM [DSCSYS].dbo.EDIXA WITH(NOLOCK)");
                sql.AppendLine(" WHERE (XA025 = @TraceID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("TraceID", traceID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new EDILog
                        {
                            OrderID = item.Field<string>("OrderID"),
                            ModelNo = item.Field<string>("ModelNo"),
                            Why = item.Field<string>("Why")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }



        /// <summary>
        /// [BBC] 取得對應客戶代號
        /// </summary>
        /// <param name="search">查詢參數(暫未使用)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<CustRef> GetRefCust(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<CustRef> dataList = new List<CustRef>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Platform_ID, ERP_ID, DispType");
                sql.AppendLine(" FROM TWBBC_Mall_RefTable");
                sql.AppendLine(" WHERE (1=1)");

                #region >> filter <<
                //if (search != null)
                //{
                //    //過濾空值
                //    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                //    foreach (var item in thisSearch)
                //    {
                //        switch (item.Key)
                //        {
                //            case "DataID":
                //                sql.Append(" AND (ID = @ID)");

                //                cmd.Parameters.AddWithValue("DataID", item.Value);
                //                break;

                //        }
                //    }
                //}
                #endregion

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new CustRef
                        {
                            Platform_ID = item.Field<string>("Platform_ID"),
                            ERP_ID = item.Field<string>("ERP_ID"),
                            DispType = item.Field<string>("DispType")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 取得Excel內容
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <returns></returns>
        /// <example>
        /// <table id="listTable" class="ui celled compact table nowrap" style="width:100%;">
        ///     <asp:Literal ID="lt_DataHtml" runat="server"><thead><tr><th>工作表未選擇</th></tr></thead></asp:Literal>
        /// </table>
        /// </example>
        public StringBuilder GetExcel_Html(string filePath, string sheetName)
        {
            try
            {
                //宣告
                StringBuilder html = new StringBuilder();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);

                //[HTML] - 取得欄位, 輸出標題欄 (GetColumnNames)
                var queryCols = excelFile.GetColumnNames(sheetName);

                html.Append("<thead>");
                html.Append("<tr>");
                foreach (var col in queryCols)
                {
                    html.Append("<th class=\"grey-bg lighten-3\">{0}</th>".FormatThis(col.ToString()));
                }
                html.Append("</tr>");
                html.Append("</thead>");


                //[處理合併儲存格] - 暫存欄:OrderID
                string tmp_OrderID = "";

                //[HTML] - 取得欄位值, 輸出內容欄 (Worksheet)
                var queryVals = excelFile.Worksheet(sheetName);

                html.Append("<tbody>");
                foreach (var val in queryVals)
                {
                    //[處理合併儲存格] - 目前的OrderID
                    string curr_OrderID = val[0].ToString();

                    //[處理合併儲存格] - 目前欄位非空值, 填入暫存值
                    if (!string.IsNullOrEmpty(curr_OrderID))
                    {
                        tmp_OrderID = curr_OrderID;
                    }

                    //內容迴圈
                    html.Append("<tr>");

                    int myCol = 0;
                    foreach (var col in queryCols)
                    {
                        //訂單單號為欄位的第一欄
                        if (myCol.Equals(0))
                        {
                            //OrderID:若目前欄位為空值,則填入暫存值
                            html.Append("<td>{0}</td>".FormatThis(string.IsNullOrEmpty(curr_OrderID) ? tmp_OrderID : curr_OrderID));
                        }
                        else
                        {
                            html.Append("<td>{0}</td>".FormatThis(val[col]));
                        }

                        myCol++;
                    }

                    html.Append("</tr>");
                }

                html.Append("</tbody>");

                //output
                return html;
            }
            catch (Exception)
            {

                throw new Exception("請檢查Excel格式是否正確!! 不同的商城對應不同的格式。");
            }
        }


        /// <summary>
        /// [BBC] 取得Excel欄位 - Step2讀取(Postback)
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="mallID">商城</param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetExcel_DT(string filePath, string sheetName, string mallID)
        {
            try
            {
                //----- 宣告 -----
                List<RefColumn> dataList = new List<RefColumn>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string myOrderID = "";
                string myProdID = "";
                string myProdSpec = "";
                string myProdName = "";
                int myBuyCnt = 1;
                double myBuyPrice = 0;
                double myTotalPrice = 0;
                string myShipNo = "";  //物流單號/出貨單號
                string myShipWho = "";
                string myShipAddr = "";
                string myShipTel = "";
                string myNickName = "";
                string myBuyRemark = ""; //買家備註
                string mySellRemark = ""; //賣家備註
                string myBuy_ProdName = ""; //平台產品名稱
                string myBuy_Place = ""; //分配機構
                string myBuy_Warehouse = ""; //倉庫
                string myBuy_Sales = ""; //採購員
                string myBuy_Time = ""; //訂購時間
                string myCustOrderID = ""; //會員訂單


                //資料迴圈
                foreach (var val in queryVals)
                {
                    #region >> 欄位處理:其他欄位 <<

                    switch (mallID)
                    {
                        case "1":
                            //Costco訂單檔
                            myCustOrderID = val[0]; //會員訂單
                            myProdID = val[9]; //商品ID                            
                            myProdName = val[11]; //商品名
                            myBuyCnt = string.IsNullOrEmpty(val[12]) ? 1 : Convert.ToInt16(val[12]); //數量
                            myNickName = val[4]; //會員名
                            myShipNo = val[0]; //會員訂單(與出貨檔關聯)
                            myOrderID = val[8]; //商城訂單號(與出貨檔關聯)
                            myBuy_Time = val[3];

                            break;

                        default:


                            break;
                    }

                    #endregion


                    //加入項目(key = ShipmentNo+OrderID)
                    var data = new RefColumn
                    {
                        OrderID = myOrderID.Trim(),
                        CustOrderID = myCustOrderID.Trim(),
                        ProdID = myProdID.Trim(),
                        ProdSpec = myProdSpec.Trim(),
                        ProdName = myProdName.Trim(),
                        BuyCnt = myBuyCnt,
                        BuyPrice = myBuyPrice,
                        TotalPrice = myTotalPrice,
                        ShipmentNo = myShipNo.Trim(),
                        ShipWho = myShipWho.Replace("'", "").Trim(),
                        ShipAddr = myShipAddr.Replace("'", "").Trim(),
                        ShipTel = myShipTel.Replace("'", "").Trim(),
                        NickName = myNickName.Trim(),
                        Buy_ProdName = myBuy_ProdName.Trim(),
                        Buy_Place = myBuy_Place.Trim(),
                        Buy_Warehouse = myBuy_Warehouse.Trim(),
                        Buy_Sales = myBuy_Sales.Trim(),
                        Buy_Time = myBuy_Time.Trim(),
                        BuyRemark = myBuyRemark.Trim(),
                        SellRemark = mySellRemark.Trim()
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }


                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception ex)
            {

                throw new Exception("請檢查Excel格式是否正確(順序/屬性),各商城格式不同." + ex.Message.ToString());
            }
        }

        /// <summary>
        /// [BBC] 取得Excel欄位 - Step2讀取(出貨檔)(Postback)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="mallID"></param>
        /// <returns></returns>
        /// <remarks>
        /// 適用商城:1=Costco
        /// </remarks>
        public IQueryable<RefColumn> GetExcel_ShipDT(string filePath, string sheetName, string mallID)
        {
            try
            {
                //----- 宣告 -----
                List<RefColumn> dataList = new List<RefColumn>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string myShipNo = "";  //出貨單號
                string myShipWho = "";
                string myShipAddr = "";
                string myShipTel = "";
                string myOrderNo = ""; //訂單號碼

                //資料迴圈
                foreach (var val in queryVals)
                {
                    #region >> 欄位處理:其他欄位 <<

                    switch (mallID)
                    {
                        case "1":
                            //Costco出貨檔
                            myShipNo = val[0]; //出貨單號(會員訂單)(與訂單檔關聯)
                            myShipWho = val[1]; //收貨人
                            myShipTel = val[4]; //電話
                            myShipAddr = val[6] + val[7] + val[8] + val[9]; //收貨地址(區號+路名+街號)
                            myOrderNo = val[12]; //訂單號碼(與訂單檔關聯)

                            break;

                        default:

                            break;
                    }

                    #endregion


                    //加入項目(key = ShipmentNo+OrderID)
                    var data = new RefColumn
                    {
                        ShipmentNo = myShipNo.Trim(),
                        ShipWho = myShipWho.Replace("'", "").Trim(),
                        ShipAddr = myShipAddr.Replace("'", "").Trim(),
                        ShipTel = myShipTel.Replace("'", "").Trim(),
                        OrderID = myOrderNo.Trim()
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }


                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception ex)
            {

                throw new Exception("請檢查出貨資料格式是否正確(順序/屬性),各商城格式不同." + ex.Message.ToString());
            }
        }


        /// <summary>
        /// [BBC] 取得活動贈品內容-單身
        /// </summary>
        /// <param name="mallID">商城ID</param>
        /// <param name="parentID">BBC匯入檔DataID</param>
        /// <param name="promoType">1:滿額送贈品 / 2:買指定商品送贈品</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetPromoItems(string mallID, string parentID, string promoType, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    switch (promoType)
                    {
                        case "1":
                            sql.AppendLine(" ;WITH TblPromo AS (");
                            //--取得活動內容:Type=1
                            sql.AppendLine(" SELECT Base.Data_ID AS PromoID, Base.PromoName, Base.TargetMoney AS PromoTarget");
                            sql.AppendLine(" FROM TWBBC_Mall_Promo Base");
                            sql.AppendLine(" WHERE (Base.StartTime <= GETDATE()) AND (Base.EndTime >= GETDATE())");
                            sql.AppendLine("  AND (Base.MallID = @MallID)");
                            sql.AppendLine("  AND (Base.PromoType = 1)");
                            sql.AppendLine(" )");
                            sql.AppendLine(" , TblGroup AS (");
                            //--取得平台單號:判斷匯入資料(總金額>=活動價格)
                            sql.AppendLine(" SELECT OrderID, TotalPrice, TblPromo.PromoID, TblPromo.PromoName");
                            sql.AppendLine(" , ROW_NUMBER() OVER (PARTITION BY OrderID ORDER BY OrderID, TblPromo.PromoTarget DESC) AS RowRank");
                            sql.AppendLine(" FROM TWBBC_Mall_ImportData_DT, TblPromo");
                            sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (ProdID <> 'W001') AND (TotalPrice >= TblPromo.PromoTarget)");
                            sql.AppendLine(" GROUP BY OrderID, TotalPrice, TblPromo.PromoID, TblPromo.PromoName, TblPromo.PromoTarget");
                            sql.AppendLine(" )");
                            //--取得贈品:組合成要新增的資料
                            sql.AppendLine(" SELECT");
                            sql.AppendLine(" DT.OrderID, GiftDT.ModelNo AS ProdID, GiftDT.Qty AS BuyCnt");
                            sql.AppendLine(" , 0 AS BuyPrice, DT.TotalPrice AS TotalPrice");
                            sql.AppendLine(" , GiftDT.ModelNo AS ERP_ModelNo");
                            sql.AppendLine(" , 'NTD' AS Currency, DT.ShipWho, DT.ShipAddr, DT.ShipTel, 'Y' AS IsGift");
                            sql.AppendLine(" , TblGroup.PromoID, TblGroup.PromoName");
                            sql.AppendLine(" FROM TWBBC_Mall_ImportData_DT DT");
                            sql.AppendLine("  INNER JOIN TblGroup ON DT.OrderID = TblGroup.OrderID");
                            sql.AppendLine("  INNER JOIN TWBBC_Mall_Promo_DT GiftDT ON TblGroup.PromoID = GiftDT.Parent_ID");
                            sql.AppendLine(" WHERE (DT.Parent_ID = @ParentID) AND (DT.ProdID <> 'W001') AND (DT.ShipWho <> '') AND (TblGroup.RowRank = 1)");
                            sql.AppendLine(" GROUP BY DT.Parent_ID, DT.OrderID, DT.TotalPrice, DT.ShipAddr, DT.ShipWho, DT.ShipTel, GiftDT.ModelNo, GiftDT.Qty, TblGroup.PromoID, TblGroup.PromoName");

                            break;

                        default:
                            sql.AppendLine(" ;WITH TblPromo AS (");
                            //--取得活動內容:指定商品
                            sql.AppendLine(" SELECT Base.Data_ID AS PromoID, Base.PromoName, Base.TargetItem AS PromoTarget");
                            sql.AppendLine(" FROM TWBBC_Mall_Promo Base");
                            sql.AppendLine(" WHERE (Base.StartTime <= GETDATE()) AND (Base.EndTime >= GETDATE())");
                            sql.AppendLine("  AND (Base.MallID = @MallID)");
                            sql.AppendLine("  AND (Base.PromoType = 2)");
                            sql.AppendLine(" )");
                            sql.AppendLine(" , TblGroup AS (");
                            //--取得平台單號:判斷匯入資料(購買商品對應)
                            sql.AppendLine(" SELECT OrderID, ERP_ModelNo, TblPromo.PromoID, TblPromo.PromoName");
                            sql.AppendLine(" FROM TWBBC_Mall_ImportData_DT");
                            sql.AppendLine("  INNER JOIN TblPromo ON ERP_ModelNo = TblPromo.PromoTarget");
                            sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (ProdID <> 'W001')");
                            sql.AppendLine(" GROUP BY OrderID, ERP_ModelNo, TblPromo.PromoID, TblPromo.PromoName");
                            sql.AppendLine(" )");
                            //--取得贈品:組合成要新增的資料
                            sql.AppendLine(" SELECT ");
                            sql.AppendLine(" DT.OrderID, GiftDT.ModelNo AS ProdID, GiftDT.Qty AS BuyCnt");
                            sql.AppendLine(" , 0 AS BuyPrice, DT.TotalPrice AS TotalPrice");
                            sql.AppendLine(" , GiftDT.ModelNo AS ERP_ModelNo");
                            sql.AppendLine(" , 'NTD' AS Currency, DT.ShipWho, DT.ShipAddr, DT.ShipTel, 'Y' AS IsGift");
                            sql.AppendLine(" , TblGroup.PromoID, TblGroup.PromoName");
                            sql.AppendLine(" FROM TWBBC_Mall_ImportData_DT DT");
                            sql.AppendLine("  INNER JOIN TblGroup ON DT.OrderID = TblGroup.OrderID");
                            sql.AppendLine("  INNER JOIN TWBBC_Mall_Promo_DT GiftDT ON TblGroup.PromoID = GiftDT.Parent_ID");
                            sql.AppendLine(" WHERE (DT.Parent_ID = @ParentID) AND (DT.ProdID <> 'W001') AND (DT.ShipWho <> '')");
                            sql.AppendLine(" GROUP BY DT.Parent_ID, DT.OrderID, DT.TotalPrice, DT.ShipAddr, DT.ShipWho, DT.ShipTel, GiftDT.ModelNo, GiftDT.Qty, TblGroup.PromoID, TblGroup.PromoName");

                            break;

                    }

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("MallID", mallID);
                    cmd.Parameters.AddWithValue("ParentID", parentID);


                    //----- 資料取得 -----
                    return dbConn.LookupDT(cmd, out ErrMsg);

                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return null;

            }

        }


        /// <summary>
        /// [BBC] 取得活動設定單頭
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<PromoBase> GetPromoConfig(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<PromoBase> dataList = new List<PromoBase>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.Data_ID, Base.PromoName, Base.MallID");
                sql.AppendLine(" , Base.StartTime, Base.EndTime, Base.PromoType, Base.TargetMoney, Base.TargetItem");
                sql.AppendLine(" , Cls.Class_Name AS MallName");
                sql.AppendLine(" , Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                sql.AppendLine(" , (SELECT COUNT(*) FROM TWBBC_Mall_Promo_DT WHERE (Parent_ID = Base.Data_ID)) AS ChildCnt");
                sql.AppendLine(" FROM TWBBC_Mall_Promo Base");
                sql.AppendLine("  INNER JOIN TWBBC_Mall_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine(" WHERE (1=1)");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "DataID":
                                sql.Append(" AND (Base.Data_ID = @DataID)");

                                cmd.Parameters.AddWithValue("DataID", item.Value);
                                break;

                            case "Keyword":
                                sql.Append(" AND (");
                                sql.Append("  (UPPER(Base.PromoName) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);
                                break;

                            case "Mall":
                                sql.Append(" AND (Base.MallID = @MallID)");

                                cmd.Parameters.AddWithValue("MallID", item.Value);
                                break;

                            case "sDate":
                                sql.Append(" AND (Base.StartTime <= @startDate) AND (Base.EndTime >= @startDate)");

                                cmd.Parameters.AddWithValue("startDate", item.Value.ToDateString("yyyy/MM/dd 00:00:00"));
                                break;

                            //case "eDate":
                            //    sql.Append(" AND (Base.EndTime >= @endDate)");

                            //    cmd.Parameters.AddWithValue("endDate", item.Value.ToDateString("yyyy/MM/dd 23:59:59"));
                            //    break;

                        }
                    }
                }
                #endregion


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new PromoBase
                        {
                            Data_ID = item.Field<Guid>("Data_ID"),
                            MallID = item.Field<int>("MallID"),
                            MallName = item.Field<string>("MallName"),
                            PromoName = item.Field<string>("PromoName"),
                            PromoType = item.Field<Int16>("PromoType"),
                            TypeName = getPromoTypeName(item.Field<Int16>("PromoType")),
                            TargetMoney = item.Field<double?>("TargetMoney"),
                            TargetItem = item.Field<string>("TargetItem"),
                            StartTime = item.Field<DateTime>("StartTime").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            EndTime = item.Field<DateTime>("EndTime").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            ChildCnt = item.Field<Int32>("ChildCnt"),

                            Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                            Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                            Create_Who = item.Field<string>("Create_Who"),
                            Update_Who = item.Field<string>("Update_Who"),
                            Create_Name = item.Field<string>("Create_Name"),
                            Update_Name = item.Field<string>("Update_Name")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        /// <summary>
        /// 回傳活動類型名稱(1:滿額送贈品 / 2:買指定商品送贈品)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string getPromoTypeName(Int16 type)
        {
            switch (type)
            {
                case 1:
                    return "滿額送贈品";

                default:
                    return "買指定商品送贈品";
            }
        }


        /// <summary>
        /// [BBC] 取得活動設定單身
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <returns></returns>
        public IQueryable<PromoDT> GetPromoConfigDetail(string parentID, out string ErrMsg)
        {
            //----- 宣告 -----
            List<PromoDT> dataList = new List<PromoDT>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Data_ID, ModelNo, Qty");
                sql.AppendLine(" FROM TWBBC_Mall_Promo_DT");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY ModelNo");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new PromoDT
                        {
                            Data_ID = item.Field<int>("Data_ID"),
                            ModelNo = item.Field<string>("ModelNo"),
                            Qty = item.Field<int>("Qty")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC] 自訂客戶商品對應
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<RefModel> GetRefModelList(Dictionary<string, string> search, int sort, out string ErrMsg)
        {
            //----- 宣告 -----
            List<RefModel> dataList = new List<RefModel>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.Data_ID, Base.MG002, Base.MG003, Base.MG006");
                sql.AppendLine(" FROM refCOPMG Base");
                sql.AppendLine(" WHERE (DB = 'TW')");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "DataID":
                                sql.Append(" AND (Base.Data_ID = @DataID)");

                                cmd.Parameters.AddWithValue("DataID", item.Value);
                                break;

                            case "Keyword":
                                sql.Append(" AND (");
                                sql.Append("  (UPPER(Base.MG002) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("  OR (UPPER(Base.MG003) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);
                                break;

                            case "Mall":
                                sql.Append(" AND (Base.MallID = @MallID)");

                                cmd.Parameters.AddWithValue("MallID", item.Value);
                                break;

                            case "Cust":
                                sql.Append(" AND (Base.MG001 = @CustID)");

                                cmd.Parameters.AddWithValue("CustID", item.Value);
                                break;

                        }
                    }
                }
                #endregion

                if (sort.Equals(1))
                {
                    //List
                    sql.AppendLine(" ORDER BY Base.MG002, Base.MG003");
                }
                else
                {
                    //Add
                    sql.AppendLine(" ORDER BY Base.Data_ID DESC");
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefModel
                        {
                            Data_ID = item.Field<Int64>("Data_ID"),
                            ModelNo = item.Field<string>("MG002"),
                            CustModelNo = item.Field<string>("MG003"),
                            CustSpec = item.Field<string>("MG006")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        #endregion



        #region >> 出貨明細表 <<

        /// <summary>
        /// [BBC] 取得Excel欄位 - 出貨明細,匯入物流單號時使用
        /// 回傳Excel資料
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <returns>
        /// OrderID=銷貨單號
        /// ShipNo=物流單號
        /// </returns>
        public IQueryable<RefColumn> GetExcel_ShipNoData(string filePath, string sheetName)
        {
            try
            {
                //----- 宣告 -----
                List<RefColumn> dataList = new List<RefColumn>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string _OrderID = ""; //銷貨單號
                string _ShipNo = ""; //物流單號

                //資料迴圈
                foreach (var val in queryVals)
                {
                    _OrderID = val[2];
                    _ShipNo = val[14];

                    //加入項目
                    var data = new RefColumn
                    {
                        OrderID = _OrderID,
                        ShipmentNo = _ShipNo
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }

                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception ex)
            {

                throw new Exception("請檢查Excel格式是否正確!!" + ex.Message.ToString());
            }
        }


        /// <summary>
        /// [BBC出貨明細] 未匯出的資料
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<Shipment> GetShipmentData(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<Shipment> dataList = new List<Shipment>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" ;WITH TblERP AS (");
                sql.AppendLine(" SELECT");
                sql.AppendLine("  RTRIM(COPMA.MA001 COLLATE Chinese_Taiwan_Stroke_CI_AS) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , RTRIM(COPTH.TH001) + '-' + RTRIM(COPTH.TH002) AS Erp_SO_ID");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine("  , RTRIM(COPTD.TD004) COLLATE Chinese_Taiwan_Stroke_CI_AS AS ModelNo");
                sql.AppendLine("  , RTRIM(COPTD.TD014) COLLATE Chinese_Taiwan_Stroke_CI_AS AS CustModelNo");
                sql.AppendLine("  , CONVERT(INT, CASE WHEN COPTH.TH008 = 0 THEN COPTH.TH024 ELSE COPTH.TH008 END) AS BuyCnt");
                sql.AppendLine("  , CONVERT(FLOAT, COPTG.TG045 + COPTG.TG046) AS TotalPrice");
                sql.AppendLine(" FROM [prokit2].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTG WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                //條件:網路訂單 / 排除W001
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTD.TD004 NOT IN ('W001'))");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT Base.Data_ID SrcParentID, DT.Data_ID SrcDataID");
                sql.AppendLine("  , TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , Cls.Class_Name AS MallName, DT.OrderID");
                sql.AppendLine("  , ROW_NUMBER() OVER (PARTITION BY DT.OrderID ORDER BY DT.OrderID, DT.ShipmentNo ASC) AS RowRank");
                sql.AppendLine("  , DT.ERP_ModelNo");
                sql.AppendLine("  , TblERP.BuyCnt");
                sql.AppendLine("  , TblERP.Erp_SO_ID");
                sql.AppendLine("  , ISNULL(DT.ShipmentNo, '') AS ShipNo");
                sql.AppendLine("  , DT.ShipWho, DT.ShipAddr, DT.ShipTel");
                sql.AppendLine("  , TblERP.TotalPrice AS TotalPrice");
                sql.AppendLine(" FROM [PKEF].dbo.TWBBC_Mall_ImportData AS Base");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_ImportData_DT AS DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND DT.ERP_ModelNo = TblERP.ModelNo AND Base.CustID = TblERP.CustID");
                sql.AppendLine(" WHERE (DT.IsPass = 'Y')");

                //條件:已匯過的不顯示
                sql.AppendLine("  AND (DT.OrderID NOT IN(");
                sql.AppendLine(" 	SELECT RelDT.OrderID");
                sql.AppendLine(" 	FROM [PKEF].dbo.TWBBC_Mall_ShipExport Rel");
                sql.AppendLine(" 	 INNER JOIN [PKEF].dbo.TWBBC_Mall_ShipExport_DT RelDT ON Rel.Data_ID = RelDT.Parent_ID");
                sql.AppendLine("  ))");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "CustID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.CustID = @CustID)");

                                    cmd.Parameters.AddWithValue("CustID", item.Value);
                                }

                                break;

                            case "sDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Import_Time >= @startDate)");

                                    cmd.Parameters.AddWithValue("startDate", item.Value);
                                }

                                break;

                            case "eDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Import_Time <= @endDate)");

                                    cmd.Parameters.AddWithValue("endDate", item.Value);
                                }

                                break;

                            case "doExport":
                                //是否為匯出
                                if (item.Value.Equals("Y"))
                                {
                                    sql.Append(" AND (DT.ShipmentNo <> '')");
                                }

                                break;

                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY DT.ShipmentNo, DT.OrderID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 180;

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Shipment
                        {
                            RowRank = item.Field<Int64>("RowRank"),
                            SrcParentID = item.Field<Guid>("SrcParentID"),
                            SrcDataID = item.Field<int>("SrcDataID"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            MallName = item.Field<string>("MallName"),
                            OrderID = item.Field<string>("OrderID"),
                            ModelNo = item.Field<string>("ERP_ModelNo"),
                            BuyCnt = item.Field<int>("BuyCnt"),
                            Erp_SO_ID = item.Field<string>("Erp_SO_ID"),
                            ShipNo = item.Field<string>("ShipNo"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            TotalPrice = item.Field<double>("TotalPrice")

                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC出貨明細] 已匯出記錄
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ShipExport> GetShipmentHistory(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ShipExport> dataList = new List<ShipExport>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.SeqNo, Base.Data_ID, Base.CustID, RTRIM(COPMA.MA002) AS CustName, Base.Create_Time");
                sql.AppendLine("  , Base.sDate, Base.eDate");
                sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine(" FROM [PKEF].dbo.TWBBC_Mall_ShipExport Base");
                sql.AppendLine("  LEFT JOIN [prokit2].dbo.COPMA WITH(NOLOCK) ON Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN = COPMA.MA001");
                sql.AppendLine(" WHERE (1=1)");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "CustID":
                                //CustID
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.CustID = @CustID)");

                                    cmd.Parameters.AddWithValue("CustID", item.Value);
                                }

                                break;

                            case "sDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Create_Time >= @startDate)");

                                    cmd.Parameters.AddWithValue("startDate", item.Value);
                                }

                                break;

                            case "eDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Create_Time <= @endDate)");

                                    cmd.Parameters.AddWithValue("endDate", item.Value);
                                }

                                break;

                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY Base.Create_Time DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ShipExport
                        {
                            SeqNo = item.Field<int>("SeqNo"),
                            Data_ID = item.Field<Guid>("Data_ID"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            sDate = item.Field<DateTime>("sDate").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            eDate = item.Field<DateTime>("eDate").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Create_Name = item.Field<string>("Create_Name")

                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [BBC出貨明細] 已匯出的資料明細
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<Shipment> GetShipmentHistoryDetail(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<Shipment> dataList = new List<Shipment>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" ;WITH TblERP AS (");
                sql.AppendLine(" SELECT");
                sql.AppendLine("  RTRIM(COPMA.MA001 COLLATE Chinese_Taiwan_Stroke_CI_AS) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , RTRIM(COPTH.TH001) + '-' + RTRIM(COPTH.TH002) AS Erp_SO_ID");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine("  , RTRIM(COPTD.TD004) COLLATE Chinese_Taiwan_Stroke_CI_AS AS ModelNo");
                sql.AppendLine("  , RTRIM(COPTD.TD014) COLLATE Chinese_Taiwan_Stroke_CI_AS AS CustModelNo");
                sql.AppendLine("  , CONVERT(INT, CASE WHEN COPTH.TH008 = 0 THEN COPTH.TH024 ELSE COPTH.TH008 END) AS BuyCnt");
                sql.AppendLine("  , CONVERT(FLOAT, COPTG.TG045 + COPTG.TG046) AS TotalPrice");
                sql.AppendLine(" FROM [prokit2].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTG WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                //條件:網路訂單 / 排除W001
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTD.TD004 NOT IN ('W001'))");
                //條件:關聯,節省效能
                sql.AppendLine(" AND (EXISTS (");
                sql.AppendLine(" SELECT * FROM [PKEF].[dbo].[TWBBC_Mall_ShipExport_DT]");
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine("  AND (OrderID = COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS)");
                sql.AppendLine("  AND (ModelNo = RTRIM(COPTD.TD004) COLLATE Chinese_Taiwan_Stroke_CI_AS)");
                sql.AppendLine(" ))");

                sql.AppendLine(" )");
                sql.AppendLine(" SELECT DT.Data_ID");
                sql.AppendLine("  , TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , Cls.Class_Name AS MallName, DT.OrderID");
                sql.AppendLine("  , ROW_NUMBER() OVER (PARTITION BY DT.OrderID ORDER BY DT.OrderID, DT.ShipmentNo ASC) AS RowRank");
                sql.AppendLine("  , DT.ERP_ModelNo");
                sql.AppendLine("  , TblERP.BuyCnt");
                sql.AppendLine("  , TblERP.Erp_SO_ID");
                sql.AppendLine("  , ISNULL(DT.ShipmentNo, '') AS ShipNo");
                sql.AppendLine("  , DT.ShipWho, DT.ShipAddr, DT.ShipTel");
                sql.AppendLine("  , TblERP.TotalPrice AS TotalPrice");
                sql.AppendLine(" FROM [PKEF].dbo.TWBBC_Mall_ShipExport AS Rel");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_ShipExport_DT AS RelDT ON Rel.Data_ID = RelDT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_ImportData AS Base ON Base.Data_ID = RelDT.SrcParentID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_ImportData_DT AS DT ON Base.Data_ID = DT.Parent_ID AND DT.Data_ID = RelDT.SrcDataID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND DT.ERP_ModelNo = TblERP.ModelNo AND Base.CustID = TblERP.CustID");
                sql.AppendLine(" WHERE (DT.IsPass = 'Y')");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "DataID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Rel.Data_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY DT.ShipmentNo, DT.OrderID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 180;

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Shipment
                        {
                            RowRank = item.Field<Int64>("RowRank"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            MallName = item.Field<string>("MallName"),
                            OrderID = item.Field<string>("OrderID"),
                            ModelNo = item.Field<string>("ERP_ModelNo"),
                            BuyCnt = item.Field<int>("BuyCnt"),
                            Erp_SO_ID = item.Field<string>("Erp_SO_ID"),
                            ShipNo = item.Field<string>("ShipNo"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel")

                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }



        /// <summary>
        /// [BBC出貨明細] 物流名單下載 & 物流單號回寫用
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<Shipment> GetShipNameList(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<Shipment> dataList = new List<Shipment>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" ;WITH TblERP AS (");
                sql.AppendLine(" SELECT");
                sql.AppendLine("  RTRIM(COPMA.MA001 COLLATE Chinese_Taiwan_Stroke_CI_AS) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , RTRIM(COPTH.TH001) + '-' + RTRIM(COPTH.TH002) AS Erp_SO_ID");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine(" FROM [prokit2].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTG WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                //條件:網路訂單
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y')");
                sql.AppendLine(" GROUP BY COPMA.MA001, COPMA.MA002, RTRIM(COPTH.TH001) + '-' + RTRIM(COPTH.TH002), COPTC.TC012");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , Cls.Class_Name AS MallName");
                sql.AppendLine("  , Base.Data_ID AS SrcParentID, DT.OrderID, DT.CustOrderID");
                sql.AppendLine("  , TblERP.Erp_SO_ID");
                sql.AppendLine("  , ISNULL(DT.ShipmentNo, '') AS ShipNo");
                sql.AppendLine("  , DT.ShipWho, DT.ShipAddr, DT.ShipTel, DT.ERP_ModelNo");
                sql.AppendLine(" FROM [PKEF].dbo.TWBBC_Mall_ImportData AS Base");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_ImportData_DT AS DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.TWBBC_Mall_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND Base.CustID = TblERP.CustID");
                //條件:物流單號空白
                sql.AppendLine(" WHERE (DT.IsPass = 'Y') AND (DT.ShipmentNo = '')");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "CustID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.CustID = @CustID)");

                                    cmd.Parameters.AddWithValue("CustID", item.Value);
                                }

                                break;

                            case "sDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Import_Time >= @startDate)");

                                    cmd.Parameters.AddWithValue("startDate", item.Value);
                                }

                                break;

                            case "eDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Import_Time <= @endDate)");

                                    cmd.Parameters.AddWithValue("endDate", item.Value);
                                }

                                break;

                            case "ERP_SOID":
                                //銷貨單號集合

                                string[] aryID = Regex.Split(item.Value, ",");
                                ArrayList aryLst = new ArrayList(aryID);

                                //GetSQLParam:SQL WHERE IN的方法
                                sql.AppendLine(" AND (TblERP.Erp_SO_ID IN ({0}))".FormatThis(CustomExtension.GetSQLParam(aryLst, "params")));
                                for (int row = 0; row < aryLst.Count; row++)
                                {
                                    cmd.Parameters.AddWithValue("params" + row, aryLst[row]);
                                }

                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" GROUP BY TblERP.CustID, TblERP.CustName, Cls.Class_Name, Base.Data_ID, DT.OrderID, DT.CustOrderID");
                sql.AppendLine("  , TblERP.Erp_SO_ID, ISNULL(DT.ShipmentNo, ''), DT.ShipWho, DT.ShipAddr, DT.ShipTel, DT.ERP_ModelNo");
                sql.AppendLine(" ORDER BY DT.ShipWho, DT.ShipAddr, DT.OrderID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 120;

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Shipment
                        {
                            SrcParentID = item.Field<Guid>("SrcParentID"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            MallName = item.Field<string>("MallName"),
                            OrderID = item.Field<string>("OrderID"),
                            CustOrderID = item.Field<string>("CustOrderID"),
                            Erp_SO_ID = item.Field<string>("Erp_SO_ID"),
                            ShipNo = item.Field<string>("ShipNo"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            ModelNo = item.Field<string>("ERP_ModelNo")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        #endregion


        #region >> 平台對應 <<
        /// <summary>
        /// [BBC][平台對應] 反查ERP 訂單/銷貨單資料 - 單頭
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ERPDataList> GetERPDataList(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ERPDataList> dataList = new List<ERPDataList>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----

                sql.AppendLine(" ;WITH TblERP AS (");
                sql.AppendLine(" SELECT");
                sql.AppendLine("  RTRIM(COPMA.MA001 COLLATE Chinese_Taiwan_Stroke_CI_AS) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , COPTC.TC001, COPTC.TC002, COPTH.TH001, COPTH.TH002");
                sql.AppendLine("  , RTRIM(ACRTB.TB001) AS TB001, RTRIM(ACRTB.TB002) AS TB002");
                sql.AppendLine("  , CONVERT(FLOAT, COPTH.TH013) AS Price");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine("  , COPTD.TD004 COLLATE Chinese_Taiwan_Stroke_CI_AS AS ModelNo");
                sql.AppendLine("  , COPTD.TD014 COLLATE Chinese_Taiwan_Stroke_CI_AS AS CustModelNo");
                sql.AppendLine(" FROM [prokit2].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [prokit2].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                sql.AppendLine("  LEFT JOIN [prokit2].dbo.ACRTB WITH(NOLOCK) ON ACRTB.TB005 = COPTH.TH001 AND ACRTB.TB006 = COPTH.TH002");
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y')");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT DT.OrderID, Cls.Class_Name");
                sql.AppendLine("  , TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , TblERP.TC001, TblERP.TC002, TblERP.TH001, TblERP.TH002, TblERP.TB001, TblERP.TB002");
                sql.AppendLine("  , SUM(TblERP.Price) AS TotalPrice");
                sql.AppendLine("  , DT.ShipmentNo, DT.ShipTel");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData Base");
                sql.AppendLine("  INNER JOIN TWBBC_Mall_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN TWBBC_Mall_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND DT.ERP_ModelNo = TblERP.ModelNo AND Base.CustID = TblERP.CustID");
                sql.AppendLine(" WHERE (DT.IsPass = 'Y') ");

                #region >> filter <<
                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "Keyword":
                                sql.Append(" AND (");
                                sql.Append("  (UPPER(TblERP.OrderID) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("  OR (UPPER(TblERP.TC001 + TblERP.TC002) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("  OR (UPPER(TblERP.TH001 + TblERP.TH002) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("  OR (UPPER(DT.ShipmentNo) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("  OR (UPPER(DT.ShipTel) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);
                                break;

                            case "Mall":
                                sql.Append(" AND (Base.MallID = @MallID)");

                                cmd.Parameters.AddWithValue("MallID", item.Value);
                                break;

                            case "sDate":
                                sql.Append(" AND (Base.Import_Time >= @startDate)");

                                cmd.Parameters.AddWithValue("startDate", item.Value.ToDateString("yyyy/MM/dd 00:00:00"));
                                break;

                            case "eDate":
                                sql.Append(" AND (Base.Import_Time <= @endDate)");

                                cmd.Parameters.AddWithValue("endDate", item.Value.ToDateString("yyyy/MM/dd 23:59:59"));
                                break;

                        }
                    }
                }
                #endregion

                sql.AppendLine(" GROUP BY DT.OrderID, Cls.Class_Name, TblERP.CustID, TblERP.CustName, TblERP.TC001, TblERP.TC002, TblERP.TH001, TblERP.TH002");
                sql.AppendLine(" , TblERP.TB001, TblERP.TB002, DT.ShipmentNo, DT.ShipTel");
                sql.AppendLine(" ORDER BY TblERP.TH001, TblERP.TH002");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ERPDataList
                        {
                            OrderID = item.Field<string>("OrderID"),
                            MallName = item.Field<string>("Class_Name"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            ShipmentNo = item.Field<string>("ShipmentNo"),
                            ShipTel = item.Field<string>("ShipTel"),
                            TC001 = item.Field<string>("TC001"),
                            TC002 = item.Field<string>("TC002"),
                            TH001 = item.Field<string>("TH001"),
                            TH002 = item.Field<string>("TH002"),
                            TA001 = item.Field<string>("TB001"),
                            TA002 = item.Field<string>("TB002"),
                            TotalPrice = item.Field<double>("TotalPrice")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        #endregion

        #endregion



        #region -----// Create //-----

        #region >> BBC匯入 <<
        /// <summary>
        /// [BBC] 建立基本資料 - Step1執行
        /// Default Status = 10
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create(ImportData instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData( ");
                sql.AppendLine("  Data_ID, TraceID, MallID, CustID, Status, Upload_File, Upload_ShipFile");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @TraceID, @MallID, @CustID, 10, @Upload_File, @Upload_ShipFile");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("MallID", instance.MallID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("Upload_File", instance.Upload_File);
                cmd.Parameters.AddWithValue("Upload_ShipFile", instance.Upload_ShipFile);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] 建立單身暫存, 更新主檔欄位 - Step2執行
        /// </summary>
        /// <param name="baseData">匯入檔基本資料</param>
        /// <param name="query">Excel資料</param>
        /// <returns></returns>
        public bool Create_Temp(ImportData baseData, IQueryable<RefColumn> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE TWBBC_Mall_ImportData SET Status = 11, Sheet_Name = @Sheet_Name, Sheet_ShipName = @Sheet_ShipName, Update_Who = @Update_Who, Update_Time = GETDATE() WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in query)
                {
                    if (!string.IsNullOrEmpty(item.ProdID))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                        sql.AppendLine("  FROM TWBBC_Mall_ImportData_TempDT ");
                        sql.AppendLine("  WHERE Parent_ID = @DataID ");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_TempDT( ");
                        sql.AppendLine("  Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice");
                        sql.AppendLine("  , ShipWho, ShipAddr, ShipTel, ProdSpec, ProdName, NickName");

                        //其他補充欄位
                        sql.AppendLine("  , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                        sql.AppendLine("  , BuyRemark, SellRemark, CustOrderID");

                        sql.AppendLine(" ) VALUES (");

                        //col:Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice
                        sql.AppendLine("  @DataID, @NewID, '{0}', '{1}', {2}, {3}, {4}".FormatThis(
                            item.OrderID, item.ProdID.Trim(), item.BuyCnt
                            , item.BuyPrice == null ? 0 : item.BuyPrice
                            , item.TotalPrice == null ? 0 : item.TotalPrice
                            ));
                        //col:ShipWho, ShipAddr, ShipTel, ProdSpec, ProdName, NickName
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}'".FormatThis(
                            item.ShipWho, item.ShipAddr, item.ShipTel
                            , HttpUtility.HtmlEncode(item.ProdSpec), HttpUtility.HtmlEncode(item.ProdName), item.NickName
                            ));

                        //col:補充欄位
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}', N'{3}', N'{4}'".FormatThis(
                            HttpUtility.HtmlEncode(item.Buy_ProdName), item.Buy_Place, item.Buy_Warehouse, item.Buy_Sales, item.Buy_Time
                            ));
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}'".FormatThis(item.BuyRemark, item.SellRemark, item.CustOrderID));

                        sql.AppendLine(" );");

                    }
                }

                //處理合併欄位, 總金額為 0的欄位
                //sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_TempDT SET TotalPrice = ISNULL((");
                //sql.AppendLine("  SELECT TOP 1 Totalprice FROM TWBBC_Mall_ImportData_TempDT Ref WHERE (Ref.Parent_ID = @DataID)");
                //sql.AppendLine("    AND (Ref.OrderID = TWBBC_Mall_ImportData_TempDT.OrderID) AND (Ref.TotalPrice <> 0)");
                //sql.AppendLine(" ), 0)");
                //sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (TotalPrice = 0)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Sheet_Name", baseData.Sheet_Name);
                cmd.Parameters.AddWithValue("Sheet_ShipName", baseData.Sheet_ShipName);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] 建立Log
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="desc">描述</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_Log(ImportData baseData, string desc, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (");
                sql.AppendLine("  SELECT ISNULL(MAX(Log_ID), 0) + 1 AS NewID FROM TWBBC_Mall_ImportData_Log");
                sql.AppendLine(" )");

                sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_Log( ");
                sql.AppendLine("  Log_ID, Data_ID, TraceID, Log_Desc, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @NewID, @DataID, @TraceID, @Log_Desc, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", baseData.TraceID);
                cmd.Parameters.AddWithValue("Log_Desc", desc.Left(1500));

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] 建立EDI資料 - Step4執行
        /// SELECT EDI所需欄位資料, 並分批(50筆)呼叫Webservice(API_ErpData), 寫入EDI
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_EDI(ImportData baseData, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----

                //EDI欄位設定
                sql.AppendLine(" SELECT");
                sql.AppendLine("  Corp.Corp_ID AS XA001");
                sql.AppendLine(", '1' AS XA002"); //訂單轉銷單
                sql.AppendLine(", '2250' AS XA003"); //單別
                sql.AppendLine(", LEFT(DT.OrderID, 40) AS XA006");  //平台單號 (excel訂單號)
                sql.AppendLine(", Base.CustID AS XA007");
                sql.AppendLine(", RTRIM(Cust.MA016) AS XA008");   //ERP業務人員ID
                sql.AppendLine(", DT.Currency AS XA009");
                sql.AppendLine(", CONVERT(VARCHAR(8), Base.Create_Time, 112) AS XA010"); //單據日期(抓建立日)
                sql.AppendLine(", LEFT(DT.ERP_ModelNo, 40) AS XA011");
                sql.AppendLine(", (CASE WHEN DT.IsGift = 'Y' THEN 0 ELSE DT.BuyCnt END) AS XA012"); //數量
                sql.AppendLine(", (CASE WHEN DT.IsGift = 'Y' THEN 0 ELSE DT.ERP_NewPrice END) AS XA013"); //單價
                sql.AppendLine(", '22' AS XA014"); //庫別
                sql.AppendLine(" , LEFT(DT.ShipAddr, 250) AS XA015");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA016");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA017");
                sql.AppendLine(" , '1' AS XA020"); //贈備品
                sql.AppendLine(" , (CASE WHEN DT.IsGift = 'Y' THEN DT.BuyCnt ELSE 0 END) AS XA021"); //贈備品量
                sql.AppendLine(" , LEFT(DT.ShipWho, 30) AS XA022");
                sql.AppendLine(" , LEFT(DT.ShipTel, 20) AS XA023");
                sql.AppendLine(" , LEFT(('TW-BBC-' + MallCls.Class_Name), 20) AS XA024"); //來源平台
                sql.AppendLine(" , LEFT(Base.TraceID, 20) AS XA025");  //原始單號(填入追蹤編號)
                sql.AppendLine(" , LEFT(DT.ProdID, 30) AS XA026"); //SKU碼
                sql.AppendLine(" , '2' AS XA027"); //是否開發票(預設值)
                sql.AppendLine(" , '1' AS XA028"); //折讓或銷退(預設值)
                sql.AppendLine(" , '' AS XA031");
                sql.AppendLine(" , '' AS XA032");
                sql.AppendLine(" , '' AS XA033"); //銷退原因
                sql.AppendLine("  , RIGHT(('000' + CAST(DT.Data_ID AS VARCHAR(4))), 4) AS XA034"); //自訂序號
                sql.AppendLine(" FROM TWBBC_Mall_ImportData Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN TWBBC_Mall_ImportData_DT DT WITH(NOLOCK) ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN TWBBC_Mall_RefClass MallCls WITH(NOLOCK) ON Base.MallID = MallCls.Class_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustID = Cust.MA001 AND Cust.DBS = Cust.DBC");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Param_Corp Corp WITH(NOLOCK) ON Cust.DBC = Corp.Corp_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer_Data CustDT WITH(NOLOCK) ON Cust.MA001 = CustDT.Cust_ERPID");
                sql.AppendLine(" WHERE (Base.Data_ID = @DataID) AND (DT.IsPass = 'Y')");
                sql.AppendLine(" ORDER BY DT.OrderID, DT.Data_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "沒有可匯入的資料,請確認Step3的「即將匯入」是否有資料.";
                        return false;
                    }

                    //建立EDI資料 - API_ErpData (Local)
                    API_ERPData EDI = new API_ERPData();

                    #region -- 資料批次處理 --

                    string myError = "";

                    /* 每50筆呼叫一次API */
                    int totalRow = DT.Rows.Count;   //--總筆數
                    int batchNum = 50;  //--每區筆數
                    int section = (totalRow / batchNum);    //--迴圈數
                    int skipNum = 0;   //--略過筆數

                    for (int row = 0; row <= section; row++)
                    {
                        //重置略過筆數
                        if (row > 0)
                        {
                            skipNum = skipNum + batchNum;
                        }
                        //判斷略過筆數是否 大於 總筆數
                        if (skipNum > totalRow) { skipNum = totalRow; }

                        //query
                        var query = DT.AsEnumerable().Skip(skipNum).Take(batchNum);

                        if (query.Count() > 0)
                        {
                            //設定DataSet, 命名MyEDITable
                            using (DataSet myDS = new DataSet())
                            {
                                using (DataTable myDT = query.CopyToDataTable())
                                {
                                    myDT.TableName = "MyEDITable";
                                    myDS.Tables.Add(myDT);
                                }

                                //回傳Webservice (資料DataSet, Token, 測試模試(Y/N))
                                string _testMode = "N";
                                if (false == EDI.Insert(myDS, _testMode, out ErrMsg))
                                {
                                    myError += "列數:{0} ~ {1}發生錯誤...{2}\n".FormatThis(skipNum, skipNum + batchNum, ErrMsg);
                                }
                            }
                        }

                        query = null;

                    }

                    if (!string.IsNullOrEmpty(myError))
                    {
                        ErrMsg = myError;
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                    #endregion

                }
            }

        }


        /// <summary>
        /// [促銷活動] 建立單頭
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_Promo(PromoBase instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO TWBBC_Mall_Promo( ");
                sql.AppendLine("  Data_ID, PromoName, MallID, StartTime, EndTime");
                sql.AppendLine("  , PromoType, TargetMoney, TargetItem");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @PromoName, @MallID, @StartTime, @EndTime");
                sql.AppendLine("  , @PromoType, @TargetMoney, @TargetItem");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("PromoName", instance.PromoName);
                cmd.Parameters.AddWithValue("MallID", instance.MallID);
                cmd.Parameters.AddWithValue("StartTime", instance.StartTime.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("EndTime", instance.EndTime.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("PromoType", instance.PromoType);
                cmd.Parameters.AddWithValue("TargetMoney", instance.TargetMoney == 0 ? DBNull.Value : (object)instance.TargetMoney);
                cmd.Parameters.AddWithValue("TargetItem", instance.TargetItem);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// [促銷活動] 建立單身
        /// </summary>
        /// <param name="myData"></param>
        /// <param name="parentID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_PromoDT(PromoDT inst, string parentID, string updateWho, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE TWBBC_Mall_Promo SET");
                sql.AppendLine(" Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @Parent_ID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (");
                sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID FROM TWBBC_Mall_Promo_DT WHERE (Parent_ID = @Parent_ID)");
                sql.AppendLine(" );");

                sql.Append(" INSERT INTO TWBBC_Mall_Promo_DT( ");
                sql.Append("  Parent_ID, Data_ID, ModelNo, Qty");
                sql.Append(" ) VALUES (");
                sql.Append("  @Parent_ID, @NewID, @ModelNo, @Qty");
                sql.Append(" );");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", parentID);
                cmd.Parameters.AddWithValue("Update_Who", updateWho);
                cmd.Parameters.AddWithValue("ModelNo", inst.ModelNo);
                cmd.Parameters.AddWithValue("Qty", inst.Qty);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] 建立商品對應(refCOPMG)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_RefModel(RefModel instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO refCOPMG( ");
                sql.AppendLine("  MallID, MG001, MG002, MG003, MG006, DB");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @MallID, @MG001, @MG002, @MG003, @MG006, 'TW'");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("MallID", instance.MallID);
                cmd.Parameters.AddWithValue("MG001", instance.CustID);
                cmd.Parameters.AddWithValue("MG002", instance.ModelNo);
                cmd.Parameters.AddWithValue("MG003", instance.CustModelNo);
                cmd.Parameters.AddWithValue("MG006", instance.CustSpec);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] Step2.1手動新增品項
        /// </summary>
        /// <param name="parentID">單頭編號</param>
        /// <param name="orderID">平台單號</param>
        /// <param name="itemID">RefCOPMG.DataID</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_TempNewItem(string parentID, string orderID, int itemID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_TempDT(");
                sql.AppendLine("  Data_ID, Parent_ID, OrderID, ProdID, ProdSpec, ProdName, CustOrderID");
                sql.AppendLine("  , BuyCnt, BuyPrice, TotalPrice");
                sql.AppendLine("  , ShipWho, ShipAddr, ShipTel");
                sql.AppendLine("  , NickName, BuyRemark, SellRemark)");
                sql.AppendLine(" SELECT TOP 1 ");
                sql.AppendLine(" (");
                sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                sql.AppendLine("  FROM TWBBC_Mall_ImportData_TempDT");
                sql.AppendLine("  WHERE Parent_ID = @Parent_ID");
                sql.AppendLine(" ) AS Data_ID");
                sql.AppendLine(" , Parent_ID, OrderID");
                sql.AppendLine(" , Ref.MG003 AS ProdID, Ref.MG006 AS ProdSpec, '***手動新增***' AS ProdName, CustOrderID");
                sql.AppendLine(" , 1 AS BuyCnt, 0 AS BuyPrice, TotalPrice");
                sql.AppendLine(" , ShipWho, ShipAddr, ShipTel");
                sql.AppendLine(" , NickName, BuyRemark, SellRemark");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData_TempDT");
                sql.AppendLine("  LEFT JOIN [PKSYS].[dbo].[refCOPMG] Ref ON Ref.Data_ID = @ItemID AND Ref.DB = 'TW'");
                sql.AppendLine(" WHERE (Parent_ID  = @Parent_ID) AND (OrderID = @OrderID)");
                sql.AppendLine(" ORDER BY Data_ID DESC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", parentID);
                cmd.Parameters.AddWithValue("OrderID", orderID);
                cmd.Parameters.AddWithValue("ItemID", itemID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion


        #region >> 出貨明細表 <<

        /// <summary>
        /// [出貨明細] 建立匯出記錄 - 單頭
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_ShipExport(ShipExport instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO TWBBC_Mall_ShipExport( ");
                sql.AppendLine("  Data_ID, CustID, sDate, eDate, Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @CustID, @sDate, @eDate, @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("sDate", instance.sDate.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("eDate", instance.eDate.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [出貨明細] 建立匯出記錄 - 單身
        /// </summary>
        /// <param name="myData"></param>
        /// <param name="parentID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_ShipExportDT(IQueryable<ShipExportDT> myData, string parentID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in myData)
                {
                    sql.AppendLine(" SET @NewID = (");
                    sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID FROM TWBBC_Mall_ShipExport_DT WHERE (Parent_ID = @Parent_ID)");
                    sql.AppendLine(" );");

                    sql.AppendLine(" INSERT INTO TWBBC_Mall_ShipExport_DT( ");
                    sql.Append("  Parent_ID, Data_ID, OrderID, ModelNo, SrcParentID, SrcDataID");
                    sql.Append(" ) VALUES (");
                    sql.Append("  @Parent_ID, @NewID, '{0}', '{1}', '{2}', {3}".FormatThis(
                            item.OrderID, item.ModelNo, item.SrcParentID, item.SrcDataID
                        ));
                    sql.Append(" );");
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", parentID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion


        #endregion



        #region -----// Update //-----

        /// <summary>
        /// 回寫物流單號
        /// </summary>
        /// <param name="instance">excel來源資料</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 銷貨單號 = instance.OrderID;
        /// 物流單號 = instance.ShipmentNo;
        /// </remarks>
        public bool Update_ShipmentDT(IQueryable<RefColumn> instance, out string ErrMsg)
        {
            //銷貨單號集合 -- [A]
            string strSoID_Group = "";
            ArrayList ary = new ArrayList();
            foreach (var item in instance)
            {
                ary.Add(item.OrderID);
            }
            strSoID_Group = string.Join(",", ary.ToArray());
            if (string.IsNullOrWhiteSpace(strSoID_Group))
            {
                ErrMsg = "無法取得銷貨單號,請檢查上傳的Excel";
                return false;
            }


            //取得對應原始資料的ParentID, OrderID -- [B]
            Dictionary<string, string> search = new Dictionary<string, string>();

            search.Add("ERP_SOID", strSoID_Group);

            var _data = GetShipNameList(search, out ErrMsg);
            if (_data == null)
            {
                ErrMsg = "無法取得對應資料(GetShipNameList),請通知MIS";
                return false;
            }


            //將[B]與Excel Join, 以銷售單號對應物流單號 -- [C]
            var dataCombine = instance.Join(_data,
             pk => pk.OrderID,  //Excel中的OrderID (即銷貨單號)
             fk => fk.Erp_SO_ID,  //資料表中的銷貨單號
             (pk, fk) => new
             {
                 Erp_SO_ID = fk.Erp_SO_ID,
                 ParentID = fk.SrcParentID,
                 OrderID = fk.OrderID,
                 ShipNo = pk.ShipmentNo
             });


            //將[C]回寫至ShipmentNo
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                foreach (var item in dataCombine)
                {
                    sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT");
                    sql.AppendLine(" SET ShipmentNo = N'{0}' WHERE (Parent_ID = '{1}') AND (OrderID = '{2}');".FormatThis(
                         item.ShipNo, item.ParentID, item.OrderID
                        ));
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }



        #endregion



        #region -----// Delete //-----

        #region >> BBC匯入 <<
        /// <summary>
        /// [BBC] 刪除資料
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData WHERE (Data_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// [BBC] 清空暫存
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_Temp(string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData_TempDT WHERE (Parent_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// [促銷活動] 刪除設定檔
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_Promo(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM TWBBC_Mall_Promo_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM TWBBC_Mall_Promo WHERE (Data_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// [促銷活動] 刪除贈品
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_PromoDT(string parentID, string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM TWBBC_Mall_Promo_DT WHERE (Parent_ID = @Parent_ID) AND (Data_ID = @Data_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", parentID);
                cmd.Parameters.AddWithValue("Data_ID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// [BBC] 刪除商品對應(refCOPMG)
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_RefModel(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM refCOPMG WHERE (Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }
        }

        #endregion

        #endregion



        #region -----// BBC匯入Check //-----

        /// <summary>
        /// [BBC] Check.1 - ERP品號
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// Step2 時執行(after 下一步)
        /// *** 重要:資料庫欄位若有修改, 必須調整此區的SQL ***
        /// *** 重要:此處有寫死DB Name, 若有變動須注意 ***
        /// 暫存檔與COPMG對應比較, 更新 ERP_ModelNo
        /// , 不正常的資料 IsPass = 'N',doWhat = '查無ERP品號'
        /// , 其他IsPass = 'Y', 並Insert至 TWBBC_Mall_ImportData_DT
        /// </remarks>
        public bool Check_Step1(ImportData baseData, out string ErrMsg)
        {

            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----

                //clear
                sql.AppendLine(" DELETE FROM TWBBC_Mall_ImportData_DT WHERE (Parent_ID = @DataID);");

                /*
                 * [判斷商城]
                 * 1:Costco
                 */
                switch (baseData.MallID)
                {
                    //case 999:
                    //    //需要使用時要重新撰寫此區(目前為未修改的程式碼)
                    //    //特殊商城規格, 編號一對多:要再對應MG006(商品規格), 編號一對一,則排除MG006
                    //    //ERP.COPMG無法重複資料,對應至PKSYS.refCOPMG
                    //    sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_DT (");
                    //    sql.AppendLine(" Parent_ID, Data_ID");
                    //    sql.AppendLine(" , OrderID, ProdID, ProdSpec, BuyCnt, BuyPrice, TotalPrice");
                    //    sql.AppendLine(" , IsPass, doWhat, ERP_ModelNo, ERP_Price, ERP_NewPrice, Cnt_Price");
                    //    sql.AppendLine(" , Currency, ShipWho, ShipAddr, ShipTel");
                    //    sql.AppendLine(" , IsGift, Create_Time");
                    //    sql.AppendLine(" , NickName");
                    //    sql.AppendLine(" , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                    //    sql.AppendLine(" , BuyRemark, SellRemark");
                    //    sql.AppendLine(" )");
                    //    //--比對1:無商品規格的對應品號
                    //    sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                    //    sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.ProdSpec, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice");
                    //    sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                    //    sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                    //    sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                    //    sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                    //    sql.AppendLine("  , 'NTD' AS Currency, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel");
                    //    sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                    //    sql.AppendLine("  , Tmp.NickName");
                    //    sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                    //    sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                    //    sql.AppendLine(" FROM TWBBC_Mall_ImportData Base");
                    //    sql.AppendLine("  INNER JOIN TWBBC_Mall_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                    //    sql.AppendLine("  LEFT JOIN [PKSYS].dbo.refCOPMG Ref ON Ref.DB = 'TW' AND Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN)");
                    //    sql.AppendLine("   AND (ISNULL(Ref.MG006, '') = '')");
                    //    sql.AppendLine(" WHERE (Base.Data_ID = @DataID) AND (Ref.MallID = @MallID) AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");
                    //    sql.AppendLine(" UNION ALL");
                    //    //--比對2:有商品規格的對應品號
                    //    sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                    //    sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.ProdSpec, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice");
                    //    sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                    //    sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                    //    sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                    //    sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                    //    sql.AppendLine("  , 'NTD' AS Currency, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel");
                    //    sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                    //    sql.AppendLine("  , Tmp.RemarkTitle, '', '', '', Tmp.NickName");
                    //    sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                    //    sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                    //    sql.AppendLine(" FROM TWBBC_Mall_ImportData Base");
                    //    sql.AppendLine("  INNER JOIN TWBBC_Mall_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                    //    sql.AppendLine("  LEFT JOIN [PKSYS].dbo.refCOPMG Ref ON Ref.DB = 'TW' AND Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN)");
                    //    sql.AppendLine("   AND (Tmp.ProdSpec = (ISNULL(Ref.MG006, '') COLLATE Chinese_Taiwan_Stroke_BIN))");
                    //    sql.AppendLine(" WHERE (Base.Data_ID = @DataID) AND (Ref.MallID = @MallID) AND (Tmp.ProdSpec <> '') AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");

                    //    cmd.Parameters.AddWithValue("MallID", baseData.MallID);

                    //    break;


                    default:
                        //關聯[prokit2].dbo.COPMG
                        sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_DT (");
                        sql.AppendLine(" Parent_ID, Data_ID");
                        sql.AppendLine(" , OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice");
                        sql.AppendLine(" , IsPass, doWhat, ERP_ModelNo, ERP_Price, ERP_NewPrice, Cnt_Price");
                        sql.AppendLine(" , Currency, ShipWho, ShipAddr, ShipTel");
                        sql.AppendLine(" , IsGift, Create_Time");
                        sql.AppendLine(" , NickName, CustOrderID");
                        sql.AppendLine(" , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                        sql.AppendLine(" , BuyRemark, SellRemark");
                        sql.AppendLine(" )");
                        sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                        sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                        sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                        sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                        sql.AppendLine("  , 'NTD' AS Currency, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel");
                        sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                        sql.AppendLine("  , Tmp.NickName, Tmp.CustOrderID");
                        sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                        sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                        sql.AppendLine(" FROM TWBBC_Mall_ImportData Base");
                        sql.AppendLine("  INNER JOIN TWBBC_Mall_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                        sql.AppendLine("  LEFT JOIN [prokit2].dbo.COPMG Ref ON (Tmp.ProdID COLLATE Chinese_Taiwan_Stroke_BIN) = Ref.MG003 AND (Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN) = Ref.MG001");
                        sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                        sql.AppendLine(" ORDER BY Tmp.Data_ID");

                        break;
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] Check.2 - 重複的單號
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// Step2 時執行
        /// TWBBC_Mall_Import_DT 與其他不同Parent_ID的資料比對, 判斷是否有重複的OrderID + ProdID, 若有重複則 IsPass = 'N',doWhat = '重複的單號'
        /// </remarks>
        public bool Check_Step2(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT");
                sql.AppendLine(" SET IsPass = 'N', doWhat = '重複的單號'");
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine("  AND (Data_ID IN (");
                sql.AppendLine("     SELECT Base.Data_ID");
                sql.AppendLine("     FROM TWBBC_Mall_ImportData_DT Base");
                sql.AppendLine("     WHERE (Base.Parent_ID = @DataID)");
                sql.AppendLine("      AND EXISTS (");
                sql.AppendLine("       SELECT OrderID FROM TWBBC_Mall_ImportData_DT Chk");
                sql.AppendLine("       WHERE (Base.OrderID = Chk.OrderID) AND (Base.ProdID = Chk.ProdID) ");
                sql.AppendLine("        AND (Chk.Parent_ID <> @DataID) AND (Chk.IsPass = 'Y')");
                sql.AppendLine("      )");
                sql.AppendLine("  ))");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [BBC] Check.3
        /// 取得ERP價格, 並更新 ERP_Price
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// Step2 時執行
        /// </remarks>
        public bool Check_Step3(string dataID, out string ErrMsg)
        {
            //***** 取得必要參數CustID, DBS, SWID ******
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            string CustID, DBS, ModelNos, Qtys, MallID;

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.CustID, Base.MallID, Cust.DBS");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustID = Cust.MA001");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer_Data CustDT WITH(NOLOCK) ON Cust.MA001 = CustDT.Cust_ERPID");
                sql.AppendLine(" WHERE (Cust.DBS = Cust.DBC) AND (Base.Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "無法取得客戶資料,請確認客戶基本資料已設定.(Step2:Check3)";
                        return false;
                    }

                    //填入資料
                    CustID = DT.Rows[0]["CustID"].ToString();
                    DBS = DT.Rows[0]["DBS"].ToString();
                    MallID = DT.Rows[0]["MallID"].ToString();
                }
            }


            //***** 取得要取價的品號 ModelNos, Qtys ***** 
            List<String> iModelNo = new List<String>();
            List<String> iQty = new List<String>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                sql.Clear();
                cmd.Parameters.Clear();

                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ERP_ModelNo AS ModelNo, BuyCnt");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData_DT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (ERP_ModelNo IS NOT NULL) AND (IsGift = 'N')");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "請檢查ERP「客戶核價單」、「客戶品號建立」;若繼續報錯,請通知資訊部.(Step2:Check3)";
                        return false;
                    }

                    //填入資料
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        iModelNo.Add(DT.Rows[row]["ModelNo"].ToString());
                        iQty.Add(DT.Rows[row]["BuyCnt"].ToString());
                    }

                    ModelNos = string.Join(",", iModelNo.ToArray());
                    Qtys = string.Join(",", iQty.ToArray());
                }
            }


            //***** 取得ERP資料 - 報價, 並回寫至 TWBBC_Mall_ImportData_DT ***** 
            List<RefColumn> dataList = new List<RefColumn>();
            var data = new RefColumn();

            API_ERPData ERPData = new API_ERPData();
            //-- 取得價格 --
            using (DataTable DT = ERPData.GetPrice(CustID, DBS, ModelNos, Qtys, out ErrMsg))
            {
                if (DT == null)
                {
                    ErrMsg = "無法取得ERP報價資料.(Step2:Check3)";
                    return false;
                }
                if (DT.Rows.Count == 0)
                {
                    ErrMsg = "查無ERP報價資料.(Step2:Check3)";
                    return false;
                }

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    //加入項目
                    data = new RefColumn
                    {
                        ERP_ModelNo = DT.Rows[row]["ModelNo"].ToString(),   //key
                        BuyCnt = DT.Rows[row]["BuyQty"] == DBNull.Value ? 1 : Convert.ToInt16(DT.Rows[row]["BuyQty"]),   //key
                        ERP_Price = DT.Rows[row]["SpQtyPrice"] == null ? 0
                         : DT.Rows[row]["IsTax"].Equals("Y")
                            ? Math.Round(Convert.ToDouble(DT.Rows[row]["SpQtyPrice"]) / 1.05, 0)  //[價格重計]含稅除1.05
                            : Convert.ToDouble(DT.Rows[row]["SpQtyPrice"]),
                        Currency = DT.Rows[row]["Currency"].ToString()
                    };


                    //將項目加入至集合
                    dataList.Add(data);
                }
            }


            //----- 更新 TWBBC_Mall_ImportData_DT, 價格 -----
            if (!Update_Price(dataID, dataList.AsQueryable()))
            {
                ErrMsg = "未正確取得ERP價格.(Step2:Check3.Update_Price)";
                return false;
            }


            //OK
            return true;
        }


        /// <summary>
        /// [BBC] Step2, 判斷活動, 加入贈品
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Check_Promo(ImportData baseData, out string ErrMsg)
        {
            //宣告
            DataTable DT = new DataTable("PromoTable");

            //取得參數值
            string mallID = baseData.MallID.ToString();
            string id = baseData.Data_ID.ToString();

            //取得資料,並合併Table,用來判斷是否符合多個活動
            DT = GetPromoDT(DT, mallID, id, "1", out ErrMsg); //類型1
            DT = GetPromoDT(DT, mallID, id, "2", out ErrMsg); //類型2

            //判斷
            if (DT == null)
            {
                //無活動資料
                return true;
            }

            //處理活動資料,新增贈品
            #region -- 資料處理 --

            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                sql.AppendLine(" DECLARE @NewID AS INT ");

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    sql.AppendLine(" SET @NewID = (");
                    sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                    sql.AppendLine("  FROM TWBBC_Mall_ImportData_DT");
                    sql.AppendLine("  WHERE (Parent_ID = @DataID)");
                    sql.AppendLine(" );");
                    sql.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_DT( ");
                    sql.AppendLine("  Parent_ID, Data_ID, OrderID, ProdID");
                    sql.AppendLine("  , BuyCnt, BuyPrice, TotalPrice, IsPass, ERP_ModelNo, Currency");
                    sql.AppendLine("  , ShipWho, ShipAddr, ShipTel, IsGift");
                    sql.AppendLine("  , PromoID, PromoName");
                    sql.AppendLine(" ) VALUES (");

                    //col:Parent_ID, Data_ID, OrderID, ProdID
                    sql.AppendLine("  @DataID, @NewID, '{0}', '{1}'".FormatThis(
                        DT.Rows[row]["OrderID"], DT.Rows[row]["ProdID"]
                        ));
                    //col:BuyCnt, BuyPrice, TotalPrice, IsPass, ERP_ModelNo, Currency
                    sql.AppendLine(", {0}, {1}, {2}, 'Y', '{3}', '{4}'".FormatThis(
                        DT.Rows[row]["BuyCnt"]
                        , DT.Rows[row]["BuyPrice"]
                        , DT.Rows[row]["TotalPrice"]
                        , DT.Rows[row]["ERP_ModelNo"]
                        , DT.Rows[row]["Currency"]
                        ));
                    //col:ShipWho, ShipAddr, ShipTel, IsGift
                    sql.AppendLine(", N'{0}', N'{1}', N'{2}', '{3}'".FormatThis(
                        DT.Rows[row]["ShipWho"]
                        , DT.Rows[row]["ShipAddr"]
                        , DT.Rows[row]["ShipTel"]
                        , DT.Rows[row]["IsGift"]
                        ));
                    //col:PromoID, PromoName
                    sql.AppendLine(", '{0}', N'{1}'".FormatThis(
                        DT.Rows[row]["PromoID"], DT.Rows[row]["PromoName"]
                        ));
                    //col:Buy

                    sql.AppendLine(" );");
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", id);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

            #endregion

        }


        /// <summary>
        /// 處理DataTable資料內容,合併Table
        /// </summary>
        /// <param name="srcDT"></param>
        /// <param name="mallID">商城</param>
        /// <param name="id">匯入檔資料編號</param>
        /// <param name="type">活動類型</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        private DataTable GetPromoDT(DataTable srcDT, string mallID, string id, string type, out string ErrMsg)
        {
            DataTable getDT = GetPromoItems(mallID, id, type, out ErrMsg);
            if (getDT == null || getDT.Rows.Count == 0)
            {
                return srcDT;
            }
            srcDT.Merge(getDT);

            return srcDT;
        }

        #endregion



        #region -----// Update //-----

        #region >> BBC匯入 <<
        /// <summary>
        /// [BBC] 更新ERP價格欄位
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// ERP_Price, ERP_NewPrice
        /// </remarks>
        private bool Update_Price(string dataID, IQueryable<RefColumn> query)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                int row = 0;
                foreach (var item in query)
                {
                    sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT");
                    sql.AppendLine(" SET ERP_Price = @ERP_Price_{0}, Currency = @Currency_{0}, ERP_NewPrice = @ERP_Price_{0}".FormatThis(row));
                    sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (IsGift = 'N')");
                    sql.AppendLine("  AND (ERP_ModelNo = @ModelNo_{0}) AND (BuyCnt = @BuyCnt_{0});".FormatThis(row));


                    cmd.Parameters.AddWithValue("ERP_Price_" + row, item.ERP_Price);
                    cmd.Parameters.AddWithValue("Currency_" + row, item.Currency);
                    cmd.Parameters.AddWithValue("ModelNo_" + row, item.ERP_ModelNo);
                    cmd.Parameters.AddWithValue("BuyCnt_" + row, item.BuyCnt);

                    row++;
                }

                //將價格為 0 的資料Update 狀態
                sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT SET IsPass = 'N', doWhat = N'查無ERP價格'");
                sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (IsPass = 'Y') AND (ERP_Price = 0) AND (IsGift = 'N');");


                //----- SQL 執行 -----
                if (string.IsNullOrEmpty(sql.ToString()))
                {
                    return false;
                }
                else
                {
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("DataID", dataID);

                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
        }


        /// <summary>
        /// [BBC] Step3按下一步時執行
        /// 計算差額, 更新要匯入的ERP價格, 並判斷是否新增W001
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// ERP_NewPrice, (W001)
        /// </remarks>
        public bool Update_NewPrice(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            #region >> 計算差額 <<

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- 清除參數 -----
                sql.Clear();
                cmd.Parameters.Clear();

                //----- SQL 查詢語法 -----
                //** 更新價格 **
                sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT");
                sql.AppendLine(" SET Cnt_Price = (");
                sql.AppendLine("     SELECT CONVERT(FLOAT,(");
                sql.AppendLine("          CONVERT(numeric(21, 2), (SELECT TOP 1 TotalPrice FROM TWBBC_Mall_ImportData_DT Ref WHERE (Ref.Parent_ID = @DataID) AND (Ref.OrderID = Data.OrderID)))");
                sql.AppendLine("          - CONVERT(numeric(21, 2), SUM(Data.ERP_Price * Data.BuyCnt))");
                sql.AppendLine("     )) AS Cnt_Price");
                sql.AppendLine("     FROM TWBBC_Mall_ImportData_DT Data");
                sql.AppendLine("     WHERE (Data.Parent_ID = @DataID) AND (Data.OrderID = TWBBC_Mall_ImportData_DT.OrderID) AND (Data.IsGift = 'N')");
                sql.AppendLine("     GROUP BY Data.OrderID");
                sql.AppendLine(" )");
                sql.AppendLine(" WHERE (Parent_ID = @DataID);");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    return false;
                }
            }

            #endregion


            /*
             * [判斷方法] **** NO ****
             * W = 差額
             * If (W < 0)
             *  新增W001
             * Else (W > 0)
             *  Update 每筆訂單的ERP價格
             *  NewPrice = (Cnt_Price/BuyCnt) + ERP_Price
             *  
             * [規則]
             * 京東POP、天貓才有W001, 京東VC沒有
             * 京東POP(W) = (應付金額) - ERP總金額
             * 天貓(W) = 買家實際支付金額 - ERP總金額
             * W > 0 : 分攤至該筆訂單的第一筆價格 
             * (X=差額/購買數量,  Y= X+ERP單價,  Y=>Update [ERP_NewPrice])
             * W < 0 : 折讓, 新增W001
             * 京東VC = 每個品項直接取得ERP價格 ***** MallID = 3 不計算 *****
             * 
             * 只取得可匯入的資料IsPass='Y'
             */
            #region >> 判斷差額並更新價格 <<

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- 清除參數 -----
                sql.Clear();
                cmd.Parameters.Clear();

                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Tbl.Data_ID, Tbl.OrderID, Tbl.Cnt_Price");
                sql.AppendLine(" FROM (");
                sql.AppendLine("     SELECT");
                //-- Group 原始單號
                sql.AppendLine("      ROW_NUMBER() OVER (PARTITION BY OrderID ORDER BY OrderID, Data_ID ASC) AS RowRank");
                sql.AppendLine("      , Data_ID, OrderID, Cnt_Price");
                sql.AppendLine("     FROM TWBBC_Mall_ImportData_DT");
                sql.AppendLine("     WHERE (Parent_ID = @DataID) AND (IsPass = 'Y')");
                sql.AppendLine(" ) AS Tbl");
                sql.AppendLine(" WHERE (Tbl.RowRank = 1)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "查無資料, 請確認";
                        return false;
                    }


                    #region *** 資料處理 ***

                    //宣告SQL
                    StringBuilder dataSQL = new StringBuilder();
                    dataSQL.AppendLine(" DECLARE @NewID AS INT");


                    using (SqlCommand cmdDT = new SqlCommand())
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //差額
                            double CntPrice = Convert.ToDouble(DT.Rows[row]["Cnt_Price"]);
                            //資料編號
                            int RowID = Convert.ToInt32(DT.Rows[row]["Data_ID"]);
                            //訂單編號
                            string OrderID = DT.Rows[row]["OrderID"].ToString();

                            //判斷差額
                            if (CntPrice < 0)
                            {
                                //--- 新增W001 ---
                                dataSQL.AppendLine(" SET @NewID = (");
                                dataSQL.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                                dataSQL.AppendLine("  FROM TWBBC_Mall_ImportData_DT ");
                                dataSQL.AppendLine("  WHERE (Parent_ID = @DataID) ");
                                dataSQL.AppendLine(" );");

                                dataSQL.AppendLine(" INSERT INTO TWBBC_Mall_ImportData_DT( ");
                                dataSQL.AppendLine("  Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice");
                                dataSQL.AppendLine("  , IsPass, ERP_ModelNo, ERP_Price, ERP_NewPrice, Currency, IsGift, Create_Time");
                                dataSQL.AppendLine(" ) VALUES (");
                                //Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice
                                dataSQL.AppendLine("  @DataID, @NewID, N'{0}', 'W001', 1, {1}, {1}".FormatThis(
                                    OrderID, CntPrice
                                    ));
                                //IsPass, ERP_ModelNo, ERP_Price, ERP_NewPrice, Currency, IsGift, Create_Time
                                dataSQL.AppendLine("  , 'Y', 'W001', {0}, {0}, 'NTD', 'N', GETDATE()".FormatThis(
                                    CntPrice
                                    ));
                                dataSQL.AppendLine(");");
                            }
                            else if (CntPrice > 0)
                            {
                                //--- 更新價格 ---
                                dataSQL.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT");
                                dataSQL.AppendLine(" SET ERP_NewPrice = (CAST({0} AS FLOAT)/BuyCnt) + ERP_Price".FormatThis(CntPrice));
                                dataSQL.AppendLine(" WHERE (Parent_ID = @DataID) AND (Data_ID = {0});".FormatThis(RowID));

                            }

                        }


                        //--- 更新主檔狀態 ---
                        dataSQL.AppendLine(" UPDATE TWBBC_Mall_ImportData SET Status = 12 WHERE (Data_ID = @DataID);");

                        //----- SQL 執行 -----
                        cmdDT.CommandText = dataSQL.ToString();
                        cmdDT.Parameters.AddWithValue("DataID", dataID);

                        if (false == dbConn.ExecuteSql(cmdDT, out ErrMsg))
                        {
                            ErrMsg = "差額計算, 更新價格失敗";
                            return false;
                        }
                    }


                    #endregion


                }
            }

            #endregion

            return true;

        }


        /// <summary>
        /// [BBC] 更新其他不足欄位 - Step3最後執行
        /// 1) 填滿因合併欄位造成空白的資料
        /// 2) 發票資料關聯檔
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_Info(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //** 更新其他不足欄位(訂購人/地址/電話) **
                sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_DT");
                sql.AppendLine(" SET ");
                sql.AppendLine(" ShipWho = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipWho FROM TWBBC_Mall_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipWho <> '')");
                sql.AppendLine("  AND (Rel.OrderID = TWBBC_Mall_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , ShipAddr = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipAddr FROM TWBBC_Mall_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipAddr <> '')");
                sql.AppendLine("  AND (Rel.OrderID = TWBBC_Mall_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , ShipTel = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipTel FROM TWBBC_Mall_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipTel <> '')");
                sql.AppendLine("  AND (Rel.OrderID = TWBBC_Mall_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , ShipmentNo = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipmentNo FROM TWBBC_Mall_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipmentNo <> '')");
                sql.AppendLine("  AND (Rel.OrderID = TWBBC_Mall_ImportData_DT.OrderID)");
                sql.AppendLine(" )");

                sql.AppendLine(" WHERE (Parent_ID = @DataID);");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("Creater", fn_Params.UserGuid);

                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    return false;
                }
            }


            return true;

        }


        /// <summary>
        /// [BBC] 狀態更新
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="_st">12=資料整理/13=匯入完成</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_Status(string dataID, int _st, out string ErrMsg)
        {
            StringBuilder sql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE TWBBC_Mall_ImportData SET Status = @status, Import_Time = GETDATE() WHERE (Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("status", _st);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// [促銷活動] - 更新單頭資料
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_PromoData(PromoBase inst, out string ErrMsg)
        {
            StringBuilder sql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE TWBBC_Mall_Promo SET PromoName = @PromoName, MallID = @MallID");
                sql.AppendLine(" , StartTime = @StartTime, EndTime = @EndTime, PromoType = @PromoType, TargetMoney = @TargetMoney, TargetItem = @TargetItem");
                sql.AppendLine(" , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @Data_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", inst.Data_ID);
                cmd.Parameters.AddWithValue("PromoName", inst.PromoName);
                cmd.Parameters.AddWithValue("MallID", inst.MallID);
                cmd.Parameters.AddWithValue("StartTime", inst.StartTime.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("EndTime", inst.EndTime.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("PromoType", inst.PromoType);
                cmd.Parameters.AddWithValue("TargetMoney", inst.TargetMoney == 0 ? DBNull.Value : (object)inst.TargetMoney);
                cmd.Parameters.AddWithValue("TargetItem", inst.TargetItem);
                cmd.Parameters.AddWithValue("Update_Who", inst.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// [BBC] - 更新暫存資料(Step2-1)
        /// </summary>
        /// <param name="dataItems"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_TempDT(List<RefColumn> dataItems, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                int row = 0;

                //----- Data Loop -----
                foreach (var item in dataItems)
                {
                    //cnt
                    row++;


                    //SQL
                    sql.AppendLine(" UPDATE TWBBC_Mall_ImportData_TempDT");
                    sql.AppendLine(" SET ProdID = '{0}', BuyCnt = {1}".FormatThis(item.ProdID, item.BuyCnt));
                    sql.AppendLine(" WHERE (Parent_ID = @Parent_ID) AND (Data_ID = @DataID_{0});".FormatThis(row));

                    cmd.Parameters.AddWithValue("DataID_" + row, item.Data_ID);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", dataItems.FirstOrDefault().Parent_ID);

                //return
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion


        #endregion



        #region -- 取得原始資料(BBC匯入) --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.Data_ID, Base.TraceID, Base.MallID, Base.CustID, Base.Status, Base.Upload_File, Base.Upload_ShipFile, Base.Sheet_Name, Base.Sheet_ShipName");
                sql.AppendLine("   , Base.Import_Time, Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
                sql.AppendLine("   , Cls.Class_Name AS MallName, ClsSt.Class_Name AS StatusName");
                sql.AppendLine("   , (SELECT TOP 1 RTRIM(MA002) FROM PKSYS.dbo.Customer WITH(NOLOCK) WHERE (MA001 = Base.CustID) AND (DBS = DBC)) AS CustName");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                sql.AppendLine("   , (SELECT COUNT(*) FROM TWBBC_Mall_ImportData_Log WHERE (Data_ID = Base.Data_ID)) AS LogCnt");
                sql.AppendLine(" FROM TWBBC_Mall_ImportData Base");
                sql.AppendLine("  INNER JOIN TWBBC_Mall_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  LEFT JOIN TWBBC_Mall_RefClass ClsSt ON Base.Status = ClsSt.Class_ID");
                sql.AppendLine(" WHERE (1 = 1) ");

                /* Search */
                #region >> filter <<
                if (search != null)
                {
                    string filterDateType = "";
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Base.TraceID)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Base.CustID)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (");
                                    sql.Append("      Base.CustID IN (SELECT TOP 1 RTRIM(MA001) FROM PKSYS.dbo.Customer WHERE (MA001 = Base.CustID) AND (DBS = DBC) AND (UPPER(RTRIM(MA002)) LIKE '%' + UPPER(@Keyword) + '%'))");
                                    sql.Append("    )");
                                    //原始單號
                                    sql.Append("    OR (");
                                    sql.Append("      Base.Data_ID IN (SELECT Parent_ID FROM TWBBC_Mall_ImportData_DT WHERE ((UPPER(RTRIM(OrderID)) LIKE '%' + UPPER(@Keyword) + '%') ))");
                                    sql.Append("    )");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)mySearch.TraceID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.TraceID = @TraceID)");

                                    cmd.Parameters.AddWithValue("TraceID", item.Value);
                                }

                                break;

                            case (int)mySearch.Status:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Status = @Status)");

                                    cmd.Parameters.AddWithValue("Status", item.Value);
                                }

                                break;

                            case (int)mySearch.DateType:
                                switch (item.Value)
                                {
                                    case "1":
                                        filterDateType = "Base.Create_Time";
                                        break;

                                    case "2":
                                        filterDateType = "Base.Import_Time";
                                        break;

                                    default:
                                        filterDateType = "Base.Create_Time";
                                        break;
                                }

                                break;


                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));

                                    cmd.Parameters.AddWithValue("sDate", item.Value.ToDateString("yyyy/MM/dd 00:00:00"));
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));

                                    cmd.Parameters.AddWithValue("eDate", item.Value.ToDateString("yyyy/MM/dd 23:59:59"));
                                }

                                break;


                            case (int)mySearch.Mall:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.MallID = @MallID)");

                                    cmd.Parameters.AddWithValue("MallID", item.Value);
                                }

                                break;

                        }
                    }
                }
                #endregion


                sql.AppendLine(") AS Tbl ");
                sql.AppendLine(" ORDER BY Tbl.Status, Tbl.Create_Time DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 取得類別資料
        /// </summary>
        /// <param name="cls">類別參數</param>
        /// <returns></returns>
        private DataTable LookupRawData_Status(myClass cls)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Class_ID AS ID, Class_Name AS Label");
                sql.AppendLine(" FROM TWBBC_Mall_RefClass WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Class_Type = @Class) AND (Display = 'Y')");
                sql.AppendLine(" ORDER BY Sort");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Class", cls);


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }

        #endregion


    }

}