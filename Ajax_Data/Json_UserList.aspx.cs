using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using Newtonsoft.Json;

public partial class Json_UserList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[參數宣告] - SqlCommand
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;
                    string block = Request.Form["block"] == null ? "N" : Request.Form["block"].ToString();

                    //[SQL] - 清除cmd參數
                    cmd.Parameters.Clear();

                    //[SQL] - 執行SQL
                    StringBuilder SBSql = new StringBuilder();

                    SBSql.Append("SELECT Tbl.* FROM ");
                    SBSql.Append("( ");
                    SBSql.Append("    SELECT SID AS id, '0' AS pId, '【' + SName + '】' AS name, CAST(Sort AS NVARCHAR) AS Sort, '{0}' AS [chkDisabled] ".FormatThis(
                        //鎖定checkbox(資訊需求轉寄)
                        block.Equals("Y") ? "true" : "false"
                        ));
                    SBSql.Append("    FROM Shipping WITH (NOLOCK) ");
                    SBSql.Append("    WHERE Display = 'Y' ");

                    SBSql.Append("    UNION ALL ");

                    SBSql.Append("    SELECT Dept.DeptID AS id, Dept.Area AS pId, Dept.DeptName AS name, CAST(100 + Dept.Sort AS NVARCHAR) AS Sort, 'false' AS [chkDisabled] ");
                    SBSql.Append("    FROM User_Dept Dept WITH (NOLOCK) ");
                    SBSql.Append("    WHERE (Dept.Display = 'Y') ");

                    if (block.Equals("Y"))
                    {
                        //排除資訊部(資訊需求轉寄)
                        SBSql.Append(" AND (Dept.DeptID NOT IN ('109'))");
                    }

                    SBSql.Append("    UNION ALL ");

                    //使用v_+ 工號, 用來判斷此為要取用的值, 並在寫入時replace 'v_'為空白
                    SBSql.Append("    SELECT 'v_' + Prof.Account_Name AS id, Prof.DeptID AS pId, Prof.Display_Name AS name, Prof.Account_Name AS Sort, 'false' AS [chkDisabled] ");
                    SBSql.Append("    FROM User_Profile Prof WITH (NOLOCK) ");
                    SBSql.Append("        INNER JOIN User_Dept Dept ON Prof.DeptID = Dept.DeptID ");
                    SBSql.Append("    WHERE (Dept.Display = 'Y') AND (Prof.Display = 'Y') AND (Prof.Email IS NOT NULL) AND (Prof.Email <> '') ");

                    if (block.Equals("Y"))
                    {
                        //排除資訊部(資訊需求轉寄)
                        SBSql.Append(" AND (Dept.DeptID NOT IN ('109'))");
                    }

                    SBSql.Append(") AS Tbl ");

                    SBSql.Append("ORDER BY Tbl.Sort ");

                    //[SQL] - Command
                    cmd.CommandText = SBSql.ToString();

                    //[參數宣告] - DataTable
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                    }
                }
            }
            catch (Exception)
            {
                Response.Write(null);
            }

        }

    }
}