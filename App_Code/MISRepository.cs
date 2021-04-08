using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MISData.Models;
using PKLib_Method.Methods;


namespace MISData.Controllers
{
    public class MISRepository
    {
        public string ErrMsg;

        #region -----// Read //-----


        #region *** 網站功能點擊 S ***
        /// <summary>
        /// 取得選單點擊記錄 - Tree
        /// </summary>
        /// <param name="dbID">網站編號</param>
        /// <param name="_year">年份(from 2020)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<WebMenu> Get_WebClickList(string dbID, string _year, string _sDate, string _eDate, out string ErrMsg)
        {
            //Declare
            ErrMsg = "";
            List<WebMenu> dataList = new List<WebMenu>();

            try
            {
                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string sql = CheckSQL(dbID);

                    //replace
                    sql = sql.Replace("##year##", _year.ToString())
                            .Replace("##sDate##", _sDate)
                            .Replace("##eDate##", _eDate);


                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("dbID", dbID);


                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            string _menuName = item.Field<int>("Level").Equals(1)
                                    ? "【{0}】".FormatThis(item.Field<string>("MenuName"))
                                    : item.Field<string>("MenuName");

                            int _clickCnt = item.Field<int>("ClickCnt");

                            //加入項目
                            var data = new WebMenu
                            {
                                MenuID = item.Field<int>("MenuID"),
                                ParentID = item.Field<int>("ParentID"),
                                MenuName = _menuName,
                                ClickCnt = _clickCnt,
                                Remark = item.Field<string>("Remark")
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
                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// 判斷網站取得對應的SQL - Tree
        /// </summary>
        /// <param name="dbID">指定編號</param>
        /// <returns></returns>
        /// <remarks>
        /// 1:ProductCenter, 2:PKHome, 3:PKEF, 4:PKReport
        /// </remarks>
        private string CheckSQL(string dbID)
        {
            string sql = "";

            switch (dbID)
            {
                case "1":
                    //ProductCenter
                    sql = @"
                        ;WITH TblBase AS (
	                        SELECT Base.Prog_ID AS MenuID, Base.Up_Id AS ParentID, Base.Prog_Name_zh_TW AS MenuName
	                        , Base.lv Level
	                        , COUNT(Ref.Menu_ID) AS ClickCnt
	                        FROM [ProductCenter].dbo.Program Base WITH (NOLOCK)
	                         LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON Base.Prog_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 	                          AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
	                        WHERE (Base.Display = 'Y')
	                        GROUP BY Base.Prog_ID, Base.Up_Id, Base.Prog_Name_zh_TW, Base.lv
                        )
                        SELECT TblBase.*
                         , menu.Sort, ISNULL(menu.Remark, '') AS Remark
                        FROM [ProductCenter].dbo.Program menu
                         INNER JOIN TblBase ON menu.Prog_ID = TblBase.MenuID
                        WHERE (menu.lv < 5)
                        ORDER BY menu.Sort, menu.Prog_ID";

                    break;


                case "2":
                    //PKHome
                    sql = @"
                        ;WITH TblBase AS (
	                        SELECT Base.Menu_ID AS MenuID, Base.Parent_ID AS ParentID, Base.MenuName_zh_TW AS MenuName
	                        , Base.Menu_Level Level
	                        , COUNT(Ref.Menu_ID) AS ClickCnt
	                        FROM [PKSYS].dbo.Home_Menu Base WITH (NOLOCK)
	                         LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON Base.Menu_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 	                          AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
	                        WHERE (Base.Display = 'Y')
	                        GROUP BY Base.Menu_ID, Base.Parent_ID, Base.MenuName_zh_TW, Base.Menu_Level
                        )
                        SELECT TblBase.*
                         , menu.Sort, ISNULL(menu.Remark, '') AS Remark
                        FROM [PKSYS].dbo.Home_Menu menu
                         INNER JOIN TblBase ON menu.Menu_ID = TblBase.MenuID
                        WHERE (menu.Menu_Level < 5)
                        ORDER BY menu.Sort, menu.Menu_ID";

                    break;


                case "3":
                    //PKEF
                    sql = @"
                        ;WITH TblBase AS (
	                        SELECT Base.Prog_ID AS MenuID, Base.Up_Id AS ParentID, Base.Prog_Name_zh_TW AS MenuName
	                        , Base.lv Level
	                        , COUNT(Ref.Menu_ID) AS ClickCnt
	                        FROM [PKEF].dbo.Program Base WITH (NOLOCK)
	                         LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON Base.Prog_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 	                          AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
	                        WHERE (Base.Display = 'Y') AND (Base.MenuDisplay = 'PKEF')
	                        GROUP BY Base.Prog_ID, Base.Up_Id, Base.Prog_Name_zh_TW, Base.lv
                        )
                        SELECT TblBase.*
                         , menu.Sort, ISNULL(menu.Remark, '') AS Remark
                        FROM [PKEF].dbo.Program menu
                         INNER JOIN TblBase ON menu.Prog_ID = TblBase.MenuID
                        WHERE (menu.lv < 5)
                        ORDER BY menu.Sort, menu.Prog_ID";

                    break;

                case "4":
                    //PKReport
                    sql = @"
                        ;WITH TblBase AS (
	                        SELECT Base.Prog_ID AS MenuID, Base.Up_Id AS ParentID, Base.Prog_Name_zh_TW AS MenuName
	                        , Base.lv Level
	                        , COUNT(Ref.Menu_ID) AS ClickCnt
	                        FROM [PKReport].dbo.Program Base WITH (NOLOCK)
	                         LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON Base.Prog_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 	                          AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
	                        WHERE (Base.Display = 'Y')
	                        GROUP BY Base.Prog_ID, Base.Up_Id, Base.Prog_Name_zh_TW, Base.lv
                        )
                        SELECT TblBase.*
                         , menu.Sort, ISNULL(menu.Remark, '') AS Remark
                        FROM [PKReport].dbo.Program menu
                         INNER JOIN TblBase ON menu.Prog_ID = TblBase.MenuID
                        WHERE (menu.lv < 5)
                        ORDER BY menu.Sort, menu.Prog_ID";

                    break;
            }

            return sql;
        }


        /// <summary>
        /// 取得選單點擊記錄 - Table
        /// </summary>
        /// <param name="dbID">網站編號</param>
        /// <param name="_year">年份(from 2020)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<WebMenuTable> Get_WebClickList_byTable(string dbID, string _year, string _sDate, string _eDate, out string ErrMsg)
        {
            //Declare
            ErrMsg = "";
            List<WebMenuTable> dataList = new List<WebMenuTable>();

            try
            {
                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string sql = CheckSQL_byTable(dbID);

                    //replace
                    sql = sql.Replace("##year##", _year.ToString())
                            .Replace("##sDate##", _sDate)
                            .Replace("##eDate##", _eDate);


                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.AddWithValue("dbID", dbID);


                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new WebMenuTable
                            {
                                MenuID = item.Field<int>("MenuID"),
                                Lv1Name = item.Field<string>("Lv1Name"),
                                Lv2Name = item.Field<string>("Lv2Name"),
                                Lv3Name = item.Field<string>("Lv3Name"),
                                Lv4Name = item.Field<string>("Lv4Name"),
                                ClickCnt = item.Field<int>("ClickCnt"),
                                Lv = item.Field<int>("Lv"),
                                Remark = item.Field<string>("Remark")
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
                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// 判斷網站取得對應的SQL - Table
        /// </summary>
        /// <param name="dbID">指定編號</param>
        /// <returns></returns>
        /// <remarks>
        /// 1:ProductCenter, 2:PKHome, 3:PKEF, 4:PKReport
        /// </remarks>
        private string CheckSQL_byTable(string dbID)
        {
            string sql = "";

            switch (dbID)
            {
                case "1":
                    //ProductCenter
                    sql = @"
;WITH TblBase AS (
 SELECT Prog_ID, Prog_Name_zh_TW AS ProgName, Prog_Link, Up_Id, Lv, Sort, Remark
 FROM [ProductCenter].dbo.Program
 WHERE (Display = 'Y')
)
, tblCTE AS (
 SELECT TblBase.*
 , (CASE WHEN ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID)) END)
   AS rn
 FROM TblBase
 WHERE (Lv =1)
 
 UNION ALL
 
 SELECT nextLv.*
 , rn + '/' + 
   (CASE WHEN ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID)) END)    
 FROM TblBase nextLv JOIN tblCTE ON nextLv.Up_Id = tblCTE.Prog_ID
)
SELECT tblCTE.Prog_ID AS MenuID
, (CASE WHEN tblCTE.Lv = 1 THEN tblCTE.ProgName ELSE '' END) AS Lv1Name
, (CASE WHEN tblCTE.Lv = 2 THEN tblCTE.ProgName ELSE '' END) AS Lv2Name
, (CASE WHEN tblCTE.Lv = 3 THEN tblCTE.ProgName ELSE '' END) AS Lv3Name
, (CASE WHEN tblCTE.Lv = 4 THEN tblCTE.ProgName ELSE '' END) AS Lv4Name
, tblCTE.Lv
, ISNULL(tblCTE.Remark, '') AS Remark
, COUNT(Ref.Menu_ID) AS ClickCnt
FROM tblCTE
 LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON tblCTE.Prog_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
GROUP BY tblCTE.Prog_ID, tblCTE.Lv, tblCTE.ProgName, tblCTE.rn, tblCTE.Remark
ORDER BY rn";

                    break;


                case "2":
                    //PKHome
                    sql = @"
;WITH TblBase AS (
 SELECT Menu_ID, MenuName_zh_TW AS ProgName, Url, Parent_ID, Menu_Level, Sort, Remark
 FROM [PKSYS].dbo.Home_Menu
 WHERE (Display = 'Y')
)
, tblCTE AS (
 SELECT TblBase.*
 , (CASE WHEN ROW_NUMBER() OVER (ORDER BY Sort, Menu_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Menu_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Menu_ID)) END)
   AS rn
 FROM TblBase
 WHERE (Menu_Level =1)
 
 UNION ALL
 
 SELECT nextLv.*
 , rn + '/' + 
   (CASE WHEN ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Menu_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Menu_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Menu_ID)) END)    
 FROM TblBase nextLv JOIN tblCTE ON nextLv.Parent_ID = tblCTE.Menu_ID
)
SELECT tblCTE.Menu_ID AS MenuID
, (CASE WHEN tblCTE.Menu_Level = 1 THEN tblCTE.ProgName ELSE '' END) AS Lv1Name
, (CASE WHEN tblCTE.Menu_Level = 2 THEN tblCTE.ProgName ELSE '' END) AS Lv2Name
, (CASE WHEN tblCTE.Menu_Level = 3 THEN tblCTE.ProgName ELSE '' END) AS Lv3Name
, (CASE WHEN tblCTE.Menu_Level = 4 THEN tblCTE.ProgName ELSE '' END) AS Lv4Name
, tblCTE.Menu_Level AS Lv
, ISNULL(tblCTE.Remark, '') AS Remark
, COUNT(Ref.Menu_ID) AS ClickCnt
FROM tblCTE
 LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON tblCTE.Menu_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
GROUP BY tblCTE.Menu_ID, tblCTE.Menu_Level, tblCTE.ProgName, tblCTE.rn, tblCTE.Remark
ORDER BY rn
";

                    break;


                case "3":
                    //PKEF
                    sql = @"
;WITH TblBase AS (
 SELECT Prog_ID, Prog_Name_zh_TW AS ProgName, Prog_Link, Up_Id, Lv, Sort, Remark
 FROM [PKEF].dbo.Program
 WHERE (Display = 'Y') AND (MenuDisplay = 'PKEF')
)
, tblCTE AS (
 SELECT TblBase.*
 , (CASE WHEN ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID)) END)
   AS rn
 FROM TblBase
 WHERE (Lv =1)
 
