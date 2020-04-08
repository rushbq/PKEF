using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToExcel;
using PKLib_Method.Methods;
using SH_BBC.Controllers;
using SH_BBC.Models;

/*
 * [Step2]
 * - 選擇Sheet:使用LinqToExcel帶出預覽資料
 * - 上一步:重選
 * - 下一步:
 *   1.依不同的商城做不同的資料處理
 *   2.排除空白的資料列
 */

public partial class mySHBBC_StockImportStep2 : SecurityIn
{
    public string ErrMsg;

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("865", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
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
        if (string.IsNullOrEmpty(Req_DataID))
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
        SHBBCRepository _data = new SHBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();


        //----- 原始資料:條件篩選 -----
        search.Add("DataID", Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetStockImportData(search, out ErrMsg).Take(1)
            .Select(fld => new
            {
                CustID = fld.CustID,
                CustName = fld.CustName,
                FileName = fld.Upload_File,
                MallID = fld.MallID,
                MallName = fld.MallName

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        string FileName = query.FileName;

        //取得完整路徑
        string filePath = @"{0}{1}\{2}".FormatThis(
            System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"]
            , UploadFolder
            , FileName);

        lt_MallName.Text = query.MallName;
        hf_FullFileName.Value = filePath;
        hf_MallID.Value = query.MallID.ToString();

        query = null;

        //----- [元件][LinqToExcel] - 取得工作表 -----
        Set_SheetMenu(filePath);

    }


    /// <summary>
    /// 產生工作表選單
    /// </summary>
    /// <param name="filePath"></param>
    private void Set_SheetMenu(string filePath)
    {
        //查詢Excel
        var excelFile = new ExcelQueryFactory(filePath);

        //取得Excel 頁籤
        var data = excelFile.GetWorksheetNames();

        this.ddl_Sheet.Items.Clear();
        this.ddl_Sheet.Items.Add(new ListItem("選擇要匯入的工作表", ""));

        foreach (var item in data)
        {
            this.ddl_Sheet.Items.Add(new ListItem(item.ToString(), item.ToString()));
        }


    }
    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SHBBCRepository _data = new SHBBCRepository();


        #region -- 存入Table --

        //[Excel] - 取得參數
        var filePath = this.hf_FullFileName.Value;
        string sheetName = this.ddl_Sheet.SelectedValue;
        string mallID = this.hf_MallID.Value;

        //[Excel] - 取得Excel資料欄位
        var query_Xls = _data.GetExcel_DT_ECStock(filePath, sheetName, mallID);


        //建立資料篩選條件
        var baseData = new StockImportData
        {
            Data_ID = new Guid(Req_DataID),
            Update_Who = fn_Params.UserGuid
        };


        //建立Table
        if (!_data.Create_StockDataDT(baseData, query_Xls, out ErrMsg))
        {
            //[Log]
            string Msg = "單身資料建立失敗 (Step2);" + ErrMsg;

            //Show Error
            this.lt_Msg.Text = Msg;
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        #endregion



        //導至下一步
        Response.Redirect("{0}mySHBBC/StockImportStep3.aspx?dataID={1}".FormatThis(
            fn_Params.WebUrl
            , Req_DataID));

    }


    /// <summary>
    /// 選擇工作表, 產生預覽資料 - onChange
    /// </summary>
    protected void ddl_Sheet_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.ddl_Sheet.SelectedIndex > 0 && !string.IsNullOrEmpty(this.hf_FullFileName.Value))
        {
            //宣告
            StringBuilder html = new StringBuilder();
            var filePath = this.hf_FullFileName.Value;
            string sheetName = this.ddl_Sheet.SelectedValue;

            //取得資料
            SHBBCRepository _data = new SHBBCRepository();

            html = _data.GetExcel_Html(filePath, sheetName);

            //Output Html
            this.lt_tbBody.Text = html.ToString();
        }
    }
    #endregion


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
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}SHBBC/ECStock/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}