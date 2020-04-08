using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_Invoice.Aisino.Controllers;
using SZ_Invoice.Aisino.Models;

public partial class mySZInvoice_BatchStep1 : SecurityIn
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
                string defEdate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
                this.filter_sDate.Text = defEdate;
                this.filter_eDate.Text = defEdate;

            }


        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// 帶出要匯入的客戶
    /// </summary>
    protected void btn_GetData_Click(object sender, EventArgs e)
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

        //----- 宣告:資料參數 -----
        SZ_InvoiceRepository _data = new SZ_InvoiceRepository();

        //----- 原始資料:取得未開票的資料 -----
        var query = _data.CheckInvoiceDataGroup(sDate.ToDateString("yyyyMMdd"), eDate.ToDateString("yyyyMMdd"))
            .Select(fld => new
            {
                CustID = fld.CustID,
                CustName = fld.CustName,
                TotalPrice = fld.TotalPrice
            });
        if (query.Count() == 0)
        {
            CustomExtension.AlertMsg("[檢查] 查無可開票資料,請確認", "");
            return;
        }

        //----- 資料繫結 -----
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();
    }



    /// <summary>
    /// 批次產生轉入檔
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        try
        {
            //取得已勾選的客戶
            string GetCustGroup = this.tb_CbxValues.Text;
            if (string.IsNullOrEmpty(GetCustGroup))
            {
                CustomExtension.AlertMsg("至少要勾選一家客戶，請確認!", "");
            }

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


            #region -- [Data] 檢查檔建立 --

            //----- 宣告:資料參數 -----
            SZ_InvoiceRepository _data = new SZ_InvoiceRepository();
            string batchErrMsg = "";
            string erp_sDate = sDate.ToDateString("yyyyMMdd");
            string erp_eDate = eDate.ToDateString("yyyyMMdd");

            /*
               勾選填入原始值為: C012345,C123408 -- 依「,」分組
             */
            string[] aryCust = Regex.Split(GetCustGroup, @"\,{1}");
            foreach (string custID in aryCust)
            {
                //[BaseData] - 取得完整資料(含單號)
                var query = _data.CheckInvoiceData(custID, erp_sDate, erp_eDate);

                //[LoopData] - 資料迴圈(客戶), 將取回資料GroupBy
                var queryLoop = query
                    .GroupBy(gp => new
                    {
                        CustID = gp.CustID,
                        CustName = gp.CustName
                    })
                    .Select(fld => new
                    {
                        CustID = fld.Key.CustID,
                        CustName = fld.Key.CustName
                    });
                foreach (var job in queryLoop)
                {
                    //設定主旨#{GUID在SQL執行時插入}
                    string subject = "批次-({0}) {1} #".FormatThis(job.CustID, job.CustName);


                    //----- 執行新增, 並取得Guid -----
                    string getGuid = _data.CreateBaseData(subject, custID, erp_sDate, erp_eDate, query, out ErrMsg);


                    //判斷GUID是否為空
                    if (string.IsNullOrEmpty(getGuid))
                    {
                        batchErrMsg += "{0},取號失敗\n".FormatThis(job.CustName);
                    }
                }
            }

            #endregion


            //Show alert
            if (!string.IsNullOrEmpty(batchErrMsg))
            {
                CustomExtension.AlertMsg(batchErrMsg, "");
                return;
            }

            //redirect
            Response.Redirect("{0}mySZInvoice/List.aspx".FormatThis(Application["WebUrl"]));
        }
        catch (Exception)
        {

            throw;
        }
    }

}