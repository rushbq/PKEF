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
using LogRecord;

public partial class Auth_SetGroup : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";

            //[權限判斷] - 權限設定
            if (fn_CheckAuth.CheckAuth_User("9903", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }
            //[判斷&取得參數] - 群組GUID (Http Request)
            if (fn_Extensions.String_字數(Request.QueryString["GroupID"], "1", "40", out ErrMsg))
            {
                this.tb_Group_ID.Text = Request.QueryString["GroupID"].ToString();
                //帶出資料
                LookupData();
            }
        }
    }

    #region --按鈕區--
    //帶出權限資料
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //[初始化]
            string ErrMsg = "";

            //[判斷&取得參數] - 群組名稱/GUID
            if (fn_Extensions.String_字數(this.tb_Group_ID.Text, "1", "40", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("請選擇正確的「群組」！", "");
                return;
            }
            //帶出資料
            LookupData();
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
        }
    }

    //設定權限
    protected void btn_GetProgID_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            //[判斷參數] - GUID
            if (string.IsNullOrEmpty(this.lt_Guid.Text))
            {
                fn_Extensions.JsAlert("系統發生錯誤 - GUID遺失！", "");
                return;
            }
            //[判斷參數] - 權限
            if (string.IsNullOrEmpty(this.hf_ProgID.Value.Trim()))
            {
                fn_Extensions.JsAlert("未勾選任何權限！", "");
                return;
            }
            else
            {
                //取得權限編號
                string[] aryProgID = this.hf_ProgID.Value.Split(',');
                if (aryProgID == null)
                {
                    fn_Extensions.JsAlert("未勾選任何權限！", "");
                    return;
                }
                //設定權限
                using (SqlCommand cmd = new SqlCommand())
                {
                    //[SQL] - 清除cmd參數
                    cmd.Parameters.Clear();
                    //[SQL] - 執行SQL
                    StringBuilder SBSql = new StringBuilder();

                    #region "Log處理"
                    //[暫存參數] - 原權限ID
                    List<string> iProgID_Old = new List<string>();

                    //[SQL] - (LOG) 取得原權限ID
                    SBSql.AppendLine(" SELECT Prog_ID FROM User_Group_Rel_Program WHERE (Guid = @Param_Guid); ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Param_Guid", this.lt_Guid.Text);
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            iProgID_Old.Add(DT.Rows[i]["Prog_ID"].ToString());
                        }
                    }

                    //[暫存參數] - 新權限ID
                    List<string> iProgID_New = new List<string>();
                    for (int i = 0; i < aryProgID.Length; i++)
                    {
                        iProgID_New.Add(aryProgID[i].ToString());
                    }
                    #endregion

                    //[SQL] - 清除cmd參數
                    cmd.Parameters.Clear();
                    SBSql.Clear();
                    SBSql.AppendLine(" DELETE FROM User_Group_Rel_Program WHERE (Guid = @Param_Guid); ");
                    for (int i = 0; i < aryProgID.Length; i++)
                    {
                        SBSql.AppendLine(string.Format(
                            " INSERT INTO User_Group_Rel_Program (Guid, Prog_ID) VALUES (@Param_Guid, @Prog_ID{0});"
                            , i));

                        cmd.Parameters.AddWithValue("Prog_ID" + i, aryProgID[i].ToString());
                    }
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Param_Guid", this.lt_Guid.Text);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料儲存失敗！", "");
                        return;
                    }
                    else
                    {
                        //帶出資料
                        LookupData();

                        //寫入Log
                        fn_Log.Log_AD_withAuth("Group", "設定權限", this.lt_GroupName.Text
                            , "設定 " + this.lt_GroupName.Text + " 的使用權限"
                            , fn_Params.UserAccount
                            , iProgID_Old, iProgID_New, this.lt_Guid.Text);

                        fn_Extensions.JsAlert("資料儲存成功！", "");
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定權限！", "");
        }
    }

    //設定停啟用
    protected void btn_ChStatus_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";
            //[判斷參數] - GUID
            if (string.IsNullOrEmpty(this.lt_Guid.Text))
            {
                fn_Extensions.JsAlert("系統發生錯誤 - GUID遺失！", "");
                return;
            }
            //設定停/啟用
            using (SqlCommand cmd = new SqlCommand())
            {
                //處理描述
                string ProcDesc = "";
                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();
                //[SQL] - 執行SQL
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" UPDATE User_Group SET Display = @Param_Display, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (Guid = @Param_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", this.lt_Guid.Text);
                if (this.btn_ChStatus.Text == "設定啟用")
                {
                    cmd.Parameters.AddWithValue("Param_Display", "Y");
                    ProcDesc = string.Format("將 {0} 設定為啟用", this.lt_GroupName.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("Param_Display", "N");
                    ProcDesc = string.Format("將 {0} 設定為停用", this.lt_GroupName.Text);
                }
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("設定失敗！", "");
                    return;
                }
                else
                {
                    //帶出資料
                    LookupData();

                    //寫入Log
                    fn_Log.Log_AD("Group", "停/啟用", this.lt_GroupName.Text, ProcDesc, fn_Params.UserAccount);

                    fn_Extensions.JsAlert("設定成功！", "");
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定狀態！", "");
        }

    }
    #endregion

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
                SBSql.AppendLine(" SELECT Guid, Display_Name, Account_Name, Display, Update_Time ");
                SBSql.AppendLine(" FROM User_Group ");
                SBSql.AppendLine(" WHERE (Guid = @Param_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_Guid", this.tb_Group_ID.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無此群組 - " + this.tb_Group_Name.Text + "!", "");
                        return;
                    }

                    //[填入資料]
                    this.pl_Data.Visible = true;
                    this.lt_GroupName.Text = DT.Rows[0]["Display_Name"].ToString();  //顯示名稱
                    this.lt_Guid.Text = DT.Rows[0]["Guid"].ToString();  //GUID
                    //顯示停/啟用按鈕
                    if (DT.Rows[0]["Display"].ToString() == "Y")
                    {
                        this.btn_ChStatus.Text = "設定停用";
                        this.lb_UpdTime.Text = "";
                    }
                    else
                    {
                        this.btn_ChStatus.Text = "設定啟用";
                        this.lb_UpdTime.Text = string.Format("(停用於{0})"
                            , DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm"));
                    }
                    //[按鈕] - 加入BlockUI
                    this.btn_GetProgID.Attributes["onclick"] = fn_Extensions.BlockJs(
                        "Save",
                        "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                    //帶出權限表
                    LookupAuthData();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 基本資料！", "");
        }

    }

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
            if (CreateMenu(SBHtml, this.lt_Guid.Text, out ErrMsg))
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
    /// <param name="GUID">要查詢的GUID</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateMenu(StringBuilder SBHtml, string GUID, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SBHtml.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Rel.Prog_ID AS Rel_ID ");
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM Program LEFT JOIN User_Group_Rel_Program Rel ");
                SBSql.AppendLine("  ON Program.Prog_ID = Rel.Prog_ID AND (Rel.Guid = @Param_Guid)");
                SBSql.AppendLine(" WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_Guid", GUID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    SBHtml.AppendLine("<ul id=\"TreeView\" class=\"filetree\">");
                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        //判斷是否有該權限
                        string hasChecked = "";
                        if (string.IsNullOrEmpty(DT.Rows[i]["Rel_ID"].ToString()) == false) { hasChecked = "checked"; }

                        //顯示項目
                        SBHtml.AppendLine(string.Format(
                            "<li>" +
                            "<span class=\"folder\"><a></a></span>&nbsp;" +
                            "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"{0}\" {2}><strong class=\"Font15\">{1}</strong></label>"
                            , DT.Rows[i]["Prog_ID"].ToString()
                            , DT.Rows[i]["Prog_Name"].ToString()
                            , hasChecked));

                        //判斷是否有下層資料並回傳
                        CreateSubMenu(
                             DT.Rows[i]["Prog_ID"].ToString()
                             , SBHtml
                             , GUID
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
    /// <param name="GUID">欲查詢的GUID</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateSubMenu(string Up_ID, StringBuilder SBHtml, string GUID, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.Child_Cnt");
                SBSql.AppendLine("  , Program.Lv_Path, Rel.Prog_ID AS Rel_ID");
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM Program LEFT JOIN User_Group_Rel_Program Rel ");
                SBSql.AppendLine("  ON Program.Prog_ID = Rel.Prog_ID AND (Rel.Guid = @Param_Guid)");
                SBSql.AppendLine(" WHERE (Program.Up_Id = @UP_ID) AND (Program.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UP_ID", Up_ID);
                cmd.Parameters.AddWithValue("Param_Guid", GUID);
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
                                "<label><input type=\"checkbox\" id=\"cb_{1}\" runat=\"server\" rel=\"cb_{4}\" value=\"{0}\" {3}>{2}</label>"
                                , DT.Rows[i]["Prog_ID"].ToString()
                                , DT.Rows[i]["Lv_Path"].ToString().Replace(",", "_") + "_" + DT.Rows[i]["Prog_ID"].ToString()
                                , DT.Rows[i]["Prog_Name"].ToString()
                                , hasChecked
                                , DT.Rows[i]["Lv_Path"].ToString().Replace(",", "_")));

                            //判斷是否有下層資料並回傳
                            CreateSubMenu(
                                 DT.Rows[i]["Prog_ID"].ToString()
                                 , SBHtml
                                 , GUID
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
