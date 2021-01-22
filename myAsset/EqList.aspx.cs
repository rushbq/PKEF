using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AssetData.Controllers;
using PKLib_Method.Methods;


public partial class myAsset_EqList : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //必要參數檢查
                if (string.IsNullOrWhiteSpace(Req_DBS))
                {
                    CustomExtension.AlertMsg("必要參數不存在", fn_Params.WebUrl);
                    return;
                }

                //取得公司別
                lt_CorpName.Text = Req_DBS;
                Page.Title += " | " + Req_DBS;

                //[權限判斷]
                string checkID = Req_DBS.Equals("TW") ? "553" : "554";
                if (fn_CheckAuth.CheckAuth_User(checkID, out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //Get Class
                Get_ClassList("A", filter_ClsLv1, "所有資料", Req_Cls1);
                Get_ClassList("B", filter_ClsLv2, "所有資料", Req_Cls2);

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
    /// <param name="pageIndex"></param>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:網址參數 -----
        int RecordsPerPage = 10;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        int DataCnt = 0;
        ArrayList PageParam = new ArrayList();  //分類暫存條件參數

        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            #region >> 條件篩選 <<
            //Params
            string _sDateA = Req_sDateA;
            string _eDateA = Req_eDateA;
            string _sDateB = Req_sDateB;
            string _eDateB = Req_eDateB;
            string _ErpID = Req_ErpID;
            string _ClsLv1 = Req_Cls1;
            string _ClsLv2 = Req_Cls2;

            //Search固定參數
            PageParam.Add("dbs=" + Req_DBS);

            //[查詢條件] - DateA
            if (!string.IsNullOrWhiteSpace(_sDateA))
            {
                search.Add("sDateA", _sDateA);
                PageParam.Add("sDateA=" + Server.UrlEncode(_sDateA));
                filter_sDateA.Text = _sDateA;
            }
            if (!string.IsNullOrWhiteSpace(_eDateA))
            {
                search.Add("eDateA", _eDateA);
                PageParam.Add("eDateA=" + Server.UrlEncode(_eDateA));
                filter_eDateA.Text = _eDateA;
            }

            //[查詢條件] - DateB
            if (!string.IsNullOrWhiteSpace(_sDateB))
            {
                search.Add("sDateB", _sDateB);
                PageParam.Add("sDateB=" + Server.UrlEncode(_sDateB));
                filter_sDateB.Text = _sDateB;
            }
            if (!string.IsNullOrWhiteSpace(_eDateB))
            {
                search.Add("eDateB", _eDateB);
                PageParam.Add("eDateB=" + Server.UrlEncode(_eDateB));
                filter_eDateB.Text = _eDateB;
            }

            //[查詢條件] - ErpID
            if (!string.IsNullOrWhiteSpace(_ErpID))
            {
                search.Add("ErpID", _ErpID);
                PageParam.Add("ErpID=" + Server.UrlEncode(_ErpID));
                filter_ErpID.Text = _ErpID;
            }

            //[查詢條件] - ClsLv1
            if (!string.IsNullOrWhiteSpace(_ClsLv1))
            {
                search.Add("ClsLv1", _ClsLv1);
                PageParam.Add("ClsLv1=" + Server.UrlEncode(_ClsLv1));
                filter_ClsLv1.SelectedValue = _ClsLv1;
            }
            //[查詢條件] - ClsLv2
            if (!string.IsNullOrWhiteSpace(_ClsLv2))
            {
                search.Add("ClsLv2", _ClsLv2);
                PageParam.Add("ClsLv2=" + Server.UrlEncode(_ClsLv2));
                filter_ClsLv2.SelectedValue = _ClsLv2;
            }
            #endregion

            //----- 原始資料:取得所有資料 -----
            var query = _data.Get_AssetList(search, Req_DBS, StartRow, RecordsPerPage, true
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
            if (query.Rows.Count == 0)
            {
                ph_EmptyData.Visible = true;
                ph_Data.Visible = false;

                //Clear
                //CustomExtension.setCookie("ARData", "", -1);
            }
            else
            {
                ph_EmptyData.Visible = false;
                ph_Data.Visible = true;

                //分頁設定
                string getPager = CustomExtension.Pagination(TotalRow, RecordsPerPage, pageIndex, 5
                    , FuncPath(), PageParam, false, true);

                Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                lt_Pager.Text = getPager;

                //重新整理頁面Url
                //string reSetPage = "{0}&page={1}{2}".FormatThis(
                //    FuncPath()
                //    , pageIndex
                //    , (PageParam.Count == 0 ? "" : "&") + string.Join("&", PageParam.ToArray()));

                //暫存頁面Url, 給其他頁使用
                //CustomExtension.setCookie("ARData", Server.UrlEncode(reSetPage), 1);

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

        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        //----- 方法:刪除資料 -----
        if (false == _data.Delete_AssetList(Get_DataID, out ErrMsg))
        {
            CustomExtension.AlertMsg("刪除失敗", ErrMsg);
            return;
        }
        else
        {
            //導向本頁(帶參數)
            Response.Redirect(filterUrl());
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        //try
        //{
        //    if (e.Item.ItemType == ListViewItemType.DataItem)
        //    {
        //        ListViewDataItem dataItem = (ListViewDataItem)e.Item;

        //        //取得資料:單身數
        //        Int32 _Cnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ChildCnt"));


        //    }
        //}
        //catch (Exception)
        //{
        //    throw;
        //}
    }


    /// <summary>
    /// 取得類別
    /// </summary>
    /// <param name="clsType">A,B</param>
    /// <param name="ddl"></param>
    /// <param name="rootName"></param>
    /// <param name="inputValue"></param>
    private void Get_ClassList(string clsType, DropDownList ddl, string rootName, string inputValue)
    {
        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        //----- 原始資料:取得所有資料 -----
        DataTable query = _data.GetClass_Asset(clsType, out ErrMsg);

        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        for (int row = 0; row < query.Rows.Count; row++)
        {
            ddl.Items.Add(new ListItem(
                query.Rows[row]["Label"].ToString()
                , query.Rows[row]["ID"].ToString()
                ));
        }

        //被選擇值
        if (!string.IsNullOrWhiteSpace(inputValue))
        {
            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(inputValue));
        }

        query = null;
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
        string _sDateA = this.filter_sDateA.Text;
        string _eDateA = this.filter_eDateA.Text;
        string _sDateB = this.filter_sDateB.Text;
        string _eDateB = this.filter_eDateB.Text;
        string _ErpID = this.filter_ErpID.Text;
        string _ClsLv1 = this.filter_ClsLv1.SelectedValue;
        string _ClsLv2 = this.filter_ClsLv2.SelectedValue;

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}&page=1".FormatThis(thisPage));

        //[查詢條件] - DateA
        if (!string.IsNullOrWhiteSpace(_sDateA))
        {
            url.Append("&sDateA=" + Server.UrlEncode(_sDateA));
        }
        if (!string.IsNullOrWhiteSpace(_eDateA))
        {
            url.Append("&eDateA=" + Server.UrlEncode(_eDateA));
        }

        //[查詢條件] - DateB
        if (!string.IsNullOrWhiteSpace(_sDateB))
        {
            url.Append("&sDateB=" + Server.UrlEncode(_sDateB));
        }
        if (!string.IsNullOrWhiteSpace(_eDateB))
        {
            url.Append("&eDateB=" + Server.UrlEncode(_eDateB));
        }

        //[查詢條件] - ErpID
        if (!string.IsNullOrWhiteSpace(_ErpID))
        {
            url.Append("&ErpID=" + Server.UrlEncode(_ErpID));
        }

        //[查詢條件] - ClsLv1
        if (!string.IsNullOrWhiteSpace(_ClsLv1))
        {
            url.Append("&ClsLv1=" + Server.UrlEncode(_ClsLv1));
        }
        //[查詢條件] - ClsLv2
        if (!string.IsNullOrWhiteSpace(_ClsLv2))
        {
            url.Append("&ClsLv2=" + Server.UrlEncode(_ClsLv2));
        }

        return url.ToString();
    }

    #endregion


    #region -- 網址參數 --


    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myAsset/EqList.aspx".FormatThis(fn_Params.WebUrl);
    }

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
    /// 取得傳遞參數 - ErpID
    /// </summary>
    public string Req_ErpID
    {
        get
        {
            String _data = Request.QueryString["ErpID"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "20", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_ErpID = value;
        }
    }
    private string _Req_ErpID;

    /// <summary>
    /// 取得傳遞參數 - sDateA
    /// </summary>
    public string Req_sDateA
    {
        get
        {
            String _data = Request.QueryString["sDateA"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_sDateA = value;
        }
    }
    private string _Req_sDateA;

    /// <summary>
    /// 取得傳遞參數 - eDateA
    /// </summary>
    public string Req_eDateA
    {
        get
        {
            String _data = Request.QueryString["eDateA"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_eDateA = value;
        }
    }
    private string _Req_eDateA;

    /// <summary>
    /// 取得傳遞參數 - sDateB
    /// </summary>
    public string Req_sDateB
    {
        get
        {
            String _data = Request.QueryString["sDateB"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_sDateB = value;
        }
    }
    private string _Req_sDateB;

    /// <summary>
    /// 取得傳遞參數 - eDateB
    /// </summary>
    public string Req_eDateB
    {
        get
        {
            String _data = Request.QueryString["eDateB"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "10", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_eDateB = value;
        }
    }
    private string _Req_eDateB;

    /// <summary>
    /// 取得傳遞參數 - 類別1
    /// </summary>
    public string Req_Cls1
    {
        get
        {
            String _data = Request.QueryString["ClsLv1"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "4", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Cls1 = value;
        }
    }
    private string _Req_Cls1;

    /// <summary>
    /// 取得傳遞參數 - 類別2
    /// </summary>
    public string Req_Cls2
    {
        get
        {
            String _data = Request.QueryString["ClsLv2"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "4", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_Cls2 = value;
        }
    }
    private string _Req_Cls2;

    /// <summary>
    /// 取得傳遞參數 - DBS(必要參數)
    /// </summary>
    public string Req_DBS
    {
        get
        {
            String _data = Request.QueryString["dbs"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "3", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_DBS = value;
        }
    }
    private string _Req_DBS;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}?dbs={1}".FormatThis(FuncPath(), Req_DBS);
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;

    #endregion
}