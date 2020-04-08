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
using System.Text.RegularExpressions;
using System.Collections;

public partial class InfoSales_Edit : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 業務資料維護
                if (fn_CheckAuth.CheckAuth_User("410", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無權限使用本功能！", "script:parent.$.fancybox.close()");
                    return;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");


                //取得公司別
                Get_CorpList();

                //讀取資料
                if (false == string.IsNullOrEmpty(Param_thisID))
                {
                    //讀取資料
                    View_Data();
                }

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 讀取資料
    /// </summary>
    private void View_Data()
    {
        try
        {
            string ErrMsg;

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                cmd.Parameters.Clear();

                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Prof.Display_Name, Prof.Account_Name, Prof.ERP_LoginID, Prof.ERP_UserID ");
                SBSql.AppendLine("  , Corp.Corp_UID");
                SBSql.AppendLine("    FROM User_Profile Prof ");
                SBSql.AppendLine("    INNER JOIN User_Dept Dept ON Prof.DeptID = Dept.DeptID");
                SBSql.AppendLine("    LEFT JOIN Staff_Rel_Corp Corp ON Prof.Account_Name = Corp.StaffID");
                SBSql.AppendLine("    WHERE (Prof.Display = 'Y') AND (Prof.Account_Name = @UserID) ");
                //[查詢條件] - 區域別
                SBSql.Append(" AND (Dept.Area IN ({0}))".FormatThis(fn_Extensions.GetSQLParam(Param_AreaCode, "Area")));

                for (int row = 0; row < Param_AreaCode.Count; row++)
                {
                    cmd.Parameters.AddWithValue("Area{0}".FormatThis(row), Param_AreaCode[row].ToString());
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("UserID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "script:parent.$.fancybox.close()");
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.lb_UserID.Text = DT.Rows[0]["Account_Name"].ToString();
                        this.lb_UserName.Text = DT.Rows[0]["Display_Name"].ToString();

                        //公司別
                        CheckBoxList cbl = this.cbl_Corp;

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            for (int col = 0; col < cbl.Items.Count; col++)
                            {
                                if (cbl.Items[col].Value.Equals(DT.Rows[row]["Corp_UID"].ToString()))
                                {
                                    cbl.Items[col].Selected = true;
                                }
                            }

                        }

                    }
                }
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 讀取資料");
        }
    }

    /// <summary>
    /// 取得公司別
    /// </summary>
    /// <returns></returns>
    private bool Get_CorpList()
    {
        try
        {
            this.cbl_Corp.Items.Clear();

            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                // ↓↓ SQL查詢組成 ↓↓
                SBSql.AppendLine(" SELECT Corp_UID AS ID, Corp_Name AS Label ");
                SBSql.AppendLine(" FROM Param_Corp WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort ");
                cmd.CommandText = SBSql.ToString();
                // ↑↑ SQL查詢組成 ↑↑

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        this.cbl_Corp.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString() + "&nbsp;&nbsp;&nbsp;"
                                     , DT.Rows[row]["ID"].ToString()));
                    }

                }
            }

            return true;
        }
        catch (Exception )
        {
            return false;
        }
    }

    #endregion

    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            Edit_Data();

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }


    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {

            StringBuilder SBSql = new StringBuilder();
           
            var GetCbItems = from ListItem item in this.cbl_Corp.Items where item.Selected select item.Value;
            if (GetCbItems.Count() > 0)
            {
                //先清除資料
                SBSql.AppendLine(" DELETE FROM Staff_Rel_Corp WHERE (StaffID = @DataID);");

                int row = 0;

                foreach (var itemVal in GetCbItems)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Staff_Rel_Corp( ");
                    SBSql.AppendLine("  StaffID, Corp_UID");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @DataID, @Corp_UID_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("Corp_UID_{0}".FormatThis(row), itemVal);
                }
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", PageUrl);
                return;
            }
            else
            {
                fn_Extensions.JsAlert("", "script:parent.$.fancybox.close()");
            }

        }
    }

    #endregion -- 資料編輯 End --

    #region -- 參數設定 --

    private List<string> _Param_AreaCode;
    public List<string> Param_AreaCode
    {
        get
        {
            string ErrMsg;
            return fn_Extensions.GetAreaCode("411#412#413", fn_Params.UserGuid, out ErrMsg);
        }
        set
        {
            this._Param_AreaCode = value;
        }
    }

    /// <summary>
    /// 本筆資料的編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["UserID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["UserID"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return string.Format(@"InfoSales_Corp_Edit.aspx?UserID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_thisID)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    #endregion

}