 UNION ALL
 
 SELECT nextLv.*
 , rn + '/' + 
   (CASE WHEN ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID)) END)    
 FROM TblBase nextLv JOIN tblCTE ON nextLv.Up_Id = tblCTE.Prog_ID
)
SELECT tblCTE.Prog_ID AS MenuID
, (CASE WHEN tblCTE.Lv = 1 THEN tblCTE.ProgName ELSE '' END) AS Lv1Name
, (CASE WHEN tblCTE.Lv = 2 THEN tblCTE.ProgName ELSE '' END) AS Lv2Name
, (CASE WHEN tblCTE.Lv = 3 THEN tblCTE.ProgName ELSE '' END) AS Lv3Name
, (CASE WHEN tblCTE.Lv = 4 THEN tblCTE.ProgName ELSE '' END) AS Lv4Name
, tblCTE.Lv
, ISNULL(tblCTE.Remark, '') AS Remark
, COUNT(Ref.Menu_ID) AS ClickCnt
FROM tblCTE
 LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON tblCTE.Prog_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
GROUP BY tblCTE.Prog_ID, tblCTE.Lv, tblCTE.ProgName, tblCTE.rn, tblCTE.Remark
ORDER BY rn";

                    break;

                case "4":
                    //PKReport
                    sql = @"
