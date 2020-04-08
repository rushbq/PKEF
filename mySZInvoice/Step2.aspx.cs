using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_Invoice.Aisino.Controllers;
using SZ_Invoice.Aisino.Models;

public partial class mySZInvoice_Step2 : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1010", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"]
                        , HttpUtility.UrlEncode(ErrMsg))
                        , true);
                    return;
                }


                //判斷參數是否為空
                Check_Params();

                //取得資料
                LookupData();

                //預覽資料
                LookupImportData();

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
        SZ_InvoiceRepository _data = new SZ_InvoiceRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var _getData = _data.GetDataList(search);

        if (_getData.Count() == 0)
        {
            this.ph_Content.Visible = false;
            this.ph_Buttons.Visible = false;
            CustomExtension.AlertMsg("查無資料,請重新確認!", "");
            return;
        }

        //----- 資料整理 -----
        var query = _getData.Take(1)
            .Select(fld => new
            {
                Inv_UID = fld.Inv_UID,
                IsInsert = fld.IsInsert,
                InvType = fld.InvType,
                CustID = fld.CustID,
                CustName = fld.CustName,
                BuyerName = fld.BuyerName,
                DataType = fld.DataType

            }).FirstOrDefault();

        //Check
        if (query.IsInsert.Equals("Y"))
        {
            CustomExtension.AlertMsg("重複轉入,請重新確認", "{0}mySZInvoice/List.aspx".FormatThis(Application["WebUrl"]));
            return;
        }

        //----- 資料整理:填入資料 -----
        string TraceID = query.Inv_UID;
        string CustName = "{1}&nbsp;({0})".FormatThis(query.CustID, query.CustName);
        string InvType = query.InvType;
        string dataType = query.DataType.ToString();

        this.lt_Inv_UID.Text = TraceID;
        this.lt_CustName.Text = CustName;
        this.lt_InvType.Text = "{0} {1}".FormatThis(
            getInvType(InvType)
            , InvType.Equals("x") ? "<br/>(發票類型未設定, 若不設定轉入時將預設為「專票」)" : ""
            );
        lt_vendeename.Text = query.BuyerName;
        lbtn_ReNew.Visible = !dataType.Equals("2");
        ph_attMsg.Visible = !dataType.Equals("2");

        //開票類型=1:一般/2:電商
        string _dataType = query.DataType.ToString();
        hf_DataType.Value = _dataType;
             

        query = null;

    }


    /// <summary>
    /// 預覽資料
    /// </summary>
    private void LookupImportData()
    {
        //----- 宣告:資料參數 -----
        SZ_InvoiceRepository _data = new SZ_InvoiceRepository();

        //----- 原始資料:取得所有資料 -----
        var _getData = _data.GetDataLines(Req_DataID, "Steps");
        if (_getData == null)
        {
            this.ph_Content.Visible = false;
            this.ph_Buttons.Visible = false;
            CustomExtension.AlertMsg("查無資料,請重新確認!", "");
            return;
        }

        //----- 資料繫結 -----
        this.lvDataList.DataSource = _getData;
        this.lvDataList.DataBind();

    }


    /// <summary>
    /// 取得發票類型名稱
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string getInvType(string type)
    {
        switch (type)
        {
            case "0":
                return "專票";

            case "2":
                return "普票";

            default:
                return "<a href=\"{0}CustInfo/Cust_Search.aspx?t=2\">尚未設定, 點此前往設定</a>".FormatThis(Application["WebUrl"]);
        }
    }


    public string getVendTax(string tax)
    {
        if (string.IsNullOrEmpty(tax))
        {
            return "稅號空白，請至ERP客戶資料填寫「銀行帳號(一)」";
        }
        else
        {
            return tax;
        }
    }

    #endregion


    #region -- 資料編輯 --

    protected void lbtn_ReNew_Click(object sender, EventArgs e)
    {
        try
        {
            //----- 宣告:資料參數 -----
            SZ_InvoiceRepository _data = new SZ_InvoiceRepository();

            //----- 資料處理 -----
            bool isOK = _data.Delete(Req_DataID);
            if (!isOK)
            {
                CustomExtension.AlertMsg("資料處理失敗,請聯絡管理員", PageUrl);
                return;
            }
            else
            {
                Response.Redirect("{0}mySZInvoice/Step1.aspx".FormatThis(Application["WebUrl"]));
            }


        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btn_Next_Click(object sender, EventArgs e)
    {
        try
        {
            //----- 宣告:資料參數 -----
            SZ_InvoiceRepository _data = new SZ_InvoiceRepository();

            //----- 資料處理 -----
            bool isOK = _data.CreateImportData(Req_DataID, fn_Params.UserGuid, hf_DataType.Value, out ErrMsg);
            if (!isOK)
            {
                this.ph_Message.Visible = true;
                this.lt_ErrMsg.Text = ErrMsg;
                //CustomExtension.AlertMsg("資料處理失敗,請聯絡管理員", PageUrl);
                return;
            }
            else
            {
                Response.Redirect("{0}mySZInvoice/Step3.aspx?dataID={1}&type={2}".FormatThis(Application["WebUrl"], Req_DataID, hf_DataType.Value));
            }
        }
        catch (Exception)
        {

            throw;
        }
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
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}mySZInvoice/Step2.aspx?dataID={1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;

    #endregion

}