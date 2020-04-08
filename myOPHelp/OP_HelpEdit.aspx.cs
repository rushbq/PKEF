using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ExtensionMethods;
using MailMethods;
using ExtensionUI;

public partial class OP_HelpEdit : SecurityIn
{
    //回覆權限
    public bool ReplyAuth = false;
    //其他共用宣告
    public string ErrMsg;
    public string MailSender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];
    public string MailSenderName = "寶工綠巨人";
    public string MailFuncName = "PKEF, 品號需求登記";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 需求登記
                if (fn_CheckAuth.CheckAuth_User("524", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //取得回覆權限
                ReplyAuth = fn_CheckAuth.CheckAuth_User("525", out ErrMsg);

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "myOPHelp/OP_HelpSearch.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, "", true, false, new List<string> { "TW", "SH", "SZ" }, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //設定預設值 - 部門
                this.ddl_Dept.SelectedValue = Param_DeptID;

                //[取得/檢查參數] - 問題類型
                if (fn_Extensions.Menu_OPClass(this.ddl_Req_Class, "", true, new List<string> { "登記問題類型" }, out ErrMsg) == false)
                {
                    this.ddl_Req_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //Show / Hide
                this.ph_MailItem.Visible = true;
                this.ph_MailList.Visible = false;

                //[參數判斷] - 是否為修改資料
                if (false == string.IsNullOrEmpty(Param_thisID))
                {
                    View_Data();
                }
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", Session["BackListUrl"].ToString());
                return;
            }
        }
    }


    #region -- 資料顯示 --
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void View_Data()
    {
        try
        {
            string ErrMsg;

            //[取得/檢查參數] - 系統編號
            if (string.IsNullOrEmpty(Param_thisID))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                return;
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                SBSql.AppendLine(" SELECT Main.*, HelpStatus.Class_Name AS HStatus");
                SBSql.AppendLine("  , Cls.Notify");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Create_Who)) AS Create_Name");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Update_Who)) AS Update_Name");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Agree_Who)) AS Agree_Name");
                SBSql.AppendLine("  , (SELECT Email FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Req_Who)) AS Req_Email");
                SBSql.AppendLine(" FROM OP_Help Main");
                SBSql.AppendLine("  INNER JOIN OP_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID");
                SBSql.AppendLine("  INNER JOIN OP_ParamClass Cls ON Main.Req_Class = Cls.Class_ID");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID);");

                //若為回覆人員,則Update狀態為處理中(10)
                if (ReplyAuth)
                {
                    SBSql.AppendLine("UPDATE OP_Help SET Help_Status = 10 WHERE (TraceID = @TraceID) AND (Help_Status = 9)");
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //判斷狀態, 只有待處理或回覆人員(權限代號:521)才可呆在編輯頁
                        bool keepGoing;
                        string Help_Status = DT.Rows[0]["Help_Status"].ToString();
                        if (Help_Status.Equals("9") || ReplyAuth)
                        {
                            keepGoing = true;
                        }
                        else
                        {
                            keepGoing = false;
                        }
                        if (false == keepGoing)
                        {
                            //導向View頁
                            Response.Redirect("OP_HelpView.aspx?TraceID={0}".FormatThis(Server.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey))));
                            return;
                        }

                        //[填入資料]
                        string traceID = DT.Rows[0]["TraceID"].ToString();
                        this.lt_TraceID.Text = convertTraceID(traceID);
                        this.ddl_Dept.SelectedValue = DT.Rows[0]["Req_Dept"].ToString();
                        this.tb_EmpValue.Text = DT.Rows[0]["Req_Who"].ToString();
                        this.tb_Email.Text = DT.Rows[0]["Req_Email"].ToString();
                        this.ddl_Req_Class.SelectedValue = DT.Rows[0]["Req_Class"].ToString();
                        this.tb_Help_Subject.Text = DT.Rows[0]["Help_Subject"].ToString();
                        this.tb_Help_Content.Text = DT.Rows[0]["Help_Content"].ToString();
                        this.rbl_IsAgree.SelectedValue = DT.Rows[0]["IsAgree"].ToString();

                        //取得狀態
                        string helpStatus = DT.Rows[0]["Help_Status"].ToString();
                        string setCss;
                        //判斷狀態, 給予不同的顏色
                        switch (helpStatus)
                        {
                            case "9":
                                setCss = "styleBluelight";
                                break;

                            case "10":
                                setCss = "styleEarth";
                                break;

                            default:
                                setCss = "styleGreen";
                                break;
                        }
                        this.lb_Help_Status.Text = DT.Rows[0]["HStatus"].ToString();
                        this.lb_Help_Status.CssClass = setCss;

                        //主管核示(權限申請)
                        string IsNotify = DT.Rows[0]["Notify"].ToString();
                        this.ph_Agree.Visible = IsNotify.Equals("Y");

                        //回覆資料
                        if (ReplyAuth)
                        {
                            //顯示回覆區
                            this.ph_Reply.Visible = true;
                            //填入回覆資料
                            this.tb_Reply_Hours.Text = DT.Rows[0]["Reply_Hours"].ToString();
                            this.tb_Reply_Content.Text = DT.Rows[0]["Reply_Content"].ToString();
                            this.tb_Reply_Date.Text = string.IsNullOrEmpty(DT.Rows[0]["Reply_Date"].ToString())
                                ? DateTime.Today.ToShortDateString().ToDateString("yyyy/MM/dd")
                                : DT.Rows[0]["Reply_Date"].ToString().ToDateString("yyyy/MM/dd");

                            //判斷狀態, 顯示相關按鈕
                            switch (Help_Status)
                            {
                                case "9":
                                    this.btn_onTop.Visible = true;
                                    this.btn_Inform.Visible = true;
                                    this.btn_Done.Visible = true;
                                    break;

                                case "10":
                                    this.btn_onTop.Visible = true;
                                    this.btn_Inform.Visible = true;
                                    this.btn_Done.Visible = true;
                                    break;

                                case "11":
                                    this.btn_Back.Visible = true;
                                    break;
                            }
                        }

                        //填入建立 & 修改資料
                        this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                        this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Agree_Who.Text = DT.Rows[0]["Agree_Name"].ToString();
                        this.lt_Agree_Time.Text = DT.Rows[0]["Agree_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.ph_MailItem.Visible = false;
                        this.ph_MailList.Visible = true;

                        //填入圖片資料
                        LookupDataList();

                        //CC名單
                        LookupData_CC(traceID);
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }

    /// <summary>
    /// 副程式 - 顯示附件列表
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT AttachID, AttachFile FROM OP_Help_Attach WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
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
            fn_Extensions.JsAlert("系統發生錯誤 - 附件列表！", "");
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                switch (e.CommandName)
                {
                    case "Del":
                        #region * 刪除 *
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            string ErrMsg;
                            StringBuilder SBSql = new StringBuilder();
                            cmd.Parameters.Clear();

                            //[取得參數] - 編號
                            string GetDataID = ((HiddenField)e.Item.FindControl("hf_PicID")).Value;
                            //[取得參數] - 檔案名稱
                            string GetThisFile = ((HiddenField)e.Item.FindControl("hf_OldFile")).Value;

                            //[SQL] - 刪除資料
                            SBSql.AppendLine(" DELETE FROM OP_Help_Attach WHERE (AttachID = @Param_ID) ");
                            cmd.CommandText = SBSql.ToString();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                            {
                                fn_Extensions.JsAlert("圖片刪除失敗！", "");
                            }
                            else
                            {
                                //刪除檔案
                                IOManage.DelFile(Param_FileFolder, GetThisFile);

                                //頁面跳至明細
                                fn_Extensions.JsAlert("", PageUrl);
                            }
                        }
                        #endregion

                        break;

                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }


    /// <summary>
    /// CC名單
    /// </summary>
    /// <param name="traceID"></param>
    private void LookupData_CC(string traceID)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.Append(" SELECT Prof.Display_Name AS Label, Base.Item_ID AS ID, Base.Item_Email AS Mail");
                SBSql.Append(" FROM OP_Help_CC Base");
                SBSql.Append("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Base.Item_Name = Prof.Account_Name");
                SBSql.Append(" WHERE (Base.TraceID = @DataID)");
                SBSql.Append(" ORDER BY Prof.DeptID, Prof.Account_Name");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", traceID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lv_MailList.DataSource = DT.DefaultView;
                    this.lv_MailList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 單身！", "");
        }
    }

    #endregion -- 資料顯示 End --


    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg = "";
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Help_Subject.Text, "1", "80", out ErrMsg) == false)
            {
                SBAlert.Append("「填寫主旨」請輸入1 ~ 40個字\\n");
            }
            if (fn_Extensions.String_資料長度Byte(this.tb_Help_Content.Text, "1", "1000", out ErrMsg) == false)
            {
                SBAlert.Append("「詳細說明」請輸入1 ~ 500個字\\n");
            }
            if (fn_Extensions.String_資料長度Byte(this.tb_Reply_Content.Text, "0", "1000", out ErrMsg) == false)
            {
                SBAlert.Append("「處理回覆」請輸入1 ~ 500個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();
            HttpFileCollection hfc = Request.Files;
            for (int i = 0; i <= hfc.Count - 1; i++)
            {
                HttpPostedFile hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpf);
                    ITempList.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hpf));
                }
            }
            #endregion

            #region "資料儲存"
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
    private void Add_Data(List<TempParam> ITempList)
    {
        string ErrMsg;
        string NewTraceID, IsNotify;

        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            string thisDate = DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd");

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 設定追蹤編號(類別自訂編號(2)-日期(8)-流水號(4), 14碼)
            SBSql.AppendLine(" DECLARE @CustomCID AS varchar(2), @NewID AS varchar(4), @IsNotify AS char(1) ");
            //自訂編號
            SBSql.AppendLine(" SET @CustomCID = (");
            SBSql.AppendLine("  SELECT Custom_ID FROM OP_ParamClass WHERE Class_ID = @Class_ID ");
            SBSql.AppendLine(" );");
            //是否通知主管
            SBSql.AppendLine(" SET @IsNotify = (");
            SBSql.AppendLine("  SELECT Notify FROM OP_ParamClass WHERE Class_ID = @Class_ID ");
            SBSql.AppendLine(" );");
            //New ID
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(CAST(RIGHT(TraceID ,4) AS INT)) ,0) + 1 FROM OP_Help WHERE (SUBSTRING(TraceID,3,8) = @thisDate)");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" SELECT (@CustomCID + @thisDate + RIGHT('000' + @NewID, 4)) AS NewTraceID, @IsNotify AS IsNotify ");

            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Class_ID", this.ddl_Req_Class.SelectedValue);
            cmd.Parameters.AddWithValue("thisDate", thisDate);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                NewTraceID = DT.Rows[0]["NewTraceID"].ToString();
                IsNotify = DT.Rows[0]["IsNotify"].ToString();
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO OP_Help( ");
            SBSql.AppendLine("  TraceID, Req_Class, Req_Who, Req_Dept, Help_Subject, Help_Content, Help_Status");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @TraceID, @Req_Class, @Req_Who, @Req_Dept, @Help_Subject, @Help_Content, @Help_Status");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("TraceID", NewTraceID);
            cmd.Parameters.AddWithValue("Req_Class", this.ddl_Req_Class.SelectedValue);
            cmd.Parameters.AddWithValue("Req_Who", this.tb_EmpValue.Text);
            cmd.Parameters.AddWithValue("Req_Dept", this.ddl_Dept.SelectedValue);
            cmd.Parameters.AddWithValue("Help_Subject", fn_stringFormat.Filter_Html(this.tb_Help_Subject.Text));
            cmd.Parameters.AddWithValue("Help_Content", this.tb_Help_Content.Text);
            cmd.Parameters.AddWithValue("Help_Status", 9);  // OP_ParamClass, 待處理的ID = 9 (這是固定值)
            cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserAccount);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                Response.Write(ErrMsg);
                fn_Extensions.JsAlert("資料新增失敗！", PageUrl);
                return;
            }
        }

        //本頁網址, 含編號
        string NewUri = "OP_HelpEdit.aspx?TraceID={0}".FormatThis(Server.UrlEncode(Cryptograph.MD5Encrypt(NewTraceID, DesKey)));
        if (false == AttachFiles(ITempList, NewTraceID, out ErrMsg))
        {
            fn_Extensions.JsAlert("附件上傳失敗，請重新上傳！", NewUri);
            return;
        }
        else
        {
            //發信給指定人員(OP_Help_Receiver, MailType=1)
            SendMail_Req(NewTraceID, fn_stringFormat.Filter_Html(this.tb_Help_Subject.Text));

            //發送通知信給主管(權限申請)
            if (IsNotify.Equals("Y"))
            {
                SendMail_InfoSupervisor(NewTraceID, fn_stringFormat.Filter_Html(this.tb_Help_Subject.Text), this.ddl_Dept.SelectedValue);
            }

            //新增CC名單, 發信給CC名單
            if (CCMails(NewTraceID))
            {
                SendMail_Req_byCC(NewTraceID, fn_stringFormat.Filter_Html(this.tb_Help_Subject.Text));
            }

            //導向列表頁
            Response.Redirect(Session["BackListUrl"].ToString());
            return;
        }
    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data(List<TempParam> ITempList)
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE OP_Help ");
            SBSql.AppendLine(" SET Req_Class = @Req_Class ");
            SBSql.AppendLine("  , Req_Who = @Req_Who, Req_Dept = @Req_Dept, Help_Subject = @Help_Subject, Help_Content = @Help_Content");
            SBSql.AppendLine("  , Reply_Hours = @Reply_Hours, Reply_Content = @Reply_Content, Reply_Date = @Reply_Date");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine("  , IsAgree = @IsAgree");

            //主管
            if (this.rbl_IsAgree.SelectedValue.Equals("Y"))
            {
                SBSql.Append(", Agree_Time = GETDATE()");
            }

            SBSql.AppendLine(" WHERE (TraceID = @TraceID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("TraceID", Param_thisID);
            cmd.Parameters.AddWithValue("Req_Class", this.ddl_Req_Class.SelectedValue);
            cmd.Parameters.AddWithValue("Req_Who", this.tb_EmpValue.Text);
            cmd.Parameters.AddWithValue("Req_Dept", this.ddl_Dept.SelectedValue);
            cmd.Parameters.AddWithValue("Help_Subject", this.tb_Help_Subject.Text);
            cmd.Parameters.AddWithValue("Help_Content", this.tb_Help_Content.Text);
            cmd.Parameters.AddWithValue("Reply_Hours", string.IsNullOrEmpty(this.tb_Reply_Hours.Text) ? DBNull.Value : (Object)this.tb_Reply_Hours.Text);
            cmd.Parameters.AddWithValue("Reply_Content", this.tb_Reply_Content.Text);
            cmd.Parameters.AddWithValue("Reply_Date", string.IsNullOrEmpty(this.tb_Reply_Date.Text) ? DBNull.Value : (Object)this.tb_Reply_Date.Text);
            cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserAccount);
            cmd.Parameters.AddWithValue("IsAgree", this.rbl_IsAgree.SelectedValue);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }
            //本頁網址, 含編號
            string NewUri = "OP_HelpEdit.aspx?TraceID={0}".FormatThis(Server.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey)));
            if (false == AttachFiles(ITempList, Param_thisID, out ErrMsg))
            {
                fn_Extensions.JsAlert("附件上傳失敗！", NewUri);
                return;
            }
            else
            {
                //導向列表頁
                Response.Redirect(Session["BackListUrl"].ToString());
                return;
            }
        }
    }

    /// <summary>
    /// 新增附件
    /// </summary>
    /// <param name="ITempList"></param>
    /// <param name="TraceID"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool AttachFiles(List<TempParam> ITempList, string TraceID, out string ErrMsg)
    {
        if (ITempList.Count == 0)
        {
            ErrMsg = "";
            return true;
        }

        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - Statement
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" Declare @New_ID AS INT ");

            for (int i = 0; i < ITempList.Count; i++)
            {
                SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(AttachID), 0) + 1 FROM OP_Help_Attach) ");
                SBSql.AppendLine(" INSERT INTO OP_Help_Attach( ");
                SBSql.AppendLine("  AttachID, TraceID, AttachFile");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @New_ID, @TraceID");
                SBSql.AppendLine(string.Format(", @FileNewName_{0} ", i));
                SBSql.AppendLine(" ); ");
                cmd.Parameters.AddWithValue("FileNewName_" + i, ITempList[i].Param_Pic);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("TraceID", TraceID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                return false;
            }
            else
            {
                //[IO] - 儲存檔案
                for (int i = 0; i < ITempList.Count; i++)
                {
                    HttpPostedFile hpf = ITempList[i].Param_hpf;
                    if (hpf.ContentLength > 0)
                    {
                        IOManage.Save(hpf, Param_FileFolder, ITempList[i].Param_Pic, Param_Width, Param_Height);
                    }
                }

                return true;
            }
        }

    }


    /// <summary>
    /// 新增自訂通知人員
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private bool CCMails(string traceID)
    {
        //[欄位檢查] - 取得勾選值
        string inputValue = this.tb_Emps.Text;
        if (string.IsNullOrEmpty(inputValue))
        {
            return false;
        }

        //[取得參數值] - 編號組合(工號)
        string[] strAry = Regex.Split(inputValue, @"\|{2}");
        var query = from el in strAry
                    select new
                    {
                        Val = el.ToString().Trim()
                    };

        //[資料儲存]
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 取得新系統序號
            SBSql.AppendLine("DECLARE @NewID AS INT ");

            //[SQL] - 資料新增
            foreach (var item in query)
            {
                //-- 產生序號 --
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(Item_ID) ,0) + 1 FROM OP_Help_CC WHERE (TraceID = @TraceID)");
                SBSql.AppendLine(" );");

                //新增資料
                SBSql.Append(" INSERT INTO OP_Help_CC( ");
                SBSql.Append("  TraceID, Item_ID, Item_Name, Item_Email");
                SBSql.Append(" )");
                SBSql.Append(" SELECT @TraceID, @NewID, Account_Name, Email");
                SBSql.Append(" FROM PKSYS.dbo.User_Profile ");
                SBSql.Append(" WHERE (Email IS NOT NULL) AND (Email <> '') AND (Account_Name = N'{0}'); ".FormatThis(item.Val));

            }
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("TraceID", traceID);

            return dbConn.ExecuteSql(cmd, out ErrMsg);
        }
    }


    /// <summary>
    /// 回覆並通知
    /// </summary>
    protected void btn_Inform_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Help_Content.Text, "1", "1000", out ErrMsg) == false)
            {
                SBAlert.Append("「詳細說明」請輸入1 ~ 500個字\\n");
            }
            if (string.IsNullOrEmpty(this.tb_Reply_Date.Text) || this.tb_Reply_Date.Text.IsDate() == false)
            {
                SBAlert.Append("「回覆日期」沒填寫\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "資料儲存"
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" UPDATE OP_Help SET Help_Status = 10 ");
                SBSql.AppendLine("  , Reply_Hours = @Reply_Hours, Reply_Content = @Reply_Content, Reply_Date = @Reply_Date ");
                SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Hours", string.IsNullOrEmpty(this.tb_Reply_Hours.Text) ? DBNull.Value : (Object)this.tb_Reply_Hours.Text);
                cmd.Parameters.AddWithValue("Reply_Content", this.tb_Reply_Content.Text);
                cmd.Parameters.AddWithValue("Reply_Date", this.tb_Reply_Date.Text);
                cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserAccount);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("回覆失敗！", PageUrl);
                    return;
                }
                else
                {
                    //發信給需求者
                    SendMail_Inform(Param_thisID, fn_stringFormat.Filter_Html(this.tb_Help_Subject.Text), this.tb_Email.Text);

                    //導向列表頁
                    Response.Redirect(Session["BackListUrl"].ToString());
                    return;
                }

            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 回覆", "");
            return;
        }

    }

    /// <summary>
    /// 結案
    /// </summary>
    protected void btn_Done_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (string.IsNullOrEmpty(this.tb_Reply_Hours.Text))
            {
                SBAlert.Append("「處理工時」沒填寫\\n");
            }
            if (fn_Extensions.String_資料長度Byte(this.tb_Help_Content.Text, "1", "1000", out ErrMsg) == false)
            {
                SBAlert.Append("「詳細說明」請輸入1 ~ 500個字\\n");
            }
            if (string.IsNullOrEmpty(this.tb_Reply_Date.Text) || this.tb_Reply_Date.Text.IsDate() == false)
            {
                SBAlert.Append("「回覆日期」沒填寫\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "資料儲存"
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" UPDATE OP_Help SET Help_Status = 11 ");
                SBSql.AppendLine("  , Reply_Hours = @Reply_Hours, Reply_Content = @Reply_Content, Reply_Date = @Reply_Date ");
                SBSql.AppendLine("  , onTop = 'N', Update_Who = @Update_Who, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Hours", string.IsNullOrEmpty(this.tb_Reply_Hours.Text) ? DBNull.Value : (Object)this.tb_Reply_Hours.Text);
                cmd.Parameters.AddWithValue("Reply_Content", this.tb_Reply_Content.Text);
                cmd.Parameters.AddWithValue("Reply_Date", this.tb_Reply_Date.Text);
                cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserAccount);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("結案失敗！", PageUrl);
                    return;
                }
                else
                {
                    //發信給需求者
                    SendMail_Done(Param_thisID, fn_stringFormat.Filter_Html(this.tb_Help_Subject.Text), this.tb_Email.Text);

                    //導向列表頁
                    Response.Redirect(Session["BackListUrl"].ToString());
                    return;
                }

            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 結案", "");
            return;
        }

    }

    /// <summary>
    /// 後悔 - 變更為「處理中」
    /// </summary>
    protected void btn_Back_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" UPDATE OP_Help SET Help_Status = 10 ");
                SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
                cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserAccount);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", PageUrl);
                    return;
                }
                else
                {
                    fn_Extensions.JsAlert("已將狀態設為「處理中」", PageUrl);
                    return;
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 後悔", "");
            return;
        }

    }


    /// <summary>
    /// 設為置頂, 結案後會取消
    /// </summary>
    protected void btn_onTop_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" UPDATE OP_Help SET onTop = 'Y' ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("設為置頂失敗！", PageUrl);
                    return;
                }
                else
                {
                    fn_Extensions.JsAlert("已設為置頂", PageUrl);
                    return;
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設為置頂", "");
            return;
        }

    }

    #endregion -- 資料編輯 End --

    #region -- 其他功能 --
    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <param name="ReSize">是否顯示放大圖</param>
    /// <returns>string</returns>
    public string PicUrl(string PicName, bool ReSize)
    {
        string preView = "";

        if (PicName.ToLower().IndexOf(".jpg") > 0 || PicName.ToLower().IndexOf(".png") > 0)
        {
            //圖片預覽
            if (ReSize)
            {
                preView = string.Format(
                   "<a class=\"PicGroup L2Img\" rel=\"PicGroup\" href=\"{0}\"><img src=\"{0}\" border=\"0\" style=\"width:120px\" alt=\"Pic\"></a>"
                   , Param_WebFolder + PicName);
            }
            else
            {
                preView = string.Format(
                   "<img src=\"{0}\" border=\"0\" width=\"300\" alt=\"Pic\" style=\"padding-right:5px;\">"
                   , Param_WebFolder + PicName);
            }
        }
        else
        {
            preView = string.Format(
                 "<a href=\"{0}\">{1}</a>"
                 , Param_WebFolder + PicName
                 , PicName);
        }

        //輸出Html
        return preView;
    }


    /// <summary>
    /// 顯示附件(EMail Html)
    /// </summary>
    /// <returns></returns>
    private string showAttachments(string TraceID)
    {
        try
        {
            StringBuilder showImg = new StringBuilder();
            showImg.Append("<ul>");

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT AttachFile FROM OP_Help_Attach WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("TraceID", TraceID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        showImg.Append("<li>{0}</li>".FormatThis(PicUrl(DT.Rows[row]["AttachFile"].ToString(), false)));
                    }
                }
            }

            showImg.Append("</ul>");
            return showImg.ToString();
        }
        catch (Exception)
        {
            return "錯誤 - 無法正常顯示附件";
        }
    }

    #endregion


    #region -- 發信 --

    /// <summary>
    /// [需求登記] 通知客服單位(New Task)(1)
    /// </summary>
    /// <param name="TraceID">追蹤編號</param>
    /// <param name="Subject">主旨</param>
    private void SendMail_Req(string TraceID, string Subject)
    {
        //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = MailFuncName;

        //[設定參數] - 寄件人
        fn_Mail.Sender = MailSender;

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = MailSenderName;

        //[設定參數] - 收件人
        fn_Mail.Reciever = fn_CustomUI.OP_emailReceiver("1");

        //[設定參數] - 轉寄人群組
        fn_Mail.CC = null;

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = "[品號需求登記] {0};追蹤編號:{1}".FormatThis(Subject, convertTraceID(TraceID));

        //[設定參數] - 郵件內容
        fn_Mail.MailBody = GetMailBody("1", TraceID);

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();

        //[判斷參數] - 寄件是否成功
        if (!fn_Mail.MessageCode.Equals(200))
        {
            //Response.Write(fn_Mail.Message);
            fn_Extensions.JsAlert("發生錯誤 - 發信", "");
            return;
        }
    }

    /// <summary>
    /// [需求登記] CC(5)
    /// </summary>
    /// <param name="TraceID">追蹤編號</param>
    /// <param name="Subject">主旨</param>
    private void SendMail_Req_byCC(string TraceID, string Subject)
    {
        //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = MailFuncName;

        //[設定參數] - 寄件人
        fn_Mail.Sender = MailSender;

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = MailSenderName;

        //[設定參數] - 收件人
        fn_Mail.Reciever = fn_CustomUI.OP_emailCC(TraceID);

        //[設定參數] - 轉寄人群組
        fn_Mail.CC = null;

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = "[品號需求登記][轉寄通知] {0};追蹤編號:{1}".FormatThis(Subject, convertTraceID(TraceID));

        //[設定參數] - 郵件內容
        fn_Mail.MailBody = GetMailBody("5", TraceID);

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();

        //[判斷參數] - 寄件是否成功
        if (!fn_Mail.MessageCode.Equals(200))
        {
            //Response.Write(fn_Mail.Message);
            fn_Extensions.JsAlert("發生錯誤 - CC發信", "");
            return;
        }
    }

    /// <summary>
    /// [需求登記] 通知信給主管同意(New Task)(1)
    /// </summary>
    /// <param name="TraceID">追蹤編號</param>
    /// <param name="Subject">主旨</param>
    private void SendMail_InfoSupervisor(string TraceID, string Subject, string DeptID)
    {
        //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = MailFuncName;

        //[設定參數] - 寄件人
        fn_Mail.Sender = MailSender;

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = MailSenderName;

        //[設定參數] - 收件人(需求者部門主管)
        fn_Mail.Reciever = fn_CustomUI.emailReceiver_Supervisor(DeptID);

        //[設定參數] - 轉寄人群組
        fn_Mail.CC = null;

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = "[品號需求登記][核准通知] {0};追蹤編號:{1}".FormatThis(Subject,  convertTraceID(TraceID));

        //[設定參數] - 郵件內容
        fn_Mail.MailBody = GetMailBody("2", TraceID);

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();

        //[判斷參數] - 寄件是否成功
        if (!fn_Mail.MessageCode.Equals(200))
        {
            //Response.Write(fn_Mail.Message);
            fn_Extensions.JsAlert("發生錯誤 - 申請發信", "");
            return;
        }
    }

    /// <summary>
    /// [回覆通知] 寄回覆確認信(僅回覆)(3)
    /// </summary>
    /// <param name="TraceID">追蹤編號</param>
    /// <param name="Subject">主旨</param>
    /// <param name="ReqEMail">需求者EMail</param>
    private void SendMail_Inform(string TraceID, string Subject, string ReqEMail)
    {
        if (string.IsNullOrEmpty(ReqEMail))
        {
            fn_Extensions.JsAlert("需求者EMail是空白，無法寄送通知信", "");
            return;
        }

        //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = MailFuncName;

        //[設定參數] - 寄件人
        fn_Mail.Sender = MailSender;

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = MailSenderName;

        //[設定參數] - 收件人
        List<string> emailTo = new List<string>();
        emailTo.Add(ReqEMail);

        fn_Mail.Reciever = emailTo;

        //[設定參數] - 轉寄人群組
        fn_Mail.CC = fn_CustomUI.OP_emailCC(TraceID);

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = "[品號需求登記][回覆通知] {0};追蹤編號:{1}".FormatThis(Subject,  convertTraceID(TraceID));

        //[設定參數] - 郵件內容
        fn_Mail.MailBody = GetMailBody("3", TraceID);

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();

        //[判斷參數] - 寄件是否成功
        if (!fn_Mail.MessageCode.Equals(200))
        {
            //Response.Write(fn_Mail.Message);
            fn_Extensions.JsAlert("發生錯誤 - 回覆通知", "");
            return;
        }
    }

    /// <summary>
    /// [結案通知] 寄結案信(4)
    /// </summary>
    /// <param name="TraceID">追蹤編號</param>
    /// <param name="Subject">主旨</param>
    /// <param name="ReqEMail">需求者EMail</param>
    private void SendMail_Done(string TraceID, string Subject, string ReqEMail)
    {
        if (string.IsNullOrEmpty(ReqEMail))
        {
            fn_Extensions.JsAlert("需求者EMail是空白，無法寄送通知信", "");
            return;
        }

        //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = MailFuncName;

        //[設定參數] - 寄件人
        fn_Mail.Sender = MailSender;

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = MailSenderName;

        //[設定參數] - 收件人
        List<string> emailTo = new List<string>();
        emailTo.Add(ReqEMail);

        fn_Mail.Reciever = emailTo;


        //[設定參數] - 轉寄人群組(結案通知)
        List<string> emailToCC = new List<string>();
        emailToCC.AddRange(fn_CustomUI.OP_emailReceiver("2"));
        emailToCC.AddRange(fn_CustomUI.OP_emailCC(TraceID));

        fn_Mail.CC = emailToCC;

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = "[品號需求登記][結案通知] {0};追蹤編號:{1}".FormatThis(Subject, convertTraceID(TraceID));

        //[設定參數] - 郵件內容
        fn_Mail.MailBody = GetMailBody("4",TraceID);

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();

        //[判斷參數] - 寄件是否成功
        if (!fn_Mail.MessageCode.Equals(200))
        {
            //Response.Write(fn_Mail.Message);
            fn_Extensions.JsAlert("發生錯誤 - 結案通知", "");
            return;
        }
    }

    /// <summary>
    /// 取得信件內容
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <remarks>
    /// [type]
    /// 1:通知客服單位 / 2:權限核准 / 3:回覆通知 / 4:結案通知 / 5:新需求轉寄
    /// </remarks>
    private StringBuilder GetMailBody(string type, string traceID)
    {
        StringBuilder mailBody = new StringBuilder();
        StringBuilder SBSql = new StringBuilder();

        switch (type)
        {
            case "1":
                #region >> 通知客服單位 <<

                using (SqlCommand cmd = new SqlCommand())
                {
                    SBSql.AppendLine(" SELECT Main.* ");
                    SBSql.AppendLine("  , ReqClass.Class_Name AS HClass, HelpStatus.Class_Name AS HStatus ");
                    SBSql.AppendLine("  , Prof.Account_Name, Prof.Display_Name ");
                    SBSql.AppendLine(" FROM OP_Help Main ");
                    SBSql.AppendLine("  INNER JOIN OP_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID ");
                    SBSql.AppendLine("  INNER JOIN OP_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID ");
                    SBSql.AppendLine("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Main.Req_Who = Prof.Account_Name ");
                    SBSql.AppendLine(" WHERE (TraceID = @TraceID); ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("TraceID", traceID);
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count > 0)
                        {
                            mailBody.AppendLine("<div>");
                            mailBody.AppendLine("<h4>【品號需求登記】</h4>");
                            mailBody.AppendLine("</div>");
                            mailBody.AppendLine("<div>");
                            mailBody.AppendLine("    <table style=\"background: #dfebc7;border-spacing: 1px;line-height: 25px;width: 100%; font-size:12px;\">");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"width:100px;background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">追蹤編號</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;color: #f15b61;font-size: 13px;font-weight: bold;\">{0}</td>".FormatThis(convertTraceID(traceID)));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">需求者</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">({0}) {1}</td>".FormatThis(DT.Rows[0]["Account_Name"].ToString(), DT.Rows[0]["Display_Name"].ToString()));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">問題類別</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">{0}</td>".FormatThis(DT.Rows[0]["HClass"].ToString()));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">填寫主旨</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">{0}</td>".FormatThis(DT.Rows[0]["Help_Subject"].ToString()));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">有圖有真相</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\" valign=\"top\">{0}</td>".FormatThis(showAttachments(traceID)));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">詳細說明</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">{0}</td>".FormatThis(DT.Rows[0]["Help_Content"].ToString().Replace("\r", "<br/>")));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("    </table>");
                            mailBody.AppendLine("</div>");
                            mailBody.AppendLine("<div style=\"background: #f3f3f3; border-top: 3px solid #e8e8e8; padding: 5px 10px; text-align: center;\">");
                            mailBody.Append("    <a href=\"{0}\" style=\"text-decoration:none;font-weight:bold;\">&raquo;&raquo;&raquo; 知道了，來去處理 &laquo;&laquo;&laquo;</a>".FormatThis(
                                "{0}?t=OPHelp_Reply&dataID={1}".FormatThis(Application["WebUrl"].ToString(), Server.UrlEncode(Cryptograph.MD5Encrypt(traceID, DesKey)))
                                ));
                            mailBody.AppendLine("</div>");
                        }
                    }
                }

                #endregion

                return mailBody;


            case "2":
                //權限核准
                mailBody.AppendLine("<h3>目前有一筆需求登記申請,需要您的同意.</h3>");
                mailBody.AppendLine("<div style=\"background: #f3f3f3; padding: 10px 10px; text-align: center;\">");
                mailBody.AppendLine("<a href=\"{0}\" style=\"text-decoration:none;font-weight:bold;\">&raquo;&raquo;&raquo; 請點我前往處理 &laquo;&laquo;&laquo;</a>".FormatThis(
                    "{0}myOPHelp/Req_Process.aspx?dataID={1}".FormatThis(Application["WebUrl"].ToString(), traceID)
                    ));
                mailBody.AppendLine("</div>");

                return mailBody;


            case "3":
                //回覆通知
                mailBody.AppendLine("<div style=\"padding-top:10px;\">{0}</div>".FormatThis("您的需求登記正在處理中，<br/>目前尚有些問題待確認，{0}。"
                    .FormatThis("<a href=\"{0}\" style=\"text-decoration:none;font-weight:bold;\">請點我查看，並提供更詳細的資料</a>"
                        .FormatThis(
                            "{0}?t=OPHelp_View&dataID={1}".FormatThis(Application["WebUrl"].ToString(), Server.UrlEncode(Cryptograph.MD5Encrypt(traceID, DesKey)))
                        ))));
                mailBody.AppendLine("<br/><br/><div>追蹤編號：<span style=\"color:blue;\">{0}</span></div>".FormatThis(traceID));


                return mailBody;


            case "4":
                //結案通知
                mailBody.AppendLine("<div style=\"padding-top:10px;\">{0}</div>".FormatThis("您的需求登記已結案，{0}。"
                   .FormatThis("<a href=\"{0}\" style=\"text-decoration:none;font-weight:bold;\">請點我查看，並填寫滿意度評分</a>"
                       .FormatThis(
                           "{0}?t=OPHelp_View&dataID={1}".FormatThis(Application["WebUrl"].ToString(), Server.UrlEncode(Cryptograph.MD5Encrypt(traceID, DesKey)))
                       ))));
                mailBody.AppendLine("<br/><br/><div>追蹤編號：<span style=\"color:blue;\">{0}</span></div>".FormatThis(traceID));


                return mailBody;


            case "5":
                #region >> 新需求轉寄 <<

                using (SqlCommand cmd = new SqlCommand())
                {
                    SBSql.AppendLine(" SELECT Main.* ");
                    SBSql.AppendLine("  , ReqClass.Class_Name AS HClass, HelpStatus.Class_Name AS HStatus ");
                    SBSql.AppendLine("  , Prof.Account_Name, Prof.Display_Name ");
                    SBSql.AppendLine(" FROM OP_Help Main ");
                    SBSql.AppendLine("  INNER JOIN OP_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID ");
                    SBSql.AppendLine("  INNER JOIN OP_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID ");
                    SBSql.AppendLine("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Main.Req_Who = Prof.Account_Name ");
                    SBSql.AppendLine(" WHERE (TraceID = @TraceID); ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("TraceID", traceID);
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count > 0)
                        {
                            mailBody.AppendLine("<div>");
                            mailBody.AppendLine("<h4>【品號需求登記】</h4>");
                            mailBody.AppendLine("</div>");
                            mailBody.AppendLine("<div>");
                            mailBody.AppendLine("    <table style=\"background: #dfebc7;border-spacing: 1px;line-height: 25px;width: 100%; font-size:12px;\">");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"width:100px;background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">追蹤編號</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;color: #f15b61;font-size: 13px;font-weight: bold;\">{0}</td>".FormatThis(convertTraceID(traceID)));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">需求者</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">({0}) {1}</td>".FormatThis(DT.Rows[0]["Account_Name"].ToString(), DT.Rows[0]["Display_Name"].ToString()));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">問題類別</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">{0}</td>".FormatThis(DT.Rows[0]["HClass"].ToString()));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">填寫主旨</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">{0}</td>".FormatThis(DT.Rows[0]["Help_Subject"].ToString()));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">有圖有真相</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\" valign=\"top\">{0}</td>".FormatThis(showAttachments(traceID)));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("        <tr>");
                            mailBody.AppendLine("            <td style=\"background: #f5f9e8; border-left: 4px solid #dfeac3; color: #72993e; padding-right: 5px; text-align: right;\">詳細說明</td>");
                            mailBody.AppendLine("            <td style=\"background: #fff; padding: 5px;\">{0}</td>".FormatThis(DT.Rows[0]["Help_Content"].ToString().Replace("\r", "<br/>")));
                            mailBody.AppendLine("        </tr>");
                            mailBody.AppendLine("    </table>");
                            mailBody.AppendLine("</div>");
                        }
                    }
                }

                #endregion

                return mailBody;

            default:
                return mailBody;
        }
    }

    private string convertTraceID(string traceID)
    {
        return traceID.Insert(2, "-").Insert(11, "-");
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 我的部門代號
    /// </summary>
    private string _Param_DeptID;
    public string Param_DeptID
    {
        get
        {
            //取得目前使用者的部門
            return ADService.getDepartmentFromGUID(fn_Params.UserGuid);
        }
        set
        {
            this._Param_DeptID = value;
        }
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    private string DesKey
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

    /// <summary>
    /// 本筆資料的編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["TraceID"]) ? "" : Cryptograph.MD5Decrypt(Request.QueryString["TraceID"].ToString(), DesKey);
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
            return string.Format(@"OP_HelpEdit.aspx?TraceID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"PKEF\OP_Help\";
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"PKEF/OP_Help/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png|docx|xlsx|pptx|pdf";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 1280;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 1024;
        }
        set
        {
            this._Param_Height = value;
        }
    }
    #endregion


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
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
        /// <param name="Param_Pic">系統檔名</param>
        /// <param name="Param_OrgPic">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
        }
    }


}