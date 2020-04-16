using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ERP_CopyData.Models;
using PKLib_Method.Methods;


/*
 * 
 */
namespace ERP_CopyData.Controllers
{

    public class ERP_CopyDataRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// [核價單/報價單複製] 檢查來源ERP資料庫是否有資料
        /// </summary>
        /// <param name="PrimaryID"></param>
        /// <param name="SubID"></param>
        /// <param name="SrcCompanyID"></param>
        /// <param name="flowType"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CheckPriceData(string PrimaryID, string SubID, string SrcCompanyID, string flowType, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
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
                if (flowType.ToUpper().Equals("A"))
                {
                    //核價單
                    sql.AppendLine(" SELECT COUNT(*) AS Cnt");
                    sql.AppendLine(" FROM ##SrcDatabase##.dbo.PURTL");
                    sql.AppendLine(" WHERE (TL001 = @PrimaryID) AND (TL002 = @SubID)");
                }
                else
                {
                    //報價單
                    sql.AppendLine(" SELECT COUNT(*) AS Cnt");
                    sql.AppendLine(" FROM ##SrcDatabase##.dbo.COPTA");
                    sql.AppendLine(" WHERE (TA001 = @PrimaryID) AND (TA002 = @SubID)");
                }


                //Replace DB 前置詞
                sql.Replace("##SrcDatabase##", SrcDatabase);


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("PrimaryID", PrimaryID);
                cmd.Parameters.AddWithValue("SubID", SubID);

