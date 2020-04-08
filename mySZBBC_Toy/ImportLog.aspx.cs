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
using SZBBC_Toy.Controllers;
using SZBBC_Toy.Models;


public partial class mySZBBC_ImportLog : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("920", out ErrMsg) == false)
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
        SZBBCToyRepository _data = new SZBBCToyRepository();
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

            //EDI轉入失敗記錄
            LookupData_EdiLog(traceID);

            //ERP 訂單/銷貨單
            LookupData_ErpData();

            //ERP 借出單
            LookupData_ErpInvData(traceID);

            //ERP 銷退單
            LookupData_ErpRbData(traceID);

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
        SZBBCToyRepository _data = new SZBBCToyRepository();


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetLogList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_LogList.DataSource = query;
        this.lv_LogList.DataBind();


        //release
        query = null;

    }


    /// <summary>
    /// ERP 訂單/銷貨單
    /// </summary>
    private void LookupData_ErpData()
    {
        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();
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

    /// <summary>
    /// ERP 借出單
    /// </summary>
    private void LookupData_ErpInvData(string traceID)
    {
        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();


        //----- 原始資料:取得資料 -----
        var query = _data.GetInvData(traceID);


        //----- 資料整理:繫結 ----- 
        this.lv_ErpInvData.DataSource = query;
        this.lv_ErpInvData.DataBind();


        //release
        query = null;
    }

    /// <summary>
    /// ERP 銷退單
    /// </summary>
    private void LookupData_ErpRbData(string traceID)
    {
        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();


        //----- 原始資料:取得資料 -----
        var query = _data.GetRebackData(traceID);


        //----- 資料整理:繫結 ----- 
        this.lv_ErpRbData.DataSource = query;
        this.lv_ErpRbData.DataBind();


        //release
        query = null;
    }


    /// <summary>
    /// EDI Log
    /// </summary>
    /// <param name="traceID"></param>
    private void LookupData_EdiLog(string traceID)
    {
        //----- 宣告:資料參數 -----
        SZBBCToyRepository _data = new SZBBCToyRepository();


        //----- 原始資料:取得資料 -----
        var query = _data.GetEDILog(traceID);

        if (query != null)
        {
            //----- 資料整理:繫結 ----- 
            this.lv_EdiLog.DataSource = query;
            this.lv_EdiLog.DataBind();
        }

        //release
        query = null;
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
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("EF_SZBBC");

            return string.IsNullOrWhiteSpace(tempUrl) ? "{0}mySZBBC_Toy/ImportList.aspx".FormatThis(fn_Params.WebUrl) : Server.UrlDecode(tempUrl);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}SZBBC_Toy/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["Ftp_RefUrl"] + System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}