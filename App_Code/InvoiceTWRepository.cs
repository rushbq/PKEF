using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Invoice.Models;
using PKLib_Method.Methods;

/*
 * [使用功能]
 * 1. 轉出匯款資料(TW)
 */
namespace Invoice.TW.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        Status = 3,
        StartDate = 4,
        EndDate = 5,
        CheckRel = 6
    }

    public class InvoiceRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        #region >> 申請單:匯款資料轉出 <<

        /// <summary>
        /// 取得費用申請單資料
        /// </summary>
        /// <param name="startDate">開始日(格式:yyyyMMdd)</param>
        /// <param name="endDate">結束日(格式:yyyyMMdd)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        public DataTable Get_FeeData(string startDate, string endDate, Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                string sql = @"
                ;WITH TblErp AS (
	                /* CorpUID = 1 = TW */
	                SELECT 
		                RTRIM(ERP.MA001) ERP_SupID, RTRIM(ERP.MA002) ERP_SupName
		                , ISNULL(Info.Data_ID, 0) AS InfoID, ISNULL(Prof.Account_Name, '') AS User_Account, ISNULL(Prof.Display_Name, '') AS User_Name
		                , RTRIM(ERP.MA003) AS tw_AccName, ERP.MA028 AS tw_Account, ERP.MA027 AS tw_BankID, Bank.MO006 AS tw_BankName
		                , Info.cn_Account, Info.cn_AccName, Info.cn_Email, Info.cn_BankName
		                , Info.cn_BankID, Info.cn_SaleID, Info.cn_State, Info.cn_City, Info.cn_BankType
		                , Info.ww_Account, Info.ww_AccName, Info.ww_Tel, Info.ww_Addr
		                , Info.ww_BankName, Info.ww_BankBranch, Info.ww_BankAddr, Info.ww_Country, Info.ww_Code
	                FROM Param_Corp Corp WITH(NOLOCK)
	                INNER JOIN Supplier_ERPData ERP WITH(NOLOCK) ON Corp.Corp_ID = RTRIM(ERP.COMPANY)
	                LEFT JOIN Supplier_ExtendedInfo Info WITH(NOLOCK) ON RTRIM(ERP.MA001) = Info.ERP_ID AND Corp.Corp_UID = Info.Corp_UID
	                LEFT JOIN User_Profile Prof WITH(NOLOCK) ON Info.Purchaser = Prof.Account_Name
	                LEFT JOIN [DSCSYS].dbo.CMSMO Bank WITH(NOLOCK) ON ERP.MA027 COLLATE Chinese_Taiwan_Stroke_BIN = Bank.MO001
	                WHERE (Corp.Display = 'Y') AND (Corp.Corp_UID = 1)
                )
                SELECT RTRIM(Base.TF001) AS PayFid
                , RTRIM(Base.TF002) AS PaySid
                , Base.TF003 AS PayDate
                , Base.TF009 AS PayPrice /*本幣總金額*/
                , Base.TF011 AS Remark
                , TblErp.ERP_SupID AS SupID, TblErp.ERP_SupName AS SupName
                /* 收款人名稱, 收款人帳號, 銀行代號, 銀行名稱*/
                , TblErp.tw_AccName, TblErp.tw_Account, TblErp.tw_BankID, TblErp.tw_BankName
                , ROW_NUMBER() OVER(ORDER BY Base.TF001, Base.TF002) AS SerialNo
                FROM [prokit2].dbo.PCMTF Base
                 LEFT JOIN TblErp ON Base.TF200 = TblErp.ERP_SupID COLLATE Chinese_Taiwan_Stroke_BIN
                WHERE (1=1) AND (Base.TF003 >= @sDate) AND (Base.TF003 <= @eDate)";

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
                                sql += " AND ((RTRIM(Base.TF001)+RTRIM(Base.TF002)) IN ({0}))".FormatThis(
                                    CustomExtension.GetSQLParam(_listVals, "params"));
                                for (int row = 0; row < _listVals.Count; row++)
                                {
                                    cmd.Parameters.AddWithValue("params" + row, _listVals[row]);
                                }

                                break;
                        }
                    }
                }
                #endregion

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("sDate", startDate);
                cmd.Parameters.AddWithValue("eDate", endDate);

                //----- 資料取得 -----
                return dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }

        }

        /// <summary>
        /// 取得費用申請單匯款資料(已勾選)
        /// </summary>
        /// <param name="traceID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable Get_FeeData_DT(string traceID, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                string sql = @"
                SELECT SeqNo, TraceID, PayERPID, tw_Account, tw_AccName, tw_BankID, tw_BankName
                 , PayWho, PayAcc1, PayAcc2, PayPrice
                FROM [PKEF].dbo.TW_Fee_Export
                WHERE (TraceID = @TraceID)";


                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("TraceID", traceID);

                //----- 資料取得 -----
                return dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }

        }

        #endregion


        #endregion



        #region  -----// Create //-----

        #region >> 申請單:轉入匯款資料 <<

        /// <summary>
        /// 建立暫存匯款資料
        /// </summary>
        /// <param name="_traiceID"></param>
        /// <param name="_payWho"></param>
        /// <param name="_payAcc1"></param>
        /// <param name="_payAcc2"></param>
        /// <param name="erpIDs"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_FeeData(string _traiceID, string _payWho, string _payAcc1, string _payAcc2, string erpIDs
            , out string ErrMsg)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"
                DELETE FROM [PKEF].dbo.TW_Fee_Export WHERE (TraceID = @TraceID)
                ;WITH TblErp AS (
	                /* CorpUID = 1 = TW */
	                SELECT 
		                RTRIM(ERP.MA001) ERP_SupID, RTRIM(ERP.MA002) ERP_SupName
		                , ISNULL(Info.Data_ID, 0) AS InfoID, ISNULL(Prof.Account_Name, '') AS User_Account, ISNULL(Prof.Display_Name, '') AS User_Name
		                , RTRIM(ERP.MA003) AS tw_AccName, ERP.MA028 AS tw_Account, ERP.MA027 AS tw_BankID, Bank.MO006 AS tw_BankName
		                , Info.cn_Account, Info.cn_AccName, Info.cn_Email, Info.cn_BankName
		                , Info.cn_BankID, Info.cn_SaleID, Info.cn_State, Info.cn_City, Info.cn_BankType
		                , Info.ww_Account, Info.ww_AccName, Info.ww_Tel, Info.ww_Addr
		                , Info.ww_BankName, Info.ww_BankBranch, Info.ww_BankAddr, Info.ww_Country, Info.ww_Code
	                FROM Param_Corp Corp WITH(NOLOCK)
	                INNER JOIN Supplier_ERPData ERP WITH(NOLOCK) ON Corp.Corp_ID = RTRIM(ERP.COMPANY)
	                LEFT JOIN Supplier_ExtendedInfo Info WITH(NOLOCK) ON RTRIM(ERP.MA001) = Info.ERP_ID AND Corp.Corp_UID = Info.Corp_UID
	                LEFT JOIN User_Profile Prof WITH(NOLOCK) ON Info.Purchaser = Prof.Account_Name
	                LEFT JOIN [DSCSYS].dbo.CMSMO Bank WITH(NOLOCK) ON ERP.MA027 COLLATE Chinese_Taiwan_Stroke_BIN = Bank.MO001
	                WHERE (Corp.Display = 'Y') AND (Corp.Corp_UID = 1)
                )

                INSERT INTO [PKEF].dbo.TW_Fee_Export (
                SeqNo, TraceID, PayERPID
                , tw_AccName, tw_Account, tw_BankID, tw_BankName
                , PayWho, PayAcc1, PayAcc2, PayPrice
                , Create_Who, Create_Time
                )
                SELECT
                ROW_NUMBER() OVER(ORDER BY Base.TF001, Base.TF002) AS SerialNo
                , @TraceID
                , RTRIM(Base.TF001) + RTRIM(Base.TF002) AS id
                , TblErp.tw_AccName, TblErp.tw_Account, TblErp.tw_BankID, TblErp.tw_BankName
                , @PayWho, @PayAcc1, @PayAcc2, Base.TF009
                , @Creater, GETDATE()
                FROM [prokit2].dbo.PCMTF Base
                 LEFT JOIN TblErp ON Base.TF200 = TblErp.ERP_SupID COLLATE Chinese_Taiwan_Stroke_BIN
                WHERE (1=1)";

                //將以逗號分隔的字串轉為Array
                string[] aryData = Regex.Split(erpIDs, @"\,{1}");
                ArrayList _listVals = new ArrayList(aryData);

                //GetSQLParam:SQL WHERE IN的方法
                sql += " AND (RTRIM(Base.TF001)+RTRIM(Base.TF002)) IN ({0})".FormatThis(CustomExtension.GetSQLParam(_listVals, "params"));
                for (int row = 0; row < _listVals.Count; row++)
                {
                    cmd.Parameters.AddWithValue("params" + row, _listVals[row]);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 60;   //單位:秒
                cmd.Parameters.AddWithValue("TraceID", _traiceID);
                cmd.Parameters.AddWithValue("PayWho", _payWho);
                cmd.Parameters.AddWithValue("PayAcc1", _payAcc1);
                cmd.Parameters.AddWithValue("PayAcc2", _payAcc2);
                cmd.Parameters.AddWithValue("Creater", fn_Params.UserGuid);

                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }

        }
        #endregion

        #endregion



        #region -----// Update //-----



        #endregion



        #region -----// Delete //-----

        #region >> 申請單:轉出匯款資料 <<
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_FeeData(string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                sql.AppendLine(" DELETE FROM TW_Fee_Export WHERE (TraceID = @TraceID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("TraceID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }
        #endregion


        #endregion


    }

}