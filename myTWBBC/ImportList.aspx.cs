using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using TW_BBC.Controllers;
using TW_BBC.Models;

public partial class myTWBBC_ImportList : SecurityIn
{
    public string ErrMsg;
    public bool closeAuth = false; //作廢權限(可在權限設定裡勾選)

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("1216", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", fn_Params.WebUrl, HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                closeAuth = fn_CheckAuth.CheckAuth_User("1217", out ErrMsg);


                //Get Class
                CreateDropDownMenu(filter_Status, true, "所有資料");

                //Get Data
                LookupDataList(Req_PageIdx);

            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --
    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:網址參數 -----
        int RecordsPerPage = 10;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        int DataCnt = 0;
        ArrayList PageParam = new ArrayList();  //分類暫存條件參數

        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            #region >> 條件篩選 <<

            //[查詢條件] - Keyword
            if (!string.IsNullOrWhiteSpace(Req_Keyword))
            {
                search.Add("Keyword", Req_Keyword);
                PageParam.Add("k=" + Server.UrlEncode(Req_Keyword));
                filter_Keyword.Text = Req_Keyword;
            }

            //[查詢條件] - Status
            if (!string.IsNullOrWhiteSpace(Req_Status))
            {
                search.Add("Status", Req_Status);
                PageParam.Add("st=" + Server.UrlEncode(Req_Status));
                filter_Status.SelectedValue = Req_Status;
            }

            //[取得/檢查參數] - Date
            if (!string.IsNullOrWhiteSpace(Req_sDate))
            {
                search.Add("sDate", Req_sDate);
                PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
                filter_sDate.Text = Req_sDate;
            }
            if (!string.IsNullOrWhiteSpace(Req_eDate))
            {
                search.Add("eDate", Req_eDate);
                PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
                filter_eDate.Text = Req_eDate;
            }

            #endregion

            //----- 原始資料:取得所有資料 -----
            var query = _data.GetList(search, StartRow, RecordsPerPage
                , out DataCnt, out ErrMsg);

            //----- 資料整理:取得總筆數 -----
            TotalRow = DataCnt;

            //----- 資料整理:頁數判斷 -----
            if (pageIndex > ((TotalRow / RecordsPerPage) + ((TotalRow % RecordsPerPage) > 0 ? 1 : 0)) && TotalRow > 0)
            {
                StartRow = 0;
                pageIndex = 1;
            }

            //----- 資料整理:繫結 ----- 
            lvDataList.DataSource = query;
            lvDataList.DataBind();


            //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
            if (query.Count() == 0)
            {
                ph_EmptyData.Visible = true;
                ph_Data.Visible = false;

                //Clear
                CustomExtension.setCookie("TWBBC", "", -1);
            }
            else
            {
                ph_EmptyData.Visible = false;
                ph_Data.Visible = true;

                //分頁設定
                string getPager = CustomExtension.Pagination(TotalRow, RecordsPerPage, pageIndex, 5
                    , thisPage, PageParam, false, true);

                Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                lt_Pager.Text = getPager;

                //重新整理頁面Url
                string reSetPage = "{0}?page={1}{2}".FormatThis(
                    thisPage
                    , pageIndex
                    , (PageParam.Count == 0 ? "" : "&") + string.Join("&", PageParam.ToArray()));

                //暫存頁面Url, 給其他頁使用
                CustomExtension.setCookie("TWBBC", Server.UrlEncode(reSetPage), 1);

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

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        //取得Key值
        string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

        switch (e.CommandName.ToUpper())
        {
            case "DOCLOSE":
                //----- 宣告:資料參數 -----
                TWBBCRepository _data = new TWBBCRepository();

                try
                {
                    var data = new ImportData
                    {
                        Data_ID = new Guid(Get_DataID),
                        Status = 99,
                        Update_Who = fn_Params.UserGuid
                    };

                    //----- 方法:更新資料 -----
                    if (false == _data.Update_Status(data, out ErrMsg))
                    {
                        CustomExtension.AlertMsg("作廢失敗", "");
                        return;
                    }
                    else
                    {
                        //導向本頁
                        Response.Redirect(filterUrl());
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

                //取得資料:錯誤訊息
                string _errMessage = DataBinder.Eval(dataItem.DataItem, "ErrMessage").ToString();
                //取得資料:狀態
                string Get_Status = DataBinder.Eval(dataItem.DataItem, "Status").ToString();

                //欄位顯示:Error
                PlaceHolder ph_ErrLog = (PlaceHolder)e.Item.FindControl("ph_ErrLog");
                ph_ErrLog.Visible = !string.IsNullOrWhiteSpace(_errMessage);

                //欄位顯示:Edit/Del
                PlaceHolder ph_KeepGo = (PlaceHolder)e.Item.FindControl("ph_KeepGo");
                PlaceHolder ph_Del = (PlaceHolder)e.Item.FindControl("ph_Del");

                /* 判斷狀態:按鈕顯示時機
                 * 編輯:不顯示=13,99
                 * 作廢:顯示=13
                 */
                bool showbtn_Edit = false;
                bool showbtn_Del = false;

                switch (Get_Status)
                {
                    case "99":
                        showbtn_Edit = false;
                        break;

                    case "13":
                        //完成
                        showbtn_Del = true;
                        break;

                    default:
                        showbtn_Edit = true;
                        break;
                }
                //編輯鈕顯示否
                ph_KeepGo.Visible = showbtn_Edit;
                //作廢鈕顯示否
                ph_Del.Visible = closeAuth && showbtn_Del;

            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 產生下拉選單
    /// </summary>
    /// <param name="_drp">控制項</param>
    /// <param name="_showRoot">是否顯示根選單</param>
    /// <param name="_rootText">根選單文字</param>
    private void CreateDropDownMenu(DropDownList _drp, bool _showRoot, string _rootText)
    {
        //----- 宣告:資料參數 -----
        TWBBCRepository _data = new TWBBCRepository();

        try
        {
            //取得資料(type=1)
            var data = _data.GetClassList("1", null, out ErrMsg);

            //繫結設定
            _drp.DataSource = data;
            _drp.DataValueField = "ID";
            _drp.DataTextField = "Label";
            _drp.DataBind();

            //判斷是否有root
            if (_showRoot)
            {
                _drp.Items.Insert(0, new ListItem(_rootText, ""));
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

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// [按鈕] - 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        //執行查詢
        Response.Redirect(filterUrl(), false);
    }

    #endregion


    #region -- 附加功能 --

    /// <summary>
    /// 含查詢條件的完整網址(新查詢)
    /// </summary>
    /// <returns></returns>
    public string filterUrl()
    {
        //Params
        string _sDate = filter_sDate.Text;
        string _eDate = filter_eDate.Text;
        string _Keyword = filter_Keyword.Text;
        string _Status = filter_Status.SelectedValue;

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?page=1".FormatThis(thisPage));

        //[查詢條件] - Date
        if (!string.IsNullOrWhiteSpace(_sDate))
        {
            url.Append("&sDate=" + Server.UrlEncode(_sDate));
        }
        if (!string.IsNullOrWhiteSpace(_eDate))
        {
            url.Append("&eDate=" + Server.UrlEncode(_eDate));
        }

        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(_Keyword))
        {
            url.Append("&k=" + Server.UrlEncode(_Keyword));
        }

        //[查詢條件] - Status
        if (!string.IsNullOrWhiteSpace(_Status))
        {
            url.Append("&st=" + Server.UrlEncode(_Status));
        }

        return url.ToString();
    }


    /// <summary>
    /// 狀態顏色
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string GetStatusColor(string value)
    {
        switch (value)
        {
            case "10":
                return "orange";

            case "11":
            case "12":
                return "teal";

            case "14":
                return "red";

            default:
                return "grey";
        }
    }


    /// <summary>
    /// 繼續匯入的網址,依不同狀態前往指定頁面
    /// </summary>
    /// <param name="_status">狀態</param>
    /// <param name="_id">主檔編號</param>
    /// <returns></returns>
    public string keepGoingUrl(int _status, string _id)
    {
        string url = "";
        string goPage = "#!";
        /*
        * 判斷狀態, 導至指定頁面
        */
        switch (_status)
        {
            case 10:
                //轉向至Step2
                goPage = "ImportStep2.aspx";
                break;

            case 11:
            case 12:
            case 14:
                //轉向至Step3
                goPage = "ImportStep3.aspx";
                break;

            default:
                break;
        }

        url = "{0}myTWBBC/{1}?id={2}".FormatThis(fn_Params.WebUrl, goPage, _id);
        return url;
    }

    #endregion


    #region -- 網址參數 --


    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myTWBBC/ImportList.aspx".FormatThis(
            fn_Params.WebUrl);
    }


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    public int Req_PageIdx
    {
        get
        {
            int data = Request.QueryString["Page"] == null ? 1 : Convert.ToInt32(Request.QueryString["Page"]);
            return data;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }
    private int _Req_PageIdx;


    /// <summary>
    /// 取得傳遞參數 - sDate
    /// 預設30 days內
    /// </summary>
    public string Req_sDate
    {
        get
        {
            String _data = Request.QueryString["sDate"];
            string dt = DateTime.Now.AddDays(-30).ToShortDateString().ToDateString("yyyy/MM/dd");
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : dt;
        }
        set
        {
            this._Req_sDate = value;
        }
    }
    private string _Req_sDate;


    /// <summary>
    /// 取得傳遞參數 - eDate
    /// </summary>
    public string Req_eDate
    {
        get
        {
            String _data = Request.QueryString["eDate"];
            string dt = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : dt;
        }
        set
        {
            this._Req_eDate = value;
        }
    }
    private string _Req_eDate;

    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    public string Req_Keyword
    {
        get
        {
            String _data = Request.QueryString["k"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "20", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    private string _Req_Keyword;

    /// <summary>
    /// 取得傳遞參數 - Status
    /// </summary>
    public string Req_Status
    {
        get
        {
            String _data = Request.QueryString["st"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "3", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Status = value;
        }
    }
    private string _Req_Status;


    #endregion
}