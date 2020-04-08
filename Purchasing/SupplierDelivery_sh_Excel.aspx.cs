using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.IO;

public partial class SupplierDelivery_tw_Excel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //取得資料
                LookupDataList();

                string dtNow = DateTime.Now.ToString().ToDateString("yyyyMMdd");
                Response.AddHeader("content-disposition", "attachment; filename=Summary_" + dtNow + ".xls");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Write("<meta http-equiv=Content-Type content=text/html;charset=utf-8>");

                EnableViewState = false;

                StringWriter tw = new System.IO.StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                this.lvDataList.RenderControl(hw);
                this.lvDataList.Visible = true;

                System.Text.Encoding.GetEncoding("UTF-8");
                Response.Write(tw.ToString());
            }
        }
        catch (Exception)
        {
            Response.Write("系統發生錯誤！");
        }
        
    }

    #region -- 資料取得 --
    /// <summary>
    /// 取得資料列表
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  PURMA.MA001, PURMA.MA002, Sup.SupDelDay, CMSMV.MV002 ");
                SBSql.AppendLine(" FROM [SHPK2].dbo.PURMA ");
                SBSql.AppendLine("  LEFT JOIN [SHPK2].dbo.SupplierA Sup ON PURMA.MA001 = Sup.SupCode ");
                SBSql.AppendLine("  LEFT JOIN [SHPK2].dbo.CMSMV ON PURMA.MA047 = CMSMV.MV001 ");
                SBSql.AppendLine(" WHERE (1 = 1) ");

                #region "查詢條件"
                //[查詢條件] - 廠商代號
                if (false == string.IsNullOrEmpty(BgSupID) && false == string.IsNullOrEmpty(EdSupID))
                {
                    //兩欄皆有值，查詢區間資料
                    SBSql.Append(" AND (");
                    SBSql.Append("  UPPER(PURMA.MA001) BETWEEN UPPER(@BgSupID) AND UPPER(@EdSupID) ");
                    SBSql.Append(" )");

                    cmd.Parameters.AddWithValue("BgSupID", BgSupID);
                    cmd.Parameters.AddWithValue("EdSupID", EdSupID);
                }
                else
                {
                    if (false == string.IsNullOrEmpty(BgSupID))
                    {
                        SBSql.Append(" AND (UPPER(PURMA.MA001) = UPPER(@BgSupID))");
                        cmd.Parameters.AddWithValue("BgSupID", BgSupID);
                    }
                    if (false == string.IsNullOrEmpty(EdSupID))
                    {
                        SBSql.Append(" AND (UPPER(PURMA.MA001) = UPPER(@EdSupID))");
                        cmd.Parameters.AddWithValue("EdSupID", EdSupID);
                    }
                }

                //[查詢條件] - Keyword
                if (string.IsNullOrEmpty(Keyword) == false)
                {
                    SBSql.Append(" AND (");
                    SBSql.Append("  (UPPER(PURMA.MA001) LIKE '%' + UPPER(@Keyword) + '%') ");
                    SBSql.Append("  OR (UPPER(PURMA.MA002) LIKE '%' + UPPER(@Keyword) + '%') ");
                    SBSql.Append(" )");
                    cmd.Parameters.AddWithValue("Keyword", Keyword);
                }

                //[查詢條件] - 採購人員
                if (string.IsNullOrEmpty(Employee) == false)
                {
                    SBSql.Append(" AND (CMSMV.MV001 = @Employee) ");
                    cmd.Parameters.AddWithValue("Employee", Employee);
                }
                #endregion

                SBSql.AppendLine(" ORDER BY PURMA.MA001 ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();

                //[SQL] - 取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.ERP_Ana, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            Response.Write("系統發生錯誤 - 取得資料列表！");
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 廠商代號 - Bg
    /// </summary>
    private string _BgSupID;
    public string BgSupID
    {
        get { return string.IsNullOrEmpty(Request["BgSupID"]) ? "" : fn_stringFormat.Filter_Html(Request["BgSupID"].ToString()); }
        private set { this._BgSupID = value; }
    }

    /// <summary>
    /// 廠商代號 - Ed
    /// </summary>
    private string _EdSupID;
    public string EdSupID
    {
        get { return string.IsNullOrEmpty(Request["EdSupID"]) ? "" : fn_stringFormat.Filter_Html(Request["EdSupID"].ToString()); }
        private set { this._EdSupID = value; }
    }

    /// <summary>
    /// 採購人員
    /// </summary>
    private string _Employee;
    public string Employee
    {
        get { return string.IsNullOrEmpty(Request["Employee"]) ? "" : fn_stringFormat.Filter_Html(Request["Employee"].ToString()); }
        private set { this._Employee = value; }
    }

    /// <summary>
    /// 關鍵字
    /// </summary>
    private string _Keyword;
    public string Keyword
    {
        get { return string.IsNullOrEmpty(Request["Keyword"]) ? "" : fn_stringFormat.Filter_Html(Request["Keyword"].ToString()); }
        private set { this._Keyword = value; }
    }

    #endregion

}