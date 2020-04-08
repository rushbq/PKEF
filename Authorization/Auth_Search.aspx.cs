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

public partial class Auth_Search : SecurityIn
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
            LookupAuthData();
        }
    }

    #region --權限資料--
    /// <summary>
    /// 權限資料
    /// </summary>
    void LookupAuthData()
    {
        try
        {
            string ErrMsg = "";
            //[取得資料] - 權限資料
            StringBuilder SBHtml = new StringBuilder();
            if (CreateMenu(SBHtml, out ErrMsg))
                this.lt_AuthProg.Text = SBHtml.ToString();
            else
                Response.Write("無任何權限項目...." + ErrMsg);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取權限資料！", "");
        }

    }

    /// <summary>
    /// [建立選單] - 第一層
    /// </summary>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateMenu(StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SBHtml.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort ");
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM Program ");
                SBSql.AppendLine(" WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    SBHtml.AppendLine("<ul id=\"TreeView\" class=\"filetree\">");
                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        //顯示項目
                        SBHtml.AppendLine(string.Format(
                            "<li>" +
                            "<span class=\"folder\"><a></a></span>&nbsp;" +
                            "<label><strong class=\"Font15\">{0}</strong></label>"
                            , DT.Rows[i]["Prog_Name"].ToString()));

                        //判斷是否有下層資料並回傳
                        CreateSubMenu(
                             DT.Rows[i]["Prog_ID"].ToString()
                             , SBHtml
                             , out ErrMsg);

                        SBHtml.AppendLine("</li>");
                    }
                    SBHtml.AppendLine("</ul>");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// [建立選單] - 第二層及以後的階層
    /// </summary>
    /// <param name="Up_ID">上一層編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateSubMenu(string Up_ID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.Child_Cnt, Program.Lv_Path");
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM Program ");
                SBSql.AppendLine(" WHERE (Program.Up_Id = @UP_ID) AND (Program.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UP_ID", Up_ID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        SBHtml.AppendLine("<ul>");
                        for (int i = 0; i <= DT.Rows.Count - 1; i++)
                        {
                            int Child_Cnt = Convert.ToInt32(DT.Rows[i]["Child_Cnt"]);
                            //顯示項目
                            SBHtml.AppendLine(string.Format(
                                "<li id=\"li_{0}\">" +
                                "<span class=\"" + SubMenuCss(Child_Cnt) + "\"><a></a></span>&nbsp;<label>{1}</label>&nbsp;{2}"
                                , DT.Rows[i]["Prog_ID"].ToString()
                                , DT.Rows[i]["Prog_Name"].ToString()
                                , "<a class=\"AuthSearch\" dataId=\"" + DT.Rows[i]["Prog_ID"].ToString() + "\">(查詢)</a>"
                                ));

                            //判斷是否有下層資料並回傳
                            CreateSubMenu(
                                 DT.Rows[i]["Prog_ID"].ToString()
                                 , SBHtml
                                 , out ErrMsg);

                            SBHtml.AppendLine("</li>");
                        }
                        SBHtml.AppendLine("</ul>");
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    //[建立選單] - 判斷是否有子項目,顯示不同Css樣式
    private string SubMenuCss(int Child_Cnt)
    {
        if (Child_Cnt > 0)
            return "folder";
        else
            return "file";
    }
    #endregion

}
