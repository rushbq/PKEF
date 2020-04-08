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

public partial class Channel_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 通路目標 - 編輯
                if (fn_CheckAuth.CheckAuth_User("131", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "TargetSet/Channel_Search.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[帶出選單] - 出貨地
                if (fn_Extensions.Menu_ShipFrom(this.ddl_ShipFrom, Request.QueryString["ShipFrom"], true, out ErrMsg) == false)
                {
                    this.ddl_ShipFrom.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[帶出選單] - 年
                if (fn_Extensions.Menu_Year(this.ddl_Year, Request.QueryString["SetYear"], false, 1, out ErrMsg) == false)
                {
                    this.ddl_Year.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[帶出選單] - 月
                if (fn_Extensions.Menu_Month(this.ddl_Mon, Request.QueryString["SetMonth"], false, out ErrMsg) == false)
                {
                    this.ddl_Mon.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得參數] - 通路別
                if (fn_Extensions.Menu_Channel(this.ddl_Channel, Request.QueryString["CID"], true, out ErrMsg) == false)
                {
                    this.ddl_Mon.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[帶出預設值] - 基本選單
                initDefMenu();
                //提示訊息
                if (string.IsNullOrEmpty(Request.QueryString["ShowDT"]))
                {
                    this.lt_MessageTxt.Text = "Step 1. 填入相關欄位後，按下「新增」即可新增目標";
                }
                else
                {
                    this.lt_MessageTxt.Text = "Step 2. 填寫通路目標金額";
                }

                //[參數判斷] - 是否為修改資料
                if (string.IsNullOrEmpty(Param_thisID) == false)
                {
                    View_Data();
                }

                //顯示關聯資料 - 本年
                View_DataList();
                //顯示關聯資料 - 前一年
                View_DataList_LastYear();
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
    /// 預設值 (無資料庫連線)
    /// </summary>
    private void initDefMenu()
    {
        try
        {
            #region ** 預設 - 年月 **
            //取得目前日期
            DateTime currDate = DateTime.Now;

            if (string.IsNullOrEmpty(Request.QueryString["SetYear"]))
            {
                //顯示本月
                this.ddl_Year.SelectedValue = currDate.Year.ToString();
                this.ddl_Mon.SelectedValue = currDate.Month.ToString();
            }

            //判斷是否有顯示下個月的請求
            if (false == string.IsNullOrEmpty(Param_PrevDate))
            {
                //判斷日期格式
                if (Param_PrevDate.IsDate())
                {
                    DateTime getDate = Convert.ToDateTime(Param_PrevDate).AddMonths(1);

                    //顯示下一個月
                    this.ddl_Year.SelectedValue = getDate.Year.ToString();
                    this.ddl_Mon.SelectedValue = getDate.Month.ToString();
                }
            }
            #endregion

        }
        catch (Exception)
        {

            throw;
        }
    }

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
                fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT UID, ShipFrom, SetYear, SetMonth, Channel.CID, Channel.CName ");
                SBSql.AppendLine("  , Amount_NTD, Amount_USD, Amount_RMB");
                SBSql.AppendLine("  , OrdAmount_NTD, OrdAmount_USD, OrdAmount_RMB");
                SBSql.AppendLine(" FROM Target_Channel INNER JOIN PKSYS.dbo.Channel ");
                SBSql.AppendLine("  ON Target_Channel.CID = Channel.CID ");
                SBSql.AppendLine(" WHERE (Target_Channel.UID = @UID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.hf_UID.Value = DT.Rows[0]["UID"].ToString();
                        this.ddl_ShipFrom.SelectedValue = DT.Rows[0]["ShipFrom"].ToString().Trim();
                        this.ddl_Channel.SelectedValue = DT.Rows[0]["CID"].ToString().Trim();
                        this.ddl_Year.SelectedValue = DT.Rows[0]["SetYear"].ToString().Trim();
                        this.ddl_Mon.SelectedValue = DT.Rows[0]["SetMonth"].ToString().Trim();
                        this.tb_Amount_NTD.Text = (DT.Rows[0]["Amount_NTD"].ToString().Equals("0")) ? "" : DT.Rows[0]["Amount_NTD"].ToString();
                        this.tb_Amount_USD.Text = (DT.Rows[0]["Amount_USD"].ToString().Equals("0")) ? "" : DT.Rows[0]["Amount_USD"].ToString();
                        this.tb_Amount_RMB.Text = (DT.Rows[0]["Amount_RMB"].ToString().Equals("0")) ? "" : DT.Rows[0]["Amount_RMB"].ToString();
                        this.tb_OrdAmount_NTD.Text = (DT.Rows[0]["OrdAmount_NTD"].ToString().Equals("0")) ? "" : DT.Rows[0]["OrdAmount_NTD"].ToString();
                        this.tb_OrdAmount_USD.Text = (DT.Rows[0]["OrdAmount_USD"].ToString().Equals("0")) ? "" : DT.Rows[0]["OrdAmount_USD"].ToString();
                        this.tb_OrdAmount_RMB.Text = (DT.Rows[0]["OrdAmount_RMB"].ToString().Equals("0")) ? "" : DT.Rows[0]["OrdAmount_RMB"].ToString();

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.btn_Save.Text = "儲存";
                        //顯示提醒文字
                        this.lt_MessageTxt.Text = "修改通路目標金額";
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
    /// 顯示關聯資料 - 本年
    /// </summary>
    private void View_DataList()
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT UID, SetMonth ");
                SBSql.AppendLine("  , Amount_NTD, Amount_USD, Amount_RMB");
                SBSql.AppendLine("  , OrdAmount_NTD, OrdAmount_USD, OrdAmount_RMB");
                SBSql.AppendLine(" FROM Target_Channel ");
                SBSql.AppendLine(" WHERE (ShipFrom = @ShipFrom) AND (CID = @CID) AND (SetYear = @SetYear) ");
                SBSql.AppendLine(" ORDER BY SetMonth ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
                cmd.Parameters.AddWithValue("CID", this.ddl_Channel.SelectedValue);
                cmd.Parameters.AddWithValue("SetYear", this.ddl_Year.SelectedValue);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 顯示關聯資料！", "");
            return;
        }

    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;
                    StringBuilder SBSql = new StringBuilder();

                    //取得編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                    //[SQL] - 檢查是否已被關聯
                    //SBSql.AppendLine(" DECLARE @RowNum1 AS INT ");
                    //SBSql.AppendLine(" SET @RowNum1 = (SELECT COUNT(*) FROM Icon_Rel_Certification WHERE (Pic_ID = @Param_ID)) ");
                    //SBSql.AppendLine(" IF(@RowNum1) > 0 ");
                    //SBSql.AppendLine("  SELECT 'Y' AS IsRel ");
                    //SBSql.AppendLine(" ELSE ");
                    //SBSql.AppendLine("  SELECT 'N' AS IsRel ");
                    //cmd.CommandText = SBSql.ToString();
                    //cmd.Parameters.Clear();
                    //cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    //using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    //{
                    //    if (DT.Rows[0]["IsRel"].Equals("Y"))
                    //    {
                    //        fn_Extensions.JsAlert("無法刪除，此項目已被使用！", "");
                    //        return;
                    //    }
                    //}

                    //[SQL] - 刪除資料
                    SBSql.Clear();
                    SBSql.AppendLine(" DELETE FROM Target_Channel WHERE (UID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    if (dbConn.ExecuteSql(cmd, dbConn.DBS.EFLocal, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料刪除失敗！", "");
                    }
                    else
                    {
                        fn_Extensions.JsAlert("資料刪除成功！", PageUrl_byYear);
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - ItemCommand！", "");
        }

    }

    /// <summary>
    /// 顯示關聯資料 - 前一年
    /// </summary>
    private void View_DataList_LastYear()
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT SetMonth ");
                SBSql.AppendLine("  , Amount_NTD, Amount_USD, Amount_RMB");
                SBSql.AppendLine("  , OrdAmount_NTD, OrdAmount_USD, OrdAmount_RMB");
                SBSql.AppendLine(" FROM Target_Channel ");
                SBSql.AppendLine(" WHERE (ShipFrom = @ShipFrom) AND (CID = @CID) AND (SetYear = @SetYear) ");
                SBSql.AppendLine(" ORDER BY SetMonth ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
                cmd.Parameters.AddWithValue("CID", this.ddl_Channel.SelectedValue);
                cmd.Parameters.AddWithValue("SetYear", Convert.ToInt16(this.ddl_Year.SelectedValue) - 1);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList_LastYear.DataSource = DT.DefaultView;
                    this.lvDataList_LastYear.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 顯示關聯資料！", "");
            return;
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

            //[參數檢查] - 選填項目
            Amount_NTD = string.IsNullOrEmpty(this.tb_Amount_NTD.Text) ? "0" : this.tb_Amount_NTD.Text.Trim();
            Amount_USD = string.IsNullOrEmpty(this.tb_Amount_USD.Text) ? "0" : this.tb_Amount_USD.Text.Trim();
            Amount_RMB = string.IsNullOrEmpty(this.tb_Amount_RMB.Text) ? "0" : this.tb_Amount_RMB.Text.Trim();
            OrdAmount_NTD = string.IsNullOrEmpty(this.tb_OrdAmount_NTD.Text) ? "0" : this.tb_OrdAmount_NTD.Text.Trim();
            OrdAmount_USD = string.IsNullOrEmpty(this.tb_OrdAmount_USD.Text) ? "0" : this.tb_OrdAmount_USD.Text.Trim();
            OrdAmount_RMB = string.IsNullOrEmpty(this.tb_OrdAmount_RMB.Text) ? "0" : this.tb_OrdAmount_RMB.Text.Trim();
            if (fn_Extensions.Num_正整數(Amount_NTD, "0", "9999999999999", out ErrMsg) == false)
            {
                SBAlert.Append("「銷售金額-NTD」請輸入整數\\n");
            }
            if (fn_Extensions.Num_正整數(Amount_USD, "0", "9999999999999", out ErrMsg) == false)
            {
                SBAlert.Append("「銷售金額-USD」請輸入整數\\n");
            }
            if (fn_Extensions.Num_正整數(Amount_RMB, "0", "9999999999999", out ErrMsg) == false)
            {
                SBAlert.Append("「銷售金額-RMB」請輸入整數\\n");
            }
            if (fn_Extensions.Num_正整數(OrdAmount_NTD, "0", "9999999999999", out ErrMsg) == false)
            {
                SBAlert.Append("「接單金額-NTD」請輸入整數\\n");
            }
            if (fn_Extensions.Num_正整數(OrdAmount_USD, "0", "9999999999999", out ErrMsg) == false)
            {
                SBAlert.Append("「接單金額-USD」請輸入整數\\n");
            }
            if (fn_Extensions.Num_正整數(OrdAmount_RMB, "0", "9999999999999", out ErrMsg) == false)
            {
                SBAlert.Append("「接單金額-RMB」請輸入整數\\n");
            }
            //if (Amount_NTD == "0" && Amount_USD == "0" && Amount_RMB == "0")
            //{
            //    SBAlert.Append("至少要填寫一項「銷售金額」\\n");
            //}

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
            //--- 判斷資料重複 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Target_Channel ");
            SBSql.AppendLine(" WHERE (ShipFrom = @ShipFrom) AND (CID = @CID) AND (SetYear = @SetYear) AND (SetMonth = @SetMonth) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
            cmd.Parameters.AddWithValue("CID", this.ddl_Channel.SelectedValue);
            cmd.Parameters.AddWithValue("SetYear", this.ddl_Year.SelectedValue);
            cmd.Parameters.AddWithValue("SetMonth", this.ddl_Mon.SelectedValue);
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("資料重複新增！\\n若要調整目標，請在列表處按下「修改」", "");
                    return;
                }
            }

            //--- 取得新編號 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 取得最新編號
            int New_ID;
            SBSql.AppendLine(" SELECT (ISNULL(MAX(UID), 0) + 1) AS New_ID FROM Target_Channel ");
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
            {
                New_ID = Convert.ToInt32(DT.Rows[0]["New_ID"]);
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO Target_Channel( ");
            SBSql.AppendLine("  UID, ShipFrom, CID, SetYear, SetMonth");
            SBSql.AppendLine("  , Amount_NTD, Amount_USD, Amount_RMB");
            SBSql.AppendLine("  , OrdAmount_NTD, OrdAmount_USD, OrdAmount_RMB");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @New_ID, @ShipFrom, @CID, @SetYear, @SetMonth");
            SBSql.AppendLine("  , @Amount_NTD, @Amount_USD, @Amount_RMB");
            SBSql.AppendLine("  , @OrdAmount_NTD, @OrdAmount_USD, @OrdAmount_RMB");
            SBSql.AppendLine("  , @Param_CreateWho, GETDATE() ");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("New_ID", New_ID);
            cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
            cmd.Parameters.AddWithValue("CID", this.ddl_Channel.SelectedValue);
            cmd.Parameters.AddWithValue("SetYear", this.ddl_Year.SelectedValue);
            cmd.Parameters.AddWithValue("SetMonth", this.ddl_Mon.SelectedValue);
            cmd.Parameters.AddWithValue("Amount_NTD", Amount_NTD);
            cmd.Parameters.AddWithValue("Amount_USD", Amount_USD);
            cmd.Parameters.AddWithValue("Amount_RMB", Amount_RMB);
            cmd.Parameters.AddWithValue("OrdAmount_NTD", OrdAmount_NTD);
            cmd.Parameters.AddWithValue("OrdAmount_USD", OrdAmount_USD);
            cmd.Parameters.AddWithValue("OrdAmount_RMB", OrdAmount_RMB);
            cmd.Parameters.AddWithValue("Param_CreateWho", fn_Params.UserAccount);
            if (dbConn.ExecuteSql(cmd, dbConn.DBS.EFLocal, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", PageUrl);
                return;
            }
            else
            {
                //判斷月份，若為月底則不詢問"繼續新增"
                if (this.ddl_Mon.SelectedValue.Equals("12"))
                {
                    fn_Extensions.JsAlert("資料新增成功！", PageUrl_byYear);
                    return;
                }
                else
                {
                    //傳送參數 - 目前年月/出貨地/通路代號
                    string NextUrl =
                         "Channel_Edit.aspx?ShowDT=Y&PrevDate=" + Server.UrlEncode(string.Format("{0}/{1}", this.ddl_Year.SelectedValue, this.ddl_Mon.SelectedValue))
                         + "&ShipFrom=" + Server.UrlEncode(this.ddl_ShipFrom.SelectedValue)
                         + "&CID=" + Server.UrlEncode(this.ddl_Channel.SelectedValue);
                    string js = "if(confirm('資料新增成功！\\n是否要繼續新增下一個月')){location.href='" + NextUrl + "';} else {location.href='" + PageUrl_byYear + "';}";
                    ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
                    return;
                }
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
            //--- 判斷資料重複 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Target_Channel ");
            SBSql.AppendLine(" WHERE (UID <> @UID)");
            SBSql.AppendLine("   AND (ShipFrom = @ShipFrom) AND (CID = @CID) AND (SetYear = @SetYear) AND (SetMonth = @SetMonth)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("UID", this.hf_UID.Value);
            cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
            cmd.Parameters.AddWithValue("CID", this.ddl_Channel.SelectedValue);
            cmd.Parameters.AddWithValue("SetYear", this.ddl_Year.SelectedValue);
            cmd.Parameters.AddWithValue("SetMonth", this.ddl_Mon.SelectedValue);
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("資料重複新增！\\n若要調整目標，請在列表處按下「修改」", "");
                    return;
                }
            }

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Target_Channel ");
            SBSql.AppendLine(" SET ShipFrom = @ShipFrom, CID = @CID, SetYear = @SetYear, SetMonth = @SetMonth");
            SBSql.AppendLine("  , Amount_NTD = @Amount_NTD, Amount_USD = @Amount_USD, Amount_RMB = @Amount_RMB");
            SBSql.AppendLine("  , OrdAmount_NTD = @OrdAmount_NTD, OrdAmount_USD = @OrdAmount_USD, OrdAmount_RMB = @OrdAmount_RMB");
            SBSql.AppendLine("  , Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (UID = @UID) ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("UID", this.hf_UID.Value);
            cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
            cmd.Parameters.AddWithValue("CID", this.ddl_Channel.SelectedValue);
            cmd.Parameters.AddWithValue("SetYear", this.ddl_Year.SelectedValue);
            cmd.Parameters.AddWithValue("SetMonth", this.ddl_Mon.SelectedValue);
            cmd.Parameters.AddWithValue("Amount_NTD", Amount_NTD);
            cmd.Parameters.AddWithValue("Amount_USD", Amount_USD);
            cmd.Parameters.AddWithValue("Amount_RMB", Amount_RMB);
            cmd.Parameters.AddWithValue("OrdAmount_NTD", OrdAmount_NTD);
            cmd.Parameters.AddWithValue("OrdAmount_USD", OrdAmount_USD);
            cmd.Parameters.AddWithValue("OrdAmount_RMB", OrdAmount_RMB);
            cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Params.UserAccount);
            if (dbConn.ExecuteSql(cmd, dbConn.DBS.EFLocal, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", PageUrl_byYear);
                return;
            }
        }
    }

    #endregion -- 資料編輯 End --

    #region -- 按鈕區 --
    /// <summary>
    /// 年份切換
    /// </summary>
    protected void ddl_Year_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(PageUrl_byYear);
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 上一個年月(2012/12)
    /// </summary>
    private string _Param_PrevDate;
    public string Param_PrevDate
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["PrevDate"])
                ? ""
                : fn_stringFormat.Filter_Html(Request.QueryString["PrevDate"].ToString());
        }
        set
        {
            this._Param_PrevDate = value;
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
            return string.IsNullOrEmpty(Request.QueryString["EditID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["EditID"].ToString());
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
            return string.Format(@"Channel_Edit.aspx?EditID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_thisID)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    private string _PageUrl_byYear;
    public string PageUrl_byYear
    {
        get
        {
            string Url =
                      "Channel_Edit.aspx?ShowDT=Y&SetYear=" + Server.UrlEncode(this.ddl_Year.SelectedValue)
                      + "&ShipFrom=" + Server.UrlEncode(this.ddl_ShipFrom.SelectedValue)
                      + "&CID=" + Server.UrlEncode(this.ddl_Channel.SelectedValue);
            return Url;
        }
        set
        {
            this._PageUrl_byYear = value;
        }
    }
    #endregion

    #region -- 金額設定(接單) --
    private string _OrdAmount_NTD;
    public string OrdAmount_NTD
    {
        get
        {
            return string.IsNullOrEmpty(_OrdAmount_NTD) ? "-" : _OrdAmount_NTD;
        }
        set
        {
            this._OrdAmount_NTD = value;
        }
    }
    private string _OrdAmount_USD;
    public string OrdAmount_USD
    {
        get
        {
            return string.IsNullOrEmpty(_OrdAmount_USD) ? "-" : _OrdAmount_USD;
        }
        set
        {
            this._OrdAmount_USD = value;
        }
    }
    private string _OrdAmount_RMB;
    public string OrdAmount_RMB
    {
        get
        {
            return string.IsNullOrEmpty(_OrdAmount_RMB) ? "-" : _OrdAmount_RMB;
        }
        set
        {
            this._OrdAmount_RMB = value;
        }
    }
    #endregion

    #region -- 金額設定(銷售) --
    private string _Amount_NTD;
    public string Amount_NTD
    {
        get
        {
            return string.IsNullOrEmpty(_Amount_NTD) ? "-" : _Amount_NTD;
        }
        set
        {
            this._Amount_NTD = value;
        }
    }
    private string _Amount_USD;
    public string Amount_USD
    {
        get
        {
            return string.IsNullOrEmpty(_Amount_USD) ? "-" : _Amount_USD;
        }
        set
        {
            this._Amount_USD = value;
        }
    }
    private string _Amount_RMB;
    public string Amount_RMB
    {
        get
        {
            return string.IsNullOrEmpty(_Amount_RMB) ? "-" : _Amount_RMB;
        }
        set
        {
            this._Amount_RMB = value;
        }
    }
    #endregion

}
