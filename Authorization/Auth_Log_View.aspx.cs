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

public partial class Auth_SetGroup : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";

            //[權限判斷] - 異動記錄
            if (fn_CheckAuth.CheckAuth_User("9905", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("無使用權限！", "script:parent.$.fancybox.close();");
            }
            //[取得/檢查參數] - LogID(系統編號)
            Param_LogID = string.IsNullOrEmpty(Request.QueryString["LogID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["LogID"].ToString());
            if (fn_Extensions.Num_正整數(Param_LogID, "1", "999999999", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close();");
            }

            //帶出資料
            LookupData();
        }
    }

    #region --權限資料--
    /// <summary>
    /// 基本資料
    /// </summary>
    void LookupData()
    {
        try
        {
            string ErrMsg = "";
            //[取得資料] - 基本資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT  Proc_Time, Proc_Type, Proc_Action, Proc_Account ");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Log_AD.Create_Who)) AS Create_Name ");
                SBSql.AppendLine(" FROM Log_AD ");
                SBSql.AppendLine(" WHERE (Log_ID = @Param_LogID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_LogID", Param_LogID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料", "script:parent.$.fancybox.close();");
                    }

                    //[填入資料]
                    this.lt_ProcAccount.Text = DT.Rows[0]["Proc_Account"].ToString();  //異動帳戶
                    this.lt_ProcTime.Text = DT.Rows[0]["Proc_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");  //異動時間
                    this.lt_ProcAction.Text = DT.Rows[0]["Proc_Action"].ToString();  //異動動作
                    this.lt_ProcWho.Text = DT.Rows[0]["Create_Name"].ToString();  //異動處理者

                    //[取得資料] - 原權限資料
                    StringBuilder SBHtml = new StringBuilder();
                    if (CreateMenu(SBHtml, "Old", DT.Rows[0]["Proc_Type"].ToString(), out ErrMsg))
                        this.lt_OldAuthProg.Text = SBHtml.ToString();
                    else
                        this.lt_OldAuthProg.Text = "";

                    //[取得資料] - 新權限資料
                    SBHtml.Clear();
                    if (CreateMenu(SBHtml, "New", DT.Rows[0]["Proc_Type"].ToString(), out ErrMsg))
                        this.lt_NewAuthProg.Text = SBHtml.ToString();
                    else
                        this.lt_NewAuthProg.Text = "";
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料", "");
        }

    }

    /// <summary>
    /// [建立選單] - 第一層
    /// </summary>
    /// <param name="SBHtml">Html</param>
    /// <param name="LogType">原權限(Old)/新權限(New)</param>
    /// <param name="ProcType">處理類別(Group/User)</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateMenu(StringBuilder SBHtml, string LogType, string ProcType
        , out string ErrMsg)
    {
        try
        {
            switch (ProcType)
            {
                case "Group":
                    Param_LogTable = "Log_User_Group_Rel_Program";
                    break;

                case "User":
                    Param_LogTable = "Log_User_Profile_Rel_Program";
                    break;

                default:
                    ErrMsg = "Type錯誤";
                    return false;
            }
            using (SqlCommand cmd = new SqlCommand())
            {
                SBHtml.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Rel.Prog_ID AS Rel_ID ");
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine(string.Format(" FROM Program LEFT JOIN {0} AS Rel ", Param_LogTable));
                SBSql.AppendLine("  ON Program.Prog_ID = Rel.Prog_ID AND (Rel.Log_ID = @Param_LogID) AND (LogType = @Param_LogType)");
                SBSql.AppendLine(" WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_LogID", Param_LogID);
                cmd.Parameters.AddWithValue("Param_LogType", LogType);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    SBHtml.AppendLine(string.Format("<ul id=\"TreeView_{0}\" class=\"filetree\">", LogType));
                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        //判斷是否有該權限
                        string hasChecked = "";
                        if (string.IsNullOrEmpty(DT.Rows[i]["Rel_ID"].ToString()) == false) { hasChecked = "checked"; }

                        //顯示項目
                        SBHtml.AppendLine(string.Format(
                            "<li>" +
                            "<span class=\"folder\"><a></a></span>&nbsp;" +
                            "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"{0}\" {2} disabled><strong class=\"Font15\">{1}</strong></label>"
                            , DT.Rows[i]["Prog_ID"].ToString()
                            , DT.Rows[i]["Prog_Name"].ToString()
                            , hasChecked));

                        //判斷是否有下層資料並回傳
                        CreateSubMenu(
                             DT.Rows[i]["Prog_ID"].ToString()
                             , SBHtml
                             , LogType
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
    /// <param name="LogType">原權限(Old)/新權限(New)</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateSubMenu(string Up_ID, StringBuilder SBHtml, string LogType, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.Child_Cnt");
                SBSql.AppendLine("  , Program.Lv_Path, Rel.Prog_ID AS Rel_ID");
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine(string.Format(" FROM Program LEFT JOIN {0} AS Rel ", Param_LogTable));
                SBSql.AppendLine("  ON Program.Prog_ID = Rel.Prog_ID AND (Rel.Log_ID = @Param_LogID) AND (LogType = @Param_LogType)");
                SBSql.AppendLine(" WHERE (Program.Up_Id = @UP_ID) AND (Program.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UP_ID", Up_ID);
                cmd.Parameters.AddWithValue("Param_LogID", Param_LogID);
                cmd.Parameters.AddWithValue("Param_LogType", LogType);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        SBHtml.AppendLine("<ul>");
                        for (int i = 0; i <= DT.Rows.Count - 1; i++)
                        {
                            //判斷是否有該權限
                            string hasChecked = "";
                            if (string.IsNullOrEmpty(DT.Rows[i]["Rel_ID"].ToString()) == false) { hasChecked = "checked"; }

                            //顯示項目
                            SBHtml.AppendLine(string.Format(
                                "<li id=\"li_{0}\">" +
                                "<span class=\"" + SubMenuCss(Convert.ToInt32(DT.Rows[i]["Child_Cnt"])) + "\"><a></a></span>&nbsp;" +
                                "<label><input type=\"checkbox\" id=\"cb_{1}\" runat=\"server\" rel=\"cb_{4}\" value=\"{0}\" {3} disabled>{2}</label>"
                                , DT.Rows[i]["Prog_ID"].ToString()
                                , DT.Rows[i]["Lv_Path"].ToString().Replace(",", "_") + "_" + DT.Rows[i]["Prog_ID"].ToString()
                                , DT.Rows[i]["Prog_Name"].ToString()
                                , hasChecked
                                , DT.Rows[i]["Lv_Path"].ToString().Replace(",", "_")));

                            //判斷是否有下層資料並回傳
                            CreateSubMenu(
                                 DT.Rows[i]["Prog_ID"].ToString()
                                 , SBHtml
                                 , LogType
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

    //[參數] - 編號
    private string _Param_LogID;
    public string Param_LogID
    {
        get;
        set;
    }

    //[參數] - Log資料表
    private string _Param_LogTable;
    public string Param_LogTable
    {
        get;
        set;
    }
}
