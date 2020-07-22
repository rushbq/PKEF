using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LinqToExcel;
using PKLib_Method.Methods;


/*
 * COPMG檢查重複
 */
namespace ERP_CheckModel.Controllers
{

    public class ERP_CheckProdDataRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="dbs">TW/SH</param>
        /// <param name="custID">客編</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetList(string dbs, string custID, out string ErrMsg)
        {
            ErrMsg = "";

            try
            {
                //----- 宣告 -----
                StringBuilder sql = new StringBuilder();

                /* 設定DB Name */
                string SrcDatabase;
                //來源DB
                switch (dbs.ToUpper())
                {
                    case "SH":
                        SrcDatabase = "SHPK2";
                        break;

                    default:
                        SrcDatabase = "prokit2";
                        break;
                }


                //----- 資料查詢 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" SELECT MG001 AS CustID, RTRIM(MG002) AS PKModel, RTRIM(MG003) AS CustModel, RTRIM(MG005) AS ModelName");
                    sql.AppendLine(" FROM ##SrcDatabase##.dbo.COPMG");
                    sql.AppendLine(" WHERE (UPPER(MG001) = UPPER(@CustID)) AND MG003 IN (");
                    sql.AppendLine(" 	SELECT RTRIM(MG003) AS CustModel");
                    sql.AppendLine(" 	FROM ##SrcDatabase##.dbo.COPMG");
                    sql.AppendLine(" 	WHERE (UPPER(MG001) = UPPER(@CustID))");
                    sql.AppendLine(" 	GROUP BY MG003");
                    sql.AppendLine(" 	HAVING COUNT(*) > 1");
                    sql.AppendLine(" )");

                    //Replace DB 前置詞
                    sql.Replace("##SrcDatabase##", SrcDatabase);

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("CustID", custID);


                    //----- 資料取得 -----
                    return dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg);
                }



            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }



        #endregion


    }
}
