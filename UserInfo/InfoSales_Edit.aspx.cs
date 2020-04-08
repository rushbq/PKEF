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

                //讀取資料
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

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                cmd.Parameters.Clear();

                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Prof.Display_Name, Prof.Account_Name, Prof.ERP_LoginID, Prof.ERP_UserID ");
                SBSql.AppendLine("  , Prof.Email, Prof.NickName, Prof.Tel, Prof.Tel_Ext, Prof.Mobile");
                SBSql.AppendLine("  , Prof.IM_Skype, Prof.IM_QQ, Prof.IM_Line");
                SBSql.AppendLine("  , Prof.ERP_LoginID, Prof.ERP_UserID");
                SBSql.AppendLine("    FROM User_Profile Prof ");
                SBSql.AppendLine("    INNER JOIN User_Dept Dept ON Prof.DeptID = Dept.DeptID");
                SBSql.AppendLine("    INNER JOIN Shipping ON Dept.Area = Shipping.SID");
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
                        this.hf_UID.Value = DT.Rows[0]["Account_Name"].ToString();
                        this.lb_UserID.Text = DT.Rows[0]["Account_Name"].ToString();
                        this.lb_UserName.Text = DT.Rows[0]["Display_Name"].ToString();
                        this.lb_Email.Text = DT.Rows[0]["Email"].ToString();
                        this.tb_NickName.Text = DT.Rows[0]["NickName"].ToString();
                        this.tb_ERP_LoginID.Text = DT.Rows[0]["ERP_LoginID"].ToString().Trim();
                        this.tb_ERP_UserID.Text = DT.Rows[0]["ERP_UserID"].ToString().Trim();
                        this.tb_Tel.Text = DT.Rows[0]["Tel"].ToString();
                        this.tb_TelExt.Text = DT.Rows[0]["Tel_Ext"].ToString();
                        this.tb_Mobile.Text = DT.Rows[0]["Mobile"].ToString();
                        this.tb_IM_Skype.Text = DT.Rows[0]["IM_Skype"].ToString();
                        this.tb_IM_Line.Text = DT.Rows[0]["IM_Line"].ToString();
                        this.tb_IM_QQ.Text = DT.Rows[0]["IM_QQ"].ToString();

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
            string ClassName = string.IsNullOrEmpty(this.tb_NickName.Text) ? "" : this.tb_NickName.Text.Trim();
            if (fn_Extensions.String_資料長度Byte(ClassName, "1", "50", out ErrMsg) == false)
            {
                SBAlert.Append("「稱謂」請輸入1 ~ 50個字\\n");
            }

            string ERP_LoginID = string.IsNullOrEmpty(this.tb_ERP_LoginID.Text) ? "" : this.tb_ERP_LoginID.Text.Trim();
            if (fn_Extensions.String_資料長度Byte(ERP_LoginID, "1", "10", out ErrMsg) == false)
            {
                SBAlert.Append("「ERP登入代號」請輸入1 ~ 10個字\\n");
            }

            string ERP_UserID = string.IsNullOrEmpty(this.tb_ERP_UserID.Text) ? "" : this.tb_ERP_UserID.Text.Trim();
            if (fn_Extensions.String_資料長度Byte(ERP_UserID, "1", "10", out ErrMsg) == false)
            {
                SBAlert.Append("「ERP員工代號」請輸入1 ~ 10個字\\n");
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
                    //無新增功能
                    //Add_Data();
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
            SBSql.AppendLine(" UPDATE User_Profile ");
            SBSql.AppendLine(" SET NickName = @NickName, Tel = @Tel, Tel_Ext = @Tel_Ext, Mobile = @Mobile");
            SBSql.AppendLine("  ,IM_Skype = @IM_Skype, IM_Line = @IM_Line, IM_QQ = @IM_QQ");
            SBSql.AppendLine("  ,ERP_LoginID = @ERP_LoginID, ERP_UserID = @ERP_UserID");
            SBSql.AppendLine(" WHERE (Account_Name = @UID) ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("UID", this.hf_UID.Value);
            cmd.Parameters.AddWithValue("NickName", this.tb_NickName.Text.Trim());
            cmd.Parameters.AddWithValue("ERP_LoginID", this.tb_ERP_LoginID.Text.Trim());
            cmd.Parameters.AddWithValue("ERP_UserID", this.tb_ERP_UserID.Text.Trim());
            cmd.Parameters.AddWithValue("Tel", this.tb_Tel.Text.Trim());
            cmd.Parameters.AddWithValue("Tel_Ext", this.tb_TelExt.Text.Trim());
            cmd.Parameters.AddWithValue("Mobile", this.tb_Mobile.Text.Trim());
            cmd.Parameters.AddWithValue("IM_Skype", this.tb_IM_Skype.Text.Trim());
            cmd.Parameters.AddWithValue("IM_Line", this.tb_IM_Line.Text.Trim());
            cmd.Parameters.AddWithValue("IM_QQ", this.tb_IM_QQ.Text.Trim());
            if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", PageUrl);
                return;
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
            return string.Format(@"InfoSales_Edit.aspx?UserID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_thisID)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    #endregion

}
