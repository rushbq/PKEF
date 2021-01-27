using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AssetData.Controllers;
using AssetData.Models;
using PKLib_Method.Methods;


public partial class myAsset_EqEdit : SecurityIn
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
                Get_ClassList("A", ddl_ClsLv1, "請選擇", "");
                Get_ClassList("B", ddl_ClsLv2, "請選擇", "");


                //[參數判斷] - 資料編號
                if (Req_DataID.Equals("new"))
                {
                    ph_Details.Visible = false;
                }
                else
                {
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
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            //----- 原始資料:條件篩選 -----
            search.Add("DataID", Req_DataID);

            //----- 原始資料:取得所有資料 -----
            IEnumerable<DataRow> query = (_data.GetOne_AssetList(search, Req_DBS, out ErrMsg)).AsEnumerable();

            //----- 資料整理:繫結 ----- 
            if (query == null)
            {
                CustomExtension.AlertMsg("無法取得資料,即將返回列表頁.", Page_SearchUrl);
                return;
            }

            foreach (DataRow item in query)
            {
                #region >> 欄位填寫 <<

                //--- 填入基本資料 ---
                lt_SeqNo.Text = item.Field<Int32>("SeqNo").ToString();
                hf_DataID.Value = item.Field<Guid>("Data_ID").ToString();
                ddl_ClsLv1.SelectedValue = item.Field<Int32>("ClassLv1").ToString();
                ddl_ClsLv2.SelectedValue = item.Field<Int32>("ClassLv2").ToString();
                tb_AName.Text = item.Field<string>("AName").ToString();
                tb_OnlineDate.Text = item.Field<string>("OnlineDate").ToString();
                tb_StartDate.Text = item.Field<string>("StartDate").ToString();
                tb_EndDate.Text = item.Field<string>("EndDate").ToString();
                tb_IPAddr.Text = item.Field<string>("IPAddr").ToString();
                tb_WebUrl.Text = item.Field<string>("WebUrl").ToString();
                tb_Remark.Text = item.Field<string>("Remark").ToString();

                //維護資訊
                info_Creater.Text = item.Field<string>("Create_Name").ToString();
                info_CreateTime.Text = item.Field<string>("Create_Time").ToString().ToDateString("yyyy-MM-dd HH:mm");
                info_Updater.Text = item.Field<string>("Update_Name").ToString();
                info_UpdateTime.Text = item.Field<string>("Update_Time").ToString().ToDateString("yyyy-MM-dd HH:mm");

                #endregion

                ph_Details.Visible = true;
                lt_SaveBase.Text = "修改基本資料";
            }


            #region >> 其他功能 <<

            //-- 載入其他資料 --
            LookupData_DTList();

            #endregion

        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            //Release
            _data = null;
        }

    }

    /// <summary>
    /// 取得類別
    /// </summary>
    /// <param name="clsType">A,B</param>
    /// <param name="ddl">控制項</param>
    /// <param name="rootName">根目錄名稱</param>
    /// <param name="inputValue">輸入值</param>
    private void Get_ClassList(string clsType, DropDownList ddl, string rootName, string inputValue)
    {
        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        try
        {
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


    #region -- 資料編輯:基本資料 --
    /// <summary>
    /// SAVE-基本資料
    /// </summary>
    protected void btn_doSaveBase_Click(object sender, EventArgs e)
    {
        string errTxt = "";

        if (string.IsNullOrWhiteSpace(tb_AName.Text))
        {
            errTxt += "名稱空白\\n";
        }
        if (ddl_ClsLv1.SelectedIndex == 0 || ddl_ClsLv2.SelectedIndex == 0)
        {
            errTxt += "類別或用途未選擇\\n";
        }

        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        /* 執行新增/更新 */
        if (string.IsNullOrEmpty(hf_DataID.Value))
        {
            Add_Data();
        }
        else
        {
            Edit_Data();
        }
    }

    /// <summary>
    /// 副程式-資料新增
    /// </summary>
    private void Add_Data()
    {
        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        try
        {
            //----- 設定:資料欄位 -----
            string _guid = "";
            string _AName = tb_AName.Text.Trim();
            string _ClassLv1 = ddl_ClsLv1.SelectedValue;
            string _ClassLv2 = ddl_ClsLv2.SelectedValue;
            string _OnlineDate = tb_OnlineDate.Text;
            string _StartDate = tb_StartDate.Text;
            string _EndDate = tb_EndDate.Text;
            string _IPAddr = tb_IPAddr.Text;
            string _WebUrl = tb_WebUrl.Text;
            string _Remark = tb_Remark.Text;

            var data = new AssetBase
            {
                ClassLv1 = _ClassLv1,
                ClassLv2 = _ClassLv2,
                AName = _AName,
                OnlineDate = _OnlineDate.ToDateString("yyyy/MM/dd"),
                StartDate = _StartDate.ToDateString("yyyy/MM/dd"),
                EndDate = _EndDate.ToDateString("yyyy/MM/dd"),
                IPAddr = _IPAddr,
                WebUrl = _WebUrl,
                Remark = _Remark
            };

            //----- 方法:新增資料 -----
            _guid = _data.Create_AssetBase(Req_DBS, data, out ErrMsg);
            if (string.IsNullOrWhiteSpace(_guid))
            {
                lt_ShowMsg.Text = ErrMsg;
                ph_ErrMessage.Visible = true;
                CustomExtension.AlertMsg("新增失敗", "");
                return;
            }
            else
            {
                //更新本頁Url
                string thisNewUrl = "{0}myAsset/EqEdit.aspx?dbs={1}&id={2}".FormatThis(fn_Params.WebUrl, Req_DBS, _guid);

                //導向本頁
                Response.Redirect(thisNewUrl);
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
    /// 副程式-資料修改
    /// </summary>
    private void Edit_Data()
    {
        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        try
        {
            //----- 設定:資料欄位 -----
            string _id = hf_DataID.Value;
            string _AName = tb_AName.Text.Trim();
            string _ClassLv1 = ddl_ClsLv1.SelectedValue;
            string _ClassLv2 = ddl_ClsLv2.SelectedValue;
            string _OnlineDate = tb_OnlineDate.Text;
            string _StartDate = tb_StartDate.Text;
            string _EndDate = tb_EndDate.Text;
            string _IPAddr = tb_IPAddr.Text;
            string _WebUrl = tb_WebUrl.Text;
            string _Remark = tb_Remark.Text;

            var data = new AssetBase
            {
                ClassLv1 = _ClassLv1,
                ClassLv2 = _ClassLv2,
                AName = _AName,
                OnlineDate = _OnlineDate.ToDateString("yyyy/MM/dd"),
                StartDate = _StartDate.ToDateString("yyyy/MM/dd"),
                EndDate = _EndDate.ToDateString("yyyy/MM/dd"),
                IPAddr = _IPAddr,
                WebUrl = _WebUrl,
                Remark = _Remark
            };

            //----- 方法:更新資料 -----
            if (!_data.Update_AssetBase(_id, data, out ErrMsg))
            {
                lt_ShowMsg.Text = ErrMsg;
                ph_ErrMessage.Visible = true;
                CustomExtension.AlertMsg("更新失敗", "");
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
    }

    #endregion


    #region -- 資料顯示:資產清單 --

    /// <summary>
    /// 顯示資產清單
    /// </summary>
    private void LookupData_DTList()
    {
        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        try
        {
            //----- 原始資料:取得所有資料 -----
            var query = _data.GetDT_Asset(Req_DataID, out ErrMsg);

            //----- 資料整理:繫結 ----- 
            lv_Detail.DataSource = query;
            lv_Detail.DataBind();

            query = null;
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            //Release
            _data = null;
        }

    }

    protected void lv_Detail_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_ID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            //----- 宣告:資料參數 -----
            AssetRepository _data = new AssetRepository();
            try
            {
                //----- 方法:刪除資料 -----
                if (false == _data.Delete_AssetItem(Req_DataID, Get_ID, out ErrMsg))
                {
                    CustomExtension.AlertMsg("資料刪除失敗", "");
                    return;
                }

                //導向本頁
                Response.Redirect(thisPage + "#section2");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //Release
                _data = null;
            }

        }
    }

    #endregion


    #region -- 資料編輯:資產清單 --

    //SAVE-贈品清單
    protected void btn_SaveDetail_Click(object sender, EventArgs e)
    {
        string errTxt = "";
        string _assetVal = val_AssetVal.Text.Trim();
        string _dataID = hf_DataID.Value;

        if (string.IsNullOrWhiteSpace(_assetVal))
        {
            errTxt += "資產編號空白\\n";
        }
        //alert
        if (!string.IsNullOrEmpty(errTxt))
        {
            CustomExtension.AlertMsg(errTxt, "");
            return;
        }

        //----- 宣告:資料參數 -----
        AssetRepository _data = new AssetRepository();

        try
        {
            //----- 方法:建立資料 -----
            if (!_data.Create_AssetItem(_dataID, _assetVal, out ErrMsg))
            {
                CustomExtension.AlertMsg("單身資料建立失敗", thisPage);
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(thisPage + "#section1");
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


    #region -- 傳遞參數 --
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
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String _data = Request.QueryString["id"];

            return string.IsNullOrWhiteSpace(_data) ? "new" : _data.Trim();
        }
        set
        {
            _Req_DataID = value;
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
            //string tempUrl = CustomExtension.getCookie("SupInvCheckA");

            //return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/Set" : Server.UrlDecode(tempUrl);
            return "{0}myAsset/EqList.aspx?dbs={1}".FormatThis(fn_Params.WebUrl, Req_DBS);
        }
        set
        {
            _Page_SearchUrl = value;
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
            return "{0}myAsset/EqEdit.aspx?dbs={1}&id={2}".FormatThis(fn_Params.WebUrl, Req_DBS, Req_DataID);
        }
        set
        {
            _thisPage = value;
        }
    }

    #endregion
}