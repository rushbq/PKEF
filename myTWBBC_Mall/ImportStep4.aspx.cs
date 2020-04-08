using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PKLib_Method.Methods;
using TWBBC_Mall.Controllers;
using TWBBC_Mall.Models;


public partial class myTWBBC_Mall_ImportStep4 : SecurityIn
{
    public string ErrMsg;

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
            hf_TraceID.Value = _traceID;
            hf_MallID.Value = _mallID;
            lb_TraceID.Text = _traceID;
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_Mall.Text = _mallName;


            //載入單身資料
            LookupData_Detail();

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
    /// 取得單身資料
    /// </summary>
    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        TWBBCMallRepository _data = new TWBBCMallRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDetailList(Req_DataID);

        //----- 資料整理:可匯入的資料 -----
        var data_Y = query.Where(f => f.IsPass.Equals("Y"));

        //----- 資料整理:繫結 ----- 
        this.lvDataList_Y.DataSource = data_Y;
        this.lvDataList_Y.DataBind();


        //----- 資料整理:不可匯入的資料 -----
        var data_N = query.Where(f => f.IsPass.Equals("N"));

        //----- 資料整理:繫結 ----- 
        this.lvDataList_N.DataSource = data_N;
        this.lvDataList_N.DataBind();


        query = null;
    }
    #endregion


    #region -- 附加功能 --

   
    #endregion


    #region -- 按鈕事件 --
    
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
            //建立基本資料參數
            var baseData = new ImportData
            {
                Data_ID = new Guid(_dataID),
                TraceID = hf_TraceID.Value,
                MallID = Convert.ToInt16(this.hf_MallID.Value)
            };

            //建立EDI
            if (!_data.Create_EDI(baseData, out ErrMsg))
            {
                //[Log]
                string Msg = "EDI匯入失敗(Step4)...\n" + ErrMsg;
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

            //匯入完成, 更新狀態
            if (!_data.Update_Status(_dataID, 13, out ErrMsg))
            {
                //[Log]
                string Msg = "狀態更新失敗(Step4)..." + ErrMsg;
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

            //清空暫存
            _data.Delete_Temp(_dataID);

            //前往下一步
            Response.Redirect(nextPage);

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
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}/ImportStep4.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
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
            return "{0}/ImportStep5.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
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