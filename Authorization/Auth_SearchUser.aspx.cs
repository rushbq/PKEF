using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Collections;

public partial class Auth_SearchUser : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string ErrMsg;

            //[權限判斷] - 權限設定
            if (fn_CheckAuth.CheckAuth_User("9906", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }
            //帶出資料
            LookupData();
        }
    }

    #region -- 資料取得 --
    private void LookupData()
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[清除參數]
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Dept.Area,Dept.DeptID, Dept.DeptName ");
                SBSql.AppendLine("    , Prof.Guid, Prof.Account_Name, Prof.Display_Name ");
                SBSql.AppendLine("    , ROW_NUMBER() OVER(PARTITION BY Dept.DeptID ORDER BY Dept.Area_Sort, Dept.DeptID ASC) AS GP_Rank ");
                //計算部門名單數
                SBSql.AppendLine("    , (SELECT COUNT(*) FROM User_Profile WHERE (DeptID = Dept.DeptID) AND (Display = 'Y') AND (Guid IN ( ");
                SBSql.AppendLine("     SELECT Guid FROM PKEF.dbo.User_Profile_Rel_Program ");
                SBSql.AppendLine("    ))) AS UserCnt ");
                SBSql.AppendLine(" FROM User_Dept Dept ");
                SBSql.AppendLine("    INNER JOIN User_Profile Prof ON Dept.DeptID = Prof.DeptID ");
                SBSql.AppendLine(" WHERE (Dept.Display = 'Y') AND (Prof.Display = 'Y') ");
                SBSql.AppendLine("  AND (Prof.Guid IN (SELECT Guid FROM PKEF.dbo.User_Profile_Rel_Program)) ");
                SBSql.AppendLine(" ORDER BY Dept.Area_Sort, Dept.DeptID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.lt_Content.Text = "<div class=\"styleEarth Font13\" style=\"padding:15px 15px 15px 15px\">尚未有人員權限..</div>";
                        return;
                    }
                    //[輸出Html]
                    StringBuilder html = new StringBuilder();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //[取得欄位資料]
                        #region * 取得欄位資料 *
                        string Area = DT.Rows[row]["Area"].ToString();
                        string DeptID = DT.Rows[row]["DeptID"].ToString();
                        string DeptName = DT.Rows[row]["DeptName"].ToString();
                        string GP_Rank = DT.Rows[row]["GP_Rank"].ToString();
                        string Guid = DT.Rows[row]["Guid"].ToString();
                        string Account_Name = DT.Rows[row]["Account_Name"].ToString();
                        string Display_Name = DT.Rows[row]["Display_Name"].ToString();
                        int UserCnt = Convert.ToInt32(DT.Rows[row]["UserCnt"]);
                        #endregion

                        //[HTML] - 顯示, 每類標頭 (GP_Rank = 1)
                        if (Convert.ToInt16(GP_Rank).Equals(1))
                        {
                            if (row > 0)
                            {
                                html.AppendLine("</ul>");
                                html.AppendLine("</td>");
                                //[Table] - Column (Content) ,End ----------
                                html.AppendLine("</tr>");
                                html.AppendLine("</tbody>"); //tbody - 縮合功能使用
                            }

                            html.AppendLine("<tr class=\"ModifyHead DTtoggle\" style=\"cursor: pointer\" rel=\"#dt" + row + "\" imgrel=\"#img" + row + "\" title=\"展開\">");
                            html.AppendLine("<td colspan=\"5\">");
                            //顯示箭頭圖片
                            html.AppendLine("<img src=\"../images/icon_down.png\" id=\"img" + row + "\" />");
                            html.Append(string.Format("{0} - {1}({2})<em class=\"TableModifyTitleIcon\"></em></td>"
                                , fn_Desc.PubAll.AreaDesc(Area)
                                , DeptName
                                , UserCnt));
                            html.AppendLine("</tr>");
                            html.AppendLine("<tbody id=\"dt" + row + "\" style=\"display:none\">"); //tbody - 縮合功能使用
                            //[Table] - Row
                            html.AppendLine("<tr>");
                            //[Table] - Column (Content), Start ----------
                            html.AppendLine("<td class=\"TableModifyTd\">");
                            html.AppendLine("<ul class=\"as-selections\">");
                        }
                        //[HTML] - 顯示名單
                        html.AppendLine("<li class=\"as-selection-item blur\">");
                        html.AppendLine(
                            string.Format("<a href=\"Auth_SetUser.aspx?ProfileID={0}\" style=\"background:transparent;cursor:pointer;\" class=\"styleBlack infoBox\">{1}"
                            , Server.UrlEncode(DT.Rows[row]["Guid"].ToString())
                            , Display_Name
                            ));
                        html.AppendLine("<span class=\"JQ-ui-icon ui-icon-person\"></span></a>");
                        html.AppendLine("</li>");
                    }
                    //補上結尾
                    html.AppendLine("</ul>");
                    html.AppendLine("</td>");
                    html.AppendLine("</tr>");
                    html.AppendLine("</tbody>");

                    //輸出Html
                    this.lt_Content.Text = html.ToString();
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #endregion

}
