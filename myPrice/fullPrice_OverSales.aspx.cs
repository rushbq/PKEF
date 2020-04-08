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

public partial class myPrice_fullPrice_OverSales : SecurityIn
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
                LookupData_Base();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 取得基本資料
    /// </summary>
    private void LookupData_Base()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT RTRIM(MA001) AS CustID, RTRIM(MA002) AS CustName");
                SBSql.AppendLine(" FROM Customer WITH(NOLOCK) ");
                SBSql.AppendLine(" WHERE (DBS = DBC) AND (MA001 = @DataID); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "index.aspx");
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.lt_CustName.Text = "({0}){1}".FormatThis(DT.Rows[0]["CustID"].ToString(), DT.Rows[0]["CustName"].ToString());

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 基本資料");
        }
    }


    #endregion

    #region -- 按鈕設定 --

    /// <summary>
    /// 匯出PriceList Excel
    /// </summary>
    protected void btn_PriceList_Click(object sender, EventArgs e)
    {
        //查詢StoreProcedure (myPrc_GetCustFullPrice_OverSales)
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "myPrc_GetCustFullPrice_OverSales";
            cmd.Parameters.AddWithValue("CustID", Req_DataID);
            cmd.Parameters.AddWithValue("ProdClass", DBNull.Value);
            cmd.CommandTimeout = 90;

            //取得回傳值
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
            {
                if (DT == null)
                {
                    fn_Extensions.JsAlert("No Data." + ErrMsg, "");
                    return;
                }

                if (DT.Rows.Count == 0)
                {
                    fn_Extensions.JsAlert("Fail", "");
                    return;
                }

                //取得Datatable, 篩選欄位
                var query =
                from el in DT.AsEnumerable()
                orderby el.Field<string>("Model_No")
                select new
                {
                    Stop_Offer = el.Field<string>("Stop_Offer"),
                    Item_NO = el.Field<string>("Model_No"),
                    Description = el.Field<string>("Model_Name_en_US"),
                    Class = el.Field<string>("ClassName_en_US"),
                    Currency = el.Field<string>("Currency"),
                    Unit_Price = el.Field<double?>("myPrice"),
                    Unit = el.Field<string>("Unit"),
                    Quote_Date = el.Field<string>("QuoteDate"),
                    MOQ = el.Field<int?>("MOQ"),
                    VOL = el.Field<string>("Vol"),
                    Page = el.Field<string>("Page"),
                    Qty_Inner = el.Field<int?>("InnerBox_Qty"),
                    NW = el.Field<double?>("InnerBox_NW"),
                    GW = el.Field<double?>("InnerBox_GW"),
                    CUFT = el.Field<double?>("InnerBox_Cuft"),
                    BarCode = el.Field<string>("BarCode"),
                    Packing = el.Field<string>("Packing_en_US".FormatThis(fn_Language.Param_Lang)),
                    Ship_From = el.Field<string>("Ship_From"),
                    Term = el.Field<string>("TransTermValue")
                };

                //Linq轉DataTable
                DataTable myDT = fn_CustomUI.LINQToDataTable(query);

                //匯出Excel
                fn_CustomUI.ExportExcel(myDT
                    , "{0}-PriceList-{1}.xlsx".FormatThis(
                     DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd")
                     , this.lt_CustName.Text)
                    );
            }
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
            return string.IsNullOrEmpty(data) ? "" : Cryptograph.MD5Decrypt(data, fn_Params.DesKey);
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    #endregion

}