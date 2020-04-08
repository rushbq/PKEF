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

public partial class WR_Class_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 類別維護
                if (fn_CheckAuth.CheckAuth_User("310", out ErrMsg) == false && fn_CheckAuth.CheckAuth_User("142", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無權限使用本功能！", "script:parent.$.fancybox.close()");
                    return;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, Param_DeptID, true, false, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[參數判斷] - ClassID
                if (false == string.IsNullOrEmpty(Param_thisID))
                {
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

            //[取得/檢查參數] - 系統編號
            if (fn_Extensions.Num_正整數(Param_thisID, "1", "999999999", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT * ");
                SBSql.AppendLine(" FROM TTD_Class ");
                SBSql.AppendLine(" WHERE (Class_ID = @Class_ID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Class_ID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "script:parent.$.fancybox.close()");
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.hf_UID.Value = DT.Rows[0]["Class_ID"].ToString();
                        this.tb_ClassName.Text = DT.Rows[0]["Class_Name"].ToString();
                        this.ddl_Dept.SelectedValue = DT.Rows[0]["DeptID"].ToString();
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString(); ;
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                    }
                }
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 讀取資料");
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
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] 
            string ClassName = string.IsNullOrEmpty(this.tb_ClassName.Text) ? "" : this.tb_ClassName.Text.Trim();
            if (fn_Extensions.String_字數(ClassName, "1", "50", out ErrMsg) == false)
            {
                SBAlert.Append("「類別名稱」請輸入1 ~ 50個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "資料儲存"
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data();
                    break;

                case "EDIT":
                    Edit_Data();
                    break;

                default:
                    throw new Exception("走錯路囉!");
            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //--- 取得新編號 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 取得最新編號
            int New_ID;
            SBSql.AppendLine(" SELECT (ISNULL(MAX(Class_ID), 0) + 1) AS New_ID FROM TTD_Class ");
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                New_ID = Convert.ToInt32(DT.Rows[0]["New_ID"]);
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO TTD_Class( ");
            SBSql.AppendLine("  Class_ID, Class_Name, Display, Sort, DeptID");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @New_ID, @Class_Name, @Display, @Sort, @DeptID");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("New_ID", New_ID);
            cmd.Parameters.AddWithValue("Class_Name", this.tb_ClassName.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("DeptID", this.ddl_Dept.SelectedValue);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", PageUrl);
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！", "script:parent.location.reload();");
                return;
            }
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

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE TTD_Class ");
            SBSql.AppendLine(" SET Class_Name = @Class_Name, Display = @Display, Sort = @Sort, DeptID = @DeptID");
            SBSql.AppendLine(" WHERE (Class_ID = @UID) ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("UID", this.hf_UID.Value);
            cmd.Parameters.AddWithValue("Class_Name", this.tb_ClassName.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("DeptID", this.ddl_Dept.SelectedValue);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", "script:parent.location.reload();");
                return;
            }
        }
    }

    #endregion -- 資料編輯 End --

    #region -- 參數設定 --
    /// <summary>
    /// 部門代號
    /// </summary>
    private string _Param_DeptID;
    public string Param_DeptID
    {
        get
        {
            /* 判斷權限 
             *   管理者權限(311):帶出所有部門
             */
            string ErrMsg;
            string deptID = "";

            //無:管理者權限，有:一般權限
            if (fn_CheckAuth.CheckAuth_User("311", out ErrMsg) == false)
            {
                //取得目前使用者的部門
                deptID = ADService.getDepartmentFromGUID(fn_Params.UserGuid);
                //鎖定部門選單
                this.ddl_Dept.Enabled = false;
            }

            //若deptID為空，則判斷是否有request
            return (string.IsNullOrEmpty(deptID)) ? "" : deptID;
        }
        set
        {
            this._Param_DeptID = value;
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
            return string.IsNullOrEmpty(Request.QueryString["Class_ID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["Class_ID"].ToString());
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
            return string.Format(@"WR_Class_Edit.aspx?Class_ID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_thisID)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    #endregion

}
