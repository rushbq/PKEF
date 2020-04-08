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
using System.Collections;
using Newtonsoft.Json;

public partial class Order_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 訂單
                if (fn_CheckAuth.CheckAuth_User("710", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    #region -- 資料編輯 Start --
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region >>> 1. 取得資料 <<<

            //客戶編號
            string custID = this.tb_CustID.Text.Trim();
            //資料庫別
            string DBS = this.tb_DBS.Text;
            //庫別
            string SWID = this.tb_SWID.Text;
            //輸入品號(多筆)
            string modelNo = this.myValues.Text;
            //輸入數量
            string qty = this.myValues_Qty.Text;
            //業務人員
            string SalesID = this.tb_SalesID.Text;
            //收貨人
            string ShipWho = this.tb_ShipWho.Text;
            //郵遞區號
            string ZipCode = this.tb_ZipCode.Text;
            //收貨地址
            string ShipAddr = this.tb_ShipAddr.Text;
            //聯絡電話
            string Tel = this.tb_ContactTel.Text;
            //自訂單號
            string myOrderID = this.tb_myOrderID.Text;

            /*
             * [取得ERP資料 - DataTable]
             * <回傳欄位>
             * ModelNo
             * Model_Name_zh_TW
             * Currency
             * UnitPrice
             * SpQty
             * SpQtyPrice
             * BuyQty
             * StockNum
             */

            //產生追蹤編號
            string TraceID = custID + Cryptograph.GetCurrentTime().ToString();

            //取得ERP資料
            API_ERPData ERPData = new API_ERPData();
            DataTable DT = ERPData.GetData(custID, DBS, modelNo, qty, SWID, out ErrMsg);
            if (DT == null)
            {
                fn_Extensions.JsAlert("無法取得ERP資料，請檢查填寫資料", "");
                return;
            }
            if (DT.Rows.Count == 0)
            {
                fn_Extensions.JsAlert("無法取得ERP資料", "");
                return;
            }

            //存入基本資料
            List<myBaseData> iBaseData = new List<myBaseData>();
            iBaseData.Add(new myBaseData(custID, DBS, SWID, SalesID, ShipWho, ZipCode, ShipAddr, Tel, myOrderID));

            #endregion


            #region >>> 2. 建立訂單 <<<

            if (false == Insert_Order(iBaseData, DT, TraceID, out ErrMsg))
            {
                fn_Extensions.JsAlert("訂單建立失敗!...ID={0}".FormatThis(TraceID), "");
                Response.Write(ErrMsg);
                return;
            }

            #endregion


            #region >>> 3. 轉入EDI <<<

            if (false == Insert_EDI(TraceID, out ErrMsg))
            {
                fn_Extensions.JsAlert("EDI轉入失敗", "");
                return;
            }

            //成功,跳轉回第一步
            fn_Extensions.JsAlert("訂單轉入成功!", PageUrl);

            #endregion
        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 建立訂單
    /// </summary>
    /// <param name="iBaseData">基本資料</param>
    /// <param name="myDT">ERP資料</param>
    /// <param name="TraceID">追蹤編號</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool Insert_Order(List<myBaseData> iBaseData, DataTable myDT, string TraceID, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //設定欄位值
                string orderDate = DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd");

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                //[SQL] - 取得訂單編號(共16碼:2碼代碼 + 西元年 + 月 + 日 + 6碼流水號)
                SBSql.AppendLine(" DECLARE @NewID AS INT, @NewOrderID AS varchar(16) ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(OrderAutoNum), 0) + 1 AS NewID ");
                SBSql.AppendLine("  FROM Order_Main ");
                SBSql.AppendLine("  WHERE OrderDate = @OrderDate ");
                SBSql.AppendLine(" )");
                SBSql.AppendLine(" SET @NewOrderID = 'TW' + @OrderDate + RIGHT('00000' + CAST(@NewID AS VARCHAR), 6);");


                //[SQL] - 新增 Order_Main
                #region * 新增 Order_Main *
                SBSql.Append("INSERT INTO Order_Main(");
                SBSql.Append(" OrderID, OrderAutoNum");
                SBSql.Append(" , MyOrderID, OrderDate, CustERPID, Salesman");
                SBSql.Append(" , ShipWho, ShippingAddr, ZipCode, ContactTel");
                SBSql.Append(" , TraceID, Remark, Create_Who, Create_Time");
                SBSql.Append(" )VALUES( ");
                SBSql.Append(" @NewOrderID, @NewID");
                SBSql.Append(" , @MyOrderID, @OrderDate, @CustERPID, @Salesman");
                SBSql.Append(" , @ShipWho, @ShippingAddr, @ZipCode, @ContactTel");
                SBSql.Append(" , @TraceID, @Remark, @Create_Who, GETDATE()");
                SBSql.AppendLine(");");

                cmd.Parameters.AddWithValue("MyOrderID", iBaseData[0].myOrderID.Trim().Left(20));
                cmd.Parameters.AddWithValue("OrderDate", orderDate);
                cmd.Parameters.AddWithValue("CustERPID", iBaseData[0].CustID);
                cmd.Parameters.AddWithValue("Salesman", iBaseData[0].SalesID);
                cmd.Parameters.AddWithValue("ShipWho", iBaseData[0].ShipWho.Left(50));
                cmd.Parameters.AddWithValue("ShippingAddr", iBaseData[0].ShipAddr.Left(200));
                cmd.Parameters.AddWithValue("ZipCode", iBaseData[0].ZipCode.Left(10));
                cmd.Parameters.AddWithValue("ContactTel", iBaseData[0].Tel.Left(40));
                cmd.Parameters.AddWithValue("TraceID", TraceID);
                cmd.Parameters.AddWithValue("Remark", "");
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);

                #endregion


                //[SQL] - 新增 Order_SW
                #region * 新增 Order_SW *

                SBSql.Append("INSERT INTO Order_SW(");
                SBSql.Append("OrderID, SWID");
                SBSql.Append(" )VALUES( ");
                SBSql.Append("@NewOrderID, @SWID");
                SBSql.AppendLine(");");

                cmd.Parameters.AddWithValue("SWID", iBaseData[0].SWID);

                #endregion


                //[SQL] - 新增 Order_Prod
                #region * 新增 Order_Prod *

                for (int row = 0; row < myDT.Rows.Count; row++)
                {
                    SBSql.Append("INSERT INTO Order_Prod(");
                    SBSql.Append("OrderID, SWID, Model_No, Currency, SellPrice, BuyCnt, StockNum");
                    SBSql.Append(" )VALUES( ");
                    SBSql.Append("@NewOrderID, @SWID");
                    SBSql.Append(", @ModelNo_{0}, @Currency_{0}, @SellPrice_{0}, @BuyCnt_{0}, @StockNum_{0}".FormatThis(row));
                    SBSql.AppendLine(");");

                    //[價格重計]含稅除1.05
                    int erpPrice = Convert.ToInt32(myDT.Rows[row]["SpQtyPrice"]);
                    if (myDT.Rows[row]["IsTax"].ToString().Equals("Y"))
                    {
                        erpPrice = Convert.ToInt32(Math.Round(Convert.ToDouble(myDT.Rows[row]["SpQtyPrice"]) / 1.05, 0));
                    }

                    cmd.Parameters.AddWithValue("ModelNo_{0}".FormatThis(row), myDT.Rows[row]["ModelNo"].ToString());
                    cmd.Parameters.AddWithValue("Currency_{0}".FormatThis(row), myDT.Rows[row]["Currency"].ToString());
                    cmd.Parameters.AddWithValue("SellPrice_{0}".FormatThis(row), erpPrice);
                    cmd.Parameters.AddWithValue("BuyCnt_{0}".FormatThis(row), myDT.Rows[row]["BuyQty"].ToString());
                    cmd.Parameters.AddWithValue("StockNum_{0}".FormatThis(row), myDT.Rows[row]["StockNum"].ToString());
                }

                #endregion


                #region * 新增 贈品 *

                //輸入品號(多筆)
                string modelNos = this.myValues_Gift.Text;

                //輸入數量
                string qtys = this.myValues_Gift_Qty.Text;

                //庫存數量
                string stocks = this.myValues_Gift_StockNum.Text;


                if (!string.IsNullOrEmpty(modelNos))
                {
                    //取得陣列資料
                    string[] strAry_ID = Regex.Split(modelNos, @"\,{1}");
                    string[] strAry_Qty = Regex.Split(qtys, @"\,{1}");
                    string[] strAry_Stock = Regex.Split(stocks, @"\,{1}");

                    //宣告暫存清單
                    List<TempParam_Item> ITempList = new List<TempParam_Item>();

                    //存入暫存清單
                    for (int row = 0; row < strAry_ID.Length; row++)
                    {
                        ITempList.Add(
                            new TempParam_Item(strAry_ID[row]
                                , Convert.ToInt16(strAry_Qty[row])
                                , Convert.ToInt16(strAry_Stock[row]))
                            );
                    }

                    //過濾重複資料
                    var query = from el in ITempList
                                group el by new
                                {
                                    ID = el.tmp_ID,
                                    Qty = el.tmp_Qty,
                                    Stock = el.tmp_Stock
                                } into gp
                                select new
                                {
                                    ID = gp.Key.ID,
                                    Qty = gp.Key.Qty,
                                    Stock = gp.Key.Stock
                                };

                    int idx = 0;
                    foreach (var item in query)
                    {
                        SBSql.Append("INSERT INTO Order_Prod(");
                        SBSql.Append("OrderID, SWID, Model_No, Currency, SellPrice, BuyCnt, StockNum, IsGift, Remark");
                        SBSql.Append(" )VALUES( ");
                        SBSql.Append("@NewOrderID, @SWID");
                        SBSql.Append(", @Gift_Model_{0}, @myCurrency, 0, @BuyCnt_Gift_{0}, @Gift_StockNum_{0}, 'Y', '贈品'".FormatThis(idx));
                        SBSql.AppendLine(");");

                        cmd.Parameters.AddWithValue("Gift_Model_" + idx, item.ID);
                        cmd.Parameters.AddWithValue("BuyCnt_Gift_" + idx, item.Qty);
                        cmd.Parameters.AddWithValue("Gift_StockNum_" + idx, item.Stock);

                        idx++;
                    }
                }

                #endregion


                #region * 新增 運費 (B001) *
                //SWID = 98, 庫別對應 10
                double myFreight = Convert.ToDouble(this.tb_Freight.Text);
                if (myFreight > 0)
                {
                    SBSql.Append("INSERT INTO Order_SW(");
                    SBSql.Append("OrderID, SWID");
                    SBSql.Append(" )VALUES( ");
                    SBSql.Append("@NewOrderID, 98");
                    SBSql.AppendLine(");");

                    SBSql.Append("INSERT INTO Order_Prod(");
                    SBSql.Append("OrderID, SWID, Model_No, Currency, SellPrice, BuyCnt, StockNum, Remark");
                    SBSql.Append(" )VALUES( ");
                    SBSql.Append("@NewOrderID, 98");
                    SBSql.Append(", 'B001', @myCurrency, @FreightPrice, 1, 99, '運費'");
                    SBSql.AppendLine(");");

                    cmd.Parameters.AddWithValue("FreightPrice", myFreight);
                }

                #endregion


                #region * 新增 折扣 (W001) *
                double myDisPrice = Convert.ToDouble(this.tb_DisPrice.Text);
                if (myDisPrice > 0)
                {
                    SBSql.Append("INSERT INTO Order_Prod(");
                    SBSql.Append("OrderID, SWID, Model_No, Currency, SellPrice, BuyCnt, StockNum, Remark");
                    SBSql.Append(" )VALUES( ");
                    SBSql.Append("@NewOrderID, @SWID");
                    SBSql.Append(", 'W001', @myCurrency, @SellPrice_Dis, 1, 99, @DisRemark");
                    SBSql.AppendLine(");");

                    cmd.Parameters.AddWithValue("SellPrice_Dis", -myDisPrice);
                    cmd.Parameters.AddWithValue("DisRemark", this.tb_DisRemark.Text.Right(200));
                }

                #endregion

                //[SQL] - 執行
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("myCurrency", myDT.Rows[0]["Currency"].ToString());

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }
        catch (Exception ex)
        {
            //顯示失敗
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 建立EDI
    /// </summary>
    /// <param name="TraceID">追蹤編號</param>
    /// <returns></returns>
    /// <remarks>
    /// [EDI欄位]
    /// XA001   公司別	C	20.0	客戶提供
    /// XA002   型態別	C	1.0	1.一般訂單轉銷貨 2.一般訂單 3.報價單 A.多角訂單
    /// XA003   單別	C	4.0	客戶提供
    /// XA004   單號	C	11.0	依單別編碼方式判斷 1.客戶提供 2.由系統自動編碼 -> (填NULL)
    /// XA005   流程代號	C	2.0	客戶提供 -> (填NULL)
    /// XA006   平台單號	C	20.0	平台轉入 (PK)(系統產生的單號TW2018xxxx)
    /// XA007   客戶代號	C	10.0	平台轉入
    /// XA008   業務人員	C	10.0	平台轉入
    /// XA009   幣別	C	4.0	平台轉入
    /// XA010   單據日期	C	8.0	平台轉入
    /// XA011   品號	C	40.0	平台轉入 (PK)
    /// XA012   數量	N	16.3	平台轉入
    /// XA013   單價	N	21.6	平台轉入
    /// XA014   庫別	C	10.0	客戶提供 (PK)
    /// XA015   出貨地址	V	255.0	客戶提供
    /// XA016   預交日	C	8.0	客戶提供
    /// XA017   生效日	C	8.0	客戶提供
    /// XA020   贈/備品 (1/2)
    /// XA021   贈/備品量
    /// XA022   收貨人
    /// XA023   電話
    /// XA024   來源平台
    /// XA025   原始單號(自訂單號)
    /// </remarks>
    private bool Insert_EDI(string TraceID, out string ErrMsg)
    {
        //[設定參數], 取得TokeID
        string TokenID = System.Web.Configuration.WebConfigurationManager.AppSettings["API_TokenID"];

        //建立DataTable, 從訂單取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數
            cmd.Parameters.Clear();

            //[SQL] - 讀取訂單資料
            SBSql.AppendLine("SELECT Param_Corp.Corp_ID AS XA001 ");
            /* 判斷:庫存-購買量 >= 0 */
            SBSql.AppendLine("    , (CASE WHEN (Order_Prod.StockNum - Order_Prod.BuyCnt) >= 0 THEN 1 ELSE 2 END) AS XA002 ");
            SBSql.AppendLine("    , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_單別') AND (UPPER(Param_Name) = UPPER(Param_Corp.DB_Name))) AS XA003 ");
            SBSql.AppendLine("    , Order_Main.OrderID AS XA006 ");
            SBSql.AppendLine("    , Order_Main.CustERPID AS XA007 ");
            SBSql.AppendLine("    , Order_Main.Salesman AS XA008 ");
            SBSql.AppendLine("    , Order_Prod.Currency AS XA009 ");
            SBSql.AppendLine("    , Order_Main.OrderDate AS XA010 ");
            SBSql.AppendLine("    , RTRIM(Order_Prod.Model_No) AS XA011 ");
            //SBSql.AppendLine("    , Order_Prod.BuyCnt AS XA012 ");
            SBSql.AppendLine("    , (CASE WHEN (Order_Prod.IsGift) = 'Y' THEN 0 ELSE Order_Prod.BuyCnt END) AS XA012 ");
            SBSql.AppendLine("    , Order_Prod.SellPrice AS XA013 ");
            SBSql.AppendLine("    , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_庫別') AND (UPPER(Param_Name) = UPPER(Order_Prod.SWID))) AS XA014 ");
            SBSql.AppendLine("    , LEFT(Order_Main.ShippingAddr, 250) AS XA015 ");
            SBSql.AppendLine("    , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_預交日') AND (UPPER(Param_Name) = UPPER(Param_Corp.DB_Name))) AS StockDay ");
            SBSql.AppendLine("    , CONVERT(varchar(8), Order_Main.Create_Time, 112) AS XA017 ");

            SBSql.AppendLine("    , 1 AS XA020 ");
            SBSql.AppendLine("    , (CASE WHEN (Order_Prod.IsGift) = 'Y' THEN Order_Prod.BuyCnt ELSE 0 END) AS XA021 ");
            SBSql.AppendLine("    , LEFT(Order_Main.ShipWho, 30) AS XA022 ");
            SBSql.AppendLine("    , LEFT(Order_Main.ContactTel, 20) AS XA023");
            SBSql.AppendLine("    , '內業BBC' AS XA024 ");
            SBSql.AppendLine("    , Order_Main.MyOrderID AS XA025 ");
            SBSql.AppendLine(" FROM Order_Main ");
            SBSql.AppendLine("    INNER JOIN Order_SW ON Order_Main.OrderID = Order_SW.OrderID ");
            SBSql.AppendLine("    INNER JOIN Order_Prod ON Order_SW.OrderID = Order_Prod.OrderID AND Order_SW.SWID = Order_Prod.SWID ");
            SBSql.AppendLine("    INNER JOIN PKSYS.dbo.Customer ON Order_Main.CustERPID = Customer.MA001 AND Customer.DBS = Customer.DBC ");
            SBSql.AppendLine("    INNER JOIN PKSYS.dbo.Param_Corp ON Customer.DBC = Param_Corp.Corp_ID ");
            SBSql.AppendLine(" WHERE (Order_Main.TraceID = @TraceID) ");


            //[SQL] - 執行
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("TraceID", TraceID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return false;
                }

                //新增欄位 - 預交日
                DT.Columns.Add("XA016", typeof(string));

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    //設定預交日
                    string myBookDay = GetWorkDate(DateTime.Now, Convert.ToInt16(DT.Rows[row]["StockDay"])).ToShortDateString().ToDateString("yyyyMMdd");
                    DT.Rows[row]["XA016"] = myBookDay;
                }

                //呼叫Webservice - ws_EDI
                API_EDI.ws_EDI EDI = new API_EDI.ws_EDI();

                //傳送DataTable 給EDI
                DT.TableName = "MyEDITable";

                //轉成DataSet
                DataSet myDS = new DataSet();
                myDS.Tables.Add(DT);

                //回傳DT (資料DataSet, Token, 測試模試(Y/N))
                return EDI.InsertData_byDT(myDS, TokenID, TestMode, out ErrMsg);

            }
        }
    }

    #endregion -- 資料編輯 End --


    #region -- 其他功能 --

    /// <summary>
    /// 取得 N 個工作日後的日期
    /// </summary>
    /// <param name="inputDate">輸入日期</param>
    /// <param name="inputDays">N</param>
    /// <returns></returns>
    private DateTime GetWorkDate(DateTime inputDate, int inputDays)
    {
        DateTime tempDT = inputDate;
        while (inputDays-- > 0)
        {
            tempDT = tempDT.AddDays(1);
            while (tempDT.DayOfWeek == DayOfWeek.Saturday || tempDT.DayOfWeek == DayOfWeek.Sunday)
            {
                tempDT = tempDT.AddDays(1);
            }
        }
        return tempDT;
    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "Order_Edit.aspx";
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// 是否為測試模式
    /// </summary>
    private string _TestMode;
    public string TestMode
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["EDI_TestMode"];
        }
        set
        {
            this._TestMode = value;
        }
    }
    #endregion


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam_Item
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
        private int _tmp_Qty;
        public int tmp_Qty
        {
            get { return this._tmp_Qty; }
            set { this._tmp_Qty = value; }
        }


        /// <summary>
        /// [參數] - 庫存量
        /// </summary>
        private int _tmp_Stock;
        public int tmp_Stock
        {
            get { return this._tmp_Stock; }
            set { this._tmp_Stock = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        public TempParam_Item(string tmp_ID)
        {
            this._tmp_ID = tmp_ID;
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        /// <param name="tmp_Qty">數量</param>
        public TempParam_Item(string tmp_ID, int tmp_Qty, int tmp_Stock)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Qty = tmp_Qty;
            this._tmp_Stock = tmp_Stock;
        }
    }

    #region -- 自訂參數 --
    /// <summary>
    /// 暫存資料
    /// </summary>
    public class myBaseData
    {
        /// <summary>
        /// 客戶編號
        /// </summary>
        private string _CustID;
        public string CustID
        {
            get { return this._CustID; }
            set { this._CustID = value; }
        }

        /// <summary>
        /// 資料庫別
        /// </summary>
        private string _DBS;
        public string DBS
        {
            get { return this._DBS; }
            set { this._DBS = value; }
        }

        /// <summary>
        /// 出貨庫別
        /// </summary>
        private string _SWID;
        public string SWID
        {
            get { return this._SWID; }
            set { this._SWID = value; }
        }

        /// <summary>
        /// 業務人員
        /// </summary>
        private string _SalesID;
        public string SalesID
        {
            get { return this._SalesID; }
            set { this._SalesID = value; }
        }

        /// <summary>
        /// 收貨人
        /// </summary>
        private string _ShipWho;
        public string ShipWho
        {
            get { return this._ShipWho; }
            set { this._ShipWho = value; }
        }

        /// <summary>
        /// 郵遞區號
        /// </summary>
        private string _ZipCode;
        public string ZipCode
        {
            get { return this._ZipCode; }
            set { this._ZipCode = value; }
        }

        /// <summary>
        /// 收貨地址
        /// </summary>
        private string _ShipAddr;
        public string ShipAddr
        {
            get { return this._ShipAddr; }
            set { this._ShipAddr = value; }
        }

        /// <summary>
        /// 聯絡電話
        /// </summary>
        private string _Tel;
        public string Tel
        {
            get { return this._Tel; }
            set { this._Tel = value; }
        }

        /// <summary>
        /// 自訂單號
        /// </summary>
        private string _myOrderID;
        public string myOrderID
        {
            get { return this._myOrderID; }
            set { this._myOrderID = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        public myBaseData(string CustID, string DBS, string SWID, string SalesID, string ShipWho, string ZipCode, string ShipAddr, string Tel, string myOrderID)
        {
            this._CustID = CustID;
            this._DBS = DBS;
            this._SWID = SWID;
            this._SalesID = SalesID;
            this._ShipWho = ShipWho;
            this._ZipCode = ZipCode;
            this._ShipAddr = ShipAddr;
            this._Tel = Tel;
            this._myOrderID = myOrderID;
        }
    }

    #endregion
}