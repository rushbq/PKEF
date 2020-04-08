using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;
using LogRecord;

public partial class Shipment_Search : SecurityIn
{
    //回覆權限
    public bool ReplyAuth = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 出貨單-台灣
                if (fn_CheckAuth.CheckAuth_User("720", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[取得/檢查參數] - 出貨狀態
                this.ddl_ShipClass.SelectedValue = Request.QueryString["ReqClass"];

                //[取得/檢查參數] - StartDate
                String StartDate = Request.QueryString["StartDate"];
                if (fn_Extensions.String_字數(StartDate, "1", "10", out ErrMsg) && StartDate.IsDate())
                {
                    this.tb_StartDate.Text = fn_stringFormat.Filter_Html(StartDate.ToString().Trim());
                }
                //[取得/檢查參數] - EndDate
                String EndDate = Request.QueryString["EndDate"];
                if (fn_Extensions.String_字數(EndDate, "1", "10", out ErrMsg) && StartDate.IsDate())
                {
                    this.tb_EndDate.Text = fn_stringFormat.Filter_Html(EndDate.ToString().Trim());
                }
                //[取得/檢查參數] - Keyword
                String Keyword = Request.QueryString["Keyword"];
                if (fn_Extensions.String_字數(Keyword, "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = Keyword.Trim();
                }

                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //[帶出資料]
                LookupDataList(page);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="page">頁數</param>
    private void LookupDataList(int page)
    {
        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        string ErrMsg;
        try
        {
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "myBBC/Shipment_Search.aspx?f=sh";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string StartDate = fn_stringFormat.Filter_Html(this.tb_StartDate.Text);
            string EndDate = fn_stringFormat.Filter_Html(this.tb_EndDate.Text);
            string Keyword = fn_stringFormat.Filter_Html(this.tb_Keyword.Text.Trim()).Replace("-", "");
            string Req_Class = fn_stringFormat.Filter_Html(this.ddl_ShipClass.SelectedValue);   //出貨狀態

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Base.OrderID, Base.MyOrderID, Base.CustERPID AS CustID, Cust.MA002 AS CustName ");
            SBSql.AppendLine("      , Base.ShipWho, Base.ShippingAddr, Base.Create_Time, Sub.ShipNo ");
            SBSql.AppendLine("      , myPara.Class_Name AS ShipTypeName, ISNULL(Sub.ShipStatus, 0) AS ShipStatus ");
            SBSql.AppendLine("      , Base.Cancel_Who, Base.Cancel_Time");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.Cancel_Time, Sub.ShipStatus ASC, Base.Create_Time DESC) AS RowRank ");
            SBSql.AppendLine("    FROM Order_Main Base ");
            SBSql.AppendLine("      INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustERPID = Cust.MA001 AND Cust.DBC = Cust.DBS ");
            SBSql.AppendLine("      LEFT JOIN Order_Shipping Sub ON Base.OrderID = Sub.OrderID ");
            SBSql.AppendLine("      LEFT JOIN Order_ParamClass myPara ON Sub.ShipType = myPara.Class_ID ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));

                this.ViewState["Page_LinkStr"] += "&StartDate=" + Server.UrlEncode(StartDate);
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));

                this.ViewState["Page_LinkStr"] += "&EndDate=" + Server.UrlEncode(EndDate);
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.OrderID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.MyOrderID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.CustERPID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Cust.MA002 LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }
            //[查詢條件] - 類別
            if (false == string.IsNullOrEmpty(Req_Class))
            {
                SBSql.Append(" AND (ISNULL(Sub.ShipStatus, 0) = @ReqClass)");

                cmd.Parameters.AddWithValue("ReqClass", Req_Class);

                this.ViewState["Page_LinkStr"] += "&ReqClass=" + Server.UrlEncode(Req_Class);
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Order_Main Base ");
            SBSql.AppendLine("   INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustERPID = Cust.MA001 AND Cust.DBC = Cust.DBS ");
            SBSql.AppendLine("   LEFT JOIN Order_Shipping Sub ON Base.OrderID = Sub.OrderID ");
            SBSql.AppendLine("   LEFT JOIN Order_ParamClass myPara ON Sub.ShipType = myPara.Class_ID ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                SBSql.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmdTotalCnt.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                SBSql.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmdTotalCnt.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.OrderID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.MyOrderID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Base.CustERPID LIKE '%' + @Keyword + '%') ");
                SBSql.Append("      OR (Cust.MA002 LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }
            //[查詢條件] - 類別
            if (false == string.IsNullOrEmpty(Req_Class))
            {
                SBSql.Append(" AND (ISNULL(Sub.ShipStatus, 0) = @ReqClass)");

                cmdTotalCnt.Parameters.AddWithValue("ReqClass", Req_Class);
            }

            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalCnt, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    //判斷是否為頁碼過大, 帶往最後一頁
                    if (TotalCnt > 0 & BgItem > TotalCnt)
                    {
                        if (TotalCnt % PageSize == 0)
                        {
                            TotalPage = Convert.ToInt16(TotalCnt / PageSize);
                        }
                        else
                        {
                            TotalPage = Convert.ToInt16(Math.Floor((double)TotalCnt / PageSize)) + 1;
                        }
                        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + TotalPage);
                    }
                    else
                    {
                        //隱藏分頁
                        this.pl_Page.Visible = false;
                    }
                }
                else
                {
                    #region "分頁控制"
                    //計算總頁數
                    if (TotalCnt % PageSize == 0)
                    {
                        TotalPage = Convert.ToInt16(TotalCnt / PageSize);
                    }
                    else
                    {
                        TotalPage = Convert.ToInt16(Math.Floor((double)TotalCnt / PageSize)) + 1;
                    }
                    //判斷頁數
                    if (page < 1)
                        page = 1;
                    if (page > TotalPage)
                        page = TotalPage;
                    //一次n頁的頁碼
                    int PageTen = 0;
                    if (page % PageRoll == 0)
                        PageTen = page;
                    else
                        PageTen = (Convert.ToInt16(Math.Floor((double)page / PageRoll)) + 1) * PageRoll;
                    //帶入頁數資料
                    int FirstItem = (page - 1) * PageSize + 1;
                    int LastItem = FirstItem + (PageSize - 1);
                    if (LastItem > TotalCnt)
                        LastItem = TotalCnt;
                    //填入頁數資料
                    int i = 0;
                    for (i = 1; i <= TotalPage; i++)
                    {
                        this.ddl_Page_List.Items.Insert(i - 1, Convert.ToString(i));
                        this.ddl_Page_List.Items[i - 1].Value = Convert.ToString(i);
                    }
                    this.ddl_Page_List.SelectedValue = Convert.ToString(page); //頁碼下拉選單
                    this.lt_TotalPage.Text = Convert.ToString(TotalPage);  // n 頁
                    this.lt_Page_DataCntInfo.Text = "第 " + FirstItem + " - " + LastItem + " 筆，共 " + TotalCnt + " 筆";

                    //[分頁] - 顯示頁碼
                    StringBuilder sb = new StringBuilder();

                    //[頁碼] - 第一頁
                    if (page >= 2)
                    {
                        sb.AppendFormat("<a href=\"{0}&page=1\" class=\"PagePre\">第一頁</a>", this.ViewState["Page_LinkStr"]);
                    }
                    //[頁碼] - 數字頁碼
                    sb.AppendLine("<div class=\"Pages\">");

                    for (i = PageTen - (PageRoll - 1); i <= PageTen; i++)
                    {
                        if (i > TotalPage)
                            break;
                        if (i == page)
                        {
                            sb.AppendFormat("<span>{0}</span>", i);
                        }
                        else
                        {
                            sb.AppendFormat("<a href=\"{0}&page={1}\">{1}</a>", this.ViewState["Page_LinkStr"], i);
                        }
                    }
                    sb.AppendLine("</div>");

                    //[頁碼] - 最後1頁
                    if (page < TotalPage)
                    {
                        sb.AppendFormat("<a href=\"{0}&page={1}\" class=\"PageNext\">{2}</a>", this.ViewState["Page_LinkStr"], TotalPage, "最後一頁");
                    }
                    //顯示分頁
                    this.pl_Page.Visible = true;
                    this.lt_Page_Link.Text = sb.ToString();

                    #endregion
                }

                //暫存目前頁碼 
                this.ViewState["page"] = page;

                //暫存目前Url
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"] + "&page=" + page;

                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 取得資料列表！", "");
        }
        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                #region >> 出貨方式 <<

                string ShipTypeName = DataBinder.Eval(dataItem.DataItem, "ShipTypeName").ToString();

                if (!string.IsNullOrEmpty(ShipTypeName))
                {
                    Label lb_ShipType = (Label)e.Item.FindControl("lb_ShipType");
                    lb_ShipType.Text = "<i class=\"fa fa-truck\"></i> " + ShipTypeName;
                }

                #endregion


                #region >> 狀態 <<

                //取得狀態
                string helpStatus = DataBinder.Eval(dataItem.DataItem, "ShipStatus").ToString();
                string CancelTime = DataBinder.Eval(dataItem.DataItem, "Cancel_Time").ToString();

                bool showEdit, showView;
                string StatusName, StatusCss;

                //判斷狀態, 給予不同的顏色
                Label lb_Status = (Label)e.Item.FindControl("lb_Status");

                if (!string.IsNullOrEmpty(CancelTime))
                {
                    StatusCss = "label label-default";
                    StatusName = "取消訂單";
                    showEdit = false;
                    showView = true;
                }
                else
                {
                    switch (helpStatus)
                    {
                        case "0":
                            StatusCss = "label label-warning";
                            StatusName = "待處理";
                            showEdit = true;
                            showView = false;

                            break;

                        case "1":
                            StatusCss = "label label-info";
                            StatusName = "出貨中";
                            showEdit = true;
                            showView = false;

                            break;

                        case "2":
                            StatusCss = "label label-success";
                            StatusName = "已出貨";
                            showEdit = false;
                            showView = true;

                            break;

                        default:
                            StatusCss = "label label-warning";
                            StatusName = "待處理";
                            showEdit = true;
                            showView = false;

                            break;
                    }
                }

                

                //設定狀態
                lb_Status.CssClass = StatusCss;
                lb_Status.Text = StatusName;

                PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                PlaceHolder ph_View = (PlaceHolder)e.Item.FindControl("ph_View");
                ph_Edit.Visible = showEdit;
                ph_View.Visible = showView;

                #endregion

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;
                    StringBuilder SBSql = new StringBuilder();

                    //取得Key值
                    string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                    SBSql.AppendLine(" UPDATE Order_Main SET Cancel_Who = @Who, Cancel_Time = GETDATE() WHERE (OrderID = @OrderID)");

                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("OrderID", Get_DataID);
                    cmd.Parameters.AddWithValue("Who", fn_Params.UserGuid);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料處理失敗！", "");
                        return;
                    }
                }

                //redirect
                Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - ItemCommand！", "");
        }

    }

    #endregion

    #region -- 前端頁面控制 --
    //分頁跳轉
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    //搜尋
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Shipment_Search.aspx?f=sh");

            //[查詢條件] - 開始日期
            if (string.IsNullOrEmpty(this.tb_StartDate.Text) == false)
            {
                SBUrl.Append("&StartDate=" + Server.UrlEncode(this.tb_StartDate.Text));
            }
            //[查詢條件] - 結束日期
            if (string.IsNullOrEmpty(this.tb_EndDate.Text) == false)
            {
                SBUrl.Append("&EndDate=" + Server.UrlEncode(this.tb_EndDate.Text));
            }
            //[查詢條件] - 關鍵字
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Filter_Html(this.tb_Keyword.Text)));
            }
            //[查詢條件] - 出貨狀態
            if (this.ddl_ShipClass.SelectedIndex > 0)
            {
                SBUrl.Append("&ReqClass=" + Server.UrlEncode(this.ddl_ShipClass.SelectedValue));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    protected string DesKey
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
    #endregion

}