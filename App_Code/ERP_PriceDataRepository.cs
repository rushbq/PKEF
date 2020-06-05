using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP_PriceData.Models;
using LinqToExcel;
using PKLib_Method.Methods;


/*
 * 報價單匯入
 */
namespace ERP_PriceData.Controllers
{

    public class ERP_PriceDataRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// 指定資料
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ImportData> GetOne(Dictionary<string, string> search
            , out string ErrMsg)
        {
            int dataCnt = 0;
            return GetList(search, 0, 1, out dataCnt, out ErrMsg);
        }

        public IQueryable<ImportData> GetAllList(Dictionary<string, string> search
            , out int DataCnt, out string ErrMsg)
        {
            return GetList(search, 0, 9999999, out DataCnt, out ErrMsg);
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="startRow">StartRow</param>
        /// <param name="endRow">RecordsPerPage</param>
        /// <param name="DataCnt">傳址參數(資料總筆數)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ImportData> GetList(Dictionary<string, string> search
            , int startRow, int endRow
            , out int DataCnt, out string ErrMsg)
        {
            ErrMsg = "";

            try
            {
                /* 開始/結束筆數計算 */
                int cntStartRow = startRow + 1;
                int cntEndRow = startRow + endRow;

                //----- 宣告 -----
                List<ImportData> dataList = new List<ImportData>(); //資料容器
                List<SqlParameter> sqlParamList = new List<SqlParameter>(); //SQL參數容器
                List<SqlParameter> subParamList = new List<SqlParameter>(); //SQL參數取得
                StringBuilder sql = new StringBuilder(); //SQL語法容器
                StringBuilder subSql = new StringBuilder(); //條件SQL取得
                DataCnt = 0;    //資料總數

                //取得SQL語法
                subSql = _ListSQL(search);
                //取得SQL參數集合
                subParamList = _ListParams(search);


                #region >> 資料筆數SQL查詢 <<
                using (SqlCommand cmdCnt = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.Clear();
                    sql.AppendLine(" SELECT COUNT(TbAll.Data_ID) AS TotalCnt FROM (");

                    //子查詢SQL
                    sql.Append(subSql);

                    sql.AppendLine(" ) AS TbAll");

                    //----- SQL 執行 -----
                    cmdCnt.CommandText = sql.ToString();
                    cmdCnt.Parameters.Clear();
                    sqlParamList.Clear();
                    //cmd.CommandTimeout = 60;   //單位:秒

                    //----- SQL 固定參數 -----
                    //sqlParamList.Add(new SqlParameter("@Lang", lang.ToUpper()));

                    //----- SQL 條件參數 -----
                    //加入篩選後的參數
                    sqlParamList.AddRange(subParamList);

                    //加入參數陣列
                    cmdCnt.Parameters.AddRange(sqlParamList.ToArray());

                    //Execute
                    using (DataTable DTCnt = dbConn.LookupDT(cmdCnt, out ErrMsg))
                    {
                        //資料總筆數
                        if (DTCnt.Rows.Count > 0)
                        {
                            DataCnt = Convert.ToInt32(DTCnt.Rows[0]["TotalCnt"]);
                        }
                    }

                    //*** 在SqlParameterCollection同個循環內不可有重複的SqlParam,必須清除才能繼續使用. ***
                    cmdCnt.Parameters.Clear();
                }
                #endregion


                #region >> 主要資料SQL查詢 <<

                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.Clear();
                    sql.AppendLine(" SELECT TbAll.* FROM (");

                    //子查詢SQL
                    sql.Append(subSql);

                    sql.AppendLine(" ) AS TbAll");

                    sql.AppendLine(" WHERE (TbAll.RowIdx >= @startRow) AND (TbAll.RowIdx <= @endRow)");
                    sql.AppendLine(" ORDER BY TbAll.RowIdx");

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.Clear();
                    sqlParamList.Clear();
                    //cmd.CommandTimeout = 60;   //單位:秒

                    //----- SQL 固定參數 -----
                    sqlParamList.Add(new SqlParameter("@startRow", cntStartRow));
                    sqlParamList.Add(new SqlParameter("@endRow", cntEndRow));

                    //----- SQL 條件參數 -----
                    //加入篩選後的參數
                    sqlParamList.AddRange(subParamList);

                    //加入參數陣列
                    cmd.Parameters.AddRange(sqlParamList.ToArray());

                    //Execute
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
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
                                SeqNo = item.Field<Int32>("SeqNo"),
                                TraceID = item.Field<string>("TraceID"),
                                CustID = item.Field<string>("CustID"),
                                CustName = item.Field<string>("CustName"),
                                OrderType = item.Field<string>("OrderType"),
                                OrderNo = item.Field<string>("OrderNo"),
                                DBS = item.Field<string>("DBS"),
                                ValidDate = item.Field<string>("ValidDate"),
                                Status = item.Field<Int16>("Status"),
                                //10:匯入中 / 20:轉入完成
                                StatusName = item.Field<Int16>("Status").Equals(20) ? "轉入完成" : "匯入中",
                                Upload_File = item.Field<string>("Upload_File"),
                                Sheet_Name = item.Field<string>("Sheet_Name"),
                                ErrMessage = item.Field<string>("ErrMessage"),
                                ErrTime = item.Field<DateTime?>("ErrTime").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                                Import_Time = item.Field<DateTime?>("Import_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                                Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                                Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
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

                #endregion

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// 取得SQL查詢
        /// ** TSQL查詢條件寫在此 **
        /// </summary>
        /// <param name="search">search集合</param>
        /// <returns></returns>
        /// <see cref="GetTempList"/>
        private StringBuilder _ListSQL(Dictionary<string, string> search)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(" SELECT Base.SeqNo, Base.Data_ID, Base.TraceID");
            sql.AppendLine(" , Base.CustID, Base.OrderType, Base.OrderNo, Base.DBS, Base.ValidDate");
            sql.AppendLine(" , (SELECT TOP 1 RTRIM(MA002) FROM [PKSYS].dbo.Customer WITH(NOLOCK) WHERE (MA001 = Base.CustID) AND (DBS = DBC)) AS CustName");
            sql.AppendLine(" , Base.[Status]");
            sql.AppendLine(" , Base.Upload_File, Base.Sheet_Name, Base.Import_Time");
            sql.AppendLine(" , ISNULL(Base.ErrMessage, '') AS ErrMessage, Base.ErrTime");
            sql.AppendLine(" , Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
            sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
            sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
            sql.AppendLine(" , ROW_NUMBER() OVER(ORDER BY Base.[Status], Base.Create_Time DESC) AS RowIdx");
            sql.AppendLine(" FROM ErpOrderPrice_ImportData Base");
            sql.AppendLine(" WHERE (1=1)");

            /* Search */
            #region >> filter <<

            if (search != null)
            {
                //過濾空值
                var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                //查詢內容
                foreach (var item in thisSearch)
                {
                    switch (item.Key)
                    {
                        case "DataID":
                            sql.Append(" AND (Base.Data_ID = @Data_ID)");

                            break;

                        case "Keyword":
                            //關鍵字
                            sql.Append(" AND (");
                            sql.Append(" (UPPER(Base.TraceID) LIKE '%' + UPPER(@keyword) + '%')");
                            sql.Append(" OR (UPPER(Base.CustID) LIKE '%' + UPPER(@keyword) + '%')");
                            sql.Append(" OR (");
                            sql.Append("  Base.CustID IN (SELECT TOP 1 RTRIM(MA001) FROM PKSYS.dbo.Customer WHERE (MA001 = Base.CustID) AND (DBS = DBC) AND (UPPER(RTRIM(MA002)) LIKE '%' + UPPER(@keyword) + '%'))");
                            sql.Append(" )");
                            sql.Append(")");

                            break;

                        case "DBS":
                            sql.Append(" AND (Base.DBS = @DBS)");
                            break;

                        case "sDate":
                            sql.Append(" AND (Base.Create_Time >= @sDate)");
                            break;
                        case "eDate":
                            sql.Append(" AND (Base.Create_Time <= @eDate)");
                            break;

                    }
                }
            }
            #endregion

            return sql;
        }


        /// <summary>
        /// 取得條件參數
        /// ** SQL參數設定寫在此 **
        /// </summary>
        /// <param name="search">search集合</param>
        /// <returns></returns>
        /// <see cref="GetTempList"/>
        private List<SqlParameter> _ListParams(Dictionary<string, string> search)
        {
            //declare
            List<SqlParameter> sqlParamList = new List<SqlParameter>();

            //get values
            if (search != null)
            {
                //過濾空值
                var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                //查詢內容
                foreach (var item in thisSearch)
                {
                    switch (item.Key)
                    {
                        case "DataID":
                            sqlParamList.Add(new SqlParameter("@Data_ID", item.Value));

                            break;

                        case "Keyword":
                            sqlParamList.Add(new SqlParameter("@keyword", item.Value));

                            break;

                        case "DBS":
                            sqlParamList.Add(new SqlParameter("@DBS", item.Value));
                            break;

                        case "sDate":
                            sqlParamList.Add(new SqlParameter("@sDate", item.Value + " 00:00:00"));
                            break;
                        case "eDate":
                            sqlParamList.Add(new SqlParameter("@eDate", item.Value + " 23:59:59"));
                            break;

                    }
                }
            }


            return sqlParamList;
        }



        /// <summary>
        /// 取得單身資料
        /// </summary>
        /// <param name="_parentID">上層編號</param>
        /// <param name="search">查詢參數</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ImportDataDT> GetDetailList(string _parentID, Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ImportDataDT> dataList = new List<ImportDataDT>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT DT.Parent_ID, DT.Data_ID");
                sql.AppendLine(" , DT.ProdID, DT.Cust_ModelNo, DT.ERP_ModelNo, ISNULL(DT.InputPrice, 0) InputPrice");
                sql.AppendLine(" , DT.StockType, DT.IsPass, ISNULL(DT.doWhat, '') doWhat");
                sql.AppendLine(" FROM ErpOrderPrice_ImportData Base");
                sql.AppendLine("  INNER JOIN ErpOrderPrice_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine(" WHERE (DT.Parent_ID = @Parent_ID)");

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
                                sql.Append(" AND (DT.Data_ID = @DataID)");

                                cmd.Parameters.AddWithValue("DataID", item.Value);
                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY DT.Data_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", _parentID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT != null)
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new ImportDataDT
                            {
                                Data_ID = item.Field<Int32>("Data_ID"),
                                Parent_ID = item.Field<Guid>("Parent_ID"),
                                ProdID = item.Field<string>("ProdID"),      //EXCEL中的品號
                                Cust_ModelNo = item.Field<string>("Cust_ModelNo"),  //客戶品號:對應EDI欄位:XA026
                                ERP_ModelNo = item.Field<string>("ERP_ModelNo"),    //品號:對應EDI欄位:XA011
                                InputPrice = item.Field<double?>("InputPrice"),   //價格:對應EDI欄位:XA013
                                IsPass = item.Field<string>("IsPass"),      //是否通過檢查
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

        }


        /// <summary>
        /// 取得Excel欄位,用來轉入資料
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <returns></returns>
        /// <remarks>
        /// [欄位]
        /// 客戶品號/PK品號:string
        /// 訂單金額:float
        /// </remarks>
        public IQueryable<ImportDataDT> Get_ExcelData(string filePath, string sheetName)
        {
            try
            {
                //----- 宣告 -----
                List<ImportDataDT> dataList = new List<ImportDataDT>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string _ProdID = "";
                double _BuyPrice = 0;

                //資料迴圈
                foreach (var val in queryVals)
                {
                    _ProdID = val[0];
                    _BuyPrice = Convert.ToDouble(val[1]);

                    if (!string.IsNullOrWhiteSpace(_ProdID))
                    {
                        //加入項目
                        var data = new ImportDataDT
                        {
                            ProdID = _ProdID,
                            InputPrice = _BuyPrice
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

                throw new Exception("請檢查Excel格式是否正確!!格式可參考Excel範本." + ex.Message.ToString());
            }
        }


        /// <summary>
        /// 取得單別選單
        /// </summary>
        /// <param name="SrcCompanyID">取得哪家公司別的資料</param>
        /// <returns></returns>
        public IQueryable<RefClass> GetPriceTypeID(string SrcCompanyID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            List<RefClass> dataList = new List<RefClass>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                /* 設定DB Name */
                string SrcDatabase;
                //來源DB
                switch (SrcCompanyID.ToUpper())
                {
                    case "SZ":
                        SrcDatabase = "ProUnion";
                        break;

                    case "SH":
                        SrcDatabase = "SHPK2";
                        break;

                    default:
                        SrcDatabase = "prokit2";
                        break;
                }


                //--- 判斷類型,取得對應的SQL ---
                //報價(21)
                sql.AppendLine(" SELECT RTRIM(MQ001) AS ID, RTRIM(MQ002) AS Label, MQ004 AS NoType");
                sql.AppendLine(" FROM ##SrcDatabase##.dbo.CMSMQ");
                sql.AppendLine(" WHERE (MQ003 = '21')");


                //Replace DB 前置詞
                sql.Replace("##SrcDatabase##", SrcDatabase);


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
                        var data = new RefClass
                        {
                            ID = item.Field<string>("ID"),
                            Label = item.Field<string>("Label"),
                            NoType = item.Field<string>("NoType")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }

        }


        /// <summary>
        /// EDI轉入排程LOG
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


        #endregion


        #region -----// Create //-----
        /// <summary>
        /// [Step1] 建立基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create(ImportData instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO ErpOrderPrice_ImportData (");
                sql.AppendLine("  Data_ID, TraceID, CustID, OrderType, OrderNo");
                sql.AppendLine("  , DBS, ValidDate, Status, Upload_File");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @TraceID, @CustID, @OrderType, @OrderNo");
                sql.AppendLine("  , @DBS, @ValidDate, @Status, @Upload_File");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("OrderType", instance.OrderType);
                cmd.Parameters.AddWithValue("OrderNo", instance.OrderNo);
                cmd.Parameters.AddWithValue("DBS", instance.DBS);
                cmd.Parameters.AddWithValue("ValidDate", instance.ValidDate.ToDateString("yyyyMMdd"));
                cmd.Parameters.AddWithValue("Status", 10);
                cmd.Parameters.AddWithValue("Upload_File", instance.Upload_File);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);


                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [Step2] 建立單身, 更新主檔欄位
        /// </summary>
        /// <param name="baseData">單頭資料(填入ERP區間日期)</param>
        /// <param name="query">單身資料</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CreateDetail(ImportData baseData, IQueryable<ImportDataDT> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM ErpOrderPrice_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE ErpOrderPrice_ImportData SET Sheet_Name = @Sheet_Name, Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in query)
                {
                    if (!string.IsNullOrWhiteSpace(item.ProdID))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                        sql.AppendLine("  FROM ErpOrderPrice_ImportData_DT");
                        sql.AppendLine("  WHERE (Parent_ID = @DataID)");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO ErpOrderPrice_ImportData_DT( ");
                        sql.AppendLine("  Parent_ID, Data_ID, ProdID, InputPrice");
                        sql.AppendLine(" ) VALUES (");
                        sql.AppendLine("  @DataID, @NewID, '{0}', {1}".FormatThis(
                            item.ProdID, item.InputPrice));
                        sql.AppendLine(" );");
                    }
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Sheet_Name", baseData.Sheet_Name);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        ///// <summary>
        ///// [Step3] Insert單身資料, 單筆新增
        ///// </summary>
        ///// <param name="baseData"></param>
        ///// <param name="detail"></param>
        ///// <param name="ErrMsg"></param>
        ///// <returns></returns>
        //public Int32 CreateDetailItem(ImportData baseData, ImportDataDT detail, out string ErrMsg)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" UPDATE ErpOrderPrice_ImportData SET Status = 12, Update_Who = @Update_Who, Update_Time = GETDATE()");
        //        sql.AppendLine(" WHERE (Data_ID = @DataID);");

        //        sql.AppendLine(" DECLARE @NewID AS INT");
        //        sql.AppendLine(" SET @NewID = 0"); //default value

        //        if (!string.IsNullOrWhiteSpace(detail.ProdID))
        //        {
        //            sql.AppendLine(" SET @NewID = (");
        //            sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
        //            sql.AppendLine("  FROM ErpOrderPrice_ImportData_DT");
        //            sql.AppendLine("  WHERE (Parent_ID = @DataID)");
        //            sql.AppendLine(" )");

        //            sql.AppendLine(" INSERT INTO ErpOrderPrice_ImportData_DT( ");
        //            sql.AppendLine("  Parent_ID, Data_ID");
        //            sql.AppendLine("  , ShipFrom, ProdID, BuyCnt, InputCnt");
        //            sql.AppendLine(" ) VALUES (");
        //            sql.AppendLine("  @DataID, @NewID, @ShipFrom, '{0}', {1}, {1}".FormatThis(
        //                detail.ProdID, detail.BuyCnt));
        //            sql.AppendLine(" );");
        //        }

        //        sql.AppendLine(" SELECT @NewID AS myDataID");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
        //        cmd.Parameters.AddWithValue("ShipFrom", baseData.Data_Type);
        //        cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

        //        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
        //        {
        //            return Convert.ToInt32(DT.Rows[0]["myDataID"]);
        //        }
        //    }

        //}


        /// <summary>
        /// [Step3] 建立EDI資料
        /// SELECT EDI所需欄位資料, 並分批(50筆)呼叫Webservice(API_ErpData), 寫入EDI
        /// </summary>
        /// <param name="_dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_EDI(string _dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("  Corp.DB_Name AS XA001");
                sql.AppendLine("  , '3' AS XA002  /*固定為報價單(3)*/");
                sql.AppendLine("  , Base.OrderType AS XA003");
                sql.AppendLine("  , ISNULL(Base.OrderNo, '') AS XA004");
                sql.AppendLine("  , Base.TraceID AS XA006");
                sql.AppendLine("  , Base.CustID AS XA007");
                sql.AppendLine("  , Cust.MA016 AS XA008");
                sql.AppendLine("  , Cust.MA014 AS XA009");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA010");
                sql.AppendLine("  , DT.ERP_ModelNo AS XA011");
                sql.AppendLine("  , 1 AS XA012");
                sql.AppendLine("  , DT.InputPrice AS XA013");
                sql.AppendLine("  , DT.StockType AS XA014");
                sql.AppendLine("  , Cust.MA027 AS XA015");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA016");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA017");
                sql.AppendLine("  , '1' AS XA020");
                sql.AppendLine("  , 0 AS XA021");
                sql.AppendLine("  , RTRIM(Cust.MA004) AS XA022");
                sql.AppendLine("  , Cust.MA006 AS XA023");
                sql.AppendLine("  , 'EF報價匯入' AS XA024");
                sql.AppendLine("  , Base.TraceID AS XA025");
                sql.AppendLine("  , DT.ProdID AS XA026");
                sql.AppendLine("  , '2' AS XA027");
                sql.AppendLine("  , '1' AS XA028");
                sql.AppendLine("  , '' AS XA031"); //與API對應,不可省略
                sql.AppendLine("  , '' AS XA032"); //與API對應,不可省略
                sql.AppendLine("  , '' AS XA033"); //與API對應,不可省略
                sql.AppendLine("  , RIGHT(('000' + CAST(DT.Data_ID AS VARCHAR(4))), 4) AS XA034"); //自訂序號
                sql.AppendLine(" FROM ErpOrderPrice_ImportData Base");
                sql.AppendLine("  INNER JOIN ErpOrderPrice_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKSYS].dbo.Customer Cust ON Base.CustID = RTRIM(Cust.MA001) AND (Cust.DBS = Cust.DBC)");
                sql.AppendLine("  INNER JOIN [PKSYS].dbo.Param_Corp Corp ON Base.DBS = Corp.Corp_ShortName");
                sql.AppendLine(" WHERE (Base.Data_ID = @Data_ID)");
                sql.AppendLine(" ORDER BY DT.Data_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", _dataID);


                /*
                 * 選取資料,批次呼叫API
                 */
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT == null)
                    {
                        ErrMsg = "沒有可匯入的資料,請確認Step3是否有資料.若持續出錯,請通知MIS.";
                        return false;
                    }

                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "沒有可匯入的資料,請確認Step3是否有資料.";
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
                                    //TableName不可修改(與API一致)
                                    myDT.TableName = "MyEDITable";
                                    myDS.Tables.Add(myDT);
                                }

                                /*
                                 筆數過多會失敗, Http 有上限, 目前設定每 50 筆轉入
                                */
                                //回傳Webservice (API_ErpData)資料DataSet, Token, 測試模式(Y/N))
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

        #endregion


        #region -----// Check Jobs //-----

        /// <summary>
        /// (Job1檢查客戶品號) Update Cust_ModelNo, ERP_ModelNo
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_custID">客戶編號</param>
        /// <param name="_dbs">TW/SH</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob1(string _parentID, string _custID,string _dbs, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" UPDATE ErpOrderPrice_ImportData_DT");
                    sql.AppendLine(" SET IsPass = 'Y', Cust_ModelNo = RTRIM(TblErp.MG003), ERP_ModelNo = RTRIM(TblErp.MG002)");
                    sql.AppendLine(" FROM (");
                    sql.AppendLine("        SELECT ERPData.MG002, ERPData.MG003");
                    sql.AppendLine("        FROM [{0}].dbo.COPMG ERPData WITH(NOLOCK)".FormatThis(GetDBName(_dbs)));
                    sql.AppendLine("        WHERE (ERPData.MG001 = @CustID)");
                    sql.AppendLine(" ) AS TblErp");
                    sql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod ON TblErp.MG002 = Prod.Model_No COLLATE Chinese_Taiwan_Stroke_BIN");
                    sql.AppendLine(" WHERE (UPPER(ErpOrderPrice_ImportData_DT.ProdID) COLLATE Chinese_Taiwan_Stroke_BIN = UPPER(TblErp.MG003))");
                    sql.AppendLine("  AND (Parent_ID = @ParentID)");

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("ParentID", _parentID);
                    cmd.Parameters.AddWithValue("CustID", _custID);

                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// (Job2檢查寶工品號) Update ERP_ModelNo
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_detailID">單身資料編號</param>
        /// <param name="_dbs">TW/SH</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob2(string _parentID, string _detailID, string _dbs, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" UPDATE ErpOrderPrice_ImportData_DT");

                    //--將有寶工品號的項目設為Y
                    sql.AppendLine(" SET IsPass = 'Y', ERP_ModelNo = RTRIM(Prod.Model_No)");

                    sql.AppendLine(" FROM [ProductCenter].dbo.Prod_Item Prod");
                    sql.AppendLine(" WHERE (UPPER(ErpOrderPrice_ImportData_DT.ProdID) COLLATE Chinese_Taiwan_Stroke_BIN = UPPER(RTRIM(Prod.Model_No)))");

                    //--條件IsPass=N, 已檢查完客戶品號, 而沒有客戶品號的項目
                    sql.AppendLine("  AND (Parent_ID = @ParentID) AND (IsPass = 'N')");

                    ////--@Step3新增品項時使用@
                    //if (!string.IsNullOrWhiteSpace(_detailID))
                    //{
                    //    sql.Append("  AND (Data_ID = @Data_ID)");
                    //    cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    //}

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("ParentID", _parentID);

                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }


        /// <summary>
        /// (Job3設定通過狀態)
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_detailID">單身資料編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob3(string _parentID, string _detailID, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    //將未取得品號的原因寫入(job1~2)
                    sql.AppendLine(" UPDATE ErpOrderPrice_ImportData_DT SET doWhat = ProdID + ',無法取得寶工品號,請確認COPMG' WHERE (Parent_ID = @ParentID) AND (IsPass = 'N');");
                    
                    ////將已通過檢查的項目設為 E(檢查中) --後續無其他檢查
                    //sql.AppendLine(" UPDATE ErpOrderPrice_ImportData_DT SET IsPass = 'E' WHERE (Parent_ID = @ParentID) AND (IsPass = 'Y');");


                    //--@Step3新增品項時使用@
                    //if (!string.IsNullOrWhiteSpace(_detailID))
                    //{
                    //    sql.Append("  AND (Data_ID = @Data_ID)");
                    //    cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    //}

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("ParentID", _parentID);

                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }



        /// <summary>
        /// (Job4取得產品其他欄位) Update StockType
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_detailID">單身資料編號</param>
        /// <param name="_dbs"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob4(string _parentID, string _detailID, string _dbs, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" UPDATE ErpOrderPrice_ImportData_DT");
                    sql.AppendLine(" SET StockType = RTRIM(ErpProd.MB017)");
                    sql.AppendLine(" FROM [{0}].dbo.INVMB AS ErpProd WITH(NOLOCK)".FormatThis(GetDBName(_dbs)));
                    sql.AppendLine(" WHERE ");
                    sql.AppendLine("  RTRIM(ErpProd.MB001) = ErpOrderPrice_ImportData_DT.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN");
                    sql.AppendLine("  AND (Parent_ID = @ParentID)");

                    ////--@Step3新增品項時使用@
                    //if (!string.IsNullOrWhiteSpace(_detailID))
                    //{
                    //    sql.Append("  AND (Data_ID = @Data_ID)");
                    //    cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    //}

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("ParentID", _parentID);

                    return dbConn.ExecuteSql(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }



        private string GetDBName(string dbs)
        {

            //來源DB
            switch (dbs.ToUpper())
            {
                case "SZ":
                    return "ProUnion";

                case "SH":
                    return "SHPK2";

                default:
                    return "prokit2";
            }
        }
        #endregion


        #region -----// Update //-----
        /// <summary>
        /// 更新狀態
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_Status(ImportData instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE ErpOrderPrice_ImportData");
                sql.AppendLine(" SET Status = @Status");
                sql.AppendLine(" , Update_Who = @Update_Who, Update_Time = GETDATE()");

                //匯入完成
                if (instance.Status.Equals(13))
                {
                    sql.Append(", Import_Time = GETDATE()");
                }

                sql.AppendLine(" WHERE (Data_ID = @Data_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Status", instance.Status);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// Log設定
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="errMessage"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_SetLog(string dataID, string errMessage, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE ErpOrderPrice_ImportData");
                sql.AppendLine(" SET ErrMessage = @ErrMessage, ErrTime = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @Data_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", dataID);
                cmd.Parameters.AddWithValue("ErrMessage", errMessage);


                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// Log清除
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ClearLog(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE ErpOrderPrice_ImportData");
                sql.AppendLine(" SET ErrMessage = '', ErrTime = NULL");
                sql.AppendLine(" WHERE (Data_ID = @Data_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", dataID);


                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// 刪除所有資料
        /// </summary>
        /// <param name="dataID">資料編號</param>
        /// <returns></returns>
        public bool Delete(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM ErpOrderPrice_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM ErpOrderPrice_ImportData WHERE (Data_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 刪除單身項目
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool DeleteItem(string parentID, string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM ErpOrderPrice_ImportData_DT WHERE (Parent_ID = @Parent_ID) AND (Data_ID = @Data_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", parentID);
                cmd.Parameters.AddWithValue("Data_ID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }
        #endregion

    }
}
