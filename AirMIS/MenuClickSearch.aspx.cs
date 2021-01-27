using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MISData.Controllers;
using PKLib_Method.Methods;


public partial class AirMIS_MenuClickSearch : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("9801", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //取得年份(從2020開始)
                Get_Years();

                //取得月日
                Get_MonthDay(ddl_SM, 1, 12, Req_SM);
                Get_MonthDay(ddl_SD, 1, 31, Req_SD);
                Get_MonthDay(ddl_EM, 1, 12, Req_EM);
                Get_MonthDay(ddl_ED, 1, 31, Req_ED);

                int checkCnt = 2;
                int countfilter = 0;

                //[查詢條件] - Year
                if (!string.IsNullOrWhiteSpace(Req_Year))
                {
                    ddl_Year.SelectedValue = Req_Year;
                    countfilter++;
                }

                //[查詢條件] - WebID
                if (!string.IsNullOrWhiteSpace(Req_WebID))
                {
                    ddl_Website.SelectedValue = Req_WebID;
                    countfilter++;
                }

                //判斷條件是否達成
                if (countfilter == checkCnt)
                {
                    if (Req_Tab.Equals("1"))
                    {
                        /* 啟動樹狀模式 */
                        //Tree js
                        ph_treeJS.Visible = true;
                        //empty message
                        ph_tab1emptyData.Visible = false;
                    }
                    else if (Req_Tab.Equals("2"))
                    {
                        /* 啟動列表模式 */
                        //Table js
                        ph_tableJS.Visible = true;
                        //empty message
                        ph_tab2emptyData.Visible = false;

                        //data
                        LookupData();
                    }

                }

                //Tab switch
                ph_tab1.Visible = Req_Tab.Equals("1");
                ph_tab2.Visible = Req_Tab.Equals("2");

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- Table模式資料 --

    /// <summary>
    /// 取得資料-Table
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        MISRepository _data = new MISRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            string _sDate = (new DateTime(Convert.ToInt16(Req_Year), Convert.ToInt16(Req_SM), Convert.ToInt16(Req_SD))).ToShortDateString().ToDateString("yyyy/MM/dd");
            string _eDate = (new DateTime(Convert.ToInt16(Req_Year), Convert.ToInt16(Req_EM), Convert.ToInt16(Req_ED))).ToShortDateString().ToDateString("yyyy/MM/dd");
            var query = _data.Get_WebClickList_byTable(Req_WebID, Req_Year, _sDate, _eDate, out ErrMsg);


            //----- 資料整理:繫結 ----- 
            lvDataList.DataSource = query;
            lvDataList.DataBind();

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

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                #region 說明Url

                /* 設定說明 */
                Int32 _MenuID = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "MenuID"));
                string _remarkTxt = DataBinder.Eval(dataItem.DataItem, "Remark").ToString();
                if (!string.IsNullOrWhiteSpace(_remarkTxt))
                {
                    string _remarkUrl = "<a href=\"#!\" class=\"doShowMsg blue-text text-darken-1\" data-id=\"##menuID##\" data-title=\"\">(說明)</a>";

                    _remarkUrl = _remarkUrl.Replace("##menuID##", _MenuID.ToString());
                    //set html
                    Literal lt_RemarkUrl = (Literal)e.Item.FindControl("lt_RemarkUrl");
                    lt_RemarkUrl.Text = _remarkUrl;
                }
                #endregion


                /* 設定名單 */
                #region 名單Url
                Int32 _Lv = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "Lv"));
                Int32 _ClickCnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ClickCnt"));
                string _title = "";

                switch (_Lv)
                {
                    case 1:
                        _title = DataBinder.Eval(dataItem.DataItem, "Lv1Name").ToString();
                        break;

                    case 2:
                        _title = DataBinder.Eval(dataItem.DataItem, "Lv2Name").ToString();
                        break;

                    case 3:
                        _title = DataBinder.Eval(dataItem.DataItem, "Lv3Name").ToString();
                        break;

                    case 4:
                        _title = DataBinder.Eval(dataItem.DataItem, "Lv4Name").ToString();
                        break;
                }
                if (_ClickCnt > 0)
                {
                    string _Url = "<a href=\"#!\" class=\"doShowDT green-text text-darken-1\" data-id=\"##menuID##\" data-title=\"##menuName##\">(名單)</a>";

                    _Url = _Url.Replace("##menuID##", _MenuID.ToString()).Replace("##menuName##", _title);
                    //set html
                    Literal lt_ClickWho = (Literal)e.Item.FindControl("lt_ClickWho");
                    lt_ClickWho.Text = _Url;
                }

                #endregion
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
        string _Year = ddl_Year.SelectedValue;
        string _Web = ddl_Website.SelectedValue;
        string _sm = ddl_SM.SelectedValue;
        string _sd = ddl_SD.SelectedValue;
        string _em = ddl_EM.SelectedValue;
        string _ed = ddl_ED.SelectedValue;

        //Check Null
        if (string.IsNullOrWhiteSpace(_Year) || string.IsNullOrWhiteSpace(_Web))
        {
            CustomExtension.AlertMsg("年份或網站未選擇", "");
            return thisPage;
        }

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?tab={1}".FormatThis(thisPage, Req_Tab));

        //[查詢條件] - Year
        if (!string.IsNullOrWhiteSpace(_Year))
        {
            url.Append("&y=" + Server.UrlEncode(_Year));
        }

        //[查詢條件] - Web
        if (!string.IsNullOrWhiteSpace(_Web))
        {
            url.Append("&web=" + Server.UrlEncode(_Web));
        }

        url.Append("&sm=" + Server.UrlEncode(_sm));
        url.Append("&sd=" + Server.UrlEncode(_sd));
        url.Append("&em=" + Server.UrlEncode(_em));
        url.Append("&ed=" + Server.UrlEncode(_ed));

        //return
        return url.ToString();
    }


    /// <summary>
    /// 取得年份
    /// </summary>
    private void Get_Years()
    {
        int thisYear = DateTime.Now.Year;
        ddl_Year.Items.Clear();

        for (int row = 2020; row <= thisYear; row++)
        {
            ddl_Year.Items.Add(new ListItem(row.ToString(), row.ToString()));
        }

        //selected
        ddl_Year.SelectedValue = thisYear.ToString();
    }


    /// <summary>
    /// 取得月日
    /// </summary>
    /// <param name="_drp"></param>
    /// <param name="_start"></param>
    /// <param name="_end"></param>
    /// <param name="inputVal"></param>
    private void Get_MonthDay(DropDownList _drp, int _start, int _end, string inputVal)
    {
        _drp.Items.Clear();

        for (int row = _start; row <= _end; row++)
        {
            _drp.Items.Add(new ListItem(row.ToString(), row.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(inputVal))
        {
            _drp.SelectedValue = inputVal;
        }
    }
    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 取得傳遞參數 - Year
    /// </summary>
    public string Req_Year
    {
        get
        {
            String _data = Request.QueryString["y"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "4", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Year = value;
        }
    }
    private string _Req_Year;


    private string _Req_SM;
    public string Req_SM
    {
        get
        {
            String _data = Request.QueryString["sm"];

            return string.IsNullOrWhiteSpace(_data) ? "1" : _data.Trim();
        }
        set
        {
            _Req_SM = value;
        }
    }
    private string _Req_SD;
    public string Req_SD
    {
        get
        {
            String _data = Request.QueryString["sd"];

            return string.IsNullOrWhiteSpace(_data) ? "1" : _data.Trim();
        }
        set
        {
            _Req_SD = value;
        }
    }


    private string _Req_EM;
    public string Req_EM
    {
        get
        {
            String _data = Request.QueryString["em"];

            return string.IsNullOrWhiteSpace(_data) ? "12" : _data.Trim();
        }
        set
        {
            _Req_EM = value;
        }
    }
    private string _Req_ED;
    public string Req_ED
    {
        get
        {
            String _data = Request.QueryString["ed"];

            return string.IsNullOrWhiteSpace(_data) ? "31" : _data.Trim();
        }
        set
        {
            _Req_ED = value;
        }
    }



    /// <summary>
    /// 取得傳遞參數 - 網站
    /// </summary>
    private string _Req_WebID;
    public string Req_WebID
    {
        get
        {
            String _data = Request.QueryString["web"];

            return string.IsNullOrWhiteSpace(_data) ? "" : _data.Trim();
        }
        set
        {
            _Req_WebID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Tab
    /// </summary>
    public string Req_Tab
    {
        get
        {
            String _data = Request.QueryString["tab"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "1", out ErrMsg)) ? _data.Trim() : "1";
        }
        set
        {
            this._Req_Tab = value;
        }
    }
    private string _Req_Tab;

    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}AirMIS/MenuClickSearch.aspx".FormatThis(fn_Params.WebUrl);
        }
        set
        {
            _thisPage = value;
        }
    }

    #endregion
}