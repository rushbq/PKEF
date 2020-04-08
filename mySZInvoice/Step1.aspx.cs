using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_Invoice.Aisino.Controllers;
using SZ_Invoice.Aisino.Models;

public partial class mySZInvoice_Step1 : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1010", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"]
                        , HttpUtility.UrlEncode(ErrMsg))
                        , true);
                    return;
                }

                //帶入預設日期
                //string defSdate = DateTime.Now.AddMonths(-1).ToString().ToDateString("yyyy/MM/dd");
                string defEdate = DateTime.Now.AddDays(-1).ToString().ToDateString("yyyy/MM/dd");
                this.filter_sDate.Text = defEdate;
                this.filter_eDate.Text = defEdate;

            }


        }
        catch (Exception)
        {

            throw;
        }
    }


    protected void btn_Next_Click(object sender, EventArgs e)
    {
        try
        {
            #region -- [Check] Check Date --

            //Get input
            string sDate = this.filter_sDate.Text;
            string eDate = this.filter_eDate.Text;

            //Check Null
            if (string.IsNullOrEmpty(sDate) || string.IsNullOrEmpty(eDate))
            {
                CustomExtension.AlertMsg("[檢查] 請選擇正確的日期", "");
                return;
            }

            //Convert to Date
            DateTime chksDate = Convert.ToDateTime(sDate);
            DateTime chkeDate = Convert.ToDateTime(eDate);

            //Check Date
            if (chksDate > chkeDate)
            {
                CustomExtension.AlertMsg("[檢查] 請選擇正確的日期區間", "");
                return;
            }

            //Check Range
            int cntDays = new TimeSpan(chkeDate.Ticks - chksDate.Ticks).Days;
            if (cntDays > 90)
            {
                CustomExtension.AlertMsg("[檢查] 日期區間不可超過 90 天", "");
                return;
            }

            #endregion


            #region -- [Check] Check CustID --

            //Get input
            string custID = this.Cust_ID_Val.Text;

            //Check Null
            if (string.IsNullOrEmpty(custID))
            {
                CustomExtension.AlertMsg("[檢查] 請選擇客戶", "");
                return;
            }

            #endregion


            #region -- [Data] 檢查檔建立 --

            //----- 宣告:資料參數 -----
            SZ_InvoiceRepository _data = new SZ_InvoiceRepository();
            string erp_sDate = sDate.ToDateString("yyyyMMdd");
            string erp_eDate = eDate.ToDateString("yyyyMMdd");

            //----- 原始資料:取得未開票的資料 -----
            var query = _data.CheckInvoiceData(custID, erp_sDate, erp_eDate);
            if (query.Count() == 0)
            {
                CustomExtension.AlertMsg("[檢查] 查無可開票資料,請確認", "");
                return;
            }

            //取第一筆資料
            var queryTop1 = query.Take(1)
                .Select(fld => new
                {
                    CustID = fld.CustID,
                    CustName = fld.CustName
                }).FirstOrDefault();

            //設定主旨#{GUID在SQL執行時插入}
            string subject = "({0}) {1} #".FormatThis(queryTop1.CustID, queryTop1.CustName);


            //----- 執行新增, 取得Guid -----
            string getGuid = _data.CreateBaseData(subject, queryTop1.CustID, erp_sDate, erp_eDate, query, out ErrMsg);

            //判斷GUID是否為空
            if (string.IsNullOrEmpty(getGuid))
            {
                CustomExtension.AlertMsg("[系統] 自動取號失敗,請重試.", PageUrl);
                return;
            }
            else
            {
                //前往下一步
                Response.Redirect("{0}mySZInvoice/Step2.aspx?dataID={1}".FormatThis(
                    Application["WebUrl"]
                    , getGuid));
            }

            #endregion
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}mySZInvoice/Step1.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;

    #endregion

}