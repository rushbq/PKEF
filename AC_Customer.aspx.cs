using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Newtonsoft.Json;

/// <summary>
/// PKSYS客戶列表
/// </summary>
public partial class AC_Customer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[檢查參數] - 查詢關鍵字
            string keywordString = "";
            if (null != Request["q"])
            {
                keywordString = fn_stringFormat.Filter_Html(Request["q"].Trim());
            }

            //[參數宣告] - SqlCommand
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                StringBuilder SBSql = new StringBuilder();
                //清除參數
                cmd.Parameters.Clear();
                //判斷來源
                switch (Request["f"])
                {
                    case "rel":
                        //設定業務所屬客戶使用
                        SBSql.AppendLine("SELECT TOP 100 RTRIM(MA001) AS custid, MA002 AS shortName, MA003 AS fullName ");
                        SBSql.AppendLine("  , ISNULL(MA009,'') AS email, ISNULL(MA065, '') AS parentID");
                        SBSql.AppendLine(" FROM Customer WITH (NOLOCK) ");
                        SBSql.AppendLine(" WHERE (DBS = DBC)");
                        SBSql.AppendLine(" AND MA001 NOT IN ( SELECT Rel.CustID FROM Staff_Rel_Customer AS Rel) ");
                        SBSql.AppendLine(" AND ( ");
                        SBSql.AppendLine("      (MA001 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA002 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA003 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA009 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" ) ");
                        SBSql.AppendLine(" ORDER BY MA001 ");
                        break;

                    case "userSet":
                        //設定客戶目標，為使用者權限時，顯示自己的客戶(主管可見屬下的客戶)
                        SBSql.AppendLine("SELECT TOP 100 RTRIM(MA001) AS custid, MA002 AS shortName, MA003 AS fullName ");
                        SBSql.AppendLine("  , ISNULL(MA009,'') AS email, ISNULL(MA065, '') AS parentID");
                        SBSql.AppendLine(" FROM Customer WITH (NOLOCK) ");
                        SBSql.AppendLine("  INNER JOIN Staff_Rel_Customer Rel ON Customer.MA001 = Rel.CustID ");
                        SBSql.AppendLine("  INNER JOIN User_Profile Staff ON Rel.StaffID = Staff.Account_Name ");
                        SBSql.AppendLine("");
                        SBSql.AppendLine(" WHERE (Customer.DBS = Customer.DBC) ");
                        SBSql.AppendLine(" AND ( ");
                        SBSql.AppendLine("      (MA001 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA002 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA003 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA009 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" ) ");

                        //自己或主管的下屬
                        SBSql.AppendLine(" AND ((Staff.Account_Name = @Login_UserID)");
                        SBSql.AppendLine(" OR");
                        SBSql.AppendLine("   Rel.StaffID IN (");
                        SBSql.AppendLine("     SELECT Prof.Account_Name FROM User_Profile Prof INNER JOIN User_Dept_Supervisor Sup ON Prof.DeptID = Sup.DeptID");
                        SBSql.AppendLine("     WHERE (Sup.Account_Name = @Login_UserID)");
                        SBSql.AppendLine("    ))");

                        SBSql.AppendLine(" ORDER BY MA001 ");

                        cmd.Parameters.AddWithValue("Login_UserID", fn_Params.UserAccount);

                        break;

                    case "custDB":
                        //客戶的主要資料庫
                        SBSql.AppendLine("SELECT TOP 100 Cust.DBC AS myDB, Corp.Corp_Name AS myDBName ");
                        SBSql.AppendLine(" , RTRIM(Cust.MA001) AS custid, Cust.MA002 AS shortName ");
                        SBSql.AppendLine(" FROM Customer Cust WITH (NOLOCK) ");
                        SBSql.AppendLine("  LEFT JOIN Param_Corp Corp WITH (NOLOCK) ON Cust.DBC = Corp.Corp_ID ");
                        SBSql.AppendLine(" WHERE ( ");
                        SBSql.AppendLine("      (Cust.MA001 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (Cust.MA002 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (Cust.MA003 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" ) ");
                        SBSql.AppendLine(" GROUP BY Cust.DBC, Cust.MA001, Cust.MA002, Corp.Corp_Name ");
                        SBSql.AppendLine(" ORDER BY Cust.MA002 ");

                        break;

                    case "myCust":
                        //依使用者的公司別權限(Staff_Rel_Corp)取得客戶, 可在基本資料維護設定
                        SBSql.AppendLine("SELECT TOP 100");
                        SBSql.AppendLine("  myCorp.DB_Name AS myDB ");
                        SBSql.AppendLine("  , RTRIM(Cust.MA001) AS custId, Cust.MA002 AS shortName, ISNULL(MA014,'') AS currency ");
                        SBSql.AppendLine("  , Cust.MA005 AS shipWho, Cust.MA006 AS tel, ISNULL(MA027,'') AS shipAddr, MA016 AS salesID ");
                        SBSql.AppendLine("  , CustData.SWID AS SWID ");
                        SBSql.AppendLine(" FROM Customer Cust WITH (NOLOCK) ");
                        SBSql.AppendLine("  INNER JOIN Param_Corp myCorp ON UPPER(Cust.DBC) = UPPER(myCorp.Corp_ID) ");
                        SBSql.AppendLine("  INNER JOIN Staff_Rel_Corp Rel ON myCorp.Corp_UID = rel.Corp_UID ");
                        SBSql.AppendLine("  INNER JOIN Customer_Data CustData ON Cust.MA001 = CustData.Cust_ERPID");
                        SBSql.AppendLine(" WHERE (Cust.DBS = Cust.DBC) ");
                        SBSql.AppendLine(" AND (Rel.StaffID = @Login_UserID) ");
                        SBSql.AppendLine(" AND ( ");
                        SBSql.AppendLine("      (MA001 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA002 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA003 LIKE '%' + @Keyword + '%') ");

                        SBSql.AppendLine(" ) ");
                        SBSql.AppendLine(" ORDER BY MA001 ");


                        cmd.Parameters.AddWithValue("Login_UserID", fn_Params.UserAccount);

                        break;

                    default:
                        SBSql.AppendLine("SELECT TOP 100 RTRIM(MA001) AS custid, MA002 AS shortName, MA003 AS fullName ");
                        SBSql.AppendLine("  , ISNULL(MA009,'') AS email, ISNULL(MA065, '') AS parentID");
                        SBSql.AppendLine(" FROM Customer WITH (NOLOCK) ");
                        SBSql.AppendLine(" WHERE (DBS = DBC) AND ( ");
                        SBSql.AppendLine("      (MA001 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA002 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA003 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine("   OR (MA009 LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" ) ");
                        SBSql.AppendLine(" ORDER BY MA001 ");
                        break;
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));
                //[參數宣告] - DataTable
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        Response.Write("[{\"custid\":\"-\",\"shortName\":\"查無資料...\",\"fullName\":\"\",\"email\":\"\",\"parentID\":\"\"}]");
                    }
                    else
                    {
                        Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                    }
                }
            }

        }

    }
}