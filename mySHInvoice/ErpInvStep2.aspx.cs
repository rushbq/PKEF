using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SH_Invoice.Controllers;
using SH_Invoice.Models;


public partial class mySHInvoice_ErpInvStep2 : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1110", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"]
                        , HttpUtility.UrlEncode(ErrMsg))
                        , true);
                    return;
                }


                //判斷參數是否為空
                Check_Params();

                //取得資料
                LookupData();

            }


        }
        catch (Exception)
        {

            throw;
        }
    }



    #region -- 資料讀取 --

    /// <summary>
    /// 判斷參數是否為空
    /// </summary>
    private void Check_Params()
    {
        if (string.IsNullOrEmpty(Req_CustID))
        {
            this.ph_Message.Visible = true;
            this.ph_Content.Visible = false;
            this.ph_Buttons.Visible = false;
        }
        else
        {
            this.ph_Message.Visible = false;
            this.ph_Content.Visible = true;
            this.ph_Buttons.Visible = true;
        }
    }


    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SH_InvoiceRepository _data = new SH_InvoiceRepository();
        string erp_sDate = Req_sDate.ToDateString("yyyyMMdd");
        string erp_eDate = Req_eDate.ToDateString("yyyyMMdd");

        //----- 原始資料:取得所有資料 -----
        var _datalist = _data.GetErpUnBilledData(Req_CustID, erp_sDate, erp_eDate);
        if (_datalist == null || _datalist.Count() == 0)
        {
            this.ph_Content.Visible = false;
            this.ph_Buttons.Visible = false;
            CustomExtension.AlertMsg("查無資料,請重選條件!", "{0}mySHInvoice/ErpInvStep1.aspx".FormatThis(Application["WebUrl"]));
            return;
        }

        //----- 資料繫結 -----
        this.lvDataList.DataSource = _datalist;
        this.lvDataList.DataBind();


        //----- 取得第一筆資料 -----
        var query = _datalist.Take(1)
            .Select(fld => new
            {
                CustID = fld.CustID,
                CustName = fld.CustName

            }).FirstOrDefault();


        //----- 資料整理:填入資料 -----
        string CustName = "{1}&nbsp;({0})".FormatThis(query.CustID, query.CustName);
        this.lt_CustName.Text = CustName;    


        query = null;

    }


    #endregion

    
    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得CustID
    /// </summary>
    public string Req_CustID
    {
        get
        {
            string data = Request.QueryString["cust"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_CustID = value;
        }
    }
    private string _Req_CustID;


    /// <summary>
    /// 設定參數 - 取得sDate
    /// </summary>
    public string Req_sDate
    {
        get
        {
            string data = Request.QueryString["sd"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_sDate = value;
        }
    }
    private string _Req_sDate;


    /// <summary>
    /// 設定參數 - 取得eDate
    /// </summary>
    public string Req_eDate
    {
        get
        {
            string data = Request.QueryString["ed"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_eDate = value;
        }
    }
    private string _Req_eDate;

    #endregion

}