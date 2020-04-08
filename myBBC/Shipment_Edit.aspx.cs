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

public partial class Shipment_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 出貨單-台灣
                if (fn_CheckAuth.CheckAuth_User("720", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "myBBC/Shipment_Search.aspx";


                //[取得/檢查參數] - 出貨方式
                if (!fn_Extensions.Menu_OrderClass(this.ddl_ShipType, "", true, "出貨方式", "TW", out ErrMsg))
                {
                    this.ddl_ShipType.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 物流商
                if (!fn_Extensions.Menu_OrderClass(this.ddl_ShipVendor, "", true, "物流商", "TW", out ErrMsg))
                {
                    this.ddl_ShipVendor.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 運費支付方式
                if (!fn_Extensions.Menu_OrderClass(this.ddl_ShipFareType, "", true, "運費支付方式", "TW", out ErrMsg))
                {
                    this.ddl_ShipFareType.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 箱子
                if (!fn_Extensions.Menu_OrderClass(this.ddl_Box, "", true, "箱子", "TW", out ErrMsg))
                {
                    this.ddl_Box.Items.Insert(0, new ListItem("選單產生失敗", ""));
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

                SBSql.AppendLine(" SELECT Base.OrderID, Base.MyOrderID, Base.CustERPID AS CustID, Cust.MA002 AS CustName");
                SBSql.AppendLine("  , Base.ShipWho, Base.ZipCode, Base.ContactTel, Base.ShippingAddr, Base.Create_Time");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name");
                SBSql.AppendLine("  , Sub.ShipType, Sub.ShipVendor, Sub.ShipNo, Sub.ShipFareType, Sub.ShipFareMoney, Sub.ShipDate, Sub.Remark");
                SBSql.AppendLine("  , ISNULL(Sub.ShipStatus, 0) AS ShipStatus");
                SBSql.AppendLine("  , Rel.*, RelPara.Class_Name AS BoxName");
                SBSql.AppendLine(" FROM Order_Main Base ");
                SBSql.AppendLine("  INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustERPID = Cust.MA001 AND Cust.DBC = Cust.DBS");
                SBSql.AppendLine("  LEFT JOIN Order_Shipping Sub ON Base.OrderID = Sub.OrderID");
                SBSql.AppendLine("  LEFT JOIN Order_ParamClass myPara ON Sub.ShipType = myPara.Class_ID");
                SBSql.AppendLine("  LEFT JOIN Order_ShippingBox Rel ON Sub.OrderID = Rel.OrderID");
                SBSql.AppendLine("  LEFT JOIN Order_ParamClass RelPara ON Rel.BoxType = RelPara.Class_ID");
                SBSql.AppendLine(" WHERE (Base.OrderID = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //取得出貨狀態
                        string ShipStatus = DT.Rows[0]["ShipStatus"].ToString();
                        //若為已出貨,自動導向檢視頁
                        if (ShipStatus.Equals("2"))
                        {
                            Response.Redirect("Shipment_View.aspx?DataID=" + Cryptograph.MD5Encrypt(Param_thisID, DesKey));
                            return;
                        }

                        //取得出貨方式
                        string ShipType = DT.Rows[0]["ShipType"].ToString();

                        //[基本資料]
                        #region -- 基本資料 --

                        this.lt_Top_CompanyName.Text = DT.Rows[0]["CustName"].ToString();
                        this.lt_Top_OrderID.Text = DT.Rows[0]["OrderID"].ToString();
                        this.lt_OrderID.Text = DT.Rows[0]["OrderID"].ToString();
                        this.lt_MyOrderID.Text = string.IsNullOrEmpty(DT.Rows[0]["MyOrderID"].ToString()) ? "" : "({0})".FormatThis(
                            DT.Rows[0]["MyOrderID"].ToString());
                        this.lt_CustID.Text = DT.Rows[0]["CustID"].ToString();
                        this.lt_CustName.Text = DT.Rows[0]["CustName"].ToString().Trim();
                        this.lt_ShipWho.Text = "{0}<br>{1}".FormatThis(
                            DT.Rows[0]["ShipWho"].ToString()
                            , DT.Rows[0]["ContactTel"].ToString()
                            );
                        this.lt_ShipAddr.Text = "{0}&nbsp;{1}".FormatThis(
                            DT.Rows[0]["ZipCode"].ToString()
                            , DT.Rows[0]["ShippingAddr"].ToString());
                        this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");

                        //--設定狀態
                        string StatusName, StatusCss;
                        switch (ShipStatus)
                        {
                            case "0":
                                StatusCss = "label label-warning";
                                StatusName = "待處理";

                                break;

                            case "1":
                                StatusCss = "label label-info";
                                StatusName = "出貨中";

                                break;

                            default:
                                StatusCss = "label label-success";
                                StatusName = "已出貨";

                                break;
                        }
                        this.lb_Status.Text = StatusName;
                        this.lb_Status.CssClass = StatusCss;

                        #endregion

                        //[物流資料]
                        #region -- 物流資料 --

                        this.ddl_ShipType.SelectedValue = ShipType;
                        this.ddl_ShipVendor.SelectedValue = DT.Rows[0]["ShipVendor"].ToString();
                        this.tb_ShipNo.Text = DT.Rows[0]["ShipNo"].ToString();
                        this.ddl_ShipFareType.SelectedValue = DT.Rows[0]["ShipFareType"].ToString();
                        this.tb_ShipFareMoney.Text = DT.Rows[0]["ShipFareMoney"].ToString();
                        this.tb_Remark.Text = DT.Rows[0]["Remark"].ToString();

                        //出貨日
                        string myShipDate = DT.Rows[0]["ShipDate"].ToString();
                        this.tb_ShipDate.Text = string.IsNullOrEmpty(myShipDate) ? DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd") : myShipDate.ToDateString("yyyy/MM/dd");

                        //填入Tag Html
                        if (!string.IsNullOrEmpty(DT.Rows[0]["BoxID"].ToString()))
                        {
                            StringBuilder itemHtml = new StringBuilder();

                            for (int row = 0; row < DT.Rows.Count; row++)
                            {
                                //取得參數
                                string myDataID = DT.Rows[row]["BoxID"].ToString();
                                string myID = DT.Rows[row]["BoxType"].ToString();
                                string myName = DT.Rows[row]["BoxName"].ToString();
                                string myQty = DT.Rows[row]["BoxNum"].ToString();


                                //組合Html
                                itemHtml.AppendLine("<li id=\"li_{0}_{1}\" class=\"list-group-item\">".FormatThis(row, myDataID));
                                itemHtml.AppendLine("<table width=\"100%\">");
                                itemHtml.AppendLine("<tr><td style=\"width: 70%\"><h4>{0}</h4></td>".FormatThis(myName));
                                itemHtml.AppendLine("<td class=\"text-center\" style=\"width: 15%\"><input type=\"text\" id=\"qty_{0}_{1}\" value=\"{2}\" class=\"Item_Qty text-center\" style=\"width: 60px;\" maxlength=\"8\" onkeyup=\"checkNum(this)\" /></td>".FormatThis(
                                    row, myDataID, myQty
                                    ));
                                itemHtml.AppendLine("<td class=\"text-right\" style=\"width: 15%\"><button type=\"button\" class=\"btn btn-default btn-xs\" onclick=\"Delete_Item('{0}_{1}');\"><i class=\"fa fa-times\"></i>&nbsp;移除</button></td>".FormatThis(
                                    row, myDataID
                                    ));
                                itemHtml.AppendLine("</tr></table>");

                                //Hidden field
                                itemHtml.Append("<input type=\"hidden\" class=\"Item_ID\" value=\"{0}\" />".FormatThis(myID));
                                itemHtml.Append("<input type=\"hidden\" class=\"Item_Name\" value=\"{0}\" />".FormatThis(myQty));
                                itemHtml.AppendLine("</li>");
                            }

                            this.lt_ViewList.Text = itemHtml.ToString();
                        }

                        #endregion

                        //[下單品項]
                        LookupData_OrderProd();

                        //[銷貨明細] - (Local不執行)
                        if (fn_Extensions.GetIP() != "")
                        {
                            LookupData_SaleData();
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 查看下單品項
    /// </summary>
    private void LookupData_OrderProd()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Model_No, Currency, SellPrice, BuyCnt, IsGift, Remark");
                SBSql.AppendLine(" FROM Order_Prod");
                SBSql.AppendLine(" WHERE (OrderID = @DataID)");
                SBSql.AppendLine(" ORDER BY IsGift, Model_No");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList_OrderProd.DataSource = DT.DefaultView;
                    this.lvDataList_OrderProd.DataBind();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 下單品項資料");
        }
    }

    /// <summary>
    /// 查看ERP銷貨明細
    /// </summary>
    private void LookupData_SaleData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                string Get_DBName = fn_Extensions.Get_DBName("1", out ErrMsg);

                SBSql.AppendLine(" SELECT RTRIM(COPTG.TG001) SaleNoType, RTRIM(COPTG.TG002) SaleNo");
                SBSql.AppendLine("  , RTRIM(COPTH.TH004) AS ModelNo, RTRIM(COPTH.TH005) AS ModelName, COPTH.TH008 AS BuyCnt, COPTH.TH024 AS GiftCnt");
                SBSql.AppendLine(" FROM [{0}].dbo.COPTG WITH(NOLOCK)".FormatThis(Get_DBName));
                SBSql.AppendLine("    INNER JOIN [{0}].dbo.COPTH WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002".FormatThis(Get_DBName));
                SBSql.AppendLine("    INNER JOIN [{0}].dbo.COPTC WITH(NOLOCK) ON COPTH.TH014 = COPTC.TC001 AND COPTH.TH015 = COPTC.TC002".FormatThis(Get_DBName));
                //--[條件]:網路訂單=Y
                SBSql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTC.TC012 <> '')");
                //--(LEFT(COPTC.TC012, LEN(COPTC.TC012) - 1))->暢聯
                SBSql.AppendLine("    AND (COPTC.TC012 = @DataID)");
                SBSql.AppendLine(" ORDER BY COPTH.TH004");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
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
            throw new Exception("系統發生錯誤 - 銷貨資料");
        }
    }

    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 按鈕 - 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (this.ddl_ShipType.SelectedIndex == 0)
            {
                SBAlert.Append("請選擇「出貨方式」\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            //資料儲存, 狀態 = 出貨中(1)
            if (false == doSave(1))
            {
                fn_Extensions.JsAlert("資料儲存發生錯誤", "");
                return;
            }
            //設定關聯
            if (false == Set_TagRel(Param_thisID))
            {
                fn_Extensions.JsAlert("箱子設定失敗", "");
                return;
            }

            //導向本頁
            Response.Redirect(PageUrl);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    protected void btn_Go_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (this.ddl_ShipType.SelectedIndex == 0)
            {
                SBAlert.Append("請選擇「出貨方式」\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            //資料儲存, 狀態 = 已出貨(2)
            if (false == doSave(2))
            {
                fn_Extensions.JsAlert("設為出貨時發生錯誤", "");
                return;
            }
            //設定關聯
            if (false == Set_TagRel(Param_thisID))
            {
                fn_Extensions.JsAlert("箱子設定失敗", "");
                return;
            }

            //導向本頁
            Response.Redirect(PageUrl);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 出貨", "");
            return;
        }

    }


    /// <summary>
    /// 資料儲存
    /// </summary>
    /// <param name="shipStatus">出貨狀態</param>
    /// <returns></returns>
    private bool doSave(int shipStatus)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Order_Shipping WHERE (OrderID = @DataID)) = 0 ");
            //Insert
            SBSql.AppendLine("  BEGIN");
            SBSql.AppendLine("  INSERT INTO Order_Shipping(");
            SBSql.AppendLine("    OrderID, ShipType, ShipVendor, ShipNo, ShipFareType, ShipFareMoney, ShipDate");
            SBSql.AppendLine("    , ShipStatus, Remark");
            SBSql.AppendLine("    , Create_Who, Create_Time");
            SBSql.AppendLine("  ) VALUES (");
            SBSql.AppendLine("    @DataID, @ShipType, @ShipVendor, @ShipNo, @ShipFareType, @ShipFareMoney, @ShipDate");
            SBSql.AppendLine("    , @ShipStatus, @Remark");
            SBSql.AppendLine("    , @Login_Who, GETDATE()");
            SBSql.AppendLine("  )");
            SBSql.AppendLine("  END");

            SBSql.AppendLine(" ELSE");

            //Update
            SBSql.AppendLine("  BEGIN");
            SBSql.AppendLine("  UPDATE Order_Shipping");
            SBSql.AppendLine("  SET ShipType = @ShipType");
            SBSql.AppendLine("    , ShipVendor = @ShipVendor, ShipNo = @ShipNo, ShipFareType = @ShipFareType, ShipFareMoney = @ShipFareMoney, ShipDate = @ShipDate");
            SBSql.AppendLine("    , ShipStatus = @ShipStatus, Remark = @Remark");
            SBSql.AppendLine("    , Update_Who = @Login_Who, Update_Time = GETDATE()");
            SBSql.AppendLine("  WHERE (OrderID = @DataID)");
            SBSql.AppendLine("  END");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("ShipType", this.ddl_ShipType.SelectedValue);
            cmd.Parameters.AddWithValue("ShipVendor", this.ddl_ShipVendor.SelectedValue);
            cmd.Parameters.AddWithValue("ShipNo", this.tb_ShipNo.Text);
            cmd.Parameters.AddWithValue("ShipFareType", this.ddl_ShipFareType.SelectedValue);
            cmd.Parameters.AddWithValue("ShipFareMoney", this.tb_ShipFareMoney.Text);
            cmd.Parameters.AddWithValue("ShipDate", string.IsNullOrEmpty(this.tb_ShipDate.Text) ? DBNull.Value : (Object)this.tb_ShipDate.Text);
            cmd.Parameters.AddWithValue("ShipStatus", shipStatus);
            cmd.Parameters.AddWithValue("Remark", this.tb_Remark.Text);
            cmd.Parameters.AddWithValue("Login_Who", fn_Params.UserGuid);

            return dbConn.ExecuteSql(cmd, out ErrMsg);

        }
    }


    /// <summary>
    /// 關聯設定
    /// </summary>
    /// <param name="DataID">單頭資料編號</param>
    /// <returns></returns>
    private bool Set_TagRel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.myValues.Text;
        string Get_Qtys = this.myValues_Qty.Text;

        //判斷是否為空
        if (string.IsNullOrEmpty(Get_IDs) || string.IsNullOrEmpty(Get_Qtys))
        {
            return true;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(Get_IDs, ",");
        string[] strAry_Qty = Regex.Split(Get_Qtys, ",");

        //宣告暫存清單
        List<TempParam> ITempList = new List<TempParam>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam(strAry_ID[row], strAry_Qty[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID,
                        Name = el.tmp_Qty
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID,
                        Name = gp.Key.Name
                    };

        //處理Tag資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            SBSql.AppendLine(" DELETE FROM Order_ShippingBox WHERE (OrderID = @DataID) ");
            SBSql.AppendLine(" Declare @New_ID AS INT ");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(BoxID), 0) + 1 FROM Order_ShippingBox WHERE (OrderID = @DataID)) ");
                SBSql.AppendLine(" INSERT INTO Order_ShippingBox( ");
                SBSql.AppendLine("  OrderID, BoxID, BoxType, BoxNum");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @New_ID, @BoxType_{0}, @BoxNum_{0}".FormatThis(row));
                SBSql.AppendLine(" ); ");

                cmd.Parameters.AddWithValue("BoxType_" + row, item.ID);
                cmd.Parameters.AddWithValue("BoxNum_" + row, item.Name);

            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", DataID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
    #endregion -- 資料編輯 End --


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
            return string.Format(@"Shipment_Edit.aspx?DataID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    #endregion

    #region -- 暫存參數 --
    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 編號
        /// </summary>
        private string _tmp_ID;
        public string tmp_ID
        {
            get { return this._tmp_ID; }
            set { this._tmp_ID = value; }
        }

        /// <summary>
        /// [參數] - 數量
        /// </summary>
        private string _tmp_Qty;
        public string tmp_Qty
        {
            get { return this._tmp_Qty; }
            set { this._tmp_Qty = value; }
        }


        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        /// <param name="tmp_Qty">數量</param>
        public TempParam(string tmp_ID, string tmp_Qty)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Qty = tmp_Qty;
        }
    }
    #endregion
}