using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AssetData.Models;
using PKLib_Method.Methods;

/*
 * [資產管理]
 * 目前使用:myAsset
*/
namespace AssetData.Controllers
{
    public class AssetRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// 取得不分頁的清單
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dbs"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetOne_AssetList(Dictionary<string, string> search, string dbs, out string ErrMsg)
        {
            int DataCnt = 0;
            return Get_AssetList(search, dbs, 0, 0, false, out  DataCnt, out ErrMsg);
        }

        /// <summary>
        /// 取得已設定的清單
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="dbs">TW/SH</param>
        /// <param name="startRow">StartRow(從0開始)</param>
        /// <param name="endRow">RecordsPerPage</param>
        /// <param name="doPaging">是否分頁</param>
        /// <param name="DataCnt">傳址參數(資料總筆數)</param>
        /// <param name="ErrMsg"></param>
        /// <returns>DataTable</returns>
        public DataTable Get_AssetList(Dictionary<string, string> search, string dbs
            , int startRow, int endRow, bool doPaging
            , out int DataCnt, out string ErrMsg)
        {
            ErrMsg = "";
            string AllErrMsg = "";

            try
            {
                /* 開始/結束筆數計算 */
                int cntStartRow = startRow + 1;
                int cntEndRow = startRow + endRow;

                //----- 宣告 -----
                StringBuilder sql = new StringBuilder(); //SQL語法容器
                List<SqlParameter> sqlParamList = new List<SqlParameter>(); //SQL參數容器
                List<SqlParameter> sqlParamList_Cnt = new List<SqlParameter>(); //SQL參數容器
                DataTable myDT = new DataTable();
                DataCnt = 0;    //資料總數


                #region >> 主要資料SQL查詢 <<

                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string mainSql = @"
                    SELECT TbAll.*
                    FROM (
                    SELECT Base.SeqNo, Base.Data_ID
                      , Base.DBS, Base.ClassLv1, Base.ClassLv2, Base.AName
                      , ClsLv1.Class_Name AS ClsLv1Name
                      , ClsLv2.Class_Name AS ClsLv2Name
                      , ISNULL(CONVERT(VARCHAR(10), Base.OnlineDate, 111), '') AS OnlineDate
                      , ISNULL(CONVERT(VARCHAR(10), Base.StartDate, 111), '') AS StartDate
                      , ISNULL(CONVERT(VARCHAR(10), Base.EndDate, 111), '') AS EndDate
                      , Base.IPAddr, Base.WebUrl, Base.Remark
                      , CONVERT(VARCHAR(20), Base.Create_Time, 120) AS Create_Time
                      , ISNULL(CONVERT(VARCHAR(20), Base.Update_Time, 120), '') AS Update_Time
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)), '') AS Create_Name
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)), '') AS Update_Name
                      , ROW_NUMBER() OVER(ORDER BY Base.SeqNo) AS RowIdx
                     FROM [PKExcel].dbo.Asset_Data Base
                      INNER JOIN [PKExcel].dbo.Asset_RefClass ClsLv1 ON Base.ClassLv1 = ClsLv1.Class_ID
                      INNER JOIN [PKExcel].dbo.Asset_RefClass ClsLv2 ON Base.ClassLv2 = ClsLv2.Class_ID
                    WHERE (Base.DBS = @dbs)";

                    //append sql
                    sql.Append(mainSql);

                    #region >> 條件組合 <<

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
                                    //指定資料編號
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    sqlParamList.Add(new SqlParameter("@DataID", item.Value));

                                    break;

                                case "ErpID":
                                    //資產編號
                                    sql.Append(" AND (Base.Data_ID IN(");
                                    sql.Append("   SELECT Parent_ID FROM [PKExcel].dbo.Asset_DataItems WHERE (Erp_ID LIKE '%' + UPPER(@ErpID) + '%')");
                                    sql.Append(" ))");

                                    sqlParamList.Add(new SqlParameter("@ErpID", item.Value));

                                    break;

                                case "sDateA":
                                    //--維護日期(起)
                                    sql.Append(" AND (Base.StartDate >= @sDateA)");

                                    sqlParamList.Add(new SqlParameter("@sDateA", item.Value));

                                    break;
                                case "eDateA":
                                    //--維護日期(起)
                                    sql.Append(" AND (Base.StartDate <= @eDateA)");

                                    sqlParamList.Add(new SqlParameter("@eDateA", item.Value));

                                    break;

                                case "sDateB":
                                    //--維護日期(訖)
                                    sql.Append(" AND (Base.EndDate >= @sDateB)");

                                    sqlParamList.Add(new SqlParameter("@sDateB", item.Value));

                                    break;
                                case "eDateB":
                                    //--維護日期(訖)
                                    sql.Append(" AND (Base.EndDate <= @eDateB)");

                                    sqlParamList.Add(new SqlParameter("@eDateB", item.Value));

                                    break;

                                case "ClsLv1":
                                    //--類別
                                    sql.Append(" AND (Base.ClassLv1 = @ClassLv1)");

                                    sqlParamList.Add(new SqlParameter("@ClassLv1", item.Value));

                                    break;

                                case "ClsLv2":
                                    //--用途
                                    sql.Append(" AND (Base.ClassLv2 = @ClassLv2)");

                                    sqlParamList.Add(new SqlParameter("@ClassLv2", item.Value));

                                    break;
                            }
                        }
                    }
                    #endregion

                    //Sql尾段
                    sql.AppendLine(") AS TbAll");

                    //是否分頁
                    if (doPaging)
                    {
                        sql.AppendLine(" WHERE (TbAll.RowIdx >= @startRow) AND (TbAll.RowIdx <= @endRow)");

                        sqlParamList.Add(new SqlParameter("@startRow", cntStartRow));
                        sqlParamList.Add(new SqlParameter("@endRow", cntEndRow));

                    }
                    sql.AppendLine(" ORDER BY TbAll.RowIdx");


                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.Clear();

                    //----- SQL 固定參數 -----
                    sqlParamList.Add(new SqlParameter("@dbs", dbs));

                    //----- SQL 參數陣列 -----
                    cmd.Parameters.AddRange(sqlParamList.ToArray());

                    //Execute
                    myDT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg);
                    AllErrMsg += ErrMsg;
                }

                #endregion


                #region >> 資料筆數SQL查詢 <<
                using (SqlCommand cmdCnt = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.Clear();

                    string mainSql = @"
                    SELECT COUNT(Base.SeqNo) AS TotalCnt
                     FROM [PKExcel].dbo.Asset_Data Base
                      INNER JOIN [PKExcel].dbo.Asset_RefClass ClsLv1 ON Base.ClassLv1 = ClsLv1.Class_ID
                      INNER JOIN [PKExcel].dbo.Asset_RefClass ClsLv2 ON Base.ClassLv2 = ClsLv2.Class_ID
                    WHERE (Base.DBS = @dbs)";

                    //append
                    sql.Append(mainSql);


                    #region >> 條件組合 <<

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
                                    //指定資料編號
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@DataID", item.Value));

                                    break;

                                case "ErpID":
                                    //資產編號
                                    sql.Append(" AND (Base.Data_ID IN(");
                                    sql.Append("   SELECT Parent_ID FROM [PKExcel].dbo.Asset_DataItems WHERE (Erp_ID LIKE '%' + UPPER(@ErpID) + '%')");
                                    sql.Append(" ))");

                                    sqlParamList_Cnt.Add(new SqlParameter("@ErpID", item.Value));

                                    break;

                                case "sDateA":
                                    //--維護日期(起)
                                    sql.Append(" AND (Base.StartDate >= @sDateA)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@sDateA", item.Value));

                                    break;
                                case "eDateA":
                                    //--維護日期(起)
                                    sql.Append(" AND (Base.StartDate <= @eDateA)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@eDateA", item.Value));

                                    break;


                                case "sDateB":
                                    //--維護日期(訖)
                                    sql.Append(" AND (Base.EndDate >= @sDateB)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@sDateB", item.Value));

                                    break;
                                case "eDateB":
                                    //--維護日期(訖)
                                    sql.Append(" AND (Base.EndDate <= @eDateB)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@eDateB", item.Value));

                                    break;

                                case "ClsLv1":
                                    //--類別
                                    sql.Append(" AND (Base.ClassLv1 = @ClassLv1)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@ClassLv1", item.Value));

                                    break;

                                case "ClsLv2":
                                    //--用途
                                    sql.Append(" AND (Base.ClassLv2 = @ClassLv2)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@ClassLv2", item.Value));

                                    break;
                            }
                        }

                    }
                    #endregion


                    //----- SQL 執行 -----
                    cmdCnt.CommandText = sql.ToString();
                    cmdCnt.Parameters.Clear();

                    //----- SQL 固定參數 -----
                    //公司別(TW/SH)
                    sqlParamList_Cnt.Add(new SqlParameter("@dbs", dbs));

                    //----- SQL 參數陣列 -----
                    cmdCnt.Parameters.AddRange(sqlParamList_Cnt.ToArray());

                    //Execute
                    using (DataTable DTCnt = dbConn.LookupDT(cmdCnt, out ErrMsg))
                    {
                        //資料總筆數
                        if (DTCnt.Rows.Count > 0)
                        {
                            DataCnt = Convert.ToInt32(DTCnt.Rows[0]["TotalCnt"]);
                        }
                    }
                    AllErrMsg += ErrMsg;

                    //*** 在SqlParameterCollection同個循環內不可有重複的SqlParam,必須清除才能繼續使用. ***
                    cmdCnt.Parameters.Clear();
                }
                #endregion

                //return
                if (!string.IsNullOrWhiteSpace(AllErrMsg)) ErrMsg = AllErrMsg;

                //回傳集合
                return myDT;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// 取得單身資料
        /// </summary>
        /// <param name="parentID">單頭編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetDT_Asset(string parentID, out string ErrMsg)
        {
            ErrMsg = "";

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string sql = @"
                    SELECT Rel.Data_ID
                    , ISNULL(Prof.Display_Name, '-') AS Who
                    , RTRIM(Base.MB001) AS ID
                    , RTRIM(Base.MB002) AS Label
                    , RTRIM(Base.MB003) AS Spec
                    , Base.MB008 AS SupName
                    , Base.MB016 AS GetItemDate
                    , Base.MB019 AS GetItemMoney
                     FROM [prokit2].dbo.ASTMB Base WITH(NOLOCK)
                      INNER JOIN [prokit2].dbo.ASTMC DT WITH(NOLOCK) ON Base.MB001 = DT.MC001
                      INNER JOIN [PKExcel].dbo.Asset_DataItems Rel ON UPPER(Rel.Erp_ID) COLLATE Chinese_Taiwan_Stroke_BIN = UPPER(Base.MB001)
                      LEFT JOIN [PKSYS].dbo.User_Profile Prof WITH(NOLOCK) ON DT.MC003 = Prof.ERP_UserID COLLATE Chinese_Taiwan_Stroke_BIN
                    WHERE (Rel.Parent_ID = @parentID)";

                    //----- SQL 執行 -----
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("parentID", parentID);

                    //return
                    return dbConn.LookupDT(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// 取得類別
        /// </summary>
        /// <param name="clsType">A:類別/B:用途</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetClass_Asset(string clsType, out string ErrMsg)
        {
            //----- 宣告 -----
            ErrMsg = "";

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string sql = @"
                        SELECT Class_ID AS ID, Class_Name AS Label
                        FROM [PKExcel].dbo.Asset_RefClass
                        WHERE (Class_Type = @clsType) AND (Display = 'Y')
                        ORDER BY Sort, Class_ID";

                    //----- SQL 執行 -----
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("clsType", clsType);

                    //return
                    return dbConn.LookupDT(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }
        #endregion



        #region -----// Create //-----

        /// <summary>
        /// 建立基本資料
        /// </summary>
        /// <param name="dbs">TW/SH</param>
        /// <param name="inst"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public string Create_AssetBase(string dbs, AssetBase inst, out string ErrMsg)
        {
            //Get Guid
            string _guid = CustomExtension.GetGuid();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    INSERT INTO [PKExcel].dbo.Asset_Data(
                        Data_ID, DBS, ClassLv1, ClassLv2
                        , AName, OnlineDate, StartDate, EndDate
                        , IPAddr, WebUrl, Remark
                        , Create_Who, Create_Time
                    ) VALUES (
                        @Data_ID, @DBS, @ClassLv1, @ClassLv2
                        , @AName, @OnlineDate, @StartDate, @EndDate
                        , @IPAddr, @WebUrl, @Remark
                        , @Who, GETDATE()
                    )";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("Data_ID", _guid);
                cmd.Parameters.AddWithValue("DBS", dbs);
                cmd.Parameters.AddWithValue("ClassLv1", inst.ClassLv1);
                cmd.Parameters.AddWithValue("ClassLv2", inst.ClassLv2);
                cmd.Parameters.AddWithValue("AName", inst.AName);
                cmd.Parameters.AddWithValue("OnlineDate", string.IsNullOrWhiteSpace(inst.OnlineDate) ? (object)DBNull.Value : inst.OnlineDate);
                cmd.Parameters.AddWithValue("StartDate", string.IsNullOrWhiteSpace(inst.StartDate) ? (object)DBNull.Value : inst.StartDate);
                cmd.Parameters.AddWithValue("EndDate", string.IsNullOrWhiteSpace(inst.EndDate) ? (object)DBNull.Value : inst.EndDate);
                cmd.Parameters.AddWithValue("IPAddr", inst.IPAddr);
                cmd.Parameters.AddWithValue("WebUrl", inst.WebUrl);
                cmd.Parameters.AddWithValue("Remark", inst.Remark);
                cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);


                if (dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    return _guid;
                }
                else
                {
                    return "";
                }
            }

        }

        /// <summary>
        /// 建立單身資料
        /// </summary>
        /// <param name="parentID">單頭編號</param>
        /// <param name="erpID">ERP ID</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 排除重複後新增
        /// </remarks>
        public bool Create_AssetItem(string parentID, string erpID, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    DECLARE @NewID AS INT
                    SET @NewID = (SELECT ISNULL(MAX(Data_ID), 0) + 1 FROM [PKExcel].dbo.Asset_DataItems WHERE (Parent_ID = @Parent_ID))
                    IF (SELECT COUNT(*) FROM [PKExcel].dbo.Asset_DataItems WHERE (Parent_ID = @Parent_ID) AND (Erp_ID = @Erp_ID)) = 0
                    BEGIN
                        INSERT INTO [PKExcel].dbo.Asset_DataItems(
                            Parent_ID, Data_ID, Erp_ID
                        ) VALUES (
                            @Parent_ID, @NewID, @Erp_ID
                        );
                        UPDATE [PKExcel].dbo.Asset_Data
                        SET Update_Who = @Who, Update_Time = GETDATE();
                    END";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("Parent_ID", parentID);
                cmd.Parameters.AddWithValue("Erp_ID", erpID);
                cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion



        #region -----// Update //-----

        /// <summary>
        /// 更新基本資料
        /// </summary>
        /// <param name="id">資料編號</param>
        /// <param name="inst">INSTANCE</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_AssetBase(string id, AssetBase inst, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    UPDATE [PKExcel].dbo.Asset_Data
                    SET ClassLv1 = @ClassLv1, ClassLv2 = @ClassLv2
                    , AName = @AName, OnlineDate = @OnlineDate, StartDate = @StartDate, EndDate = @EndDate
                    , IPAddr = @IPAddr, WebUrl = @WebUrl, Remark = @Remark
                    , Update_Who = @Who, Update_Time = GETDATE()
                    WHERE (Data_ID = @Data_ID)";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("Data_ID", id);
                cmd.Parameters.AddWithValue("ClassLv1", inst.ClassLv1);
                cmd.Parameters.AddWithValue("ClassLv2", inst.ClassLv2);
                cmd.Parameters.AddWithValue("AName", inst.AName);
                cmd.Parameters.AddWithValue("OnlineDate", string.IsNullOrWhiteSpace(inst.OnlineDate) ? (object)DBNull.Value : inst.OnlineDate);
                cmd.Parameters.AddWithValue("StartDate", string.IsNullOrWhiteSpace(inst.StartDate) ? (object)DBNull.Value : inst.StartDate);
                cmd.Parameters.AddWithValue("EndDate", string.IsNullOrWhiteSpace(inst.EndDate) ? (object)DBNull.Value : inst.EndDate);
                cmd.Parameters.AddWithValue("IPAddr", inst.IPAddr);
                cmd.Parameters.AddWithValue("WebUrl", inst.WebUrl);
                cmd.Parameters.AddWithValue("Remark", inst.Remark);
                cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);

                //return
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion



        #region -----// Delete //-----

        /// <summary>
        /// 刪除所有資料
        /// </summary>
        /// <param name="dataID">資料編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Delete_AssetList(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DELETE FROM [PKExcel].dbo.Asset_DataItems WHERE (Parent_ID = @id);");
                sql.AppendLine("DELETE FROM [PKExcel].dbo.Asset_Data WHERE (Data_ID = @id);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("id", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 刪除單身資料
        /// </summary>
        /// <param name="parentID">單頭資料編號</param>
        /// <param name="dataID">單身資料編號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Delete_AssetItem(string parentID, string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DELETE FROM [PKExcel].dbo.Asset_DataItems WHERE (Parent_ID = @parentID) AND (Data_ID = @id);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("parentID", parentID);
                cmd.Parameters.AddWithValue("id", dataID);

                //return
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }
        #endregion



        #region -----// Others //-----

        /// <summary>
        /// 取資料庫名稱
        /// </summary>
        /// <param name="dbs">TW/SH/SZ</param>
        /// <returns></returns>
        //private string GetDBName(string dbs)
        //{
        //    switch (dbs.ToUpper())
        //    {
        //        case "SH":
        //            return "SHPK2";

        //        case "SZ":
        //            return "ProUnion";

        //        default:
        //            return "prokit2";
        //    }
        //}

        #endregion


    }
}
