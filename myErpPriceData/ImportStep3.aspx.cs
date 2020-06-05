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
using ERP_PriceData.Controllers;
using ERP_PriceData.Models;

public partial class myErpPriceData_ImportStep3 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                #region --權限--
                //[權限判斷] Start
                /* 
                 * 使用公司別代號，判斷對應的MENU ID
                 */
                bool isPass = false;

                switch (Req_DBS)
                {
                    case "TW":
                        isPass = fn_CheckAuth.CheckAuth_User("416", out ErrMsg);
                        break;

                    case "SH":
                        isPass = fn_CheckAuth.CheckAuth_User("417", out ErrMsg);
                        break;
                }

                if (!isPass)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                #endregion


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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();
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
            string _dbs = query.DBS;
            string _custID = query.CustID;
            string _custName = query.CustName;
            string _orderType = query.OrderType;
            string _orderNo = query.OrderNo;
            string _validDate = query.ValidDate;
            string _fileName = query.Upload_File;
            Int16 _status = query.Status;

            //release
            query = null;

            /*
             * 判斷狀態, 導至指定頁面
             */
            switch (_status)
            {
                case 20:
                    //完成(離開此頁)
                    Response.Redirect(Page_SearchUrl);
                    break;

                default:
                    //不採取動作
                    break;
            }

            //填入表單欄位
            lb_TraceID.Text = _traceID;
            lb_DBS.Text = fn_Desc.PubAll.AreaDesc(_dbs);
            lb_Cust.Text = "{0} ({1})".FormatThis(_custName, _custID);
            lb_OrderType.Text = _orderType;
            lb_OrderNo.Text = _orderNo;
            lb_validDate.Text = _validDate;

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
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();
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
                ERP_PriceDataRepository _data = new ERP_PriceDataRepository();

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


                //欄位顯示:檢查結果,異常時顯示
                PlaceHolder ph_attention = (PlaceHolder)e.Item.FindControl("ph_attention");
                ph_attention.Visible = _isPass.Equals("N");
                Label lb_attaSign = (Label)e.Item.FindControl("lb_attaSign");
                lb_attaSign.Attributes.Add("data-id", "atta" + _id);

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
    /// 轉入EDI排程
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        //宣告
        ERP_PriceDataRepository _data = new ERP_PriceDataRepository();
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
                Status = 20,
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


    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myErpPriceData".FormatThis(fn_Params.WebUrl);
    }

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 設定參數 - DBS
    /// </summary>
    private string _Req_DBS;
    public string Req_DBS
    {
        get
        {
            string data = Request.QueryString["dbs"];

            return string.IsNullOrWhiteSpace(data) ? "TW" : data.ToString();
        }
        set
        {
            this._Req_DBS = value;
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
    /// 上一頁網址
    /// </summary>
    private string _prevPage;
    public string prevPage
    {
        get
        {
            return "{0}/ImportStep2.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), Req_DataID, Req_DBS);
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
            return "{0}/ImportStep3.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), Req_DataID, Req_DBS);
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
            return "{0}/ImportStep4.aspx?id={1}&dbs={2}".FormatThis(FuncPath(), Req_DataID, Req_DBS);
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
            string tempUrl = CustomExtension.getCookie("ErpPriceData");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/ImportList.aspx?dbs=" + Req_DBS : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }
    #endregion

}