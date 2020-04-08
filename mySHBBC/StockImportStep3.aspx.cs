using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PKLib_Method.Methods;
using SH_BBC.Controllers;
using SH_BBC.Models;

/*
 * [Step3]
 * - 顯示可匯入/不可匯入的資料
 * - 上一步:Step2, 選擇Sheet
 * - 
 */

public partial class mySHBBC_StockImportStep3 : SecurityIn
{
    public string ErrMsg;

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


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetStockImportData(search,out ErrMsg).Take(1)
            .Select(fld => new
            {
                MallID = fld.MallID,
                MallName = fld.MallName

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        this.lt_MallName.Text = query.MallName;
        this.hf_MallID.Value = query.MallID.ToString();

        query = null;


        //載入單身資料
        LookupData_Detail();

    }

    /// <summary>
    /// 取得單身資料
    /// </summary>
    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        SHBBCRepository _data = new SHBBCRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetStockImportDetail(Req_DataID);

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

    #endregion

}