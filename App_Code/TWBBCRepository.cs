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
using TW_BBC.Models;

/*
 * [TW BBC匯入]
 */
namespace TW_BBC.Controllers
{
    public class TWBBCRepository
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
                    //sqlParamList.Add(new SqlParameter("@Lang", lang.ToUpper()));
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
                                Currency = item.Field<string>("Currency"),
                                Data_Type = item.Field<string>("Data_Type"),
                                Data_TypeName = fn_Status.showTWBBC_DataTypeName(item.Field<string>("Data_Type")),
                                Status = item.Field<Int16>("Status"),
                                StatusName = item.Field<string>("StatusName"),
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
            sql.AppendLine(" , Base.CustID, Base.OrderType, Base.Currency, Base.Data_Type");
            sql.AppendLine(" , (SELECT TOP 1 RTRIM(MA002) FROM [PKSYS].dbo.Customer WITH(NOLOCK) WHERE (MA001 = Base.CustID) AND (DBS = DBC)) AS CustName");
            sql.AppendLine(" , Base.[Status], Cls.Class_Name AS StatusName");
            sql.AppendLine(" , Base.Upload_File, Base.Sheet_Name, Base.Import_Time");
            sql.AppendLine(" , ISNULL(Base.ErrMessage, '') AS ErrMessage, Base.ErrTime");
            sql.AppendLine(" , Base.Create_Who, Base.Create_Time, Base.Update_Who, Base.Update_Time");
            sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
            sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
            sql.AppendLine(" , ROW_NUMBER() OVER(ORDER BY Base.[Status], Base.Create_Time DESC) AS RowIdx");
            sql.AppendLine(" FROM TWBBC_ImportData Base");
            sql.AppendLine("  INNER JOIN TWBBC_RefClass Cls ON Base.Status = Cls.Class_ID");
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

                        case "Status":
                            sql.Append(" AND (Base.Status = @Status)");
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