;WITH TblBase AS (
 SELECT Prog_ID, Prog_Name_zh_TW AS ProgName, Prog_Link, Up_Id, Lv, Sort, Remark
 FROM [PKReport].dbo.Program
 WHERE (Display = 'Y')
)
, tblCTE AS (
 SELECT TblBase.*
 , (CASE WHEN ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY Sort, Prog_ID)) END)
   AS rn
 FROM TblBase
 WHERE (Lv =1)
 
 UNION ALL
 
 SELECT nextLv.*
 , rn + '/' + 
   (CASE WHEN ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID) < 10
    THEN '0' + CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID))
    ELSE CONVERT(VARCHAR(MAX), ROW_NUMBER() OVER (ORDER BY nextLv.Sort, nextLv.Prog_ID)) END)    
 FROM TblBase nextLv JOIN tblCTE ON nextLv.Up_Id = tblCTE.Prog_ID
)
SELECT tblCTE.Prog_ID AS MenuID
, (CASE WHEN tblCTE.Lv = 1 THEN tblCTE.ProgName ELSE '' END) AS Lv1Name
, (CASE WHEN tblCTE.Lv = 2 THEN tblCTE.ProgName ELSE '' END) AS Lv2Name
, (CASE WHEN tblCTE.Lv = 3 THEN tblCTE.ProgName ELSE '' END) AS Lv3Name
, (CASE WHEN tblCTE.Lv = 4 THEN tblCTE.ProgName ELSE '' END) AS Lv4Name
, tblCTE.Lv
, ISNULL(tblCTE.Remark, '') AS Remark
, COUNT(Ref.Menu_ID) AS ClickCnt
FROM tblCTE
 LEFT JOIN [PKClickLOG].dbo.MenuClick_##year## Ref ON tblCTE.Prog_ID = Ref.Menu_ID AND Ref.Menu_Zone = @dbID
 AND (Ref.ClickTime >= '##sDate## 00:00') AND (Ref.ClickTime <= '##eDate## 23:59')
