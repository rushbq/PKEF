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

/// <summary>
/// 報表中心 - Report List
/// </summary>
public partial class Json_ReportList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[取得參數] - Type
                string myDataType = string.IsNullOrEmpty(Request.Form["DataType"]) ? "" : Request.Form["DataType"].ToString();

                //[參數宣告] - SqlCommand
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;

                    //[SQL] - 清除cmd參數
                    cmd.Parameters.Clear();

                    //[SQL] - 執行SQL
                    StringBuilder SBSql = new StringBuilder();

                    switch (myDataType.ToUpper())
                    {
                        case "DEALER":
                            //-- 報表第一層選單 --
                            SBSql.Append(" SELECT CAST(Prog.Prog_ID AS VARCHAR) AS id, '0' AS pId, Prog.Prog_Name_zh_TW AS name");
                            SBSql.Append(" FROM Program Prog WITH(NOLOCK)");
                            SBSql.Append(" WHERE (Prog.Display = 'Y') AND (Prog.Lv = 2) AND (Up_Id = 10000)");
                            SBSql.Append("  AND (Prog.Prog_ID IN ('11000','11100','11200'))");

                            SBSql.Append(" UNION ALL");

                            //-- 報表第二層選單 --
                            SBSql.Append(" SELECT 'v_' + CAST(Prog.Prog_ID AS VARCHAR) AS id, Prog.Up_Id AS pId, Rpt.Rpt_Desc AS name");
                            SBSql.Append(" FROM Program Prog WITH(NOLOCK)");
                            SBSql.Append("  INNER JOIN Rpt_Base Rpt ON Prog.Prog_ID = Rpt.Prog_ID");
                            SBSql.Append(" WHERE (Prog.Display = 'Y') AND (Prog.Lv = 3)");
                            SBSql.Append("  AND (Prog.Up_Id IN ('11000','11100','11200'))");

                            break;


                        default:
                            //-- 報表第一層選單 --
                            SBSql.Append(" SELECT CAST(Prog.Prog_ID AS VARCHAR) AS id, '0' AS pId, Prog.Prog_Name_zh_TW AS name");
                            SBSql.Append(" FROM Program Prog WITH(NOLOCK)");
                            SBSql.Append(" WHERE (Prog.Display = 'Y') AND (Prog.Lv = 2) AND (Up_Id = 10000)");

                            SBSql.Append(" UNION ALL");

                            //-- 報表第二層選單 --
                            SBSql.Append(" SELECT 'v_' + CAST(Prog.Prog_ID AS VARCHAR) AS id, Prog.Up_Id AS pId, Rpt.Rpt_Desc AS name");
                            SBSql.Append(" FROM Program Prog WITH(NOLOCK)");
                            SBSql.Append("  INNER JOIN Rpt_Base Rpt ON Prog.Prog_ID = Rpt.Prog_ID");
                            SBSql.Append(" WHERE (Prog.Display = 'Y') AND (Prog.Lv = 3)");

                            break;

                    }
                    /*
                     * [取值注意事項]
                     * 使用v_+ ID, 用來判斷此為要取用的值, 並在寫入時replace 'v_'為空白
                     */

                    //[SQL] - Command
                    cmd.CommandText = SBSql.ToString();

                    //[參數宣告] - DataTable
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.ReportCenter, out ErrMsg))
                    {
                        Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                    }
                }
            }
            catch (Exception)
            {
                Response.Write("error");
            }

        }

    }
}