                //----- return -----
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (Convert.ToInt32(DT.Rows[0]["Cnt"]) == 0)
                    {
                        ErrMsg = "查無資料,請確認ERP資料已建立.";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }


            }
        }


        /// <summary>
        /// 取得單別選單
        /// </summary>
        /// <param name="SrcCompanyID">取得哪家公司別的資料</param>
        /// <param name="flowType">A=核價/B=報價</param>
        /// <returns></returns>
        public IQueryable<RefClass> GetPriceTypeID(string SrcCompanyID, string flowType)
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
                //報價(21) / 核價(32)
                sql.AppendLine(" SELECT RTRIM(MQ001) AS ID, RTRIM(MQ002) AS Label");
                sql.AppendLine(" FROM ##SrcDatabase##.dbo.CMSMQ");
                sql.AppendLine(" WHERE (MQ003 = @typeID)");


                //Replace DB 前置詞
                sql.Replace("##SrcDatabase##", SrcDatabase);


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("typeID", flowType.ToUpper().Equals("A") ? "32" : "21");


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
                            Label = item.Field<string>("Label")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }

        }

        #endregion



        #region -----// Create //-----

        /// <summary>
        /// [核價單/報價單複製] 開始執行複製
        /// </summary>
        /// <param name="PrimaryID">來源單別</param>
        /// <param name="SubID">來源單號</param>
        /// <param name="SrcCompanyID">來源公司代號(TW/SH/SZ)</param>
        /// <param name="TarCompanyID">目的公司代號(TW/SH)</param>
        /// <param name="TarPrimaryID">目的單別</param>
        /// <param name="flowType">執行類型(A=核價/B=報價)</param>
        /// <param name="validDate">生效日(yyyMMdd)</param>
        /// <param name="invalidDate">失效日(yyyMMdd)</param>
        /// <param name="ErrMsg"></param>
        /// <returns>bool</returns>
        /// <remarks>
        ///
        /// </remarks>
        public bool CreatePriceData(string PrimaryID, string SubID, string SrcCompanyID
            , string TarCompanyID, string TarPrimaryID, string flowType
            , string validDate, string invalidDate, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                /* 設定DB Name */
                string SrcDatabase, TarDatabase, TarErpCompName;
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
                //目標DB
                switch (TarCompanyID.ToUpper())
                {
                    case "SH":
                        TarDatabase = "SHPK2";
                        TarErpCompName = "SHPK2";
                        break;

                    default:
                        TarDatabase = "prokit2";
                        TarErpCompName = "prokit(II)";
                        break;
                }


                //--- 判斷類型,取得對應的SQL ---
                if (flowType.ToUpper().Equals("A"))
                {
                    //核價單
                    sql.Append(SQLPriceA(PrimaryID, SubID, SrcCompanyID, TarCompanyID, TarPrimaryID));
                }
                else
                {
                    //報價單
                    sql.Append(SQLPriceB(PrimaryID, SubID, SrcCompanyID, TarCompanyID, TarPrimaryID));
                }


                //Replace DB 前置詞
                sql.Replace("##TarDatabase##", TarDatabase);
                sql.Replace("##SrcDatabase##", SrcDatabase);


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 90;   //單位:秒
                cmd.Parameters.AddWithValue("PrimaryID", PrimaryID);
                cmd.Parameters.AddWithValue("SubID", SubID);
                cmd.Parameters.AddWithValue("TarErpCompName", TarErpCompName);
                cmd.Parameters.AddWithValue("TarPrimaryID", TarPrimaryID);
                cmd.Parameters.AddWithValue("validDate", validDate);
                cmd.Parameters.AddWithValue("invalidDate", invalidDate);
                cmd.Parameters.AddWithValue("ErpCreate", "IT_IMPORT");
                cmd.Parameters.AddWithValue("ErpUsrGP", "100");

                //----- return -----
                return dbConn.ExecuteSql(cmd, out ErrMsg);

            }

        }


        /// <summary>
        /// SQL - 核價單
        /// </summary>
        /// <param name="PrimaryID"></param>
        /// <param name="SubID"></param>
        /// <param name="SrcCompanyID"></param>
        /// <param name="TarCompanyID"></param>
        /// <param name="TarPrimaryID"></param>
        /// <returns></returns>
        private StringBuilder SQLPriceA(string PrimaryID, string SubID, string SrcCompanyID
            , string TarCompanyID, string TarPrimaryID)
        {
            StringBuilder sql = new StringBuilder();

            /* 今天的日期(yyyyMMdd) */
            sql.AppendLine(" DECLARE @pToday AS VARCHAR(8)");
            sql.AppendLine(" SET @pToday = CONVERT(VARCHAR(8), GETDATE() ,112)");

            /* 取號:PK190414001 (PK+yyMMdd+流水號3碼) */
            sql.AppendLine(" DECLARE @NewID AS INT, @NewFullID AS VARCHAR(11), @FstID AS VARCHAR(2), @SecID AS VARCHAR(6)");
            sql.AppendLine(" SET @FstID = 'PK'");
            sql.AppendLine(" SET @SecID = RIGHT(@pToday, 6)");
            sql.AppendLine(" SET @NewID = (");
            sql.AppendLine(" 	SELECT ISNULL(MAX(CAST(RIGHT(TL002, 3) AS INT)), 0) + 1");
            sql.AppendLine(" 	FROM ##TarDatabase##.dbo.PURTL");
            sql.AppendLine(" 	WHERE (TL001 = @TarPrimaryID) AND (LEFT(TL002, 2) = @FstID) AND (SUBSTRING(TL002, 3, 6) = @SecID)");
            sql.AppendLine(" )");
            sql.AppendLine(" SET @NewFullID = @FstID + @SecID + RIGHT('00' + CAST(@NewID AS VARCHAR), 3)");


            /*
            [建立查詢] PURTL
            預設值:
             TL003 = Today
             TL006 確認碼 = 'N'
             TL009 列印次數 = 0
             TL010 = Today
             TL011 確認者 = ''
             TL012 簽核狀態碼 = 'N'
             TL013 傳送次數 = 0
            */
            sql.AppendLine(" INSERT INTO ##TarDatabase##.dbo.PURTL (");
            sql.AppendLine("	 COMPANY, CREATOR, USR_GROUP, CREATE_DATE, FLAG");
            sql.AppendLine("	 , TL001, TL002, TL003, TL004, TL005");
            sql.AppendLine("	 , TL006, TL007, TL008, TL009, TL010");
            sql.AppendLine("	 , TL011, TL012, TL013, TL014, TL015");
            sql.AppendLine("	 , TL016, TL017, TL018, TL019");
            sql.AppendLine(" )");

            sql.AppendLine(" SELECT @TarErpCompName AS COMPANY, @ErpCreate, @ErpUsrGP, @pToday AS CREATE_DATE, '1' AS FLAG");
            sql.AppendLine(" , @TarPrimaryID AS TL001, @NewFullID AS TL002, @pToday AS TL003, TL004, TL005");
            sql.AppendLine(" , 'N' AS TL006, TL007, TL008, 0 AS TL009, @pToday AS TL010");
            sql.AppendLine(" , '' AS TL011, 'N' AS TL012, 0 AS TL013, TL014, TL015");
            sql.AppendLine(" , TL016, TL017, TL018, TL019");
            sql.AppendLine(" FROM ##SrcDatabase##.dbo.PURTL");
            sql.AppendLine(" WHERE (TL001 = @PrimaryID) AND (TL002 = @SubID)");


            /*
            [建立查詢] PURTM
            預設值:
             TM008 分量計價 = COUNT(PURTN)
             TM011 確認碼 = 'N'
            */
            sql.AppendLine(" INSERT INTO ##TarDatabase##.dbo.PURTM (");
            sql.AppendLine("	 COMPANY, CREATOR, USR_GROUP, CREATE_DATE, FLAG");
            sql.AppendLine("	 , TM001, TM002, TM003, TM004, TM005");
            sql.AppendLine("	 , TM006, TM007, TM008, TM009, TM010");
            sql.AppendLine("	 , TM011, TM012, TM013, TM014, TM015");
            sql.AppendLine("	 , TM016, TM017, TM018, TM019, TM020");
            sql.AppendLine("	 , TM021, TM022, TM023, TM024");
            sql.AppendLine(" )");
            sql.AppendLine(" SELECT @TarErpCompName AS COMPANY, @ErpCreate, @ErpUsrGP, @pToday AS CREATE_DATE, '1' AS FLAG");
            sql.AppendLine(" , @TarPrimaryID AS TM001, @NewFullID AS TM002, TM003, TM004, TM005");
            sql.AppendLine(" , TM006, TM007");
            sql.AppendLine(" , (CASE WHEN (SELECT COUNT(*) FROM ##SrcDatabase##.dbo.PURTN WHERE TN001 = TM001 AND TN002 = TM002 AND TN003 = TM003) > 0 THEN 'Y' ELSE 'N' END) AS TM008");
            sql.AppendLine(" , TM009, TM010");
            sql.AppendLine(" , 'N' AS TM011, TM012, TM013, @validDate, @invalidDate");
            sql.AppendLine(" , TM016, TM017, TM018, TM019, TM020");
            sql.AppendLine(" , TM021, TM022, TM023, TM024");
            sql.AppendLine(" FROM ##SrcDatabase##.dbo.PURTM");
            sql.AppendLine(" WHERE (TM001 = @PrimaryID) AND (TM002 = @SubID)");


            /*
            [建立查詢] PURTN
            */
            sql.AppendLine(" INSERT INTO ##TarDatabase##.dbo.PURTN (");
            sql.AppendLine("	 COMPANY, CREATOR, USR_GROUP, CREATE_DATE, FLAG");
            sql.AppendLine("	 , TN001, TN002, TN003");
            sql.AppendLine("	 , TN007, TN008, TN009, TN010");
            sql.AppendLine("	 , TN011, TN012, TN013, TN014, TN015, TN016");
            sql.AppendLine(" )");
            sql.AppendLine(" SELECT @TarErpCompName AS COMPANY, @ErpCreate, @ErpUsrGP, @pToday AS CREATE_DATE, '1' AS FLAG");
            sql.AppendLine(" , @TarPrimaryID AS TN001, @NewFullID AS TN002, TN003");
            sql.AppendLine(" , TN007, TN008, TN009, TN010");
            sql.AppendLine(" , TN011, TN012, TN013, TN014, TN015, TN016");
            sql.AppendLine(" FROM ##SrcDatabase##.dbo.PURTN");
            sql.AppendLine(" WHERE (TN001 = @PrimaryID) AND (TN002 = @SubID)");

            
            //return
            return sql;

        }


        /// <summary>
        /// SQL - 報價單
        /// </summary>
        /// <param name="PrimaryID"></param>
        /// <param name="SubID"></param>
        /// <param name="SrcCompanyID"></param>
        /// <param name="TarCompanyID"></param>
        /// <param name="TarPrimaryID"></param>
        /// <returns></returns>
        private StringBuilder SQLPriceB(string PrimaryID, string SubID, string SrcCompanyID
           , string TarCompanyID, string TarPrimaryID)
        {
            StringBuilder sql = new StringBuilder();

            /* 今天的日期(yyyyMMdd) */
            sql.AppendLine(" DECLARE @pToday AS VARCHAR(8)");
            sql.AppendLine(" SET @pToday = CONVERT(VARCHAR(8), GETDATE() ,112)");

            /* 取號:PK190414001 (PK+yyMMdd+流水號3碼) */
            sql.AppendLine(" DECLARE @NewID AS INT, @NewFullID AS VARCHAR(11), @FstID AS VARCHAR(2), @SecID AS VARCHAR(6)");
            sql.AppendLine(" SET @FstID = 'PK'");
            sql.AppendLine(" SET @SecID = RIGHT(@pToday, 6)");
            sql.AppendLine(" SET @NewID = (");
            sql.AppendLine(" 	SELECT ISNULL(MAX(CAST(RIGHT(TA002, 3) AS INT)), 0) + 1");
            sql.AppendLine(" 	FROM ##TarDatabase##.dbo.COPTA");
            sql.AppendLine(" 	WHERE (TA001 = @TarPrimaryID) AND (LEFT(TA002, 2) = @FstID) AND (SUBSTRING(TA002, 3, 6) = @SecID)");
            sql.AppendLine(" )");
            sql.AppendLine(" SET @NewFullID = @FstID + @SecID + RIGHT('00' + CAST(@NewID AS VARCHAR), 3)");


            /*
            [建立查詢] COPTA
            預設值:
             TA003 = Today
             TA013 = Today
             TA015 確認者 = ''
             TA018 列印次數 = 0
             TA019 確認碼 = 'N'
             TA029 簽核狀態碼 = 'N'
             TA031 傳送次數 = 0
            */
            sql.AppendLine(" INSERT INTO ##TarDatabase##.dbo.COPTA (");
            sql.AppendLine("	 COMPANY, CREATOR, USR_GROUP, CREATE_DATE, FLAG");
            sql.AppendLine("	 , TA001, TA002, TA003, TA004, TA005");
            sql.AppendLine("	 , TA006, TA007, TA008, TA009, TA010");
            sql.AppendLine("	 , TA011, TA012, TA013, TA014, TA015");
            sql.AppendLine("	 , TA016, TA017, TA018, TA019, TA020");
            sql.AppendLine("	 , TA021, TA022, TA023, TA024, TA025");
            sql.AppendLine("	 , TA026, TA027, TA028, TA029, TA030");
            sql.AppendLine("	 , TA031, TA032, TA033, TA034, TA035");
            sql.AppendLine("	 , TA036, TA037, TA038, TA039, TA040");
            sql.AppendLine("	 , TA041, TA042, TA043, TA044, TA045");
            sql.AppendLine("	 , TA046, TA047, TA048, TA049, TA050");
            sql.AppendLine("	 , TA051, TA052, TA053, TA054, TA055");
            sql.AppendLine("	 , TA056, TA057, TA058, TA059, TA060");
            sql.AppendLine("	 , TA061, TA062, TA063, TA200, TA201");
            sql.AppendLine(" )");

            sql.AppendLine(" SELECT @TarErpCompName AS COMPANY, @ErpCreate, @ErpUsrGP, @pToday AS CREATE_DATE, '1' AS FLAG");
            sql.AppendLine(" , @TarPrimaryID AS TA001, @NewFullID AS TA002, @pToday AS  TA003, TA004, TA005");
            sql.AppendLine(" , TA006, TA007, TA008, TA009, TA010");
            sql.AppendLine(" , TA011, TA012, @pToday AS TA013, TA014, '' AS TA015");
            sql.AppendLine(" , TA016, TA017, 0 AS TA018, 'N' AS TA019, TA020");
            sql.AppendLine(" , TA021, TA022, TA023, TA024, TA025");
            sql.AppendLine(" , TA026, TA027, TA028, 'N' AS TA029, TA030");
            sql.AppendLine(" , 0 AS TA031, TA032, TA033, TA034, TA035");
            sql.AppendLine(" , TA036, TA037, TA038, TA039, TA040");
            sql.AppendLine(" , TA041, TA042, TA043, TA044, TA045");
            sql.AppendLine(" , TA046, TA047, TA048, TA049, TA050");
            sql.AppendLine(" , TA051, TA052, TA053, TA054, TA055");
            sql.AppendLine(" , TA056, TA057, TA058, TA059, TA060");
            sql.AppendLine(" , TA061, TA062, TA063, TA200, TA201");
            sql.AppendLine(" FROM ##SrcDatabase##.dbo.COPTA");
            sql.AppendLine(" WHERE (TA001 = @PrimaryID) AND (TA002 = @SubID)");


            /*
            [建立查詢] COPTB
            預設值:
             TB011 確認碼 = 'N'
             TB013 分量計價 = COUNT(COPTK)
            */
            sql.AppendLine(" INSERT INTO ##TarDatabase##.dbo.COPTB (");
            sql.AppendLine("	 COMPANY, CREATOR, USR_GROUP, CREATE_DATE, FLAG");
            sql.AppendLine("	 , TB001, TB002, TB003, TB004, TB005");
            sql.AppendLine("	 , TB006, TB007, TB008, TB009, TB010");
            sql.AppendLine("	 , TB011, TB012, TB013, TB014, TB015");
            sql.AppendLine("	 , TB016, TB017, TB018, TB019, TB020");
            sql.AppendLine("	 , TB021, TB022, TB023, TB024, TB025");
            sql.AppendLine("	 , TB026, TB027, TB028, TB029, TB030");
            sql.AppendLine("	 , TB031, TB032, TB033, TB034, TB035");
            sql.AppendLine("	 , TB036, TB037, TB038, TB039, TB040");
            sql.AppendLine("	 , TB041, TB042, TB200, TB207, TB208");
            sql.AppendLine("	 , TB209, TB210");
            sql.AppendLine(" )");
            sql.AppendLine(" SELECT @TarErpCompName AS COMPANY, @ErpCreate, @ErpUsrGP, @pToday AS CREATE_DATE, '1' AS FLAG");
            sql.AppendLine("	 , @TarPrimaryID AS TB001, @NewFullID AS TB002, TB003, TB004, TB005");
            sql.AppendLine("	 , TB006, TB007, TB008, TB009, TB010");
            sql.AppendLine("	 , 'N' AS TB011, TB012");
            sql.AppendLine("     , (CASE WHEN (SELECT COUNT(*) FROM ##SrcDatabase##.dbo.COPTK WHERE TK001 = TB001 AND TK002 = TB002 AND TK003 = TB003) > 0 THEN 'Y' ELSE 'N' END) AS TB013");
            sql.AppendLine("	 , TB014, TB015");
            sql.AppendLine("	 , @validDate, @invalidDate, TB018, TB019, TB020");
            sql.AppendLine("	 , TB021, TB022, TB023, TB024, TB025");
            sql.AppendLine("	 , TB026, TB027, TB028, TB029, TB030");
            sql.AppendLine("	 , TB031, TB032, TB033, TB034, TB035");
            sql.AppendLine("	 , TB036, TB037, TB038, TB039, TB040");
            sql.AppendLine("	 , TB041, TB042, TB200, TB207, TB208");
            sql.AppendLine("	 , TB209, TB210");
            sql.AppendLine(" FROM ##SrcDatabase##.dbo.COPTB");
            sql.AppendLine(" WHERE (TB001 = @PrimaryID) AND (TB002 = @SubID)");


            /*
            [建立查詢] COPTK
            */
            sql.AppendLine(" INSERT INTO ##TarDatabase##.dbo.COPTK (");
            sql.AppendLine("	 COMPANY, CREATOR, USR_GROUP, CREATE_DATE, FLAG");
            sql.AppendLine("	 , TK001, TK002, TK003, TK004, TK005");
            sql.AppendLine("	 , TK006, TK007, TK008, TK009, TK010");
            sql.AppendLine("	 , TK011, TK012, TK013, TK014");
            sql.AppendLine(" )");
            sql.AppendLine(" SELECT @TarErpCompName AS COMPANY, @ErpCreate, @ErpUsrGP, @pToday AS CREATE_DATE, '1' AS FLAG");
            sql.AppendLine(" , @TarPrimaryID AS TK001, @NewFullID AS TK002, TK003, TK004, TK005");
            sql.AppendLine("	 , TK006, TK007, TK008, TK009, TK010");
            sql.AppendLine("	 , TK011, TK012, TK013, TK014");
            sql.AppendLine(" FROM ##SrcDatabase##.dbo.COPTK");
            sql.AppendLine(" WHERE (TK001 = @PrimaryID) AND (TK002 = @SubID)");



            //return
            return sql;

        }

        #endregion

    }
}
