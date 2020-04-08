using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ExtensionMethods;

public partial class CustDB_Edit : SecurityIn
{
    //回覆權限
    public bool ReplyAuth = false;
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 客戶主要資料庫設定
                if (fn_CheckAuth.CheckAuth_User("420", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - 公司別
                if (fn_Extensions.Menu_Corp(this.rbl_NewDB, "", out ErrMsg) == false)
                {
                    this.rbl_NewDB.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 公司別(報價資料庫)
                if (fn_Extensions.Menu_Corp(this.cbl_PriceDB, null, out ErrMsg) == false)
                {
                    this.cbl_PriceDB.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[參數判斷] - 是否為修改資料
                if (false == string.IsNullOrEmpty(Param_thisID))
                {
                    View_Data();
                }

                //顯示未設定的客戶
                LookupDataList();

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "");
                return;
            }
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void View_Data()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                SBSql.AppendLine(" SELECT Cust.DBC AS myDB, Corp.Corp_Name AS myDBName, RTRIM(Cust.MA001) AS custid, Cust.MA002 AS shortName ");
                SBSql.AppendLine(" FROM Customer Cust WITH (NOLOCK) ");
                SBSql.AppendLine("  LEFT JOIN Param_Corp Corp WITH (NOLOCK) ON Cust.DBC = Corp.Corp_ID");
                SBSql.AppendLine(" WHERE (Cust.MA001 = @DataID); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", PageUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        string CustID = DT.Rows[0]["custid"].ToString();
                        this.tb_CustName.Text = DT.Rows[0]["shortName"].ToString();
                        this.tb_CustID.Text = CustID;
                        this.lb_CustID.Text = "({0})".FormatThis(DT.Rows[0]["custid"].ToString());

                        string DBC = DT.Rows[0]["myDB"].ToString();
                        string DBCName = DT.Rows[0]["myDBName"].ToString();
                        if (DBC.ToUpper().Equals("INPROCESS") || string.IsNullOrEmpty(DBC))
                        {
                            this.lb_CurrDB.Text = "-- 主要資料庫空白, 請儘快完成設定 --";
                        }
                        else
                        {
                            this.lb_CurrDB.Text = DBCName;
                        }

                        //取得報價資料庫
                        Get_PriceDB_Items(CustID);

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }

    /// <summary>
    /// 顯示尚未設定的資料
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT RTRIM(MA001) AS custid, MA002 AS shortName");
                SBSql.AppendLine(" FROM Customer WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (DBC IS NULL) OR (DBC = '') OR (UPPER(DBC) = 'INPROCESS') ");
                SBSql.AppendLine(" GROUP BY DBC, MA001, MA002 ");
                SBSql.AppendLine(" ORDER BY DBC, MA001, MA002 ");
                cmd.CommandText = SBSql.ToString();
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
            fn_Extensions.JsAlert("系統發生錯誤 - 客戶列表！", "");
        }
    }

    /// <summary>
    /// 取得報價資料庫 已勾選的選項
    /// </summary>
    private void Get_PriceDB_Items(string CustID)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT Cust_ERPID, PriceDB");
            SBSql.AppendLine(" FROM Customer_PriceDB");
            SBSql.AppendLine(" WHERE (Cust_ERPID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", CustID);
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
            {
                CheckBoxList cbl = this.cbl_PriceDB;

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    for (int col = 0; col < cbl.Items.Count; col++)
                    {
                        if (cbl.Items[col].Value.Equals(DT.Rows[row]["PriceDB"].ToString()))
                        {
                            cbl.Items[col].Selected = true;
                        }
                    }

                }
            }
        }

    }

    #endregion -- 資料顯示 End --


    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            //報價資料庫選項
            IEnumerable<string> PriceDB_Checked = this.cbl_PriceDB.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => item.Value);

            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (string.IsNullOrEmpty(this.rbl_NewDB.SelectedValue))
            {
                SBAlert.Append("請選擇「主要資料庫」\\n");
            }

            if (PriceDB_Checked.Count() == 0)
            {
                SBAlert.Append("請選擇「報價資料庫」\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //Update, 主要資料庫
                SBSql.AppendLine(" UPDATE Customer SET DBC = @DBC WHERE (MA001 = @DataID);");

                //Update, 報價資料庫
                SBSql.AppendLine("  DELETE FROM Customer_PriceDB WHERE (Cust_ERPID = @DataID);");

                int row = 0;
                foreach (var val in PriceDB_Checked)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Customer_PriceDB(Cust_ERPID, PriceDB) VALUES(@DataID, @PriceDB_{0});".FormatThis(row));
                    cmd.Parameters.AddWithValue("PriceDB_{0}".FormatThis(row), val);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", this.tb_CustID.Text);
                cmd.Parameters.AddWithValue("DBC", this.rbl_NewDB.SelectedValue);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", "");
                    return;
                }


                //導向本頁
                string thisUrl = "{0}?CustID={1}".FormatThis(PageUrl, this.tb_CustID.Text);
                Response.Redirect(thisUrl);
                return;
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    #endregion -- 資料編輯 End --

    #region -- 參數設定 --
    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "CustDB_Edit.aspx";
        }
        set
        {
            this._PageUrl = value;
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
            return string.IsNullOrEmpty(Request.QueryString["CustID"]) ? "" : Request.QueryString["CustID"].ToString().Trim();
        }
        set
        {
            this._Param_thisID = value;
        }
    }
    #endregion

}