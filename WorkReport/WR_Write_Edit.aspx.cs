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

public partial class WR_Write_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 填寫日誌
                if (fn_CheckAuth.CheckAuth_User("320", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無權限使用本功能！", "script:setTimeout(function () { parent.window.hs.getExpander().close(); }, 900);");
                    return;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - 類別
                if (fn_Extensions.Menu_TTDClass(this.ddl_Class, "", true, Param_DeptID, out ErrMsg) == false)
                {
                    this.ddl_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[參數判斷] - Task_ID
                if (string.IsNullOrEmpty(Param_thisID))
                {
                    this.tb_TaskDate.Text = DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd");
                }
                else
                {
                    //讀取資料
                    View_Data();
                }
                //預設欄位值
                this.tb_DateNow.Text = DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd");

                //載入常用類別
                Create_HotClass();
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
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:setTimeout(function () { parent.window.hs.getExpander().close(); }, 900);");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT * ");
                SBSql.AppendLine(" FROM TTD_Task ");
                SBSql.AppendLine(" WHERE (Task_ID = @Task_ID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Task_ID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "script:setTimeout(function () { parent.window.hs.getExpander().close(); }, 900);");
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.hf_UID.Value = DT.Rows[0]["Task_ID"].ToString();
                        this.lb_Task_ID.Text = DT.Rows[0]["Task_ID"].ToString();
                        this.tb_TaskName.Text = DT.Rows[0]["Task_Name"].ToString();
                        this.ddl_Class.SelectedValue = DT.Rows[0]["Class_ID"].ToString();
                        this.tb_Remark.Text = DT.Rows[0]["Remark"].ToString();
                        this.tb_TaskDate.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy/MM/dd");

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";

                        //判斷是否為過去的資料(今日以前)
                        DateTime createDate = Convert.ToDateTime(DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy/MM/dd"));
                        DateTime currDate = Convert.ToDateTime(DateTime.Now.ToString().ToDateString("yyyy/MM/dd"));
                        if (createDate < currDate)
                        {
                            //鎖定欄位
                            this.tb_TaskDate.Enabled = false;
                            this.cv_Date.Enabled = false;
                            this.ddl_Class.Enabled = false;
                            this.tb_TaskName.Enabled = false;
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
    /// 常用類別 (Top 3)
    /// </summary>
    private void Create_HotClass()
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT TOP 3 Cls.Class_ID, Cls.Class_Name, COUNT(Cls.Class_ID) AS Cnt ");
                SBSql.AppendLine(" FROM TTD_Class Cls INNER JOIN TTD_Task Task ON Cls.Class_ID = Task.Class_ID ");
                SBSql.AppendLine(" WHERE (Task.Create_Who = @Create_Who) AND (Cls.Display = 'Y') ");
                SBSql.AppendLine(" GROUP BY Cls.Class_ID, Cls.Class_Name ");
                SBSql.AppendLine(" ORDER BY Cnt DESC ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Task_ID", Param_thisID);
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserAccount);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.pl_Hot.Visible = false;
                    }
                    else
                    {
                        this.pl_Hot.Visible = true;

                        //填入選項
                        this.rbl_HotClass.Items.Clear();
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            this.rbl_HotClass.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString() + "&nbsp;"
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }

                    }
                }
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 常用類別");
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

            //[參數檢查] - 類別
            if (this.ddl_Class.SelectedIndex > 0)
            {
                input_Class = this.ddl_Class.SelectedValue;
            }
            else
            {
                input_Class = this.rbl_HotClass.SelectedValue;
            }
            if (string.IsNullOrEmpty(input_Class))
            {
                SBAlert.Append("請選擇「類別」\\n");
            }
            //[參數檢查] - 日期
            string TaskDate = string.IsNullOrEmpty(this.tb_TaskDate.Text) ? "" : this.tb_TaskDate.Text.Trim();
            if (!fn_Extensions.IsDate(TaskDate))
            {
                SBAlert.Append("「日期」格式錯誤(ex:2010/05/12)\\n");
            }
            //[參數檢查] - 工作項目
            string TaskName = string.IsNullOrEmpty(this.tb_TaskName.Text) ? "" : this.tb_TaskName.Text.Trim();
            if (fn_Extensions.String_資料長度Byte(TaskName, "1", "60", out ErrMsg) == false)
            {
                SBAlert.Append("「工作項目」請輸入1 ~ 30個字\\n");
            }
            //[參數檢查] - 備註
            string Remark = string.IsNullOrEmpty(this.tb_Remark.Text) ? "" : this.tb_Remark.Text.Trim();
            if (fn_Extensions.String_資料長度Byte(Remark, "0", "10000", out ErrMsg) == false)
            {
                SBAlert.Append("「備註」請輸入1 ~ 5000個字\\n");
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
            SBSql.AppendLine(" SELECT (ISNULL(MAX(Task_ID), 0) + 1) AS New_ID FROM TTD_Task ");
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
            SBSql.AppendLine(" INSERT INTO TTD_Task( ");
            SBSql.AppendLine("  Task_ID, Task_Name, Class_ID, Remark, Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @New_ID, @Task_Name, @Class_ID, @Remark, @Create_Who, @Create_Time");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("New_ID", New_ID);
            cmd.Parameters.AddWithValue("Task_Name", fn_stringFormat.Filter_String(this.tb_TaskName.Text.Trim()));
            cmd.Parameters.AddWithValue("Class_ID", input_Class);
            cmd.Parameters.AddWithValue("Remark", fn_stringFormat.Filter_String(this.tb_Remark.Text));
            cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserAccount);
            cmd.Parameters.AddWithValue("Create_Time", this.tb_TaskDate.Text.Trim());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！", "script:parent.location.href='WR_Write.aspx'");
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
            SBSql.AppendLine(" UPDATE TTD_Task ");
            SBSql.AppendLine(" SET Task_Name = @Task_Name, Class_ID = @Class_ID, Remark = @Remark, Create_Time = @Create_Time, Update_Time = GETDATE()");
            SBSql.AppendLine(" WHERE (Task_ID = @UID) ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("UID", this.hf_UID.Value);
            cmd.Parameters.AddWithValue("Task_Name", fn_stringFormat.Filter_String(this.tb_TaskName.Text.Trim()));
            cmd.Parameters.AddWithValue("Class_ID", input_Class);
            cmd.Parameters.AddWithValue("Remark", fn_stringFormat.Filter_String(this.tb_Remark.Text));
            cmd.Parameters.AddWithValue("Create_Time", this.tb_TaskDate.Text.Trim());
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
            //取得目前使用者的部門
            return ADService.getDepartmentFromGUID(fn_Params.UserGuid);
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
            return string.IsNullOrEmpty(Request.QueryString["Task_ID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["Task_ID"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 輸入 - 類別
    /// </summary>
    private string _input_Class;
    public string input_Class
    {
        get;
        set;
    }
    #endregion

}
