using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Invoice.Controllers;
using Invoice.Models;
using PKLib_Method.Methods;

public partial class myInvoice_Extend_InvNoStep2 : SecurityIn
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
                CustomExtension.AlertMsg("查無資料", prevPage);
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
            hf_CompID.Value = _compID;
            hf_DataID.Value = _dataID;


            //載入單身資料
            LookupData_Detail(_compID, _custID, _sDate.ToDateString("yyyyMMdd"), _eDate.ToDateString("yyyyMMdd"));

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
    /// 單身資料 - ERP未開票資料
    /// </summary>
    /// <param name="compID">SH/SZ</param>
    /// <param name="custID">客編</param>
    /// <param name="startDate">erp日期格式(yyyyMMdd)</param>
    /// <param name="endDate">erp日期格式(yyyyMMdd)</param>
    private void LookupData_Detail(string compID, string custID, string startDate, string endDate)
    {
        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.GetErpUnBilledData(compID, custID, startDate, endDate, out ErrMsg);

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


    #region -- 按鈕事件 --
    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        //Params
        string _guid = hf_DataID.Value;
        string _compID = hf_CompID.Value;

        //取得已勾選
        string cbxGroup = tb_CbxValues.Text;

        //將已勾選單號加入集合
        List<InvData_DT> dataList = new List<InvData_DT>();

        string[] aryGP = Regex.Split(cbxGroup, @"\,{1}");
        foreach (string item in aryGP)
        {
            //加入項目
            var data = new InvData_DT
                {
                    Erp_AR_ID = item
                };

            //將項目加入至集合
            dataList.Add(data);
        }

        //寫入單身資料檔
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //[Job1] 開始寫入單身
            if (!_data.CreateDetail_Inv2ERP(_guid, dataList.AsQueryable(), out ErrMsg))
            {
                CustomExtension.AlertMsg("資料處理失敗-新增暫存, 請通知資訊人員.", "");
                return;
            }


            //[Job2] ERP資料回寫
            if (!_data.Update_InvData(_guid, _compID, out ErrMsg))
            {
                CustomExtension.AlertMsg("資料處理失敗-ERP回寫, 請通知資訊人員.", "");
                return;
            }


            //[Job3] 變更狀態
            if (!_data.Update_InvDataStatus(_guid, out ErrMsg))
            {
                CustomExtension.AlertMsg("資料處理失敗-狀態變更, 請通知資訊人員.", thisPage);
                return;
            }


            //OK
            Response.Redirect("{0}/InvNoStep3.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), _guid, _compID));

        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "資料處理時發生錯誤;" + ex.Message.ToString();
            ph_ErrMessage.Visible = true;
            lt_ShowMsg.Text = msg;
            return;
        }

        finally
        {
            _data = null;
        }

    }

    /// <summary>
    /// 刪除本次轉入
    /// </summary>
    protected void btn_Back_Click(object sender, EventArgs e)
    {
        //Params
        string _guid = hf_DataID.Value;

        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            if (!_data.Delete_InvData(_guid))
            {
                CustomExtension.AlertMsg("資料處理失敗-刪除暫存, 請通知資訊人員.", "");
                return;
            }

            //返回上一步
            Response.Redirect(prevPage);

        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "刪除暫存時發生錯誤;" + ex.Message.ToString();
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
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}/InvNoStep2.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), Req_DataID, Req_DBS);
        }
        set
        {
            this._thisPage = value;
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
            return "{0}/InvNoStep1.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._prevPage = value;
        }
    }


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