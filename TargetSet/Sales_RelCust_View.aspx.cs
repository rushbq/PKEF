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

public partial class Dept_ViewItem : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 人員關聯客戶
                if (fn_CheckAuth.CheckAuth_User("140", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //顯示資料
                View_DataList();
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
    /// 顯示資料
    /// </summary>
    private void View_DataList()
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Customer.* ");
                SBSql.AppendLine(" FROM Customer ");
                SBSql.AppendLine("  INNER JOIN Staff_Rel_Customer Rel ON Customer.MA001 = Rel.CustID AND Customer.DBC = Customer.DBS ");
                SBSql.AppendLine(" WHERE (Rel.StaffID = @StaffID) ");
                SBSql.AppendLine(" ORDER BY Customer.MA001 ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("StaffID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 顯示資料！", "");
            return;
        }
    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 本筆資料的編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["StaffID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["StaffID"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 人員名
    /// </summary>
    private string _Param_Display_Name;
    public string Param_Display_Name
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["Display_Name"]) ? "" : Request.QueryString["Display_Name"].ToString();
        }
        set
        {
            this._Param_Display_Name = value;
        }
    }
    #endregion
}