GROUP BY tblCTE.Prog_ID, tblCTE.Lv, tblCTE.ProgName, tblCTE.rn, tblCTE.Remark
ORDER BY rn";

                    break;
            }

            return sql;
        }


        /// <summary>
        /// 取得點擊明細
        /// </summary>
        /// <param name="menuID">Menu編號</param>
        /// <param name="dbID">指定編號</param>
        /// <param name="_year"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 1:ProductCenter, 2:PKHome, 3:PKEF, 4:PKReport
        /// </remarks>
        public DataTable GetDT_Click(string menuID, string dbID, string _year, out string ErrMsg)
        {
            ErrMsg = "";

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string sql = @"
                    SELECT COUNT(*) AS tot, Who.Display_Name
                    FROM [PKClickLOG].dbo.MenuClick_##year## Base
                     INNER JOIN [PKSYS].dbo.User_Profile Who ON Base.User_Guid = Who.Guid
                    WHERE (Base.Menu_Zone = @dbID) AND (Base.Menu_ID = @menuID)
                    GROUP BY Who.Display_Name
                    ORDER BY 1 DESC";

                    sql = sql.Replace("##year##", _year);

                    //----- SQL 執行 -----
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("dbID", dbID);
                    cmd.Parameters.AddWithValue("menuID", menuID);

                    //return
                    return dbConn.LookupDT(cmd, out ErrMsg);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }
        #endregion *** 網站功能點擊 E ***


        #region *** 資訊需求 S ***
        /// <summary>
        /// [資訊需求] 取得不分頁的清單
        /// </summary>
        /// <param name="search"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ItHelpData> GetOne_IThelp(Dictionary<string, string> search, out string ErrMsg)
        {
            int DataCnt = 0;
            return Get_IThelpList(search, 0, 0, false, out DataCnt, out ErrMsg);
        }

        /// <summary>
        /// [資訊需求] 取得已設定的清單
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="startRow">StartRow(從0開始)</param>
        /// <param name="endRow">RecordsPerPage</param>
        /// <param name="doPaging">是否分頁</param>
        /// <param name="DataCnt">傳址參數(資料總筆數)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ItHelpData> Get_IThelpList(Dictionary<string, string> search
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
                List<ItHelpData> dataList = new List<ItHelpData>();
                DataCnt = 0;    //資料總數

                #region >> 資料筆數SQL查詢 <<
                using (SqlCommand cmdCnt = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string mainSql = @"
                    SELECT COUNT(Base.SeqNo) AS TotalCnt
                    FROM IT_Help Base
                      INNER JOIN IT_Help_ParamClass ReqClass ON Base.Req_Class = ReqClass.Class_ID
                      INNER JOIN IT_Help_ParamClass HelpStatus ON Base.Help_Status = HelpStatus.Class_ID
                      INNER JOIN [PKSYS].dbo.User_Profile Prof ON Base.Req_Who = Prof.Account_Name
                      INNER JOIN [PKSYS].dbo.User_Dept Dept ON Base.Req_Dept = Dept.DeptID
                    WHERE (1 = 1)";

                    //append
                    sql.Append(mainSql);


                    #region >> 條件組合 <<

                    if (search != null)
                    {
                        //過濾空值
                        var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));
                        string filterDateType = "Base.Create_Time";

                        //查詢內容
                        foreach (var item in thisSearch)
                        {
                            switch (item.Key)
                            {
                                case "DataID":
                                    //指定資料編號
                                    sql.Append(" AND (Base.DataID = @DataID)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@DataID", item.Value));

                                    break;

                                case "Keyword":
                                    //主旨, TraceID
                                    sql.Append(" AND (");
                                    sql.Append("   (UPPER(Base.TraceID) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("   OR (UPPER(Base.Help_Subject) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    sqlParamList_Cnt.Add(new SqlParameter("@Keyword", item.Value));

                                    break;

                                case "DateType":
                                    switch (item.Value)
                                    {
                                        case "A":
                                            filterDateType = "Base.Create_Time";
                                            break;

                                        case "B":
                                            filterDateType = "Base.Finish_Time";
                                            break;

                                        default:
                                            filterDateType = "Base.Create_Time";
                                            break;
                                    }

                                    break;

                                case "sDate":
                                    sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));
                                    sqlParamList_Cnt.Add(new SqlParameter("@sDate", item.Value + " 00:00:00"));

                                    break;
                                case "eDate":
                                    sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));
                                    sqlParamList_Cnt.Add(new SqlParameter("@eDate", item.Value + " 23:59:59"));

                                    break;

                                case "ReqClass":
                                    //--問題類別
                                    sql.Append(" AND (Base.Req_Class = @ReqClass)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@ReqClass", item.Value));

                                    break;

                                case "HelpStatus":
                                    //--處理狀態
                                    sql.Append(" AND (Base.Help_Status = @HelpStatus)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@HelpStatus", item.Value));

                                    break;

                                case "Dept":
                                    //需求部門
                                    sql.Append(" AND (Base.Req_Dept = @Dept)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@Dept", item.Value));

                                    break;

                                case "ReqWho":
                                    //需求人
                                    sql.Append(" AND (Base.Req_Who = @ReqWho)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@ReqWho", item.Value));

                                    break;

                                case "FinishWho":
                                    //結案人
                                    sql.Append(" AND (Base.Finish_Who IN (");
                                    sql.Append("   SELECT [Guid] FROM [PKSYS].dbo.User_Profile WHERE Account_Name = @FinishWho");
                                    sql.Append(" ))");

                                    sqlParamList_Cnt.Add(new SqlParameter("@FinishWho", item.Value));

                                    break;

                                case "unClose":
                                    //--未結案需求
                                    sql.Append(" AND (Base.Help_Status <> 125)");

                                    break;
                            }
                        }

                    }
                    #endregion

                    //----- SQL 執行 -----
                    cmdCnt.CommandText = sql.ToString();
                    cmdCnt.Parameters.Clear();

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


                #region >> 主要資料SQL查詢 <<
                sql.Clear();

                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string mainSql = @"
                    SELECT TbAll.*
                    FROM (
                    SELECT Base.SeqNo, Base.DataID, Base.TraceID, CONVERT(VARCHAR(10), Base.Create_Time, 111) AS CreateDay
                      , Base.Help_Subject, ISNULL(Base.Help_Content, '') Help_Content, ISNULL(Base.Help_Benefit, '') Help_Benefit
                      , Base.Apply_Type, Help_Way

                      /* 處理狀態, 需求類別 */
                      , Base.Help_Status, Base.Req_Class
                      , HelpStatus.Class_Name AS Help_StatusName, ReqClass.Class_Name AS Req_ClassName

                      /* 權限申請 */
                      , Base.IsAgree, Base.Agree_Time
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Agree_Who)), '') AS Agree_WhoName
  
                      /* 需求單位 */
                      , Base.Req_Who, Base.Req_Dept, Prof.Email AS Req_Email, ISNULL(Prof.Tel_Ext, '') AS Req_TelExt
                      , Prof.Display_Name AS Req_WhoName, ISNULL(Prof.NickName, '') AS Req_NickName, Dept.DeptName AS Req_DeptName

                      /* 驗收 */
                      , Base.IsRate, Base.RateScore, Base.RateContent
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Guid = Base.RateWho)), '') AS RateWhoName

                      /* 回覆資料 */
                      , ISNULL(Base.Reply_Content, '') Reply_Content, Base.onTopWho
                      , (CASE WHEN Base.onTopWho = @currUser THEN Base.onTop ELSE 'N' END) AS onTop
                      , Base.Finish_Hours, Base.Finish_Time, Base.Finish_Who, Base.Wish_Time
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Guid = Base.Finish_Who)), '') AS Finish_WhoName
                      /* 最新進度 */
                      , ISNULL(
                       (SELECT TOP 1 Cls.Class_Name + '_' + ISNULL(CONVERT(VARCHAR(20), DT.Proc_Time, 111), '') 
                        FROM IT_Help_DT DT
	                     INNER JOIN IT_Help_ParamClass Cls ON DT.Class_ID = Cls.Class_ID
	                    WHERE (DT.ParentID = Base.DataID) ORDER BY DT.DetailID DESC)
                      , '') AS ProcInfo
                      , CONVERT(VARCHAR(20), Base.Create_Time, 120) AS Create_Time
                      , ISNULL(CONVERT(VARCHAR(20), Base.Update_Time, 120), '') AS Update_Time
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Guid = Base.Create_Who)), '') AS Create_Name
                      , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Guid = Base.Update_Who)), '') AS Update_Name
                      , ROW_NUMBER() OVER (ORDER BY (CASE WHEN Base.onTopWho = @currUser THEN Base.onTop ELSE 'N' END) DESC, HelpStatus.Sort ASC, Base.Create_Time DESC) AS RowIdx
                    FROM IT_Help Base
                      INNER JOIN IT_Help_ParamClass ReqClass ON Base.Req_Class = ReqClass.Class_ID
                      INNER JOIN IT_Help_ParamClass HelpStatus ON Base.Help_Status = HelpStatus.Class_ID
                      INNER JOIN [PKSYS].dbo.User_Profile Prof ON Base.Req_Who = Prof.Account_Name
                      INNER JOIN [PKSYS].dbo.User_Dept Dept ON Base.Req_Dept = Dept.DeptID
                    WHERE (1 = 1)";

                    //append sql
                    sql.Append(mainSql);

                    #region >> 條件組合 <<

                    if (search != null)
                    {
                        //過濾空值
                        var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));
                        string filterDateType = "Base.Create_Time";

                        //查詢內容
                        foreach (var item in thisSearch)
                        {
                            switch (item.Key)
                            {
                                case "DataID":
                                    //指定資料編號
                                    sql.Append(" AND (Base.DataID = @DataID)");

                                    sqlParamList.Add(new SqlParameter("@DataID", item.Value));

                                    break;

                                case "Keyword":
                                    //主旨, TraceID
                                    sql.Append(" AND (");
                                    sql.Append("   (UPPER(Base.TraceID) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("   OR (UPPER(Base.Help_Subject) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    sqlParamList.Add(new SqlParameter("@Keyword", item.Value));

                                    break;


                                case "DateType":
                                    switch (item.Value)
                                    {
                                        case "A":
                                            filterDateType = "Base.Create_Time";
                                            break;

                                        case "B":
                                            filterDateType = "Base.Finish_Time";
                                            break;

                                        default:
                                            filterDateType = "Base.Create_Time";
                                            break;
                                    }

                                    break;

                                case "sDate":
                                    sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));
                                    sqlParamList.Add(new SqlParameter("@sDate", item.Value + " 00:00:00"));

                                    break;
                                case "eDate":
                                    sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));
                                    sqlParamList.Add(new SqlParameter("@eDate", item.Value + " 23:59:59"));

                                    break;


                                case "ReqClass":
                                    //--問題類別
                                    sql.Append(" AND (Base.Req_Class = @ReqClass)");

                                    sqlParamList.Add(new SqlParameter("@ReqClass", item.Value));

                                    break;

                                case "HelpStatus":
                                    //--處理狀態
                                    sql.Append(" AND (Base.Help_Status = @HelpStatus)");

                                    sqlParamList.Add(new SqlParameter("@HelpStatus", item.Value));

                                    break;

                                case "Dept":
                                    //需求部門
                                    sql.Append(" AND (Base.Req_Dept = @Dept)");

                                    sqlParamList.Add(new SqlParameter("@Dept", item.Value));

                                    break;

                                case "ReqWho":
                                    //需求人
                                    sql.Append(" AND (Base.Req_Who = @ReqWho)");

                                    sqlParamList.Add(new SqlParameter("@ReqWho", item.Value));

                                    break;

                                case "FinishWho":
                                    //結案人
                                    sql.Append(" AND (Base.Finish_Who IN (");
                                    sql.Append("   SELECT [Guid] FROM [PKSYS].dbo.User_Profile WHERE Account_Name = @FinishWho");
                                    sql.Append(" ))");

                                    sqlParamList.Add(new SqlParameter("@FinishWho", item.Value));

                                    break;

                                case "unClose":
                                    //--未結案需求
                                    sql.Append(" AND (Base.Help_Status <> 125)");

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
                    sqlParamList.Add(new SqlParameter("@currUser", fn_Params.UserGuid)); //用來判斷onTop參數

                    //----- SQL 參數陣列 -----
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
                            var data = new ItHelpData
                            {
                                SeqNo = item.Field<Int64>("SeqNo"),
                                DataID = item.Field<Guid>("DataID"),
                                TraceID = item.Field<string>("TraceID"),
                                CreateDay = item.Field<string>("CreateDay"),
                                Apply_Type = item.Field<Int16>("Apply_Type"),
                                Req_Class = item.Field<Int32>("Req_Class"),
                                Req_ClassName = item.Field<string>("Req_ClassName"),
                                Req_Who = item.Field<string>("Req_Who"),
                                Req_WhoName = item.Field<string>("Req_WhoName"),
                                Req_NickName = item.Field<string>("Req_NickName"),
                                Req_Email = item.Field<string>("Req_Email"),
                                Req_TelExt = item.Field<string>("Req_TelExt"),
                                Req_Dept = item.Field<string>("Req_Dept"),
                                Req_DeptName = item.Field<string>("Req_DeptName"),
                                Help_Subject = item.Field<string>("Help_Subject"),
                                Help_Content = item.Field<string>("Help_Content"),
                                Help_Benefit = item.Field<string>("Help_Benefit"),
                                Help_Status = item.Field<Int32>("Help_Status"),
                                Help_StatusName = item.Field<string>("Help_StatusName"),
                                Help_Way = item.Field<Int16>("Help_Way"),
                                Reply_Content = item.Field<string>("Reply_Content"),
                                ProcInfo = item.Field<string>("ProcInfo"),
                                onTop = item.Field<string>("onTop"),
                                onTopWho = item.Field<string>("onTopWho"),
                                IsRate = item.Field<string>("IsRate"),
                                RateScore = item.Field<Int16>("RateScore"),
                                RateContent = item.Field<string>("RateContent"),
                                RateWhoName = item.Field<string>("RateWhoName"),
                                Finish_Hours = item.Field<double?>("Finish_Hours"),
                                Finish_Time = item.Field<DateTime?>("Finish_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                                Finish_WhoName = item.Field<string>("Finish_WhoName"),
                                Wish_Time = item.Field<DateTime?>("Wish_Time").ToString().ToDateString("yyyy/MM/dd"),

                                Create_Time = item.Field<string>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                                Update_Time = item.Field<string>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                                Create_Name = item.Field<string>("Create_Name"),
                                Update_Name = item.Field<string>("Update_Name"),
                                Agree_Time = item.Field<DateTime?>("Agree_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                                Agree_WhoName = item.Field<string>("Agree_WhoName"),
                                IsAgree = item.Field<string>("IsAgree")
                            };


                            //將項目加入至集合
                            dataList.Add(data);

                        }
                    }

                    //return err
                    if (!string.IsNullOrWhiteSpace(AllErrMsg)) ErrMsg = AllErrMsg;

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
        /// [資訊需求] 取得類別
        /// </summary>
        /// <param name="clsType">A:處理狀態, B:需求類別, C:處理記錄類別</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetClass_IThelp(string clsType, out string ErrMsg)
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
                    FROM [PKEF].dbo.IT_Help_ParamClass
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


        /// <summary>
        /// [資訊需求] 取得目前處理狀態
        /// </summary>
        /// <param name="id">DataID</param>
        /// <returns></returns>
        public string GetOne_ITHelpStatus(string id)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Cls.Class_ID");
                sql.AppendLine(" FROM IT_Help Base");
                sql.AppendLine("  INNER JOIN IT_Help_ParamClass Cls ON Base.Help_Status = Cls.Class_ID");
                sql.AppendLine(" WHERE (Base.DataID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", id);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT == null)
                    {
                        return "";
                    }
                    else
                    {
                        return DT.Rows[0]["Class_ID"].ToString();
                    }
                }
            }
        }


        /// <summary>
        /// [資訊需求] 取得檔案附件
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="detailID"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        public IQueryable<ITHelpAttachment> GetITHelpFileList(string parentID, string detailID, string _type)
        {
            //----- 宣告 -----
            List<ITHelpAttachment> dataList = new List<ITHelpAttachment>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT DetailID, AttachID, AttachFile, AttachFile_Org, ISNULL(Create_Who, '') AS Create_Who");
                sql.AppendLine(" FROM IT_Help_Attach WITH(NOLOCK)");
                sql.AppendLine(" WHERE (AttachType = @type) AND (ParentID = @ParentID)");

                //單身關聯ID
                if (!string.IsNullOrWhiteSpace(detailID))
                {
                    sql.AppendLine(" AND (DetailID = @DetailID)");

                    cmd.Parameters.AddWithValue("DetailID", detailID);
                }

                sql.AppendLine(" ORDER BY Create_Time");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("type", _type);
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
                        var data = new ITHelpAttachment
                        {
                            AttachID = item.Field<int>("AttachID"),
                            AttachFile = item.Field<string>("AttachFile"),
                            AttachFile_Org = item.Field<string>("AttachFile_Org"),
                            Create_Who = item.Field<string>("Create_Who")
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
        /// [資訊需求] 取得轉寄人員
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IQueryable<ITHelpCC> GetITHelpCCList(string parentID)
        {
            //----- 宣告 -----
            List<ITHelpCC> dataList = new List<ITHelpCC>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Prof.Display_Name AS Who, Base.CC_Email AS Email");
                sql.AppendLine(" FROM IT_Help_CC Base");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Base.CC_Who = Prof.Account_Name");
                sql.AppendLine(" WHERE (ParentID = @ParentID)");
                sql.AppendLine(" ORDER BY Prof.DeptID, Prof.Account_Name");

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
                        var data = new ITHelpCC
                        {
                            CC_Who = item.Field<string>("Who"),
                            CC_Email = item.Field<string>("Email")
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
        /// [資訊需求] 取得處理進度
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IQueryable<ITHelpProc> GetITHelpProcList(string parentID)
        {
            //----- 宣告 -----
            List<ITHelpProc> dataList = new List<ITHelpProc>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    SELECT Base.DetailID, Base.Proc_Desc, Base.Proc_Time, Base.Confirm_Time, Base.Wish_Time, Base.Create_Time
                     , Cls.Class_Name
                     , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Guid = Base.Create_Who)), '') AS Create_WhoName
                    FROM IT_Help_DT Base
                     INNER JOIN IT_Help_ParamClass Cls ON Base.Class_ID = Cls.Class_ID
                    WHERE (Base.ParentID = @ParentID)
                    ORDER BY Base.Proc_Time DESC";


                //----- SQL 執行 -----
                cmd.CommandText = sql;
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
                        var data = new ITHelpProc
                        {
                            DetailID = item.Field<int>("DetailID"),
                            Class_Name = item.Field<string>("Class_Name"),
                            Proc_Desc = item.Field<string>("Proc_Desc"),
                            Proc_Time = item.Field<DateTime?>("Proc_Time").ToString().ToDateString("yy/MM/dd HH:mm:ss"),
                            Confirm_Time = item.Field<DateTime?>("Confirm_Time").ToString().ToDateString("yy/MM/dd HH:mm:ss"),
                            Wish_Time = item.Field<DateTime?>("Wish_Time").ToString().ToDateString("yy/MM/dd"),
                            Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yy/MM/dd HH:mm:ss"),
                            Create_WhoName = item.Field<string>("Create_WhoName")
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
        /// [資訊需求] 固定收信清單
        /// </summary>
        /// <param name="type">新需求=1 / 結案=2</param>
        /// <returns></returns>
        public IQueryable<ITHelpReceiver> GetITHelpReceiver(string type)
        {
            //----- 宣告 -----
            List<ITHelpReceiver> dataList = new List<ITHelpReceiver>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT MailAddress AS Email");
                sql.AppendLine(" FROM IT_Help_Receiver");
                sql.AppendLine(" WHERE (MailType = @type) AND (Display = 'Y')");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("type", type);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ITHelpReceiver
                        {
                            Email = item.Field<string>("Email"),
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
        /// 回傳狀態描述(不同css)
        /// </summary>
        /// <param name="_val">判斷值</param>
        /// <param name="_label">顯示文字</param>
        /// <returns></returns>
        public string GetITHelp_StatusLabel(string _val, string _label)
        {
            string css = "";
            string icon = "";

            switch (_val)
            {
                case "110":
                    //未處理
                    css = "ui blue basic label";
                    icon = "<i class=\"clock icon\"></i>";
                    break;

                case "115":
                    //處理中
                    css = "ui green label";
                    icon = "<i class=\"tasks icon\"></i>";
                    break;

                case "120":
                    //測試中
                    css = "ui orange label";
                    icon = "<i class=\"bug icon\"></i>";
                    break;

                case "125":
                    //已結案
                    css = "ui green basic label";
                    icon = "<i class=\"coffee icon\"></i>";
                    break;
            }

            return "<div class=\"{0}\">{2}{1}</div>".FormatThis(css, _label, icon);
        }

        #endregion *** 資訊需求 E ***


        #endregion



        #region -----// Create //-----

        #region *** 資訊需求 S ***
        /// <summary>
        /// [資訊需求] 建立資訊需求-基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CreateITHelp_Base(ItHelpData instance, out string ErrMsg)
        {
            //----- 宣告 -----
            string sql = "";

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql = @"
                    DECLARE @DeptID AS VARCHAR(5)

                    /* 取得部門ID */
                    SET @DeptID = (SELECT DeptID FROM [PKSYS].dbo.User_Profile WHERE (Account_Name = @Req_Who))

                    INSERT INTO IT_Help(
                     DataID, TraceID, Apply_Type
                     , Req_Class, Req_Who, Req_Dept
                     , Help_Subject, Help_Content, Help_Benefit, Help_Status, Help_Way
                     , Create_Who, Create_Time
                    ) VALUES (
                     @NewGuid, @NewTraceID, @Apply_Type
                     , @Req_Class, @Req_Who, @DeptID
                     , @Help_Subject, @Help_Content, @Help_Benefit, @Help_Status, @Help_Way
                     , @WhoGuid, GETDATE()
                    )";

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("NewGuid", instance.DataID);
                cmd.Parameters.AddWithValue("NewTraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("Apply_Type", instance.Apply_Type);
                cmd.Parameters.AddWithValue("Req_Class", instance.Req_Class);
                cmd.Parameters.AddWithValue("Req_Who", instance.Req_Who);
                cmd.Parameters.AddWithValue("Help_Subject", instance.Help_Subject);
                cmd.Parameters.AddWithValue("Help_Content", instance.Help_Content);
                cmd.Parameters.AddWithValue("Help_Benefit", instance.Help_Benefit);
                cmd.Parameters.AddWithValue("Help_Status", 110); //待處理
                cmd.Parameters.AddWithValue("Help_Way", instance.Help_Way);
                cmd.Parameters.AddWithValue("WhoGuid", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 建立資訊需求-附件
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="_detailID">單身編號(可null)</param>
        /// <param name="_type">類型(A需求/B處理記錄)</param>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CreateITHelp_Attachment(string _parentID, string _detailID, string _type
            , List<ITHelpAttachment> instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DECLARE @NewID AS INT");

                for (int row = 0; row < instance.Count; row++)
                {
                    sql.AppendLine(" SET @NewID = (");
                    sql.AppendLine("  SELECT ISNULL(MAX(AttachID) ,0) + 1 FROM IT_Help_Attach");
                    sql.AppendLine(" )");
                    sql.AppendLine(" INSERT INTO IT_Help_Attach(");
                    sql.AppendLine("  AttachID, ParentID, DetailID, AttachFile, AttachFile_Org");
                    sql.AppendLine("  , AttachType, Create_Who, Create_Time");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @NewID, @ParentID, @DetailID, @AttachFile_{0}, @AttachFile_Org_{0}".FormatThis(row));
                    sql.AppendLine("  , @AttachType, @Create_Who, GETDATE()");
                    sql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AttachFile_{0}".FormatThis(row), instance[row].AttachFile);
                    cmd.Parameters.AddWithValue("AttachFile_Org_{0}".FormatThis(row), instance[row].AttachFile_Org);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", _parentID);
                cmd.Parameters.AddWithValue("DetailID", string.IsNullOrWhiteSpace(_detailID) ? (object)DBNull.Value : Convert.ToInt32(_detailID));
                cmd.Parameters.AddWithValue("AttachType", _type);
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 建立資訊需求-轉寄通知
        /// </summary>
        /// <param name="_parentID">單頭編號</param>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CreateITHelp_Inform(string _parentID, List<ITHelpCC> instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DECLARE @NewID AS INT");

                for (int row = 0; row < instance.Count; row++)
                {
                    sql.AppendLine(" SET @NewID = (");
                    sql.AppendLine("  SELECT ISNULL(MAX(Data_ID) ,0) + 1 FROM IT_Help_CC");
                    sql.AppendLine("  WHERE (ParentID = @ParentID)");
                    sql.AppendLine(" )");
                    sql.AppendLine(" INSERT INTO IT_Help_CC(");
                    sql.AppendLine("  ParentID, Data_ID, CC_Who");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @ParentID, @NewID, @CC_Who_{0}".FormatThis(row));
                    sql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("CC_Who_{0}".FormatThis(row), instance[row].CC_Who);
                }

                //Update & Delete Null
                sql.AppendLine(" UPDATE IT_Help_CC");
                sql.AppendLine(" SET CC_Email = Prof.Email");
                sql.AppendLine(" FROM [PKSYS].dbo.User_Profile Prof");
                sql.AppendLine(" WHERE (CC_Who = Prof.Account_Name) AND (ParentID = @ParentID);"); //工號對應

                sql.AppendLine(" DELETE FROM IT_Help_CC");
                sql.AppendLine(" WHERE (CC_Email IS NULL) AND (ParentID = @ParentID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", _parentID);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 建立處理記錄
        /// </summary>
        /// <param name="_parentID"></param>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public Int32 CreateITHelp_Proc(string _parentID, ITHelpProc instance, out string ErrMsg)
        {
            //----- 宣告 -----
            string sql = "";

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql = @"
                    /* 單身筆數為0, 將狀態設為處理中 */
                    IF (SELECT COUNT(*) FROM IT_Help_DT WHERE (ParentID = @ParentID)) = 0
                    BEGIN
                     UPDATE IT_Help
                     SET Help_Status = 115
                     WHERE (DataID = @ParentID)
                    END

                    DECLARE @NewID AS INT
                     SET @NewID = (
                      SELECT ISNULL(MAX(DetailID) ,0) + 1 FROM IT_Help_DT
                      WHERE (ParentID = @ParentID)
                     )
                     INSERT INTO IT_Help_DT(
                      ParentID, DetailID, Class_ID
                      , Proc_Desc, Proc_Time, Confirm_Time, Wish_Time
                      , Create_Who, Create_Time
                     ) VALUES (
                      @ParentID, @NewID, @Class_ID
                      , @Proc_Desc, @Proc_Time, @Confirm_Time, @Wish_Time
                      , @WhoGuid, GETDATE()
                     )

                     SELECT @NewID AS ID";

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", _parentID);
                cmd.Parameters.AddWithValue("Class_ID", instance.Class_ID);
                cmd.Parameters.AddWithValue("Proc_Desc", instance.Proc_Desc);
                cmd.Parameters.AddWithValue("Proc_Time", instance.Proc_Time.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("Confirm_Time", string.IsNullOrWhiteSpace(instance.Confirm_Time) ? (object)DBNull.Value : instance.Confirm_Time.ToDateString("yyyy/MM/dd HH:mm"));
                cmd.Parameters.AddWithValue("Wish_Time", string.IsNullOrWhiteSpace(instance.Wish_Time) ? (object)DBNull.Value : instance.Wish_Time.ToDateString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("WhoGuid", fn_Params.UserGuid);

                //Execute
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    return Convert.ToInt32(DT.Rows[0]["ID"]);
                }
            }

        }


        #endregion *** 資訊需求 E ***


        #endregion



        #region -----// Update //-----

        #region *** 資訊需求 S ***
        /// <summary>
        /// [資訊需求] 更新需求資料
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ITHelpBase(ItHelpData instance, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    DECLARE @DeptID AS VARCHAR(5)
                    /* 取得部門ID */
                    SET @DeptID = (SELECT DeptID FROM [PKSYS].dbo.User_Profile WHERE (Account_Name = @Req_Who))
                    UPDATE IT_Help
                    SET Req_Who = @Req_Who, Req_Dept = @DeptID
                    , Help_Way = @Help_Way, Apply_Type = @Apply_Type
                    , Help_Subject = @Help_Subject, Help_Content = @Help_Content, Help_Benefit = @Help_Benefit
                    , Update_Who = @WhoGuid, Update_Time = GETDATE()
                    WHERE (DataID = @DataID)";


                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", instance.DataID);
                cmd.Parameters.AddWithValue("Req_Who", instance.Req_Who);
                cmd.Parameters.AddWithValue("Apply_Type", instance.Apply_Type);
                cmd.Parameters.AddWithValue("Help_Way", instance.Help_Way);
                cmd.Parameters.AddWithValue("Help_Subject", instance.Help_Subject);
                cmd.Parameters.AddWithValue("Help_Content", instance.Help_Content);
                cmd.Parameters.AddWithValue("Help_Benefit", instance.Help_Benefit);
                cmd.Parameters.AddWithValue("WhoGuid", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 更新回覆資料
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ITHelpReply(ItHelpData instance, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    UPDATE IT_Help
                    SET Req_Class = @Req_Class, Finish_Hours = @Finish_Hours, Reply_Content = @Reply_Content
                    , Wish_Time = @Wish_Time
                    , Update_Who = @WhoGuid, Update_Time = GETDATE()
                    WHERE (DataID = @DataID)";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", instance.DataID);
                cmd.Parameters.AddWithValue("Req_Class", instance.Req_Class);
                cmd.Parameters.AddWithValue("Finish_Hours", instance.Finish_Hours.Equals(0) ? (object)DBNull.Value : instance.Finish_Hours);
                cmd.Parameters.AddWithValue("Wish_Time", string.IsNullOrWhiteSpace(instance.Wish_Time) ? (object)DBNull.Value : instance.Wish_Time);
                cmd.Parameters.AddWithValue("Reply_Content", instance.Reply_Content);
                cmd.Parameters.AddWithValue("WhoGuid", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 更新驗收意見
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ITHelpRate(ItHelpData instance, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    UPDATE IT_Help
                    SET IsRate = 'Y', RateScore = @RateScore, RateContent = @RateContent, RateWho = @WhoGuid
                    , Update_Who = @WhoGuid, Update_Time = GETDATE()
                    WHERE (DataID = @DataID)";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", instance.DataID);
                cmd.Parameters.AddWithValue("RateScore", instance.RateScore);
                cmd.Parameters.AddWithValue("RateContent", instance.RateContent);
                cmd.Parameters.AddWithValue("WhoGuid", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 更新資料-置頂
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ITHelpSetTop(string _id, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"UPDATE IT_Help SET onTop = 'Y', onTopWho = @Who WHERE (DataID = @DataID)";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", _id);
                cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// [資訊需求] 更新資料-結案(125)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ITHelpClose(ItHelpData instance, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    UPDATE IT_Help
                    SET Req_Class = @Req_Class, Reply_Content = @Reply_Content, Wish_Time = @Wish_Time
                     , Finish_Time = @Finish_Time, Finish_Hours = @Finish_Hours, Finish_Who = @Who
                     , Help_Status = @Help_Status, onTop = 'N'
                    WHERE (DataID = @DataID)";


                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", instance.DataID);
                cmd.Parameters.AddWithValue("Req_Class", instance.Req_Class);
                cmd.Parameters.AddWithValue("Reply_Content", instance.Reply_Content);
                cmd.Parameters.AddWithValue("Wish_Time", string.IsNullOrWhiteSpace(instance.Wish_Time) ? (object)DBNull.Value : instance.Wish_Time);
                cmd.Parameters.AddWithValue("Finish_Time", instance.Finish_Time);
                cmd.Parameters.AddWithValue("Finish_Hours", instance.Finish_Hours);
                cmd.Parameters.AddWithValue("Help_Status", 125);
                cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// [資訊需求] 更新資料-處理狀態
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="customID">A:待處理,B:處理中,C:測試中,D:已結案</param>
        /// <returns></returns>
        public bool Update_ITHelpStatus(string dataID, string customID)
        {
            //----- 宣告 -----
            int _clsID = 110;
            switch (customID.ToUpper())
            {
                case "B":
                    _clsID = 115;
                    break;

                case "C":
                    _clsID = 120;
                    break;

                case "D":
                    _clsID = 125;
                    break;
            }

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = "UPDATE IT_Help SET Help_Status = @Help_Status, Update_Who = @Who, Update_Time = GETDATE() WHERE (DataID = @DataID)";


                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("Help_Status", _clsID);
                cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);

                //Execute
                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion *** 資訊需求 E ***

        #endregion



        #region -----// Delete //-----

        #region *** 資訊需求 S ***
        /// <summary>
        /// [資訊需求] 刪除-檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_ITHelpFiles(string _id)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM IT_Help_Attach");
                sql.AppendLine(" WHERE (AttachID = @AttachID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("AttachID", _id);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// [資訊需求] - 刪除處理記錄
        /// </summary>
        /// <param name="_parentID"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public bool Delete_ITHelpProcItem(string _parentID, string _id)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    DELETE FROM IT_Help_Attach WHERE (ParentID = @ParentID) AND (DetailID = @DetailID);
                    DELETE FROM IT_Help_DT WHERE (ParentID = @ParentID) AND (DetailID = @DetailID);
                    ";

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", _parentID);
                cmd.Parameters.AddWithValue("DetailID", _id);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// [資訊需求] - 刪除全部
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public bool Delete_ITHelp(string _id)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                    DELETE FROM IT_Help_CC WHERE (ParentID = @ParentID);
                    DELETE FROM IT_Help_Attach WHERE (ParentID = @ParentID);
                    DELETE FROM IT_Help_DT WHERE (ParentID = @ParentID);
                    DELETE FROM IT_Help WHERE (DataID = @ParentID)
                    ";

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", _id);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        #endregion *** 資訊需求 E ***

        #endregion

    }
}