                        case "Status":
                            sqlParamList.Add(new SqlParameter("@Status", item.Value));
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
                sql.AppendLine(" , DT.ShipFrom, DT.DBName, DT.Company");
                sql.AppendLine(" , DT.ProdID, DT.Cust_ModelNo, DT.ERP_ModelNo, DT.BuyCnt, DT.InputCnt");
                sql.AppendLine(" , ISNULL(DT.BuyPrice, 0) BuyPrice, ISNULL(DT.ERP_Price, 0) ERP_Price");
                sql.AppendLine(" , DT.IsGift, DT.StockType, ISNULL(DT.InnerBox, 0) AS InnerBox, ISNULL(DT.OuterBox, 0) AS OuterBox");
                sql.AppendLine(" , ISNULL(DT.ProdMsg, '') ProdMsg, DT.QuoteDate, DT.LastSalesDay, DT.IsPass, ISNULL(DT.doWhat, '') doWhat");
                sql.AppendLine(" FROM TWBBC_ImportData Base");
                sql.AppendLine("  INNER JOIN TWBBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
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
                                ShipFrom = item.Field<string>("ShipFrom"),  //出貨地
                                DBName = item.Field<string>("DBName"),      //對應資料庫名
                                Company = item.Field<string>("Company"),    //ERP公司別:對應EDI欄位XA001
                                ProdID = item.Field<string>("ProdID"),      //EXCEL中的品號
                                Cust_ModelNo = item.Field<string>("Cust_ModelNo"),  //客戶品號:對應EDI欄位:XA026
                                ERP_ModelNo = item.Field<string>("ERP_ModelNo"),    //品號:對應EDI欄位:XA011
                                BuyCnt = item.Field<int>("BuyCnt"),         //訂單數量
                                InputCnt = item.Field<int>("InputCnt"),     //修改數量(預設為訂單數量):對應EDI欄位:XA012
                                BuyPrice = item.Field<double?>("BuyPrice"), //訂單金額
                                ERP_Price = item.Field<double?>("ERP_Price"),   //ERP價格:對應EDI欄位:XA013
                                IsGift = item.Field<string>("IsGift"),      //贈品(Y/N),Y:XA012=0, XA020=1, XA021=InputCnt
                                StockType = item.Field<string>("StockType"),    //出貨庫別:對應EDI欄位XA014(取INVMB主要庫別MB017)
                                InnerBox = item.Field<int>("InnerBox"),     //內盒產品數量(MB201)
                                OuterBox = item.Field<int>("OuterBox"),     //外箱整箱數量(MB073)
                                ProdMsg = item.Field<string>("ProdMsg"),    //產銷訊息(MB202)
                                QuoteDate = item.Field<string>("QuoteDate"),//核價日
                                LastSalesDay = item.Field<string>("LastSalesDay"),//LastSalesDay
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
        /// 訂單數量:int
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
                int _BuyCnt = 0;
                double _BuyPrice = 0;

                //資料迴圈
                foreach (var val in queryVals)
                {
                    if (!string.IsNullOrWhiteSpace(val[0]))
                    {
                        _ProdID = val[0];
                        _BuyCnt = Convert.ToInt32(val[1]);
                        _BuyPrice = Convert.ToDouble(val[2]);

                        if (!string.IsNullOrWhiteSpace(_ProdID))
                        {
                            //加入項目
                            var data = new ImportDataDT
                            {
                                ProdID = _ProdID,
                                BuyCnt = _BuyCnt,
                                BuyPrice = _BuyPrice
                            };

                            //將項目加入至集合
                            dataList.Add(data);
                        }
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
        /// 取得參考類別
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<RefClass> GetClassList(string _type, Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<RefClass> dataList = new List<RefClass>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Class_ID, Class_Name");
                sql.AppendLine(" FROM TWBBC_RefClass");
                sql.AppendLine(" WHERE (Class_Type = @Class_Type)");

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
                            case "Shown":
                                sql.Append(" AND (Display = @Disp)");
                                cmd.Parameters.AddWithValue("Disp", item.Value);

                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY Sort, Class_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Class_Type", _type);

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
                            var data = new RefClass
                            {
                                ID = item.Field<Int32>("Class_ID"),
                                Label = item.Field<string>("Class_Name")
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
        /// [BBC] 反查ERP訂單
        /// </summary>
        /// <param name="shipfrom"></param>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ERPOrderData> GetERPData_Order(string shipfrom, Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ERPOrderData> dataList = new List<ERPOrderData>();
            StringBuilder sql = new StringBuilder();
            string _dbName = shipfrom.Equals("TW") ? "prokit2" : "SHPK2";  //區分台灣,上海

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("  COPTC.TC001, COPTC.TC002, COPTC.TC003, COPTD.TD004, COPTD.TD005, COPTD.TD007, ROUND(COPTD.TD008, 0) TD008");
                sql.AppendLine(" FROM [{0}].dbo.COPTC WITH(NOLOCK) INNER JOIN [{0}].dbo.COPTD WITH(NOLOCK)".FormatThis(_dbName));
                sql.AppendLine("  ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTC.TC201 = 'TW-BBC')");

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
        /// <param name="shipfrom"></param>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ERPOrderData> GetERPData_Sales(string shipfrom, Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<ERPOrderData> dataList = new List<ERPOrderData>();
            StringBuilder sql = new StringBuilder();
            string _dbName = shipfrom.Equals("TW") ? "prokit2" : "SHPK2";  //區分台灣,上海

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
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTC.TC201 = 'TW-BBC')");

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


        #endregion


        #region -----// Create //-----
        /// <summary>
        /// [Step1] 建立BBC基本資料
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
                sql.AppendLine(" INSERT INTO TWBBC_ImportData (");
                sql.AppendLine("  Data_ID, TraceID, CustID, OrderType");
                sql.AppendLine("  , Currency, Data_Type, Status, Upload_File");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @TraceID, @CustID, @OrderType");
                sql.AppendLine("  , @Currency, @Data_Type, @Status, @Upload_File");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("OrderType", instance.OrderType);
                cmd.Parameters.AddWithValue("Currency", instance.Currency);
                cmd.Parameters.AddWithValue("Data_Type", instance.Data_Type);
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
                sql.AppendLine(" DELETE FROM TWBBC_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE TWBBC_ImportData SET Status = 11, Sheet_Name = @Sheet_Name, Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                foreach (var item in query)
                {
                    if (!string.IsNullOrWhiteSpace(item.ProdID))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                        sql.AppendLine("  FROM TWBBC_ImportData_DT");
                        sql.AppendLine("  WHERE (Parent_ID = @DataID)");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO TWBBC_ImportData_DT( ");
                        sql.AppendLine("  Parent_ID, Data_ID");
                        sql.AppendLine("  , ShipFrom, ProdID, BuyCnt, InputCnt, BuyPrice");
                        sql.AppendLine(" ) VALUES (");
                        sql.AppendLine("  @DataID, @NewID, @ShipFrom, '{0}', {1}, {1}, {2}".FormatThis(
                            item.ProdID, item.BuyCnt, item.BuyPrice));
                        sql.AppendLine(" );");
                    }
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("ShipFrom", baseData.Data_Type);
                cmd.Parameters.AddWithValue("Sheet_Name", baseData.Sheet_Name);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [Step3] Insert單身資料, 單筆新增
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="detail"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public Int32 CreateDetailItem(ImportData baseData, ImportDataDT detail, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE TWBBC_ImportData SET Status = 12, Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT");
                sql.AppendLine(" SET @NewID = 0"); //default value

                if (!string.IsNullOrWhiteSpace(detail.ProdID))
                {
                    sql.AppendLine(" SET @NewID = (");
                    sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                    sql.AppendLine("  FROM TWBBC_ImportData_DT");
                    sql.AppendLine("  WHERE (Parent_ID = @DataID)");
                    sql.AppendLine(" )");

                    sql.AppendLine(" INSERT INTO TWBBC_ImportData_DT( ");
                    sql.AppendLine("  Parent_ID, Data_ID");
                    sql.AppendLine("  , ShipFrom, ProdID, BuyCnt, InputCnt");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @DataID, @NewID, @ShipFrom, '{0}', {1}, {1}".FormatThis(
                        detail.ProdID, detail.BuyCnt));
                    sql.AppendLine(" );");
                }

                sql.AppendLine(" SELECT @NewID AS myDataID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("ShipFrom", baseData.Data_Type);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    return Convert.ToInt32(DT.Rows[0]["myDataID"]);
                }
            }

        }


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
                sql.AppendLine("  DT.Company AS XA001");
                sql.AppendLine("  , '2' AS XA002  /*固定為訂單(2)*/");
                sql.AppendLine("  , Base.OrderType AS XA003");
                sql.AppendLine("  , Base.TraceID AS XA006");
                sql.AppendLine("  , Base.CustID AS XA007");
                sql.AppendLine("  , Cust.MA016 AS XA008");
                sql.AppendLine("  , Base.Currency AS XA009");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA010");
                sql.AppendLine("  , DT.ERP_ModelNo AS XA011");
                sql.AppendLine("  , DT.InputCnt AS XA012");
                sql.AppendLine("  , DT.ERP_Price AS XA013");
                sql.AppendLine("  , DT.StockType AS XA014");
                sql.AppendLine("  , Cust.MA027 AS XA015");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA016");
                sql.AppendLine("  , CONVERT(CHAR(10),GETDATE(),112) AS XA017");
                sql.AppendLine("  , '1' AS XA020");
                sql.AppendLine("  , (CASE WHEN DT.IsGift = 'Y' THEN DT.InputCnt ELSE 0 END) AS XA021");
                sql.AppendLine("  , RTRIM(Cust.MA004) AS XA022");
                sql.AppendLine("  , Cust.MA006 AS XA023");
                sql.AppendLine("  , 'TW-BBC' AS XA024");
                sql.AppendLine("  , Base.TraceID AS XA025");
                sql.AppendLine("  , DT.ProdID AS XA026");
                sql.AppendLine("  , '2' AS XA027");
                sql.AppendLine("  , '1' AS XA028");
                sql.AppendLine("  , '' AS XA031"); //與API對應,不可省略
                sql.AppendLine("  , '' AS XA032"); //與API對應,不可省略
                sql.AppendLine("  , '' AS XA033"); //與API對應,不可省略
                sql.AppendLine("  , RIGHT(('000' + CAST(DT.Data_ID AS VARCHAR(4))), 4) AS XA034"); //自訂序號
                sql.AppendLine(" FROM TWBBC_ImportData Base");
                sql.AppendLine("  INNER JOIN TWBBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN [PKSYS].dbo.Customer Cust ON Base.CustID = RTRIM(Cust.MA001) AND DBS=DBC");
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


        #region -----// BBC Check Jobs //-----

        /// <summary>
        /// (Job1檢查客戶品號) Update Cust_ModelNo, ERP_ModelNo, ShipFrom
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_custID">客戶編號</param>
        /// <param name="_dataType">匯入類型</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob1(string _parentID, string _custID, string _dataType, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT");
                    sql.AppendLine(" SET IsPass = 'Y', Cust_ModelNo = RTRIM(TblErp.MG003), ERP_ModelNo = RTRIM(TblErp.MG002)");

                    //--@若Data_Type=Prod, 加入此行(以程式判斷)@
                    if (_dataType.ToUpper().Equals("PROD"))
                        sql.AppendLine(" , ShipFrom = RTRIM(Prod.Ship_From)");

                    sql.AppendLine(" FROM (");
                    sql.AppendLine("     SELECT TblGrp.Ship, TblGrp.MG002, TblGrp.MG003");
                    sql.AppendLine("     FROM (");
                    sql.AppendLine("         SELECT 'TW' AS Ship, ERPData_TW.MG002, ERPData_TW.MG003");
                    sql.AppendLine("         FROM [prokit2].dbo.COPMG ERPData_TW WITH(NOLOCK)");
                    sql.AppendLine("         WHERE (ERPData_TW.MG001 = @CustID)");
                    sql.AppendLine("         UNION ALL");
                    sql.AppendLine("         SELECT 'SH' AS Ship, ERPData_SH.MG002, ERPData_SH.MG003");
                    sql.AppendLine("         FROM [SHPK2].dbo.COPMG ERPData_SH WITH(NOLOCK)");
                    sql.AppendLine("         WHERE (ERPData_SH.MG001 = @CustID)");
                    sql.AppendLine("     ) AS TblGrp");
                    sql.AppendLine("     GROUP BY TblGrp.Ship, TblGrp.MG002, TblGrp.MG003");
                    sql.AppendLine(" ) AS TblErp");
                    sql.AppendLine("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod ON TblErp.MG002 = Prod.Model_No COLLATE Chinese_Taiwan_Stroke_BIN AND TblErp.Ship = Prod.Ship_From");
                    sql.AppendLine(" WHERE (UPPER(TWBBC_ImportData_DT.ProdID) COLLATE Chinese_Taiwan_Stroke_BIN = UPPER(TblErp.MG003))");
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
        /// (Job2檢查寶工品號) Update ERP_ModelNo, ShipFrom
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_dataType">匯入類型</param>
        /// <param name="_detailID">單身資料編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob2(string _parentID, string _dataType, string _detailID, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT");

                    //--將有寶工品號的項目設為Y
                    sql.AppendLine(" SET IsPass = 'Y', ERP_ModelNo = RTRIM(Prod.Model_No)");

                    //--@若Data_Type=Prod, 加入此行(以程式判斷)@
                    if (_dataType.ToUpper().Equals("PROD"))
                        sql.AppendLine(" , ShipFrom = RTRIM(Prod.Ship_From)");

                    sql.AppendLine(" FROM [ProductCenter].dbo.Prod_Item Prod");
                    sql.AppendLine(" WHERE (UPPER(TWBBC_ImportData_DT.ProdID) COLLATE Chinese_Taiwan_Stroke_BIN = UPPER(RTRIM(Prod.Model_No)))");

                    //--條件IsPass=N, 已檢查完客戶品號, 而沒有客戶品號的項目為N
                    sql.AppendLine("  AND (Parent_ID = @ParentID) AND (IsPass = 'N') AND (Prod.Ship_From IN ('TW','SH'))");

                    //--@Step3新增品項時使用@
                    if (!string.IsNullOrWhiteSpace(_detailID))
                    {
                        sql.Append("  AND (Data_ID = @Data_ID)");
                        cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    }

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
        /// (Job3取得公司別/資料庫名) Update DBName, Company
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
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT SET doWhat = ProdID + ',無法取得寶工品號,請確認COPMG' WHERE (Parent_ID = @ParentID) AND (IsPass = 'N');");
                    //將已通過檢查的項目設為 E(檢查中)
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT SET IsPass = 'E' WHERE (Parent_ID = @ParentID) AND (IsPass = 'Y');");

                    //執行更新:DBName, Company
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT");
                    sql.AppendLine(" SET DBName = Corp.[DB_Name], Company = Flow.Company");
                    sql.AppendLine(" FROM [PKSYS].dbo.Param_Corp Corp");
                    sql.AppendLine("  INNER JOIN [PKSYS].dbo.Param_ERPFlow Flow ON Corp.Corp_ID = Flow.Company");
                    sql.AppendLine(" WHERE (Flow.FlowName = TWBBC_ImportData_DT.ShipFrom)");
                    sql.AppendLine("  AND (Parent_ID = @ParentID)");

                    //--@Step3新增品項時使用@
                    if (!string.IsNullOrWhiteSpace(_detailID))
                    {
                        sql.Append("  AND (Data_ID = @Data_ID)");
                        cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    }

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
        /// (Job4取得產品其他欄位) Update InnerBox, OuterBox, ProdMsg, StockType
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_detailID">單身資料編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob4(string _parentID, string _detailID, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" ;WITH TblProd AS (");
                    sql.AppendLine("     SELECT 'TW' AS DBS, RTRIM(MB001) AS ModelNo");
                    sql.AppendLine("     , RTRIM(MB017) AS StockType, MB201 AS InnerBox, MB073 AS OuterBox, MB202 AS ProdMsg");
                    sql.AppendLine("     FROM [prokit2].dbo.INVMB WITH(NOLOCK)");
                    sql.AppendLine("     UNION ALL");
                    sql.AppendLine("     SELECT 'SH' AS DBS, RTRIM(MB001) AS ModelNo");
                    sql.AppendLine("     , RTRIM(MB017) AS StockType, MB201 AS InnerBox, MB073 AS OuterBox, MB202 AS ProdMsg");
                    sql.AppendLine("     FROM [SHPK2].dbo.INVMB WITH(NOLOCK)");
                    sql.AppendLine(" )");
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT");
                    sql.AppendLine(" SET InnerBox = TblProd.InnerBox, OuterBox = TblProd.OuterBox, ProdMsg = TblProd.ProdMsg, StockType = TblProd.StockType");
                    sql.AppendLine(" FROM TblProd");
                    sql.AppendLine(" WHERE UPPER(TblProd.DBS) = UPPER(TWBBC_ImportData_DT.ShipFrom)");
                    sql.AppendLine("  AND TblProd.ModelNo = TWBBC_ImportData_DT.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN");
                    sql.AppendLine("  AND (Parent_ID = @ParentID)");

                    //--@Step3新增品項時使用@
                    if (!string.IsNullOrWhiteSpace(_detailID))
                    {
                        sql.Append("  AND (Data_ID = @Data_ID)");
                        cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    }

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
        /// (Job5取得價格) - 使用程式批次修改
        /// LinQ取得DBS, 迴圈執行取價, 取得後批次更新回單身
        /// </summary>
        /// <param name="_parentID"></param>
        /// <param name="_custID"></param>
        /// <param name="_detailID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckJob5(string _parentID, string _custID, string _detailID, out string ErrMsg)
        {
            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法[取得單身資料](條件IsPass=E=檢查中) -----
                    sql.AppendLine(" SELECT DT.DBName, DT.ERP_ModelNo, DT.InputCnt, Base.Currency");
                    sql.AppendLine(" FROM TWBBC_ImportData Base");
                    sql.AppendLine("  INNER JOIN TWBBC_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");
                    sql.AppendLine(" WHERE (DT.Parent_ID = @ParentID) AND (DT.IsPass = 'E')");

                    //--@Step3新增品項時使用@
                    if (!string.IsNullOrWhiteSpace(_detailID))
                    {
                        sql.Append("  AND (DT.Data_ID = @Data_ID)");
                        cmd.Parameters.AddWithValue("Data_ID", _detailID);
                    }

                    sql.AppendLine(" ORDER BY DT.DBName, DT.ERP_ModelNo");

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("ParentID", _parentID);

                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        /*
                         * 依不同DBS分次執行取價
                         * 取完價格後再UPDATE回單身
                         */
                        if (DT.Rows.Count == 0)
                        {
                            ErrMsg = "查無單身資料(Job5)";
                            return false;
                        }

                        //All Data
                        var _data = DT.AsEnumerable()
                            .Select(el => new
                            {
                                DBS = el.Field<string>("DBName"),
                                ItemNo = el.Field<string>("ERP_ModelNo"),
                                Qty = el.Field<Int32>("InputCnt"),
                                Currency = el.Field<string>("Currency")
                            });

                        //DBS Group
                        var query_GP = DT.AsEnumerable()
                            .GroupBy(gp => new
                            {
                                DBS = gp.Field<string>("DBName")
                            })
                            .Select(el => new
                            {
                                ID = el.Key.DBS
                            });

                        //Group Loop(依DBS跑迴圈)
                        foreach (var gp in query_GP)
                        {
                            List<ImportDataDT> dataList = new List<ImportDataDT>();
                            var dataItem = new ImportDataDT();

                            //------- Group Start -------

                            #region -- DBS Group Loop ---

                            string _dbs = gp.ID;
                            ArrayList aryModelNo = new ArrayList();
                            ArrayList aryQty = new ArrayList();

                            //取得各自的DBS
                            var filterData = _data.Where(el => el.DBS.Equals(gp.ID));
                            foreach (var item in filterData)
                            {
                                //加入暫存
                                aryModelNo.Add(item.ItemNo);
                                aryQty.Add(item.Qty);
                            }

                            //幣別
                            string curr = filterData.FirstOrDefault().Currency;

                            //取價(SP)
                            DataTable getPrice = GetQuotePrice(_dbs, _custID, aryModelNo, aryQty, curr, out ErrMsg);
                            if (getPrice == null)
                            {
                                //無報價資料
                                ErrMsg += ";SP取價失敗(" + string.Join(", ", aryModelNo.OfType<string>()) + ")";
                                return false;
                            }

                            //價格資料取出
                            for (int row = 0; row < getPrice.Rows.Count; row++)
                            {
                                dataItem = new ImportDataDT
                                {
                                    DBName = getPrice.Rows[row]["DBS"].ToString(),
                                    ERP_ModelNo = getPrice.Rows[row]["ModelNo"].ToString(),
                                    ERP_Price = getPrice.Rows[row]["SpQtyPrice"] == null ? 0 : Convert.ToDouble(getPrice.Rows[row]["SpQtyPrice"]),
                                    QuoteDate = getPrice.Rows[row]["QuoteDate"].ToString(),
                                    LastSalesDay = getPrice.Rows[row]["LastSalesDay"].ToString()
                                };


                                //將項目加入至集合
                                dataList.Add(dataItem);
                            }

                            //----- 更新單身ERP價格 -----
                            if (!Update_QuotePrice(_parentID, dataList.AsQueryable(), out ErrMsg))
                            {
                                return false;
                            }

                            #endregion

                            //------- Group End -------
                        }

                        //finish
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }


        /// <summary>
        /// 預存程序:取得價格(TWBBC_GetQuotePrice)
        /// Job5
        /// </summary>
        /// <param name="_DBS"></param>
        /// <param name="_CustID">客戶代號</param>
        /// <param name="aryModelNo">品號(多筆)</param>
        /// <param name="aryQty">數量(多筆)</param>
        /// <param name="_currency">幣別</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetQuotePrice(string _DBS, string _CustID, ArrayList aryModelNo, ArrayList aryQty
            , string _currency
            , out string ErrMsg)
        {
            try
            {
                //[判斷參數] - 判斷是否有傳入值
                if (string.IsNullOrWhiteSpace(_DBS) || string.IsNullOrWhiteSpace(_CustID))
                {
                    ErrMsg = "「ERP客戶編號」或「客戶DBS」未傳入資料";
                    return null;
                }
                //[判斷參數] - 判斷傳入值是否全部為空
                if (aryModelNo.Count == 0)
                {
                    ErrMsg = "品號資料空白";
                    return null;
                }

                //Array轉換字串
                string strModelNo = (aryModelNo.Count > 0) ? string.Join(",", aryModelNo.ToArray()) : "";
                string strQty = (aryQty.Count > 0) ? string.Join(",", aryQty.ToArray()) : "";

                //查詢StoreProcedure (TWBBC_GetQuotePrice)
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TWBBC_GetQuotePrice";
                    cmd.Parameters.AddWithValue("DBS", _DBS);
                    cmd.Parameters.AddWithValue("CustIDs", _CustID);
                    cmd.Parameters.AddWithValue("Currency", _currency);
                    cmd.Parameters.AddWithValue("ModelNos", string.IsNullOrEmpty(strModelNo) ? DBNull.Value : (Object)strModelNo);
                    cmd.Parameters.AddWithValue("Qty", string.IsNullOrEmpty(strQty.ToString()) ? DBNull.Value : (Object)strQty);
                    //取得回傳值, 輸出參數
                    SqlParameter Msg = cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 200);
                    Msg.Direction = ParameterDirection.Output;

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            ErrMsg += "(資料回傳筆數為 0)";
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
        /// 更新價格 (區分不同資料庫)
        /// Job5
        /// </summary>
        /// <param name="_parentID"></param>
        /// <param name="_datalist"></param>
        /// <returns></returns>
        private bool Update_QuotePrice(string _parentID, IQueryable<ImportDataDT> _datalist, out string ErrMsg)
        {
            //Check Null
            if (_datalist.Count() == 0)
            {
                ErrMsg = "取價商品無法取得.請確認單身資料是否遺失.";
                return false;
            }

            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                int row = 0;
                string dbname = "";
                foreach (var item in _datalist)
                {
                    /*
                     * 取價成功的項目
                     */
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT");
                    sql.AppendLine(" SET ERP_Price = {0}, QuoteDate = '{1}', LastSalesDay = '{2}', IsPass = 'Y'".FormatThis(
                        item.ERP_Price
                        , item.QuoteDate
                        , item.LastSalesDay
                        ));
                    sql.AppendLine(" WHERE (Parent_ID = @Parent_ID) AND (UPPER(DBName) = UPPER(@DBName)) AND (IsPass = 'E')");
                    sql.AppendLine("  AND (ERP_ModelNo = N'{0}');".FormatThis(item.ERP_ModelNo));

                    row++;
                    dbname = item.DBName;
                }

                /*
                 * 符合以下條件,將品項設為不通過&查無價格
                 * xxERP_Price = 0xx, IsGift = 'N', IsPass = 'E'
                 */
                sql.AppendLine(" UPDATE TWBBC_ImportData_DT SET IsPass = 'N', doWhat = N'查無ERP價格, 請檢查ERP客戶商品計價.'");
                sql.AppendLine(" WHERE (Parent_ID = @Parent_ID) AND (UPPER(DBName) = UPPER(@DBName)) AND (IsPass = 'E') AND (IsGift = 'N');");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", _parentID);
                cmd.Parameters.AddWithValue("DBName", dbname);


                return dbConn.ExecuteSql(cmd, out ErrMsg);
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
                sql.AppendLine(" UPDATE TWBBC_ImportData");
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
        /// [Step3] 更新單身資料
        /// [欄位] 數量, 贈品
        /// </summary>
        /// <param name="_parentID"></param>
        /// <param name="_datalist"></param>
        /// <returns></returns>
        public bool Update_Items(string _parentID, IQueryable<ImportDataDT> _datalist, out string ErrMsg)
        {
            //Check Null
            if (_datalist.Count() == 0)
            {
                ErrMsg = "請確認單身資料是否遺失.";
                return false;
            }

            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                int row = 0;
                foreach (var item in _datalist)
                {
                    sql.AppendLine(" UPDATE TWBBC_ImportData_DT");
                    sql.AppendLine(" SET InputCnt = @InputCnt_{0}, IsGift = @IsGift_{0}".FormatThis(row));
                    sql.AppendLine(" WHERE (Parent_ID = @Parent_ID) AND (Data_ID = @Data_ID_{0})".FormatThis(row));

                    cmd.Parameters.AddWithValue("Data_ID_" + row, item.Data_ID);
                    cmd.Parameters.AddWithValue("InputCnt_" + row, item.InputCnt);
                    cmd.Parameters.AddWithValue("IsGift_" + row, item.IsGift);

                    row++;
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Parent_ID", _parentID);


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
                sql.AppendLine(" UPDATE TWBBC_ImportData");
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
                sql.AppendLine(" UPDATE TWBBC_ImportData");
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
                sql.AppendLine(" DELETE FROM TWBBC_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM TWBBC_ImportData WHERE (Data_ID = @DataID);");

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
                sql.AppendLine(" DELETE FROM TWBBC_ImportData_DT WHERE (Parent_ID = @Parent_ID) AND (Data_ID = @Data_ID)");

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