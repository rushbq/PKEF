using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqToExcel;
using PKLib_Method.Methods;
using TWBBC_Mall.Controllers;
using TWBBC_Mall.Models;

/// <summary>
/// 下一步時的判斷
/// 使用自訂客戶品號對應的商城:Step3
/// 其他:Step4
/// </summary>
public partial class myTWBBC_Mall_ImportStep2 : SecurityIn
{
    public string ErrMsg;

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1231", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                //取得基本資料
                LookupData();

            }
            catch (Exception)
            {

                throw;
            }

        }
    }


    #region -- 資料讀取 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //Check null
        if (string.IsNullOrWhiteSpace(Req_DataID))
        {
            CustomExtension.AlertMsg("查無資料", Page_SearchUrl);
            return;
        }

        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        try
        {
            //----- 原始資料:條件篩選 -----
            search.Add((int)mySearch.DataID, Req_DataID);

            //----- 原始資料:取得所有資料 -----
            var data = _data.GetDataList(search, out ErrMsg).FirstOrDefault();
            if (data == null)
            {
                CustomExtension.AlertMsg("查無資料", Page_SearchUrl);
                return;
            }

            //----- 資料整理:填入資料 -----
            string _dataID = data.Data_ID.ToString();
            string _traceID = data.TraceID;
            string _custID = data.CustID;
            string _custName = data.CustName;
            string _mallID = data.MallID.ToString();
            string _mallName = data.MallName;
            string _fileName = data.Upload_File;
            string _fileNameShip = data.Upload_ShipFile;
            Int16 _status = data.Status;

            //release
            data = null;

            /*
             * 判斷狀態, 導至指定頁面
             */
            switch (_status)
            {
                case 13:
                case 99:
                    //完成,作廢(離開此頁)
                    Response.Redirect(Page_SearchUrl);
                    break;

                default:
                    //不採取動作
                    break;
            }


            //填入表單欄位
            hf_DataID.Value = _dataID;
            lb_TraceID.Text = _traceID;
            hf_TraceID.Value = _traceID;
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_Mall.Text = _mallName;

            /* [商城判斷] 顯示指定區域
             * 1:Costco, 
             */
            switch (_mallID)
            {
                case "1":
                    ph_ShipDetail.Visible = true;
                    break;

                default:
                    ph_ShipDetail.Visible = false;
                    break;
            }

            //設定檔案完整路徑
            string _diskurl = System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"];
            string filePath = @"{0}{1}{2}".FormatThis(
                _diskurl
                , UploadFolder(_traceID).Replace("/", "\\")
                , _fileName);
            string filePath_Ship = string.IsNullOrWhiteSpace(_fileNameShip) ? ""
                : @"{0}{1}{2}".FormatThis(
                    _diskurl
                    , UploadFolder(_traceID).Replace("/", "\\")
                    , _fileNameShip);

            //存放完整路徑
            hf_FullFileName.Value = filePath;
            hf_ShipFullFileName.Value = filePath_Ship;  //指定商城才有

            //----- [元件][LinqToExcel] - 取得工作表 -----
            Set_SheetMenu(filePath, ddl_Sheet);
            Set_SheetMenu(filePath_Ship, ddl_ShipSheet);
        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "載入單頭資料時發生錯誤;" + ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            lt_ShowMsg.Text = msg;
            return;
        }
        finally
        {
            //release
            _data = null;
        }
    }


    /// <summary>
    /// 產生工作表選單
    /// </summary>
    /// <param name="filePath"></param>
    private void Set_SheetMenu(string filePath, DropDownList ddl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            //查詢Excel
            var excelFile = new ExcelQueryFactory(filePath);

            //取得Excel 頁籤
            var data = excelFile.GetWorksheetNames();

            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("選擇要匯入的工作表", ""));

            foreach (var item in data)
            {
                ddl.Items.Add(new ListItem(item.ToString(), item.ToString()));
            }

            data = null;
        }
        catch (Exception ex)
        {
            lt_ShowMsg.Text = ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            ph_WorkBtns.Visible = false;
        }


    }

    #endregion


    #region -- 附加功能 --

    /// <summary>
    /// (訂單明細) 選擇工作表, 產生預覽資料 - onChange
    /// </summary>
    protected void ddl_Sheet_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_Sheet.SelectedIndex > 0 && !string.IsNullOrEmpty(this.hf_FullFileName.Value))
        {
            //取得參數
            string filePath = hf_FullFileName.Value;
            string sheetName = ddl_Sheet.SelectedValue;
            StringBuilder html = new StringBuilder();

            //取得資料
            TWBBCMallRepository _data = new TWBBCMallRepository();
            try
            {
                //Get Excel Data to html
                html = _data.GetExcel_Html(filePath, sheetName);

                //Output Html
                lt_DataHtml.Text = html.ToString();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _data = null;
            }

        }
    }

    /// <summary>
    /// (出貨明細) 選擇工作表, 產生預覽資料 - onChange
    /// </summary>
    protected void ddl_ShipSheet_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_ShipSheet.SelectedIndex > 0 && !string.IsNullOrEmpty(hf_ShipFullFileName.Value))
        {
            //取得參數
            string filePath = hf_ShipFullFileName.Value;
            string sheetName = ddl_ShipSheet.SelectedValue;
            StringBuilder html = new StringBuilder();

            //取得資料
            TWBBCMallRepository _data = new TWBBCMallRepository();
            try
            {
                //Get Excel Data to html
                html = _data.GetExcel_Html(filePath, sheetName);

                //Output Html
                lt_ShipHtml.Text = html.ToString();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _data = null;
            }

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
        TWBBCMallRepository _data = new TWBBCMallRepository();
        try
        {
            string id = hf_DataID.Value;
            string traceID = hf_TraceID.Value;

            //----- 方法:刪除資料 -----
            if (false == _data.Delete(id, out ErrMsg))
            {
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = "重新上傳操作失敗..." + ErrMsg;
                return;
            }
            else
            {
                //刪除整個Folder檔案
                _ftp.FTP_DelFolder(UploadFolder(traceID));

                //導向至Step1
                Response.Redirect(prevPage);
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        //宣告
        TWBBCMallRepository _data = new TWBBCMallRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();
        string _dataID = hf_DataID.Value;

        try
        {
            #region -- [Get] 取得基本資料 --

            //----- 原始資料:條件篩選 -----
            search.Add((int)mySearch.DataID, _dataID);

            //----- 原始資料:取得所有資料 -----
            var data = _data.GetDataList(search, out ErrMsg).FirstOrDefault();

            //----- 資料整理:填入資料 -----
            string _traceID = data.TraceID;
            string _mallID = data.MallID.ToString();
            string _custID = data.CustID;

            //release
            data = null;


            //----- 資料整理:取得輸入資料 -----
            string _sheet = ddl_Sheet.SelectedValue;
            string _sheetShip = ddl_ShipSheet.SelectedValue;
            string _filePath = hf_FullFileName.Value;
            string _filePath_Ship = hf_ShipFullFileName.Value;

            #endregion


            #region -- [Check] 欄位檢查 --

            //declare
            string errTxt = "";


            /* [商城判斷] 檢查工作表(出貨檔)
             * 1:Costco, 
             */
            switch (_mallID)
            {
                case "1":
                    if (string.IsNullOrWhiteSpace(_sheetShip))
                    {
                        errTxt += "[檢查] 工作表-出貨資料未選擇";
                    }
                    break;

                default:
                    break;
            }

            //檢查工作表(訂單主檔)
            if (string.IsNullOrWhiteSpace(_sheet))
            {
                errTxt += "[檢查] 工作表-訂單明細未選擇";
            }

            //show alert
            if (!string.IsNullOrWhiteSpace(errTxt))
            {
                CustomExtension.AlertMsg(errTxt, thisPage);
                return;
            }

            #endregion


            #region -- 存入暫存Table --

            //建立基本資料參數, 用途:更新基本資料, 活動判斷, log記錄
            var baseData = new ImportData
            {
                Data_ID = new Guid(_dataID),
                TraceID = _traceID,
                MallID = Convert.ToInt16(_mallID),
                Sheet_Name = _sheet,
                Sheet_ShipName = _sheetShip,
                Update_Who = fn_Params.UserGuid
            };

            //宣告資料容器
            IQueryable<RefColumn> fulldata = null;

            //[Excel] - 取得Excel資料欄位(訂單明細)
            var query_Xls = _data.GetExcel_DT(_filePath, _sheet, _mallID);


            /* [商城判斷] 檢查工作表(出貨檔)
            * 1:Costco, 
            */
            switch (_mallID)
            {
                case "1":
                    //[Excel] - 取得Excel資料欄位(出貨資料)
                    var query_Ship = _data.GetExcel_ShipDT(_filePath_Ship, _sheetShip, _mallID);

                    /* Linq Join 資料結合(明細檔+出貨檔)
                     * .Join(
                     *  參數1:要加入的資料來源 (query_Ship)
                     *  參數2:主表要join的值 (pk => pk.ShipmentNo + pk.OrderID,)
                     *  參數3:次表要join的值 (fk => fk.ShipmentNo + fk.OrderID,)
                     *  參數4:將資料集合起來 ((pk, fk))
                     *  new{ 重新命名欄位 }
                     * )
                     */
                    var dataCombine = query_Xls.Join(query_Ship,
                         pk => pk.ShipmentNo + pk.OrderID,
                         fk => fk.ShipmentNo + fk.OrderID,
                         (pk, fk) => new RefColumn
                         {
                             OrderID = pk.OrderID,
                             CustOrderID = pk.CustOrderID,
                             ProdID = pk.ProdID,
                             NickName = pk.NickName,
                             ProdName = pk.ProdName,
                             BuyCnt = pk.BuyCnt,
                             Buy_ProdName = pk.ProdName,
                             Buy_Time = pk.Buy_Time,
                             ShipWho = fk.ShipWho,
                             ShipAddr = fk.ShipAddr,
                             ShipTel = fk.ShipTel
                         });

                    //指派join的資料
                    fulldata = dataCombine;

                    break;

                default:
                    //直接取明細檔資料
                    fulldata = query_Xls;

                    break;
            }


            //建立暫存Table
            if (!_data.Create_Temp(baseData, fulldata, out ErrMsg))
            {
                //[Log]
                string Msg = "暫存Table建立失敗(Step2)..." + ErrMsg;
                _data.Create_Log(baseData, Msg, out ErrMsg);

                //Show Error
                lt_ShowMsg.Text = Msg;
                ph_ErrMessage.Visible = true;
                return;
            }
            else
            {
                ph_ErrMessage.Visible = false;
            }

            #endregion


            /* [商城判斷] 使用自訂客戶品號的商城,需導至Step3
            * EXCEL整理頁 = Response.Redirect("{0}mySZBBC/ImportStep2-1.aspx?dataID={1}".FormatThis(fn_Params.WebUrl, Req_DataID));
            * 其他直接處理資料
            */
            switch (_mallID)
            {
                default:
                    /* ----- 開始資料判斷&處理 ----- */

                    #region -- 資料Check.1:品號 --

                    if (!_data.Check_Step1(baseData, out ErrMsg))
                    {
                        //[Log]
                        string Msg = "請檢查ERP客戶品號是否重複.<small>Check.1:品號。單身未建立(Step2.Check_Step1)</small>";
                        _data.Create_Log(baseData, Msg, out ErrMsg);

                        //Show Error
                        lt_ShowMsg.Text = Msg;
                        ph_ErrMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        ph_ErrMessage.Visible = false;
                    }

                    #endregion


                    #region -- 資料Check.2:訂單編號 --

                    if (!_data.Check_Step2(_dataID, out ErrMsg))
                    {
                        //[Log]
                        string Msg = "Check.2:訂單編號。訂單編號檢查失敗, 狀態碼未更新(Step2)..." + ErrMsg;
                        _data.Create_Log(baseData, Msg, out ErrMsg);

                        //Show Error
                        lt_ShowMsg.Text = Msg;
                        ph_ErrMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        ph_ErrMessage.Visible = false;
                    }

                    #endregion


                    #region -- 資料Check:活動 --

                    if (!_data.Check_Promo(baseData, out ErrMsg))
                    {
                        //[Log]
                        string Msg = "Check:促銷活動。資料判斷時出了問題(Step2)..." + ErrMsg;
                        _data.Create_Log(baseData, Msg, out ErrMsg);

                        //Show Error
                        lt_ShowMsg.Text = Msg;
                        ph_ErrMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        ph_ErrMessage.Visible = false;
                    }

                    #endregion


                    #region -- 資料Check.3:價格 --

                    if (!_data.Check_Step3(_dataID, out ErrMsg))
                    {
                        //[Log]
                        string Msg = "Check.3:價格。ERP價格檢查失敗, 狀態碼及價格未更新(Step2)..." + ErrMsg;
                        _data.Create_Log(baseData, Msg, out ErrMsg);

                        //Show Error
                        lt_ShowMsg.Text = Msg;
                        ph_ErrMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        ph_ErrMessage.Visible = false;
                    }

                    #endregion

                    //Update Status
                    if (!_data.Update_Status(_dataID, 12, out ErrMsg))
                    {
                        //[Log]
                        string Msg = "狀態更新失敗(Step2)..." + ErrMsg;
                        _data.Create_Log(baseData, Msg, out ErrMsg);

                        //Show Error
                        ph_ErrMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        ph_ErrMessage.Visible = false;
                    }

                    /* ----- 結束資料判斷&處理 ----- */

                    //前往下一步
                    Response.Redirect(nextPage);

                    break;
            }



        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }

    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC_Mall".FormatThis(fn_Params.WebUrl);
    }


    /// <summary>
    /// 上傳目錄(+TraceID)
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder(string traceID)
    {
        return "{0}TWBBC_Mall/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceID);
    }

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 設定參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            string data = Request.QueryString["id"];

            return string.IsNullOrWhiteSpace(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 上一頁網址
    /// </summary>
    private string _prevPage;
    public string prevPage
    {
        get
        {
            return "{0}/ImportStep1.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._prevPage = value;
        }
    }

    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}/ImportStep2.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._thisPage = value;
        }
    }

    /// <summary>
    /// 下一頁網址
    /// </summary>
    private string _nextPage;
    public string nextPage
    {
        get
        {
            return "{0}/ImportStep4.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._nextPage = value;
        }
    }


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("EF_TWBBC_Mall");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }
    #endregion

}