using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections;

public partial class Sales_RelCust_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 人員關聯客戶 - 編輯
                if (fn_CheckAuth.CheckAuth_User("141", out ErrMsg) == false && fn_CheckAuth.CheckAuth_User("142", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無權限使用本功能！", "script:parent.$.fancybox.close()");
                    return;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");


                //[參數判斷] - StaffID
                if (string.IsNullOrEmpty(Param_thisID))
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                    return;
                }
                else
                {
                    View_Data();
                }

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 讀取資料
    /// </summary>
    private void View_Data()
    {
        try
        {
            string ErrMsg;

            //[取得/檢查參數] - 系統編號
            if (fn_Extensions.Num_正整數(Param_thisID, "1", "999999999", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 人員資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ERP_UserID, Display_Name ");
                SBSql.AppendLine(" FROM User_Profile ");
                SBSql.AppendLine(" WHERE (Account_Name = @StaffID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("StaffID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無此人員！", "script:parent.$.fancybox.close()");
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.lt_Display_Name.Text = string.Format("{0} - {1}"
                            , DT.Rows[0]["ERP_UserID"].ToString()
                            , DT.Rows[0]["Display_Name"].ToString());
                    }
                }
            }

            //[取得資料] - 關聯資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Customer.MA001 AS CustID, Customer.MA002 AS CustName ");
                SBSql.AppendLine(" FROM Customer ");
                SBSql.AppendLine("  INNER JOIN Staff_Rel_Customer Rel ON Customer.MA001 = Rel.CustID AND Customer.DBC = Customer.DBS ");
                SBSql.AppendLine(" WHERE (Rel.StaffID = @StaffID) ");
                SBSql.AppendLine(" ORDER BY Customer.MA001 ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("StaffID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    //填入資料
                    StringBuilder html = new StringBuilder();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        html.AppendLine("<li id=\"li_" + row + "\" class=\"as-selection-item blur\">");
                        html.Append(string.Format("({0}) {1}"
                            , DT.Rows[row]["CustID"].ToString()
                            , DT.Rows[row]["CustName"].ToString()));
                        html.Append("<input type=\"text\" class=\"Item_Val\" value=\"" + DT.Rows[row]["CustID"].ToString() + "\" style=\"display:none\" />");
                        html.AppendLine("<a style=\"background:transparent\" href=\"javascript:Delete_Item('" + row + "');\"><span class=\"JQ-ui-icon ui-icon-trash\"></span></a>");
                        html.AppendLine("</li>");
                    }

                    this.lt_Items.Text = html.ToString();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 讀取資料");
        }
    }


    #endregion

    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;
            string inputValue = Filter_Value(this.tb_Cust_Item_Val.Text);
            //[欄位檢查]
            if (string.IsNullOrEmpty(inputValue))
            {
                fn_Extensions.JsAlert("客戶清單空白，無法存檔\\n若要刪除全部資料，請在列表頁按下「移除」", "");
                return;
            }

            //[資料儲存]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //--- 清空原設定 ---
                SBSql.AppendLine(" DELETE FROM Staff_Rel_Customer WHERE (StaffID = @StaffID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("StaffID", Param_thisID);
                if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    fn_Extensions.JsAlert("存檔發生錯誤", "");
                    return;
                }

                //--- 開始新增資料 ---
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();
                //[SQL] - 資料新增
                string[] strAry = Regex.Split(inputValue, @"\|{4}");
                var query = from el in strAry
                            select new
                            {
                                Val = el.ToString().Trim()
                            };
                int row = 0;
                foreach (var item in query)
                {
                    row++;
                    SBSql.AppendLine(" INSERT INTO Staff_Rel_Customer(StaffID, CustID) ");
                    SBSql.AppendLine(" VALUES (@StaffID, @CustID" + row + "); ");

                    cmd.Parameters.AddWithValue("CustID" + row, item.Val);
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("StaffID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", "");
                    return;
                }
                else
                {
                    //回傳至母頁
                    string jsWord = string.Format(
                        "parent.$('#Cnt_{0}').text('{1}');"
                        , Param_thisID
                        , row);

                    //執行轉頁
                    fn_Extensions.JsAlert("資料儲存成功！", string.Format("script:location.href='{0}';{1}", PageUrl, jsWord));
                    return;
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }
    }

    /// <summary>
    /// 篩選過濾重複資料
    /// </summary>
    /// <param name="inputValue">輸入值</param>
    /// <returns></returns>
    private string Filter_Value(string inputValue)
    {
        if (string.IsNullOrEmpty(inputValue))
        {
            return "";
        }

        ArrayList aryItem = new ArrayList();
        //拆解值
        string[] strAry = Regex.Split(inputValue, @"\|{4}");
        //篩選，移除重複資料
        var query = from el in strAry
                    group el by el.ToString().Trim() into gp
                    select new
                    {
                        Val = gp.Key
                    };
        foreach (var item in query)
        {
            aryItem.Add(item.Val);
        }
        //回傳篩選後的資料
        return string.Join("||||", aryItem.ToArray());
    }
    #endregion -- 資料編輯 End --

    #region -- 參數設定 --
    /// <summary>
    /// 本筆資料的編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["StaffID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["StaffID"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return string.Format(@"Sales_RelCust_Edit.aspx?StaffID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_thisID)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    #endregion

}
