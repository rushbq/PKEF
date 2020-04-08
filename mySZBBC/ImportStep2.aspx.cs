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
using SZ_BBC.Controllers;
using SZ_BBC.Models;

/*
 * [Step2]
 * - 選擇Sheet:使用LinqToExcel帶出預覽資料
 * - 重新上傳:刪除資料及檔案, 返回index頁
 * - 下一步:
 *   1.依不同的商城做不同的資料處理
 *   2.排除品號/編號空白的資料列
 *   3.單頭:Update SheetName, Status = 11
 *   3.存入暫存Table, 取得正確的品號, Update狀態
 *     - Check 單號是否重複匯入(同一個商城)
 *     - Check 品號
 *     - Check Price
 */

public partial class mySZBBC_ImportStep2 : SecurityIn
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
                if (fn_CheckAuth.CheckAuth_User("810", out ErrMsg) == false)
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
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                FileName = fld.Upload_File,
                MallID = fld.MallID,
                MallName = fld.MallName,
                DataType = fld.Data_Type,
                DataTypeName = fld.Data_TypeName

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string CustID = query.CustID;
        string CustName = query.CustName;
        string FileName = query.FileName;

        //取得完整路徑
        string filePath = @"{0}{1}{2}\{3}".FormatThis(
            System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"]
            , UploadFolder
            , TraceID
            , FileName);

        this.lt_TraceID.Text = TraceID;
        this.lt_CustName.Text = "{0} ({1})".FormatThis(CustName, CustID);
        this.lt_MallName.Text = query.MallName;
        this.lt_TypeName.Text = query.DataTypeName;
        this.hf_FullFileName.Value = filePath;
        this.hf_MallID.Value = query.MallID.ToString();
        this.hf_Type.Value = query.DataType.ToString();
        this.hf_TraceID.Value = TraceID;

        query = null;

        //----- [元件][LinqToExcel] - 取得工作表 -----
        Set_SheetMenu(filePath);


        //退貨庫別(退貨才顯示)
        this.pal_reStock.Visible = this.hf_Type.Value.Equals("2");
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
        SZBBCRepository _data = new SZBBCRepository();

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
            Response.Redirect("{0}mySZBBC/ImportIndex.aspx".FormatThis(Application["WebUrl"]));
        }

    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        /*
         * [Step2 -> Step3]
         * /// --- 未出貨訂單(1), 退貨單(2) 流程 --- ///
         * 1. 將Excel資料匯至暫存Table (刪除此ID下的舊資料)
         * 2. Check.1=暫存檔與COPMG對應比較, 更新 ERP_ModelNo, 不正常的資料 IsPass = 'N',doWhat = '查無ERP品號', 其他IsPass = 'Y', 並Insert至 BBC_ImportData_DT
         * 3. Check.2=BBC_Import_DT 與其他不同Parent_ID的資料比對, 判斷是否有重複的OrderID + ProdID, 若有重複則 IsPass = 'N',doWhat = '重複的單號'
         * 4. Check.3=取得ERP價格, 並更新 ERP_Price, 不正常的資料 IsPass = 'N',doWhat = '查無ERP價格'
         * 5. 更新主檔狀態 BBC_ImportData.Status = 11, Sheet_Name
         * 
         * /// --- 已出貨訂單(3) 流程 --- ///
         * 1. 將Excel資料匯至暫存Table (刪除此ID下的舊資料)
         * 2. 比對BBC_Import_DT裡的OrderID, ProdID -> Update ShipmentNo
         * 3. 更新主檔狀態 BBC_ImportData.Status = 11, Sheet_Name
         * 
         * /// --- 下一步加入Excel整理頁 ---
         * 條件:京東POP(1), 天貓(2)
         */

        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();


        #region -- 存入暫存Table --

        //[Excel] - 取得參數
        var filePath = this.hf_FullFileName.Value;
        string sheetName = this.ddl_Sheet.SelectedValue;
        string mallID = this.hf_MallID.Value;
        string dataType = this.hf_Type.Value;
        string traceID = this.hf_TraceID.Value;
        string reStock = this.ddl_reStockType.SelectedValue;

        //[Excel] - 取得Excel資料欄位
        var query_Xls = _data.GetExcel_DT(filePath, sheetName, mallID, dataType, traceID, reStock);


        //建立基本資料參數, 用途:資料篩選條件
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID,
            MallID = Convert.ToInt32(mallID),
            Sheet_Name = sheetName,
            Update_Who = fn_Params.UserGuid,
            Data_Type = Convert.ToDecimal(dataType)
        };


        //建立暫存Table
        if (!_data.Create_Temp(baseData, query_Xls, out ErrMsg))
        {
            //[Log]
            string Msg = "暫存Table建立失敗(Step2)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            //Response.Write(ErrMsg);
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        #endregion

        //流程選擇
        switch (dataType)
        {
            case "1":
            case "2":
                myFlow1(baseData);

                break;

            default:
                myFlow2(baseData);

                break;
        }
    }

    /// <summary>
    /// 未出貨訂單(1), 退貨單(2) 流程
    /// </summary>
    /// <param name="baseData">基本資料參數</param>
    private void myFlow1(ImportData baseData)
    {
        /* 
         * 若為POP(1), 天貓(2)，此處不做處理，將處理機制留在Step2-1
         * 其他則繼續向下處理
         */
        if (baseData.Data_Type.Equals(1))
        {
            switch (baseData.MallID)
            {
                case 1:
                case 2:
                    //Excel整理頁
                    Response.Redirect("{0}mySZBBC/ImportStep2-1.aspx?dataID={1}".FormatThis(
                        fn_Params.WebUrl, Req_DataID));
                    break;
            }
        }


        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();


        #region -- 資料Check.1:品號 --

        if (!_data.Check_Step1(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "請檢查ERP客戶品號是否重複,<br/>POP/天貓請檢查平台「客戶商品對應」是否重複<small>Check.1:品號。單身未建立(Step2.Check_Step1)</small>";
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


        #region -- 資料Check.2:訂單編號 --

        if (!_data.Check_Step2(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.2:訂單編號。訂單編號檢查失敗, 狀態碼未更新(Step2)..." + ErrMsg;
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


        #region -- 資料Check:活動 --

        if (!_data.Check_Promo(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "Check:促銷活動。資料判斷時出了問題(Step2)..." + ErrMsg;
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


        #region -- 資料Check.3:價格 --

        if (!_data.Check_Step3(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "Check.3:價格。ERP價格檢查失敗, 狀態碼及價格未更新(Step2)..." + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            //Response.Write(ErrMsg);
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        #endregion


        //Next Page
        Response.Redirect("{0}mySZBBC/ImportStep3.aspx?dataID={1}".FormatThis(fn_Params.WebUrl, Req_DataID));

    }


    /// <summary>
    /// 已出貨訂單(3) 流程
    /// </summary>
    /// <param name="baseData">基本資料參數</param>
    private void myFlow2(ImportData baseData)
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        ////新增關聯檔_ShipmentNo:Parent_ID, OrderID, ProdID, ShipmentNo
        ////在發貨時, 未出貨訂單LEFT JOIN 關聯檔, 顯示物流單號
        //if (!_data.Create_ShipmentNo(baseData, out ErrMsg))
        //{
        //    //[Log]
        //    string Msg = "建立物流單號關聯失敗。(Step2)...\n" + ErrMsg;
        //    _data.Create_Log(baseData, Msg, out ErrMsg);

        //    //Show Error
        //    this.ph_Message.Visible = true;
        //    return;
        //}
        //else
        //{
        //    this.ph_Message.Visible = false;
        //}


        //匯入完成, 更新狀態
        if (!_data.Update_Status(Req_DataID, out ErrMsg))
        {
            //導至完成頁
            Response.Redirect("{0}mySZBBC/ImportStep5.aspx?dataID={1}&st=500".FormatThis(
                Application["WebUrl"]
                , Req_DataID));
        }
        else
        {
            //清空暫存
            _data.Delete_Temp(Req_DataID);

            //導至完成頁
            Response.Redirect("{0}mySZBBC/ImportStep5.aspx?dataID={1}&st=200".FormatThis(
                Application["WebUrl"]
                , Req_DataID));
        }
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
            SZBBCRepository _data = new SZBBCRepository();

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
            return "{0}SZBBC/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}