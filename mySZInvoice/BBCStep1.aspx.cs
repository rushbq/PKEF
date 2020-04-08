using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_BBC.Controllers;
using SZ_Invoice.Aisino.Controllers;
using SZ_Invoice.Aisino.Models;

public partial class mySZInvoice_BBCStep1 : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1012", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"]
                        , HttpUtility.UrlEncode(ErrMsg))
                        , true);
                    return;
                }

                //Get Class
                Get_ClassList(myClass.mall, filter_Mall, "選擇商城");

                //帶入預設日期
                string defSdate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
                string defEdate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
                this.filter_sDate.Text = defSdate;
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
        string mall = filter_Mall.SelectedValue;
        string sDate = this.filter_sDate.Text;
        string eDate = this.filter_eDate.Text;

        //Check Null
        if (string.IsNullOrWhiteSpace(mall) || string.IsNullOrWhiteSpace(sDate) || string.IsNullOrWhiteSpace(eDate))
        {
            CustomExtension.AlertMsg("[檢查] 請選擇商城及正確的日期", "");
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
        if (cntDays > 30)
        {
            CustomExtension.AlertMsg("[檢查] 日期區間不可超過 30 天", "");
            return;
        }

        #endregion

        //----- 宣告:資料參數 -----
        SZ_InvoiceRepository _data = new SZ_InvoiceRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        search.Add("Check", "Y");

        //----- 原始資料:取得未開票的資料 -----
        var query = _data.GetBBCBillData(mall, sDate.ToDateString("yyyyMMdd"), eDate.ToDateString("yyyyMMdd"), search, out ErrMsg);
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
            //取得已勾選
            string GetDataGroup = this.tb_CbxValues.Text;
            if (string.IsNullOrEmpty(GetDataGroup))
            {
                CustomExtension.AlertMsg("至少要勾選一筆項目，請確認!", "");
            }


            #region -- [Check] Check Date --

            //Get input
            string mall = filter_Mall.SelectedValue;
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
            if (cntDays > 30)
            {
                CustomExtension.AlertMsg("[檢查] 日期區間不可超過 30 天", "");
                return;
            }

            #endregion


            #region -- [Data] 檢查檔建立 --

            //----- 宣告:資料參數 -----
            SZ_InvoiceRepository _data = new SZ_InvoiceRepository();
            Dictionary<string, string> search = new Dictionary<string, string>();
            string erp_sDate = sDate.ToDateString("yyyyMMdd");
            string erp_eDate = eDate.ToDateString("yyyyMMdd");

            /*
             * [OrderID]
               已勾選的項目值: C012345,C123408 -- 依「,」分組
             */
            search.Add("OrderIDs", GetDataGroup);

            //----- 取得未開票的指定資料(OrderID) -----
            var query = _data.GetBBCBillData(mall, erp_sDate, erp_eDate, search, out ErrMsg);
            if (query.Count() == 0)
            {
                CustomExtension.AlertMsg("[檢查] 查無可開票資料,請確認", "");
                return;
            }

            //將query值回傳並批次新增(寫入PKSYS發票中繼檔)
            if (!_data.CreateBaseData_BBC(erp_sDate, erp_eDate, query, out ErrMsg))
            {
                ph_Message.Visible = true;
                lt_Msg.Text = "產生資料時發生錯誤..." + ErrMsg;
                return;
            }

            #endregion


            //redirect
            Response.Redirect("{0}mySZInvoice/BBCList.aspx".FormatThis(fn_Params.WebUrl));
        }
        catch (Exception)
        {

            throw;
        }
    }



    /// <summary>
    /// 取得商城
    /// </summary>
    /// <param name="cls">類別參數</param>
    /// <param name="ddl">下拉選單object</param>
    /// <param name="rootName">第一選項顯示名稱</param>
    private void Get_ClassList(myClass cls, DropDownList ddl, string rootName)
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls).Where(fld => fld.IsStock.Equals("Y"));


        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        query = null;
    }

}