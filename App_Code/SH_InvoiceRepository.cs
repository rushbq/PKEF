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
using SH_Invoice.Models;

/*
 * SH開票平台:
 * 一般紙本發票, 電商紙本發票
 *  - 發票轉入
 */
namespace SH_Invoice.Controllers
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


    public class SH_InvoiceRepository
    {
        public string ErrMsg;

        #region -----// Read //-----


        #region >> 一般紙本發票 <<

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
                sql.AppendLine(" FROM [SHPK2].dbo.ACRTA WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN [SHPK2].dbo.ACRTB WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002 ");
                sql.AppendLine("  INNER JOIN [SHPK2].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = ACRTA.TA004 ");
                sql.AppendLine("  LEFT JOIN [SHPK2].dbo.COPTG WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 ");
                sql.AppendLine("  LEFT JOIN [SHPK2].dbo.COPTJ WITH(NOLOCK) ON ACRTA.TA001 = COPTJ.TJ025 AND ACRTA.TA002 = COPTJ.TJ026 ");
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
                ////不可與E6的資料重複
                //sql.AppendLine(" WHERE (LEN(TblBase.Erp_SO_ID) >= 1)");
                //sql.AppendLine("  AND TblBase.Erp_SO_ID NOT IN (");
                //sql.AppendLine("     SELECT DT.Erp_SO_ID COLLATE Chinese_Taiwan_Stroke_BIN");
                //sql.AppendLine("     FROM SH_Invoice_E6 Base");
                //sql.AppendLine("      INNER JOIN SH_Invoice_E6_Items DT ON Base.Data_ID = DT.Parent_ID");
                //sql.AppendLine(" )");
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

      
        #endregion


        #region -----// Create //-----

   

        #endregion


        #region -----// Update //-----

        #region >> 一般紙本發票 <<

        /// <summary>
        /// [一般紙本發票][手動填發票] ERP發票號碼更新
        /// (SQL使用開票用帳號=ERPTAX)
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="who"></param>
        /// <param name="invNo"></param>
        /// <param name="invDate"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool UpdateInvNo(string dataID, string who, string invNo, string invDate,out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            string invoiceDate = invDate.ToDateString("yyyyMMdd");

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //--Update ERP
                sql.AppendLine(" UPDATE [SHPK2].dbo.ACRTA ");
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

        ///// <summary>
        ///// [共用]刪除資料 - Step2
        ///// </summary>
        ///// <param name="dataID"></param>
        ///// <returns></returns>
        //public bool Delete(string dataID)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 語法 -----
        //        sql.AppendLine(" DELETE FROM SH_Invoice_E6_Items WHERE (Parent_ID = @DataID);");
        //        sql.AppendLine(" DELETE FROM SH_Invoice_E6_BBCData WHERE (Parent_ID = @DataID);");
        //        sql.AppendLine(" DELETE FROM SH_Invoice_E6 WHERE (Data_ID = @DataID);");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", dataID);

        //        return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
        //    }
        //}


        #endregion
    }
}
