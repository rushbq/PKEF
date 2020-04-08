using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class Google_GetData : System.Web.UI.Page
{
    /// <summary>
    /// 案件數(類別)
    /// </summary>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetCountbyClass(string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - Parameters
            cmd.Parameters.Clear();

            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT COUNT(Main.TraceID) AS GroupCnt, ReqClass.Class_ID AS GroupID, ReqClass.Class_Name AS GroupName ");
            sbSQL.Append(" FROM IT_Help Main ");
            sbSQL.Append("  INNER JOIN IT_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID ");
            sbSQL.Append(" WHERE (ReqClass.Display = 'Y')");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Main.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Main.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }

            sbSQL.Append(" GROUP BY ReqClass.Class_ID, ReqClass.Class_Name");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                int val = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組名稱
                    cat = dr[2].ToString();
                    //群組數值
                    val = Convert.ToInt32(dr[0]);
                    dataList.Add(new Data(cat, val));
                }
                return dataList;
            }
        }

    }

    /// <summary>
    /// 案件數(部門)
    /// </summary>
    /// <param name="AreaCode">區域別</param>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetCountbyDept(string AreaCode, string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - Parameters
            cmd.Parameters.Clear();

            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT COUNT(Main.TraceID) AS GroupCnt, Dept.DeptID AS GroupID, Dept.DeptName AS GroupName ");
            sbSQL.Append(" FROM IT_Help Main ");
            sbSQL.Append("  INNER JOIN PKSYS.dbo.User_Dept Dept ON Main.Req_Dept = Dept.DeptID ");
            sbSQL.Append(" WHERE (Dept.Display = 'Y') ");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Main.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Main.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }
            //[查詢條件] - 區域
            if (false == string.IsNullOrEmpty(AreaCode))
            {
                sbSQL.Append(" AND (Dept.Area = @AreaCode)");
                cmd.Parameters.AddWithValue("AreaCode", AreaCode);
            }

            sbSQL.Append(" GROUP BY Dept.DeptID, Dept.DeptName ");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                int val = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組名稱
                    cat = dr[2].ToString();
                    //群組數值
                    val = Convert.ToInt32(dr[0]);
                    dataList.Add(new Data(cat, val));
                }
                return dataList;
            }
        }

    }

    /// <summary>
    /// 時數(類別)
    /// </summary>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetHourByClass(string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - Parameters
            cmd.Parameters.Clear();

            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT SUM(ISNULL(Base.Reply_Hours, 0)) AS GroupSum");
            sbSQL.Append("  , COUNT(Base.TraceID) AS GroupCnt");
            sbSQL.Append("  , ReqClass.Class_Name AS GroupName");
            sbSQL.Append(" FROM IT_Help Base ");
            sbSQL.Append("  INNER JOIN IT_ParamClass ReqClass ON Base.Req_Class = ReqClass.Class_ID ");
            sbSQL.Append(" WHERE (ReqClass.Display = 'Y') AND (Base.Reply_Hours > 0)");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }

            sbSQL.Append(" GROUP BY ReqClass.Class_ID, ReqClass.Class_Name ");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                double val = 0;
                int val1 = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組數值
                    val = Convert.ToDouble(dr[0]);
                    val1 = Convert.ToInt32(dr[1]);

                    //群組名稱
                    cat = dr[2].ToString();

                    dataList.Add(new Data(cat, val, val1));
                }
                return dataList;
            }
        }

    }

    /// <summary>
    /// 時數(人員)
    /// </summary>
    /// <param name="AreaCode">區域別</param>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetHourByWho(string AreaCode, string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - Parameters
            cmd.Parameters.Clear();

            //[SQL] - 查詢
            sbSQL.Append(" SELECT SUM(Base.Reply_Hours) AS GroupSum");
            sbSQL.Append("  , COUNT(Base.TraceID) AS GroupCnt");
            sbSQL.Append("  , Prof.Account_Name + ' (' + Prof.Display_Name + ')' AS GroupName");
            sbSQL.Append(" FROM IT_Help Base");
            sbSQL.Append("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Base.Update_Who = Prof.Account_Name");
            sbSQL.Append("  INNER JOIN PKSYS.dbo.User_Dept Dept ON Prof.DeptID = Dept.DeptID");
            sbSQL.Append(" WHERE (Base.Update_Who IS NOT NULL) AND (Base.Reply_Hours > 0) AND (Prof.Display = 'Y')");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }
            //[查詢條件] - 區域
            if (false == string.IsNullOrEmpty(AreaCode))
            {
                sbSQL.Append(" AND (Dept.Area = @AreaCode)");
                cmd.Parameters.AddWithValue("AreaCode", AreaCode);
            }

            sbSQL.Append(" GROUP BY Prof.Account_Name, Prof.Display_Name");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                double val = 0;
                int val1 = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組數值
                    val = Convert.ToDouble(dr[0]);
                    val1 = Convert.ToInt32(dr[1]);

                    //群組名稱
                    cat = dr[2].ToString();

                    dataList.Add(new Data(cat, val, val1));
                }
                return dataList;
            }
        }

    }

    /// <summary>
    /// 滿意度
    /// </summary>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetRateStar(string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - Parameters
            cmd.Parameters.Clear();

            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT");
            sbSQL.Append(" ISNULL(SUM(CASE WHEN Base.RateScore = 1 THEN 1 ELSE 0 END) ,0) AS '一星'");
            sbSQL.Append(" , ISNULL(SUM(CASE WHEN Base.RateScore = 2 THEN 1 ELSE 0 END) ,0) AS '二星'");
            sbSQL.Append(" , ISNULL(SUM(CASE WHEN Base.RateScore = 3 THEN 1 ELSE 0 END) ,0) AS '三星'");
            sbSQL.Append(" , ISNULL(SUM(CASE WHEN Base.RateScore = 4 THEN 1 ELSE 0 END) ,0) AS '四星'");
            sbSQL.Append(" , ISNULL(SUM(CASE WHEN Base.RateScore = 5 THEN 1 ELSE 0 END) ,0) AS '五星'");
            sbSQL.Append(" FROM IT_Help Base");
            sbSQL.Append(" WHERE (Base.IsRate = 'Y')");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();

            //取得資料
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                int val = 0;

                foreach (DataRow dr in DT.Rows)
                {
                    foreach (DataColumn dc in DT.Columns)
                    {
                        //群組名稱
                        cat = dc.ColumnName.ToString();

                        //群組數值
                        val = Convert.ToInt32(dr[dc]);

                        dataList.Add(new Data(cat, val));
                    }
                }

                return dataList;
            }
        }

    }
}

public class Data
{
    public string ColumnName = "";
    public double Value = 0;
    public double Value1 = 0;

    public Data(string columnName, double value)
    {
        ColumnName = columnName;
        Value = value;
    }

    public Data(string columnName, double value, double value1)
    {
        ColumnName = columnName;
        Value = value;
        Value1 = value1;
    }
}
