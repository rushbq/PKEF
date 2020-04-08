using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class myDW : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 官網下載
                if (fn_CheckAuth.CheckAuth_User("620", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[取得/檢查參數] - 下載分類
                if (fn_Extensions.Menu_FileClass(this.rbl_Class, Req_Class, out ErrMsg) == false)
                {
                    this.rbl_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 語系分類
                if (fn_Extensions.Menu_FileLangType(this.rbl_LangType, Req_LangType, out ErrMsg) == false)
                {
                    this.rbl_LangType.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 檔案分類
                if (fn_Extensions.Menu_FileType(this.ddl_Type, "", true, Req_Class, out ErrMsg) == false)
                {
                    this.ddl_Type.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 下載對象
                if (fn_Extensions.Menu_FileTarget(this.rbl_Target, "", out ErrMsg) == false)
                {
                    this.rbl_Target.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 經銷商群組
                if (fn_Extensions.Menu_CustGroup(this.ddl_CustGroup, "", true, out ErrMsg) == false)
                {
                    this.ddl_CustGroup.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                #region >> 搜尋項目 <<

                //[取得/檢查參數] - 下載對象(搜尋)
                if (fn_Extensions.Menu_FileTarget(this.ddl_Srh_Target, Req_Target, true, out ErrMsg) == false)
                {
                    this.ddl_Srh_Target.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 檔案分類(搜尋)
                if (fn_Extensions.Menu_FileType(this.ddl_Srh_Type, Req_FileType, true, Req_Class, out ErrMsg) == false)
                {
                    this.ddl_Srh_Type.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - Keyword(搜尋)
                if (fn_Extensions.String_字數(Req_Keyword, "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = Req_Keyword.Trim();
                }
                #endregion

                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //[帶出資料]
                if (!string.IsNullOrEmpty(Req_Class) && !string.IsNullOrEmpty(Req_LangType))
                {
                    this.ph_Class.Visible = false;
                    this.ph_Data.Visible = true;

                    //基本資料
                    LookupData_Base();

                    //檔案列表
                    LookupDataList(page);
                }

                //判斷是否為修改資料
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData_Edit();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 基本資料 
    /// </summary>
    private void LookupData_Base()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.Append(" DECLARE @ClassName AS nvarchar(50), @LangTypeName AS nvarchar(50)");
                SBSql.Append(" SET @ClassName = ");
                SBSql.Append("    (SELECT TOP 1 Class_Name FROM File_Class WHERE (Class_ID = @FileClass) AND (LOWER(LangCode) = 'zh-tw'))");
                SBSql.Append(" SET @LangTypeName = ");
                SBSql.Append("    (SELECT TOP 1 Class_Name FROM File_LangType WHERE (Class_ID = @FileLang))");

                SBSql.Append(" SELECT @ClassName AS ClassName, @LangTypeName AS LangTypeName");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("FileClass", Req_Class);
                cmd.Parameters.AddWithValue("FileLang", Req_LangType);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //帶出資料
                    this.lt_ClassName.Text = DT.Rows[0]["ClassName"].ToString();
                    this.lt_LangTypeName.Text = DT.Rows[0]["LangTypeName"].ToString();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 基本資料");
        }
    }

    /// <summary>
    /// 修改資料
    /// </summary>
    private void LookupData_Edit()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.Append(" SELECT Base.SeqNo, Base.File_ID, Base.FileType_ID, Base.Target, Base.FileName, Base.DisplayName");
                SBSql.Append("  , RTRIM(Cust.MA001) AS CustID, RTRIM(RTRIM(Cust.MA002)) AS CustName");
                SBSql.Append(" FROM File_List Base");
                SBSql.Append("  LEFT JOIN File_Rel_Cust RelCust ON Base.File_ID = RelCust.File_ID");
                SBSql.Append("  LEFT JOIN [PKSYS].dbo.Customer Cust ON RelCust.Cust_ERPID = Cust.MA001");
                SBSql.Append(" WHERE (Base.File_ID = @DataID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //[填入資料]
                    this.ddl_Type.SelectedValue = DT.Rows[0]["FileType_ID"].ToString();
                    this.rbl_Target.SelectedValue = DT.Rows[0]["Target"].ToString();

                    //判斷是否有經銷群組
                    string myCustID = DT.Rows[0]["CustID"].ToString();
                    if (!string.IsNullOrEmpty(myCustID))
                    {
                        this.tb_CustID.Text = DT.Rows[0]["CustID"].ToString();
                        this.tb_CustName.Text = "({0}) {1}".FormatThis(DT.Rows[0]["CustID"].ToString(), DT.Rows[0]["CustName"].ToString());
                    }

                    //自訂檔名
                    string myFileName = DT.Rows[0]["DisplayName"].ToString();
                    this.tb_DwFileName.Text = Path.GetFileNameWithoutExtension(myFileName); //顯示檔名
                    this.lt_FileExt.Text = Path.GetExtension(myFileName);   //顯示副檔名
                    this.hf_myFileName.Value = myFileName;

                    //檔案Url
                    string myFile = DT.Rows[0]["FileName"].ToString();
                    this.hf_OldFile.Value = myFile;
                    if (!string.IsNullOrEmpty(myFile))
                    {
                        this.ph_files.Visible = true;
                    }

                    //經銷群組List
                    this.lt_GPList.Text = Get_CustGPList();

                    //品號List
                    this.lt_ViewList.Text = Get_ModelList();

                    //Flag設定 & 欄位顯示/隱藏
                    this.hf_flag.Value = "Edit";
                    this.rfv_fu_Files.Enabled = false;
                    this.lt_Save.Text = "更新檔案";
                    this.lt_Mode.Text = "修改檔案內容";
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 修改資料");
        }
    }

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
            //[參數宣告] - 筆數/分頁設定
            int PageSize = 10;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數


            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.Append(" SELECT TBL.* ");
            SBSql.Append(" FROM ( ");
            SBSql.Append("    SELECT ");
            SBSql.Append("      Base.SeqNo, Base.File_ID, Base.FileName, Base.DisplayName");
            SBSql.Append("      , FType.Class_Name AS FileTypeName");
            SBSql.Append("      , FTarget.Class_Name AS TargetName");
            SBSql.Append("      , ISNULL(Base.Update_Time, Base.Create_Time) AS MtTime");
            SBSql.Append("      , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = ISNULL(Base.Update_Who, Base.Create_Who))) AS MtName");
            SBSql.Append("      , ROW_NUMBER() OVER (ORDER BY Base.Create_Time DESC, Base.Update_Time DESC) AS RowRank ");
            SBSql.Append("    FROM File_List Base ");
            SBSql.Append("      INNER JOIN File_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = 'zh-tw'");
            SBSql.Append("      INNER JOIN File_LangType Lang ON Base.LangType_ID = Lang.Class_ID");
            SBSql.Append("      INNER JOIN File_Type FType ON Base.FileType_ID = FType.Class_ID AND LOWER(FType.LangCode) = 'zh-tw'");
            SBSql.Append("      INNER JOIN File_Target FTarget ON Base.Target = FTarget.Class_ID");
            SBSql.Append("    WHERE (Cls.Class_ID = @FileClass) AND (Lang.Class_ID = @FileLang)");

            #region "查詢條件"
            //[查詢條件] - 檔案分類
            if (false == string.IsNullOrEmpty(Req_FileType))
            {
                SBSql.Append(" AND (Base.FileType_ID = @FileType_ID) ");
                cmd.Parameters.AddWithValue("FileType_ID", Req_FileType);
            }
            //[查詢條件] - 下載對象
            if (false == string.IsNullOrEmpty(Req_Target))
            {
                SBSql.Append(" AND (Base.Target = @Target_ID) ");
                cmd.Parameters.AddWithValue("Target_ID", Req_Target);
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.DisplayName LIKE '%' + @Keyword + '%') ");
                SBSql.Append("     OR Base.File_ID IN (SELECT File_ID FROM File_Rel_ModelNo WHERE Model_No LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);
            }
            #endregion

            SBSql.Append(" ) AS TBL");
            SBSql.Append(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.Append(" ORDER BY RowRank");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
            cmd.Parameters.AddWithValue("FileClass", Req_Class);
            cmd.Parameters.AddWithValue("FileLang", Req_LangType);


            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.Append(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.Append(" FROM File_List Base ");
            SBSql.Append("  INNER JOIN File_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = 'zh-tw'");
            SBSql.Append("  INNER JOIN File_LangType Lang ON Base.LangType_ID = Lang.Class_ID");
            SBSql.Append("  INNER JOIN File_Type FType ON Base.FileType_ID = FType.Class_ID AND LOWER(FType.LangCode) = 'zh-tw'");
            SBSql.Append("  INNER JOIN File_Target FTarget ON Base.Target = FTarget.Class_ID");
            SBSql.Append(" WHERE (Cls.Class_ID = @FileClass) AND (Lang.Class_ID = @FileLang)");

            #region "查詢條件"
            //[查詢條件] - 檔案分類
            if (false == string.IsNullOrEmpty(Req_FileType))
            {
                SBSql.Append(" AND (Base.FileType_ID = @FileType_ID) ");
                cmdTotalCnt.Parameters.AddWithValue("FileType_ID", Req_FileType);
            }
            //[查詢條件] - 下載對象
            if (false == string.IsNullOrEmpty(Req_Target))
            {
                SBSql.Append(" AND (Base.Target = @Target_ID) ");
                cmdTotalCnt.Parameters.AddWithValue("Target_ID", Req_Target);
            }
            //[查詢條件] - 關鍵字
            if (false == string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("      (Base.DisplayName LIKE '%' + @Keyword + '%') ");
                SBSql.Append("     OR Base.File_ID IN (SELECT File_ID FROM File_Rel_ModelNo WHERE Model_No LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            cmdTotalCnt.Parameters.AddWithValue("FileClass", Req_Class);
            cmdTotalCnt.Parameters.AddWithValue("FileLang", Req_LangType);

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
                        Response.Redirect(Page_CurrentUrl + "&page=" + TotalPage);
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
                        sb.AppendFormat("<a href=\"{0}&page=1\" class=\"PagePre\">第一頁</a>", Page_CurrentUrl);
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
                            sb.AppendFormat("<a href=\"{0}&page={1}\">{1}</a>", Page_CurrentUrl, i);
                        }
                    }
                    sb.AppendLine("</div>");

                    //[頁碼] - 最後1頁
                    if (page < TotalPage)
                    {
                        sb.AppendFormat("<a href=\"{0}&page={1}\" class=\"PageNext\">{2}</a>", Page_CurrentUrl, TotalPage, "最後一頁");
                    }
                    //顯示分頁
                    this.pl_Page.Visible = true;
                    this.lt_Page_Link.Text = sb.ToString();

                    #endregion
                }


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


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
            string Get_FileName = ((HiddenField)e.Item.FindControl("hf_FileName")).Value;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //刪除資料
                SBSql.AppendLine("DELETE FROM File_Rel_ModelNo WHERE (File_ID = @DataID);");
                SBSql.AppendLine("DELETE FROM File_Rel_CustGroup WHERE (File_ID = @DataID);");
                SBSql.AppendLine("DELETE FROM File_Rel_Cust WHERE (File_ID = @DataID);");
                SBSql.AppendLine("DELETE FROM File_List WHERE (File_ID = @DataID); ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Get_DataID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料處理失敗", "");
                    return;
                }
                else
                {
                    //刪除檔案
                    if (false == fn_FTP.FTP_DelFile(UploadFolder, Get_FileName))
                    {
                        fn_Extensions.JsAlert("資料刪除成功,檔案刪除失敗", "");
                    }

                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }
        }
    }

    /// <summary>
    /// 品號List
    /// </summary>
    /// <returns></returns>
    private string Get_ModelList()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.Append(" SELECT Rel.Model_No, Prod.Model_Name_zh_TW AS ModelName");
            SBSql.Append(" FROM File_List Base");
            SBSql.Append("  INNER JOIN File_Rel_ModelNo Rel ON Base.File_ID = Rel.File_ID");
            SBSql.Append("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod ON Rel.Model_No = Prod.Model_No");
            SBSql.Append(" WHERE (Base.File_ID = @DataID)");

            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                StringBuilder itemHtml = new StringBuilder();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    //取得參數
                    string myID = DT.Rows[row]["Model_No"].ToString();
                    string myName = DT.Rows[row]["ModelName"].ToString();

                    //組合Html
                    itemHtml.AppendLine("<li id=\"li_{0}_{1}\" class=\"list-group-item\">".FormatThis(row, myID));
                    itemHtml.AppendLine("<table width=\"100%\">");
                    itemHtml.AppendLine("<tr><td style=\"width: 85%\"><label class=\"label label-warning\">{0}</label><h4>{1}</h4></td>".FormatThis(myID, myName));

                    itemHtml.AppendLine("<td class=\"text-right\" style=\"width: 15%\"><button type=\"button\" class=\"btn btn-default btn-xs\" onclick=\"Delete_Item('{0}_{1}');\"><i class=\"fa fa-times\"></i>&nbsp;移除</button></td>".FormatThis(
                        row, myID
                        ));
                    itemHtml.AppendLine("</tr></table>");

                    //Hidden field
                    itemHtml.Append("<input type=\"hidden\" class=\"Item_ID\" value=\"{0}\" />".FormatThis(myID));
                    itemHtml.AppendLine("</li>");
                }

                return itemHtml.ToString();

            }

        }
    }

    /// <summary>
    /// 經銷商群組List
    /// </summary>
    /// <returns></returns>
    private string Get_CustGPList()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.Append(" SELECT CustGP.Group_ID, CustGP.Group_Name");
            SBSql.Append(" FROM File_List Base");
            SBSql.Append("  INNER JOIN File_Rel_CustGroup RelCustGP ON Base.File_ID = RelCustGP.File_ID");
            SBSql.Append("  INNER JOIN File_CustGroup CustGP ON RelCustGP.Group_ID = CustGP.Group_ID");
            SBSql.Append(" WHERE (Base.File_ID = @DataID)");

            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                StringBuilder itemHtml = new StringBuilder();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    //取得參數
                    string myID = DT.Rows[row]["Group_ID"].ToString();
                    string myName = DT.Rows[row]["Group_Name"].ToString();

                    //組合Html
                    itemHtml.AppendLine("<li id=\"li_{0}_{1}\" class=\"list-group-item\">".FormatThis(row, myID));
                    itemHtml.AppendLine("<table width=\"100%\">");
                    itemHtml.AppendLine("<tr><td style=\"width: 85%\"><h4>{0}</h4></td>".FormatThis(myName));

                    itemHtml.AppendLine("<td class=\"text-right\" style=\"width: 15%\"><button type=\"button\" class=\"btn btn-default btn-xs\" onclick=\"Delete_Item('{0}_{1}');\"><i class=\"fa fa-times\"></i>&nbsp;移除</button></td>".FormatThis(
                        row, myID
                        ));
                    itemHtml.AppendLine("</tr></table>");

                    //Hidden field
                    itemHtml.Append("<input type=\"hidden\" class=\"Item_ID\" value=\"{0}\" />".FormatThis(myID));
                    itemHtml.AppendLine("</li>");
                }

                return itemHtml.ToString();

            }

        }
    }

    #endregion

    #region -- 前端頁面控制 --
    //分頁跳轉
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(Page_CurrentUrl + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    //搜尋
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("myDW.aspx?f=sh");

            //[查詢條件] - Class
            if (!string.IsNullOrEmpty(this.rbl_Class.SelectedValue))
            {
                SBUrl.Append("&Class=" + Server.UrlEncode(this.rbl_Class.SelectedValue));
            }
            //[查詢條件] - LangType
            if (!string.IsNullOrEmpty(this.rbl_LangType.SelectedValue))
            {
                SBUrl.Append("&LangType=" + Server.UrlEncode(this.rbl_LangType.SelectedValue));
            }

            //[查詢條件] - 下載對象
            if (!string.IsNullOrEmpty(this.ddl_Srh_Target.SelectedValue))
            {
                SBUrl.Append("&ReqTarget=" + Server.UrlEncode(this.ddl_Srh_Target.SelectedValue));
            }
            //[查詢條件] - 檔案分類
            if (!string.IsNullOrEmpty(this.ddl_Srh_Type.SelectedValue))
            {
                SBUrl.Append("&ReqFileType=" + Server.UrlEncode(this.ddl_Srh_Type.SelectedValue));
            }
            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text.Trim()))
            {
                SBUrl.Append("&ReqKeyword=" + Server.UrlEncode(this.tb_Keyword.Text.Trim()));
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

    #region -- 資料編輯 --
    /// <summary>
    /// 基本設定存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_DwFileName.Text, "1", "30", out ErrMsg) == false)
            {
                SBAlert.Append("「自訂檔名」請輸入1 ~ 30個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //副檔名檢查參數
            int errExt = 0;

            //[IO] - 暫存檔案名稱
            List<IOTempParam> ITempList = new List<IOTempParam>();

            //取得上傳檔案
            HttpPostedFile hpFile = this.fu_Files.PostedFile;
            if (hpFile != null)
            {
                if (hpFile.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpFile);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        //暫存檔案資訊
                        ITempList.Add(new IOTempParam(IOManage.FileNewName, this.hf_OldFile.Value, hpFile, "P1"));
                    }
                    else
                    {
                        errExt++;
                    }
                }
            }


            //未符合檔案規格的警示訊息
            if (errExt > 0)
            {
                fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data(ITempList);
                    break;

                case "EDIT":
                    Edit_Data(ITempList);
                    break;

                default:
                    throw new Exception("走錯路囉!");
            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data(List<IOTempParam> ITempList)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            string pic1 = "";

            //取得圖片參數
            var queryPic = from el in ITempList
                           select new
                           {
                               NewPic = el.Param_FileName,
                               PicKind = el.Param_FileKind
                           };
            foreach (var item in queryPic)
            {
                if (item.PicKind.Equals("P1"))
                {
                    pic1 = item.NewPic;
                }
            }

            //取得副檔名
            string GetFileExt = Path.GetExtension(pic1);
            //產生Guid
            string guid = fn_Extensions.GetGuid();

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO File_List( ");
            SBSql.AppendLine("  File_ID, Class_ID, LangType_ID, FileType_ID");
            SBSql.AppendLine("  , Target, FileName, DisplayName");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @myGuid, @Class_ID, @LangType_ID, @FileType_ID");
            SBSql.AppendLine("  , @Target, @FileName, @DisplayName");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" )");

            //建立關聯 - 下載對象(指定經銷商)
            if (this.rbl_Target.SelectedValue.Equals("4"))
            {
                string custID = this.tb_CustID.Text;
                if (!string.IsNullOrEmpty(custID))
                {
                    SBSql.Append("INSERT INTO File_Rel_Cust(");
                    SBSql.Append("Cust_ERPID, File_ID");
                    SBSql.Append(") VALUES(");
                    SBSql.Append("@CustID, @myGuid");
                    SBSql.Append(")");

                    cmd.Parameters.AddWithValue("CustID", custID);
                }
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("myGuid", guid);
            cmd.Parameters.AddWithValue("Class_ID", this.rbl_Class.SelectedValue);
            cmd.Parameters.AddWithValue("LangType_ID", this.rbl_LangType.SelectedValue);
            cmd.Parameters.AddWithValue("FileType_ID", this.ddl_Type.SelectedValue);
            cmd.Parameters.AddWithValue("Target", this.rbl_Target.SelectedValue);
            cmd.Parameters.AddWithValue("FileName", pic1.Left(60));
            cmd.Parameters.AddWithValue("DisplayName", "{0}{1}".FormatThis(this.tb_DwFileName.Text.Trim(), GetFileExt));
            cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
                return;
            }

            //判斷資料夾是否存在
            fn_FTP.FTP_CheckFolder(UploadFolder);

            //儲存檔案
            int errCnt = 0;
            for (int row = 0; row < ITempList.Count; row++)
            {
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                if (hpf.ContentLength > 0)
                {
                    string FileName = ITempList[row].Param_FileName;
                    if (false == fn_FTP.FTP_doUpload(hpf, UploadFolder, FileName))
                    {
                        errCnt++;
                    }
                }
            }

            if (errCnt > 0)
            {
                fn_Extensions.JsAlert("資料新增成功,檔案上傳失敗！", "");
                return;
            }

            //設定關聯
            if (false == Set_ModelRel(guid))
            {
                fn_Extensions.JsAlert("品號設定失敗", "");
                return;
            }
            //建立關聯 - 下載對象(經銷商群組)
            if (this.rbl_Target.SelectedValue.Equals("3"))
            {
                if (false == Set_GroupRel(guid))
                {
                    fn_Extensions.JsAlert("經銷商群組設定失敗", "");
                    return;
                }
            }


            //導向本頁
            Response.Redirect(Page_CurrentUrl);

        }

    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data(List<IOTempParam> ITempList)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            string pic1 = this.hf_OldFile.Value;

            //取得圖片參數
            var queryPic = from el in ITempList
                           select new
                           {
                               NewPic = el.Param_FileName,
                               PicKind = el.Param_FileKind
                           };
            foreach (var item in queryPic)
            {
                if (item.PicKind.Equals("P1"))
                {
                    pic1 = item.NewPic;
                }
            }
            //取得副檔名
            string GetFileExt = Path.GetExtension(pic1);

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE File_List ");
            SBSql.AppendLine(" SET FileType_ID = @FileType, Target = @Target");
            SBSql.AppendLine("  , FileName = @FileName, DisplayName = @DisplayName");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (File_ID = @DataID) ");

            //建立關聯 - 下載對象(指定經銷商)
            if (this.rbl_Target.SelectedValue.Equals("4"))
            {
                string custID = this.tb_CustID.Text;
                if (!string.IsNullOrEmpty(custID))
                {
                    //移除所有關聯 - 下載對象
                    SBSql.Append("DELETE FROM File_Rel_CustGroup WHERE (File_ID = @DataID);");
                    SBSql.Append("DELETE FROM File_Rel_Cust WHERE (File_ID = @DataID);");

                    //重新建立關聯
                    SBSql.Append("INSERT INTO File_Rel_Cust(");
                    SBSql.Append("Cust_ERPID, File_ID");
                    SBSql.Append(") VALUES(");
                    SBSql.Append("@CustID, @DataID");
                    SBSql.Append(");");

                    cmd.Parameters.AddWithValue("CustID", custID);
                }
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("FileType", this.ddl_Type.SelectedValue);
            cmd.Parameters.AddWithValue("Target", this.rbl_Target.SelectedValue);
            cmd.Parameters.AddWithValue("FileName", pic1.Left(60));
            cmd.Parameters.AddWithValue("DisplayName", "{0}{1}".FormatThis(this.tb_DwFileName.Text, GetFileExt));
            cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserGuid);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //[檔案處理]
            if (ITempList.Count > 0)
            {
                //判斷資料夾是否存在
                fn_FTP.FTP_CheckFolder(UploadFolder);

                //儲存檔案
                int errCnt = 0;
                for (int row = 0; row < ITempList.Count; row++)
                {
                    HttpPostedFile hpf = ITempList[row].Param_hpf;
                    if (hpf.ContentLength > 0)
                    {
                        string FileName = ITempList[row].Param_FileName;
                        string OldFile = ITempList[row].Param_OrgFileName;

                        //刪除舊檔案
                        fn_FTP.FTP_DelFile(UploadFolder, OldFile);

                        //上傳新檔案
                        if (false == fn_FTP.FTP_doUpload(hpf, UploadFolder, FileName))
                        {
                            errCnt++;
                        }
                    }
                }

                if (errCnt > 0)
                {
                    fn_Extensions.JsAlert("資料更新成功,檔案上傳失敗！", "");
                    return;
                }
            }

            //設定關聯
            if (false == Set_ModelRel(Param_thisID))
            {
                fn_Extensions.JsAlert("品號設定失敗", "");
                return;
            }

            //建立關聯 - 下載對象(經銷商群組)
            if (this.rbl_Target.SelectedValue.Equals("3"))
            {
                if (false == Set_GroupRel(Param_thisID))
                {
                    fn_Extensions.JsAlert("經銷商群組設定失敗", "");
                    return;
                }
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }

    /// <summary>
    /// 關聯設定 - 品號
    /// </summary>
    /// <param name="DataID">單頭資料編號</param>
    /// <returns></returns>
    private bool Set_ModelRel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.myModelValues.Text;

        //判斷是否為空
        if (string.IsNullOrEmpty(Get_IDs))
        {
            return true;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(Get_IDs, ",");

        //宣告暫存清單
        List<TempParam> ITempList = new List<TempParam>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam(strAry_ID[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID
                    };

        //處理資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            SBSql.AppendLine(" DELETE FROM File_Rel_ModelNo WHERE (File_ID = @DataID); ");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                SBSql.AppendLine(" INSERT INTO File_Rel_ModelNo( ");
                SBSql.AppendLine("  File_ID, Model_No");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @Model_No_{0}".FormatThis(row));
                SBSql.AppendLine(" ); ");

                cmd.Parameters.AddWithValue("Model_No_" + row, item.ID);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", DataID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }

    /// <summary>
    /// 關聯設定 - 群組
    /// </summary>
    /// <param name="DataID">單頭資料編號</param>
    /// <returns></returns>
    private bool Set_GroupRel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.myGPValues.Text;

        //判斷是否為空
        if (string.IsNullOrEmpty(Get_IDs))
        {
            return true;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(Get_IDs, ",");

        //宣告暫存清單
        List<TempParam> ITempList = new List<TempParam>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam(strAry_ID[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID
                    };

        //處理資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            SBSql.Append("DELETE FROM File_Rel_CustGroup WHERE (File_ID = @DataID);");
            SBSql.Append("DELETE FROM File_Rel_Cust WHERE (File_ID = @DataID);");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                SBSql.AppendLine(" INSERT INTO File_Rel_CustGroup( ");
                SBSql.AppendLine("  File_ID, Group_ID");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @Group_ID_{0}".FormatThis(row));
                SBSql.AppendLine(" ); ");

                cmd.Parameters.AddWithValue("Group_ID_" + row, item.ID);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", DataID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - Class
    /// </summary>
    private string _Req_Class;
    public string Req_Class
    {
        get
        {
            String reqData = Request.QueryString["Class"];
            return (string.IsNullOrEmpty(reqData)) ? "" : fn_stringFormat.Filter_Html(reqData).Trim();
        }
        set
        {
            this._Req_Class = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - LangType
    /// </summary>
    private string _Req_LangType;
    public string Req_LangType
    {
        get
        {
            String reqData = Request.QueryString["LangType"];
            return (string.IsNullOrEmpty(reqData)) ? "" : fn_stringFormat.Filter_Html(reqData).Trim();
        }
        set
        {
            this._Req_LangType = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Search:FileType
    /// </summary>
    private string _Req_FileType;
    public string Req_FileType
    {
        get
        {
            String reqData = Request.QueryString["ReqFileType"];
            return (string.IsNullOrEmpty(reqData)) ? "" : fn_stringFormat.Filter_Html(reqData).Trim();
        }
        set
        {
            this._Req_FileType = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Search:Target
    /// </summary>
    private string _Req_Target;
    public string Req_Target
    {
        get
        {
            String reqData = Request.QueryString["ReqTarget"];
            return (string.IsNullOrEmpty(reqData)) ? "" : fn_stringFormat.Filter_Html(reqData).Trim();
        }
        set
        {
            this._Req_Target = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Search:Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String reqData = Request.QueryString["ReqKeyword"];
            return (string.IsNullOrEmpty(reqData)) ? "" : fn_stringFormat.Filter_Html(reqData).Trim();
        }
        set
        {
            this._Req_Keyword = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Request.QueryString["DataID"];

            return string.IsNullOrEmpty(DataID) ? "" : HttpUtility.UrlEncode(DataID);
        }
        set
        {
            this._Param_thisID = value;
        }
    }


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}myDownload/myDW.aspx?Class={1}&LangType={2}&ReqTarget={3}&ReqFileType={4}&ReqKeyword={5}".FormatThis(
                Application["WebUrl"]
                , HttpUtility.UrlEncode(Req_Class)
                , HttpUtility.UrlEncode(Req_LangType)
                , HttpUtility.UrlEncode(Req_Target)
                , HttpUtility.UrlEncode(Req_FileType)
                , HttpUtility.UrlEncode(Req_Keyword)
            );

        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }
    #endregion

    #region -- 上傳參數 --
    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png|pdf|rar|zip";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return @"ProdFiles/{0}".FormatThis(Req_Class);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class IOTempParam
    {
        /// <summary>
        /// [參數] - 檔名
        /// </summary>
        private string _Param_FileName;
        public string Param_FileName
        {
            get { return this._Param_FileName; }
            set { this._Param_FileName = value; }
        }

        /// <summary>
        /// [參數] -原始檔名
        /// </summary>
        private string _Param_OrgFileName;
        public string Param_OrgFileName
        {
            get { return this._Param_OrgFileName; }
            set { this._Param_OrgFileName = value; }
        }

        /// <summary>
        /// [參數] - 類別
        /// </summary>
        private string _Param_FileKind;
        public string Param_FileKind
        {
            get { return this._Param_FileKind; }
            set { this._Param_FileKind = value; }
        }

        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_FileName">系統檔名</param>
        /// <param name="Param_OrgFileName">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public IOTempParam(string Param_FileName, string Param_OrgFileName, HttpPostedFile Param_hpf, string Param_FileKind)
        {
            this._Param_FileName = Param_FileName;
            this._Param_OrgFileName = Param_OrgFileName;
            this._Param_hpf = Param_hpf;
            this._Param_FileKind = Param_FileKind;
        }

    }
    #endregion

    #region -- 暫存參數 --
    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 編號
        /// </summary>
        private string _tmp_ID;
        public string tmp_ID
        {
            get { return this._tmp_ID; }
            set { this._tmp_ID = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        public TempParam(string tmp_ID)
        {
            this._tmp_ID = tmp_ID;
        }
    }
    #endregion
}