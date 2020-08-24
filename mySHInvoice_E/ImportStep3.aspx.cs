using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PKLib_Method.Methods;
using SH_Invoice_E.Controllers;
using SH_Invoice_E.Models;

/*
 * [Step3]
 * - 顯示可匯入/不可匯入的資料
 * - 上一步:Step2, 選擇Sheet
 * - 下一步:Step4, 匯入ERP
 */

public partial class mySHInvoiceE_ImportStep3 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1113", out ErrMsg) == false)
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
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SH_Invoice_ERepository _data = new SH_Invoice_ERepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;

        this.lt_TraceID.Text = TraceID;
        this.hf_TraceID.Value = TraceID;

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
        SH_Invoice_ERepository _data = new SH_Invoice_ERepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDetailList(Req_DataID);

        //----- 資料整理:可匯入的資料 -----
        var data_Y = query.Where(f => f.IsPass.Equals("Y"));

        //----- 資料整理:繫結 ----- 
        this.lvDataList_Y.DataSource = data_Y;
        this.lvDataList_Y.DataBind();

        //----- 判斷可匯入資料筆數 -----
        if (data_Y.Count() == 0)
        {
            lbtn_Next.Visible = false;
        }


        //----- 資料整理:不可匯入的資料 -----
        var data_N = query.Where(f => f.IsPass.Equals("N"));

        //----- 資料整理:繫結 ----- 
        this.lvDataList_N.DataSource = data_N;
        this.lvDataList_N.DataBind();


        query = null;
    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SH_Invoice_ERepository _data = new SH_Invoice_ERepository();

        //取得參數
        string traceID = this.hf_TraceID.Value;

        //建立基本資料參數
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID,
            Update_Who = fn_Params.UserGuid
        };

        //更新ERP發票號碼
        if (!_data.UpdateInvNo(baseData.Data_ID.ToString(), out ErrMsg))
        {
            //[Log]
            string Msg = "ERP發票號碼更新失敗 (Step3);" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_Message.Visible = true;
            return;
        }

        //更新主檔狀態及匯入時間
        if (!_data.Update_Status(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "ERP發票號碼已更新，但此筆狀態更新失敗 (Step3)，請聯絡系統管理員!;" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            this.ph_Message.Visible = true;
            return;
        }

        //導至下一步
        Response.Redirect("{0}mySHInvoice_E/ImportStep4.aspx?dataID={1}&st=200".FormatThis(
            Application["WebUrl"]
            , Req_DataID));

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