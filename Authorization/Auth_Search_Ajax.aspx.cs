using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using ExtensionMethods;

public partial class Auth_Search_Ajax : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (string.IsNullOrEmpty(Param_ID))
                {
                    Response.Write("參數傳遞錯誤");
                    return;
                }
                //填入資料
                Param_Title = Get_BaseData(Param_ID);
                Param_GroupNameList = Get_GroupData(Param_ID);
                Param_UserNameList = Get_UserData(Param_ID);
            }
            catch (Exception)
            {
                Response.Write("系統發生錯誤");
                return;
            }

        }
    }

    /// <summary>
    /// 基本資料
    /// </summary>
    /// <param name="ProgID">功能編號</param>
    /// <returns></returns>
    private string Get_BaseData(string ProgID)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT ");
                SBSql.AppendLine("  Prog.Lv_Path_Name, Prog.Prog_Name_zh_TW AS Prog_Name ");
                SBSql.AppendLine(" FROM Program Prog ");
                SBSql.AppendLine(" WHERE (Prog.Prog_ID = @Prog_ID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Prog_ID", ProgID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    //路徑名稱
                    string ProgPathName = DT.Rows[0]["Lv_Path_Name"].ToString().Replace("｜", " &gt; ");
                    //目前功能名稱
                    string ProgCurrName = DT.Rows[0]["Prog_Name"].ToString();

                    return (string.IsNullOrEmpty(ProgPathName) ? "" : ProgPathName + " &gt; ") + ProgCurrName;
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 人員名單
    /// </summary>
    /// <param name="ProgID">功能編號</param>
    /// <returns></returns>
    private string Get_GroupData(string ProgID)
    {
        try
        {
            string ErrMsg;
            StringBuilder html = new StringBuilder();
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT ");
                SBSql.AppendLine("  Data.Account_Name, Data.Display_Name, Rel.Guid ");
                SBSql.AppendLine(" FROM User_Group_Rel_Program Rel ");
                SBSql.AppendLine("	 INNER JOIN PKSYS.dbo.User_Group Data ON Rel.Guid = Data.Guid ");
                SBSql.AppendLine(" WHERE (Rel.Prog_ID = @Prog_ID) AND (Data.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Data.Display_Name ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Prog_ID", ProgID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "<span class=\"styleEarth\">-- 尚未設定 --</span>";
                    }

                    html.AppendLine("<ul>");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        html.AppendLine(string.Format("<li><a href=\"{1}\" class=\"styleBlack\">{0}</a></li>"
                            , DT.Rows[row]["Display_Name"].ToString()
                            , "Auth_SetGroup.aspx?GroupID=" + Server.UrlEncode(DT.Rows[row]["Guid"].ToString())
                            ));
                    }
                    html.AppendLine("</ul>");
                }
            }

            return html.ToString();

        }
        catch (Exception)
        {

            throw;
        }
    }

    private string Get_UserData(string ProgID)
    {
        try
        {
            string ErrMsg;
            StringBuilder html = new StringBuilder();
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT ");
                SBSql.AppendLine("  Data.Account_Name, Data.Display_Name, Rel.Guid ");
                SBSql.AppendLine(" FROM User_Profile_Rel_Program Rel ");
                SBSql.AppendLine("	 INNER JOIN PKSYS.dbo.User_Profile Data ON Rel.Guid = Data.Guid ");
                SBSql.AppendLine(" WHERE (Rel.Prog_ID = @Prog_ID) AND (Data.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Data.Display_Name, Data.Account_Name ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Prog_ID", ProgID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "<span class=\"styleEarth\">-- 尚未設定 --</span>";
                    }

                    html.AppendLine("<ul>");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        html.AppendLine(string.Format("<li><a href=\"{2}\" class=\"styleBlack\"><span class=\"styleGraylight\">{0}</span> ({1})</a></li>"
                            , DT.Rows[row]["Account_Name"].ToString()
                            , DT.Rows[row]["Display_Name"].ToString()
                            , "Auth_SetUser.aspx?ProfileID=" + Server.UrlEncode(DT.Rows[row]["Guid"].ToString())
                            ));
                    }
                    html.AppendLine("</ul>");
                }
            }

            return html.ToString();

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 參數設定 --
    /// <summary>
    /// 取得編號
    /// </summary>
    private string _Param_ID;
    public string Param_ID
    {
        get
        {
            return this._Param_ID != null ? this._Param_ID : Request.Form["dataId"].ToString();
        }
        set
        {
            this._Param_ID = value;
        }
    }

    /// <summary>
    /// 表頭
    /// </summary>
    private string _Param_Title;
    public string Param_Title
    {
        get;
        set;
    }

    /// <summary>
    /// 群組名單
    /// </summary>
    private string _Param_GroupNameList;
    public string Param_GroupNameList
    {
        get;
        set;
    }

    /// <summary>
    /// 人員名稱
    /// </summary>
    private string _Param_UserNameList;
    public string Param_UserNameList
    {
        get;
        set;
    }
    #endregion

}