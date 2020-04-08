using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Method.Methods;
using ProdExt.Models;

namespace ProdExt.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2
    }

    public class ProdExtRepository
    {
        public string ErrMsg;
        
        #region -----// Update & Set //-----

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool SetInvoice(ItemData baseData, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("IF (SELECT COUNT(*) FROM Prod_ExtendedInfo WHERE (Model_No = @DataID)) = 0 ");
                sql.AppendLine("BEGIN ");
                sql.AppendLine("    INSERT INTO Prod_ExtendedInfo ( ");
                sql.AppendLine("    Model_No, SafeQty_SZEC, SafeQty_A01, SafeQty_B01");
                sql.AppendLine("    , Create_Who, Create_Time ");
                sql.AppendLine("    ) VALUES ( ");
                sql.AppendLine("    @DataID, @SafeQty_SZEC, @SafeQty_A01, @SafeQty_B01");
                sql.AppendLine("    , @Create_Who, GETDATE() ");
                sql.AppendLine("    ); ");
                sql.AppendLine("END ");
                sql.AppendLine(" ELSE ");
                sql.AppendLine("BEGIN ");
                sql.AppendLine("    UPDATE Prod_ExtendedInfo ");
                sql.AppendLine("    SET SafeQty_SZEC = @SafeQty_SZEC, SafeQty_A01 = @SafeQty_A01, SafeQty_B01 = @SafeQty_B01");
                sql.AppendLine("    , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                sql.AppendLine("    WHERE (Model_No = @DataID); ");
                sql.AppendLine("END ");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Model_No);
                cmd.Parameters.AddWithValue("SafeQty_SZEC", baseData.SafeQty_SZEC);
                cmd.Parameters.AddWithValue("SafeQty_A01", baseData.SafeQty_A01);
                cmd.Parameters.AddWithValue("SafeQty_B01", baseData.SafeQty_B01);
                cmd.Parameters.AddWithValue("Create_Who", baseData.Create_Who);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }


        }


        #endregion

    }

}