using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_Invoice_E.Controllers;
using SZ_Invoice_E.Models;


public partial class mySZInvoiceE_ImportLog : SecurityIn
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
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷編號是否為空
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Message.Visible = true;
                    this.ph_Data.Visible = false;

                    return;
                }

                this.ph_Message.Visible = false;
                this.ph_Data.Visible = true;

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
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetDataList(search).Take(1);


        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();


        //載入其他明細資料
        if (query.Count() > 0)
        {
            string traceID = query.FirstOrDefault().TraceID;

            //匯入錯誤記錄
            LookupData_Log();

            //ERP 結帳單
            LookupData_ErpData();

        }

        //release
        query = null;
    }


    /// <summary>
    /// 匯入Log
    /// </summary>
    private void LookupData_Log()
    {
        //----- 宣告:資料參數 -----
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetLogList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_LogList.DataSource = query;
        this.lv_LogList.DataBind();


        //release
        query = null;

    }


    /// <summary>
    /// ERP 結帳單
    /// </summary>
    private void LookupData_ErpData()
    {
        //----- 宣告:資料參數 -----
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //----- 原始資料:取得資料 -----
        var query = _data.GetERPData(search);

        //----- 資料整理:繫結 ----- 
        this.lv_ErpData.DataSource = query;
        this.lv_ErpData.DataBind();


        //release
        query = null;

    }

    #endregion


    protected void btn_Export_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //----- 方法:取得資料 -----
        var query = _data.GetERPData(search);
        if (query.Count() == 0)
        {
            CustomExtension.AlertMsg("查無資料,請重新確認條件.", "");
            return;
        }

        /* [開始匯出] */
        var _newDT = query
            .Select(fld => new
            {
                OrderID = fld.OrderID,
                InvoiceNo = fld.InvoiceNo,
                InvoiceDate = fld.InvoiceDate,
                InvoicePrice = fld.InvPrice,
                Erp_AR_ID = fld.ErpID,
                Erp_SO_ID = fld.ErpSOID,
                ErpPrice = fld.ErpPrice
            });

        //將IQueryable轉成DataTable
        DataTable myDT = CustomExtension.LINQToDataTable(_newDT);

        if (myDT.Rows.Count > 0)
        {
            //重新命名欄位標頭
            myDT.Columns["OrderID"].ColumnName = "平台單號";
            myDT.Columns["InvoiceNo"].ColumnName = "發票號";
            myDT.Columns["InvoiceDate"].ColumnName = "發票日";
            myDT.Columns["InvoicePrice"].ColumnName = "發票金額";
            myDT.Columns["Erp_AR_ID"].ColumnName = "結帳單號";
            myDT.Columns["Erp_SO_ID"].ColumnName = "銷貨單號";
            myDT.Columns["ErpPrice"].ColumnName = "結帳單金額";

        }

        //release
        query = null;

        //匯出Excel
        CustomExtension.ExportExcel(
            myDT
            , "ErpData-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
            , false);
    }


    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            string data = Request.QueryString["dataID"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("EF_SZInvoice");

            return string.IsNullOrWhiteSpace(tempUrl) ? "{0}mySZInvoice_E/ImportList.aspx".FormatThis(fn_Params.WebUrl) : Server.UrlDecode(tempUrl);          
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    private string _Page_SearchUrl;



    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}SZInvoice/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["Ftp_RefUrl"] + System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}