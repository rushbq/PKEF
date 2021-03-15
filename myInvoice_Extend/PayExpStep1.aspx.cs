using System;
using System.Web;
using Invoice.Controllers;
using Invoice.Models;
using PKLib_Method.Methods;

public partial class myInvoice_Extend_PayExpStep1 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1114", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //帶入預設日期
                string defEdate = DateTime.Now.AddDays(-1).ToString().ToDateString("yyyy/MM/dd");
                this.filter_sDate.Text = defEdate;
                this.filter_eDate.Text = defEdate;

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
    #region -- 資料讀取 --

    /// <summary>
    /// ERP付款單
    /// </summary>
    /// <param name="startDate">erp日期格式(yyyyMMdd)</param>
    /// <param name="endDate">erp日期格式(yyyyMMdd)</param>
    private void LookupData_Detail(string startDate, string endDate)
    {
        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.Get_PaymentData(startDate.ToDateString("yyyyMMdd"), endDate.ToDateString("yyyyMMdd")
                , null, out ErrMsg);

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

            //next button
            ph_Next.Visible = true;
            ph_ErrMessage.Visible = false;
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
    /// 取付款單資料
    /// </summary>
    protected void btn_GetData_Click(object sender, EventArgs e)
    {
        string errTxt = "";
        string _sdate = filter_sDate.Text;
        string _edate = filter_eDate.Text;

        //必填檢查
        if (string.IsNullOrWhiteSpace(_sdate) || string.IsNullOrWhiteSpace(_edate))
        {
            errTxt += "===請檢查以下欄位===\\n";
            errTxt += "付款日起訖\\n";
        }
        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        //取得資料
        LookupData_Detail(_sdate, _edate);

    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        string errTxt = "";
        string _sdate = filter_sDate.Text;
        string _edate = filter_eDate.Text;
        string _payAcc1 = tb_PayAcc1.Text;
        string _payAcc2 = tb_PayAcc2.Text;
        string _payWho = tb_PayWho.Text;
        //取得已勾選
        string cbxGroup = tb_CbxValues.Text;
        string _traceID = NewTraceID();

        //必填檢查
        if (string.IsNullOrWhiteSpace(_sdate) || string.IsNullOrWhiteSpace(_edate)
             || string.IsNullOrWhiteSpace(_payAcc1) || string.IsNullOrWhiteSpace(_payWho))
        {
            errTxt += "===請檢查以下欄位===\\n";
            errTxt += "付款日\\n";
            errTxt += "付款人名称\\n";
            errTxt += "付款人帐号\\n";
        }
        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();
        if(!_data.Create_PaymentData(_traceID, _payWho, _payAcc1, _payAcc2, cbxGroup, out ErrMsg))
        {
            lt_ShowMsg.Text = ErrMsg;
            ph_ErrMessage.Visible = true;
            return;
        }
        //導至下一步
        Response.Redirect("{0}/PayExpStep2.aspx?id={1}".FormatThis(FuncPath(), _traceID));
       
    }

    #endregion


    #region -- 資料編輯 Start --


    /// <summary>
    /// New TraceID
    /// </summary>
    /// <returns></returns>
    private string NewTraceID()
    {
        //產生TraceID
        long ts = Cryptograph.GetCurrentTime();

        Random rnd = new Random();
        int myRnd = rnd.Next(1, 99);

        return "{0}{1}".FormatThis(ts, myRnd);
    }
    #endregion -- 資料編輯 End --



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
            return "{0}/PayExpStep1.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }

    #endregion




}