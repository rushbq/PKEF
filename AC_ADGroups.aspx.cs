using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;

/// <summary>
/// 列出AD群組
/// </summary>
public partial class AC_ADGroups : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[檢查參數] - 查詢關鍵字
                string keywordString = Request.QueryString["q"].Trim();

                //[參數宣告] - SqlCommand
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg = "";
                    StringBuilder SBSql = new StringBuilder();
                    //[判斷參數] - 來源
                    switch (Request.QueryString["Srh_Type"])
                    {
                        case "System":  //管理者可查全部
                            SBSql.AppendLine("SELECT (Account_Name + ' (' + Display_Name + ')') AS Search_Value, Guid ");
                            SBSql.AppendLine(" FROM User_Group ");
                            SBSql.AppendLine(" WHERE (Display = 'Y') AND (Account_Name LIKE '%' + @Keyword + '%') OR (Display_Name LIKE '%' + @Keyword + '%') ");
                            SBSql.AppendLine(" ORDER BY Sort, Display_Name ");

                            break;
                        case "User":  //使用者只能查Display = "Y"
                            SBSql.AppendLine("SELECT (Account_Name + ' (' + Display_Name + ')') AS Search_Value, Guid ");
                            SBSql.AppendLine(" FROM User_Group ");
                            SBSql.AppendLine(" WHERE (Display = 'Y') AND ((Account_Name LIKE '%' + @Keyword + '%') OR (Display_Name LIKE '%' + @Keyword + '%')) ");
                            SBSql.AppendLine(" ORDER BY Sort, Display_Name ");

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
                        StringBuilder SBHtml = new StringBuilder();
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            SBHtml.Clear();
                            SBHtml.Append(DT.Rows[i]["Search_Value"].ToString().Replace("|", "｜"));
                            //"|" 為值的分隔符號
                            SBHtml.AppendLine("|" + DT.Rows[i]["Guid"].ToString().Replace("|", "｜"));
                            Response.Write(SBHtml);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            Response.Write("");
        }
    }
}