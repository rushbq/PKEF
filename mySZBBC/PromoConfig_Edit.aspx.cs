using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using SZ_BBC.Controllers;
using SZ_BBC.Models;

public partial class mySZBBC_PromoConfig_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("820", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //Get Class
                Get_ClassList(myClass.mall, ddl_Mall, "選擇商城");

                //[參數判斷] - 資料編號
                if (string.IsNullOrWhiteSpace(Req_DataID))
                {
                    lb_Msg.Visible = true;
                    btn_SaveDetail.Visible = false;
                    ph_Btn.Visible = false;
                }
                else
                {
                    lb_Msg.Visible = false;
                    btn_SaveDetail.Visible = true;
                    ph_Btn.Visible = true;

                    //載入資料
                    LookupData();
                }

            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示:基本資料 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----
        search.Add("DataID", Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetPromoConfig(search, out ErrMsg).Take(1).FirstOrDefault();

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            //this.ph_ErrMessage.Visible = true;
            //this.ph_Data.Visible = false;
            //this.lt_ShowMsg.Text = "無法取得資料";
            CustomExtension.AlertMsg("無法取得資料", Page_SearchUrl);
            return;
        }


        //填入資料
        lt_DataID.Text = query.Data_ID.ToString();
        hf_DataID.Value = query.Data_ID.ToString();
        tb_PromoName.Text = query.PromoName;
        tb_sDate.Text = query.StartTime;
        tb_eDate.Text = query.EndTime;
        ddl_Mall.SelectedValue = query.MallID.ToString();
        short _type = query.PromoType;
        string _money = query.TargetMoney == null ? "" : query.TargetMoney.ToString();
        string _item = query.TargetItem;
        switch (_type)
        {
            case 1:
                rb_Type1.Checked = _type.Equals(1);
                tb_TargetMoney.Text = _money;
                break;

            case 2:
                rb_Type2.Checked = _type.Equals(2);
                tb_TargetItem.Text = _item;
                val_TargetItem.Text = _item;
                break;
        }

        //維護資訊
        this.info_Creater.Text = query.Create_Name;
        this.info_CreateTime.Text = query.Create_Time;
        this.info_Updater.Text = query.Update_Name;
        this.info_UpdateTime.Text = query.Update_Time;


        //-- 載入其他資料 --
        LookupData_Detail();

    }


    /// <summary>
    /// 取得類別
    /// </summary>
    /// <param name="cls"></param>
    /// <param name="ddl"></param>
    /// <param name="rootName"></param>
    /// <param name="inputValue"></param>
    private void Get_ClassList(myClass cls, DropDownList ddl, string rootName)
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls);


        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }


        query = null;
    }
    #endregion


    #region -- 資料編輯:基本資料 --

    //SAVE-基本資料
    protected void btn_SaveStay_Click(object sender, EventArgs e)
    {
        doSave("1");
    }
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        doSave("2");
    }

    private void doSave(string type)
    {
        string errTxt = "";
        string sDate = this.tb_sDate.Text;
        string eDate = this.tb_eDate.Text;

        if (string.IsNullOrWhiteSpace(this.tb_PromoName.Text))
        {
            errTxt += "活動名稱空白\\n";
        }
        if (string.IsNullOrWhiteSpace(sDate) || string.IsNullOrWhiteSpace(eDate))
        {
            errTxt += "活動時間空白\\n";
        }
        else
        {
            if (Convert.ToDateTime(sDate.ToDateString("yyyy/MM/dd HH:mm")) >= Convert.ToDateTime(eDate.ToDateString("yyyy/MM/dd HH:mm")))
            {
                errTxt += "請填寫正確的「活動時間」\\n";
            }
        }

        if (this.ddl_Mall.SelectedIndex == 0)
        {
            errTxt += "請選擇「對應商城」\\n";
        }
        
        #region ** 活動類型欄位檢查 **

        if (!rb_Type1.Checked && !rb_Type2.Checked)
        {
            errTxt += "請選擇「活動類型」\\n";
        }
        if (rb_Type1.Checked)
        {
            string _money = tb_TargetMoney.Text;
            if (string.IsNullOrWhiteSpace(_money))
            {
                errTxt += "請填寫「金額」\\n";
            }
            if (!CustomExtension.IsNumeric(_money))
            {
                errTxt += "請填寫正確的「金額」\\n";
            }
        }
        if (rb_Type2.Checked)
        {
            string _item = val_TargetItem.Text;
            if (string.IsNullOrWhiteSpace(_item))
            {
                errTxt += "請填寫正確的「品號」\\n";
            }
        }

        #endregion


        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        /* 執行新增/更新 */
        if (string.IsNullOrEmpty(this.hf_DataID.Value))
        {
            Add_Data();
        }
        else
        {
            Edit_Data(type);
        }
    }


    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        string _promoName = tb_PromoName.Text.Left(50);
        Int32 _mallID = Convert.ToInt32(ddl_Mall.SelectedValue);
        string _sDate = tb_sDate.Text.ToDateString("yyyy/MM/dd HH:mm");
        string _eDate = tb_eDate.Text.ToDateString("yyyy/MM/dd HH:mm");
        Int32 _promoType = rb_Type1.Checked ? 1 : rb_Type2.Checked ? 2 : 0;
        double _money = rb_Type1.Checked ? Convert.ToDouble(tb_TargetMoney.Text) : 0;
        string _item = rb_Type2.Checked ? val_TargetItem.Text : "";

        //----- 設定:資料欄位 -----
        //產生Guid
        string guid = CustomExtension.GetGuid();

        var data = new PromoBase
        {
            Data_ID = new Guid(guid),
            PromoName = _promoName,
            MallID = _mallID,
            StartTime = _sDate,
            EndTime = _eDate,
            PromoType = Convert.ToInt16(_promoType),
            TargetMoney = _money,
            TargetItem = _item,
            Create_Who = fn_Params.UserGuid


        };

        //----- 方法:新增資料 -----
        if (!_data.Create_Promo(data, out ErrMsg))
        {
            //Response.Write(ErrMsg);
            CustomExtension.AlertMsg("新增失敗", "");
            return;
        }
        else
        {
            //更新本頁Url
            string thisUrl = "{0}PromoConfig_Edit.aspx?id={1}".FormatThis(FuncPath(), guid);

            //導向本頁
            Response.Redirect(thisUrl);
        }
    }


    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data(string type)
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        string _promoName = tb_PromoName.Text.Left(50);
        Int32 _mallID = Convert.ToInt32(ddl_Mall.SelectedValue);
        string _sDate = tb_sDate.Text.ToDateString("yyyy/MM/dd HH:mm");
        string _eDate = tb_eDate.Text.ToDateString("yyyy/MM/dd HH:mm");
        Int32 _promoType = rb_Type1.Checked ? 1 : rb_Type2.Checked ? 2 : 0;
        double _money = rb_Type1.Checked ? Convert.ToDouble(tb_TargetMoney.Text) : 0;
        string _item = rb_Type2.Checked ? val_TargetItem.Text : "";

        //----- 設定:資料欄位 -----
        var data = new PromoBase
        {
            Data_ID = new Guid(Req_DataID),
            PromoName = _promoName,
            MallID = _mallID,
            StartTime = _sDate,
            EndTime = _eDate,
            PromoType = Convert.ToInt16(_promoType),
            TargetMoney = _money,
            TargetItem = _item,
            Update_Who = fn_Params.UserGuid
        };

        //----- 方法:更新資料 -----
        if (!_data.Update_PromoData(data, out ErrMsg))
        {
            Response.Write(ErrMsg);
            CustomExtension.AlertMsg("更新失敗", thisPage);
            return;
        }
        else
        {
            if (type.Equals("1"))
            {
                //導向本頁
                Response.Redirect(thisPage);
            }
            else
            {
                //導向列表頁
                Response.Redirect(Page_SearchUrl);
            }
        }

    }


    #endregion


    #region -- 資料顯示:贈品清單 --

    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetPromoConfigDetail(Req_DataID, out ErrMsg);

        //----- 資料整理:繫結 ----- 
        this.lvDetailList.DataSource = query;
        this.lvDetailList.DataBind();
    }

    protected void lvDetailList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string dataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            switch (e.CommandName.ToUpper())
            {
                case "DOCLOSE":
                    //----- 宣告:資料參數 -----
                    SZBBCRepository _data = new SZBBCRepository();

                    //----- 方法:刪除資料 -----
                    if (false == _data.Delete_PromoDT(Req_DataID, dataID, out ErrMsg))
                    {
                        _data = null;

                        CustomExtension.AlertMsg("刪除失敗", "");
                        return;
                    }
                    else
                    {
                        _data = null;
                        //導向本頁
                        Response.Redirect(thisPage + "#DTList");
                    }

                    break;
            }
        }
    }


    #endregion


    #region -- 資料編輯:贈品清單 --

    //SAVE-贈品清單
    protected void btn_SaveDetail_Click(object sender, EventArgs e)
    {
        string errTxt = "";

        if (string.IsNullOrWhiteSpace(this.val_ModelNo.Text))
        {
            errTxt += "品號空白\\n";
        }
        if (string.IsNullOrWhiteSpace(this.tb_Qty.Text))
        {
            errTxt += "數量空白\\n";
        }
        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        //----- 設定:資料欄位 -----
        var data = new PromoDT
        {
            ModelNo = this.val_ModelNo.Text,
            Qty = Convert.ToInt32(this.tb_Qty.Text)
        };

        //----- 方法:建立資料 -----
        if (!_data.Create_PromoDT(data, Req_DataID, fn_Params.UserGuid, out ErrMsg))
        {
            CustomExtension.AlertMsg("單身資料建立失敗", thisPage);
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(thisPage + "#DTList");
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
        return "{0}mySZBBC/".FormatThis(
            fn_Params.WebUrl);
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
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}PromoConfig_Edit.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            this._thisPage = value;
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
            string tempUrl = CustomExtension.getCookie("EF_PromoCfg");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "PromoConfig.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

    #endregion

}