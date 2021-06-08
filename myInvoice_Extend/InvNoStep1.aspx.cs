using System;
using System.Web;
using Invoice.SH.Controllers;
using Invoice.Models;
using PKLib_Method.Methods;

public partial class myInvoice_Extend_InvNoStep1 : SecurityIn
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

                //帶入預設日期
                string defEdate = DateTime.Now.AddDays(-1).ToString().ToDateString("yyyy/MM/dd");
                this.filter_sDate.Text = defEdate;
                this.filter_eDate.Text = defEdate;

                //帶預設值
                if (!string.IsNullOrWhiteSpace(Req_DBS))
                {
                    ddl_DBS.SelectedValue = Req_DBS;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }

    #region -- 按鈕事件 --
    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        string errTxt = "";
        string _dbs = ddl_DBS.SelectedValue;
        string _cust = val_Cust.Text;
        string _sdate = filter_sDate.Text;
        string _edate = filter_eDate.Text;
        string _invDate = tb_InvDate.Text;
        string _invNo = tb_InvNo.Text;

        //必填檢查
        if (string.IsNullOrWhiteSpace(_dbs) || string.IsNullOrWhiteSpace(_cust)
             || string.IsNullOrWhiteSpace(_sdate) || string.IsNullOrWhiteSpace(_edate)
             || string.IsNullOrWhiteSpace(_invDate) || string.IsNullOrWhiteSpace(_invNo))
        {
            errTxt += "===請檢查以下欄位===\\n";
            errTxt += "公司別\\n";
            errTxt += "客戶\\n";
            errTxt += "結帳日\\n";
            errTxt += "發票日期\\n";
            errTxt += "發票號碼\\n";
        }
        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }


        //資料處理
        string[] myData = Add_Data();

        //取得回傳參數
        string DataID = myData[0];
        string ProcCode = myData[1];
        string Message = myData[2];

        //判斷是否處理成功
        if (!ProcCode.Equals("200"))
        {
            lt_ShowMsg.Text = Message;
            ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //導至下一步
            Response.Redirect("{0}/InvNoStep2.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), DataID, _dbs));
            return;
        }
    }

    #endregion


    #region -- 資料編輯 Start --

    /// <summary>
    /// 資料新增
    /// </summary>
    /// <returns></returns>
    private string[] Add_Data()
    {
        //回傳參數初始化
        string DataID = "";
        string ProcCode = "0";
        string Message = "";

        //取得欄位資料
        string _dbs = ddl_DBS.SelectedValue;
        string _cust = val_Cust.Text;
        string _sdate = filter_sDate.Text;
        string _edate = filter_eDate.Text;
        string _invDate = tb_InvDate.Text;
        string _invNo = tb_InvNo.Text;


        #region -- 資料處理 --

        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        //產生Guid
        string guid = CustomExtension.GetGuid();

        //----- 設定:資料欄位 -----
        var data = new InvData_Base
        {
            Data_ID = new Guid(guid),
            CompID = _dbs,
            CustID = _cust,
            erp_sDate = _sdate,
            erp_eDate = _edate,
            InvDate = _invDate,
            InvNo = _invNo,
            Create_Who = fn_Params.UserGuid
        };

        //----- 方法:建立資料 -----      
        if (!_data.Create_Inv2ERP(data, out ErrMsg))
        {
            //顯示錯誤
            Message = "資料建立失敗<br/>({0})".FormatThis(ErrMsg);
            return new string[] { DataID, ProcCode, Message };
        }
        else
        {
            DataID = guid;
            ProcCode = "200";
            return new string[] { DataID, ProcCode, Message };
        }


        #endregion
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
            return "{0}/InvNoStep1.aspx?dbs={1}".FormatThis(FuncPath(), Req_DBS);
        }
        set
        {
            this._thisPage = value;
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