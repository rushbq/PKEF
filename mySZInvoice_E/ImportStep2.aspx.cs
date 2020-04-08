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
using SZ_Invoice_E.Controllers;
using SZ_Invoice_E.Models;

/*
 * [Step2]
 * - 選擇Sheet:使用LinqToExcel帶出預覽資料
 * - 重新上傳:刪除資料及檔案, 返回index頁
 * - 下一步:
 *   1.排除空白的資料列
 *   2.單頭:Update SheetName, Status = 11
 *   3.存入暫存Table
 *     - Check 單號是否重複匯入
 */

public partial class mySZInvoiceE_ImportStep2 : SecurityIn
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
                if (fn_CheckAuth.CheckAuth_User("1011", out ErrMsg) == false)
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
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                FileName = fld.Upload_File

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string FileName = query.FileName;

        //取得完整路徑
        string filePath = @"{0}{1}{2}\{3}".FormatThis(
            System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"]
            , UploadFolder
            , TraceID
            , FileName);

        this.lt_TraceID.Text = TraceID;
        this.hf_FullFileName.Value = filePath;
        this.hf_TraceID.Value = TraceID;

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
    /// 重新上傳:刪除資料及檔案
    /// </summary>
    protected void lbtn_ReNew_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();

        //----- 方法:刪除資料 -----
        if (false == _data.Delete(Req_DataID))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //刪除整個Folder檔案
            _ftp.FTP_DelFolder(UploadFolder + this.lt_TraceID.Text);

            //導向至首頁
            Response.Redirect("{0}mySZInvoice_E/ImportStep1.aspx".FormatThis(Application["WebUrl"]));
        }

    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();


        #region -- 存入暫存Table --

        //[Excel] - 取得參數
        var filePath = this.hf_FullFileName.Value;
        string sheetName = this.ddl_Sheet.SelectedValue;
        string traceID = this.hf_TraceID.Value;

        //[Excel] - 取得Excel資料欄位
        var query_Xls = _data.GetExcel_DT(filePath, sheetName, traceID);


        //建立基本資料參數, 用途:資料篩選條件
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID,
            Sheet_Name = sheetName,
            Update_Who = fn_Params.UserGuid
        };


        //建立暫存Table
        if (!_data.Create_Temp(baseData, query_Xls, out ErrMsg))
        {
            //[Log]
            string Msg = "暫存資料建立失敗 (Step2);" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        #endregion

        #region -- 資料Check:暫存寫入正式DT --

        if (!_data.Check_Step1(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check:資料整理時發生錯誤 (Step2);" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        #endregion

        #region -- 資料Check:訂單編號 --

        if (!_data.Check_Step2(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check:平台單號檢查時發生錯誤 (Step2);" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        #endregion


        //導至下一步
        Response.Redirect("{0}mySZInvoice_E/ImportStep3.aspx?dataID={1}".FormatThis(
            Application["WebUrl"]
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
            SZ_Invoice_ERepository _data = new SZ_Invoice_ERepository();

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
            return "{0}SZInvoice/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}