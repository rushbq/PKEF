using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Invoice.SH.Controllers;
using PKLib_Method.Methods;

public partial class myInvoice_Extend_FeeExpStep2 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1115", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //check null
                if (string.IsNullOrWhiteSpace(Req_DataID))
                {
                    Response.Redirect(prevPage);
                    return;
                }

                //Get data
                LookupData_Detail();
            }
            catch (Exception)
            {

                throw;
            }

        }
    }


    #region -- 資料讀取 --


    /// <summary>
    /// 單身資料
    /// </summary>
    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.Get_FeeData_DT(Req_DataID, out ErrMsg);

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
    /// 匯出
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        //Declare
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            //取資料
            var query = _data.Get_FeeData_DT(Req_DataID, out ErrMsg);

            DataTable myDT = query;

            #region ** 填入指定欄位 **

            Dictionary<string, string> _col = new Dictionary<string, string>();
            _col.Add("PayWho", "付款人名称");
            _col.Add("PayAcc1", "付款人帐号");
            _col.Add("PayAcc2", "付费账户");
            _col.Add("cn_AccName", "收款人名称");
            _col.Add("cn_BankType", "收款人开户行");
            _col.Add("cn_BankName", "开户行名称");
            _col.Add("cn_BankID", "接收行");
            _col.Add("cn_Account", "收款人账号");
            _col.Add("PayPrice", "金额");
            _col.Add("setPayDate", "指定付款日期");
            _col.Add("Priority", "处理优先级");
            _col.Add("toUse", "用途");

            //將指定的欄位,轉成陣列
            string[] selectedColumns = _col.Keys.ToArray();

            //資料複製到新的Table(內容為指定的欄位資料)
            DataTable newDT = new DataView(myDT).ToTable(true, selectedColumns);

            #endregion


            #region ** 重新命名欄位,顯示為中文 **

            foreach (var item in _col)
            {
                string _id = item.Key;
                string _name = item.Value;

                newDT.Columns[_id].ColumnName = _name;

            }
            #endregion


            //匯出Excel
            CustomExtension.ExportExcel(
                newDT
                , "ExcelData-Pay-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
                , false);

        }
        catch (Exception ex)
        {
            //Show Error
            string msg = "匯出時發生錯誤;" + ex.Message.ToString();
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
        //----- 宣告:資料參數 -----
        InvoiceRepository _data = new InvoiceRepository();

        try
        {
            if (!_data.Delete_FeeData(Req_DataID))
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
            return "{0}/FeeExpStep2.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
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
            return "{0}/FeeExpStep1.aspx".FormatThis(FuncPath());
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

    #endregion


}