using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SH_Invoice.Controllers;
using SH_Invoice.Models;

public partial class mySHInvoice_ErpInvStep1 : SecurityIn
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

            //Next Step
            Response.Redirect("{0}mySHInvoice/ErpInvStep2.aspx?cust={1}&sd={2}&ed={3}".FormatThis(
                   Application["WebUrl"]
                   , Server.UrlEncode(custID)
                   , Server.UrlEncode(sDate)
                   , Server.UrlEncode(eDate)));

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
            return "{0}mySHInvoice/ErpInvStep1.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;

    #endregion

}