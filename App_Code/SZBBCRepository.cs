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
using SZ_BBC.Models;

/*
 * [SZ BBC匯入]
 * 主要資料庫:ProUnion
 */
namespace SZ_BBC.Controllers
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


    public class SZBBCRepository
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
        public IQueryable<ImportData> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// [BBC] 取得匯入資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ImportData> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ImportData> dataList = new List<ImportData>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
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
                        Data_Type = item.Field<Int16>("Data_Type"),
                        Status = item.Field<Int16>("Status"),
                        Upload_File = item.Field<string>("Upload_File"),
                        Sheet_Name = item.Field<string>("Sheet_Name"),
                        MallName = item.Field<string>("MallName"),
                        CustName = item.Field<string>("CustName"),
                        StatusName = item.Field<string>("StatusName"),
                        Data_TypeName = item.Field<string>("Data_TypeName"),
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
        /// [BBC] 取得類別
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
                        Label = item.Field<string>("Label"),
                        IsStock = item.Field<string>("StockReport")
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
                sql.AppendLine(" SELECT *");
                sql.AppendLine(" FROM BBC_ImportData_DT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY Data_ID");


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
                            Freight = item.Field<double>("Freight"),
                            IsPass = item.Field<string>("IsPass"),
                            doWhat = item.Field<string>("doWhat"),
                            ERP_ModelNo = item.Field<string>("ERP_ModelNo"),
                            ERP_Price = item.Field<double>("ERP_Price"),
                            Currency = item.Field<string>("Currency"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            StockNum = item.Field<int>("StockNum"),
                            IsGift = item.Field<string>("IsGift")

                            //Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm")
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
                sql.AppendLine(" SELECT OrderID, TotalPrice, ShipmentNo, ShipWho, ShipAddr, ShipTel");
                sql.AppendLine(" , BuyRemark, SellRemark, ISNULL(Inv_Remark, '') Inv_Remark, ISNULL(Inv_Type, '2') Inv_Type");
                sql.AppendLine(" , Inv_Title, Inv_Number, Inv_Message");
                sql.AppendLine(" FROM BBC_ImportData_TempDT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (ShipWho <> '') AND (Parent_ID = @ParentID)");
                sql.AppendLine(" GROUP BY OrderID, TotalPrice, ShipmentNo, ShipWho, ShipAddr, ShipTel, BuyRemark, SellRemark");
                sql.AppendLine(" , ISNULL(Inv_Remark, ''), Inv_Type, Inv_Title, Inv_Number, Inv_Message");
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
                            ShipmentNo = item.Field<string>("ShipmentNo"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            BuyRemark = item.Field<string>("BuyRemark"),
                            SellRemark = item.Field<string>("SellRemark"),
                            InvRemark = item.Field<string>("Inv_Remark"),
                            InvType = item.Field<string>("Inv_Type"),
                            InvTitle = item.Field<string>("Inv_Title"),
                            InvNumber = item.Field<string>("Inv_Number"),
                            InvMessage = item.Field<string>("Inv_Message")
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
                sql.AppendLine(" FROM BBC_ImportData_TempDT WITH(NOLOCK)");
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
        /// <returns></returns>
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
                sql.AppendLine(" FROM BBC_ImportData_Log WITH(NOLOCK)");
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
        /// [BBC] 反查ERP 訂單/銷貨單資料 - 單身
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ERPData> GetERPData(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ERPData> dataList = new List<ERPData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                //--訂單資訊
                sql.AppendLine(" COPTC.TC001, COPTC.TC002, COPTD.TD004, COPTD.TD005, COPTD.TD007, ROUND(COPTD.TD008, 0) TD008");
                //--銷貨單資訊
                sql.AppendLine(" , COPTH.TH001, COPTH.TH002, COPTH.TH004, COPTH.TH005, COPTH.TH007, ROUND(COPTH.TH008, 0) TH008");
                //--原始資料
                sql.AppendLine(" , DT.OrderID, DT.ERP_ModelNo ModelNo");
                sql.AppendLine(" FROM BBC_ImportData Base");
                sql.AppendLine("  INNER JOIN BBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");

                //COPTC:訂單單頭 ,條件:OrderID = TC012, CustID = TC004");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTC WITH(NOLOCK) ON DT.OrderID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC012");
                sql.AppendLine("     AND Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC004");

                //COPTD:訂單單身, 條件:ERP_ModelNo = TD004, COPTC.TC001 = COPTD.TD001, COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("     AND DT.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = COPTD.TD004 AND DT.ProdID COLLATE Chinese_Taiwan_Stroke_BIN = COPTD.TD014");

                //COPTH:銷貨單單身, 條件:COPTH.TH014 = COPTC.TC001, COPTH.TH015 = COPTC.TC002, (訂單序號)COPTH.TH016 = COPTD.TD003");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTH.TH014 = COPTC.TC001 AND COPTH.TH015 = COPTC.TC002 AND COPTH.TH016 = COPTD.TD003");

                //Filter
                sql.AppendLine(" WHERE (DT.IsPass = 'Y') AND (COPTC.TC200 = 'Y')");


                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
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

                            case (int)mySearch.TraceID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.TraceID = @TraceID)");

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
                        var data = new ERPData
                        {
                            OrderID = item.Field<string>("OrderID"),
                            ModelNo = item.Field<string>("ModelNo"),

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
        /// [BBC] 反查ERP 銷退單
        /// </summary>
        /// <param name="traceID">平台追蹤編號</param>
        /// <returns></returns>
        public IQueryable<RebackData> GetRebackData(string traceID)
        {
            //----- 宣告 -----
            List<RebackData> dataList = new List<RebackData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ");
                sql.AppendLine("    RTRIM(TI001) TI001, RTRIM(TI002) TI002, RTRIM(TJ004) TJ004, TJ005, TJ007, TI020, TJ023, TJ052");
                sql.AppendLine(" FROM ProUnion.dbo.COPTI WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN ProUnion.dbo.COPTJ WITH(NOLOCK) ON COPTI.TI001 = COPTJ.TJ001 AND COPTI.TI002 = COPTJ.TJ002");
                sql.AppendLine(" WHERE (RTRIM(COPTI.TI001) + RTRIM(COPTI.TI002) IN (");
                sql.AppendLine("     SELECT RTRIM(XB029) + RTRIM(XB030)");
                sql.AppendLine("     FROM DSCSYS.dbo.EDIXB WITH(NOLOCK)");
                sql.AppendLine("     WHERE (XB002 = 'B') AND (XB025 = @TraceID)");
                sql.AppendLine(" ))");


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
                        var data = new RebackData
                        {
                            TI001 = item.Field<string>("TI001"),
                            TI002 = item.Field<string>("TI002"),
                            TJ004 = item.Field<string>("TJ004"),    //品號
                            TJ005 = item.Field<string>("TJ005"),    //品名
                            TJ007 = Convert.ToInt16(item.Field<Decimal?>("TJ007")),  //數量

                            TI020 = item.Field<string>("TI020"),    //單頭備註
                            TJ023 = item.Field<string>("TJ023"),    //單身備註
                            TJ052 = item.Field<string>("TJ052")    //銷退原因代號
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
        /// [BBC] 反查ERP 借出單
        /// </summary>
        /// <param name="traceID">平台追蹤編號</param>
        /// <returns></returns>
        public IQueryable<InvData> GetInvData(string traceID)
        {
            //----- 宣告 -----
            List<InvData> dataList = new List<InvData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(INVTG.TG001) TG001, RTRIM(INVTG.TG002) TG002, COPTC.TC012");
                sql.AppendLine(" , INVTG.TG004, INVTG.TG005, INVTG.TG007, INVTG.TG008, INVTG.TG009");
                sql.AppendLine(" , INVTG.TG014, INVTG.TG015");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.INVTG WITH(NOLOCK) ON INVTG.TG014 = COPTD.TD001 AND INVTG.TG015 = COPTD.TD002 AND INVTG.TG016 = COPTD.TD003");
                sql.AppendLine(" WHERE (COPTC.TC202 = @TraceID)");

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
                        var data = new InvData
                        {
                            TG001 = item.Field<string>("TG001"),
                            TG002 = item.Field<string>("TG002"),
                            TC012 = item.Field<string>("TC012"),
                            TG004 = item.Field<string>("TG004"),    //品號
                            TG005 = item.Field<string>("TG005"),    //品名
                            TG007 = item.Field<string>("TG007"),    //轉出庫別
                            TG008 = item.Field<string>("TG008"),    //轉入庫別
                            TG009 = Convert.ToInt16(item.Field<Decimal?>("TG009")),  //數量

                            TG014 = item.Field<string>("TG014"),    //來源單別
                            TG015 = item.Field<string>("TG015")    //來源單號
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
        /// [BBC] 取得對應客戶代號(VC匯入特例)
        /// </summary>
        /// <param name="search">查詢參數(未使用)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 拆解EXCEL客戶使用
        /// </remarks>
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
                sql.AppendLine(" FROM BBC_RefTable");
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
        /// [BBC] EDI轉入排程LOG
        /// </summary>
        /// <param name="traceID">平台追蹤編號</param>
        /// <returns></returns>
        public IQueryable<EDILog> GetEDILog(string traceID)
        {
            //----- 宣告 -----
            List<EDILog> dataList = new List<EDILog>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(XA006) OrderID, RTRIM(XA011) ModelNo, XA019 Why");
                sql.AppendLine(" FROM DSCSYS.dbo.EDIXA WITH(NOLOCK)");
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
        /// [BBC] 取得Excel內容
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <returns></returns>
        /// <example>
        /// <table id="listTable" class="stripe" cellspacing="0" width="100%" style="width:100%;">
        ///     <asp:Literal ID="lt_tbBody" runat="server"></asp:Literal>
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
                    html.Append("<th>{0}</th>".FormatThis(col.ToString()));
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
        /// [BBC] 取得Excel必要欄位 - Step2讀取
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="mallID">商城</param>
        /// <param name="dataType">1:未出貨訂單, 2:退貨單, 3:已出貨訂單</param>
        /// <param name="traceID">trace id</param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetExcel_DT(string filePath, string sheetName, string mallID, string dataType
            , string traceID, string reStock)
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
                double myFreight = 0;
                string myShipmentNo = "";
                string myShipWho = "";
                string myShipAddr = "";
                string myShipTel = "";
                string myRemarkTitle = "";
                string myRemarkID = "";
                string myNickName = "";
                string myBuyRemark = ""; //買家備註
                string mySellRemark = ""; //賣家備註

                /*
                //發票資料-直接從EXCEL取得並產生 (20190424-取消Excel填入,改由Web輸入)
                //發票類型, 發票抬頭, 稅號, 地址/電話, 開戶行/帳號, 寄票信息, 發票備註
                string myInvFullData = ""; //完整開票資料(需要解析)
                string myInvType = "";
                string myInvAddrInfo = "";
                string myInvBankInfo = "";
                string myInvRemark = "";
                 */
                string myInvTitle = "";
                string myInvNumber = "";
                string myInvMessage = "";

                /* 出貨單列印補充欄位190402 */
                string myBuy_ProdName = ""; //平台產品名稱
                string myBuy_Place = ""; //分配機構
                string myBuy_Warehouse = ""; //倉庫
                string myBuy_Sales = ""; //採購員
                string myBuy_Time = ""; //訂購時間


                //[處理合併儲存格] - 暫存欄:OrderID
                string tmp_OrderID = "";
                string tmp_Remark = "";
                string tmp_Freight = "";
                string tmp_Price = "";

                //資料迴圈
                foreach (var val in queryVals)
                {
                    #region >> 欄位處理:單號 <<

                    //[處理合併儲存格] - 目前的單號(Key)
                    string curr_OrderID;
                    string curr_Remark;
                    string curr_Freight;
                    string curr_Price;

                    /*
                     * [判斷匯入模式]
                     * 1:未出貨訂單 / 2:已出貨訂單 / 3:退貨單
                     */
                    switch (dataType)
                    {
                        case "1":
                        case "3":
                            //** 訂單匯入 **
                            curr_OrderID = val[0].ToString();
                            break;

                        default:
                            //** 退貨單匯入 **
                            curr_OrderID = val[5].ToString();
                            break;
                    }

                    //[處理合併儲存格] - 目前欄位非空值, 填入暫存值
                    //唯品會的是「-」
                    if (mallID.Equals("5"))
                    {
                        if (!curr_OrderID.Equals("-"))
                        {
                            tmp_OrderID = curr_OrderID;
                        }

                        //[設定參數] - OrderID
                        myOrderID = curr_OrderID.Equals("-") ? tmp_OrderID : curr_OrderID;
                    }
                    else
                    {
                        //其他為合併格
                        if (!string.IsNullOrEmpty(curr_OrderID))
                        {
                            tmp_OrderID = curr_OrderID;
                        }

                        //[設定參數] - OrderID
                        myOrderID = string.IsNullOrEmpty(curr_OrderID) ? tmp_OrderID : curr_OrderID;
                    }

                    //Check null
                    if (string.IsNullOrEmpty(myOrderID))
                    {
                        break;
                    }

                    #endregion


                    #region >> 欄位處理:其他欄位 <<

                    //判斷匯入模式
                    switch (dataType)
                    {
                        case "1":
                        case "3":
                            //** 訂單匯入 **

                            #region --匯入欄位--

                            /*
                             * [判斷商城]
                             * 1:京東POP / 2:天貓 / 3:京東VC / 4:eService / 5:唯品會 / 6:京東廠送 / 7:eService促銷 / 999:通用版
                             */
                            switch (mallID)
                            {
                                case "1":
                                    //京東POP
                                    myProdID = val[1];
                                    myProdName = val[2];
                                    myBuyCnt = string.IsNullOrEmpty(val[4]) ? 0 : Convert.ToInt16(val[4]);
                                    myBuyPrice = string.IsNullOrEmpty(val[8]) ? 0 : Convert.ToDouble(val[8]);
                                    myTotalPrice = string.IsNullOrEmpty(val[11]) ? 0 : Convert.ToDouble(val[11]);
                                    myFreight = string.IsNullOrEmpty(val[12]) ? 0 : Convert.ToDouble(val[12]);
                                    myShipmentNo = val[25];
                                    myShipWho = val[14];
                                    myShipAddr = val[15];
                                    myShipTel = string.IsNullOrEmpty(val[16]) ? val[17] : val[16];
                                    myNickName = val[14];
                                    myBuyRemark = val[18]; //買家備註
                                    mySellRemark = val[19]; //賣家備註

                                    curr_Remark = myOrderID + "-" + myShipWho + "-" + myShipmentNo;
                                    //[處理合併儲存格] - 目前欄位非空值, 填入暫存值
                                    if (!string.IsNullOrEmpty(myShipWho))
                                    {
                                        //訂單號+客戶名+物流單號
                                        tmp_Remark = curr_Remark;
                                    }
                                    myRemarkTitle = string.IsNullOrEmpty(myShipWho) ? tmp_Remark : curr_Remark;

                                    myInvMessage = val[20];  //發票類型(僅供參考)
                                    myInvTitle = val[21]; //發票抬頭
                                    myInvNumber = val[23];

                                    break;

                                case "2":
                                    //天貓
                                    myProdID = val[20];
                                    myProdSpec = val[21];
                                    myProdName = val[19];
                                    myBuyCnt = string.IsNullOrEmpty(val[22]) ? 0 : Convert.ToInt16(val[22]);
                                    myBuyPrice = string.IsNullOrEmpty(val[4]) ? 0 : Convert.ToDouble(val[4]);
                                    myTotalPrice = string.IsNullOrEmpty(val[4]) ? 0 : Convert.ToDouble(val[4]);
                                    //myFreight = Convert.ToDouble(val[10]);
                                    myShipmentNo = val[13];
                                    myShipWho = val[5];
                                    myShipAddr = val[10];
                                    myShipTel = "{0}{1}".FormatThis(val[11], string.IsNullOrEmpty(val[12]) ? "" : ";" + val[12]);
                                    myNickName = val[3];
                                    myBuyRemark = val[17]; //買家備註
                                    mySellRemark = val[16]; //賣家備註

                                    curr_Remark = myOrderID + "-" + myShipWho + "-" + myShipmentNo;
                                    //[處理合併儲存格] - 目前欄位非空值, 填入暫存值
                                    if (!string.IsNullOrEmpty(myShipWho))
                                    {
                                        //訂單號+客戶名+物流單號
                                        tmp_Remark = curr_Remark;
                                    }
                                    myRemarkTitle = string.IsNullOrEmpty(myShipWho) ? tmp_Remark : curr_Remark;
                                    //curr_Remark = myNickName;
                                    //[處理合併儲存格] - 目前欄位非空值, 填入暫存值
                                    //if (!string.IsNullOrEmpty(myNickName))
                                    //{
                                    //    //客戶名
                                    //    tmp_Remark = curr_Remark;
                                    //}
                                    //myRemarkTitle = string.IsNullOrEmpty(myNickName) ? tmp_Remark : curr_Remark;


                                    break;

                                case "3":
                                    //京東VC
                                    myProdID = val[3];
                                    myBuyCnt = string.IsNullOrEmpty(val[12]) ? 0 : Convert.ToInt16(val[12]);

                                    myShipWho = val[9];
                                    myShipAddr = val[8];
                                    myShipTel = val[10];
                                    myRemarkTitle = myOrderID + "-" + myShipWho; //訂單號+客戶名

                                    /* 出貨單列印補充欄位190402 */
                                    myBuyPrice = string.IsNullOrEmpty(val[11]) ? 0 : Convert.ToDouble(val[11]);
                                    myTotalPrice = myBuyCnt * myBuyPrice;
                                    myBuy_ProdName = val[4]; //平台產品名稱
                                    myBuy_Place = val[6]; //分配機構
                                    myBuy_Warehouse = val[7]; //倉庫
                                    myBuy_Sales = val[20]; //採購員
                                    myBuy_Time = val[21]; //訂購時間

                                    break;


                                case "4":
                                case "7":
                                    //** eService 格式
                                    myOrderID = traceID;    //將Order 設定為 TraceID
                                    myProdID = val[0];
                                    myBuyCnt = string.IsNullOrEmpty(val[1]) ? 0 : Convert.ToInt16(val[1]);
                                    myRemarkTitle = traceID; //訂單號

                                    break;


                                case "5":
                                    //唯品會
                                    myProdID = val[34];
                                    myBuyCnt = string.IsNullOrEmpty(val[28]) ? 0 : Convert.ToInt16(val[28]);
                                    myBuyPrice = string.IsNullOrEmpty(val[31]) ? 0 : Convert.ToDouble(val[31]);

                                    //運費:含「-」處理
                                    string getFreight = val[19];
                                    curr_Freight = getFreight;
                                    if (!getFreight.Equals("-"))
                                    {
                                        tmp_Freight = curr_Freight;
                                    }
                                    //myFreight = string.IsNullOrEmpty(val[19]) ? 0 : Convert.ToDouble(val[19]);
                                    myFreight = getFreight.Equals("-") ? Convert.ToDouble(tmp_Freight) : Convert.ToDouble(curr_Freight);

                                    //總金額:含「-」處理
                                    string getPrice = val[20];
                                    curr_Price = getPrice;
                                    if (!getPrice.Equals("-"))
                                    {
                                        tmp_Price = curr_Price;
                                    }
                                    //myTotalPrice = string.IsNullOrEmpty(val[20]) ? 0 : Convert.ToDouble(val[20]);
                                    myTotalPrice = getPrice.Equals("-") ? Convert.ToDouble(tmp_Price) : Convert.ToDouble(curr_Price);

                                    myShipmentNo = val[38];
                                    myShipWho = val[7];
                                    myShipAddr = val[8];
                                    myShipTel = val[9];
                                    myNickName = val[7];


                                    //備註(處理「-」)
                                    curr_Remark = myOrderID + "-" + myShipWho + "-" + myShipmentNo;
                                    if (!getPrice.Equals("-"))
                                    {
                                        tmp_Remark = curr_Remark;
                                    }
                                    myRemarkTitle = getPrice.Equals("-") ? tmp_Remark : curr_Remark;

                                    break;


                                case "6":
                                    //京東廠送
                                    myProdID = val[1];
                                    myBuyCnt = string.IsNullOrEmpty(val[3]) ? 0 : Convert.ToInt16(val[3]);
                                    myBuyPrice = string.IsNullOrEmpty(val[14]) ? 0 : Convert.ToDouble(val[14]);
                                    myShipmentNo = val[20];
                                    myShipWho = val[5];
                                    myShipAddr = val[7];
                                    myShipTel = val[6];
                                    myNickName = val[5];

                                    myRemarkTitle = myOrderID + "-" + myShipWho + "-" + myShipmentNo;

                                    break;

                                case "999":
                                    //通用版
                                    myProdID = val[6];
                                    myBuyCnt = string.IsNullOrEmpty(val[7]) ? 0 : Convert.ToInt16(val[7]);
                                    myBuyPrice = string.IsNullOrEmpty(val[5]) ? 0 : Convert.ToDouble(val[5]);
                                    //myFreight = Convert.ToDouble(val[4]);
                                    myShipmentNo = val[8];
                                    myShipWho = val[2];
                                    myShipAddr = val[3];
                                    myShipTel = val[4];
                                    myNickName = val[2];

                                    myRemarkTitle = val[9];

                                    break;

                                default:
                                    //其他

                                    break;
                            }
                            #endregion

                            break;


                        default:
                            //** 退貨單匯入 **

                            //判斷商城, 取得各對應欄位, 設定各內容參數
                            switch (mallID)
                            {
                                case "3":
                                    //京東VC
                                    myProdID = val[2];
                                    myBuyCnt = 1;
                                    myBuyPrice = Convert.ToDouble(val[7]);
                                    myTotalPrice = Convert.ToDouble(val[7]);
                                    myRemarkTitle = val[5]; //出庫單號
                                    myRemarkID = val[10].ToString().Left(2);

                                    break;

                                default:
                                    //其他

                                    break;
                            }

                            break;
                    }

                    #endregion


                    //加入項目
                    var data = new RefColumn
                    {
                        OrderID = myOrderID,
                        ProdID = myProdID,
                        ProdSpec = myProdSpec,
                        ProdName = myProdName,
                        BuyCnt = myBuyCnt,
                        BuyPrice = myBuyPrice,
                        TotalPrice = myTotalPrice,
                        Freight = myFreight,
                        ShipmentNo = myShipmentNo.Replace("'", ""),
                        ShipWho = myShipWho.Replace("'", ""),
                        ShipAddr = myShipAddr.Replace("'", ""),
                        ShipTel = myShipTel.Replace("'", ""),
                        RemarkTitle = myRemarkTitle,
                        RemarkID = myRemarkID,
                        StockReback = reStock,
                        NickName = myNickName,
                        //(20190424-remark)
                        //InvType = myInvType,
                        //InvAddrInfo = myInvAddrInfo,
                        //InvBankInfo = myInvBankInfo,
                        InvMessage = myInvMessage,
                        InvTitle = myInvTitle,
                        InvNumber = myInvNumber,
                        Buy_ProdName = HttpUtility.HtmlEncode(myBuy_ProdName),
                        Buy_Place = myBuy_Place,
                        Buy_Warehouse = myBuy_Warehouse,
                        Buy_Sales = myBuy_Sales,
                        Buy_Time = myBuy_Time,
                        BuyRemark = myBuyRemark,
                        SellRemark = mySellRemark
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }


                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception ex)
            {

                throw new Exception("請檢查Excel格式是否正確!!各商城對應的格式不同." + ex.Message.ToString());
            }
        }


        /// <summary>
        /// [BBC] 匯入開票資料拆欄位-取發票資料「:」後的文字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string splitItem(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            string[] aryCol = Regex.Split(value, ":");

            return aryCol[1].ToString().Trim();
        }

        /// <summary>
        /// [BBC] 取得庫別代號
        /// </summary>
        /// <param name="swid">出貨庫別代號</param>
        /// <returns></returns>
        private String GetStockType(string swid)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT StockType");
                sql.AppendLine(" FROM ShippingWarehouse WITH(NOLOCK)");
                sql.AppendLine(" WHERE (SWID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", swid);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return DT.Rows[0]["StockType"].ToString();
                    }

                }
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
                            sql.AppendLine(" FROM BBC_Promo Base");
                            sql.AppendLine(" WHERE (Base.StartTime <= GETDATE()) AND (Base.EndTime >= GETDATE())");
                            sql.AppendLine("  AND (Base.MallID = @MallID)");
                            sql.AppendLine("  AND (Base.PromoType = 1)");
                            sql.AppendLine(" )");
                            sql.AppendLine(" , TblGroup AS (");
                            //--取得平台單號:判斷匯入資料(總金額>=活動價格)
                            sql.AppendLine(" SELECT OrderID, TotalPrice, TblPromo.PromoID, TblPromo.PromoName");
                            sql.AppendLine(" , ROW_NUMBER() OVER (PARTITION BY OrderID ORDER BY OrderID, TblPromo.PromoTarget DESC) AS RowRank");
                            sql.AppendLine(" FROM BBC_ImportData_DT, TblPromo");
                            sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (ProdID <> 'W001') AND (TotalPrice >= TblPromo.PromoTarget)");
                            sql.AppendLine(" GROUP BY OrderID, TotalPrice, TblPromo.PromoID, TblPromo.PromoName, TblPromo.PromoTarget");
                            sql.AppendLine(" )");
                            //--取得贈品:組合成要新增的資料
                            sql.AppendLine(" SELECT");
                            sql.AppendLine(" DT.OrderID, GiftDT.ModelNo AS ProdID, GiftDT.Qty AS BuyCnt");
                            sql.AppendLine(" , 0 AS BuyPrice, DT.TotalPrice AS TotalPrice, 0 AS Freight");
                            sql.AppendLine(" , GiftDT.ModelNo AS ERP_ModelNo");
                            sql.AppendLine(" , 'RMB' AS Currency, DT.ShipWho, DT.ShipAddr, DT.ShipTel, GiftDT.Qty AS StockNum, 'Y' AS IsGift, 1 AS StockStatus");
                            sql.AppendLine(" , TblGroup.PromoID, TblGroup.PromoName");
                            sql.AppendLine(" FROM BBC_ImportData_DT DT");
                            sql.AppendLine("  INNER JOIN TblGroup ON DT.OrderID = TblGroup.OrderID");
                            sql.AppendLine("  INNER JOIN BBC_Promo_DT GiftDT ON TblGroup.PromoID = GiftDT.Parent_ID");
                            sql.AppendLine(" WHERE (DT.Parent_ID = @ParentID) AND (DT.ProdID <> 'W001') AND (DT.ShipWho <> '') AND (TblGroup.RowRank = 1)");
                            sql.AppendLine(" GROUP BY DT.Parent_ID, DT.OrderID, DT.TotalPrice, DT.ShipAddr, DT.ShipWho, DT.ShipTel, GiftDT.ModelNo, GiftDT.Qty, TblGroup.PromoID, TblGroup.PromoName");

                            break;

                        default:
                            sql.AppendLine(" ;WITH TblPromo AS (");
                            //--取得活動內容:指定商品
                            sql.AppendLine(" SELECT Base.Data_ID AS PromoID, Base.PromoName, Base.TargetItem AS PromoTarget");
                            sql.AppendLine(" FROM BBC_Promo Base");
                            sql.AppendLine(" WHERE (Base.StartTime <= GETDATE()) AND (Base.EndTime >= GETDATE())");
                            sql.AppendLine("  AND (Base.MallID = @MallID)");
                            sql.AppendLine("  AND (Base.PromoType = 2)");
                            sql.AppendLine(" )");
                            sql.AppendLine(" , TblGroup AS (");
                            //--取得平台單號:判斷匯入資料(購買商品對應)
                            sql.AppendLine(" SELECT OrderID, ERP_ModelNo, TblPromo.PromoID, TblPromo.PromoName");
                            sql.AppendLine(" FROM BBC_ImportData_DT");
                            sql.AppendLine("  INNER JOIN TblPromo ON ERP_ModelNo = TblPromo.PromoTarget");
                            sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (ProdID <> 'W001')");
                            sql.AppendLine(" GROUP BY OrderID, ERP_ModelNo, TblPromo.PromoID, TblPromo.PromoName");
                            sql.AppendLine(" )");
                            //--取得贈品:組合成要新增的資料
                            sql.AppendLine(" SELECT ");
                            sql.AppendLine(" DT.OrderID, GiftDT.ModelNo AS ProdID, GiftDT.Qty AS BuyCnt");
                            sql.AppendLine(" , 0 AS BuyPrice, DT.TotalPrice AS TotalPrice, 0 AS Freight");
                            sql.AppendLine(" , GiftDT.ModelNo AS ERP_ModelNo");
                            sql.AppendLine(" , 'RMB' AS Currency, DT.ShipWho, DT.ShipAddr, DT.ShipTel, GiftDT.Qty AS StockNum, 'Y' AS IsGift, 1 AS StockStatus");
                            sql.AppendLine(" , TblGroup.PromoID, TblGroup.PromoName");
                            sql.AppendLine(" FROM BBC_ImportData_DT DT");
                            sql.AppendLine("  INNER JOIN TblGroup ON DT.OrderID = TblGroup.OrderID");
                            sql.AppendLine("  INNER JOIN BBC_Promo_DT GiftDT ON TblGroup.PromoID = GiftDT.Parent_ID");
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
                sql.AppendLine(" , (SELECT COUNT(*) FROM BBC_Promo_DT WHERE (Parent_ID = Base.Data_ID)) AS ChildCnt");
                sql.AppendLine(" FROM BBC_Promo Base");
                sql.AppendLine("  INNER JOIN BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
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
                sql.AppendLine(" FROM BBC_Promo_DT");
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

        #endregion


        #region >> BBC單據匯出 <<

        /// <summary>
        /// [BBC單據匯出] 經銷商線上下單-單據資料
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        /// <remarks>
        /// DB:ProUnion
        /// </remarks>
        public IQueryable<ERPData> GetERPDataByDealer(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ERPData> dataList = new List<ERPData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("  COPTC.TC001, COPTC.TC002, COPTC.TC003, COPTD.TD004, COPTD.TD005, ROUND(COPTD.TD008, 0) TD008");
                sql.AppendLine("  , COPTH.TH001, COPTH.TH002, COPTH.TH007, ROUND(COPTH.TH008, 0) TH008");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTH.TH014 = COPTC.TC001 AND COPTH.TH015 = COPTC.TC002 AND COPTH.TH016 = COPTD.TD003");
                sql.AppendLine("  WHERE (RTRIM(COPTC.TC001) + RTRIM(COPTC.TC002) IN (");
                sql.AppendLine("      SELECT RTRIM(XB029) + RTRIM(XB030)");
                sql.AppendLine("      FROM DSCSYS.dbo.EDIXB WITH(NOLOCK)");
                sql.AppendLine("      WHERE (XB002 <> 'B') AND (XB024 = '官網線上下單')");
                sql.AppendLine("  )) ");


                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (COPTC.TC003 >= @startDate)");

                                    cmd.Parameters.AddWithValue("startDate", item.Value);
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (COPTC.TC003 <= @endDate)");

                                    cmd.Parameters.AddWithValue("endDate", item.Value);
                                }

                                break;
                        }
                    }
                }

                sql.AppendLine(" ORDER BY COPTD.TD001, COPTD.TD002, COPTD.TD003");

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
                        var data = new ERPData
                        {
                            TC001 = item.Field<string>("TC001"),
                            TC002 = item.Field<string>("TC002"),
                            TC003 = item.Field<string>("TC003"),
                            TD004 = item.Field<string>("TD004"),    //品號
                            TD005 = item.Field<string>("TD005"),    //品名
                            TD008 = Convert.ToInt16(item.Field<Decimal?>("TD008")),  //數量

                            TH001 = item.Field<string>("TH001"),
                            TH002 = item.Field<string>("TH002"),
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
        /// [BBC單據匯出] VC銷退單-單據資料
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        /// <remarks>
        /// DB:ProUnion
        /// </remarks>
        public IQueryable<RebackData> GetERPRebackData(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<RebackData> dataList = new List<RebackData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("    RTRIM(TI001) TI001, RTRIM(TI002) TI002, TI003, RTRIM(TJ004) TJ004, TJ005, TJ007");
                sql.AppendLine("    , TI020, TJ023, TJ052");
                sql.AppendLine("    , TJ025, TJ026");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTI WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTJ WITH(NOLOCK) ON COPTI.TI001 = COPTJ.TJ001 AND COPTI.TI002 = COPTJ.TJ002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON COPTJ.TJ025 = ACRTB.TB001 AND COPTJ.TJ026 = ACRTB.TB002");
                sql.AppendLine(" WHERE (RTRIM(COPTI.TI001) + RTRIM(COPTI.TI002) IN (");
                sql.AppendLine("     SELECT RTRIM(XB029) + RTRIM(XB030)");
                sql.AppendLine("     FROM DSCSYS.dbo.EDIXB WITH(NOLOCK)");
                sql.AppendLine("     WHERE (XB002 = 'B') AND (XB024 = '京東VC')");
                sql.AppendLine(" ))");

                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (COPTI.TI003 >= @startDate)");

                                    cmd.Parameters.AddWithValue("startDate", item.Value);
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (COPTI.TI003 <= @endDate)");

                                    cmd.Parameters.AddWithValue("endDate", item.Value);
                                }

                                break;
                        }
                    }
                }

                sql.AppendLine(" ORDER BY COPTI.TI001, COPTI.TI002");

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
                        var data = new RebackData
                        {
                            TI001 = item.Field<string>("TI001"),
                            TI002 = item.Field<string>("TI002"),
                            TI003 = item.Field<string>("TI003"),    //銷退日
                            TJ004 = item.Field<string>("TJ004"),    //品號
                            TJ005 = item.Field<string>("TJ005"),    //品名
                            TJ007 = Convert.ToInt16(item.Field<Decimal?>("TJ007")),  //數量

                            TI020 = item.Field<string>("TI020"),    //單頭備註
                            TJ023 = item.Field<string>("TJ023"),    //單身備註
                            TJ052 = item.Field<string>("TJ052"),    //銷退原因代號
                            TJ025 = item.Field<string>("TJ025"),    //結帳單別
                            TJ026 = item.Field<string>("TJ026")     //結帳單號
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
                sql.AppendLine("  RTRIM(COPMA.MA001) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , RTRIM(COPTH.TH001) + '-' + RTRIM(COPTH.TH002) AS Erp_SO_ID");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine("  , RTRIM(COPTD.TD004) COLLATE Chinese_Taiwan_Stroke_CI_AS AS ModelNo");
                sql.AppendLine("  , RTRIM(COPTD.TD014) COLLATE Chinese_Taiwan_Stroke_CI_AS AS CustModelNo");
                sql.AppendLine("  , (CASE WHEN CHARINDEX('-', REVERSE(COPTG.TG020)) > 0 THEN ");
                sql.AppendLine("     REVERSE(SUBSTRING(REVERSE(COPTG.TG020), 1, CHARINDEX('-', REVERSE(COPTG.TG020))-1 ))");
                sql.AppendLine("    ELSE '' END");
                sql.AppendLine("  ) AS ShipNo");
                sql.AppendLine("  , CONVERT(INT, CASE WHEN COPTH.TH008 = 0 THEN COPTH.TH024 ELSE COPTH.TH008 END) AS BuyCnt");
                sql.AppendLine("  , CONVERT(FLOAT, COPTG.TG045 + COPTG.TG046) AS TotalPrice");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                //條件:已確認 / 排除W001
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTD.TD004 NOT IN ('W001'))");
                sql.AppendLine(" )");
                sql.AppendLine(" , TblInvItem AS (");
                sql.AppendLine(" SELECT InvItem.TraceID, InvItem.OrderID");
                sql.AppendLine("  , InvItem.Inv_Type AS InvType");
                sql.AppendLine("  , InvItem.Inv_Title AS InvTitle");
                sql.AppendLine("  , InvItem.Inv_Number AS InvNumber");
                sql.AppendLine("  , InvItem.Inv_AddrInfo AS InvAddrInfo");
                sql.AppendLine("  , InvItem.Inv_BankInfo AS InvBankInfo");
                sql.AppendLine("  , ROW_NUMBER() OVER (PARTITION BY InvItem.OrderID ORDER BY InvItem.OrderID ASC) AS RowRank");
                sql.AppendLine(" FROM [PKEF].[dbo].[BBC_InvoiceItem] InvItem");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT Base.Data_ID SrcParentID, DT.Data_ID SrcDataID");
                sql.AppendLine("  , TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , Cls.Class_Name AS MallName, DT.OrderID");
                sql.AppendLine("  , ROW_NUMBER() OVER (PARTITION BY DT.OrderID ORDER BY DT.OrderID, TblERP.ShipNo ASC) AS RowRank");
                sql.AppendLine("  , DT.ERP_ModelNo");
                sql.AppendLine("  , TblERP.BuyCnt");
                sql.AppendLine("  , TblERP.Erp_SO_ID");
                sql.AppendLine("  , ISNULL(TblERP.ShipNo, DT.ShipmentNo) AS ShipNo");
                sql.AppendLine("  , DT.ShipWho, DT.ShipAddr, DT.ShipTel");
                sql.AppendLine("  , TblERP.TotalPrice AS InvPrice");
                sql.AppendLine("  , InvItem.InvType");
                sql.AppendLine("  , InvItem.InvTitle");
                sql.AppendLine("  , InvItem.InvNumber");
                sql.AppendLine("  , InvItem.InvAddrInfo");
                sql.AppendLine("  , InvItem.InvBankInfo");
                sql.AppendLine(" FROM [PKEF].dbo.BBC_ImportData AS Base");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_ImportData_DT AS DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND DT.ERP_ModelNo = TblERP.ModelNo AND DT.ProdID = TblERP.CustModelNo");
                sql.AppendLine("  LEFT JOIN TblInvItem InvItem ON DT.OrderID = InvItem.OrderID AND Base.TraceID = InvItem.TraceID AND InvItem.RowRank = 1");
                sql.AppendLine(" WHERE (1=1)");

                //條件:已匯過的不顯示
                sql.AppendLine("  AND (DT.OrderID NOT IN(");
                sql.AppendLine(" 	SELECT RelDT.OrderID");
                sql.AppendLine(" 	FROM [PKEF].dbo.BBC_ShipExport Rel");
                sql.AppendLine(" 	 INNER JOIN [PKEF].dbo.BBC_ShipExport_DT RelDT ON Rel.Data_ID = RelDT.Parent_ID");
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
                                    sql.Append(" AND (TblERP.ShipNo <> '')");
                                }

                                break;

                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY DT.OrderID");

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
                            InvType = GetInvTypeName(item.Field<string>("InvType")),
                            InvPrice = item.Field<double>("InvPrice"),
                            InvTitle = item.Field<string>("InvTitle"),
                            InvNumber = item.Field<string>("InvNumber"),
                            InvAddrInfo = item.Field<string>("InvAddrInfo"),
                            InvBankInfo = item.Field<string>("InvBankInfo")

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
                sql.AppendLine(" FROM [PKEF].dbo.BBC_ShipExport Base");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN = COPMA.MA001");
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
                sql.AppendLine("  RTRIM(COPMA.MA001) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , RTRIM(COPTH.TH001) + '-' + RTRIM(COPTH.TH002) AS Erp_SO_ID");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine("  , RTRIM(COPTD.TD004) COLLATE Chinese_Taiwan_Stroke_CI_AS AS ModelNo");
                sql.AppendLine("  , RTRIM(COPTD.TD014) COLLATE Chinese_Taiwan_Stroke_CI_AS AS CustModelNo");
                sql.AppendLine("  , (CASE WHEN CHARINDEX('-', REVERSE(COPTG.TG020)) > 0 THEN ");
                sql.AppendLine("     REVERSE(SUBSTRING(REVERSE(COPTG.TG020), 1, CHARINDEX('-', REVERSE(COPTG.TG020))-1 ))");
                sql.AppendLine("    ELSE '' END");
                sql.AppendLine("  ) AS ShipNo");
                sql.AppendLine("  , CONVERT(INT, CASE WHEN COPTH.TH008 = 0 THEN COPTH.TH024 ELSE COPTH.TH008 END) AS BuyCnt");
                sql.AppendLine("  , CONVERT(FLOAT, COPTG.TG045 + COPTG.TG046) AS TotalPrice");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                //條件:已確認 / 排除W001
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTD.TD004 NOT IN ('W001'))");
                //條件:關聯,節省效能
                sql.AppendLine(" AND (EXISTS (");
                sql.AppendLine(" SELECT * FROM [PKEF].[dbo].[BBC_ShipExport_DT]");
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine("  AND (OrderID = COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS)");
                sql.AppendLine("  AND (ModelNo = RTRIM(COPTD.TD004) COLLATE Chinese_Taiwan_Stroke_CI_AS)");
                sql.AppendLine(" ))");

                sql.AppendLine(" )");
                sql.AppendLine(" , TblInvItem AS (");
                sql.AppendLine(" SELECT InvItem.TraceID, InvItem.OrderID");
                sql.AppendLine("  , InvItem.Inv_Type AS InvType");
                sql.AppendLine("  , InvItem.Inv_Title AS InvTitle");
                sql.AppendLine("  , InvItem.Inv_Number AS InvNumber");
                sql.AppendLine("  , InvItem.Inv_AddrInfo AS InvAddrInfo");
                sql.AppendLine("  , InvItem.Inv_BankInfo AS InvBankInfo");
                sql.AppendLine("  , ROW_NUMBER() OVER (PARTITION BY InvItem.OrderID ORDER BY InvItem.OrderID ASC) AS RowRank");
                sql.AppendLine(" FROM [PKEF].[dbo].[BBC_InvoiceItem] InvItem");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT DT.Data_ID");
                sql.AppendLine("  , TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , Cls.Class_Name AS MallName, DT.OrderID");
                sql.AppendLine("  , ROW_NUMBER() OVER (PARTITION BY DT.OrderID ORDER BY DT.OrderID, TblERP.ShipNo ASC) AS RowRank");
                sql.AppendLine("  , DT.ERP_ModelNo");
                sql.AppendLine("  , TblERP.BuyCnt");
                sql.AppendLine("  , TblERP.Erp_SO_ID");
                sql.AppendLine("  , ISNULL(TblERP.ShipNo, DT.ShipmentNo) AS ShipNo");
                sql.AppendLine("  , DT.ShipWho, DT.ShipAddr, DT.ShipTel");
                sql.AppendLine("  , TblERP.TotalPrice AS InvPrice");
                sql.AppendLine("  , InvItem.InvType");
                sql.AppendLine("  , InvItem.InvTitle");
                sql.AppendLine("  , InvItem.InvNumber");
                sql.AppendLine("  , InvItem.InvAddrInfo");
                sql.AppendLine("  , InvItem.InvBankInfo");
                sql.AppendLine(" FROM [PKEF].dbo.BBC_ShipExport AS Rel");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_ShipExport_DT AS RelDT ON Rel.Data_ID = RelDT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_ImportData AS Base ON Base.Data_ID = RelDT.SrcParentID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_ImportData_DT AS DT ON Base.Data_ID = DT.Parent_ID AND DT.Data_ID = RelDT.SrcDataID");
                sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND DT.ERP_ModelNo = TblERP.ModelNo AND DT.ProdID = TblERP.CustModelNo");
                sql.AppendLine("  LEFT JOIN TblInvItem InvItem ON DT.OrderID = InvItem.OrderID AND Base.TraceID = InvItem.TraceID AND InvItem.RowRank = 1");
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

                sql.AppendLine(" ORDER BY DT.OrderID");

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
                            ShipTel = item.Field<string>("ShipTel"),
                            InvType = GetInvTypeName(item.Field<string>("InvType")),
                            InvPrice = item.Field<double>("InvPrice"),
                            InvTitle = item.Field<string>("InvTitle"),
                            InvNumber = item.Field<string>("InvNumber"),
                            InvAddrInfo = item.Field<string>("InvAddrInfo"),
                            InvBankInfo = item.Field<string>("InvBankInfo")

                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        private string GetInvTypeName(string value)
        {
            switch (value)
            {
                case "1":
                    return "普票";

                case "2":
                    return "专票";

                default:
                    return "";
            }
        }

        #endregion


        #region >> 電商庫存 <<
        /// <summary>
        /// [電商庫存] 庫存列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<StockData> GetStockData(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<StockData> dataList = new List<StockData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @stockDate AS VARCHAR(10)");
                sql.AppendLine(" SET @stockDate = @setDate");

                sql.AppendLine(" ;WITH TblModel AS (");
                sql.AppendLine("  SELECT DT.ERP_ModelNo");
                sql.AppendLine("  FROM [PKEF].dbo.BBC_StockData Base");
                sql.AppendLine("   INNER JOIN [PKEF].dbo.BBC_StockData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  WHERE (Base.StockDate = @stockDate) AND (DT.IsPass = 'Y')");
                sql.AppendLine("  GROUP BY DT.ERP_ModelNo");
                sql.AppendLine(" )");
                sql.AppendLine(" , TblEC1 AS (");
                /* 京東POP */
                sql.AppendLine("  SELECT DT.ERP_ModelNo, DT.ProdID, ISNULL(DT.StockNum, 0) AS StockNum ");
                sql.AppendLine("  FROM [PKEF].dbo.BBC_StockData Base");
                sql.AppendLine("   INNER JOIN [PKEF].dbo.BBC_StockData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  WHERE (Base.MallID = 1) AND (Base.StockDate = @stockDate) AND (DT.IsPass = 'Y')");
                sql.AppendLine(" )");
                sql.AppendLine(" , TblEC2 AS (");
                /* 天貓 */
                sql.AppendLine("  SELECT DT.ERP_ModelNo, DT.ProdID, ISNULL(DT.StockNum, 0) AS StockNum ");
                sql.AppendLine("  FROM [PKEF].dbo.BBC_StockData Base");
                sql.AppendLine("   INNER JOIN [PKEF].dbo.BBC_StockData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  WHERE (Base.MallID = 2) AND (Base.StockDate = @stockDate) AND (DT.IsPass = 'Y')");
                sql.AppendLine(" )");
                sql.AppendLine(" , TblEC3 AS (");
                /* 唯品會 */
                sql.AppendLine("  SELECT DT.ERP_ModelNo, DT.ProdID, ISNULL(DT.StockNum, 0) AS StockNum");
                sql.AppendLine("  FROM [PKEF].dbo.BBC_StockData Base");
                sql.AppendLine("   INNER JOIN [PKEF].dbo.BBC_StockData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  WHERE (Base.MallID = 5) AND (Base.StockDate = @stockDate) AND (DT.IsPass = 'Y')");
                sql.AppendLine(" )");
                sql.AppendLine(" , TblEC4 AS (");
                /* 京東廠送 */
                sql.AppendLine("  SELECT DT.ERP_ModelNo, DT.ProdID, ISNULL(DT.StockNum, 0) AS StockNum");
                sql.AppendLine("  FROM [PKEF].dbo.BBC_StockData Base");
                sql.AppendLine("   INNER JOIN [PKEF].dbo.BBC_StockData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  WHERE (Base.MallID = 6) AND (Base.StockDate = @stockDate) AND (DT.IsPass = 'Y')");
                sql.AppendLine(" )");
                sql.AppendLine(" , Tbl_PreSell AS (");
                /* 預計銷(訂單):TD016 = 結案碼, TD021 = 確認碼 */
                sql.AppendLine(" 	SELECT p.ModelNo, p.A01 AS PreSell_A01, p.B01 AS PreSell_B01");
                sql.AppendLine(" 	FROM (");
                sql.AppendLine(" 		SELECT (ISNULL(SUM(TD008 - TD009 + TD024 - TD025), 0)");
                sql.AppendLine(" 		  - ISNULL((");
                sql.AppendLine(" 			SELECT SUM(ISNULL(INVTG.TG009, 0))");
                sql.AppendLine(" 			FROM [ProUnion].dbo.INVTG WITH(NOLOCK)");
                sql.AppendLine(" 			WHERE COPTD.TD004 = TG004 AND TG007 = COPTD.TD007 AND TG001 = '1302' AND TG008 = 'C01' AND TG024 = 'N'");
                sql.AppendLine(" 		  ),0)");
                sql.AppendLine(" 		 ) AS PreSell");
                sql.AppendLine(" 		 , RTRIM(TD004) AS ModelNo");
                sql.AppendLine(" 		 , TD007 AS StockType");
                sql.AppendLine(" 		FROM [ProUnion].dbo.COPTD WITH (NOLOCK)");
                sql.AppendLine(" 		WHERE (TD016 = 'N') AND (TD021 = 'Y') AND (TD007 IN ('A01','B01'))");
                sql.AppendLine(" 		GROUP BY TD004, TD007");
                sql.AppendLine(" 	) t ");
                sql.AppendLine(" 	PIVOT (");
                sql.AppendLine(" 		SUM(PreSell)");
                sql.AppendLine(" 		FOR StockType IN ([A01], [B01])");
                sql.AppendLine(" 	) p");
                sql.AppendLine(" )");
                sql.AppendLine(" , Tbl_PreIN AS (");
                /* 預計進(採購單):TD016 = 結案碼, TD018 = 確認碼 */
                sql.AppendLine(" 	SELECT p.ModelNo, p.A01 AS PreIN_A01, p.B01 AS PreIN_B01");
                sql.AppendLine(" 	FROM (");
                sql.AppendLine(" 		SELECT ISNULL(SUM(TD008 - TD015), 0) AS PreIN, RTRIM(TD004) AS ModelNo, TD007 AS StockType");
                sql.AppendLine(" 		FROM [ProUnion].dbo.PURTD WITH (NOLOCK)");
                sql.AppendLine(" 		WHERE (TD016 = 'N') AND (TD018 = 'Y') AND (TD007 IN ('A01','B01'))");
                sql.AppendLine(" 		GROUP BY TD004, TD007");
                sql.AppendLine(" 	) t ");
                sql.AppendLine(" 	PIVOT (");
                sql.AppendLine(" 		SUM(PreIN)");
                sql.AppendLine(" 		FOR StockType IN ([A01], [B01])");
                sql.AppendLine(" 	) p");
                sql.AppendLine(" )");
                sql.AppendLine(" , Tbl_Stock AS (");
                sql.AppendLine(" 	SELECT p.ModelNo, p.A01 AS StockQty_A01, p.B01 AS StockQty_B01");
                sql.AppendLine(" 	FROM (");
                sql.AppendLine(" 		SELECT MC007 AS StockQty, RTRIM(MC001) AS ModelNo, MC002 AS StockType");
                sql.AppendLine(" 		FROM [ProUnion].dbo.INVMC WITH (NOLOCK)");
                sql.AppendLine(" 		WHERE (MC002 IN ('A01','B01'))");
                sql.AppendLine(" 	) t ");
                sql.AppendLine(" 	PIVOT (");
                sql.AppendLine(" 		SUM(StockQty)");
                sql.AppendLine(" 		FOR StockType IN ([A01], [B01])");
                sql.AppendLine(" 	) p");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT TblModel.ERP_ModelNo");
                sql.AppendLine(" , ISNULL(TblEC1.ProdID, '') AS EC1_SKU, ISNULL(TblEC1.StockNum, 0) AS EC1_StockNum  /* 京東POP(1) */");
                sql.AppendLine(" , ISNULL(TblEC2.ProdID, '') AS EC2_SKU, ISNULL(TblEC2.StockNum, 0) AS EC2_StockNum  /* 天貓(2) */");
                sql.AppendLine(" , ISNULL(TblEC3.ProdID, '') AS EC3_SKU, ISNULL(TblEC3.StockNum, 0) AS EC3_StockNum  /* 唯品會(5) */");
                sql.AppendLine(" , ISNULL(TblEC4.ProdID, '') AS EC4_SKU, ISNULL(TblEC4.StockNum, 0) AS EC4_StockNum  /* 京東廠送(6) */");
                sql.AppendLine(" , ISNULL(stock.StockQty_B01, 0) AS StockQty_B01, ISNULL(sell.PreSell_B01, 0) AS PreSell_B01, ISNULL(buy.PreIN_B01, 0) AS PreIN_B01");

                sql.AppendLine(" FROM TblModel");
                sql.AppendLine("  LEFT JOIN TblEC1 ON TblModel.ERP_ModelNo = TblEC1.ERP_ModelNo");
                sql.AppendLine("  LEFT JOIN TblEC2 ON TblModel.ERP_ModelNo = TblEC2.ERP_ModelNo");
                sql.AppendLine("  LEFT JOIN TblEC3 ON TblModel.ERP_ModelNo = TblEC3.ERP_ModelNo");
                sql.AppendLine("  LEFT JOIN TblEC4 ON TblModel.ERP_ModelNo = TblEC4.ERP_ModelNo");
                sql.AppendLine("  LEFT JOIN Tbl_Stock AS stock ON TblModel.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = stock.ModelNo");
                sql.AppendLine("  LEFT JOIN Tbl_PreSell AS sell ON TblModel.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = sell.ModelNo");
                sql.AppendLine("  LEFT JOIN Tbl_PreIN AS buy ON TblModel.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = buy.ModelNo");
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
                            case "setDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    cmd.Parameters.AddWithValue("setDate", item.Value);
                                }

                                break;

                            case "Keyword":
                                sql.Append(" AND (");
                                sql.Append("  (UPPER(TblModel.ERP_ModelNo) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);
                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY TblModel.ERP_ModelNo");

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
                        var data = new StockData
                        {
                            ModelNo = item.Field<string>("ERP_ModelNo"),
                            EC1_SKU = item.Field<string>("EC1_SKU"),
                            EC1_StockNum = item.Field<int>("EC1_StockNum"),
                            EC2_SKU = item.Field<string>("EC2_SKU"),
                            EC2_StockNum = item.Field<int>("EC2_StockNum"),
                            EC3_SKU = item.Field<string>("EC3_SKU"),
                            EC3_StockNum = item.Field<int>("EC3_StockNum"),
                            EC4_SKU = item.Field<string>("EC4_SKU"),
                            EC4_StockNum = item.Field<int>("EC4_StockNum"),
                            StockQty_B01 = Convert.ToInt32(item.Field<decimal>("StockQty_B01")),
                            PreSell_B01 = Convert.ToInt32(item.Field<decimal>("PreSell_B01")),
                            PreIN_B01 = Convert.ToInt32(item.Field<decimal>("PreIN_B01"))
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
        /// [電商庫存] 匯入資料 - 單頭
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<StockImportData> GetStockImportData(Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<StockImportData> dataList = new List<StockImportData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.Data_ID");
                sql.AppendLine(" , Base.MallID, Base.CustID, Base.StockDate, Base.Upload_File");
                sql.AppendLine(" , Cls.Class_Name AS MallName");
                sql.AppendLine(" , Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                sql.AppendLine(" FROM BBC_StockData Base");
                sql.AppendLine("  INNER JOIN BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
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
                            case "MallID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.AppendLine(" AND (Base.MallID = @MallID)");
                                    cmd.Parameters.AddWithValue("MallID", item.Value);
                                }

                                break;


                            case "StockDate":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.AppendLine(" AND (Base.StockDate = @StockDate)");
                                    cmd.Parameters.AddWithValue("StockDate", item.Value);
                                }

                                break;

                            case "DataID":
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.AppendLine(" AND (Base.Data_ID = @DataID)");
                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;
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
                        var data = new StockImportData
                        {
                            Data_ID = item.Field<Guid>("Data_ID"),
                            MallID = item.Field<int>("MallID"),
                            CustID = item.Field<string>("CustID"),
                            StockDate = item.Field<DateTime>("StockDate").ToString().ToDateString("yyyy/MM/dd"),
                            Upload_File = item.Field<string>("Upload_File"),
                            MallName = item.Field<string>("MallName"),
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
        /// [電商庫存] 匯入資料 - 單身
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <returns></returns>
        public IQueryable<StockImportDataDT> GetStockImportDetail(string parentID)
        {
            //----- 宣告 -----
            List<StockImportDataDT> dataList = new List<StockImportDataDT>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT *");
                sql.AppendLine(" FROM BBC_StockData_DT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY Data_ID");


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
                        var data = new StockImportDataDT
                        {
                            ProdID = item.Field<string>("ProdID"),
                            StockNum = item.Field<int>("StockNum"),
                            ERP_ModelNo = item.Field<string>("ERP_ModelNo"),
                            IsPass = item.Field<string>("IsPass"),
                            doWhat = item.Field<string>("doWhat")
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
        /// [電商庫存] 取得Excel必要欄位
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="mallID">商城ID</param>
        /// <param name="traceID">Trace id</param>
        /// <returns></returns>
        public IQueryable<StockImportDataDT> GetExcel_DT_ECStock(string filePath, string sheetName, string mallID)
        {
            try
            {
                //----- 宣告 -----
                List<StockImportDataDT> dataList = new List<StockImportDataDT>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string myProdID = "";
                int myStockNum = 0;


                //資料迴圈
                foreach (var val in queryVals)
                {
                    #region --匯入欄位--

                    /*
                    * [判斷商城]
                    * 1:京東POP / 2:天貓 / 3:京東VC / 4:eService / 5:唯品會 / 6:VC工業品 / 999:通用版
                    */
                    switch (mallID)
                    {
                        case "1":
                            //京東POP
                            myProdID = val[2];
                            myStockNum = string.IsNullOrEmpty(val[7]) ? 0 : Convert.ToInt16(val[7]);

                            break;

                        case "2":
                            //天貓
                            myProdID = val[2];
                            myStockNum = string.IsNullOrEmpty(val[11]) ? 0 : Convert.ToInt16(val[11]);

                            break;

                        case "5":
                            //唯品會
                            myProdID = val[1];
                            myStockNum = string.IsNullOrEmpty(val[5]) ? 0 : Convert.ToInt16(val[5]);

                            break;


                        case "6":
                            //京東廠送
                            myProdID = val[0];
                            myStockNum = string.IsNullOrEmpty(val[2]) ? 0 : Convert.ToInt16(val[2]);

                            break;

                        default:
                            //其他

                            break;
                    }
                    #endregion


                    //加入項目(排除空值)
                    if (!string.IsNullOrWhiteSpace(myProdID))
                    {
                        var data = new StockImportDataDT
                        {
                            ProdID = myProdID,
                            StockNum = myStockNum
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }

                }


                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception ex)
            {

                throw new Exception("請檢查Excel格式是否正確!" + ex.Message.ToString());
            }
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
                sql.AppendLine("  RTRIM(COPMA.MA001) AS CustID, RTRIM(COPMA.MA002) AS CustName");
                sql.AppendLine("  , COPTC.TC001, COPTC.TC002, COPTH.TH001, COPTH.TH002");
                sql.AppendLine("  , RTRIM(ACRTB.TB001) AS TB001, RTRIM(ACRTB.TB002) AS TB002");
                sql.AppendLine("  , CONVERT(FLOAT, COPTH.TH013) AS Price");
                sql.AppendLine("  , COPTC.TC012 COLLATE Chinese_Taiwan_Stroke_CI_AS AS OrderID");
                sql.AppendLine("  , COPTD.TD004 COLLATE Chinese_Taiwan_Stroke_CI_AS AS ModelNo");
                sql.AppendLine("  , COPTD.TD014 COLLATE Chinese_Taiwan_Stroke_CI_AS AS CustModelNo");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTC WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTD.TD001 = COPTH.TH014 AND COPTD.TD002 = COPTH.TH015 AND COPTD.TD003 = COPTH.TH016");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTC.TC004");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTB.TB005 = COPTH.TH001 AND ACRTB.TB006 = COPTH.TH002");
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y')");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT DT.OrderID, Cls.Class_Name");
                sql.AppendLine("  , TblERP.CustID, TblERP.CustName");
                sql.AppendLine("  , TblERP.TC001, TblERP.TC002, TblERP.TH001, TblERP.TH002, TblERP.TB001, TblERP.TB002");
                sql.AppendLine("  , SUM(TblERP.Price) AS TotalPrice");
                sql.AppendLine("  , DT.ShipmentNo, DT.ShipTel");
                sql.AppendLine(" FROM BBC_ImportData Base");
                sql.AppendLine("  INNER JOIN BBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN TblERP ON DT.OrderID = TblERP.OrderID AND DT.ERP_ModelNo = TblERP.ModelNo AND DT.ProdID = TblERP.CustModelNo");
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


        #region >> 客戶商品對應 <<

        /// <summary>
        /// [BBC] 自訂客戶商品對應 (仿ERP.COPMG)
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
                sql.AppendLine(" SELECT Base.Data_ID, Base.MG002, Base.MG003, ISNULL(Base.MG006, '') AS MG006");
                sql.AppendLine(" FROM refCOPMG Base");
                sql.AppendLine(" WHERE (Base.DB = 'SZ')");

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


        /// <summary>
        /// 取得Excel欄位,用來轉入資料
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <returns></returns>
        public IQueryable<RefModel> GetRef_ExcelData(string filePath, string sheetName)
        {
            try
            {
                //----- 宣告 -----
                List<RefModel> dataList = new List<RefModel>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string _ModelNo = "";
                string _ProdID = "";
                string _ProdSpec = "";

                //資料迴圈
                foreach (var val in queryVals)
                {
                    _ModelNo = val[0];
                    _ProdID = val[1];
                    _ProdSpec = val[2];

                    //加入項目
                    var data = new RefModel
                    {
                        ModelNo = _ModelNo,
                        CustModelNo = _ProdID,
                        CustSpec = _ProdSpec
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }

                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception ex)
            {

                throw new Exception("請檢查Excel格式是否正確!!格式可參考Excel範本." + ex.Message.ToString());
            }
        }


        /// <summary>
        /// [BBC] 客戶商品對應檢查清單(同客戶品號出現2次以上)
        /// </summary>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<RefModel> GetRef_ChkList(out string ErrMsg)
        {
            //----- 宣告 -----
            List<RefModel> dataList = new List<RefModel>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" ;WITH TblGroup AS (");
                sql.AppendLine(" 	SELECT MallID, MG001, MG003, COUNT(*) AS repeatCnt");
                sql.AppendLine(" 	FROM refCOPMG WITH(NOLOCK)");
                sql.AppendLine(" 	WHERE (DB = 'SZ') AND (ISNULL(MG006, '') = '') AND (MG002 <> MG003)");
                sql.AppendLine(" 	GROUP BY MallID, MG001, MG003");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT Base.Data_ID, Base.MG001 AS CustID, Base.MG002 AS ModelNo, Base.MG003 AS CustModelNo");
                sql.AppendLine("  , Cls.Class_Name AS MallName");
                sql.AppendLine(" FROM refCOPMG Base");
                sql.AppendLine("  INNER JOIN TblGroup ON Base.MallID = TblGroup.MallID AND Base.MG001 = TblGroup.MG001 AND Base.MG003 = TblGroup.MG003");
                sql.AppendLine("  INNER JOIN PKEF.dbo.BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine(" WHERE (Base.DB = 'SZ') AND (TblGroup.repeatCnt > 1)");

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
                            CustID = item.Field<string>("CustID"),
                            ModelNo = item.Field<string>("ModelNo"),
                            CustModelNo = item.Field<string>("CustModelNo"),
                            MallName = item.Field<string>("MallName")
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
        public bool Create(ImportData instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO BBC_ImportData( ");
                sql.AppendLine("  Data_ID, TraceID, MallID, CustID, SalesID, Data_Type, Status, Upload_File");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @TraceID, @MallID, @CustID, @SalesID, @Data_Type, 10, @Upload_File");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("MallID", instance.MallID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("SalesID", instance.SalesID);
                cmd.Parameters.AddWithValue("Data_Type", instance.Data_Type);
                cmd.Parameters.AddWithValue("Upload_File", instance.Upload_File);
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
                sql.AppendLine(" DELETE FROM BBC_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM BBC_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE BBC_ImportData SET Status = 11, Sheet_Name = @Sheet_Name, Update_Who = @Update_Who, Update_Time = GETDATE() WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in query)
                {
                    if (!string.IsNullOrEmpty(item.ProdID))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                        sql.AppendLine("  FROM BBC_ImportData_TempDT ");
                        sql.AppendLine("  WHERE Parent_ID = @DataID ");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO BBC_ImportData_TempDT( ");
                        sql.AppendLine("  Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice, Freight, ShipmentNo");
                        sql.AppendLine("  , ShipWho, ShipAddr, ShipTel, ProdSpec, ProdName");
                        //單頭備註/ 單身備註ID / 退貨庫別 / NickName
                        sql.AppendLine("  , RemarkTitle, RemarkID, StockReback, NickName");

                        //發票資料(20190424-remark)(於Step2-1時Update)
                        sql.AppendLine("  , Inv_Type, Inv_Title, Inv_Number, Inv_AddrInfo, Inv_BankInfo, Inv_Message, Inv_Remark");

                        //其他補充欄位
                        sql.AppendLine("  , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                        sql.AppendLine("  , BuyRemark, SellRemark");

                        sql.AppendLine(" ) VALUES (");

                        //col:Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice, Freight, ShipmentNo
                        sql.AppendLine("  @DataID, @NewID, '{0}', '{1}', {2}, {3}, {4}, {5}, '{6}'".FormatThis(
                            item.OrderID, item.ProdID.Trim(), item.BuyCnt
                            , item.BuyPrice == null ? 0 : item.BuyPrice
                            , item.TotalPrice == null ? 0 : item.TotalPrice
                            , item.Freight == null ? 0 : item.Freight
                            , item.ShipmentNo
                            ));
                        //col:ShipWho, ShipAddr, ShipTel, ProdSpec, ProdName
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}', N'{3}', N'{4}'".FormatThis(
                            item.ShipWho, item.ShipAddr, item.ShipTel
                            , HttpUtility.HtmlEncode(item.ProdSpec), HttpUtility.HtmlEncode(item.ProdName)
                            ));
                        //col:RemarkTitle, RemarkID, StockReback, NickName
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}', N'{3}'".FormatThis(
                            item.RemarkTitle, item.RemarkID, item.StockReback, item.NickName
                            ));

                        //col:發票(20190424-remark)
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}', N'{6}'".FormatThis(
                            item.InvType, item.InvTitle, item.InvNumber, item.InvAddrInfo
                            , item.InvBankInfo, item.InvMessage, item.InvRemark
                            ));

                        //col:補充欄位
                        sql.AppendLine("  , N'{0}', N'{1}', N'{2}', N'{3}', N'{4}'".FormatThis(
                            item.Buy_ProdName, item.Buy_Place, item.Buy_Warehouse, item.Buy_Sales, item.Buy_Time
                            ));
                        sql.AppendLine("  , N'{0}', N'{1}'".FormatThis(item.BuyRemark, item.SellRemark));

                        sql.AppendLine(" );");

                    }
                }

                //處理合併欄位, 總金額為 0的欄位
                sql.AppendLine(" UPDATE BBC_ImportData_TempDT SET TotalPrice = ISNULL((");
                sql.AppendLine("  SELECT TOP 1 Totalprice FROM BBC_ImportData_TempDT Ref WHERE (Ref.Parent_ID = @DataID)");
                sql.AppendLine("    AND (Ref.OrderID = BBC_ImportData_TempDT.OrderID) AND (Ref.TotalPrice <> 0)");
                sql.AppendLine(" ), 0)");
                sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (TotalPrice = 0)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Sheet_Name", baseData.Sheet_Name);
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
                sql.AppendLine("  SELECT ISNULL(MAX(Log_ID), 0) + 1 AS NewID FROM BBC_ImportData_Log");
                sql.AppendLine(" )");

                sql.AppendLine(" INSERT INTO BBC_ImportData_Log( ");
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
            string dataType = baseData.Data_Type.ToString();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----

                //EDI欄位設定
                sql.AppendLine(" SELECT");
                sql.AppendLine("  Corp.Corp_ID AS XA001");

                /*
                 * --- 欄位XA002:型態別 ---
                 * [判斷匯入模式]
                 * 1:未出貨訂單 / 2:已出貨訂單 / 3:退貨單
                 * 
                 * [判斷商城] 1:京東POP / 2:天貓 / 3:京東VC / 4:eService / 5:唯品會 / 6:VC工業品 / 999:通用版
                 * 直接設為訂單(2) = NA
                 * 直接設為銷貨單(1) = 1:京東POP / 2:天貓 / 3:京東VC / 5:唯品會 / 6:VC工業品
                 * 其他依庫存判斷
                 * 
                 * [欄位設定]
                 * XA002 = 1:銷貨 2:訂單 B:退貨
                 * 
                 */
                switch (dataType)
                {
                    case "1":
                    case "3":
                        //** 訂單匯入 **
                        switch (baseData.MallID)
                        {
                            case 4:
                                //eService:依庫存判斷
                                sql.AppendLine(", DT.StockStatus AS XA002");
                                break;

                            case 7:
                                //eService:促銷轉訂單
                                sql.AppendLine(", '2' AS XA002");
                                break;

                            default:
                                //直接為銷貨單
                                sql.AppendLine(", '1' AS XA002");
                                break;
                        }

                        break;

                    default:
                        //** 退貨單匯入 **
                        sql.AppendLine(", DT.StockStatus AS XA002");

                        break;
                }


                /*
                 * --- 欄位XA003:單別 ---
                 * 設定單別:依PKSYS設定, Mall=7(寫死)
                 */
                switch (dataType)
                {
                    case "1":
                    case "3":
                        switch (baseData.MallID)
                        {
                            case 7:
                                sql.AppendLine(", '2245' AS XA003"); //促銷單別

                                break;

                            default://** 訂單匯入 **
                                sql.AppendLine(", (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_單別') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS XA003");

                                break;
                        }
                        break;


                    default:
                        //** 退貨單匯入 **
                        sql.AppendLine(", (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_退貨單別') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS XA003");

                        break;
                }


                sql.AppendLine(", LEFT(DT.OrderID, 40) AS XA006");  //平台單號 (excel訂單號)
                sql.AppendLine(", Base.CustID AS XA007");
                sql.AppendLine(", RTRIM(Cust.MA016) AS XA008");   //ERP業務人員ID
                sql.AppendLine(", DT.Currency AS XA009");
                sql.AppendLine(", CONVERT(VARCHAR(8), Base.Create_Time, 112) AS XA010"); //單據日期(抓建立日)
                sql.AppendLine(", LEFT(DT.ERP_ModelNo, 40) AS XA011");
                sql.AppendLine(", (CASE WHEN DT.IsGift = 'Y' THEN 0 ELSE DT.BuyCnt END) AS XA012"); //數量
                sql.AppendLine(", (CASE WHEN DT.IsGift = 'Y' THEN 0 ELSE DT.ERP_NewPrice END) AS XA013"); //單價


                /*
                 * --- 欄位XA014:庫別 ---
                 * 設定出貨庫別
                 */
                //判斷模式/商城
                if (dataType.Equals("2"))
                {
                    //退貨單:退貨庫別
                    sql.AppendLine(", DT.StockReback AS XA014");
                }
                else
                {
                    //訂單/銷貨單
                    /*
                    * [判斷商城]
                    * 1:京東POP / 2:天貓 / 3:京東VC / 4:eService / 5:唯品會 / 6:VC工業品 / 999:通用版
                    */
                    switch (baseData.MallID)
                    {
                        case 4:
                        case 7:
                            //eService(抓客戶出貨庫別)
                            sql.AppendLine(", SW.StockType AS XA014");

                            break;

                        default:
                            //VC, POP, 天貓, 唯品會, VC工業品 (固定A01倉)
                            sql.AppendLine(", 'A01' AS XA014");

                            break;
                    }
                }

                sql.AppendLine(" , LEFT(DT.ShipAddr, 250) AS XA015");
                sql.AppendLine(" , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_預交日_有庫存') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS StockYes");
                sql.AppendLine(" , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_預交日_無庫存') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS StockNo");
                sql.AppendLine(" , CONVERT(VARCHAR(8), Base.Create_Time, 112) AS XA017"); //生效日(抓建立日)
                sql.AppendLine(" , '1' AS XA020"); //贈備品
                sql.AppendLine(" , (CASE WHEN DT.IsGift = 'Y' THEN DT.BuyCnt ELSE 0 END) AS XA021"); //贈備品量
                sql.AppendLine(" , LEFT(DT.ShipWho, 30) AS XA022");
                sql.AppendLine(" , LEFT(DT.ShipTel, 20) AS XA023");
                sql.AppendLine(" , MallCls.Class_Name AS XA024"); //來源平台
                sql.AppendLine(" , LEFT(Base.TraceID, 20) AS XA025");  //原始單號(填入追蹤編號)
                sql.AppendLine(" , LEFT(DT.ProdID, 30) AS XA026"); //SKU碼
                sql.AppendLine(" , '2' AS XA027"); //是否開發票(預設值)
                sql.AppendLine(" , '1' AS XA028"); //折讓或銷退(預設值)
                sql.AppendLine(" , LEFT(DT.RemarkTitle, 250) AS XA031");
                sql.AppendLine(" , LEFT(DT.Remark, 250) XA032");
                sql.AppendLine(" , (CASE WHEN Base.Data_Type = 2 THEN 'A001' ELSE '' END) AS XA033"); //銷退原因
                sql.AppendLine("  , RIGHT(('000' + CAST(DT.Data_ID AS VARCHAR(4))), 4) AS XA034"); //自訂序號
                sql.AppendLine(" , DT.StockNum"); //預交日計算用欄位
                sql.AppendLine(" FROM BBC_ImportData Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN BBC_ImportData_DT DT WITH(NOLOCK) ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN BBC_RefClass MallCls WITH(NOLOCK) ON Base.MallID = MallCls.Class_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustID = Cust.MA001 AND Cust.DBS = Cust.DBC");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Param_Corp Corp WITH(NOLOCK) ON Cust.DBC = Corp.Corp_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer_Data CustDT WITH(NOLOCK) ON Cust.MA001 = CustDT.Cust_ERPID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.ShippingWarehouse SW WITH(NOLOCK) ON CustDT.SWID = SW.SWID");
                sql.AppendLine(" WHERE (Base.Data_ID = @DataID) AND (DT.IsPass = 'Y')");
                sql.AppendLine(" ORDER BY DT.OrderID, DT.Data_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);


                /*
                 * 重新整理欄位, 計算預交日
                 * 庫存 > 0:預交日計算使用<StockYes>
                 * 庫存 < 0:預交日計算使用<StockNo>
                 * 將計算好的預交日, 插入DataSet, 並設定為XA016
                 */
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "沒有可匯入的資料,請確認Step3的「即將匯入」是否有資料.";
                        return false;
                    }

                    //[設定參數], 預交日數
                    int BookDays_StockYes = Convert.ToInt16(DT.Rows[0]["StockYes"]);
                    int BookDays_StockNo = Convert.ToInt16(DT.Rows[0]["StockNo"]);

                    //[新增欄位] - XA016:預交日
                    DT.Columns.Add("XA016", typeof(string));

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //取得庫存量
                        int StockNum = Convert.ToInt32(DT.Rows[row]["StockNum"]);

                        //庫存判斷
                        int StockDay;
                        if (StockNum > 0)
                        {
                            StockDay = BookDays_StockYes;
                        }
                        else
                        {
                            StockDay = BookDays_StockNo;
                        }

                        //設定預交日
                        string myBookDay = CustomExtension.GetWorkDate(DateTime.Now, StockDay).ToShortDateString().ToDateString("yyyyMMdd");
                        DT.Rows[row]["XA016"] = myBookDay;
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

                                /*
                                 筆數過多會失敗, Http 有上限
                                */
                                //回傳Webservice (資料DataSet, Token, 測試模試(Y/N))
                                if (false == EDI.Insert(myDS, "N", out ErrMsg))
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
                sql.AppendLine(" INSERT INTO BBC_Promo( ");
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
                sql.AppendLine(" UPDATE BBC_Promo SET");
                sql.AppendLine(" Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @Parent_ID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (");
                sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID FROM BBC_Promo_DT WHERE (Parent_ID = @Parent_ID)");
                sql.AppendLine(" );");

                sql.Append(" INSERT INTO BBC_Promo_DT( ");
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
                sql.AppendLine("  @MallID, @MG001, @MG002, @MG003, @MG006, 'SZ'");
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
                sql.AppendLine(" INSERT INTO BBC_ImportData_TempDT(");
                sql.AppendLine("  Data_ID, Parent_ID, OrderID, ProdID, ProdSpec, ProdName");
                sql.AppendLine("  , BuyCnt, BuyPrice, TotalPrice, Freight");
                sql.AppendLine("  , ShipmentNo, ShipWho, ShipAddr, ShipTel");
                sql.AppendLine("  , Inv_Title, Inv_Number, Inv_Message, Inv_Remark, Inv_Type");
                sql.AppendLine("  , RemarkTitle, RemarkID, NickName, BuyRemark, SellRemark)");
                sql.AppendLine(" SELECT TOP 1 ");
                sql.AppendLine(" (");
                sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                sql.AppendLine("  FROM BBC_ImportData_TempDT");
                sql.AppendLine("  WHERE Parent_ID = @Parent_ID");
                sql.AppendLine(" ) AS Data_ID");
                sql.AppendLine(" , Parent_ID, OrderID");
                sql.AppendLine(" , Ref.MG003 AS ProdID, Ref.MG006 AS ProdSpec, '***手動新增***' AS ProdName");
                sql.AppendLine(" , 1 AS BuyCnt, 0 AS BuyPrice, TotalPrice, Freight");
                sql.AppendLine(" , ShipmentNo, ShipWho, ShipAddr, ShipTel");
                sql.AppendLine(" , Inv_Title, Inv_Number, Inv_Message, Inv_Remark, Inv_Type");
                sql.AppendLine(" , RemarkTitle, RemarkID, NickName, BuyRemark, SellRemark");
                sql.AppendLine(" FROM BBC_ImportData_TempDT");
                sql.AppendLine("  LEFT JOIN [PKSYS].[dbo].[refCOPMG] Ref ON Ref.Data_ID = @ItemID AND Ref.DB = 'SZ'");
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
                sql.AppendLine(" INSERT INTO BBC_ShipExport( ");
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
                    sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID FROM BBC_ShipExport_DT WHERE (Parent_ID = @Parent_ID)");
                    sql.AppendLine(" );");

                    sql.AppendLine(" INSERT INTO BBC_ShipExport_DT( ");
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


        #region >> 電商庫存 <<
        /// <summary>
        /// [電商庫存] 建立單頭
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_StockData(StockImportData instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO BBC_StockData( ");
                sql.AppendLine("  Data_ID, MallID, CustID, StockDate, Upload_File");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @MallID, @CustID, @StockDate, @Upload_File");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("MallID", instance.MallID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("StockDate", instance.StockDate);
                cmd.Parameters.AddWithValue("Upload_File", instance.Upload_File);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [電商庫存] 建立單身
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool Create_StockDataDT(StockImportData baseData, IQueryable<StockImportDataDT> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM BBC_StockData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE BBC_StockData SET Update_Who = @Update_Who, Update_Time = GETDATE() WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in query)
                {
                    if (!string.IsNullOrWhiteSpace(item.ProdID))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                        sql.AppendLine("  FROM BBC_StockData_DT ");
                        sql.AppendLine("  WHERE Parent_ID = @DataID ");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO BBC_StockData_DT( ");
                        sql.AppendLine("  Parent_ID, Data_ID, ProdID, StockNum");
                        sql.AppendLine(" ) VALUES (");
                        sql.AppendLine("  @DataID, @NewID, '{0}', {1}".FormatThis(
                            item.ProdID.Trim(), item.StockNum));
                        sql.AppendLine(" );");
                    }
                }

                //Update 品號
                sql.AppendLine(" UPDATE BBC_StockData_DT");
                sql.AppendLine(" SET ERP_ModelNo = TblBase.ModelNo, IsPass = 'Y'");
                sql.AppendLine(" FROM");
                sql.AppendLine(" (");
                sql.AppendLine("  SELECT Tmp.ProdID, RTRIM(MG002) AS ModelNo");
                sql.AppendLine("  FROM BBC_StockData Base");
                sql.AppendLine("   INNER JOIN BBC_StockData_DT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                sql.AppendLine("   LEFT JOIN [ProUnion].dbo.COPMG Ref ON Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN) AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");
                sql.AppendLine("  WHERE (Base.Data_ID = @DataID)");
                sql.AppendLine(" ) AS TblBase");
                sql.AppendLine(" WHERE (BBC_StockData_DT.ProdID = TblBase.ProdID)");
                sql.AppendLine(" AND (BBC_StockData_DT.Parent_ID = @DataID);");
                //Update IsPass
                sql.AppendLine(" UPDATE BBC_StockData_DT SET IsPass = 'N', doWhat = '查無ERP品號(客戶品號資料檔)' WHERE (Parent_ID = @DataID) AND (ERP_ModelNo IS NULL);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion


        #region >> 客戶商品對應 <<
        /// <summary>
        /// [客戶商品對應-匯入] 建立客戶商品對應
        /// </summary>
        /// <param name="instance">RefModel</param>
        /// <param name="custID">Cust</param>
        /// <param name="mallID">Mall</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_RefModels(IQueryable<RefModel> instance, string custID, Int32 mallID, string db, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM refCOPMG WHERE (MG001 = @CustID) AND (MallID = @MallID) AND (DB = @db)");
                //sql.AppendLine(" DECLARE @NewID AS INT");

                foreach (var item in instance)
                {
                    //sql.AppendLine(" SET @NewID = (");
                    //sql.AppendLine("  SELECT ISNULL(MAX(Data_ID) ,0) + 1 FROM refCOPMG");
                    //sql.AppendLine(" )");
                    sql.AppendLine(" INSERT INTO refCOPMG(");
                    sql.AppendLine("  MallID, MG001, DB");
                    sql.AppendLine("  , MG002, MG003, MG006");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @MallID, @CustID, @db");
                    sql.AppendLine("  , N'{0}', N'{1}', N'{2}'".FormatThis(item.ModelNo, item.CustModelNo, item.CustSpec));
                    sql.AppendLine(" );");
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("CustID", custID);
                cmd.Parameters.AddWithValue("MallID", mallID);
                cmd.Parameters.AddWithValue("db", db);

                //Execute
                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }

        }


        #endregion


        #endregion



        #region -----// Delete //-----


        #region >> BBC匯入 <<
        /// <summary>
        /// [BBC] 刪除資料
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete(string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM BBC_InvoiceItem");
                sql.AppendLine(" WHERE EXISTS (");
                sql.AppendLine("    SELECT *");
                sql.AppendLine("    FROM BBC_ImportData Base");
                sql.AppendLine("     INNER JOIN BBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("    WHERE (Base.Data_ID = @DataID)");
                sql.AppendLine("     AND (BBC_InvoiceItem.TraceID = Base.TraceID) AND (BBC_InvoiceItem.OrderID = DT.OrderID)");
                sql.AppendLine(" );");

                sql.AppendLine(" DELETE FROM BBC_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM BBC_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM BBC_ImportData WHERE (Data_ID = @DataID);");

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
                sql.AppendLine(" DELETE FROM BBC_ImportData_TempDT WHERE (Parent_ID = @DataID);");

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
                sql.AppendLine(" DELETE FROM BBC_Promo_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM BBC_Promo WHERE (Data_ID = @DataID);");

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
                sql.AppendLine(" DELETE FROM BBC_Promo_DT WHERE (Parent_ID = @Parent_ID) AND (Data_ID = @Data_ID)");

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
        /// *** 重要:此處有寫死DB Name(ProUnion), 若有變動須注意 ***
        /// 暫存檔與COPMG對應比較, 更新 ERP_ModelNo
        /// , 不正常的資料 IsPass = 'N',doWhat = '查無ERP品號'
        /// , 其他IsPass = 'Y', 並Insert至 BBC_ImportData_DT
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
                sql.AppendLine(" DELETE FROM BBC_ImportData_DT WHERE (Parent_ID = @DataID);");

                /*
                 * [判斷商城]
                 * 1:京東POP / 2:天貓 / 3:京東VC / 4:eService / 5:唯品會 / 6:VC工業品 / 999:通用版
                 */
                switch (baseData.MallID)
                {
                    case 5:
                    case 6:
                    case 999:
                        //唯品會, VC工業品, 通用
                        sql.AppendLine(" INSERT INTO BBC_ImportData_DT (");
                        sql.AppendLine(" Parent_ID, Data_ID");
                        sql.AppendLine(" , OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice, Freight");
                        sql.AppendLine(" , IsPass, doWhat, ERP_ModelNo, ERP_Price, ERP_NewPrice, Cnt_Price");
                        sql.AppendLine(" , Currency, ShipmentNo, ShipWho, ShipAddr, ShipTel, StockNum");
                        sql.AppendLine(" , IsGift, Create_Time");
                        sql.AppendLine(" , RemarkTitle, RemarkID, Remark, StockStatus, StockReback, NickName");
                        sql.AppendLine(" , Inv_Type, Inv_Title, Inv_Number, Inv_AddrInfo, Inv_BankInfo, Inv_Message, Inv_Remark");
                        sql.AppendLine(" , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                        sql.AppendLine(" , BuyRemark, SellRemark");
                        sql.AppendLine(" )");
                        sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                        sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice, Tmp.Freight");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                        sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                        sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                        sql.AppendLine("  , 'RMB' AS Currency, Tmp.ShipmentNo, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel, 0 AS StockNum");
                        sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                        sql.AppendLine("  , Tmp.RemarkTitle, '', '', @StockStatus, '', Tmp.NickName");
                        sql.AppendLine("  , Tmp.Inv_Type, Tmp.Inv_Title, Tmp.Inv_Number, Tmp.Inv_AddrInfo, Tmp.Inv_BankInfo, Tmp.Inv_Message, Tmp.Inv_Remark");
                        sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                        sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                        sql.AppendLine(" FROM BBC_ImportData Base");
                        sql.AppendLine("  INNER JOIN BBC_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                        sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPMG Ref ON Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN) AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");
                        sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                        sql.AppendLine(" ORDER BY Tmp.Data_ID");

                        break;

                    case 3:
                        //京東VC
                        sql.AppendLine(" INSERT INTO BBC_ImportData_DT (");
                        sql.AppendLine(" Parent_ID, Data_ID");
                        sql.AppendLine(" , OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice, Freight");
                        sql.AppendLine(" , IsPass, doWhat, ERP_ModelNo, ERP_Price, ERP_NewPrice, Cnt_Price");
                        sql.AppendLine(" , Currency, ShipmentNo, ShipWho, ShipAddr, ShipTel, StockNum");
                        sql.AppendLine(" , IsGift, Create_Time");
                        sql.AppendLine(" , RemarkTitle, RemarkID, Remark, StockStatus, StockReback, NickName");
                        sql.AppendLine(" , Inv_Type, Inv_Title, Inv_Number, Inv_AddrInfo, Inv_BankInfo, Inv_Message, Inv_Remark");
                        sql.AppendLine(" , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                        sql.AppendLine(" , BuyRemark, SellRemark");
                        sql.AppendLine(" )");
                        sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                        sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice, Tmp.Freight");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                        sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                        sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                        sql.AppendLine("  , 'RMB' AS Currency, Tmp.ShipmentNo, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel, 0 AS StockNum");
                        sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                        sql.AppendLine("  , Tmp.RemarkTitle, Tmp.RemarkID, ISNULL(RefMark.Label ,''), @StockStatus, Tmp.StockReback, Tmp.NickName");
                        sql.AppendLine("  , Tmp.Inv_Type, Tmp.Inv_Title, Tmp.Inv_Number, Tmp.Inv_AddrInfo, Tmp.Inv_BankInfo, Tmp.Inv_Message, Tmp.Inv_Remark");
                        sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                        sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                        sql.AppendLine(" FROM BBC_ImportData Base");
                        sql.AppendLine("  INNER JOIN BBC_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                        //單身備註參考
                        sql.AppendLine("  LEFT JOIN BBC_RefRemark RefMark ON Tmp.RemarkID = RefMark.ID AND UPPER(Lang) = UPPER('zh-CN')");
                        sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPMG Ref ON Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN) AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");
                        sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                        sql.AppendLine(" ORDER BY Tmp.Data_ID");

                        break;


                    case 4:
                    case 7:
                        //eService格式
                        sql.AppendLine(" INSERT INTO BBC_ImportData_DT (");
                        sql.AppendLine(" Parent_ID, Data_ID");
                        sql.AppendLine(" , OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice, Freight");
                        sql.AppendLine(" , IsPass, doWhat, ERP_ModelNo, ERP_Price, ERP_NewPrice, Cnt_Price");
                        sql.AppendLine(" , Currency, ShipmentNo, ShipWho, ShipAddr, ShipTel, StockNum");
                        sql.AppendLine(" , IsGift, Create_Time");
                        sql.AppendLine(" , StockStatus");
                        sql.AppendLine(" )");
                        sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                        sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice, Tmp.Freight");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MB001) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MB001) IS NULL THEN '查無ERP品號' ELSE '' END) AS doWhat");
                        sql.AppendLine("  , RTRIM(Ref.MB001) AS ERP_ModelNo");
                        sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                        sql.AppendLine("  , 'RMB' AS Currency, Tmp.ShipmentNo, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel, 0 AS StockNum");
                        sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                        sql.AppendLine("  , @StockStatus");
                        sql.AppendLine(" FROM BBC_ImportData Base");
                        sql.AppendLine("  INNER JOIN BBC_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                        sql.AppendLine("  LEFT JOIN [ProUnion].dbo.INVMB Ref ON Tmp.ProdID = (Ref.MB001 COLLATE Chinese_Taiwan_Stroke_BIN)");
                        sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                        sql.AppendLine(" ORDER BY Tmp.Data_ID");

                        break;

                    case 1:
                    case 2:
                        //京東POP, 天貓, 編號一對多:要再對應MG006(商品規格), 編號一對一,則排除MG006
                        //ERP.COPMG無法重複資料,故對應至PKSYS.refCOPMG
                        sql.AppendLine(" INSERT INTO BBC_ImportData_DT (");
                        sql.AppendLine(" Parent_ID, Data_ID");
                        sql.AppendLine(" , OrderID, ProdID, ProdSpec, BuyCnt, BuyPrice, TotalPrice, Freight");
                        sql.AppendLine(" , IsPass, doWhat, ERP_ModelNo, ERP_Price, ERP_NewPrice, Cnt_Price");
                        sql.AppendLine(" , Currency, ShipmentNo, ShipWho, ShipAddr, ShipTel, StockNum");
                        sql.AppendLine(" , IsGift, Create_Time");
                        sql.AppendLine(" , RemarkTitle, RemarkID, Remark, StockStatus, StockReback, NickName");
                        sql.AppendLine(" , Inv_Type, Inv_Title, Inv_Number, Inv_AddrInfo, Inv_BankInfo, Inv_Message, Inv_Remark");
                        sql.AppendLine(" , Buy_ProdName, Buy_Place, Buy_Warehouse, Buy_Sales, Buy_Time");
                        sql.AppendLine(" , BuyRemark, SellRemark");
                        sql.AppendLine(" )");
                        //--比對1:無商品規格的對應品號
                        sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                        sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.ProdSpec, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice, Tmp.Freight");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                        sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                        sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                        sql.AppendLine("  , 'RMB' AS Currency, Tmp.ShipmentNo, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel, 0 AS StockNum");
                        sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                        sql.AppendLine("  , Tmp.RemarkTitle, '', '', @StockStatus, '', Tmp.NickName");
                        sql.AppendLine("  , Tmp.Inv_Type, Tmp.Inv_Title, Tmp.Inv_Number, Tmp.Inv_AddrInfo, Tmp.Inv_BankInfo, Tmp.Inv_Message, Tmp.Inv_Remark");
                        sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                        sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                        sql.AppendLine(" FROM BBC_ImportData Base");
                        sql.AppendLine("  INNER JOIN BBC_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                        sql.AppendLine("  LEFT JOIN [PKSYS].dbo.refCOPMG Ref ON Ref.DB = 'SZ' AND Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN)");
                        sql.AppendLine("   AND (ISNULL(Ref.MG006, '') = '')");
                        sql.AppendLine(" WHERE (Base.Data_ID = @DataID) AND (Ref.MallID = @MallID) AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");
                        sql.AppendLine(" UNION ALL");
                        //--比對2:有商品規格的對應品號
                        sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                        sql.AppendLine("  , Tmp.OrderID, Tmp.ProdID, Tmp.ProdSpec, Tmp.BuyCnt, Tmp.BuyPrice, Tmp.TotalPrice, Tmp.Freight");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                        sql.AppendLine("  , (CASE WHEN RTRIM(Ref.MG002) IS NULL THEN '客戶品號查無此商品' ELSE '' END) AS doWhat");
                        sql.AppendLine("  , RTRIM(Ref.MG002) AS ERP_ModelNo");
                        sql.AppendLine("  , 0 AS ERP_Price, 0 AS ERP_NewPrice, 0 AS Cnt_Price");
                        sql.AppendLine("  , 'RMB' AS Currency, Tmp.ShipmentNo, Tmp.ShipWho, Tmp.ShipAddr, Tmp.ShipTel, 0 AS StockNum");
                        sql.AppendLine("  , 'N' AS IsGift, GETDATE()");
                        sql.AppendLine("  , Tmp.RemarkTitle, '', '', @StockStatus, '', Tmp.NickName");
                        sql.AppendLine("  , Tmp.Inv_Type, Tmp.Inv_Title, Tmp.Inv_Number, Tmp.Inv_AddrInfo, Tmp.Inv_BankInfo, Tmp.Inv_Message, Tmp.Inv_Remark");
                        sql.AppendLine("  , Tmp.Buy_ProdName, Tmp.Buy_Place, Tmp.Buy_Warehouse, Tmp.Buy_Sales, Tmp.Buy_Time");
                        sql.AppendLine("  , Tmp.BuyRemark, Tmp.SellRemark");
                        sql.AppendLine(" FROM BBC_ImportData Base");
                        sql.AppendLine("  INNER JOIN BBC_ImportData_TempDT Tmp ON Base.Data_ID = Tmp.Parent_ID");
                        sql.AppendLine("  LEFT JOIN [PKSYS].dbo.refCOPMG Ref ON Ref.DB = 'SZ' AND Tmp.ProdID = (Ref.MG003 COLLATE Chinese_Taiwan_Stroke_BIN)");
                        sql.AppendLine("   AND (Tmp.ProdSpec = (ISNULL(Ref.MG006, '') COLLATE Chinese_Taiwan_Stroke_BIN))");
                        sql.AppendLine(" WHERE (Base.Data_ID = @DataID) AND (Ref.MallID = @MallID) AND (Tmp.ProdSpec <> '') AND Base.CustID = (Ref.MG001 COLLATE Chinese_Taiwan_Stroke_BIN)");

                        cmd.Parameters.AddWithValue("MallID", baseData.MallID);

                        break;
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                //1:銷貨單, 2:訂單, B:銷退單 (對應XA002:1,2,B)
                cmd.Parameters.AddWithValue("StockStatus", baseData.Data_Type.Equals(2) ? "B" : "2");

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
        /// BBC_Import_DT 與其他不同Parent_ID的資料比對, 判斷是否有重複的OrderID + ProdID, 若有重複則 IsPass = 'N',doWhat = '重複的單號'
        /// </remarks>
        public bool Check_Step2(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE BBC_ImportData_DT");
                sql.AppendLine(" SET IsPass = 'N', doWhat = '重複的單號'");
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine("  AND (Data_ID IN (");
                sql.AppendLine("     SELECT Base.Data_ID");
                sql.AppendLine("     FROM BBC_ImportData_DT Base");
                sql.AppendLine("     WHERE (Base.Parent_ID = @DataID)");
                sql.AppendLine("      AND EXISTS (");
                sql.AppendLine("       SELECT OrderID FROM BBC_ImportData_DT Chk");
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
        /// 取得ERP庫存, 並更新 StockNum
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
            string CustID, DBS, SWID, ModelNos, Qtys, MallID;

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.CustID, Base.MallID, Cust.DBS, CustDT.SWID");
                sql.AppendLine(" FROM BBC_ImportData Base WITH(NOLOCK)");
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
                        ErrMsg = "無法取得出貨庫別,請確認客戶基本資料已設定.(Check_Step3)";
                        return false;
                    }

                    //填入資料
                    CustID = DT.Rows[0]["CustID"].ToString();
                    DBS = DT.Rows[0]["DBS"].ToString();
                    SWID = DT.Rows[0]["SWID"].ToString();
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
                sql.AppendLine(" FROM BBC_ImportData_DT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (ERP_ModelNo IS NOT NULL) AND (IsGift = 'N')");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "請檢查ERP「客戶核價單」、「客戶品號建立」;若繼續報錯,請通知資訊部.(Check_Step3)";
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


            //***** 取得ERP資料 - 報價, 並回寫至 BBC_ImportData_DT ***** 
            List<RefColumn> dataList = new List<RefColumn>();
            var data = new RefColumn();

            API_ERPData ERPData = new API_ERPData();
            using (DataTable DT = ERPData.GetData(CustID, DBS, ModelNos, Qtys, SWID, out ErrMsg))
            {
                if (DT == null)
                {
                    ErrMsg = "無法取得ERP報價資料.(Check_Step3)";
                    return false;
                }
                if (DT.Rows.Count == 0)
                {
                    ErrMsg = "查無ERP報價資料.(Check_Step3)";
                    return false;
                }

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    //加入項目
                    //*** StockNum 因條件不同,在下面的程式碼進行個別更新 ***
                    data = new RefColumn
                    {
                        ERP_ModelNo = DT.Rows[row]["ModelNo"].ToString(),   //key
                        BuyCnt = DT.Rows[row]["BuyQty"] == DBNull.Value ? 1 : Convert.ToInt16(DT.Rows[row]["BuyQty"]),   //key
                        ERP_Price = DT.Rows[row]["SpQtyPrice"] == null ? 0 : Convert.ToDouble(DT.Rows[row]["SpQtyPrice"]),
                        Currency = DT.Rows[row]["Currency"].ToString(),
                        StockNum = DT.Rows[row]["StockNum"] == null ? 0 : Convert.ToInt16(DT.Rows[row]["StockNum"])
                    };


                    //將項目加入至集合
                    dataList.Add(data);
                }
            }


            //----- 更新 BBC_ImportData_DT, 價格 -----
            if (!Update_Price(dataID, dataList.AsQueryable()))
            {
                ErrMsg = "未正確取得ERP價格.(Check_Step3.Update_Price)";
                return false;
            }

            //----- 判斷商城 -----//
            ArrayList aryStockType = new ArrayList();
            ArrayList aryDBS = new ArrayList();

            switch (MallID)
            {
                case "3":
                    //VC(抓客戶出貨庫別)
                    //----- 更新 BBC_ImportData_DT, 庫存(庫存-預計銷-安全存量), myPrc_GetStock -----
                    if (!Update_Stock(dataID, dataList.AsQueryable()))
                    {
                        ErrMsg = "ERP庫存更新失敗 (f1)";
                        return false;
                    }

                    break;


                case "4":
                case "7":
                    //eService(抓客戶出貨庫別)
                    //庫存計算 = 現有庫存-預計銷-電商安全存量
                    string sockType = GetStockType(SWID);

                    #region -- 取得庫存 --

                    //設定參數
                    aryStockType.Add(sockType);
                    aryDBS.Add(DBS);

                    //宣告WebService
                    //API_GetERPData.ws_GetERPData ws_GetData_es = new API_GetERPData.ws_GetERPData();

                    //取得庫存完整資訊 (GetStockInfo)
                    DataTable DT_Stock_es = ERPData.GetStockInfo(aryStockType, new ArrayList(iModelNo), aryDBS, out ErrMsg);
                    if (DT_Stock_es == null)
                    {
                        ErrMsg = "無法取得庫存(GetStockInfo)";
                        return false;
                    }

                    var stock_es = DT_Stock_es.AsEnumerable()
                        .Select(fld => new
                        {
                            ModelNo = fld.Field<string>("ModelNo"),
                            INV_Safe = fld.Field<decimal>("INV_Safe"),
                            INV_Num = Convert.ToInt32(fld.Field<decimal>("INV_Num")),
                            INV_PreOut = Convert.ToInt32(fld.Field<decimal>("INV_PreOut")),
                            SafeQty_SZEC = fld.Field<int>("SafeQty_SZEC")
                        });


                    //清除集合
                    dataList.Clear();

                    foreach (var item in stock_es)
                    {
                        /* 
                        * 庫存取得 = 現有庫存(INV_Num) - 預計銷(INV_PreOut) - 電商安全存量(SafeQty_SZEC)
                        */
                        data = new RefColumn
                        {
                            ERP_ModelNo = item.ModelNo,
                            StockNum = item.INV_Num - item.INV_PreOut - item.SafeQty_SZEC
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }


                    //----- 更新 BBC_ImportData_DT, 庫存 -----
                    if (!Update_Stock(dataID, dataList.AsQueryable()))
                    {
                        ErrMsg = "ERP庫存更新失敗 (f3)";
                        return false;
                    }

                    #endregion

                    break;

                case "1":
                case "2":
                case "5":
                case "6":
                case "999":
                    //POP, 天貓, 唯品會, VC工業品 (固定A01倉)
                    #region -- 取得庫存 --

                    //設定參數
                    aryStockType.Add("A01");
                    aryDBS.Add(DBS);

                    //取得庫存完整資訊 (GetStockInfo)
                    DataTable DT_Stock = ERPData.GetStockInfo(aryStockType, new ArrayList(iModelNo), aryDBS, out ErrMsg);
                    if (DT_Stock == null)
                    {
                        ErrMsg = "無法取得庫存(GetStockInfo)";
                        return false;
                    }

                    var stock = DT_Stock.AsEnumerable()
                        .Select(fld => new
                        {
                            ModelNo = fld.Field<string>("ModelNo"),
                            INV_Safe = fld.Field<decimal>("INV_Safe"),
                            INV_Num = Convert.ToInt32(fld.Field<decimal>("INV_Num")),
                            INV_PreOut = Convert.ToInt32(fld.Field<decimal>("INV_PreOut"))
                        });


                    //清除集合
                    dataList.Clear();

                    foreach (var item in stock)
                    {
                        /* 安全庫存判斷
                        * 安全庫存(INV_Safe) > 0 : 庫存取得 = 現有庫存(INV_Num)
                        * 安全庫存(INV_Safe) = 0 : 庫存取得 = 現有庫存(INV_Num) - 預計銷(INV_PreOut)
                        */
                        data = new RefColumn
                        {
                            ERP_ModelNo = item.ModelNo,
                            StockNum = (item.INV_Safe > 0) ? item.INV_Num : item.INV_Num - item.INV_PreOut
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }


                    //----- 更新 BBC_ImportData_DT, 庫存 -----
                    if (!Update_Stock(dataID, dataList.AsQueryable()))
                    {
                        ErrMsg = "ERP庫存更新失敗 (f2)";
                        return false;
                    }

                    #endregion

                    break;
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

            //取得資料,並合併Table
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
                    sql.AppendLine("  FROM BBC_ImportData_DT");
                    sql.AppendLine("  WHERE (Parent_ID = @DataID)");
                    sql.AppendLine(" );");
                    sql.AppendLine(" INSERT INTO BBC_ImportData_DT( ");
                    sql.AppendLine("  Parent_ID, Data_ID, OrderID, ProdID");
                    sql.AppendLine("  , BuyCnt, BuyPrice, TotalPrice, Freight, IsPass, ERP_ModelNo, Currency");
                    sql.AppendLine("  , ShipWho, ShipAddr, ShipTel, StockNum, IsGift, StockStatus");
                    sql.AppendLine("  , PromoID, PromoName");
                    sql.AppendLine(" ) VALUES (");

                    //col:Parent_ID, Data_ID, OrderID, ProdID
                    sql.AppendLine("  @DataID, @NewID, '{0}', '{1}'".FormatThis(
                        DT.Rows[row]["OrderID"], DT.Rows[row]["ProdID"]
                        ));
                    //col:BuyCnt, BuyPrice, TotalPrice, Freight, IsPass, ERP_ModelNo, Currency
                    sql.AppendLine(", {0}, {1}, {2}, {3}, 'Y', '{4}', '{5}'".FormatThis(
                        DT.Rows[row]["BuyCnt"]
                        , DT.Rows[row]["BuyPrice"]
                        , DT.Rows[row]["TotalPrice"]
                        , DT.Rows[row]["Freight"]
                        , DT.Rows[row]["ERP_ModelNo"]
                        , DT.Rows[row]["Currency"]
                        ));
                    //col:ShipWho, ShipAddr, ShipTel, StockNum, IsGift, StockStatus
                    sql.AppendLine(", N'{0}', N'{1}', N'{2}', {3}, '{4}', '{5}'".FormatThis(
                        DT.Rows[row]["ShipWho"]
                        , DT.Rows[row]["ShipAddr"]
                        , DT.Rows[row]["ShipTel"]
                        , DT.Rows[row]["StockNum"]
                        , DT.Rows[row]["IsGift"]
                        , DT.Rows[row]["StockStatus"]
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
        /// 處理DataTable資料內容
        /// </summary>
        /// <param name="srcDT"></param>
        /// <param name="mallID">商城</param>
        /// <param name="id">匯入檔資料編號</param>
        /// <param name="type">活動類型</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        private DataTable GetPromoDT(DataTable srcDT, string mallID, string id, string type, out string ErrMsg)
        {
            SZBBCRepository _data = new SZBBCRepository();

            DataTable getDT = _data.GetPromoItems(mallID, id, type, out ErrMsg);
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
        /// [BBC] 更新ERP價格
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
                    sql.AppendLine(" UPDATE BBC_ImportData_DT");
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
                sql.AppendLine(" UPDATE BBC_ImportData_DT SET IsPass = 'N', doWhat = N'查無ERP價格'");
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
        /// [BBC] 更新庫存(退貨單不計算)
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private bool Update_Stock(string dataID, IQueryable<RefColumn> query)
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
                    sql.AppendLine(" UPDATE BBC_ImportData_DT");
                    sql.AppendLine(" SET StockNum = @StockNum_{0}".FormatThis(row));
                    sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (StockStatus <> 'B')");
                    sql.AppendLine("  AND (ERP_ModelNo = @ModelNo_{0});".FormatThis(row));


                    cmd.Parameters.AddWithValue("ModelNo_" + row, item.ERP_ModelNo);
                    cmd.Parameters.AddWithValue("StockNum_" + row, item.StockNum);

                    row++;
                }

                //Update 單據狀態,庫存足夠的Update = 1 (對應XA002)
                sql.AppendLine(" UPDATE BBC_ImportData_DT SET StockStatus = '1'");
                sql.AppendLine(" WHERE (StockNum - BuyCnt > 0) AND (Parent_ID = @DataID) AND (StockStatus <> 'B')");

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
        /// 判斷庫存, 若同單號內有一筆庫存不足, 則轉訂單(2)
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 對象：唯品會(5) / 通用(999)
        /// </remarks>
        public bool Update_NewStockType(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                //** 有一筆庫存不足則整筆轉訂單 **
                sql.AppendLine(" UPDATE BBC_ImportData_DT");
                sql.AppendLine(" SET StockStatus = '2', doWhat = '庫存不足轉訂單'");
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine(" AND (OrderID IN (");
                sql.AppendLine(" 	SELECT DT.OrderID");
                sql.AppendLine(" 	FROM BBC_ImportData_DT DT");
                sql.AppendLine(" 	WHERE (DT.Parent_ID = @DataID)");
                sql.AppendLine(" 	 AND (DT.StockStatus = '2')");
                sql.AppendLine(" 	GROUP BY DT.OrderID");
                sql.AppendLine(" ));");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    return false;
                }
            }


            return true;

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
                sql.AppendLine(" UPDATE BBC_ImportData_DT");
                sql.AppendLine(" SET Cnt_Price = (");
                sql.AppendLine("     SELECT (");
                sql.AppendLine("      CONVERT(FLOAT,");
                sql.AppendLine("         (");
                sql.AppendLine("          (SELECT TOP 1 TotalPrice FROM BBC_ImportData_DT Ref WHERE (Ref.Parent_ID = @DataID) AND (Ref.OrderID = Data.OrderID))");
                sql.AppendLine("         ) ");
                sql.AppendLine("          - SUM(Data.ERP_Price * Data.BuyCnt)");
                sql.AppendLine("      )");
                sql.AppendLine("     ) AS Cnt_Price");
                sql.AppendLine("     FROM BBC_ImportData_DT Data");
                sql.AppendLine("     WHERE (Data.Parent_ID = @DataID) AND (Data.OrderID = BBC_ImportData_DT.OrderID) AND (Data.IsGift = 'N')");
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
             * [判斷方法]
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
                sql.AppendLine("     FROM BBC_ImportData_DT");
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
                    dataSQL.AppendLine(" DECLARE @NewID AS INT, @StockStatus AS NVARCHAR(2), @Remark AS NVARCHAR(250)  ");


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

                                /* 判斷所屬訂單中的所有品號, 是否都有貨 */
                                dataSQL.AppendLine(" IF (");
                                dataSQL.AppendLine("  SELECT COUNT(*) FROM BBC_ImportData_DT WHERE (Parent_ID = @DataID) AND (OrderID = N'{0}') AND (StockStatus = '1')".FormatThis(OrderID));
                                dataSQL.AppendLine(" ) > 0");
                                dataSQL.AppendLine("  SET @StockStatus = '1'");
                                dataSQL.AppendLine(" ELSE");
                                dataSQL.AppendLine("  SET @StockStatus = '2'");

                                //--- 新增W001 ---
                                dataSQL.AppendLine(" SET @NewID = (");
                                dataSQL.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                                dataSQL.AppendLine("  FROM BBC_ImportData_DT ");
                                dataSQL.AppendLine("  WHERE (Parent_ID = @DataID) ");
                                dataSQL.AppendLine(" );");

                                //--- 取得同訂單的單頭備註 ---
                                dataSQL.AppendLine(" SET @Remark = (");
                                dataSQL.AppendLine("  SELECT TOP 1 RemarkTitle ");
                                dataSQL.AppendLine("  FROM BBC_ImportData_DT ");
                                dataSQL.AppendLine("  WHERE (Parent_ID = @DataID) AND (OrderID = N'{0}') ".FormatThis(OrderID));
                                dataSQL.AppendLine(" );");

                                dataSQL.AppendLine(" INSERT INTO BBC_ImportData_DT( ");
                                dataSQL.AppendLine("  Parent_ID, Data_ID, OrderID, ProdID, BuyCnt, BuyPrice, TotalPrice, Freight");
                                dataSQL.AppendLine("  , IsPass, ERP_ModelNo, ERP_Price, ERP_NewPrice, Currency, StockNum, IsGift, Create_Time");
                                dataSQL.AppendLine("  , StockStatus, RemarkTitle");
                                dataSQL.AppendLine(" ) VALUES (");
                                dataSQL.AppendLine("  @DataID, @NewID, N'{0}', 'W001', 1, {1}, {1}, 0".FormatThis(
                                    OrderID, CntPrice
                                    ));
                                dataSQL.AppendLine("  , 'Y', 'W001', {0},{0}, 'RMB', 1, 'N', GETDATE()".FormatThis(
                                    CntPrice
                                    ));
                                dataSQL.AppendLine("  , @StockStatus, @Remark");
                                dataSQL.AppendLine(");");
                            }
                            else if (CntPrice > 0)
                            {
                                //--- 更新價格 ---
                                dataSQL.AppendLine(" UPDATE BBC_ImportData_DT");
                                dataSQL.AppendLine(" SET ERP_NewPrice = (CAST({0} AS FLOAT)/BuyCnt) + ERP_Price".FormatThis(CntPrice));
                                dataSQL.AppendLine(" WHERE (Parent_ID = @DataID) AND (Data_ID = {0});".FormatThis(RowID));

                            }

                        }


                        //--- 更新主檔狀態 ---
                        dataSQL.AppendLine(" UPDATE BBC_ImportData SET Status = 12 WHERE (Data_ID = @DataID);");

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
                sql.AppendLine(" UPDATE BBC_ImportData_DT");
                sql.AppendLine(" SET ");
                sql.AppendLine(" ShipWho = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipWho FROM BBC_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipWho <> '')");
                sql.AppendLine("  AND (Rel.OrderID = BBC_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , ShipAddr = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipAddr FROM BBC_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipAddr <> '')");
                sql.AppendLine("  AND (Rel.OrderID = BBC_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , ShipTel = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipTel FROM BBC_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipTel <> '')");
                sql.AppendLine("  AND (Rel.OrderID = BBC_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , ShipmentNo = (");
                sql.AppendLine("  SELECT TOP 1 Rel.ShipmentNo FROM BBC_ImportData_DT Rel");
                sql.AppendLine("  WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Rel.ShipmentNo <> '')");
                sql.AppendLine("  AND (Rel.OrderID = BBC_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" , RemarkTitle = (");
                sql.AppendLine(" SELECT TOP 1 Rel.RemarkTitle FROM BBC_ImportData_DT Rel");
                sql.AppendLine(" WHERE (Rel.Parent_ID = @DataID)");
                sql.AppendLine(" AND (Rel.RemarkTitle <> '')");
                sql.AppendLine(" AND (Rel.OrderID = BBC_ImportData_DT.OrderID)");
                sql.AppendLine(" )");
                sql.AppendLine(" WHERE (Parent_ID = @DataID);");

                //** 將發票資料寫入 **
                //清空
                sql.AppendLine(" DELETE FROM BBC_InvoiceItem");
                sql.AppendLine(" WHERE EXISTS (");
                sql.AppendLine("    SELECT *");
                sql.AppendLine("    FROM BBC_ImportData Base");
                sql.AppendLine("     INNER JOIN BBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("    WHERE (Base.Data_ID = @DataID)");
                sql.AppendLine("     AND (BBC_InvoiceItem.TraceID = Base.TraceID) AND (BBC_InvoiceItem.OrderID = DT.OrderID)");
                sql.AppendLine(" )");
                //新增
                sql.AppendLine(" INSERT INTO BBC_InvoiceItem (");
                sql.AppendLine("  TraceID, OrderID");
                sql.AppendLine(" , Inv_Type, Inv_Title, Inv_Number, Inv_AddrInfo, Inv_BankInfo, Inv_Message, Inv_Remark");
                sql.AppendLine(" , Create_Who, Create_Time");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT");
                sql.AppendLine("  Base.TraceID, DT.OrderID");
                sql.AppendLine(" , DT.Inv_Type, DT.Inv_Title, DT.Inv_Number, DT.Inv_AddrInfo, DT.Inv_BankInfo, DT.Inv_Message, DT.Inv_Remark");
                sql.AppendLine(" , @Creater, GETDATE()");
                sql.AppendLine(" FROM BBC_ImportData Base");
                sql.AppendLine("  INNER JOIN BBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                sql.AppendLine("  AND (DT.Inv_Type <> '') AND (DT.Inv_Title <> '') AND (DT.Inv_Number <> '')");
                sql.AppendLine(" GROUP BY Base.TraceID, DT.OrderID, DT.Inv_Type, DT.Inv_Title, DT.Inv_Number, DT.Inv_AddrInfo, DT.Inv_BankInfo, DT.Inv_Message, DT.Inv_Remark");

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
        /// [BBC] 狀態更新為完成 - Step4執行
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_Status(string dataID, out string ErrMsg)
        {
            StringBuilder sql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE BBC_ImportData SET Status = 13, Import_Time = GETDATE() WHERE (Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

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
                sql.AppendLine(" UPDATE BBC_Promo SET PromoName = @PromoName, MallID = @MallID");
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
                /* 發票資料處理 */
                string myInvFullData = "";
                string myInvType = "";
                string myInvTitle = "";
                string myInvNumber = "";
                string myInvAddrInfo = "";
                string myInvBankInfo = "";
                string myInvMessage = "";
                string myInvRemark = "";

                int row = 0;

                //----- Data Loop -----
                foreach (var item in dataItems)
                {
                    //cnt
                    row++;

                    #region -- 發票資料處理 --

                    myInvFullData = item.Remark;
                    if (!string.IsNullOrWhiteSpace(myInvFullData))
                    {
                        //拆解列
                        string[] aryRow = Regex.Split(myInvFullData, "\n");

                        myInvType = item.InvType; //1:普;2:專票
                        myInvTitle = splitItem(aryRow[0]);
                        myInvNumber = splitItem(aryRow[1]);
                        myInvAddrInfo = splitItem(aryRow[2]) + " " + splitItem(aryRow[3]);
                        myInvBankInfo = splitItem(aryRow[4]) + " " + splitItem(aryRow[5]);
                        myInvMessage = splitItem(aryRow[6]);
                        myInvRemark = myInvFullData; //將完整開票暫存在Remark
                        /*                           
                        //(發票抬頭)[0] 单位名称: 广东一方制药有限公司
                        //(稅號)[1] 纳税人识别号: 9144060528000534XG

                        //(地址/電話)[2][3]
                        //地址: 广东省佛山市南海区里水镇旗峰工业开发区
                        //电话: 0757-85609412

                        //(開戶行/帳號)[4][5]
                        //开户银行: 中国农业银行佛山市南海里水支行
                        //开户账号: 44-519501040006042

                        //(寄票信息)[6]
                        //收票人姓名: 陈昭毓
                        */
                    }

                    #endregion

                    //SQL
                    sql.AppendLine(" UPDATE BBC_ImportData_TempDT");
                    sql.AppendLine(" SET ProdID = '{0}', BuyCnt = {1}".FormatThis(item.ProdID, item.BuyCnt));
                    sql.AppendLine(" , Inv_Type = N'{0}', Inv_Title = N'{1}', Inv_Number = N'{2}'".FormatThis(
                            myInvType, myInvTitle, myInvNumber
                        ));
                    sql.AppendLine(" , Inv_AddrInfo	= N'{0}', Inv_BankInfo = N'{1}', Inv_Message = N'{2}', Inv_Remark = N'{3}'".FormatThis(
                            myInvAddrInfo, myInvBankInfo, myInvMessage, myInvRemark
                        ));
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


        #region >> 電商庫存 <<
        /// <summary>
        /// [電商庫存] - 更新單頭資料
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_StockData(StockImportData inst, out string ErrMsg)
        {
            StringBuilder sql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE BBC_StockData SET CustID = @CustID, Upload_File = @Upload_File");
                sql.AppendLine(" , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", inst.Data_ID);
                cmd.Parameters.AddWithValue("CustID", inst.CustID);
                cmd.Parameters.AddWithValue("Upload_File", inst.Upload_File);
                cmd.Parameters.AddWithValue("Update_Who", inst.Update_Who);

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
        private DataTable LookupRawData(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.Data_ID, Base.TraceID, Base.MallID, Base.CustID, Base.Data_Type, Base.Status, Base.Upload_File, Base.Sheet_Name");
                sql.AppendLine("   , Base.Import_Time, Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
                sql.AppendLine("   , Cls.Class_Name AS MallName, ClsSt.Class_Name AS StatusName");
                sql.AppendLine("   , (SELECT TOP 1 RTRIM(MA002) FROM PKSYS.dbo.Customer WITH(NOLOCK) WHERE (MA001 = Base.CustID) AND (DBS = DBC)) AS CustName");
                sql.AppendLine("   , (CASE Base.Data_Type WHEN 1 THEN '未出貨訂單' WHEN 2 THEN '退貨單' ELSE '已出貨訂單' END) AS Data_TypeName");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                sql.AppendLine("   , (SELECT COUNT(*) FROM BBC_ImportData_Log WHERE (Data_ID = Base.Data_ID)) AS LogCnt");
                sql.AppendLine(" FROM BBC_ImportData Base");
                sql.AppendLine("  INNER JOIN BBC_RefClass Cls ON Base.MallID = Cls.Class_ID");
                sql.AppendLine("  LEFT JOIN BBC_RefClass ClsSt ON Base.Status = ClsSt.Class_ID");
                sql.AppendLine(" WHERE (1 = 1) ");

                /* Search */
                if (search != null)
                {
                    string filterDateType = "";

                    foreach (var item in search)
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
                                    sql.Append("      Base.Data_ID IN (SELECT Parent_ID FROM BBC_ImportData_DT WHERE ((UPPER(RTRIM(OrderID)) LIKE '%' + UPPER(@Keyword) + '%') OR (UPPER(ShipmentNo) LIKE '%' + UPPER(@Keyword) + '%') OR (UPPER(ShipTel) LIKE '%' + UPPER(@Keyword) + '%')))");
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
                sql.AppendLine(" SELECT Class_ID AS ID, Class_Name AS Label, StockReport");
                sql.AppendLine(" FROM BBC_RefClass WITH(NOLOCK)");
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