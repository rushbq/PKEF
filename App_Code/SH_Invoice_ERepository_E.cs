using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using LinqToExcel;
using PKLib_Method.Methods;
using SH_Invoice_E.Models;

/*
 * 主要資料庫:SHPK2
 * 資料DB:PKEF.SH_Invoice_ImportData
 * SH開票-電子發票匯入
 */
namespace SH_Invoice_E.Controllers
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
        EndDate = 7
    }


    /// <summary>
    /// 類別參數
    /// </summary>
    /// <remarks>
    /// 1:商城, 2:狀態
    /// </remarks>
    public enum myClass : int
    {
        mall = 1,
        status = 2
    }


    public class SH_Invoice_ERepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// 取得所有資料(傳入預設參數)
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
        /// 取得所有資料
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
                        Status = item.Field<Int16>("Status"),
                        Upload_File = item.Field<string>("Upload_File"),
                        Sheet_Name = item.Field<string>("Sheet_Name"),
                        StatusName = item.Field<string>("StatusName"),
                        LogCnt = item.Field<int>("LogCnt"),

                        Import_Time = item.Field<DateTime?>("Import_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                        Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                        Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),

                        Create_Who = item.Field<string>("Create_Who"),
                        Update_Who = item.Field<string>("Update_Who"),
                        Create_Name = item.Field<string>("Create_Name"),
                        Update_Name = item.Field<string>("Update_Name"),
                        Import_Name = item.Field<string>("Import_Name")

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
        /// 取得匯入資料 - 單身
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
                sql.AppendLine(" FROM SH_Invoice_ImportData_DT WITH(NOLOCK)");
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
                            InvoiceNo = item.Field<string>("InvoiceNo"),
                            InvoiceDate = item.Field<string>("InvoiceDate"),
                            InvPrice = item.Field<double>("InvPrice"),
                            IsPass = item.Field<string>("IsPass"),
                            doWhat = item.Field<string>("doWhat"),
                            Erp_AR_ID = item.Field<string>("Erp_AR_ID"),
                            Erp_SO_ID = item.Field<string>("Erp_SO_ID")

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
        /// 取得Log資料
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
                sql.AppendLine(" FROM SH_Invoice_ImportData_Log WITH(NOLOCK)");
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
        /// 反查ERP 結帳單/銷貨單 (LOG查看)
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
                sql.AppendLine(" SELECT DT.OrderID, DT.Erp_AR_ID, DT.Erp_SO_ID, ISNULL(CONVERT(FLOAT, ACRTA.TA029+ACRTA.TA030), 0) AS ErpPrice");
                sql.AppendLine(" , ACRTA.TA036 AS InvNo, ACRTA.TA200 AS InvDate, ISNULL(DT.InvPrice, 0) AS InvPrice");
                sql.AppendLine(" FROM SH_Invoice_ImportData_DT DT");
                sql.AppendLine("  INNER JOIN [SHPK2].dbo.ACRTA WITH(NOLOCK) ON (ACRTA.TA001 + '-' + ACRTA.TA002) = DT.Erp_AR_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine("  INNER JOIN [SHPK2].dbo.ACRTB WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002");
                sql.AppendLine(" WHERE (1=1)");

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
                                    sql.Append(" AND (DT.Parent_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;
                        }
                    }
                }

                sql.AppendLine(" ORDER BY DT.Erp_AR_ID");

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
                            ErpID = item.Field<string>("Erp_AR_ID"),
                            ErpSOID = item.Field<string>("Erp_SO_ID"),
                            ErpPrice = item.Field<double>("ErpPrice"),
                            InvoiceNo = item.Field<string>("InvNo"),
                            InvoiceDate = item.Field<string>("InvDate"),
                            InvPrice = item.Field<double>("InvPrice")
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
        /// 取得Excel內容
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
        /// 取得Excel必要欄位,用來轉入單身資料
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="traceID">trace id</param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetExcel_DT(string filePath, string sheetName, string traceID)
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
                string myInvoiceNo = "";
                string myInvoiceDate = "";
                double myInvoicePrice = 0;

                //[處理合併儲存格] - 暫存欄:OrderID
                string tmp_OrderID = "";

                //資料迴圈
                foreach (var val in queryVals)
                {
                    #region >> 欄位處理:單號 <<

                    //[處理合併儲存格] - 目前的單號(Key)
                    string curr_OrderID;

                    curr_OrderID = val[0].ToString();

                    //[處理合併儲存格] - 目前欄位非空值, 填入暫存值
                    if (!string.IsNullOrEmpty(curr_OrderID))
                    {
                        tmp_OrderID = curr_OrderID;
                    }

                    //[設定參數] - OrderID
                    myOrderID = string.IsNullOrEmpty(curr_OrderID) ? tmp_OrderID : curr_OrderID;

                    //Check null
                    if (string.IsNullOrEmpty(myOrderID))
                    {
                        break;
                    }

                    #endregion


                    #region >> 欄位處理:其他欄位 <<

                    myInvoiceNo = val[1];
                    myInvoiceDate = val[2];
                    myInvoicePrice = Convert.ToDouble(val[3]);

                    #endregion


                    //加入項目
                    var data = new RefColumn
                    {
                        OrderID = myOrderID,
                        InvoiceNo = myInvoiceNo,
                        InvoiceDate = myInvoiceDate.ToDateString("yyyyMMdd"),
                        InvPrice = myInvoicePrice,
                        Erp_AR_ID = ""
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

        #endregion


        #region -----// Create //-----

        /// <summary>
        /// 建立基本資料 - Step1執行
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
                sql.AppendLine(" INSERT INTO SH_Invoice_ImportData( ");
                sql.AppendLine("  Data_ID, TraceID, Status, Upload_File");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @TraceID, 10, @Upload_File");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("Upload_File", instance.Upload_File);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 建立暫存, 更新主檔欄位 - Step2執行
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool Create_Temp(ImportData baseData, IQueryable<RefColumn> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM SH_Invoice_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM SH_Invoice_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE SH_Invoice_ImportData SET Status = 11, Sheet_Name = @Sheet_Name, Update_Who = @Update_Who, Update_Time = GETDATE() WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in query)
                {
                    if (!string.IsNullOrEmpty(item.InvoiceNo))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                        sql.AppendLine("  FROM SH_Invoice_ImportData_TempDT ");
                        sql.AppendLine("  WHERE Parent_ID = @DataID ");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO SH_Invoice_ImportData_TempDT( ");
                        sql.AppendLine("  Parent_ID, Data_ID, OrderID, InvoiceNo, InvoiceDate, InvPrice");
                        sql.AppendLine(" ) VALUES (");
                        sql.AppendLine("  @DataID, @NewID, '{0}', '{1}', '{2}', {3}".FormatThis(
                            item.OrderID, item.InvoiceNo.Trim(), item.InvoiceDate, item.InvPrice));
                        sql.AppendLine(" );");
                    }
                }

                //Update結帳單號/銷貨單號
                sql.AppendLine(" UPDATE SH_Invoice_ImportData_TempDT");
                sql.AppendLine(" SET Erp_AR_ID = TblBase.Erp_AR_ID, Erp_SO_ID = TblBase.Erp_SO_ID");
                sql.AppendLine(" FROM");
                sql.AppendLine(" (");
                sql.AppendLine("     SELECT DT.OrderID AS OrderID");
                sql.AppendLine("     , RTRIM(ACRTA.TA001) + '-' + RTRIM(ACRTA.TA002) AS Erp_AR_ID");
                sql.AppendLine("     , RTRIM(ACRTB.TB005) + '-' + RTRIM(ACRTB.TB006) AS Erp_SO_ID");
                sql.AppendLine("     FROM SH_Invoice_ImportData_TempDT AS DT");
                sql.AppendLine("      INNER JOIN [SHPK2].dbo.COPTC ON COPTC.TC012 = DT.OrderID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine("      INNER JOIN [SHPK2].dbo.COPTH ON COPTC.TC001 = COPTH.TH014 AND COPTC.TC002 = COPTH.TH015");
                sql.AppendLine("      INNER JOIN [SHPK2].dbo.ACRTB WITH(NOLOCK) ON ACRTB.TB005 = COPTH.TH001 AND ACRTB.TB006 = COPTH.TH002");
                sql.AppendLine("      INNER JOIN [SHPK2].dbo.ACRTA WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002");
                sql.AppendLine("     WHERE (ACRTA.TA036 = '') AND (DT.Parent_ID = @DataID)");
                sql.AppendLine("     GROUP BY DT.OrderID, RTRIM(ACRTA.TA001) + '-' + RTRIM(ACRTA.TA002), RTRIM(ACRTB.TB005) + '-' + RTRIM(ACRTB.TB006)");
                sql.AppendLine(" ) AS TblBase");
                sql.AppendLine(" WHERE (SH_Invoice_ImportData_TempDT.OrderID = TblBase.OrderID)");
                sql.AppendLine(" AND (SH_Invoice_ImportData_TempDT.Parent_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Sheet_Name", baseData.Sheet_Name);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 建立Log
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
                sql.AppendLine("  SELECT ISNULL(MAX(Log_ID), 0) + 1 AS NewID FROM SH_Invoice_ImportData_Log");
                sql.AppendLine(" )");

                sql.AppendLine(" INSERT INTO SH_Invoice_ImportData_Log( ");
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

        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// 刪除資料
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
                sql.AppendLine(" DELETE FROM SH_Invoice_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM SH_Invoice_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM SH_Invoice_ImportData WHERE (Data_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 清空暫存
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
                sql.AppendLine(" DELETE FROM SH_Invoice_ImportData_TempDT WHERE (Parent_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        #endregion


        #region -----// Check //-----
      
        /// <summary>
        /// Check.1 - 將Temp寫入DT
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// Step2 時執行(after 下一步)
        /// *** 重要:資料庫欄位若有修改, 必須按順序調整此區的SQL ***
        /// *** 重要:此處有寫死DB Name(SHPK2), 若有變動須注意 ***
        /// </remarks>
        public bool Check_Step1(string dataID, out string ErrMsg)
        {

            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO SH_Invoice_ImportData_DT (");
                sql.AppendLine(" Parent_ID, Data_ID");
                sql.AppendLine(" , OrderID, InvoiceNo, InvoiceDate, InvPrice, IsPass, doWhat, Erp_AR_ID, Erp_SO_ID");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT Tmp.Parent_ID, Tmp.Data_ID");
                sql.AppendLine("  , Tmp.OrderID, Tmp.InvoiceNo, Tmp.InvoiceDate, Tmp.InvPrice");
                sql.AppendLine("  , (CASE WHEN RTRIM(Tmp.Erp_AR_ID) IS NULL THEN 'N' ELSE 'Y' END) AS IsPass");
                sql.AppendLine("  , (CASE WHEN RTRIM(Tmp.Erp_AR_ID) IS NULL THEN '已有發票號碼' ELSE '' END) AS doWhat");
                sql.AppendLine("  , Tmp.Erp_AR_ID");
                sql.AppendLine("  , Tmp.Erp_SO_ID");
                sql.AppendLine(" FROM SH_Invoice_ImportData_TempDT Tmp");
                sql.AppendLine(" WHERE (Tmp.Parent_ID = @DataID)");
                sql.AppendLine(" ORDER BY Tmp.Data_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// Check.2 - 重複的單號
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// Step2 時執行
        /// 其他不同Parent_ID的資料比對, 判斷是否有重複的OrderID + InvoiceNo, 若有重複則 IsPass = 'N'
        /// </remarks>
        public bool Check_Step2(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE SH_Invoice_ImportData_DT");
                sql.AppendLine(" SET IsPass = 'N', doWhat = '重複的單號及發票號'");
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine("  AND (Data_ID IN (");
                sql.AppendLine("     SELECT Base.Data_ID");
                sql.AppendLine("     FROM SH_Invoice_ImportData_DT Base");
                sql.AppendLine("     WHERE (Base.Parent_ID = @DataID)");
                sql.AppendLine("      AND EXISTS (");
                sql.AppendLine("       SELECT OrderID FROM SH_Invoice_ImportData_DT Chk");
                sql.AppendLine("       WHERE (Base.OrderID = Chk.OrderID) AND (Base.InvoiceNo = Chk.InvoiceNo) ");
                sql.AppendLine("        AND (Chk.Parent_ID <> @DataID)");
                sql.AppendLine("      )");
                sql.AppendLine("  ))");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }
        
        #endregion


        #region -----// Update //-----
        /// <summary>
        /// 更新ERP發票資料 - Step3執行
        /// ** 使用ERP_TAX帳號 **
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool UpdateInvNo(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                sql.AppendLine(" UPDATE [SHPK2].dbo.ACRTA");
                sql.AppendLine(" SET ACRTA.TA036 = DT.InvoiceNo, ACRTA.TA200 = DT.InvoiceDate");
                sql.AppendLine(" FROM [PKEF].dbo.SH_Invoice_ImportData_DT AS DT");
                sql.AppendLine(" WHERE (DT.IsPass = 'Y')");
                sql.AppendLine("  AND (DT.Parent_ID = @DataID)");
                sql.AppendLine("  AND ((ACRTA.TA001 + '-' + ACRTA.TA002) = DT.Erp_AR_ID COLLATE Chinese_Taiwan_Stroke_BIN)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.ERP_TAX, out ErrMsg);
            }
        }


        /// <summary>
        /// 狀態更新為完成 - Step3成功後執行
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_Status(ImportData baseData, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE SH_Invoice_ImportData SET Status = 12, Import_Time = GETDATE(), Import_Who = @Who WHERE (Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion


        #region -- 取得原始資料 --

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
                sql.AppendLine(" SELECT Base.Data_ID, Base.TraceID, Base.Status, Base.Upload_File, Base.Sheet_Name");
                sql.AppendLine("   , Base.Import_Time, Base.Import_Who, Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
                sql.AppendLine("   , ClsSt.Class_Name AS StatusName");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Import_Who)) AS Import_Name");
                sql.AppendLine("   , (SELECT COUNT(*) FROM SH_Invoice_ImportData_Log WHERE (Data_ID = Base.Data_ID)) AS LogCnt");
                sql.AppendLine(" FROM SH_Invoice_ImportData Base");
                sql.AppendLine("  LEFT JOIN SH_Invoice_RefClass ClsSt ON Base.Status = ClsSt.Class_ID");
                sql.AppendLine(" WHERE (1 = 1) ");

                /* Search */
                if (search != null)
                {
                    //string filterDateType = "";

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
                                    //原始單號,發票號
                                    sql.Append("    OR (");
                                    sql.Append("      Base.Data_ID IN (SELECT Parent_ID FROM SH_Invoice_ImportData_DT WHERE (UPPER(RTRIM(OrderID)) LIKE '%' + UPPER(@Keyword) + '%') OR (UPPER(RTRIM(InvoiceNo)) LIKE '%' + UPPER(@Keyword) + '%'))");
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

                            //case (int)mySearch.StartDate:
                            //    if (!string.IsNullOrEmpty(item.Value))
                            //    {
                            //        sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));

                            //        cmd.Parameters.AddWithValue("sDate", item.Value.ToDateString("yyyy/MM/dd 00:00:00"));
                            //    }

                            //    break;

                            //case (int)mySearch.EndDate:
                            //    if (!string.IsNullOrEmpty(item.Value))
                            //    {
                            //        sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));

                            //        cmd.Parameters.AddWithValue("eDate", item.Value.ToDateString("yyyy/MM/dd 23:59:59"));
                            //    }

                            //    break;

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
                sql.AppendLine(" SELECT Class_ID AS ID, Class_Name AS Label");
                sql.AppendLine(" FROM SH_Invoice_RefClass WITH(NOLOCK)");
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