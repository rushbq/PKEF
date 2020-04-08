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

public partial class Cust_View : SecurityIn
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
                SBSql.AppendLine("  , RTRIM(Base.MA001) CustID, RTRIM(Base.MA003) CustName, SW.SW_Name_zh_TW AS SW_Name");
                SBSql.AppendLine("  , RTRIM(myArea.MR003) AreaName, RTRIM(myCountry.MR003) CountryName");
                SBSql.AppendLine("  , Prof.Account_Name RepSalesID, Prof.Display_Name RepSales");
                SBSql.AppendLine("  , Corp.Corp_Name");
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
                        this.lt_CustID.Text = DT.Rows[0]["CustID"].ToString();
                        this.lt_DBName.Text = DT.Rows[0]["Corp_Name"].ToString();
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
                        this.lt_SW.Text = DT.Rows[0]["SW_Name"].ToString();

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

    #endregion -- 資料顯示 End --

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
    #endregion

}