using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class CustGP_Search : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //[帶出資料]
                LookupDataList();

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料取得 --

    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            this.ViewState["Page_Url"] = "CustGP_Search.aspx?srh=1";

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Group_ID, Group_Name, Display, Sort ");
                SBSql.AppendLine(" FROM File_CustGroup ");
                SBSql.AppendLine(" WHERE (1=1) ");

                //[查詢條件] - 關鍵字
                if (false == string.IsNullOrEmpty(Req_Keyword))
                {
                    SBSql.Append(" AND ( ");
                    SBSql.Append("      (Group_Name LIKE '%' + @Keyword + '%') ");
                    SBSql.Append(" ) ");

                    cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                    this.ViewState["Page_Url"] += "&Keyword=" + Server.UrlEncode(fn_stringFormat.Filter_Html(Req_Keyword));
                }

                SBSql.AppendLine(" ORDER BY Display DESC, Sort ASC ");
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料顯示");
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //取得Key值
                string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                using (SqlCommand cmd = new SqlCommand())
                {
                    //刪除資料
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" DELETE FROM File_CustList WHERE (Group_ID = @Param_ID); ");
                    SBSql.AppendLine(" DELETE FROM File_CustGroup WHERE (Group_ID = @Param_ID); ");

                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", Get_DataID);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("無法刪除!\\n資料已被使用..", "");
                        return;
                    }
                    else
                    {
                        //導向列表頁
                        Response.Redirect(this.ViewState["Page_Url"].ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - ItemCommand");
        }

    }

    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("CustGP_Search.aspx?srh=1");

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Filter_Html(this.tb_Keyword.Text)));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["Keyword"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "40", out ErrMsg)) ? fn_stringFormat.Filter_Html(Keyword).Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }

    #endregion
}