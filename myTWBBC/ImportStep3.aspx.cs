using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using PKLib_Method.Methods;
using TW_BBC.Controllers;
using TW_BBC.Models;

public partial class myTWBBC_ImportStep3 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1210", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }


                //取得基本資料
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
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            //----- 原始資料:條件篩選 -----
            search.Add("DataID", Req_DataID);

            //----- 原始資料:取得所有資料 -----
            var query = _data.GetOne(search, out ErrMsg).FirstOrDefault();
            if (query == null)
            {
                CustomExtension.AlertMsg("查無資料", Page_SearchUrl);
                return;
            }

            //----- 資料整理:填入資料 -----
            string _dataID = query.Data_ID.ToString();
            string _traceID = query.TraceID;
            string _dataType = query.Data_Type;
            string _dataTypeName = query.Data_TypeName;
            string _custID = query.CustID;
            string _custName = query.CustName;
            string _orderType = query.OrderType;
            string _currency = query.Currency;
            string _fileName = query.Upload_File;
            Int16 _status = query.Status;

            //release
            query = null;

            /*
             * 判斷狀態, 導至指定頁面
             */
            switch (_status)
            {
                case 13:
                case 99:
                    //完成,作廢(離開此頁)
                    Response.Redirect(Page_SearchUrl);
                    break;

                default:
                    //不採取動作
                    break;
            }

            //填入表單欄位
            lb_TraceID.Text = _traceID;
            lb_DataType.Text = _dataTypeName;
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_OrderType.Text = _orderType;
            lb_Currency.Text = _currency;
            hf_DataType.Value = _dataType;
            hf_CustID.Value = _custID;

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
    /// 單身資料
    /// </summary>
    /// <param name="_parentID"></param>
    private void LookupData_Detail(string _parentID)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.GetDetailList(_parentID, search, out ErrMsg);
            bool showBtn = true;
            bool showErrTip = false;

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

            //Check null
            if (query.Count() == 0)
            {
                showBtn = false;
                showErrTip = true;
            }

            //判斷是否有未通過的資料
            var countIsPass = query.Where(i => i.IsPass.Equals("N"));
            if (countIsPass.Count() > 0)
            {
                lb_showTip.Text = "尚有品項未通過檢查,確認無誤後才能進行下一步.";
                showBtn = false;
                showErrTip = true;
            }

            //判斷是否有檢查中的資料
            var checkIsPass = query.Where(i => i.IsPass.Equals("E"));
            if (checkIsPass.Count() > 0)
            {
                lb_showTip.Text = "品項價格未通過檢查,確認無誤後才能進行下一步.";
                showBtn = false;
                showErrTip = true;
            }

            //Show
            ph_WorkBtns.Visible = showBtn;
            ph_ErrTips.Visible = showErrTip;

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


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        //取得Key值
        string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

        switch (e.CommandName.ToUpper())
        {
            case "DODEL":
                //----- 宣告:資料參數 -----
                TWBBCRepository _data = new TWBBCRepository();

                try
                {
                    //----- 方法:刪除資料 -----
                    if (false == _data.DeleteItem(Req_DataID, Get_DataID, out ErrMsg))
                    {
                        CustomExtension.AlertMsg("刪除失敗", "");
                        return;
                    }
                    else
                    {
                        //導向本頁
                        Response.Redirect(thisPage);
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

                break;
        }

    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料:ID, IsPass, IsGift
                string _id = DataBinder.Eval(dataItem.DataItem, "Data_ID").ToString();
                string _isPass = DataBinder.Eval(dataItem.DataItem, "IsPass").ToString();
                string _isGift = DataBinder.Eval(dataItem.DataItem, "IsGift").ToString();


                //欄位顯示:檢查結果,異常時顯示
                PlaceHolder ph_attention = (PlaceHolder)e.Item.FindControl("ph_attention");
                ph_attention.Visible = _isPass.Equals("N");
                Label lb_attaSign = (Label)e.Item.FindControl("lb_attaSign");
                lb_attaSign.Attributes.Add("data-id", "atta" + _id);


                //欄位顯示:設定下拉選單(贈品), 顯示Icon
                DropDownList ddl_IsGift = (DropDownList)e.Item.FindControl("ddl_IsGift");
                ddl_IsGift.SelectedIndex =
                    ddl_IsGift.Items.IndexOf(
                    ddl_IsGift.Items.FindByValue(_isGift));

                PlaceHolder ph_Gift = (PlaceHolder)e.Item.FindControl("ph_Gift");
                ph_Gift.Visible = _isGift.Equals("Y");
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 新增項目, 新增後呼叫Job2~5
    /// </summary>
    protected void lbtn_NewItem_Click(object sender, EventArgs e)
    {
        string errTxt = "";
        string _dataID = Req_DataID;
        string _dataType = hf_DataType.Value;
        string _custID = hf_CustID.Value;
        string _modelNo = val_ModelNo.Text;
        string _cnt = tb_InputCnt.Text;

        //必填檢查
        if (string.IsNullOrWhiteSpace(_modelNo) || string.IsNullOrWhiteSpace(_cnt) || !_cnt.IsNumeric())
        {
            errTxt += "===請檢查以下欄位===\\n";
            errTxt += "品號\\n";
            errTxt += "數量\\n";
        }
        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        //宣告
        TWBBCRepository _data = new TWBBCRepository();
        try
        {
            //單頭資料inst
            var baseData = new ImportData
            {
                Data_ID = new Guid(_dataID),
                Data_Type = _dataType,
                Update_Who = fn_Params.UserGuid
            };
            //單身資料inst
            var detail = new ImportDataDT
            {
                ProdID = _modelNo.Trim(),
                BuyCnt = Convert.ToInt32(_cnt)
            };

            //建立單筆資料
            Int32 _newDataID = _data.CreateDetailItem(baseData, detail, out ErrMsg);
            if (_newDataID.Equals(0))
            {
                //Show Error
                string msg = "資料建立失敗;" + ErrMsg;
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            //執行JOB 2~5
            #region -- [Check Job2] 檢查寶工品號 --

            if (!_data.CheckJob2(_dataID, _dataType, _newDataID.ToString(), out ErrMsg))
            {
                string msg = "(Step3)檢查寶工品號...[Job2];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job3] 更新公司別/資料庫名 --

            if (!_data.CheckJob3(_dataID, _newDataID.ToString(), out ErrMsg))
            {
                string msg = "(Step3)更新公司別/資料庫名...[Job3];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job4] 更新產品其他欄位 --

            if (!_data.CheckJob4(_dataID, _newDataID.ToString(), out ErrMsg))
            {
                string msg = "(Step3)更新產品其他欄位...[Job4];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion


            #region -- [Check Job5] 取得價格 --

            if (!_data.CheckJob5(_dataID, _custID, "", out ErrMsg))
            {
                string msg = "(Step3)取得價格...[Job5];" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            #endregion

            //[清空Log]
            _data.Update_ClearLog(_dataID, out ErrMsg);

            //導至本頁
            Response.Redirect(thisPage);

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
    /// 存檔,停在本頁
    /// </summary>
    /// <remarks>
    /// 列出Listview,取得內容控制項
    /// </remarks>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        //宣告
        TWBBCRepository _data = new TWBBCRepository();
        try
        {
            List<ImportDataDT> dataList = new List<ImportDataDT>();

            for (int row = 0; row < lvDataList.Items.Count; row++)
            {
                //取得編號
                string _id = ((HiddenField)lvDataList.Items[row].FindControl("hf_DataID")).Value;
                //取得輸入欄:輸入數量
                string _inputCnt = ((TextBox)lvDataList.Items[row].FindControl("tb_InputCnt")).Text;
                //取得輸入欄:贈品
                string _isGift = ((DropDownList)lvDataList.Items[row].FindControl("ddl_IsGift")).SelectedValue;

                //加入項目
                var dataItem = new ImportDataDT
                {
                    Data_ID = Convert.ToInt32(_id),
                    InputCnt = string.IsNullOrWhiteSpace(_inputCnt) ? 1 : Convert.ToInt32(_inputCnt),
                    IsGift = _isGift
                };

                //將項目加入至集合
                dataList.Add(dataItem);
            }

            //Update
            if (!_data.Update_Items(Req_DataID, dataList.AsQueryable(), out ErrMsg))
            {
                //Show Error
                string msg = "列表資料更新失敗;" + ErrMsg;
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }

            //導至本頁
            Response.Redirect(thisPage);
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
    /// 轉入EDI排程
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        //宣告
        TWBBCRepository _data = new TWBBCRepository();
        string _dataID = Req_DataID;
        try
        {
            //EDI轉入
            if (!_data.Create_EDI(_dataID, out ErrMsg))
            {
                string msg = "(Step3)EDI轉入失敗..;" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }


            //更新狀態Item
            var data = new ImportData
            {
                Data_ID = new Guid(_dataID),
                Status = 13,
                Update_Who = fn_Params.UserGuid
            };

            //----- 方法:更新狀態 -----
            if (false == _data.Update_Status(data, out ErrMsg))
            {
                string msg = "(Step3)EDI轉入成功, 狀態更新失敗;" + ErrMsg;

                //[建立Log]
                _data.Update_SetLog(_dataID, msg, out ErrMsg);

                //Show Error
                ph_ErrMessage.Visible = true;
                lt_ShowMsg.Text = msg;
                return;
            }


            //[清空Log]
            _data.Update_ClearLog(_dataID, out ErrMsg);

            //導至下一步
            Response.Redirect(nextPage);


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
    /// 單身資料匯出Excel
    /// </summary>
    protected void btn_Export_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        DataTable DT = new DataTable();
        string _custName = lb_Cust.Text.Replace(" ", "");

        //----- 方法:取得資料 -----
        var query = _data.GetDetailList(Req_DataID, search, out ErrMsg)
            .Select(fld => new
            {
                Data_ID = fld.Data_ID,
                ShipFrom = fld.ShipFrom,
                ERP_ModelNo = fld.ERP_ModelNo,
                Cust_ModelNo = fld.Cust_ModelNo,
                BuyCnt = fld.BuyCnt,
                //(訂單數量/內盒數=小數第一位)
                CheckCnt = Convert.ToInt32(fld.InnerBox) > 0
                        ? Math.Round(Convert.ToDouble(fld.BuyCnt) / Convert.ToDouble(fld.InnerBox), 1)
                        : 0,
                CheckCnt_Int = Convert.ToInt32(fld.InnerBox) > 0
                    ? Math.Ceiling(Convert.ToDouble(fld.BuyCnt) / Convert.ToDouble(fld.InnerBox)) * fld.InnerBox
                    : 0,
                InputCnt = fld.InputCnt,
                IsGift = fld.IsGift,
                BuyPrice = fld.BuyPrice,
                ERP_Price = fld.ERP_Price,
                QuoteDate = fld.QuoteDate,
                LastSalesDay = fld.LastSalesDay,
                InnerBox = fld.InnerBox,
                OuterBox = fld.OuterBox,
                ProdMsg = fld.ProdMsg,
                doWhat = fld.doWhat
            });

        if (query.Count() == 0)
        {
            CustomExtension.AlertMsg("查無資料,請重新確認.", "");
            return;
        }

        //將IQueryable轉成DataTable
        DataTable myDT = CustomExtension.LINQToDataTable(query);

        if (myDT.Rows.Count > 0)
        {
            //重新命名欄位標頭
            myDT.Columns["Data_ID"].ColumnName = "編號";
            myDT.Columns["ShipFrom"].ColumnName = "出貨地";
            myDT.Columns["ERP_ModelNo"].ColumnName = "寶工品號";
            myDT.Columns["Cust_ModelNo"].ColumnName = "客戶品號";
            myDT.Columns["BuyCnt"].ColumnName = "訂單數量";
            myDT.Columns["CheckCnt"].ColumnName = "(訂單數量/內盒數)";
            myDT.Columns["CheckCnt_Int"].ColumnName = "比對數量";
            myDT.Columns["InputCnt"].ColumnName = "修改數量(ERP訂單數量)";
            myDT.Columns["IsGift"].ColumnName = "是否為贈品";
            myDT.Columns["BuyPrice"].ColumnName = "訂單價格";
            myDT.Columns["ERP_Price"].ColumnName = "ERP價格";
            myDT.Columns["QuoteDate"].ColumnName = "核價日";
            myDT.Columns["LastSalesDay"].ColumnName = "上次銷貨日";
            myDT.Columns["InnerBox"].ColumnName = "內盒數";
            myDT.Columns["OuterBox"].ColumnName = "外箱整箱數";
            myDT.Columns["ProdMsg"].ColumnName = "產銷訊息";
            myDT.Columns["doWhat"].ColumnName = "異常檢查";
        }

        //release
        query = null;

        //匯出Excel
        CustomExtension.ExportExcel(
            myDT
            , "{0}-{1}.xlsx".FormatThis(
                _custName
                , DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
            , false);
    }
    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC".FormatThis(fn_Params.WebUrl);
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
    /// 上一頁網址
    /// </summary>
    private string _prevPage;
    public string prevPage
    {
        get
        {
            return "{0}/ImportStep2.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._prevPage = value;
        }
    }

    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}/ImportStep3.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._thisPage = value;
        }
    }

    /// <summary>
    /// 下一頁網址
    /// </summary>
    private string _nextPage;
    public string nextPage
    {
        get
        {
            return "{0}/ImportStep4.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._nextPage = value;
        }
    }


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("TWBBC");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }
    #endregion

}