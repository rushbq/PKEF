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
using Newtonsoft.Json.Linq;

/// <summary>
/// 列出AD群組
/// </summary>
public partial class AC_ADUsers : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[檢查參數] - 查詢關鍵字
                string keywordString = "";
                if (null != Request.Form["q"])
                {
                    keywordString = fn_stringFormat.Filter_Html(Request.Form["q"].Trim());
                }

                //[參數宣告] - SqlCommand
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg = "";
                    StringBuilder SBSql = new StringBuilder();
                    //[判斷參數] - 來源
                    switch (Request.Form["type"])
                    {
                        case "System":  //管理者可查全部
                            SBSql.AppendLine("SELECT (Account_Name + ' (' + Display_Name + ')' + (CASE Display WHEN 'N' THEN '[停用]' ELSE '' END)) AS label ");
                            SBSql.AppendLine("  , Guid");
                            SBSql.AppendLine("  , (CASE WHEN CHARINDEX('-',Display_Name,0) < 1 ");
                            SBSql.AppendLine("    THEN Display_Name ");
                            SBSql.AppendLine("    ELSE LEFT(Display_Name, CHARINDEX('-',Display_Name,0)-1) ");
                            SBSql.AppendLine("  END) AS category ");
                            SBSql.AppendLine(" FROM User_Profile ");
                            SBSql.AppendLine(" WHERE ((Account_Name LIKE '%' + @Keyword + '%') OR (Display_Name LIKE '%' + @Keyword + '%')) ");
                            SBSql.AppendLine(" ORDER BY category, Account_Name ");

                            break;

                        case "User":  //使用者只能查Display = "Y"
                            SBSql.AppendLine("SELECT (Account_Name + ' (' + Display_Name + ')') AS label, Guid ");
                            SBSql.AppendLine("  , (CASE WHEN CHARINDEX('-',Display_Name,0) < 1 ");
                            SBSql.AppendLine("    THEN Display_Name ");
                            SBSql.AppendLine("    ELSE LEFT(Display_Name, CHARINDEX('-',Display_Name,0)-1) ");
                            SBSql.AppendLine("  END) AS category ");
                            SBSql.AppendLine(" FROM User_Profile ");
                            SBSql.AppendLine(" WHERE (Display = 'Y') AND ((Account_Name LIKE '%' + @Keyword + '%') OR (Display_Name LIKE '%' + @Keyword + '%')) ");
                            SBSql.AppendLine(" ORDER BY category, Account_Name ");

                            break;

                        default:
                            Response.Write("");
                            break;
                    }
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));

                    //[參數宣告] - DataTable
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                    }
                }
            }
        }

        catch (Exception)
        {
            Response.Write("error");
        }
    }
}