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
using System.Collections;
using ExtensionUI;

public partial class myPrice_PriceCompare_Create : SecurityIn
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

                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    //自動產生新資料
                    Add_Data();
                }
                else
                {
                    LookupData();
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 資料顯示:基本資料 --
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得/檢查參數] - 系統編號
            if (string.IsNullOrEmpty(Req_DataID))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Page_SearchUrl);
                return;
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Base.Sheet_ID, Base.Subject, Base.Create_Time");
                SBSql.AppendLine(" FROM Price_CompareSheet Base");
                SBSql.AppendLine(" WHERE (Base.Create_Who = @Create_Who) AND (Base.Sheet_ID = @DataID)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_SearchUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Subject.Text = DT.Rows[0]["Subject"].ToString();

                        //帶出關聯資料
                        LookupData_Cust();
                        LookupData_Prod();

                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #endregion -- 資料顯示 End --

    #region -- 資料顯示:客戶關聯 --

    /// <summary>
    /// 顯示客戶關聯
    /// </summary>
    private void LookupData_Cust()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT RTRIM(Base. MA001) ID, RTRIM(Base.MA002) AS Label");
                SBSql.Append(" FROM [PKSYS].dbo.Customer Base");
                SBSql.Append("  INNER JOIN Price_Rel_CustID Rel ON Base.MA001 = Rel.CustID AND Base.DBS = Base.DBC");
                SBSql.Append(" WHERE (Rel.Sheet_ID = @DataID)");
                SBSql.Append(" ORDER BY 1");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);

                //[SQL] - 取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lv_CustList.DataSource = DT.DefaultView;
                    this.lv_CustList.DataBind();
                }
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 顯示客戶關聯");
        }
    }

    protected void lv_CustList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Price_Rel_CustID WHERE (CustID = @DataID) AND (Sheet_ID = @Parent_ID);");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Get_DataID);
                cmd.Parameters.AddWithValue("Parent_ID", Req_DataID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除失敗", "");
                    return;
                }
                else
                {
                    Response.Redirect(Page_CurrentUrl + "#custList");
                }
            }
        }
    }

    #endregion

    #region -- 資料顯示:產品 --

    /// <summary>
    /// 顯示產品品號關聯
    /// </summary>
    private void LookupData_Prod()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT RTRIM(Rel.Model_No) ID, RTRIM(Base.Model_Name_zh_TW) AS Label");
                SBSql.Append(" FROM [ProductCenter].dbo.Prod_Item Base");
                SBSql.Append("  INNER JOIN Price_Rel_ModelNo Rel ON Base.Model_No = Rel.Model_No");
                SBSql.Append(" WHERE (Rel.Sheet_ID = @DataID)");
                SBSql.Append(" ORDER BY Rel.Model_No");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);

                //[SQL] - 取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    Response.Write(ErrMsg);
                    //DataBind            
                    this.lv_Prod.DataSource = DT.DefaultView;
                    this.lv_Prod.DataBind();
                }
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 顯示產品關聯");
        }
    }

    protected void lv_Prod_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Price_Rel_ModelNo WHERE (Model_No = @DataID) AND (Sheet_ID = @Parent_ID);");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Get_DataID);
                cmd.Parameters.AddWithValue("Parent_ID", Req_DataID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除失敗", "");
                    return;
                }
                else
                {
                    Response.Redirect(Page_CurrentUrl + "#prodList");
                }
            }
        }
    }

    #endregion

    #region -- 資料編輯 --
    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            //產生Guid
            string guid = fn_Extensions.GetGuid();

            //[SQL] - 資料新增
            SBSql.Append(" INSERT INTO Price_CompareSheet( ");
            SBSql.Append("  Sheet_ID, Subject, Create_Who, Create_Time");
            SBSql.Append(" ) VALUES ( ");
            SBSql.Append("  @myNewID, @Subject, @Create_Who, GETDATE()");
            SBSql.Append(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("myNewID", guid);
            cmd.Parameters.AddWithValue("Subject", "新的名稱#{0}".FormatThis(DateTime.Now.ToString().ToDateString("yyyyMMddHHmm")));
            cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_SearchUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}myPrice/PriceCompare_Create.aspx?DataID={1}".FormatThis(Application["WebUrl"], guid);

            //導向本頁
            Response.Redirect(thisUrl);

        }

    }

    #endregion

    #region -- 按鈕設定 --
    /// <summary>
    /// 名稱變更
    /// </summary>
    protected void btn_AddName_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" UPDATE Price_CompareSheet SET Subject = @Subject");
                SBSql.Append(" WHERE (Sheet_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Subject", this.tb_Subject.Text);
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("名稱變更失敗！", Page_CurrentUrl);
                    return;
                }

                //導向本頁
                Response.Redirect(Page_CurrentUrl);
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 名稱變更");
        }

    }


    /// <summary>
    /// 加入客戶關聯
    /// </summary>
    protected void btn_AddCust_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" IF (SELECT COUNT(*) FROM Price_Rel_CustID WHERE (Sheet_ID = @Parent_ID) AND (CustID = @DataID)) = 0");
                SBSql.Append(" BEGIN");
                SBSql.Append("  INSERT INTO Price_Rel_CustID (");
                SBSql.Append("    Sheet_ID, CustID");
                SBSql.Append("  ) VALUES (");
                SBSql.Append("    @Parent_ID, @DataID");
                SBSql.Append("  )");
                SBSql.Append(" END");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Parent_ID", Req_DataID);
                cmd.Parameters.AddWithValue("DataID", this.tb_CustID.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("加入客戶關聯失敗！", Page_CurrentUrl);
                    return;
                }

                //導向本頁
                Response.Redirect(Page_CurrentUrl + "#custList");
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 加入客戶關聯");
        }

    }

    /// <summary>
    /// 加入產品品號關聯
    /// </summary>
    protected void btn_AddProd_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" IF (SELECT COUNT(*) FROM Price_Rel_ModelNo WHERE (Sheet_ID = @Parent_ID) AND (Model_No = @DataID)) = 0");
                SBSql.Append(" BEGIN");
                SBSql.Append("  INSERT INTO Price_Rel_ModelNo (");
                SBSql.Append("    Sheet_ID, Model_No");
                SBSql.Append("  ) VALUES (");
                SBSql.Append("    @Parent_ID, @DataID");
                SBSql.Append("  )");
                SBSql.Append(" END");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Parent_ID", Req_DataID);
                cmd.Parameters.AddWithValue("DataID", this.tb_ModelNo_Val.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("加入產品品號關聯失敗！", Page_CurrentUrl);
                    return;
                }

                //導向本頁
                Response.Redirect(Page_CurrentUrl + "#prodList");
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 加入產品品號關聯");
        }

    }

    /// <summary>
    /// 刪除比較表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_DelSheet_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" DELETE FROM Price_Rel_ModelNo WHERE (Sheet_ID = @DataID);");
                SBSql.Append(" DELETE FROM Price_Rel_CustID WHERE (Sheet_ID = @DataID);");
                SBSql.Append(" DELETE FROM Price_CompareSheet WHERE (Sheet_ID = @DataID);");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除比較表失敗！", Page_CurrentUrl);
                    return;
                }

                //導向本頁
                Response.Redirect(Page_SearchUrl);
            }

        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 刪除比較表");
        }
    }

    #endregion

    #region -- 參數設定 --

    /// <summary>
    /// 取得傳遞參數 - DataID
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : data;
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}myPrice/PriceCompare_Create.aspx?DataID={1}".FormatThis(Application["WebUrl"], HttpUtility.UrlEncode(Req_DataID));
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - Search Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            return "{0}myPrice/index.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    #endregion

   
}