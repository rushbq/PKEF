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
using System.Text.RegularExpressions;

public partial class Auth_Search : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 權限設定
                if (fn_CheckAuth.CheckAuth_User("9907", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[帶出選單] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, Param_DeptID, true, false, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[帶出選單] - 人員
                if (fn_Extensions.Menu_ADUser(this.ddl_Employee, Request.QueryString["Employee"], true, Param_DeptID, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 資料取得 - init
    /// </summary>
    void LookupData()
    {
        LookupData("TW", this.lt_TreeView_TW);
        LookupData("SH", this.lt_TreeView_SH);
        LookupData("SZ", this.lt_TreeView_SZ);
    }

    /// <summary>
    /// 資料取得 - 樹狀選單
    /// </summary>
    void LookupData(string Area, Literal ltName)
    {
        try
        {
            string ErrMsg;
            StringBuilder SBHtml = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Dept.DeptID, Dept.DeptName ");
                SBSql.AppendLine("    , Prof.Account_Name, Prof.Display_Name, UserList.View_Account AS IsChecked ");
                SBSql.AppendLine("    , ROW_NUMBER() OVER(PARTITION BY Dept.DeptID ORDER BY Dept.Area_Sort, Dept.DeptID, Prof.Account_Name ASC) AS GP_Rank ");
                SBSql.AppendLine("    , ( ");
                SBSql.AppendLine("        SELECT COUNT(*) ");
                SBSql.AppendLine("        FROM User_Profile ");
                SBSql.AppendLine("            INNER JOIN PKEF.dbo.TTD_ViewAuth UserList ON User_Profile.Account_Name = UserList.View_Account ");
                SBSql.AppendLine("        WHERE (User_Profile.DeptID = Dept.DeptID) AND (UserList.Account_Name = @AuthWho) ");
                SBSql.AppendLine("    ) AS SetCnt ");
                SBSql.AppendLine(" FROM User_Dept Dept ");
                SBSql.AppendLine("    INNER JOIN User_Profile Prof ON Dept.DeptID = Prof.DeptID ");
                SBSql.AppendLine("    LEFT JOIN PKEF.dbo.TTD_ViewAuth UserList ");
                SBSql.AppendLine("       ON Prof.Account_Name = UserList.View_Account AND (UserList.Account_Name = @AuthWho) ");
                SBSql.AppendLine(" WHERE (Dept.Area = @Area) AND (Dept.Display = 'Y') AND (Prof.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Dept.Area_Sort, Dept.DeptID, Prof.Account_Name ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Area", Area);
                cmd.Parameters.AddWithValue("AuthWho", this.ddl_Employee.SelectedValue);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ltName.Text = "<div style=\"padding:5px 5px 15px 5px\"><span class=\"JQ-ui-icon ui-icon-alert\"></span>人員資料不存在，請聯絡系統管理員。</div>";
                        return;
                    }

                    //[Html] - 根目錄
                    SBHtml.AppendLine("<ul class=\"TreeView filetree\">");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //[取得欄位資料]
                        #region * 取得欄位資料 *
                        string DeptID = DT.Rows[row]["DeptID"].ToString();
                        string DeptName = DT.Rows[row]["DeptName"].ToString();
                        string GP_Rank = DT.Rows[row]["GP_Rank"].ToString();
                        string Account_Name = DT.Rows[row]["Account_Name"].ToString();
                        string Display_Name = DT.Rows[row]["Display_Name"].ToString();
                        string IsChecked = DT.Rows[row]["IsChecked"].ToString();
                        int SetCnt = Convert.ToInt32(DT.Rows[row]["SetCnt"]);
                        #endregion

                        //[HTML] - 顯示分類, 每類標頭 (GP_Rank = 1)
                        if (Convert.ToInt16(GP_Rank).Equals(1))
                        {
                            SBHtml.AppendLine(string.Format(
                                "<li>" +
                                "<span class=\"folder\"><a></a></span>&nbsp;" +
                                "<label><input type=\"checkbox\" id=\"{3}_cb_{0}\" runat=\"server\" value=\"\" {2}><strong class=\"Font14\">{1}</strong></label>"
                                , DeptID
                                , DeptName
                                , (SetCnt > 0) ? "checked" : ""
                                , Area));

                            //[HTML] - 子層的tag開頭
                            SBHtml.AppendLine(" <ul>");
                        }

                        //[HTML] - 規格內容
                        SBHtml.AppendLine(string.Format(
                                  "<li><span class=\"file\"><a></a></span>&nbsp;" +
                                  "<label><input type=\"checkbox\" id=\"{5}_cb_{0}\" runat=\"server\" value=\"{3}\" rel=\"{5}_cb_{4}\" {2}><font class=\"styleBlue\">{1}</font></label>"
                                  , DeptID + Account_Name
                                  , Account_Name + " - " + Display_Name
                                  , string.IsNullOrEmpty(IsChecked) ? "" : "checked"
                                  , Account_Name
                                  , DeptID
                                  , Area
                                  ));
                        SBHtml.AppendLine(" </li>");

                        /* [HTML]
                         * 計算每類的資料數, (GP_Rank = 總數)
                         * 顯示子層的tag結尾
                         */
                        var queryCnt =
                            from el in DT.AsEnumerable()
                            where el.Field<string>("DeptID").Equals(DeptID)
                            select el;
                        if (Convert.ToInt32(GP_Rank).Equals(queryCnt.Count()))
                        {
                            SBHtml.AppendLine(" </ul>");
                            SBHtml.AppendLine("</li>");
                        }
                    }
                    SBHtml.AppendLine("</ul>");
                }
            }

            //輸出Html
            ltName.Text = SBHtml.ToString();
        }
        catch (Exception)
        {
            throw new Exception("資料取得發生錯誤");
        }
    }
    #endregion


    #region --按鈕區--
    /// <summary>
    /// 切換部門
    /// </summary>
    protected void ddl_Dept_SelectedIndexChanged(object sender, EventArgs e)
    {
        string ErrMsg;
        if (this.ddl_Dept.SelectedIndex > 0)
        {
            //隱藏TreeView
            this.ph_viewer.Visible = false;

            //[帶出選單] - 人員
            if (fn_Extensions.Menu_ADUser(this.ddl_Employee, Request.QueryString["Employee"], true, this.ddl_Dept.SelectedValue, out ErrMsg) == false)
            {
                this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
            }
        }
    }

    /// <summary>
    /// 帶出人員設定資料
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        doSearch();
    }

    protected void ddl_Employee_SelectedIndexChanged(object sender, EventArgs e)
    {
        doSearch();
    }

    private void doSearch()
    {
        try
        {
            if (string.IsNullOrEmpty(this.ddl_Employee.SelectedValue))
            {
                this.ph_viewer.Visible = false;

                fn_Extensions.JsAlert("人員未選擇", "");
                return;
            }
            else
            {
                this.ph_viewer.Visible = true;

                //目前選擇人員
                this.lb_Employee.Text = this.ddl_Employee.SelectedItem.Text;

                //帶出資料
                LookupData();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 儲存設定
    /// </summary>
    protected void btn_GetRelID_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            //取得關聯編號, 先分析Checkbox(,) , 再分析Value(|)
            string[] aryRelID = Regex.Split(this.hf_RelID.Value, @"\,{1}");
            if (aryRelID == null)
            {
                fn_Extensions.JsAlert("設定失敗！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                //[SQL] - 刪除原關聯
                SBSql.AppendLine(" DELETE FROM TTD_ViewAuth WHERE (Account_Name = @AuthWho) ");
                SBSql.AppendLine("  AND Account_Name IN ( ");
                SBSql.AppendLine("    SELECT Account_Name ");
                SBSql.AppendLine("    FROM PKSYS.dbo.User_Profile Prof INNER JOIN PKSYS.dbo.User_Dept Dept");
                SBSql.AppendLine("        ON Prof.DeptID = Dept.DeptID");
                SBSql.AppendLine("  )");
                if (false == string.IsNullOrEmpty(this.hf_RelID.Value))
                {
                    //[SQL] - 新增關聯
                    for (int row = 0; row < aryRelID.Length; row++)
                    {
                        //分析Value(|)
                        string[] aryValue = Regex.Split(aryRelID[row], @"\|{1}");
                        SBSql.AppendLine(string.Format(
                            " INSERT INTO TTD_ViewAuth(Account_Name, View_Account) VALUES (@AuthWho, '{0}'); "
                            , aryValue[0]));
                    }
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AuthWho", this.ddl_Employee.SelectedValue);
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("設定失敗！", "");
                    return;
                }
                else
                {
                    //帶出資料
                    LookupData();

                    fn_Extensions.JsAlert("設定成功！", "");
                    return;
                }

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定關聯！", "");
            return;
        }
    }

    #endregion

    #region --參數設定--
    /// <summary>
    /// 部門代號
    /// </summary>
    private string _Param_DeptID;
    public string Param_DeptID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["DeptID"]) ? "" : Request.QueryString["DeptID"].ToString();
        }
        set
        {
            this._Param_DeptID = value;
        }
    }
    #endregion

}
