using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionUI;
using PKLib_Method.Methods;
using SZ_BBC.Controllers;
using SZ_BBC.Models;

/*
 * 單據匯出
 */
public partial class mySZBBC_DataExport : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("847", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //帶入預設日期
                string defSdate = DateTime.Now.AddMonths(-1).ToString().ToDateString("yyyy/MM/dd");
                string defEdate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
                this.filter_sDate.Text = defSdate;
                this.filter_eDate.Text = defEdate;
                this.so_sDate.Text = defSdate;
                this.so_eDate.Text = defEdate;


            }


        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 按鈕事件 --

    /// <summary>
    /// VC退貨單
    /// </summary>
    protected void lbtn_Export1_Click(object sender, EventArgs e)
    {
        //Get input
        string sDate = this.filter_sDate.Text;
        string eDate = this.filter_eDate.Text;

        //Check Null
        if (string.IsNullOrEmpty(sDate) || string.IsNullOrEmpty(eDate))
        {
            CustomExtension.AlertMsg("請選擇正確的日期", "");
            return;
        }

        //Convert to Date
        DateTime chksDate = Convert.ToDateTime(sDate);
        DateTime chkeDate = Convert.ToDateTime(eDate);

        //Check Date
        if (chksDate > chkeDate)
        {
            CustomExtension.AlertMsg("請選擇正確的日期區間", "");
            return;
        }

        //Check Range
        int cntDays = new TimeSpan(chkeDate.Ticks - chksDate.Ticks).Days;
        if (cntDays > 365)
        {
            CustomExtension.AlertMsg("日期區間不可超過一年", "");
            return;
        }

        //執行匯出
        doExport("1", sDate, eDate);
    }


    /// <summary>
    /// 經銷商專區
    /// </summary>
    protected void lbtn_Export2_Click(object sender, EventArgs e)
    {
        //Get input
        string sDate = this.so_sDate.Text;
        string eDate = this.so_eDate.Text;

        //Check Null
        if (string.IsNullOrEmpty(sDate) || string.IsNullOrEmpty(eDate))
        {
            CustomExtension.AlertMsg("請選擇正確的日期", "");
            return;
        }

        //Convert to Date
        DateTime chksDate = Convert.ToDateTime(sDate);
        DateTime chkeDate = Convert.ToDateTime(eDate);

        //Check Date
        if (chksDate > chkeDate)
        {
            CustomExtension.AlertMsg("請選擇正確的日期區間", "");
            return;
        }

        //Check Range
        int cntDays = new TimeSpan(chkeDate.Ticks - chksDate.Ticks).Days;
        if (cntDays > 365)
        {
            CustomExtension.AlertMsg("日期區間不可超過一年", "");
            return;
        }

        //執行匯出
        doExport("2", sDate, eDate);
    }


    private void doExport(string type, string sDate, string eDate)
    {
        //Fomat
        sDate = sDate.ToDateString("yyyyMMdd");
        eDate = eDate.ToDateString("yyyyMMdd");

        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();
        search.Add((int)mySearch.StartDate, sDate);
        search.Add((int)mySearch.EndDate, eDate);

        DataTable myDT;
        bool doExport = false;

        switch (type)
        {
            case "1":
                //VC退貨單
                //----- 原始資料:取得所有資料 -----
                var query1 = _data.GetERPRebackData(search)
                    .Select(fld => new
                    {
                        TI001 = fld.TI001,
                        TI002 = fld.TI002,
                        TI003 = fld.TI003,
                        TJ004 = fld.TJ004,
                        TJ005 = fld.TJ005,
                        TJ007 = fld.TJ007,
                        TI020 = fld.TI020,
                        TJ023 = fld.TJ023,
                        TJ052 = fld.TJ052,
                        TJ025 = fld.TJ025,
                        TJ026 = fld.TJ026
                    });

                //將IQueryable轉成DataTable
                myDT = fn_CustomUI.LINQToDataTable(query1);

                if (myDT.Rows.Count > 0)
                {
                    doExport = true;

                    //重新命名欄位標頭
                    myDT.Columns["TI001"].ColumnName = "銷退單別";
                    myDT.Columns["TI002"].ColumnName = "銷退單號";
                    myDT.Columns["TI003"].ColumnName = "銷退日";
                    myDT.Columns["TJ004"].ColumnName = "品號";
                    myDT.Columns["TJ005"].ColumnName = "品名";
                    myDT.Columns["TJ007"].ColumnName = "數量";
                    myDT.Columns["TI020"].ColumnName = "單頭備註";
                    myDT.Columns["TJ023"].ColumnName = "單身備註";
                    myDT.Columns["TJ052"].ColumnName = "銷退原因代號";
                    myDT.Columns["TJ025"].ColumnName = "結帳單別";
                    myDT.Columns["TJ026"].ColumnName = "結帳單號";
                }

                //release
                query1 = null;


                break;


            default:
                //----- 原始資料:取得所有資料 -----
                var query2 = _data.GetERPDataByDealer(search)
                   .Select(fld => new
                   {
                       TC001 = fld.TC001,
                       TC002 = fld.TC002,
                       TC003 = fld.TC003,
                       TD004 = fld.TD004,
                       TD005 = fld.TD005,
                       TD008 = fld.TD008,
                       TH001 = fld.TH001,
                       TH002 = fld.TH002,
                       TH007 = fld.TH007,
                       TH008 = fld.TH008
                   });

                //將IQueryable轉成DataTable
                myDT = fn_CustomUI.LINQToDataTable(query2);
               
                if (myDT.Rows.Count > 0)
                {
                    doExport = true;

                    //重新命名欄位標頭
                    myDT.Columns["TC001"].ColumnName = "訂單單別";
                    myDT.Columns["TC002"].ColumnName = "訂單單號";
                    myDT.Columns["TC003"].ColumnName = "訂單日";
                    myDT.Columns["TD004"].ColumnName = "品號";
                    myDT.Columns["TD005"].ColumnName = "品名";
                    myDT.Columns["TD008"].ColumnName = "訂單數量";
                    myDT.Columns["TH001"].ColumnName = "銷貨單別";
                    myDT.Columns["TH002"].ColumnName = "銷貨單號";
                    myDT.Columns["TH007"].ColumnName = "庫別";
                    myDT.Columns["TH008"].ColumnName = "銷貨數量";
                }

                //release
                query2 = null;


                break;
        }

        if (!doExport)
        {
            CustomExtension.AlertMsg("目前條件查無資料", "");
            return;
        }

        //匯出Excel
        fn_CustomUI.ExportExcel(
            myDT
            , "DataOutput-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
            , false);
        }

    #endregion

}