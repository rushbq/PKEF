using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using PKLib_Method.Methods;
using ERP_CopyData.Controllers;
using ERP_CopyData.Models;
using System.Web.UI.WebControls;

public partial class myDataInfo_ErpPriceCopy : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("415", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
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
        //取得輸入欄
        string _PrimaryID = tb_PrimaryID.Text.Trim();
        string _SubID = tb_SubID.Text.Trim();
        string _SrcCompanyID = ddl_SrcDB.SelectedValue;
        string _TarCompanyID = ddl_TarDB.SelectedValue;
        string _TarPrimaryID = ddl_TarTypeID.SelectedValue;
        string _flowType = ddl_flowType.SelectedValue;
        string _validDate = tb_validDate.Text.ToDateString("yyyyMMdd");
        string _invalidDate = tb_invalidDate.Text.ToDateString("yyyyMMdd");
        string errTxt = "";
      
        //檢查所有欄位
        if (string.IsNullOrWhiteSpace(_SrcCompanyID))
        {
            errTxt += "來源資料庫\\n";
        }
        if (string.IsNullOrWhiteSpace(_PrimaryID) || string.IsNullOrWhiteSpace(_SubID))
        {
            errTxt += "來源單別/單號\\n";
        }
        if (string.IsNullOrWhiteSpace(_TarCompanyID))
        {
            errTxt += "目標資料庫\\n";
        }
        if (string.IsNullOrWhiteSpace(_flowType))
        {
            errTxt += "核價單或報價單\\n";
        }
        if (string.IsNullOrWhiteSpace(_TarPrimaryID))
        {
            errTxt += "目標單別\\n";
        }

        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg("以下為必填欄位:\\n" + errTxt, "");
            return;
        }

        //----- 資料處理 -----
        ERP_CopyDataRepository _data = new ERP_CopyDataRepository();

        try
        {
            //check
            if (!_data.CheckPriceData(_PrimaryID, _SubID, _SrcCompanyID, _flowType, out ErrMsg))
            {
                CustomExtension.AlertMsg(ErrMsg, "");
                return;
            }

            //insert
            if (_data.CreatePriceData(_PrimaryID, _SubID, _SrcCompanyID, _TarCompanyID, _TarPrimaryID, _flowType
                , _validDate, _invalidDate, out ErrMsg))
            {
                CustomExtension.AlertMsg("複製完成", thisPage);
                return;
            }
            else
            {
                lt_ShowMsg.Text = ErrMsg;
                ph_ErrMessage.Visible = true;
                CustomExtension.AlertMsg("發生錯誤", "");
                return;
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }

    /// <summary>
    /// 目標資料庫 onchange
    /// </summary>
    protected void ddl_TarDB_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddl_flowType.SelectedValue = "0";
        ddl_TarTypeID.Items.Clear();
    }

    /// <summary>
    /// 核價單 / 報價單 onchange
    /// </summary>
    protected void ddl_flowType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Get_TypeList(ddl_TarTypeID, "請選擇單別", "");
    }

    /// <summary>
    /// 取得單別
    /// </summary>
    /// <param name="cls"></param>
    /// <param name="ddl"></param>
    /// <param name="rootName"></param>
    /// <param name="inputValue"></param>
    private void Get_TypeList(DropDownList ddl, string rootName, string inputValue)
    {
        string _srcComp = ddl_TarDB.SelectedValue;
        string _type = ddl_flowType.SelectedValue;

        if (string.IsNullOrWhiteSpace(_srcComp))
        {
            ddl_flowType.SelectedValue = "0";
            CustomExtension.AlertMsg("請選擇「目標資料庫」", "");
            return;
        }
        if (string.IsNullOrWhiteSpace(_type))
        {
            CustomExtension.AlertMsg("請選擇「核價單 或 報價單」", "");
            return;
        }

        //----- 宣告:資料參數 -----
        ERP_CopyDataRepository _data = new ERP_CopyDataRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetPriceTypeID(_srcComp, _type);

        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(
                "{0} - {1}".FormatThis(item.ID, item.Label)
                , item.ID));
        }

        //被選擇值
        if (!string.IsNullOrWhiteSpace(inputValue))
        {
            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(inputValue));
        }

        query = null;
    }

    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myDataInfo".FormatThis(fn_Params.WebUrl);
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
            return "{0}/ErpPriceCopy.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }

    #endregion


}