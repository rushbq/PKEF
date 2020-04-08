using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PKLib_Method.Methods;
using TWBBC_Mall.Controllers;


public partial class myTWBBC_Mall_ImportStep5 : SecurityIn
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

            //填入表單欄位
            lb_TraceID.Text = _traceID;
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_Mall.Text = _mallName;

        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "載入單頭資料時發生錯誤;" + ex.Message.ToString();
            lt_ShowMsg.Text = msg;
            ph_ErrMessage.Visible = true;
            return;
        }
        finally
        {
            //release
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