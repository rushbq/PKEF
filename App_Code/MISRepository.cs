using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MISData.Models;
using PKLib_Method.Methods;


namespace MISData.Controllers
{
    public class MISRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

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
        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="dbs">資料庫別</param>
        /// <param name="dbName">資料庫名</param>
        /// <param name="userGuid">使用者GUID</param>
        /// <returns></returns>
        private DataTable LookupRawData(string dbs, string dbName, string userGuid)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                switch (dbs)
                {
                    case "1":
                        //Home Local
                        sql.Append(GetSQL_Local());
                        break;

                    default:
                        sql.Append(GetSQL(dbName));
                        break;
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("UserGuid", userGuid);

                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// SQL - 權限中心
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetSQL_Local()
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(" SELECT Base.Menu_ID AS ID, Base.Parent_ID AS ParentID, Base.MenuName_zh_TW AS MenuName, Base.Menu_Level Level");
            sql.AppendLine("  , (CASE WHEN Auth.User_Guid IS NULL THEN 'N' ELSE 'Y' END) AS IsChecked");
            sql.AppendLine(" FROM Home_Menu Base WITH (NOLOCK)");
            sql.AppendLine("  LEFT JOIN Home_Menu_UserAuth Auth WITH (NOLOCK) ON Base.Menu_ID = Auth.Menu_ID AND (Auth.User_Guid = @UserGuid)");
            sql.AppendLine(" WHERE (Base.Display = 'Y')");
            sql.AppendLine(" ORDER BY Base.Sort, Base.Menu_ID");

            return sql;
        }


        private StringBuilder GetSQL(string dbName)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(" SELECT Base.Prog_ID AS ID, Base.Up_Id AS ParentID, Base.Prog_Name_zh_TW AS MenuName, Base.lv Level");
            sql.AppendLine("  , (CASE WHEN Auth.Guid IS NULL THEN 'N' ELSE 'Y' END) AS IsChecked");
            sql.AppendLine(" FROM [{0}].dbo.Program Base WITH (NOLOCK)".FormatThis(dbName));
            sql.AppendLine("  LEFT JOIN [{0}].dbo.User_Profile_Rel_Program Auth WITH (NOLOCK) ON Base.Prog_ID = Auth.Prog_ID AND (Auth.Guid = @UserGuid)".FormatThis(dbName));
            sql.AppendLine(" WHERE (Base.Display = 'Y')");
            sql.AppendLine(" ORDER BY Base.Sort, Base.Prog_ID");

            return sql;
        }

        #endregion

    }
}
