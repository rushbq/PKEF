using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using ExtensionMethods;
using LogRecord;

public partial class Auth_SetProfile : SecurityIn
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
            //[判斷&取得參數] - 使用者GUID (Http Request)
            if (fn_Extensions.String_字數(Request.QueryString["ProfileID"], "1", "40", out ErrMsg))
            {
                this.tb_Profile_ID.Text = Request.QueryString["ProfileID"].ToString();
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

            //[判斷&取得參數] - 使用者名稱/GUID
            if (fn_Extensions.String_字數(this.tb_Profile_ID.Text, "1", "40", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("請選擇正確的「使用者」！", "");
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
            string ErrMsg;

            //[欄位檢查] - 權限編號
            string inputValue = this.hf_ProgID.Value;
            if (string.IsNullOrEmpty(inputValue))
            {
                fn_Extensions.JsAlert("未勾選任何選項，無法存檔", "");
                return;
            }

            //[取得參數值] - 編號組合
            string[] strAry = Regex.Split(inputValue, @"\|{2}");
            var query = from el in strAry
                        select new
                        {
                            Val = el.ToString().Trim()
                        };

            //[資料儲存]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();


                #region >> Log參數處理 <<

                //[宣告參數] - 原權限ID
                List<string> iProgID_Old = new List<string>();

                //[SQL] - 取得原權限ID (記錄LOG用) 
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Profile_Rel_Program WHERE (Guid = @Param_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", Param_Guid);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        iProgID_Old.Add(DT.Rows[row]["Prog_ID"].ToString());
                    }
                }

                //[宣告參數] - 新權限ID (記錄LOG用) 
                List<string> iProgID_New = new List<string>();
                foreach (var item in query)
                {
                    iProgID_New.Add(item.Val);
                }

                #endregion

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 清除關聯
                SBSql.AppendLine(" DELETE FROM User_Profile_Rel_Program WHERE (Guid = @Param_Guid); ");

                //[SQL] - 資料新增
                int idx = 0;
                foreach (var item in query)
                {
                    idx++;
                    SBSql.AppendLine(" INSERT INTO User_Profile_Rel_Program (Guid, Prog_ID) ");
                    SBSql.AppendLine(" VALUES (@Param_Guid, @Prog_ID" + idx + "); ");

                    cmd.Parameters.AddWithValue("Prog_ID" + idx, item.Val);
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", Param_Guid);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", "");
                    return;
                }
                else
                {
                    //載入資料
                    LookupData();

                    string empNo = this.lt_ProfileName.Text;

                    //寫入Log
                    if (false == fn_Log.Log_AD_withAuth(
                         "User"
                         , "設定權限"
                         , empNo
                         , "設定個人使用權限{0}".FormatThis(empNo)
                         , fn_Params.UserAccount
                         , iProgID_Old
                         , iProgID_New
                         , this.lt_Guid.Text))
                    {
                        fn_Extensions.JsAlert("權限已設定, Log處理失敗", "");
                        return;
                    }
                    else
                    {
                        fn_Extensions.JsAlert("設定成功", "");
                        return;
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
                SBSql.AppendLine(" UPDATE User_Profile SET Display = @Param_Display, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (Guid = @Param_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", this.lt_Guid.Text);
                if (this.btn_ChStatus.Text == "設定啟用")
                {
                    cmd.Parameters.AddWithValue("Param_Display", "Y");
                    ProcDesc = string.Format("將 {0} 設定為啟用", this.lt_ProfileName.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("Param_Display", "N");
                    ProcDesc = string.Format("將 {0} 設定為停用", this.lt_ProfileName.Text);
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
                    fn_Log.Log_AD("Group", "停/啟用", this.lt_ProfileName.Text, ProcDesc, fn_Params.UserAccount);

                    fn_Extensions.JsAlert("設定成功！", "");
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定狀態！", "");
        }
    }

    //移除權限
    protected void btn_Remove_Click(object sender, EventArgs e)
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
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Profile_Rel_Program WHERE (Guid = @Param_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", this.lt_Guid.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        iProgID_Old.Add(DT.Rows[i]["Prog_ID"].ToString());
                    }
                }
                #endregion

                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();
                SBSql.Clear();
                SBSql.AppendLine(" DELETE FROM User_Profile_Rel_Program ");
                SBSql.AppendLine(" WHERE (Guid = @Param_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", this.lt_Guid.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("權限移除失敗！", "");
                    return;
                }
                else
                {
                    //帶出資料
                    LookupData();

                    //寫入Log
                    fn_Log.Log_AD_withAuth("User", "移除權限", this.lt_ProfileName.Text
                        , "移除 " + this.lt_ProfileName.Text + " 的使用權限"
                        , fn_Params.UserAccount
                        , iProgID_Old, null, this.lt_Guid.Text);

                    fn_Extensions.JsAlert("移除權限成功！", "");
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 移除權限！", "");
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
                SBSql.AppendLine(" FROM User_Profile ");
                SBSql.AppendLine(" WHERE (Guid = @Param_Guid) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_Guid", this.tb_Profile_ID.Text);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無此使用者 - " + this.tb_Profile_Name.Text + "!", "");
                        return;
                    }

                    //[填入資料]
                    this.pl_Data.Visible = true;
                    this.lt_ProfileName.Text = DT.Rows[0]["Display_Name"].ToString();  //顯示名稱
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
                    //[按鈕] - 儲存權限設定, 加入BlockUI
                    this.btn_GetProgID.Attributes["onclick"] = fn_Extensions.BlockJs(
                        "Save",
                        "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                    //[按鈕] - 移除個人權限, 加入確認詢問
                    this.btn_Remove.Attributes.Add("onclick", "return confirm('是否確定要移除此人的所有權限!?')");

                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 基本資料！", "");
        }

    }
    #endregion

    private string _Param_Guid;
    public string Param_Guid
    {
        get
        {
            return this.lt_Guid.Text;
        }
        set
        {
            this._Param_Guid = value;
        }
    }


}
