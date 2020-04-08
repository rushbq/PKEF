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

public partial class Json_GetAuthList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[取得參數] - 編號/Type
                string myGuid = Request.Form["Guid"];
                string myDataType = Request.Form["DataType"];

                //[參數宣告] - SqlCommand
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;

                    //[SQL] - 清除cmd參數
                    cmd.Parameters.Clear();

                    //[SQL] - 執行SQL
                    StringBuilder SBSql = new StringBuilder();

                    //取得User Guid的所屬群組
                    Guid objectGuid = new Guid(myGuid);
                    ArrayList aryGroup = ADService.getGroupGUIDFromGUID(objectGuid);

                    switch (myDataType.ToUpper())
                    {
                        case "USER":
                            SBSql.AppendLine(" SELECT Prog.Prog_ID AS id, Prog.Up_Id AS pId, Prog_Name_zh_TW AS name ");
                            SBSql.AppendLine("    , (CASE WHEN Rel.Guid IS NULL THEN 'false' ELSE 'true' END) AS [open] ");
                            SBSql.AppendLine("    , (CASE WHEN Rel.Guid IS NULL THEN 'false' ELSE 'true' END) AS [checked] ");
                            SBSql.AppendLine(" FROM Program Prog WITH (NOLOCK)");
                            SBSql.AppendLine("    LEFT JOIN User_Profile_Rel_Program Rel WITH (NOLOCK)");
                            SBSql.AppendLine("    ON Prog.Prog_ID = Rel.Prog_ID AND Rel.Guid = @DataID ");
                            SBSql.AppendLine(" WHERE (Prog.Display = 'Y') ");
                            SBSql.AppendLine(" ORDER BY Prog.Sort, Prog.Prog_ID ");

                            break;

                        case "USER_IN_GROUP":

                            //查詢該使用者在所屬群組中,有哪些權限(設為Disabled)
                            SBSql.AppendLine(" SELECT Prog.Prog_ID AS id, Prog.Up_Id AS pId, Prog_Name_zh_TW AS name ");
                            SBSql.AppendLine("    , 'true' AS [open] ");
                            SBSql.AppendLine("    , (CASE WHEN Rel.Prog_ID IS NULL THEN 'false' ELSE 'true' END) AS [checked], 'true' AS [chkDisabled] ");
                            SBSql.AppendLine(" FROM Program Prog WITH (NOLOCK)");
                            SBSql.AppendLine("    LEFT JOIN User_Group_Rel_Program Rel WITH (NOLOCK) ");
                            SBSql.AppendLine("    ON Prog.Prog_ID = Rel.Prog_ID ");

                            //判斷條件, 組合SQL字串
                            if (aryGroup != null && aryGroup.Count > 0)
                            {
                                List<string> listGroup = aryGroup.Cast<string>().ToList();

                                SBSql.Append(" AND (Rel.Guid IN ({0})) ".FormatThis(
                                        fn_Extensions.GetSQLParam(listGroup, "myGroupGuid")
                                    ));
                            }

                            SBSql.AppendLine(" WHERE (Prog.Display = 'Y') ");
                            SBSql.AppendLine(" GROUP BY Prog.Prog_ID, Prog.Up_Id, Prog_Name_zh_TW, Prog.Sort, Rel.Prog_ID ");
                            SBSql.AppendLine(" ORDER BY Prog.Sort, Prog.Prog_ID ");

                            //判斷條件, 組合參數串
                            if (aryGroup != null && aryGroup.Count > 0)
                            {
                                for (int row = 0; row < aryGroup.Count; row++)
                                {
                                    cmd.Parameters.AddWithValue("myGroupGuid{0}".FormatThis(row), aryGroup[row].ToString());
                                }
                            }

                            break;

                        case "COPY_GROUP":
                            //查詢該使用者在所屬群組中,有哪些權限
                            SBSql.AppendLine(" SELECT Prog.Prog_ID AS id, Prog.Up_Id AS pId, Prog_Name_zh_TW AS name ");
                            SBSql.AppendLine("    , 'true' AS [open] ");
                            SBSql.AppendLine("    , (CASE WHEN Rel.Prog_ID IS NULL THEN 'false' ELSE 'true' END) AS [checked] ");
                            SBSql.AppendLine(" FROM Program Prog WITH (NOLOCK)");
                            SBSql.AppendLine("    LEFT JOIN User_Group_Rel_Program Rel WITH (NOLOCK)");
                            SBSql.AppendLine("    ON Prog.Prog_ID = Rel.Prog_ID ");

                            //判斷條件, 組合SQL字串
                            if (aryGroup != null && aryGroup.Count > 0)
                            {
                                List<string> listGroup = aryGroup.Cast<string>().ToList();

                                SBSql.Append(" AND (Rel.Guid IN ({0})) ".FormatThis(
                                        fn_Extensions.GetSQLParam(listGroup, "myGroupGuid")
                                    ));
                            }

                            SBSql.AppendLine(" WHERE (Prog.Display = 'Y') ");
                            SBSql.AppendLine(" GROUP BY Prog.Prog_ID, Prog.Up_Id, Prog_Name_zh_TW, Prog.Sort, Rel.Prog_ID ");
                            SBSql.AppendLine(" ORDER BY Prog.Sort, Prog.Prog_ID ");

                            //判斷條件, 組合參數串
                            if (aryGroup != null && aryGroup.Count > 0)
                            {
                                for (int row = 0; row < aryGroup.Count; row++)
                                {
                                    cmd.Parameters.AddWithValue("myGroupGuid{0}".FormatThis(row), aryGroup[row].ToString());
                                }
                            }

                            break;

                        default:
                            SBSql.AppendLine(" SELECT Prog.Prog_ID AS id, Prog.Up_Id AS pId, Prog_Name_zh_TW AS name ");
                            SBSql.AppendLine("    , (CASE WHEN Rel.Guid IS NULL THEN 'false' ELSE 'true' END) AS [open] ");
                            SBSql.AppendLine("    , (CASE WHEN Rel.Guid IS NULL THEN 'false' ELSE 'true' END) AS [checked] ");
                            SBSql.AppendLine(" FROM Program Prog WITH (NOLOCK)");
                            SBSql.AppendLine("    LEFT JOIN User_Group_Rel_Program Rel WITH (NOLOCK)");
                            SBSql.AppendLine("    ON Prog.Prog_ID = Rel.Prog_ID AND Rel.Guid = @DataID ");
                            SBSql.AppendLine(" WHERE (Prog.Display = 'Y') ");
                            SBSql.AppendLine(" ORDER BY Prog.Sort, Prog.Prog_ID ");

                            break;
                    }

                    //[SQL] - Command
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("DataID", myGuid);

                    //[參數宣告] - DataTable
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
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