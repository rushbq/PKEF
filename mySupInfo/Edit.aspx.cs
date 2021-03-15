using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Data.Models;
using PKLib_Method.Methods;

public partial class mySupInfo_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 檢查必要參數 **
                if (string.IsNullOrEmpty(Req_ds))
                {
                    Response.Redirect(Application["WebUrl"] + "Main.aspx");
                    return;
                }

                //[權限判斷] Start
                /* 
                 * 使用公司別代號，判斷對應的MENU ID
                 */
                bool isPass = false;
                switch (Req_ds)
                {
                    case "3":
                        //上海寶工
                        isPass = fn_CheckAuth.CheckAuth_User("446", out ErrMsg);
                        break;


                    case "2":
                        //深圳寶工
                        isPass = fn_CheckAuth.CheckAuth_User("447", out ErrMsg);
                        break;

                    default:
                        isPass = fn_CheckAuth.CheckAuth_User("445", out ErrMsg);
                        break;
                }

                if (!isPass)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}"
                        , Application["WebUrl"], HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //[權限判斷] End


                //基本資料
                LookupData_Base();

                //通訊人列表
                LookupData_Member();

                //通訊人修改
                if (!string.IsNullOrEmpty(Req_DT_DataID))
                {
                    View_Member(Req_DT_DataID);
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
    /// 取得基本資料
    /// </summary>
    private void LookupData_Base()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)Common.mySearch.DataID, Req_DataID);
        search.Add((int)Common.mySearch.Corp, Req_ds);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetERPDataList(search).Take(1);


        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            this.lt_CorpName.Text = " - {0}".FormatThis(item.Corp_Name);
            this.lt_SupID.Text = item.ERP_SupID;
            this.lt_SupName.Text = item.ERP_SupName;
            this.hf_CorpUID.Value = item.Corp_UID.ToString();
            this.hf_SupID.Value = item.ERP_SupID;

            string userAccount = item.User_Account;
            string userName = item.User_Name;
            this.AC_UserID.Text = string.IsNullOrEmpty(userAccount) ? "" : "({0}) {1}".FormatThis(userAccount, userName);
            this.Rel_UserID_Val.Text = userAccount;
            this.lb_UserName.Text = string.IsNullOrEmpty(userName) ? "未設定" : userName;
            this.hf_InfoID.Value = item.InfoID.ToString();

            //匯款資料
            lt_tw_AccName.Text = item.tw_AccName;
            lt_tw_Account.Text = item.tw_Account;
            lt_tw_BankName.Text = item.tw_BankName;
            lt_tw_BankID.Text = item.tw_BankID;
            tb_cn_Account.Text = item.cn_Account;
            tb_cn_AccName.Text = item.cn_AccName;
            tb_cn_Email.Text = item.cn_Email;
            tb_cn_BankName.Text = item.cn_BankName;
            tb_cn_BankID.Text = item.cn_BankID;
            tb_cn_SaleID.Text = item.cn_SaleID;
            tb_cn_State.Text = item.cn_State;
            tb_cn_City.Text = item.cn_City;
            tb_cn_BankType.Text = item.cn_BankType;
            tb_ww_Account.Text = item.ww_Account;
            tb_ww_AccName.Text = item.ww_AccName;
            tb_ww_Tel.Text = item.ww_Tel;
            tb_ww_Addr.Text = item.ww_Addr;
            tb_ww_BankName.Text = item.ww_BankName;
            tb_ww_BankBranch.Text = item.ww_BankBranch;
            tb_ww_BankAddr.Text = item.ww_BankAddr;
            tb_ww_Country.Text = item.ww_Country;
            tb_ww_Code.Text = item.ww_Code;
        }
    }

    #endregion


    #region -- 資料顯示:通訊人資料 --

    private void View_Member(string id)
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)myMember.DataID, id);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetMemberList(search).FirstOrDefault();


        //----- 資料整理:繫結 ----- 
        this.hf_DtID.Value = query.Data_ID.ToString();
        this.tb_FullName.Text = query.FullName;
        this.tb_NickName.Text = query.NickName;
        this.rbl_Gender.SelectedValue = query.Gender;
        this.tb_Phone.Text = query.Phone;
        this.tb_Birthday.Text = query.Birthday;
        this.tb_Email.Text = query.Email;
        this.rbl_IsSend.SelectedValue = query.IsSendOrder;

    }


    private void LookupData_Member()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)myMember.SupID, Req_DataID);
        search.Add((int)myMember.Corp, Req_ds);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetMemberList(search);


        //----- 資料整理:繫結 ----- 
        this.lv_Members.DataSource = query;
        this.lv_Members.DataBind();
    }

    protected void lv_Members_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            int Get_DataID = Convert.ToInt16(((HiddenField)e.Item.FindControl("hf_DataID")).Value);


            //----- 宣告:資料參數 -----
            SupplierRepository _data = new SupplierRepository();


            //----- 方法:刪除資料 -----
            if (false == _data.Delete_Member(Get_DataID))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#dataList");
            }

        }
    }

    #endregion


    #region -- 資料編輯:通訊錄 Start --
    /// <summary>
    /// 通訊錄資料存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(this.hf_DtID.Value))
            {
                Add_Data();
            }
            else
            {
                Edit_Data();
            }
        }
        catch (Exception)
        {
            throw;
        }

    }


    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();


        //----- 設定:資料欄位 -----
        var data = new Member_Data
        {
            Corp_UID = Convert.ToInt32(this.hf_CorpUID.Value),
            ERP_ID = this.hf_SupID.Value,
            Email = this.tb_Email.Text,
            FullName = this.tb_FullName.Text,
            NickName = this.tb_NickName.Text,
            Gender = this.rbl_Gender.SelectedValue,
            Birthday = this.tb_Birthday.Text,
            Phone = this.tb_Phone.Text,
            IsSendOrder = this.rbl_IsSend.SelectedValue,
            Create_Who = fn_Params.UserGuid
        };

        //----- 方法:新增資料 -----
        int GetID = _data.Create_Member(data);

        if (GetID.Equals(0))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl + "#dataList");
        }
    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();


        var data = new Member_Data
        {
            Data_ID = Convert.ToInt32(this.hf_DtID.Value),
            Corp_UID = Convert.ToInt32(this.hf_CorpUID.Value),
            ERP_ID = this.hf_SupID.Value,
            Email = this.tb_Email.Text,
            FullName = this.tb_FullName.Text,
            NickName = this.tb_NickName.Text,
            Gender = this.rbl_Gender.SelectedValue,
            Birthday = this.tb_Birthday.Text,
            Phone = this.tb_Phone.Text,
            IsSendOrder = this.rbl_IsSend.SelectedValue,
            Update_Who = fn_Params.UserGuid
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update_Member(data))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl + "#dataList");
        }

    }

    #endregion -- 資料編輯:通訊錄 End --


    #region -- 資料編輯:基本資料 Start --

    /// <summary>
    /// 基本資料存檔
    /// </summary>
    protected void btn_BaseSave_Click(object sender, EventArgs e)
    {
        try
        {
            string infoID = this.hf_InfoID.Value;
            string userID = this.Rel_UserID_Val.Text;
            if (string.IsNullOrEmpty(userID))
            {
                CustomExtension.AlertMsg("「採購人員」未設定", "");
                return;
            }

            //資料處理
            if (infoID.Equals("0"))
            {
                Add_InfoData();
            }
            else
            {
                Edit_InfoData();
            }

        }
        catch (Exception)
        {
            throw;
        }

    }

    /// <summary>
    /// 資料新增 - 擴充欄位
    /// </summary>
    private void Add_InfoData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();


        //----- 設定:資料欄位 -----
        var data = new ERP_Data
        {
            Corp_UID = Convert.ToInt16(Req_ds),
            ERP_SupID = this.hf_SupID.Value,
            User_Account = this.Rel_UserID_Val.Text,
            Create_Who = fn_Params.UserGuid
        };

        //----- 方法:新增資料 -----
        if (!_data.Create_Info(data))
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
    /// 資料修改 - 擴充欄位
    /// </summary>
    private void Edit_InfoData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();


        var data = new ERP_Data
        {
            InfoID = Convert.ToInt32(this.hf_InfoID.Value),
            User_Account = this.Rel_UserID_Val.Text,
            Update_Who = fn_Params.UserGuid
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update_Info(data))
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


    #endregion -- 資料編輯:基本資料 End --


    #region -- 資料編輯:中國匯款帳戶 Start --

    /// <summary>
    /// 存檔:中國匯款帳戶
    /// </summary>
    protected void btn_SaveBankCN_Click(object sender, EventArgs e)
    {
        try
        {
            string infoID = this.hf_InfoID.Value;

            //資料處理
            if (infoID.Equals("0"))
            {
                Add_BankData();
            }
            else
            {
                Edit_BankData();
            }

        }
        catch (Exception)
        {
            throw;
        }

    }

    /// <summary>
    /// 資料新增 - 擴充欄位
    /// </summary>
    private void Add_BankData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();

        string _cn_Account = tb_cn_Account.Text;
        string _cn_AccName = tb_cn_AccName.Text;
        string _cn_Email = tb_cn_Email.Text;
        string _cn_BankName = tb_cn_BankName.Text;
        string _cn_BankID = tb_cn_BankID.Text;
        string _cn_SaleID = tb_cn_SaleID.Text;
        string _cn_State = tb_cn_State.Text;
        string _cn_City = tb_cn_City.Text;
        string _cn_BankType = tb_cn_BankType.Text;
        string _ww_Account = tb_ww_Account.Text;
        string _ww_AccName = tb_ww_AccName.Text;
        string _ww_Tel = tb_ww_Tel.Text;
        string _ww_Addr = tb_ww_Addr.Text;
        string _ww_BankName = tb_ww_BankName.Text;
        string _ww_BankBranch = tb_ww_BankBranch.Text;
        string _ww_BankAddr = tb_ww_BankAddr.Text;
        string _ww_Country = tb_ww_Country.Text;
        string _ww_Code = tb_ww_Code.Text;

        //----- 設定:資料欄位 -----
        var data = new ERP_Data
        {
            Corp_UID = Convert.ToInt16(Req_ds),
            ERP_SupID = this.hf_SupID.Value,
            User_Account = this.Rel_UserID_Val.Text,
            Create_Who = fn_Params.UserGuid,
            cn_Account = _cn_Account,
            cn_AccName = _cn_AccName,
            cn_Email = _cn_Email,
            cn_BankName = _cn_BankName,
            cn_BankID = _cn_BankID,
            cn_SaleID = _cn_SaleID,
            cn_State = _cn_State,
            cn_City = _cn_City,
            cn_BankType = _cn_BankType,
            ww_Account = _ww_Account,
            ww_AccName = _ww_AccName,
            ww_Tel = _ww_Tel,
            ww_Addr = _ww_Addr,
            ww_BankName = _ww_BankName,
            ww_BankBranch = _ww_BankBranch,
            ww_BankAddr = _ww_BankAddr,
            ww_Country = _ww_Country,
            ww_Code = _ww_Code
        };

        //----- 方法:新增資料 -----
        if (!_data.Create_Info(data))
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
    /// 資料修改 - 擴充欄位
    /// </summary>
    private void Edit_BankData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();

        string _cn_Account = tb_cn_Account.Text;
        string _cn_AccName = tb_cn_AccName.Text;
        string _cn_Email = tb_cn_Email.Text;
        string _cn_BankName = tb_cn_BankName.Text;
        string _cn_BankID = tb_cn_BankID.Text;
        string _cn_SaleID = tb_cn_SaleID.Text;
        string _cn_State = tb_cn_State.Text;
        string _cn_City = tb_cn_City.Text;
        string _cn_BankType = tb_cn_BankType.Text;
        string _ww_Account = tb_ww_Account.Text;
        string _ww_AccName = tb_ww_AccName.Text;
        string _ww_Tel = tb_ww_Tel.Text;
        string _ww_Addr = tb_ww_Addr.Text;
        string _ww_BankName = tb_ww_BankName.Text;
        string _ww_BankBranch = tb_ww_BankBranch.Text;
        string _ww_BankAddr = tb_ww_BankAddr.Text;
        string _ww_Country = tb_ww_Country.Text;
        string _ww_Code = tb_ww_Code.Text;

        var data = new ERP_Data
        {
            InfoID = Convert.ToInt32(this.hf_InfoID.Value),
            User_Account = this.Rel_UserID_Val.Text,
            Update_Who = fn_Params.UserGuid,

            cn_Account = _cn_Account,
            cn_AccName = _cn_AccName,
            cn_Email = _cn_Email,
            cn_BankName = _cn_BankName,
            cn_BankID = _cn_BankID,
            cn_SaleID = _cn_SaleID,
            cn_State = _cn_State,
            cn_City = _cn_City,
            cn_BankType = _cn_BankType,
            ww_Account = _ww_Account,
            ww_AccName = _ww_AccName,
            ww_Tel = _ww_Tel,
            ww_Addr = _ww_Addr,
            ww_BankName = _ww_BankName,
            ww_BankBranch = _ww_BankBranch,
            ww_BankAddr = _ww_BankAddr,
            ww_Country = _ww_Country,
            ww_Code = _ww_Code
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update_Info(data))
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


    #endregion -- 資料編輯:中國匯款帳戶 End --


    #region -- 資料編輯:境外匯款帳戶 Start --

    /// <summary>
    /// 存檔:境外匯款帳戶
    /// </summary>
    protected void btn_SaveBankWorld_Click(object sender, EventArgs e)
    {
        try
        {
            try
            {
                string infoID = this.hf_InfoID.Value;

                //資料處理
                if (infoID.Equals("0"))
                {
                    Add_BankData();
                }
                else
                {
                    Edit_BankData();
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        catch (Exception)
        {
            throw;
        }

    }


    #endregion -- 資料編輯:境外匯款帳戶 End --


    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}mySupInfo/Edit.aspx?ds={1}&id={2}".FormatThis(
                Application["WebUrl"]
                , Req_ds
                , Req_DataID
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
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}mySupInfo/Search.aspx?ds={1}".FormatThis(Application["WebUrl"], Req_ds);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    private string _Page_SearchUrl;


    /// <summary>
    /// 取得傳遞參數 - 主檔-資料編號
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["id"];
            return string.IsNullOrEmpty(data) ? "" : data.Trim();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;


    /// <summary>
    /// 取得傳遞參數 - ds, 公司別UID
    /// </summary>
    public string Req_ds
    {
        get
        {
            String data = Request.QueryString["ds"];
            return (CustomExtension.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_ds = value;
        }
    }
    private string _Req_ds;


    /// <summary>
    /// 取得傳遞參數 - 明細檔-資料編號
    /// </summary>
    public string Req_DT_DataID
    {
        get
        {
            String data = Request.QueryString["dtid"];
            return string.IsNullOrEmpty(data) ? "" : data.Trim();
        }
        set
        {
            this._Req_DT_DataID = value;
        }
    }
    private string _Req_DT_DataID;


    #endregion
}