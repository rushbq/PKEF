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

public partial class Cust_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //判斷來源SID是否正確
                if (!mySID.Equals(Req_SID))
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "CustInfo/Cust_Search.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_DBSave.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料處理中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - 出貨庫別
                if (!fn_Extensions.Menu_SWID(this.ddl_SWID, "", true, out ErrMsg))
                {
                    this.ddl_SWID.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 公司別(主要資料庫)
                if (fn_Extensions.Menu_Corp(this.ddl_MainDB, "", out ErrMsg) == false)
                {
                    this.ddl_MainDB.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 公司別(報價資料庫)
                if (fn_Extensions.Menu_Corp(this.cbl_PriceDB, null, out ErrMsg) == false)
                {
                    this.cbl_PriceDB.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }


                //讀取資料
                View_Data();

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", Session["BackListUrl"].ToString());
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
            //[取得/檢查參數] - 系統編號
            if (string.IsNullOrEmpty(Param_thisID))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                return;
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                SBSql.AppendLine(" SELECT Base.*");
                SBSql.AppendLine("  , RTRIM(Base.MA001) CustID, RTRIM(Base.MA003) CustName, Sub.SWID, SW.SW_Name_zh_TW SWName, Sub.InvType");
                SBSql.AppendLine("  , RTRIM(myArea.MR003) AreaName, RTRIM(myCountry.MR003) CountryName");
                SBSql.AppendLine("  , Prof.Account_Name RepSalesID, Prof.Display_Name RepSales");
                SBSql.AppendLine("  , Corp.Corp_Name, Corp.Corp_ID");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM User_Profile WHERE (Guid = Sub.Update_Who)) AS Update_Name, Sub.Update_Time ");
                SBSql.AppendLine(" FROM Customer Base ");
                SBSql.AppendLine("  INNER JOIN Param_Corp Corp ON UPPER(Base.DBC) = UPPER(Corp.Corp_ID)");
                SBSql.AppendLine("  LEFT JOIN Customer_Data Sub ON Sub.Cust_ERPID = Base.MA001");
                SBSql.AppendLine("  LEFT JOIN ShippingWarehouse SW ON Sub.SWID = SW.SWID");
                SBSql.AppendLine("  LEFT JOIN [prokit2].dbo.CMSMR myArea ON myArea.MR001 = 3 AND myArea.MR002 = Base.MA018 COLLATE Chinese_Taiwan_Stroke_BIN");
                SBSql.AppendLine("  LEFT JOIN [prokit2].dbo.CMSMR myCountry ON myCountry.MR001 = 4 AND myCountry.MR002 = Base.MA019 COLLATE Chinese_Taiwan_Stroke_BIN");
                SBSql.AppendLine("  LEFT JOIN User_Profile Prof ON Prof.ERP_UserID = Base.MA016");
                SBSql.AppendLine(" WHERE (Base.DBC = Base.DBS) AND (Base.MA001 = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        string _custID = DT.Rows[0]["CustID"].ToString();
                        this.lt_CustID.Text = _custID;
                        hf_CustID.Value = _custID;
                        this.lt_DBName.Text = DT.Rows[0]["Corp_Name"].ToString();
                        this.lt_SWName.Text = DT.Rows[0]["SWName"].ToString();
                        this.lt_CustSortName.Text = DT.Rows[0]["MA002"].ToString().Trim();
                        this.lt_CustFullName.Text = DT.Rows[0]["CustName"].ToString();
                        this.lb_AreaName.Text = DT.Rows[0]["AreaName"].ToString();
                        this.lb_CountryName.Text = DT.Rows[0]["CountryName"].ToString();
                        this.lt_Email.Text = DT.Rows[0]["MA009"].ToString();
                        this.lt_Currency.Text = DT.Rows[0]["MA014"].ToString();
                        this.lt_ShipAddr.Text = DT.Rows[0]["MA027"].ToString();
                        this.lt_RepSales.Text = "({0}) {1}".FormatThis(DT.Rows[0]["RepSalesID"].ToString(), DT.Rows[0]["RepSales"].ToString());
                        this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                        this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");

                        //[資料庫設定]
                        this.ddl_MainDB.SelectedValue = DT.Rows[0]["Corp_ID"].ToString();
                        this.ddl_SWID.SelectedValue = DT.Rows[0]["SWID"].ToString();
                        this.ddl_InvType.SelectedValue = DT.Rows[0]["InvType"].ToString();
                        Get_PriceDB_Items();

                        //[報表關聯]
                        LookupData_Rpt();

                        //[對帳單收件人]
                        LookupData_ARemail();


                        //[其他資料]
                        this.lt_MA004.Text = DT.Rows[0]["MA004"].ToString().Trim();
                        this.lt_MA005.Text = DT.Rows[0]["MA005"].ToString().Trim();
                        this.lt_MA006.Text = DT.Rows[0]["MA006"].ToString().Trim();
                        this.lt_MA007.Text = DT.Rows[0]["MA007"].ToString().Trim();
                        this.lt_MA008.Text = DT.Rows[0]["MA008"].ToString().Trim();
                        this.lt_MA017.Text = DT.Rows[0]["MA017"].ToString().Trim();
                        this.lt_MA022.Text = DT.Rows[0]["MA022"].ToString().Trim();
                        this.lt_MA023.Text = DT.Rows[0]["MA023"].ToString().Trim();
                        this.lt_MA024.Text = DT.Rows[0]["MA024"].ToString().Trim();
                        this.lt_MA025.Text = DT.Rows[0]["MA025"].ToString().Trim();
                        this.lt_MA026.Text = DT.Rows[0]["MA026"].ToString().Trim();
                        this.lt_MA027.Text = DT.Rows[0]["MA027"].ToString().Trim();
                        this.lt_MA030.Text = DT.Rows[0]["MA030"].ToString().Trim();
                        this.lt_MA031.Text = DT.Rows[0]["MA031"].ToString().Trim();
                        this.lt_MA037.Text = DT.Rows[0]["MA037"].ToString().Trim();
                        this.lt_MA038.Text = DT.Rows[0]["MA038"].ToString().Trim();
                        this.lt_MA040.Text = DT.Rows[0]["MA040"].ToString().Trim();
                        this.lt_MA041.Text = DT.Rows[0]["MA041"].ToString().Trim();
                        this.lt_MA048.Text = DT.Rows[0]["MA048"].ToString().Trim();
                        this.lt_MA051.Text = DT.Rows[0]["MA051"].ToString().Trim();
                        this.lt_MA065.Text = DT.Rows[0]["MA065"].ToString().Trim();
                        this.lt_MA066.Text = DT.Rows[0]["MA066"].ToString().Trim();
                        this.lt_MA067.Text = DT.Rows[0]["MA067"].ToString().Trim();
                        this.lt_MA076.Text = DT.Rows[0]["MA076"].ToString().Trim();
                        this.lt_MA077.Text = DT.Rows[0]["MA077"].ToString().Trim();
                        this.lt_MA078.Text = DT.Rows[0]["MA078"].ToString().Trim();
                        this.lt_MA079.Text = DT.Rows[0]["MA079"].ToString().Trim();
                        this.lt_MA080.Text = DT.Rows[0]["MA080"].ToString().Trim();
                        this.lt_MA081.Text = DT.Rows[0]["MA081"].ToString().Trim();
                        this.lt_MA098.Text = DT.Rows[0]["MA098"].ToString().Trim();
                        this.lt_MA099.Text = DT.Rows[0]["MA099"].ToString().Trim();
                        this.lt_MA100.Text = DT.Rows[0]["MA100"].ToString().Trim();
                        this.lt_MA101.Text = DT.Rows[0]["MA101"].ToString().Trim();
                        this.lt_MA118.Text = DT.Rows[0]["MA118"].ToString().Trim();
                        this.lt_MA110.Text = DT.Rows[0]["MA110"].ToString().Trim();
                        this.lt_MA071.Text = DT.Rows[0]["MA071"].ToString().Trim();

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
    /// 取得報價資料庫 已勾選的選項
    /// </summary>
    private void Get_PriceDB_Items()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT Cust_ERPID, PriceDB");
            SBSql.AppendLine(" FROM Customer_PriceDB");
            SBSql.AppendLine(" WHERE (Cust_ERPID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
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
    /// 按鈕 - 基本資料存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            //報價資料庫選項
            IEnumerable<string> PriceDB_Checked = this.cbl_PriceDB.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => item.Value);

            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (this.ddl_SWID.SelectedIndex == 0)
            {
                SBAlert.Append("請選擇「出貨庫別」\\n");
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


            //資料儲存
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料更新
                SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Customer_Data WHERE (Cust_ERPID = @DataID)) = 0 ");
                //Insert, 出貨庫別
                SBSql.AppendLine(" BEGIN");
                SBSql.AppendLine("  INSERT INTO Customer_Data(");
                SBSql.AppendLine("      Cust_ERPID, SWID, Update_Who, Update_Time");
                SBSql.AppendLine("  ) VALUES (");
                SBSql.AppendLine("      @DataID, @SWID, @Update_Who, GETDATE()");
                SBSql.AppendLine("  )");
                SBSql.AppendLine(" END");

                SBSql.AppendLine(" ELSE");

                //Update, 出貨庫別
                SBSql.AppendLine(" BEGIN");
                SBSql.AppendLine("  UPDATE Customer_Data");
                SBSql.AppendLine("  SET SWID = @SWID, InvType = @InvType");
                SBSql.AppendLine("    , Update_Who= @Update_Who, Update_Time= GETDATE()");
                SBSql.AppendLine("  WHERE (Cust_ERPID = @DataID);");

                //Update, 主要資料庫
                SBSql.AppendLine("  UPDATE Customer");
                SBSql.AppendLine("  SET DBC = @MainDB");
                SBSql.AppendLine("  WHERE (MA001 = @DataID);");

                SBSql.AppendLine(" END");

                //Update, 報價資料庫
                SBSql.AppendLine("  DELETE FROM Customer_PriceDB WHERE (Cust_ERPID = @DataID);");

                int row = 0;
                foreach (var val in PriceDB_Checked)
                {
                    row++;

                    SBSql.AppendLine("  INSERT INTO Customer_PriceDB(Cust_ERPID, PriceDB) VALUES(@DataID, @PriceDB_{0});".FormatThis(row));
                    cmd.Parameters.AddWithValue("PriceDB_{0}".FormatThis(row), val);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("SWID", this.ddl_SWID.SelectedValue);
                cmd.Parameters.AddWithValue("InvType", this.ddl_InvType.SelectedValue);
                cmd.Parameters.AddWithValue("MainDB", this.ddl_MainDB.SelectedValue);
                cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserGuid);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", "");
                    return;
                }

                //導向本頁
                Response.Redirect(PageUrl);
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }


    /// <summary>
    /// 按鈕 - 加入報表關聯
    /// </summary>
    protected void lbtn_AddItem_Click(object sender, EventArgs e)
    {
        try
        {
            //[欄位檢查] - 取得勾選值
            string inputValue = this.tb_Items.Text;
            if (string.IsNullOrEmpty(inputValue))
            {
                fn_Extensions.JsAlert("未勾選任何選項，無法存檔", PageUrl);
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

                //[SQL] - 清除原有設定資料
                SBSql.AppendLine(" DELETE FROM Customer_Report WHERE (Cust_ERPID = @Cust_ID); ");

                //[SQL] - 資料新增
                int idx = 0;
                foreach (var item in query)
                {
                    idx++;

                    //新增資料
                    SBSql.Append(" INSERT INTO Customer_Report( ");
                    SBSql.Append("  Cust_ERPID, Prog_ID");
                    SBSql.Append(" ) VALUES (");
                    SBSql.Append("  @Cust_ID, @Prog_ID_{0}".FormatThis(idx));
                    SBSql.Append(" );");

                    cmd.Parameters.AddWithValue("Prog_ID_{0}".FormatThis(idx), item.Val);
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Cust_ID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("加入失敗！", "");
                    return;
                }
                else
                {
                    //執行轉頁
                    Response.Redirect(PageUrl + "#rptData");
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 加入報表關聯", "");
            return;
        }
    }


    /// <summary>
    /// 按鈕 - 加入收件人
    /// </summary>
    protected void btn_AddEmail_Click(object sender, EventArgs e)
    {
        try
        {
            //[欄位檢查]
            string _name = tb_MailName.Text.Trim();
            string _mail = tb_Email.Text.Trim();
            if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_mail))
            {
                fn_Extensions.JsAlert("名稱, Email不可為空.", PageUrl);
                return;
            }

            if (!_mail.IsEmail())
            {
                fn_Extensions.JsAlert("Email格式錯誤.", PageUrl);
                return;
            }

            //[資料儲存]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料新增
                SBSql.Append(" IF (SELECT COUNT(*) FROM Customer_Addressbook WHERE (Email = @Email) AND (ERP_ID = @ERP_ID)) = 0");
                SBSql.Append(" BEGIN");
                SBSql.Append(" DECLARE @NewID AS INT");
                SBSql.Append(" SET @NewID = (SELECT ISNULL(MAX(Data_ID), 0) + 1 FROM Customer_Addressbook)");
                SBSql.Append(" INSERT INTO Customer_Addressbook( ");
                SBSql.Append("  Data_ID, ERP_ID, MailName, Email, Create_Who, Create_Time");
                SBSql.Append(" ) VALUES (");
                SBSql.Append("  @NewID, @ERP_ID, @MailName, @Email, @Create_Who, GETDATE()");
                SBSql.Append(" )");
                SBSql.Append(" END");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("ERP_ID", hf_CustID.Value);
                cmd.Parameters.AddWithValue("MailName", _name);
                cmd.Parameters.AddWithValue("Email", _mail);
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserAccount);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("加入失敗！", "");
                    return;
                }
                else
                {
                    //執行轉頁
                    Response.Redirect(PageUrl + "#ARemail");
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 加入收件人", "");
            return;
        }

    }
    #endregion -- 資料編輯 End --


    #region -- 資料顯示:關聯報表 --
    /// <summary>
    /// 顯示關聯報表
    /// </summary>
    private void LookupData_Rpt()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT Rpt.Prog_ID ID, Rpt.Rpt_Desc Label");
                SBSql.Append(" FROM PKSYS.dbo.Customer_Report Base");
                SBSql.Append("  INNER JOIN Rpt_Base Rpt ON Base.Prog_ID = Rpt.Prog_ID");
                SBSql.Append(" WHERE (Base.Cust_ERPID = @DataID)");
                SBSql.Append(" ORDER BY Rpt.Prog_ID");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.ReportCenter, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 關聯報表！", "");
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
                    StringBuilder SBSql = new StringBuilder();

                    //取得Key值
                    string Get_ID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                    //[SQL] - 刪除資料
                    SBSql.AppendLine(" DELETE FROM Customer_Report WHERE (Cust_ERPID = @Cust_ID) AND (Prog_ID = @DataID); ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Cust_ID", Param_thisID);
                    cmd.Parameters.AddWithValue("DataID", Get_ID);
                    if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料刪除失敗！", "");
                    }
                    else
                    {
                        //導向本頁
                        Response.Redirect(PageUrl + "#rptData");
                    }

                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    #endregion


    #region -- 資料顯示:對帳單收件人 --
    /// <summary>
    /// 顯示對帳單收件人
    /// </summary>
    private void LookupData_ARemail()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.Append(" SELECT Data_ID, MailName, Email");
                SBSql.Append(" FROM Customer_Addressbook");
                SBSql.Append(" WHERE (ERP_ID = @DataID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", hf_CustID.Value);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //DataBind            
                    this.lv_ARmail.DataSource = DT.DefaultView;
                    this.lv_ARmail.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 關聯報表！", "");
        }
    }

    protected void lv_ARmail_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    //取得Key值
                    string Get_ID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                    //[SQL] - 刪除資料
                    SBSql.AppendLine(" DELETE FROM Customer_Addressbook WHERE (Data_ID = @Data_ID)");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Data_ID", Get_ID);
                    if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("收件人刪除失敗！", "");
                    }
                    else
                    {
                        //導向本頁
                        Response.Redirect(PageUrl + "#ARemail");
                    }

                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    private string DesKey
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
        }
        set
        {
            this._DesKey = value;
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
            return string.IsNullOrEmpty(Request.QueryString["DataID"]) ? "" : Cryptograph.MD5Decrypt(Request.QueryString["DataID"].ToString(), DesKey);
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
            return string.Format(@"Cust_Edit.aspx?DataID={0}&SID={1}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey))
                , mySID);
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// 傳來的SID
    /// </summary>
    private string _Req_SID;
    public string Req_SID
    {
        get
        {
            return Request.QueryString["SID"].ToString();
        }
        set
        {
            this._Req_SID = value;
        }
    }


    /// <summary>
    /// 取得Session ID
    /// </summary>
    private string _mySID;
    protected string mySID
    {
        get
        {
            return Session.SessionID;
        }
        set
        {
            this._mySID = value;
        }
    }

    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}CustInfo/Cust_Search.aspx".FormatThis(Application["WebUrl"]);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    #endregion

}