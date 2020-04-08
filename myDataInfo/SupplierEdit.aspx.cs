using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Data.Models;

public partial class myDataInfo_SupplierEdit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("440", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Data.Visible = false;

                    //自動新增新增
                    Add_Data();
                }
                else
                {
                    this.ph_Data.Visible = true;

                    //取得選單
                    Get_CorpList(this.filter_Corp, "選擇公司別");

                    //[取得/檢查參數] - Keyword
                    if (!string.IsNullOrEmpty(Req_Keyword))
                    {
                        this.filter_Keyword.Text = Req_Keyword;
                    }
                    //[取得/檢查參數] - Corp
                    if (!string.IsNullOrEmpty(Req_Corp))
                    {
                        this.filter_Corp.SelectedValue = Req_Corp;
                    }
                    //[取得/檢查參數] - do Search
                    if (!string.IsNullOrEmpty(Req_Search))
                    {
                        //載入未關聯列表
                        LookupData_UnRel();
                    }

                    //載入資料
                    LookupData();
                    LookupData_Rel();
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --
    /// <summary>
    /// 取得資料 - 公司別
    /// </summary>
    /// <param name="ddl">下拉選單object</param>
    /// <param name="rootName">第一選項顯示名稱</param>
    private void Get_CorpList(DropDownList ddl, string rootName)
    {
        //----- 宣告:資料參數 -----
        ParamsRepository _data = new ParamsRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetCorpList(null);

        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Corp_Name, item.Corp_UID.ToString()));
        }

        query = null;
    }

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)Common.mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1);

        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            this.lt_Sup_UID.Text = item.Sup_UID.ToString();
            this.tb_Sup_Name.Text = item.Sup_Name;
            this.hf_DataID.Value = item.Sup_UID.ToString();

            //維護資訊
            this.lt_Creater.Text = item.Create_Name;
            this.lt_CreateTime.Text = item.Create_Time;
            this.lt_Updater.Text = item.Update_Name;
            this.lt_UpdateTime.Text = item.Update_Time;
        }
    }


    #endregion


    #region -- 資料顯示:關聯設定 --

    private void LookupData_UnRel()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        #region >> 條件篩選 <<

        //[取得/檢查參數] - Corp
        if (!string.IsNullOrEmpty(Req_Corp))
        {
            search.Add((int)Common.mySearch.DataID, Req_Corp);
        }

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)Common.mySearch.Keyword, Req_Keyword);
        }

        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetUnRelList(Req_DataID, search).Take(50);


        //----- 資料整理:繫結 ----- 
        this.lv_Supplier.DataSource = query;
        this.lv_Supplier.DataBind();
    }

    protected void lv_Supplier_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            int Get_Corp_UID = Convert.ToInt16(((HiddenField)e.Item.FindControl("hf_Corp_UID")).Value);
            string Get_ERP_SupID = ((HiddenField)e.Item.FindControl("hf_ERP_SupID")).Value;
            string Get_ERP_SupName = ((HiddenField)e.Item.FindControl("hf_ERP_SupName")).Value;


            //----- 宣告:資料參數 -----
            SupplierRepository _data = new SupplierRepository();


            //----- 設定:資料欄位 -----
            var data = new Rel_Data
            {
                Sup_UID = Convert.ToInt32(this.hf_DataID.Value),
                Corp_UID = Get_Corp_UID,
                ERP_SupID = Get_ERP_SupID,
                ERP_SupName = Get_ERP_SupName
            };

            //----- 方法:新增資料 -----
            if (false == _data.Create_RelData(data))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#dataRel");
            }

        }
    }

    #endregion


    #region -- 資料顯示:已關聯列表 --

    private void LookupData_Rel()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetRelList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_RelData.DataSource = query;
        this.lv_RelData.DataBind();
    }

    protected void lv_RelData_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            int Get_DataID = Convert.ToInt16(((HiddenField)e.Item.FindControl("hf_DataID")).Value);


            //----- 宣告:資料參數 -----
            SupplierRepository _data = new SupplierRepository();


            //----- 方法:刪除資料 -----
            if (false == _data.Delete_RelData(Get_DataID))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#supplierRel");
            }

        }
    }

    #endregion

    #region -- 資料編輯 Start --
    /// <summary>
    /// 資料存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            Edit_Data();
        }
        catch (Exception)
        {
            throw;
        }

    }

    /// <summary>
    /// 資料自動新增
    /// </summary>
    private void Add_Data()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();


        //----- 設定:資料欄位 -----
        var data = new Supplier
        {
            Sup_Name = "新的顯示名稱#{0}".FormatThis(DateTime.Now.ToString().ToDateString("yyyyMMddHHmm")),
            Create_Who = fn_Params.UserGuid
        };

        //----- 方法:新增資料 -----
        int GetID = _data.Create(data);

        if (GetID.Equals(0))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //更新本頁Url
            string thisUrl =
                "{0}myDataInfo/SupplierEdit.aspx?DataID={1}".FormatThis(
                    Application["WebUrl"]
                    , Cryptograph.MD5Encrypt(GetID.ToString(), DesKey)
                    );

            //導向本頁
            Response.Redirect(thisUrl);
        }
    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();


        //----- 設定:資料欄位 -----
        var data = new Supplier
        {
            Sup_UID = Convert.ToInt32(this.hf_DataID.Value),
            Sup_Name = this.tb_Sup_Name.Text,
            Update_Who = fn_Params.UserGuid
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update(data))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl);
        }

    }



    /// <summary>
    /// 刪除資料
    /// </summary>
    protected void lbtn_DelData_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();

        //----- 方法:更新資料 -----
        int DataID = Convert.ToInt32(this.hf_DataID.Value);
        if (false == _data.Delete(DataID))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向至列表頁
            Response.Redirect(Page_SearchUrl);
        }
    }
    #endregion -- 資料編輯 End --


    #region -- 按鈕事件 --

    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        StringBuilder url = new StringBuilder();

        url.Append("{0}myDataInfo/SupplierEdit.aspx?DataID={1}&srh=1".FormatThis(
            Application["WebUrl"]
            , Cryptograph.MD5Encrypt(Req_DataID, DesKey)
            ));

        //[查詢條件] - 公司別
        string corp = this.filter_Corp.SelectedValue;
        if (!string.IsNullOrEmpty(corp))
        {
            url.Append("&Corp=" + Server.UrlEncode(corp));
        }

        //[查詢條件] - 關鍵字
        string keyword = this.filter_Keyword.Text;
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
        }

        //執行轉頁
        Response.Redirect(url.ToString() + "#dataRel", false);
    }


    #endregion

    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}myDataInfo/SupplierEdit.aspx?DataID={1}{2}{3}{4}".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Req_DataID, DesKey)
                , string.IsNullOrEmpty(Req_Corp) ? "" : "&Corp=" + Server.UrlEncode(Req_Corp)
                , string.IsNullOrEmpty(Req_Keyword) ? "" : "&Keyword=" + Server.UrlEncode(Req_Keyword)
                , Req_Search.Equals("1") ? "&srh=1" : ""
                );
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("SupRel");

            return string.IsNullOrWhiteSpace(tempUrl) ? "SupplierList.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    private string _Page_SearchUrl;


    /// <summary>
    /// 取得參數 - 資料編號
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : Cryptograph.MD5Decrypt(data.ToString(), DesKey);
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;


    /// <summary>
    /// 取得參數 - 公司別
    /// </summary>
    /// <remarks>for 關聯設定</remarks>
    public string Req_Corp
    {
        get
        {
            String data = Request.QueryString["Corp"];
            return data;
        }
        set
        {
            this._Req_Corp = value;
        }
    }
    private string _Req_Corp;


    /// <summary>
    /// 取得參數 - Keyword
    /// </summary>
    /// <remarks>for 關聯設定</remarks>
    public string Req_Keyword
    {
        get
        {
            String data = Request.QueryString["Keyword"];
            return (CustomExtension.String_資料長度Byte(data, "1", "50", out ErrMsg)) ? data.Trim() : ""; ;
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    private string _Req_Keyword;

    /// <summary>
    /// 取得參數 - 是否查詢
    /// </summary>
    /// <remarks>for 關聯設定</remarks>
    public string Req_Search
    {
        get
        {
            String data = Request.QueryString["srh"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Search = value;
        }
    }
    private string _Req_Search;

    /// <summary>
    /// Deskey
    /// </summary>
    public string DesKey
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
        }
        set
        {
            this._DesKey = value;
        }
    }
    private string _DesKey;


    #endregion
}