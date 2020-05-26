using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Invoice.Controllers;
using Invoice.Models;
using PKLib_Method.Methods;

public partial class myInvoice_Extend_InvNoStep3 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷]
                string authCode = Req_DBS.Equals("SH") ? "1109" : "1009";
                if (fn_CheckAuth.CheckAuth_User(authCode, out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //讀取資料
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
        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.GetInvData_Base(Req_DataID, out ErrMsg);
            if (query == null)
            {
                CustomExtension.AlertMsg("查無資料", "");
                return;
            }

            var data = query.Take(1).FirstOrDefault();


            //----- 資料整理:填入資料 -----
            string _dataID = data.Data_ID.ToString();
            string _compID = data.CompID;
            string _custID = data.CustID;
            string _custName = data.CustName;
            string _sDate = data.erp_sDate;
            string _eDate = data.erp_eDate;
            string _invDate = data.InvDate;
            string _invNo = data.InvNo;


            //填入表單欄位
            lb_CompID.Text = _compID.Equals("SH") ? "上海" : "深圳";
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_sDate.Text = _sDate;
            lb_eDate.Text = _eDate;
            lb_InvDate.Text = _invDate;
            lb_InvNo.Text = _invNo;


            //載入單身資料
            LookupData_Detail(_dataID);

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
    /// 單身資料 - 已轉入完成之單號
    /// </summary>
    /// <param name="id">id</param>
    private void LookupData_Detail(string id)
    {
        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.GetInvData_DT(id, out ErrMsg);

            //----- 資料整理:繫結 ----- 
            lvDataList.DataSource = query;
            lvDataList.DataBind();

            //Show Error
            if (!string.IsNullOrWhiteSpace(ErrMsg))
            {
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = ErrMsg;
                return;
            }


        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "載入單身資料時發生錯誤;" + ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            lt_ShowMsg.Text = msg;
            return;
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
        return "{0}myInvoice_Extend".FormatThis(fn_Params.WebUrl);
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
    /// DBS
    /// </summary>
    private string _Req_DBS;
    public string Req_DBS
    {
        get
        {
            string data = Request.QueryString["dbs"];

            return string.IsNullOrWhiteSpace(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DBS = value;
        }
    }
    #endregion


}