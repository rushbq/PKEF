using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SH_BBC.Controllers;
using SH_BBC.Models;

/*
 * [Step3]
 * - 顯示可匯入/不可匯入的資料
 * - 上一步:Step2, 選擇Sheet
 * - 下一步:Step4, EDI匯入
 * [計算訂單差額]
 * 京東POP、天貓才有W001, 京東VC沒有
 * 京東POP(W) = (貨款金額+運費) - ERP總金額
 * 天貓(W) = 買家實際支付金額 - ERP總金額
 * W > 0 : 分攤至該筆訂單的第一筆價格 
 * (X=差額/購買數量,  Y= X+ERP單價,  Y=>Update ERP單價)
 * W < 0 : 折讓, 新增W001
 * 京東VC = 每個品項直接取得ERP價格
 */

public partial class mySHBBC_ImportStep3 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("861", out ErrMsg) == false)
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
        SHBBCRepository _data = new SHBBCRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                MallID = fld.MallID,
                MallName = fld.MallName,
                DataType = fld.Data_Type,
                DataTypeName = fld.Data_TypeName

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string CustID = query.CustID;
        string CustName = query.CustName;

        this.lt_TraceID.Text = TraceID;
        this.lt_CustName.Text = "{0} ({1})".FormatThis(CustName, CustID);
        this.lt_MallName.Text = query.MallName;
        this.lt_TypeName.Text = query.DataTypeName;
        this.hf_TraceID.Value = TraceID;
        this.hf_MallID.Value = query.MallID.ToString();
        this.hf_Type.Value = query.DataType.ToString();

        /* 
         * 若為POP(1), 天貓(2):按鈕Step2-1,Excel整理頁
         */
        if (query.DataType.Equals(1))
        {
            switch (query.MallID)
            {
                case 1:
                case 2:
                    //上一步按鈕:Excel整理頁
                    ph_step.Visible = true;
                    break;
            }
        }

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
        var query = _data.GetDetailList(Req_DataID);

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

    protected void lvDataList_Y_ItemDataBound(object sender, System.Web.UI.WebControls.ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //取得資料
            int _buyCnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "BuyCnt"));
            int _MOQ = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "inMOQ"));

            //取得控制項
            PlaceHolder ph_check = (PlaceHolder)e.Item.FindControl("ph_check");
            ph_check.Visible = _buyCnt < _MOQ;

        }
    }
    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SHBBCRepository _data = new SHBBCRepository();

        //取得參數
        string mallID = this.hf_MallID.Value;
        string dataType = this.hf_Type.Value;
        string traceID = this.hf_TraceID.Value;

        //建立基本資料參數
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = traceID,
            MallID = Convert.ToInt32(mallID)
        };


        #region --- 判斷庫存, 若同單號內有一筆庫存不足, 則轉訂單(2) ---

        //宣告判斷式:是否執行庫存判斷
        bool checkStock = false;

        //對象：通用(999)
        switch (mallID)
        {
            case "999":
                checkStock = true;

                break;


            default:
                //其他 不執行
                checkStock = false;

                break;
        }

        if (checkStock)
        {
            if (!_data.Update_NewStockType(Req_DataID, out ErrMsg))
            {
                //[Log]
                string Msg = "執行庫存判斷失敗(Step3)..." + ErrMsg;
                _data.Create_Log(baseData, Msg, out ErrMsg);

                //Show Error
                //Response.Write(ErrMsg);
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                this.ph_Message.Visible = false;
            }
        }

        #endregion


        #region --- 差額計算, 更新價格(W001) ---

        //宣告判斷式:是否執行計算
        bool checkPrice = false;

        //退貨單不執行計算(匯入類型=2)
        if (dataType.Equals("1"))
        {
            checkPrice = true;
        }

        //商城判斷
        //1:京東POP / 2:天貓 / 3:京東VC / 4:eService / 5:唯品會 / 6:VC工業品 / 999:通用版
        switch (mallID)
        {
            case "1":
            case "2":
            case "5":
                checkPrice = true;

                break;


            default:
                //其他 不執行差額計算
                checkPrice = false;

                break;
        }

        //執行差額計算
        if (checkPrice)
        {
            if (!_data.Update_NewPrice(Req_DataID, out ErrMsg))
            {
                //[Log]
                string Msg = "執行差額計算失敗(Step3)..." + ErrMsg;
                _data.Create_Log(baseData, Msg, out ErrMsg);

                //Show Error
                //Response.Write(ErrMsg);
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                this.ph_Message.Visible = false;
            }
        }

        #endregion


        //更新不足欄位(訂購人/地址/電話)
        if (!_data.Update_Info(Req_DataID, out ErrMsg))
        {
            //[Log]
            string Msg = "更新訂購人資料失敗(Step3)..." + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            //Response.Write(ErrMsg);
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }

        //導至下一步
        Response.Redirect("{0}mySHBBC/ImportStep4.aspx?dataID={1}".FormatThis(
            fn_Params.WebUrl
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