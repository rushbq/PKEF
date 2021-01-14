using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using PKLib_Method.Methods;
using SZ_Invoice.Aisino.Models;

/*
 * 深圳開票平台:
 * 一般紙本發票, 電商紙本發票
 *  - 發票轉入
 */
namespace SZ_Invoice.Aisino.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        CustID = 3,
        StartDate = 4,
        EndDate = 5,
        Type = 6,
        DataType = 7
    }


    public class SZ_InvoiceRepository
    {
        public string ErrMsg;

        #region -----// Read //-----


        #region >> 一般紙本發票 <<
        /// <summary>
        /// [一般紙本發票] 取得基本資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<DT_Base> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// [一般紙本發票] 取得基本資料 -- Step2(基本資料) / List / View
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<DT_Base> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<DT_Base> dataList = new List<DT_Base>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.Data_ID");
                sql.AppendLine("   , Base.CustID, Base.Inv_UID, Base.Inv_Subject, Base.Inv_NO, Base.Inv_Date, Base.DataType");
                sql.AppendLine("   , Base.IsInsert, Base.IsUpdate");
                sql.AppendLine("   , Base.Create_Time, Base.Update_Time");
                sql.AppendLine("   , (SELECT TOP 1 RTRIM(MA002) FROM PKSYS.dbo.Customer WITH(NOLOCK) WHERE (MA001 = Base.CustID) AND (DBS = DBC)) AS CustName");
                sql.AppendLine("   , ISNULL((SELECT TOP 1 RTRIM(InvType) FROM PKSYS.dbo.Customer_Data WITH(NOLOCK) WHERE (Cust_ERPID = Base.CustID)), 'x') AS InvType"); //一般紙本INVTYPE
                sql.AppendLine("   , ISNULL((SELECT TOP 1 (CASE InvType WHEN '2' THEN '0' ELSE '2' END) AS InvType FROM PKSYS.dbo.SZ_Invoice_E6_BBCData WITH(NOLOCK) WHERE (Parent_ID = Base.Data_ID)), '0') AS E_InvType"); //電商紙本INVTYPE(0—专用发票/ 2—普通发票)
                sql.AppendLine("   , (SELECT TOP 1 ISNULL(vendeename, '') AS Buyer FROM SZ_Invoice_E6_BBCData WITH(NOLOCK) WHERE (Parent_ID = Base.Data_ID)) AS BuyerName");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name");
                //發票系統已回寫,可UPDATE發票資料
                sql.AppendLine("   , (SELECT COUNT(*) FROM HTTAX_FEEDBACKHEADR InvRel WHERE (InvRel.unino = Base.Inv_UID)) AS IsRel");
                sql.AppendLine("   , (SELECT vendeename FROM HTTAX_SALEBILLVOUCH WHERE (unino = Base.Inv_UID)) AS vendeename");
                sql.AppendLine(" FROM SZ_Invoice_E6 Base");
                sql.AppendLine(" WHERE (1 = 1) ");

                #region --Search--

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


                            case (int)mySearch.DataType:
                                //一般紙本或電商紙本
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.DataType = @DataType)");

                                    cmd.Parameters.AddWithValue("DataType", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Base.Inv_UID)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Base.Inv_Subject)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Base.Inv_NO)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)mySearch.Type:
                                /*
                                 * 狀態條件
                                 * A:未完成轉入, IsInsert = N
                                 * B:待產生發票, IsInsert = Y,  IsUpdate = N, IsRel = 0 
                                 * C:發票已產生,待更新, IsInsert = Y,  IsUpdate = N, IsRel > 0 
                                 * D:已完成 ,IsInsert = Y, Get_IsUpdate = Y
                                 */
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    switch (item.Value)
                                    {
                                        case "A":
                                            sql.Append(" AND (Base.IsInsert = 'N')");
                                            break;

                                        case "B":
                                            sql.Append(" AND (Base.IsInsert = 'Y') AND (Base.IsUpdate = 'N')");
                                            sql.Append(" AND ((SELECT COUNT(*) FROM HTTAX_FEEDBACKHEADR InvRel WHERE (InvRel.unino = Base.Inv_UID)) = 0)");
                                            break;

                                        case "C":
                                            sql.Append(" AND (Base.IsInsert = 'Y') AND (Base.IsUpdate = 'N')");
                                            sql.Append(" AND ((SELECT COUNT(*) FROM HTTAX_FEEDBACKHEADR InvRel WHERE (InvRel.unino = Base.Inv_UID)) > 0)");
                                            break;

                                        case "D":
                                            sql.Append(" AND (Base.IsInsert = 'Y') AND (Base.IsUpdate = 'Y')");
                                            break;
                                    }

                                }

                                break;

                        }
                    }
                }

                #endregion

                sql.AppendLine(") AS Tbl ");
                sql.AppendLine(" ORDER BY Tbl.IsUpdate ASC, Tbl.IsInsert ASC, Tbl.Inv_UID DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new DT_Base
                            {
                                Data_ID = item.Field<Guid>("Data_ID"),
                                CustID = item.Field<string>("CustID"),
                                CustName = item.Field<string>("CustName"),
                                BuyerName = item.Field<string>("BuyerName"),
                                InvType = item.Field<Int16>("DataType").Equals(1) ? item.Field<string>("InvType") : item.Field<string>("E_InvType"), //發票類型
                                Inv_UID = item.Field<string>("Inv_UID"),
                                Inv_Subject = item.Field<string>("Inv_Subject"),
                                Inv_NO = item.Field<string>("Inv_NO"),
                                Inv_Date = item.Field<DateTime?>("Inv_Date").ToString(),
                                IsInsert = item.Field<string>("IsInsert"),
                                IsUpdate = item.Field<string>("IsUpdate"),
                                IsRel = item.Field<int>("IsRel"),
                                vendeename = item.Field<string>("vendeename"),
                                DataType = item.Field<Int16>("DataType"), //1:一般/2:電商

                                Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                                Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                                Create_Name = item.Field<string>("Create_Name"),
                                Update_Name = item.Field<string>("Update_Name")
                            };

                            //將項目加入至集合
                            dataList.Add(data);

                        }
                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [一般紙本發票] 取得未開票的資料 -- Step1 (Check)
        /// </summary>
        /// <param name="custID">客戶編號(可為空)</param>
        /// <param name="startDate">開始日(yyyyMMdd)</param>
        /// <param name="endDate">結束日(yyyyMMdd)</param>
        /// <returns></returns>
        public IQueryable<DT_Check> CheckInvoiceData(string custID, string startDate, string endDate)
        {
            //----- 宣告 -----
            List<DT_Check> dataList = new List<DT_Check>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(";WITH TblBase AS ( ");
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  ISNULL(RTRIM(COPTG.TG001) + '-' + RTRIM(COPTG.TG002), RTRIM(COPTJ.TJ001) + '-' + RTRIM(COPTJ.TJ002)) AS Erp_SO_ID ");
                sql.AppendLine("  , RTRIM(ACRTA.TA001) + '-' + RTRIM(ACRTA.TA002) AS Erp_AR_ID ");
                sql.AppendLine("  , RTRIM(COPMA.MA001) AS CustID ");
                sql.AppendLine("  , RTRIM(COPMA.MA003) AS CustName ");
                sql.AppendLine(" FROM [ProUnion].dbo.ACRTA WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = ACRTA.TA004 ");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 ");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTJ WITH(NOLOCK) ON ACRTA.TA001 = COPTJ.TJ025 AND ACRTA.TA002 = COPTJ.TJ026 ");
                sql.AppendLine(" WHERE (ACRTA.TA036 = '') ");

                //CustID
                if (!string.IsNullOrEmpty(custID))
                {
                    sql.AppendLine("  AND (ACRTA.TA004 = @CustID) ");
                    cmd.Parameters.AddWithValue("CustID", custID);
                }

                //結帳日
                sql.AppendLine("  AND (ACRTA.TA003 >= @sDate) AND (ACRTA.TA003 <= @eDate) ");

                sql.AppendLine(")");
                sql.AppendLine(" SELECT TblBase.*");
                sql.AppendLine(" FROM TblBase");
                //檢查重複:還未更新至ERP的資料
                sql.AppendLine(" WHERE TblBase.Erp_SO_ID NOT IN (");
                sql.AppendLine("     SELECT DT.Erp_SO_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine("     FROM SZ_Invoice_E6 Base");
                sql.AppendLine("      INNER JOIN SZ_Invoice_E6_Items DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("     WHERE (Base.IsUpdate = 'N')");
                sql.AppendLine(" )");
                sql.AppendLine(" ORDER BY TblBase.CustID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("sDate", startDate);
                cmd.Parameters.AddWithValue("eDate", endDate);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new DT_Check
                        {
                            Erp_SO_ID = item.Field<string>("Erp_SO_ID"),    //銷貨單/銷退單
                            Erp_AR_ID = item.Field<string>("Erp_AR_ID"),    //結帳單
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName")
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
        /// [一般紙本發票][批次新增]-取得未開票的資料 -- BatchStep1 (Check)
        /// </summary>
        /// <param name="startDate">開始日(yyyyMMdd)</param>
        /// <param name="endDate">結束日(yyyyMMdd)</param>
        /// <returns></returns>
        public IQueryable<DT_Check> CheckInvoiceDataGroup(string startDate, string endDate)
        {
            //----- 宣告 -----
            List<DT_Check> dataList = new List<DT_Check>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(";WITH TblBase AS ( ");
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  RTRIM(COPMA.MA001) AS CustID ");
                sql.AppendLine("  , RTRIM(COPMA.MA003) AS CustName ");
                sql.AppendLine("  , ISNULL(SUM(COPTG.TG045 + COPTG.TG046), SUM(COPTJ.TJ012)) AS TotalPrice");
                sql.AppendLine(" FROM [ProUnion].dbo.ACRTA WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = ACRTA.TA004 ");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 ");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTJ WITH(NOLOCK) ON ACRTA.TA001 = COPTJ.TJ025 AND ACRTA.TA002 = COPTJ.TJ026 ");
                sql.AppendLine(" WHERE (ACRTA.TA036 = '') ");
                //結帳日
                sql.AppendLine("  AND (ACRTA.TA003 >= @sDate) AND (ACRTA.TA003 <= @eDate) ");
                //檢查重複:還未更新至ERP的資料
                sql.AppendLine("  AND ISNULL(RTRIM(COPTG.TG001) + '-' + RTRIM(COPTG.TG002), RTRIM(COPTJ.TJ001) + '-' + RTRIM(COPTJ.TJ002)) NOT IN (");
                sql.AppendLine("     SELECT DT.Erp_SO_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine("     FROM SZ_Invoice_E6 Base");
                sql.AppendLine("      INNER JOIN SZ_Invoice_E6_Items DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("     WHERE (Base.IsUpdate = 'N')");
                sql.AppendLine(" )");
                sql.AppendLine(" GROUP BY COPMA.MA001, COPMA.MA003");
                sql.AppendLine(")");
                sql.AppendLine(" SELECT TblBase.*");
                sql.AppendLine(" FROM TblBase");
                sql.AppendLine(" ORDER BY TblBase.CustID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("sDate", startDate);
                cmd.Parameters.AddWithValue("eDate", endDate);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new DT_Check
                        {
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            TotalPrice = Convert.ToDouble(item.Field<decimal>("TotalPrice"))
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
        /// 取得單身資料 - 判斷是否已匯入，回傳不同來源資料
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="rowID"></param>
        /// <param name="act">Steps/View</param>
        /// <returns></returns>
        public IQueryable<DT_Lines> GetDataLines(string dataID, string act)
        {
            if (act.ToUpper().Equals("STEPS"))
            {
                //Steps
                return GetImportLines(dataID);
            }
            else
            {
                //View
                return GetImportedLines(dataID);
            }
        }

        /// <summary>
        /// 顯示要匯入的單身資料 -- Step2
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public IQueryable<DT_Lines> GetImportLines(string dataID)
        {

            //回傳集合
            return ResetDTLines(dataID).AsQueryable();
        }


        /// <summary>
        /// 顯示已匯入的單身資料 -- View
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        public IQueryable<DT_Lines> GetImportedLines(string dataID)
        {
            //----- 宣告 -----
            List<DT_Lines> dataList = new List<DT_Lines>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(" SELECT Base.Inv_UID AS unino");
                sql.AppendLine("  , lines.serial");
                sql.AppendLine("  , lines.qty");
                sql.AppendLine("  , lines.price	--//單價");
                sql.AppendLine("  , lines.shpamt	--//金額");
                sql.AppendLine("  , lines.taxrate");
                sql.AppendLine("  , lines.taxation	--//稅額");
                sql.AppendLine("  , ISNULL(lines.disamt, 0) disamt	--//折扣金額");
                sql.AppendLine("  , lines.tradename	--//品名");
                sql.AppendLine("  , lines.model	--//品號");
                sql.AppendLine("  , lines.unit");
                sql.AppendLine("  , lines.taxprice  --//含稅(Y/N)");
                sql.AppendLine("  , lines.bmbbh  --//編碥版本");
                sql.AppendLine("  , lines.ssflbm   --//稅收分類編碼");
                sql.AppendLine("  , lines.xsyhzc  --//享受优惠政策");
                sql.AppendLine(" FROM SZ_Invoice_E6 Base");
                sql.AppendLine("  INNER JOIN HTTAX_SALEBILLVOUCHS lines ON Base.Inv_UID = lines.unino");
                sql.AppendLine(" WHERE (Base.Data_ID = @id)");
                sql.AppendLine(" ORDER BY lines.serial");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("id", dataID);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new DT_Lines
                            {
                                serial = item.Field<Int16>("serial"),
                                qty = item.Field<double>("qty"),
                                price = item.Field<double>("price"),
                                shpamt = Convert.ToDouble(item.Field<decimal>("shpamt")),
                                taxation = Convert.ToDouble(item.Field<decimal>("taxation")),
                                disamt = Convert.ToDouble(item.Field<decimal>("disamt")),
                                tradename = item.Field<string>("tradename"),
                                model = item.Field<string>("model"),
                                unit = item.Field<string>("unit")
                            };

                            //將項目加入至集合
                            dataList.Add(data);

                        }
                    }

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// [一般紙本發票][手動填發票] 取得ERP未開票資料
        /// </summary>
        /// <param name="custID"></param>
        /// <param name="startDate">開始日(yyyyMMdd)</param>
        /// <param name="endDate">結束日(yyyyMMdd)</param>
        /// <returns></returns>
        public IQueryable<ERP_Invoice> GetErpUnBilledData(string custID, string startDate, string endDate)
        {
            //----- 宣告 -----
            List<ERP_Invoice> dataList = new List<ERP_Invoice>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(";WITH TblBase AS ( ");
                sql.AppendLine(" SELECT ");
                sql.AppendLine("  RTRIM(COPMA.MA001) AS CustID");
                sql.AppendLine("  , RTRIM(COPMA.MA003) AS CustName");
                sql.AppendLine("  , ISNULL(RTRIM(COPTG.TG001) + '-' + RTRIM(COPTG.TG002), RTRIM(COPTJ.TJ001) + '-' + RTRIM(COPTJ.TJ002)) AS Erp_SO_ID");
                sql.AppendLine("  , RTRIM(ACRTA.TA001) + '-' + RTRIM(ACRTA.TA002) AS Erp_AR_ID");
                sql.AppendLine("  , ACRTA.TA003 AS ArDate, ACRTA.TA036 AS InvNo");
                sql.AppendLine("  , ROW_NUMBER() OVER(ORDER BY ACRTA.TA001, ACRTA.TA002) AS SerialNo");
                sql.AppendLine(" FROM [ProUnion].dbo.ACRTA WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = ACRTA.TA004 ");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 ");
                sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTJ WITH(NOLOCK) ON ACRTA.TA001 = COPTJ.TJ025 AND ACRTA.TA002 = COPTJ.TJ026 ");
                sql.AppendLine(" WHERE (ACRTA.TA036 = '') ");

                //CustID
                if (!string.IsNullOrEmpty(custID))
                {
                    sql.AppendLine(" AND (ACRTA.TA004 = @CustID) ");
                    cmd.Parameters.AddWithValue("CustID", custID);
                }

                //結帳日
                sql.AppendLine(" AND (ACRTA.TA003 >= @sDate) AND (ACRTA.TA003 <= @eDate) ");

                sql.AppendLine(" GROUP BY COPMA.MA001, COPMA.MA003");
                sql.AppendLine("  , ISNULL(RTRIM(COPTG.TG001) + '-' + RTRIM(COPTG.TG002), RTRIM(COPTJ.TJ001) + '-' + RTRIM(COPTJ.TJ002))");
                sql.AppendLine("  , ACRTA.TA003, ACRTA.TA036, ACRTA.TA001, ACRTA.TA002");
                sql.AppendLine(")");
                sql.AppendLine(" SELECT TblBase.*");
                sql.AppendLine(" FROM TblBase");
                //不可與E6的資料重複
                sql.AppendLine(" WHERE (LEN(TblBase.Erp_SO_ID) >= 1)");
                sql.AppendLine("  AND TblBase.Erp_SO_ID NOT IN (");
                sql.AppendLine("     SELECT DT.Erp_SO_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                sql.AppendLine("     FROM SZ_Invoice_E6 Base");
                sql.AppendLine("      INNER JOIN SZ_Invoice_E6_Items DT ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine(" )");
                sql.AppendLine(" ORDER BY TblBase.Erp_SO_ID");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("sDate", startDate);
                cmd.Parameters.AddWithValue("eDate", endDate);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ERP_Invoice
                        {
                            Erp_SO_ID = item.Field<string>("Erp_SO_ID"),    //銷貨單/銷退單
                            Erp_AR_ID = item.Field<string>("Erp_AR_ID"),    //結帳單
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            ArDate = item.Field<string>("ArDate"),
                            InvNo = item.Field<string>("InvNo"),
                            SerialNo = item.Field<Int64>("SerialNo")
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


        #region >> 電商紙本發票 <<
        /// <summary>
        /// [電商紙本發票] 取得BBC已有開票資料,ERP未開票的內容
        /// </summary>
        /// <param name="mallID">商城代號</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<BBC_Data> GetBBCBillData(string mallID, string startDate, string endDate, Dictionary<string, string> search
            , out string ErrMsg)
        {
            try
            {
                //check null
                if (string.IsNullOrWhiteSpace(mallID) || string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                {
                    ErrMsg = "參數未正確傳遞.";
                    return null;
                }

                //----- 宣告 -----
                List<BBC_Data> dataList = new List<BBC_Data>();
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----

                    //--ERP未開票單據
                    sql.AppendLine(" ;WITH TblBase AS (");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("  RTRIM(COPTG.TG001) + '-' + RTRIM(COPTG.TG002) AS Erp_SO_ID");
                    sql.AppendLine("  , RTRIM(ACRTA.TA001) + '-' + RTRIM(ACRTA.TA002) AS Erp_AR_ID");
                    sql.AppendLine("  , RTRIM(COPMA.MA001) AS CustID");
                    sql.AppendLine("  , RTRIM(COPMA.MA003) AS CustName");
                    sql.AppendLine("  , COPTC.TC012 AS OrderID");
                    sql.AppendLine("  , CONVERT(FLOAT, COPTG.TG045 + COPTG.TG046) AS TotalPrice");
                    sql.AppendLine(" FROM [ProUnion].dbo.ACRTA WITH(NOLOCK)");
                    sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002");
                    sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = ACRTA.TA004");
                    sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002");
                    sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                    sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTC WITH(NOLOCK) ON COPTC.TC001 = COPTH.TH014 AND COPTC.TC002 = COPTH.TH015");
                    //filter:sDate, eDate, TA036發票空白
                    sql.AppendLine(" WHERE (ACRTA.TA003 >= @sDate) AND (ACRTA.TA003 <= @eDate)");
                    sql.AppendLine("  AND (ACRTA.TA036 = '')");
                    sql.AppendLine(" GROUP BY RTRIM(COPTG.TG001) + '-' + RTRIM(COPTG.TG002)");
                    sql.AppendLine("  , RTRIM(ACRTA.TA001) + '-' + RTRIM(ACRTA.TA002)");
                    sql.AppendLine("  , RTRIM(COPMA.MA001), RTRIM(COPMA.MA003), COPTC.TC012");
                    sql.AppendLine("  , COPTG.TG045 , COPTG.TG046");
                    sql.AppendLine(" )");

                    //BBC有開票資料的單據(排除贈品)
                    sql.AppendLine(" , TblBBC AS (");
                    sql.AppendLine(" SELECT Base.MallID, Base.TraceID");
                    sql.AppendLine(" , DT.OrderID COLLATE Chinese_Taiwan_Stroke_BIN AS OrderID");
                    sql.AppendLine(" FROM [PKEF].dbo.BBC_ImportData AS Base WITH(NOLOCK)");
                    sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_ImportData_DT AS DT WITH(NOLOCK) ON Base.Data_ID = DT.Parent_ID");
                    sql.AppendLine(" WHERE (Base.Status = 13) AND (DT.IsGift = 'N') AND (DT.IsPass = 'Y')");
                    //filter:MallID
                    sql.AppendLine("  AND (Base.MallID = @MallID)");
                    sql.AppendLine(" GROUP BY Base.MallID, Base.TraceID, DT.OrderID");
                    sql.AppendLine(" )");
                    sql.AppendLine(" SELECT TblBase.Erp_SO_ID, TblBase.Erp_AR_ID");
                    sql.AppendLine("  , TblBase.CustID, TblBase.CustName");
                    sql.AppendLine("  , TblBase.TotalPrice");
                    sql.AppendLine("  , TblBBC.MallID, TblBBC.TraceID, TblBBC.OrderID");
                    sql.AppendLine("  , InvItem.Inv_Type AS InvType");
                    sql.AppendLine("  , InvItem.Inv_Title AS vendeename");
                    sql.AppendLine("  , InvItem.Inv_Number AS vendeetax");
                    sql.AppendLine("  , InvItem.Inv_AddrInfo AS vendeeadress");
                    sql.AppendLine("  , InvItem.Inv_BankInfo AS vendeebnkno");
                    sql.AppendLine(" FROM TblBase");
                    sql.AppendLine("  INNER JOIN TblBBC ON TblBase.OrderID = TblBBC.OrderID");
                    //sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON (COPTG.TG001+'-'+COPTG.TG002) = TblBase.Erp_SO_ID");
                    sql.AppendLine("  INNER JOIN [PKEF].dbo.BBC_InvoiceItem AS InvItem ON TblBBC.OrderID = InvItem.OrderID AND TblBBC.TraceID = InvItem.TraceID");
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

                                case "OrderIDs":
                                    //將以逗號分隔的字串轉為Array
                                    string[] aryData = Regex.Split(item.Value, @"\,{1}");
                                    ArrayList _listVals = new ArrayList(aryData);

                                    //GetSQLParam:SQL WHERE IN的方法
                                    sql.AppendLine(" AND TblBase.OrderID IN ({0})".FormatThis(CustomExtension.GetSQLParam(_listVals, "params")));
                                    for (int row = 0; row < _listVals.Count; row++)
                                    {
                                        cmd.Parameters.AddWithValue("params" + row, _listVals[row]);
                                    }
                                    break;


                                case "Check":
                                    //--不在E6裡的資料才Show
                                    sql.AppendLine(" AND TblBase.Erp_SO_ID NOT IN (");
                                    sql.AppendLine("     SELECT DT.Erp_SO_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                                    sql.AppendLine("     FROM SZ_Invoice_E6 Base");
                                    sql.AppendLine("         INNER JOIN SZ_Invoice_E6_Items DT ON Base.Data_ID = DT.Parent_ID");
                                    sql.AppendLine("     WHERE (Base.IsUpdate = 'N')");
                                    sql.AppendLine(" )");

                                    break;

                            }
                        }
                    }
                    #endregion


                    sql.AppendLine(" ORDER BY InvItem.Inv_Title");

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.CommandTimeout = 120;   //單位:秒
                    cmd.Parameters.AddWithValue("MallID", mallID);
                    cmd.Parameters.AddWithValue("sDate", startDate);
                    cmd.Parameters.AddWithValue("eDate", endDate);

                    //----- 資料取得 -----
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new BBC_Data
                            {
                                Erp_SO_ID = item.Field<string>("Erp_SO_ID"),
                                Erp_AR_ID = item.Field<string>("Erp_AR_ID"),
                                CustID = item.Field<string>("CustID"),
                                CustName = item.Field<string>("CustName"),
                                TotalPrice = item.Field<double>("TotalPrice"),
                                MallID = item.Field<Int32>("MallID"),
                                InvType = item.Field<string>("InvType"),
                                InvTypeName = GetInvTypeName(item.Field<string>("InvType")),
                                TraceID = item.Field<string>("TraceID"),
                                OrderID = item.Field<string>("OrderID"),
                                vendeename = item.Field<string>("vendeename"),
                                vendeetax = item.Field<string>("vendeetax"),
                                vendeeadress = item.Field<string>("vendeeadress"),
                                vendeebnkno = item.Field<string>("vendeebnkno")
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
                ErrMsg = ex.Message.ToString();
                return null;
            }

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


        #endregion


        #region -----// Create //-----

        #region >> 一般紙本發票 <<
        /// <summary>
        /// [一般紙本發票] 取得本機檢查檔 - Step1
        /// </summary>
        /// <param name="subject">主旨</param>
        /// <param name="custID">客戶代號</param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="query">銷貨單號/結帳單號(迴圈)</param>
        /// <param name="ErrMsg"></param>
        /// <returns>Guid</returns>
        /// <remarks>
        /// IsPass = 'N', 代表Inv_UID重複, 需重新執行此功能
        /// </remarks>
        public string CreateBaseData(string subject, string custID, string sDate, string eDate
            , IQueryable<DT_Check> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(" DECLARE @GetToday AS VARCHAR(6), @GetNewID AS INT, @GetNewDtID AS INT ");
                sql.AppendLine("  , @GetFullID AS VARCHAR(9), @GetGuid AS uniqueidentifier ");
                sql.AppendLine("  , @IsPass AS CHAR(1) ");

                //--取得前置代碼(日期)
                sql.AppendLine(" SET @GetToday = RIGHT(CONVERT(VARCHAR(10), GETDATE(), 112), 6) ");

                //--取得新序號
                sql.AppendLine(" SET @GetNewID = ( ");
                sql.AppendLine("     SELECT ISNULL(MAX(SecID), 0) + 1 ");
                sql.AppendLine("     FROM SZ_Invoice_E6 ");
                sql.AppendLine("     WHERE (FstID = @GetToday) AND (DataType = 1) ");
                sql.AppendLine(" ) ");

                //--設定航天中繼編號
                sql.AppendLine(" SET @GetFullID = @GetToday + RIGHT('00' + CAST(@GetNewID AS VARCHAR(3)), 3) ");
                //--設定系統編號
                sql.AppendLine(" SET @GetGuid = (SELECT NEWID()) ");

                //--Check Exists
                sql.AppendLine(" SET @IsPass = CASE WHEN (SELECT COUNT(*) FROM SZ_Invoice_E6 WHERE (Inv_UID = @GetFullID)) = 0 THEN 'Y' ELSE 'N' END ");

                //--執行新增
                sql.AppendLine(" IF (@IsPass = 'Y') ");
                sql.AppendLine("  BEGIN ");

                //--(單頭)
                sql.AppendLine("     INSERT INTO SZ_Invoice_E6( ");
                sql.AppendLine("      Data_ID, FstID, SecID, CustID ");
                sql.AppendLine("      , Inv_UID, Inv_Subject, erp_sDate, erp_eDate ");
                sql.AppendLine("      , Create_Who, Create_Time, DataType ");
                sql.AppendLine("     ) VALUES ( ");
                sql.AppendLine("      @GetGuid, @GetToday, @GetNewID, @CustID ");
                sql.AppendLine("      , @GetFullID, @Subject + @GetFullID, @erp_sDate, @erp_eDate ");
                sql.AppendLine("      , @Creater, GETDATE(), 1 ");
                sql.AppendLine("     ); ");

                //--(單身)
                foreach (var item in query)
                {
                    sql.AppendLine("     SET @GetNewDtID = ( ");
                    sql.AppendLine("         SELECT ISNULL(MAX(Data_ID), 0) + 1 ");
                    sql.AppendLine("         FROM SZ_Invoice_E6_Items ");
                    sql.AppendLine("         WHERE (Parent_ID = @GetGuid) ");
                    sql.AppendLine("     ); ");
                    sql.AppendLine("     INSERT INTO SZ_Invoice_E6_Items( ");
                    sql.AppendLine("      Parent_ID, Data_ID ");
                    sql.AppendLine("      , Erp_SO_ID, Erp_AR_ID ");
                    sql.AppendLine("     ) VALUES ( ");
                    sql.AppendLine("      @GetGuid, @GetNewDtID ");
                    sql.AppendLine("      , '{0}', '{1}' ".FormatThis(item.Erp_SO_ID, item.Erp_AR_ID));
                    sql.AppendLine("     );	 ");
                }

                sql.AppendLine("  END ");

                //--回傳(是否通過, 系統編號)
                sql.AppendLine(" SELECT @IsPass AS IsPass, @GetGuid AS GetGuid ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 120;   //單位:秒
                cmd.Parameters.AddWithValue("Subject", subject);  //'(CustID) CustName-開票#@GetGuid'
                cmd.Parameters.AddWithValue("CustID", custID);
                cmd.Parameters.AddWithValue("erp_sDate", sDate);
                cmd.Parameters.AddWithValue("erp_eDate", eDate);
                cmd.Parameters.AddWithValue("Creater", fn_Params.UserGuid);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }

                    if (DT.Rows[0]["IsPass"].ToString().Equals("N"))
                    {
                        return "";
                    }

                    //return guid
                    return DT.Rows[0]["GetGuid"].ToString();
                }
            }

        }


        /// <summary>
        /// [一般紙本發票] 資料轉入中繼檔 - Step2
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="who"></param>
        /// <param name="type">1:一般紙本發票 2:電商紙本發票</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CreateImportData(string dataID, string who, string type, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                sql.AppendLine("DECLARE @rowCnt AS INT, @_InvUID AS VARCHAR(20)");
                sql.AppendLine(" SET @_InvUID = (SELECT Inv_UID FROM SZ_Invoice_E6 WHERE (Data_ID = @DataID))");

                //----- SQL 語法 (單頭) -----
                if (type.Equals("1"))
                {
                    //一般紙本
                    sql.AppendLine("BEGIN ");
                    sql.AppendLine(" INSERT INTO HTTAX_SALEBILLVOUCH ( ");
                    sql.AppendLine(" unino, erp_unino ");
                    sql.AppendLine(" , vendeename, vendeetax, vendeeadress, vendeebnkno ");
                    sql.AppendLine(" , billdate, remark, invoicekind, sta, negativesign, machineno ");
                    sql.AppendLine(" ) ");
                    sql.AppendLine(" SELECT ");
                    sql.AppendLine("     Base.Inv_UID AS unino ");
                    sql.AppendLine("     , Base.Inv_UID AS erp_unino ");
                    sql.AppendLine("     , RTRIM(COPMA.MA003) AS vendeename ");
                    sql.AppendLine("     , RTRIM(COPMA.MA071) AS vendeetax ");
                    sql.AppendLine("     , RTRIM(COPMA.MA025) AS vendeeadress ");
                    sql.AppendLine("     , RTRIM(COPMA.MA110) AS vendeebnkno ");
                    sql.AppendLine("     , NULL AS billdate ");
                    sql.AppendLine("     , '' AS remark ");
                    sql.AppendLine("     , CAST(ISNULL(Cust.InvType, 0) AS INT) AS invoicekind "); //--0—专用发票 / 2—普通发票
                    sql.AppendLine("     , 'NEW' AS sta ");
                    sql.AppendLine("     , 'N' AS negativesign ");
                    sql.AppendLine("     , 1 AS machineno"); //--深圳0, 上海1 (20200415修改0->1)
                    sql.AppendLine(" FROM SZ_Invoice_E6 Base ");
                    sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN ");
                    sql.AppendLine("  LEFT JOIN Customer_Data Cust ON Cust.Cust_ERPID = Base.CustID ");
                    sql.AppendLine(" WHERE (Base.Data_ID = @DataID) ");
                    sql.AppendLine(" ORDER BY erp_unino ");
                    sql.AppendLine("END ");
                }
                else
                {
                    //電商紙本
                    sql.AppendLine("BEGIN ");
                    sql.AppendLine(" INSERT INTO HTTAX_SALEBILLVOUCH ( ");
                    sql.AppendLine(" unino, erp_unino ");
                    sql.AppendLine(" , vendeename, vendeetax, vendeeadress, vendeebnkno ");
                    sql.AppendLine(" , billdate, remark, invoicekind, sta, negativesign, machineno ");
                    sql.AppendLine(" ) ");
                    sql.AppendLine(" SELECT ");
                    sql.AppendLine("     Base.Inv_UID AS unino");
                    sql.AppendLine("     , Base.Inv_UID AS erp_unino");
                    sql.AppendLine("     , Rel.vendeename AS vendeename");
                    sql.AppendLine("     , Rel.vendeetax AS vendeetax");
                    sql.AppendLine("     , Rel.vendeeadress AS vendeeadress");
                    sql.AppendLine("     , Rel.vendeebnkno AS vendeebnkno");
                    sql.AppendLine("     , NULL AS billdate");
                    sql.AppendLine("     , '' AS remark");
                    sql.AppendLine("     , CAST((CASE Rel.InvType WHEN '2' THEN 0 ELSE 2 END) AS INT) AS invoicekind"); //--0—专用发票 / 2—普通发票 
                    sql.AppendLine("     , 'NEW' AS sta");
                    sql.AppendLine("     , 'N' AS negativesign");
                    sql.AppendLine("     , 1 AS machineno"); //深圳0，上海1
                    sql.AppendLine(" FROM SZ_Invoice_E6 Base");
                    sql.AppendLine("  INNER JOIN SZ_Invoice_E6_BBCData Rel ON Base.Data_ID = Rel.Parent_ID");
                    sql.AppendLine("  LEFT JOIN Customer_Data Cust ON Cust.Cust_ERPID = Base.CustID");
                    sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                    sql.AppendLine(" ORDER BY erp_unino");
                    sql.AppendLine("END ");
                }

                //單頭資料筆數
                sql.AppendLine(" SET @rowCnt = (SELECT COUNT(*) FROM HTTAX_SALEBILLVOUCH WHERE (unino = @_InvUID));");

                //----- SQL 語法 (單身) -----
                /* ResetDTLines:單身資料重新整理 */

                sql.AppendLine(" IF (@rowCnt > 0)");
                sql.AppendLine(" BEGIN");

                var query_Lines = ResetDTLines(dataID);
                int row = 0;
                foreach (var line in query_Lines)
                {
                    row++;
                    sql.Append(" INSERT INTO HTTAX_SALEBILLVOUCHS (");
                    sql.Append(" unino, serial, linenumber");
                    sql.Append(" , qty, price, shpamt, taxrate, taxation, disamt, oldprice");
                    sql.Append(" , tradename, model, unit, taxprice");
                    sql.Append(" , bmbbh, ssflbm, xsyhzc");
                    sql.Append(" ) VALUES (");
                    //unino, serial, linenumber
                    sql.Append(" '{0}', {1}, '{1}'".FormatThis(line.unino, row));
                    //, qty, price, shpamt, taxrate, taxation, disamt, oldprice
                    sql.Append(" ,{0}, {1}, {2}, {3}, {4}, {5}, {6}".FormatThis(
                        line.qty, line.price, line.shpamt, line.taxrate, line.taxation, line.disamt, line.oldprice));
                    //, tradename, model, unit, taxprice
                    sql.Append(" ,N'{0}', N'{1}', N'{2}', '{3}'".FormatThis(
                        line.tradename.Replace("'", "''"), line.model, line.unit, line.taxprice));
                    //, bmbbh, ssflbm, xsyhzc
                    sql.Append(" ,N'{0}', N'{1}', N'{2}'".FormatThis(
                        line.bmbbh, line.ssflbm, line.xsyhzc));

                    sql.Append(" );");
                }


                //--Update
                sql.AppendLine("  UPDATE SZ_Invoice_E6 SET IsInsert = 'Y', Update_Who = @Update_Who, Update_Time = GETDATE(), Import_Time = GETDATE() ");
                sql.AppendLine("  WHERE (Data_ID = @DataID)");
                sql.AppendLine(" END");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("Update_Who", who);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }
        }

        /// <summary>
        /// 重新整理單身資料
        /// 銷貨折讓要分攤至各品項(不含維修費), 填入折扣金額
        /// 並重置單價及金額(折扣後)
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        private IQueryable<DT_Lines> ResetDTLines(string dataID)
        {
            /*
             * -- 取得原資料放入容器 (A):原始資料 --
                1.判斷 (A) 是否有W001/W003
                 - 有:取得折讓金額:cntX = SUM(shpamt) -> 前往 2.
                 - 無:直接INSERT所有資料
                 - W003的金額合併至W001

                2.執行折讓分攤 ((折讓金額 / (各品項金額加總)) * 該品項金額)
                 - 從(A)取資料, 排除B009 及 W001/W003, 放入容器 (B):要分攤的所有品項
                 - 從(A)取資料, 只有B009, 放入容器 (C):維修費
                 - 維修費:不可分攤

                3.計算
                 - 各品項金額加總:cntY = SUM(B.shpamt)
                 - cntRate = cntX / cntY

                4.將品項重新整理:(D)
                 - D.Add(C)  -- 維修費
                 - (B) Loop -> result = cntRate * B.shpamt(金額)
                                     -> B.Item Update disamt(折扣金額) = result
                                     -> B.Item Update 單價/金額 (折扣後單價= 單價 - (折扣金額 / 數量))
                                     -> D.Add(B.Item)
                                     -> 差額Update至最後一筆

                5.將(D) INSERT 至系統
             */

            try
            {
                #region ** (A):原始資料 **

                //----- 宣告 -----
                List<DT_Lines> dataList = new List<DT_Lines>();
                StringBuilder sql = new StringBuilder();

                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 語法 -----
                    sql.AppendLine(" ;WITH TblBase AS(");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         SUM(CAST((COPTH.TH008 - ISNULL(COPTJ.TJ007, 0)) AS FLOAT)) AS qty");
                    sql.AppendLine("         , (SUM(CAST(COPTH.TH013 AS MONEY)) / SUM(CAST((COPTH.TH008 - ISNULL(COPTJ.TJ007, 0)) AS FLOAT))) AS price	--//單價");
                    sql.AppendLine("         , SUM(CAST(COPTH.TH013 AS MONEY)) AS shpamt	--//原金額(計算用qty*price)");
                    sql.AppendLine("         , CAST(0.13 AS FLOAT) AS taxrate"); //**稅率(固定值**
                    sql.AppendLine("         , SUM(CAST(COPTH.TH038 AS MONEY)) AS taxation	--//稅額");
                    sql.AppendLine("         , COPTH.TH005 AS tradename	--//品名");
                    sql.AppendLine("         , RTRIM(COPTH.TH004) AS model	--//品號");
                    sql.AppendLine("         , (CASE COPTH.TH009 WHEN 'PCE' THEN N'个' WHEN 'BAG' THEN N'包' WHEN 'PCS' THEN N'个' WHEN 'SET' THEN N'套' ELSE COPTH.TH009 END) AS unit");
                    sql.AppendLine("         , 'Y' AS taxprice  --//含稅(Y/N)");
                    sql.AppendLine("         , '1.0' AS bmbbh  --//編碥版本");
                    sql.AppendLine("         , (CASE WHEN RTRIM(COPTH.TH004) = 'B009' THEN '202' ELSE '108040412' END) AS ssflbm   --//稅收分類編碼(會變動)");
                    sql.AppendLine("         , '0' AS xsyhzc  --//享受优惠政策(0:不享受 1:享受)");
                    sql.AppendLine("     FROM SZ_Invoice_E6 Base");
                    sql.AppendLine("      INNER JOIN SZ_Invoice_E6_Items DT ON Base.Data_ID = DT.Parent_ID");
                    sql.AppendLine("      INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON (RTRIM(ACRTB.TB005) + '-' + RTRIM(ACRTB.TB006)) = DT.Erp_SO_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                    sql.AppendLine("      INNER JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 AND COPTG.TG004 = Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN");
                    sql.AppendLine("      INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002");
                    sql.AppendLine("      LEFT JOIN [ProUnion].dbo.COPTJ WITH(NOLOCK) ON COPTH.TH001 = COPTJ.TJ015 AND COPTH.TH002 = COPTJ.TJ016 AND COPTH.TH003 = COPTJ.TJ017");
                    sql.AppendLine("     WHERE (Base.Data_ID = @id) AND ((COPTH.TH008 - ISNULL(COPTJ.TJ007, 0)) > 0)  AND (COPTH.TH013 <> 0)");
                    sql.AppendLine("     GROUP BY COPTH.TH004, COPTH.TH005, COPTH.TH009");
                    sql.AppendLine(" )");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     Base.Inv_UID AS unino");
                    sql.AppendLine("     , TblBase.qty AS qty");
                    sql.AppendLine("     , TblBase.price");
                    sql.AppendLine("     , TblBase.qty * TblBase.price AS shpamt");
                    sql.AppendLine("     , TblBase.taxrate");
                    sql.AppendLine("     , TblBase.taxation");
                    sql.AppendLine("     , TblBase.tradename");
                    sql.AppendLine("     , TblBase.model");
                    sql.AppendLine("     , TblBase.unit");
                    sql.AppendLine("     , TblBase.taxprice");
                    sql.AppendLine("     , TblBase.bmbbh");
                    sql.AppendLine("     , TblBase.ssflbm");
                    sql.AppendLine("     , TblBase.xsyhzc");
                    sql.AppendLine(" FROM SZ_Invoice_E6 Base, TblBase");
                    sql.AppendLine(" WHERE (Base.Data_ID = @id)");
                    sql.AppendLine(" ORDER BY TblBase.model ASC");

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("id", dataID);

                    //----- 資料取得 -----
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        if (DT.Rows.Count > 0)
                        {
                            //LinQ 查詢
                            var query = DT.AsEnumerable();

                            //資料迴圈
                            foreach (var item in query)
                            {
                                //加入項目
                                var data = new DT_Lines
                                {
                                    unino = item.Field<string>("unino"),
                                    qty = item.Field<double>("qty"),
                                    price = item.Field<double>("price"),
                                    shpamt = item.Field<double>("shpamt"),
                                    taxrate = item.Field<double>("taxrate"),
                                    taxation = Convert.ToDouble(item.Field<decimal>("taxation")),
                                    tradename = item.Field<string>("tradename"),
                                    model = item.Field<string>("model"),
                                    unit = item.Field<string>("unit"),
                                    taxprice = item.Field<string>("taxprice"),
                                    bmbbh = item.Field<string>("bmbbh"),
                                    ssflbm = item.Field<string>("ssflbm"),
                                    xsyhzc = item.Field<string>("xsyhzc")
                                };

                                //將項目加入至集合
                                dataList.Add(data);

                            }
                        }
                    }
                }


                #endregion


                //----- 檢查(A)是否有W001/W003 -----
                var check = dataList.Where(fld => fld.model.Equals("W001") || fld.model.Equals("W003"));
                if (check.Count() == 0)
                {
                    //無W001/W003, 直接回傳
                    return dataList.AsQueryable();
                }


                #region ** 品項整理 & 計算 **

                //----- 排除B009及 W001/W003, 放入容器(B):要分攤的所有品項 -----
                var dataB = dataList.Where(fld =>
                    !fld.model.Equals("W001")
                    && !fld.model.Equals("W003")
                    && !fld.model.Equals("B009"));

                //----- 只有B009, 放入容器(C):維修費 -----
                var dataC = dataList.Where(fld => fld.model.Equals("B009"));


                //取得原始總金額(X)
                double cntX = -check.Sum(fld => fld.shpamt);
                //各品項金額加總(Y)
                double cntY = dataB.Sum(fld => fld.shpamt);
                //計算比率
                double cntRate = cntX / cntY;


                //----- (D)品項重新組合 -----
                List<DT_Lines> dataResult = new List<DT_Lines>();

                //維修費
                dataResult.AddRange(dataC);

                //其他品項
                foreach (var item in dataB)
                {
                    //原單價
                    double oldPrice = item.price;
                    //原小計金額
                    double oldTotal = item.shpamt;
                    //原數量
                    double getQty = item.qty;

                    /* 折扣計算 S */
                    //折扣金額 - (W001分攤的金額 -> 各品項金額*比率cntRate)
                    double disTotal = Math.Round(cntRate * oldTotal, 2, MidpointRounding.AwayFromZero);
                    //折扣後單價 - (單價 - (折扣金額 / 數量))
                    double disUnitPrice = oldPrice - (disTotal / getQty);
                    //稅額
                    double taxation = disUnitPrice * getQty * item.taxrate;
                    //折扣金額
                    double disamt = disTotal;
                    /* 折扣計算 E */


                    //加入項目 - 原品項
                    var data = new DT_Lines
                    {
                        unino = item.unino,
                        qty = item.qty,
                        price = disUnitPrice,
                        shpamt = disUnitPrice * getQty,
                        taxrate = item.taxrate,
                        taxation = taxation,
                        tradename = item.tradename,
                        model = item.model,
                        unit = item.unit,
                        oldprice = oldPrice,
                        disamt = disamt,
                        taxprice = item.taxprice,
                        bmbbh = item.bmbbh,
                        ssflbm = item.ssflbm,
                        xsyhzc = item.xsyhzc
                    };

                    //將項目加入至集合
                    dataResult.Add(data);
                }

                #endregion

                //取得計算後的折讓總額
                double sumResult = dataResult.Sum(fld => fld.disamt);
                //計算分攤後的折讓差額(cntX(原始折讓金額) - sumResult(分攤後的總金額))
                double cntGapDis = Math.Round(cntX - sumResult, 2, MidpointRounding.AwayFromZero);

                //折扣補上折扣差額(+)
                dataResult.Last().disamt += cntGapDis;
                //金額扣掉折扣差額(-)
                dataResult.Last().shpamt += (-cntGapDis);


                //回傳新集合
                return dataResult.AsQueryable();

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion


        #region >> 電商紙本發票 <<
        /// <summary>
        /// [電商紙本發票] 建立本機檢查檔 - Step1
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="query">資料迴圈</param>
        /// <param name="ErrMsg"></param>
        /// <returns>Guid</returns>
        /// <remarks>
        /// </remarks>
        public bool CreateBaseData_BBC(string sDate, string eDate, IQueryable<BBC_Data> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(" DECLARE @GetToday AS VARCHAR(6), @GetNewID AS INT, @GetNewDtID AS INT, @GetNewRelID AS INT");
                sql.AppendLine("  , @GetFullID AS VARCHAR(10), @GetGuid AS uniqueidentifier ");
                sql.AppendLine("  , @IsPass AS CHAR(1) ");

                //--取得前置代碼(日期)
                sql.AppendLine(" SET @GetToday = RIGHT(CONVERT(VARCHAR(10), GETDATE(), 112), 6)");

                int row = 0;

                foreach (var item in query)
                {
                    row++;

                    //--取得新序號(注意DataType)
                    sql.AppendLine(" SET @GetNewID = ( ");
                    sql.AppendLine("     SELECT ISNULL(MAX(SecID), 0) + 1 ");
                    sql.AppendLine("     FROM SZ_Invoice_E6 ");
                    sql.AppendLine("     WHERE (FstID = @GetToday) AND (DataType = 2) ");
                    sql.AppendLine(" ) ");

                    //--設定航天中繼編號(@GetToday+@GetNewID)
                    sql.AppendLine(" SET @GetFullID = @GetToday + RIGHT('000' + CAST(@GetNewID AS VARCHAR(4)), 4) ");
                    //--設定系統編號
                    sql.AppendLine(" SET @GetGuid = (SELECT NEWID()) ");

                    //--Check Exists
                    sql.AppendLine(" SET @IsPass = CASE WHEN (SELECT COUNT(*) FROM SZ_Invoice_E6 WHERE (Inv_UID = @GetFullID)) = 0 THEN 'Y' ELSE 'N' END ");

                    //--執行新增
                    sql.AppendLine(" IF (@IsPass = 'Y') ");
                    sql.AppendLine("  BEGIN ");

                    //--(單頭)(注意DataType)
                    sql.AppendLine("     INSERT INTO SZ_Invoice_E6( ");
                    sql.AppendLine("      Data_ID, FstID, SecID, CustID ");
                    sql.AppendLine("      , Inv_UID, Inv_Subject, erp_sDate, erp_eDate ");
                    sql.AppendLine("      , Create_Who, Create_Time, DataType ");
                    sql.AppendLine("     ) VALUES ( ");
                    sql.AppendLine("      @GetGuid, @GetToday, @GetNewID, @CustID_" + row);
                    sql.AppendLine("      , @GetFullID, @Subject_" + row + ", @erp_sDate, @erp_eDate ");
                    sql.AppendLine("      , @Creater, GETDATE(), 2 ");
                    sql.AppendLine("     ); ");
                    //主旨:京东商城旗舰店B(C021085)#71464071684#上海恭锐电子科技有限公司
                    cmd.Parameters.AddWithValue("Subject_" + row, "{0}({1})#{2}#{3}".FormatThis(
                         item.CustName
                         , item.CustID
                         , item.OrderID
                         , item.vendeename
                        ));
                    cmd.Parameters.AddWithValue("CustID_" + row, item.CustID);


                    //--(單身)
                    sql.AppendLine("     SET @GetNewDtID = ( ");
                    sql.AppendLine("         SELECT ISNULL(MAX(Data_ID), 0) + 1 ");
                    sql.AppendLine("         FROM SZ_Invoice_E6_Items ");
                    sql.AppendLine("         WHERE (Parent_ID = @GetGuid) ");
                    sql.AppendLine("     ); ");
                    sql.AppendLine("     INSERT INTO SZ_Invoice_E6_Items( ");
                    sql.AppendLine("      Parent_ID, Data_ID ");
                    sql.AppendLine("      , Erp_SO_ID, Erp_AR_ID ");
                    sql.AppendLine("     ) VALUES ( ");
                    sql.AppendLine("      @GetGuid, @GetNewDtID ");
                    sql.AppendLine("      , '{0}', '{1}' ".FormatThis(item.Erp_SO_ID, item.Erp_AR_ID));
                    sql.AppendLine("     );	 ");


                    //--(關聯檔)
                    sql.AppendLine("     SET @GetNewRelID = ( ");
                    sql.AppendLine("         SELECT ISNULL(MAX(Data_ID), 0) + 1 ");
                    sql.AppendLine("         FROM SZ_Invoice_E6_BBCData ");
                    sql.AppendLine("         WHERE (Parent_ID = @GetGuid) ");
                    sql.AppendLine("     ); ");
                    sql.AppendLine("     INSERT INTO SZ_Invoice_E6_BBCData( ");
                    sql.AppendLine("      Parent_ID, Data_ID");
                    sql.AppendLine("      , Erp_SO_ID, Erp_AR_ID");
                    sql.AppendLine("      , CustID, CustName");
                    sql.AppendLine("      , TotalPrice, MallID, TraceID, OrderID");
                    sql.AppendLine("      , vendeename, vendeetax, vendeeadress, vendeebnkno");
                    sql.AppendLine("      , InvType");
                    sql.AppendLine("     ) VALUES ( ");
                    sql.AppendLine("      @GetGuid, @GetNewRelID ");
                    sql.AppendLine("      , '{0}', '{1}' ".FormatThis(item.Erp_SO_ID, item.Erp_AR_ID));
                    sql.AppendLine("      , N'{0}', N'{1}' ".FormatThis(item.CustID, item.CustName));
                    sql.AppendLine("      , {0}, {1}, '{2}', '{3}' ".FormatThis(item.TotalPrice, item.MallID, item.TraceID, item.OrderID));
                    sql.AppendLine("      , N'{0}', N'{1}', N'{2}', N'{3}' ".FormatThis(item.vendeename, item.vendeetax, item.vendeeadress, item.vendeebnkno));
                    sql.AppendLine("      , N'{0}'".FormatThis(item.InvType));
                    sql.AppendLine("     );	 ");

                    sql.AppendLine("  END ");
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 180;   //單位:秒
                cmd.Parameters.AddWithValue("erp_sDate", sDate);
                cmd.Parameters.AddWithValue("erp_eDate", eDate);
                cmd.Parameters.AddWithValue("Creater", fn_Params.UserGuid);

                //----- 資料取得 -----
                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }

        }

        #endregion

        #endregion


        #region -----// Update //-----

        #region >> 一般紙本發票 <<
        /// <summary>
        /// [一般紙本發票] 回寫ERP發票 - List Update Button
        /// (SQL使用開票用帳號=ERPTAX)
        /// Ashx_InvoiceData.ashx
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update(string dataID, string who, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            string invoiceNo = "";
            string invoiceDate = "";

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法(取得發票資料-開票系統) -----
                sql.AppendLine(" DECLARE @InvUID AS VARCHAR(20), @rowCnt AS INT, @endChar AS VARCHAR(3)");
                //取得序號
                sql.AppendLine(" SET @InvUID = (SELECT Inv_UID FROM SZ_Invoice_E6 WHERE (Data_ID = @DataID))");
                //判斷筆數
                sql.AppendLine(" SET @rowCnt = (");
                sql.AppendLine("     SELECT COUNT(*)");
                sql.AppendLine("     FROM HTTAX_FEEDBACKHEADR");
                sql.AppendLine("     WHERE (unino = @InvUID)");
                sql.AppendLine(" )");
                //多筆:組合發票號碼 xxxx01-05(取末2碼)
                sql.AppendLine(" IF (@rowCnt > 1)");
                sql.AppendLine(" BEGIN");
                sql.AppendLine("  SET @endChar = (");
                sql.AppendLine("     SELECT TOP 1 '-' + RIGHT(invoiceno, 2)");
                sql.AppendLine("     FROM HTTAX_FEEDBACKHEADR");
                sql.AppendLine("     WHERE (unino = @InvUID)");
                sql.AppendLine("     ORDER BY invoiceno DESC");
                sql.AppendLine("  )");
                sql.AppendLine(" END");
                //取得發票號碼/日期,準備回寫ERP
                sql.AppendLine(" SELECT TOP 1 invoiceno + ISNULL(@endChar, '') AS InvoiceNo, CONVERT(VARCHAR(10), affdate, 112) AS InvoiceDate");
                sql.AppendLine(" FROM HTTAX_FEEDBACKHEADR");
                sql.AppendLine(" WHERE (unino = @InvUID)");
                sql.AppendLine(" ORDER BY invoiceno ASC");

                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "開票系統查無資料,請重新確認.";
                        return false;
                    }

                    //填入資料
                    invoiceNo = DT.Rows[0]["InvoiceNo"].ToString();
                    invoiceDate = DT.Rows[0]["InvoiceDate"].ToString();
                }

                //--- do clear ---
                sql.Clear();
                cmd.Parameters.Clear();

                //--Update Base
                sql.AppendLine(" UPDATE SZ_Invoice_E6 SET Inv_NO = @InvNo, Inv_Date = @InvDate, IsUpdate = 'Y' ");
                sql.AppendLine(" , Update_Who = @Update_Who, Update_Time = GETDATE(), ERPUpd_Time = GETDATE() ");
                sql.AppendLine(" WHERE (Data_ID = @DataID);");

                //--Update ERP
                sql.AppendLine(" UPDATE [ProUnion].dbo.ACRTA ");
                sql.AppendLine(" SET TA036 = @InvNo, TA200 = @InvDate ");
                sql.AppendLine(" WHERE (RTRIM(TA001)+'-'+RTRIM(TA002) IN ( ");
                sql.AppendLine("   SELECT Erp_AR_ID COLLATE Chinese_Taiwan_Stroke_BIN ");
                sql.AppendLine("   FROM SZ_Invoice_E6_Items ");
                sql.AppendLine("   WHERE (Parent_ID = @DataID) ");
                sql.AppendLine(" )) ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("InvNo", invoiceNo);
                cmd.Parameters.AddWithValue("InvDate", invoiceDate);
                cmd.Parameters.AddWithValue("Update_Who", who);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.ERP_TAX, out ErrMsg);
            }
        }


        /// <summary>
        /// [一般紙本發票][手動填發票] ERP發票號碼更新
        /// (SQL使用開票用帳號=ERPTAX)
        /// Ashx_UpdateErpInvoice.ashx
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="invNo"></param>
        /// <param name="invDate"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool UpdateInvNo(string dataID, string invNo, string invDate, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            string invoiceDate = string.IsNullOrWhiteSpace(invDate) ? DateTime.Now.ToString().ToDateString("yyyyMMdd") : invDate.ToDateString("yyyyMMdd");

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //--Update ERP
                sql.AppendLine(" UPDATE [ProUnion].dbo.ACRTA ");
                sql.AppendLine(" SET TA036 = @InvNo, TA200 = @InvDate ");
                sql.AppendLine(" WHERE (RTRIM(TA001)+'-'+RTRIM(TA002) = @DataID) ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("InvNo", invNo);
                cmd.Parameters.AddWithValue("InvDate", string.IsNullOrWhiteSpace(invNo) ? "" : invoiceDate);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.ERP_TAX, out ErrMsg);
            }
        }

        #endregion



        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// [共用]刪除資料 - Step2
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
                //----- SQL 語法 -----
                sql.AppendLine(" DELETE FROM SZ_Invoice_E6_Items WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM SZ_Invoice_E6_BBCData WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM SZ_Invoice_E6 WHERE (Data_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }
        }


        #endregion
    }
}
