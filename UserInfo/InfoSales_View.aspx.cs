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

public partial class InfoSales_View : SecurityIn
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
                        this.lb_UserID.Text = DT.Rows[0]["Account_Name"].ToString();
                        this.lb_UserName.Text = DT.Rows[0]["Display_Name"].ToString();
                        this.lb_Email.Text = DT.Rows[0]["Email"].ToString();
                        this.lb_NickName.Text = DT.Rows[0]["NickName"].ToString();
                        this.lb_ERP_LoginID.Text = DT.Rows[0]["ERP_LoginID"].ToString().Trim();
                        this.lb_ERP_UserID.Text = DT.Rows[0]["ERP_UserID"].ToString().Trim();
                        this.lb_Tel.Text = DT.Rows[0]["Tel"].ToString();
                        this.lb_TelExt.Text = DT.Rows[0]["Tel_Ext"].ToString();
                        this.lb_Mobile.Text = DT.Rows[0]["Mobile"].ToString();
                        this.lb_IM_Skype.Text = DT.Rows[0]["IM_Skype"].ToString();
                        this.lb_IM_Line.Text = DT.Rows[0]["IM_QQ"].ToString();
                        this.lb_IM_QQ.Text = DT.Rows[0]["IM_Line"].ToString();

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
    #endregion

}
