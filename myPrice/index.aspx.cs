using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;
using LogRecord;
using System.IO;
using System.Text.RegularExpressions;

public partial class myPrice_index : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 客戶報價
                if (fn_CheckAuth.CheckAuth_User("630", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //帶出資料
                LookupData();

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 顯示資料 - 已儲存的價格比較表
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢
                SBSql.Append(" SELECT Base.Sheet_ID ID, Base.Subject Label");
                SBSql.Append(" FROM Price_CompareSheet Base WITH(NOLOCK)");
                SBSql.Append(" WHERE (Create_Who = @Create_Who)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #endregion

    #region -- 按鈕控制 --

    protected void btn_Search_Click(object sender, EventArgs e)
    {
        string custID = this.tb_CustID.Text;
        if (string.IsNullOrEmpty(custID))
        {
            fn_Extensions.JsAlert("客戶編號空白", "");
            return;
        }

        //Redirect
        Response.Redirect("fullPrice_OverSales.aspx?DataID={0}".FormatThis(Cryptograph.MD5Encrypt(custID, fn_Params.DesKey)));
    }


    #endregion

}