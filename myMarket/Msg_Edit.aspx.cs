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

public partial class Msg_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 網站訊息
                if (fn_CheckAuth.CheckAuth_User("610", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "myMarket/Msg_Search.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料處理中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");
                this.btn_Reply.Attributes["onclick"] = fn_Extensions.BlockJs(
                   "Add",
                   "<div style=\"text-align:left\">資料處理中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");
                this.btn_Finish.Attributes["onclick"] = fn_Extensions.BlockJs(
                   "Add",
                   "<div style=\"text-align:left\">資料處理中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept, "", true, false, new List<string> { "TW", "SH", "SZ" }, out ErrMsg) == false)
                {
                    this.ddl_Dept.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 部門
                if (fn_Extensions.Menu_Dept(this.ddl_Dept_Sales, "", true, false, new List<string> { "TW", "SH", "SZ" }, out ErrMsg) == false)
                {
                    this.ddl_Dept_Sales.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //讀取資料
                View_Data();
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

                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("   Cls.Class_Name, Base.InquiryID, Base.Message, Base.Status, St.Class_Name AS StName, Base.TraceID ");
                SBSql.AppendLine("   , Base.Reply_Subject, Base.Create_Time ");
                SBSql.AppendLine("   , (SELECT Email FROM PKSYS.dbo.User_Profile WHERE (Guid = @LoginGuid)) AS PubSender_Email ");
                //--會員資料
                SBSql.AppendLine("   , ISNULL(MD.Mem_Account, Base.MsgEmail) AS MemberMail, MD.Company, ISNULL(MD.LastName, Base.MsgWho) AS LastName, MD.FirstName, MD.Sex ");
                SBSql.AppendLine("   , MD.Birthday, GC.Country_Name, MD.Address, MD.Tel, MD.Mobile");
                SBSql.AppendLine("   , MD.IM_QQ, MD.IM_Wechat");
                //--回覆資料
                SBSql.AppendLine("   , (SELECT Email FROM PKSYS.dbo.User_Profile WHERE (Guid IN ( ");
                SBSql.AppendLine("       SELECT TOP 1 Reply.Create_Who FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC");
                SBSql.AppendLine("    )");
                SBSql.AppendLine("   )) AS Reply_Email ");
                SBSql.AppendLine("   , (SELECT TOP 1 Reply.Create_Time FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS Reply_Time ");
                SBSql.AppendLine("   , (SELECT TOP 1 Reply.Reply_Message FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS Reply_Message");
                SBSql.AppendLine("   , (SELECT TOP 1 Reply.ReplyID FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS ReplyID");
                SBSql.AppendLine("   , (SELECT TOP 1 AreaName FROM PKSYS.dbo.Param_Area WHERE (Param_Area.AreaCode = Base.AreaCode) AND (UPPER(LangCode) = 'ZH-TW')) AS AreaName ");
                SBSql.AppendLine(" FROM Inquiry Base ");
                SBSql.AppendLine("   INNER JOIN Inquiry_Class Cls ON Base.Class_ID = Cls.Class_ID AND Cls.LangCode = 'zh-TW' ");
                SBSql.AppendLine("   INNER JOIN Inquiry_Status St ON Base.Status = St.Class_ID ");
                SBSql.AppendLine("   LEFT JOIN Member_Data AS MD ON MD.Mem_ID = Base.Mem_ID ");
                SBSql.AppendLine("   LEFT JOIN Geocode_CountryName GC ON MD.Country_Code = GC.Country_Code AND GC.LangCode = 'zh-tw'");
                SBSql.AppendLine(" WHERE (Base.InquiryID = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("LoginGuid", fn_Params.UserGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.lt_TraceID.Text = DT.Rows[0]["TraceID"].ToString();
                        this.lb_Class.Text = DT.Rows[0]["Class_Name"].ToString();
                        this.lb_AreaName.Text = DT.Rows[0]["AreaName"].ToString();
                        this.lt_Message.Text = DT.Rows[0]["Message"].ToString().Replace("\n", "<br/>");
                        //寄件人Email:若尚未回覆，則帶目前人員的Mail
                        this.lt_ReplyMailSender.Text = string.IsNullOrEmpty(DT.Rows[0]["Reply_Email"].ToString()) ? DT.Rows[0]["PubSender_Email"].ToString() : DT.Rows[0]["Reply_Email"].ToString();
                        this.lt_MemberMail.Text = DT.Rows[0]["MemberMail"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Reply_Time.Text = DT.Rows[0]["Reply_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.tb_Subject.Text = DT.Rows[0]["Reply_Subject"].ToString();
                        this.tb_Reply_Message.Text = DT.Rows[0]["Reply_Message"].ToString();
                        this.hf_ReplyID.Value = DT.Rows[0]["ReplyID"].ToString();
                        this.hf_Status.Value = DT.Rows[0]["Status"].ToString();

                        //會員資料
                        this.modal_Email.Text = DT.Rows[0]["MemberMail"].ToString();
                        this.modal_Company.Text = DT.Rows[0]["Company"].ToString();
                        this.modal_LastName.Text = DT.Rows[0]["LastName"].ToString();
                        this.modal_FirstName.Text = DT.Rows[0]["FirstName"].ToString();
                        this.modal_Sex.Text = fn_Desc.PubAll.Sex(DT.Rows[0]["Sex"].ToString());
                        this.modal_Birthday.Text = DT.Rows[0]["Birthday"].ToString().ToDateString("yyyy-MM-dd");
                        this.modal_Country.Text = DT.Rows[0]["Country_Name"].ToString();
                        this.modal_Address.Text = DT.Rows[0]["Address"].ToString();
                        this.modal_Tel.Text = DT.Rows[0]["Tel"].ToString();
                        this.modal_Mobile.Text = DT.Rows[0]["Mobile"].ToString();
                        this.modal_qq.Text = DT.Rows[0]["IM_QQ"].ToString();
                        this.modal_wechat.Text = DT.Rows[0]["IM_Wechat"].ToString();

                        //取得狀態
                        this.hf_Current_Status.Value = DT.Rows[0]["Status"].ToString();
                        string myStatus = DT.Rows[0]["Status"].ToString();
                        string setCss;

                        //判斷狀態(查看Inquiry_Status), 給予不同的顏色
                        switch (myStatus)
                        {
                            case "1":
                                setCss = "label label-info";

                                break;

                            case "2":
                                setCss = "label label-warning";

                                break;

                            case "4":
                                setCss = "label label-default";

                                break;

                            case "5":
                                setCss = "label label-default";

                                break;

                            default:
                                setCss = "label label-success";

                                break;
                        }
                        this.lb_Status.Text = DT.Rows[0]["StName"].ToString();
                        this.lb_Status.CssClass = setCss;

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";


                        //顯示轉寄對象
                        LookupData_Rel("1", this.lt_EmpItems);
                        LookupData_Rel("2", this.lt_OtherItems);
                        LookupData_Rel("3", this.lt_SalesItems);

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
    /// 顯示關聯資料
    /// </summary>
    private void LookupData_Rel(string type, Literal showHtml)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT CC_Who AS myLabel, CC_EMail AS myValue ");
                SBSql.AppendLine(" FROM Inquiry_CC ");
                SBSql.AppendLine(" WHERE (InquiryID = @DataID) AND (CC_Type = @CC_Type) ");
                SBSql.AppendLine(" ORDER BY CC_Who ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("CC_Type", type);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        StringBuilder itemHtml = new StringBuilder();

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //取得參數
                            string myLabel = DT.Rows[row]["myLabel"].ToString();
                            string myValue = DT.Rows[row]["myValue"].ToString();
                            string idLabel, idValue;
                            switch (type)
                            {
                                case "1":
                                    idLabel = "Empitem_Name";
                                    idValue = "Empitem_Val";
                                    break;

                                case "2":
                                    idLabel = "Otheritem_Name";
                                    idValue = "Otheritem_Val";
                                    break;

                                default:
                                    idLabel = "Salesitem_Name";
                                    idValue = "Salesitem_Val";
                                    break;
                            }
                            //組合Html
                            itemHtml.AppendLine("<li id=\"li_{0}{1}\" style=\"padding-top:5px;\">".FormatThis(type, row));
                            itemHtml.Append("<input type=\"hidden\" class=\"{1}\" value=\"{0}\" />".FormatThis(myLabel, idLabel));
                            itemHtml.Append("<input type=\"hidden\" class=\"{1}\" value=\"{0}\" />".FormatThis(myValue, idValue));
                            itemHtml.Append("<a href=\"javascript:Delete_Item('{0}{1}');\" class=\"btn btn-default\" title=\"{3}\">{2}&nbsp;<span class=\"glyphicon glyphicon-trash\"></span></a>"
                                .FormatThis(type, row, myLabel, myValue));
                            itemHtml.AppendLine("</li>");
                        }

                        showHtml.Text = itemHtml.ToString();

                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取關聯資料！", "");
        }
    }
    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 按鈕 - 存檔 (目前狀態 = 待處理(1)/處理中(2))
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Subject.Text, "1", "100", out ErrMsg) == false)
            {
                SBAlert.Append("「信件主旨」請輸入1 ~ 50個字\\n");
            }
            if (fn_Extensions.String_資料長度Byte(this.tb_Reply_Message.Text, "1", "3000", out ErrMsg) == false)
            {
                SBAlert.Append("「回覆內文」請輸入1 ~ 3000個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            //設定轉寄名單 - 內部員工
            if (Set_DataRel(Param_thisID, this.val_EmpEmail.Text, this.val_EmpName.Text, "1") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 內部員工！", "");
                return;
            }

            //設定轉寄名單 - 自訂名單
            if (Set_DataRel(Param_thisID, this.val_OtherEmail.Text, this.val_OtherName.Text, "2") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 自訂名單！", "");
                return;
            }

            //設定轉寄名單 - 承辦人員
            if (Set_DataRel(Param_thisID, this.val_SalesEmail.Text, this.val_SalesName.Text, "3") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 承辦人員！", "");
                return;
            }

            //資料儲存
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料更新, 2=處理中
                SBSql.AppendLine(" UPDATE Inquiry ");
                SBSql.AppendLine(" SET Reply_Subject = @Reply_Subject, Status = 2 ");
                SBSql.AppendLine(" WHERE (InquiryID = @DataID); ");

                switch (this.hf_Status.Value)
                {
                    case "1":
                        //[SQL] - 新增回覆
                        SBSql.AppendLine(" DECLARE @New_ID AS INT ");
                        SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(ReplyID), 0) + 1 FROM Inquiry_Reply WHERE (InquiryID = @DataID)) ");
                        SBSql.AppendLine(" INSERT INTO Inquiry_Reply( ");
                        SBSql.AppendLine("  InquiryID, ReplyID, Reply_Message, Create_Who, Create_Time");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine("  @DataID, @New_ID, @Reply_Message, @Create_Who, GETDATE()");
                        SBSql.AppendLine(" ); ");

                        cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);

                        break;

                    default:
                        //[SQL] - 更新回覆
                        SBSql.AppendLine(" UPDATE Inquiry_Reply ");
                        SBSql.AppendLine(" SET Reply_Message = @Reply_Message, Update_Who = @Update_Who, Update_Time = GETDATE() ");
                        SBSql.AppendLine(" WHERE (InquiryID = @DataID) AND (ReplyID = @ReplyID); ");

                        cmd.Parameters.AddWithValue("ReplyID", this.hf_ReplyID.Value);
                        cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserGuid);

                        break;

                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Subject", this.tb_Subject.Text);
                cmd.Parameters.AddWithValue("Reply_Message", this.tb_Reply_Message.Text);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKWeb, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", "");
                    return;
                }

                //導向本頁
                Response.Redirect(PageUrl);
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }


    /// <summary>
    /// 按鈕 - 回覆 (目前狀態 = 待處理(1)、處理中(2))
    /// </summary>
    protected void btn_Reply_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Subject.Text, "1", "100", out ErrMsg) == false)
            {
                SBAlert.Append("「信件主旨」請輸入1 ~ 50個字\\n");
            }
            if (fn_Extensions.String_資料長度Byte(this.tb_Reply_Message.Text, "1", "3000", out ErrMsg) == false)
            {
                SBAlert.Append("「回覆內文」請輸入1 ~ 3000個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region >> 儲存名單 <<
            //設定轉寄名單 - 內部員工
            if (Set_DataRel(Param_thisID, this.val_EmpEmail.Text, this.val_EmpName.Text, "1") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 內部員工！", PageUrl);
                return;
            }

            //設定轉寄名單 - 自訂名單 
            if (Set_DataRel(Param_thisID, this.val_OtherEmail.Text, this.val_OtherName.Text, "2") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 自訂名單！", PageUrl);
                return;
            }

            //設定轉寄名單 - 承辦人員
            if (Set_DataRel(Param_thisID, this.val_SalesEmail.Text, this.val_SalesName.Text, "3") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 承辦人員！", "");
                return;
            }
            #endregion


            #region >> 寄信 - 回覆信轉寄對象 Start <<

            //回覆信郵件內容
            StringBuilder myMailBody = new StringBuilder();
            myMailBody.AppendLine("<div style=\"border-bottom:solid 1px #efefef;padding:10px 0;\">#Top_Logo#</div>");
            myMailBody.AppendLine("<div style=\"font-family: Segoe UI,Segoe UI Web Regular,Segoe UI Symbol,Helvetica Neue,BBAlpha Sans,S60 Sans,Microsoft JhengHei,Microsoft YaHei,sans-serif\">");

            //回覆訊息
            myMailBody.Append("<div style=\"border-bottom:solid 1px #efefef;padding:20px 0px 20px 0px;\"><br/>{0}</div>".FormatThis(
                this.tb_Reply_Message.Text.Replace("\n", "<br/>")));

            //留言訊息
            myMailBody.Append("<div style=\"margin:20px 30px;padding-left:5px;border-left: solid 3px #CCCCCC;\">{0}</div>".FormatThis(
                this.lt_Message.Text.Replace("\n", "<br/>")));

            myMailBody.AppendLine("</div>");

            //發信
            sendMail_transfer(
                this.tb_Subject.Text
                , myMailBody
                , this.lt_ReplyMailSender.Text
                , this.lt_MemberMail.Text
                , this.val_EmpEmail.Text
                , this.val_OtherEmail.Text);

            #endregion >> 寄信 - 回覆信轉寄對象 End <<


            #region >> 寄信 - 承辦人員 Start <<

            //郵件內容
            myMailBody.Clear();

            //客戶資料
            myMailBody.AppendLine("<div style=\"border-bottom:solid 1px #efefef;padding:10px 0;\">#Top_Logo#</div>");
            myMailBody.AppendLine("<div style=\"font-family: Segoe UI,Segoe UI Web Regular,Segoe UI Symbol,Helvetica Neue,BBAlpha Sans,S60 Sans,Microsoft JhengHei,Microsoft YaHei,sans-serif\">");            
            myMailBody.AppendLine("<div><h3>【留言訊息】</h3><span>{0}</span></div>".FormatThis(
                this.lt_Message.Text.Replace("\n", "<br/>")));

            myMailBody.AppendLine("<div><h3>【客戶資料】</h3></div>");
            myMailBody.Append("<table width=\"80%\" border=\"1\" cellpadding=\"8\" style=\"border-collapse: collapse\">");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">Email</td>");
            myMailBody.Append("    <td colspan=\"3\">{0}</td>".FormatThis(this.modal_Email.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">公司</td>");
            myMailBody.Append("    <td colspan=\"3\">{0}</td>".FormatThis(this.modal_Company.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td width=\"15%\" align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">生日</td>");
            myMailBody.Append("    <td width=\"35%\">{0}</td>".FormatThis(this.modal_Birthday.Text));
            myMailBody.Append("    <td width=\"15%\" align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">性別</td>");
            myMailBody.Append("    <td width=\"35%\">{0}</td>".FormatThis(this.modal_Sex.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">姓</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_FirstName.Text));
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">名</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_LastName.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">電話</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Tel.Text));
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">手機</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Mobile.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">國家</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Country.Text));
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">地址</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Address.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("</table>");
            myMailBody.AppendLine("</div>");

            //發信
            sendMail_transfer(
                "{0}-網站留言-客戶詳細資料 #{1}".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"), this.lt_TraceID.Text)
                , myMailBody
                , this.lt_ReplyMailSender.Text
                , System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"]
                , this.val_SalesEmail.Text, ""
                );

            #endregion >> 寄信 - 回覆信轉寄對象 End <<

            #region >> 資料儲存 <<
            //更新基本資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料更新, 3=已回覆
                SBSql.AppendLine(" UPDATE Inquiry ");
                SBSql.AppendLine(" SET Reply_Subject = @Reply_Subject, Status = 3 ");
                SBSql.AppendLine(" WHERE (InquiryID = @DataID); ");

                switch (this.hf_Status.Value)
                {
                    case "1":
                        //[SQL] - 新增回覆
                        SBSql.AppendLine(" DECLARE @New_ID AS INT ");
                        SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(ReplyID), 0) + 1 FROM Inquiry_Reply WHERE (InquiryID = @DataID)) ");
                        SBSql.AppendLine(" INSERT INTO Inquiry_Reply( ");
                        SBSql.AppendLine("  InquiryID, ReplyID, Reply_Message, Create_Who, Create_Time");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine("  @DataID, @New_ID, @Reply_Message, @Create_Who, GETDATE()");
                        SBSql.AppendLine(" ); ");

                        cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);

                        break;

                    default:
                        //[SQL] - 更新回覆
                        SBSql.AppendLine(" UPDATE Inquiry_Reply ");
                        SBSql.AppendLine(" SET Reply_Message = @Reply_Message, Update_Who = @Update_Who, Update_Time = GETDATE() ");
                        SBSql.AppendLine(" WHERE (InquiryID = @DataID) AND (ReplyID = @ReplyID); ");

                        cmd.Parameters.AddWithValue("ReplyID", this.hf_ReplyID.Value);
                        cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserGuid);

                        break;

                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Subject", this.tb_Subject.Text);
                cmd.Parameters.AddWithValue("Reply_Message", this.tb_Reply_Message.Text);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKWeb, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", PageUrl);
                    return;
                }

                //轉到檢視頁
                Response.Redirect("Msg_Edit.aspx?DataID={0}".FormatThis(Server.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey))));
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
    /// 按鈕 - 結案
    /// </summary>
    protected void btn_Finish_Click(object sender, EventArgs e)
    {
        try
        {
            /* 判斷狀態必須為「已回覆=3」 */
            if (!this.hf_Current_Status.Value.Equals("3"))
            {
                fn_Extensions.JsAlert("請先按下「回覆此信」按鈕，再進行結案!", "");
                return;
            }

            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Subject.Text, "1", "100", out ErrMsg) == false)
            {
                SBAlert.Append("「信件主旨」請輸入1 ~ 50個字\\n");
            }
            if (fn_Extensions.String_資料長度Byte(this.tb_Reply_Message.Text, "1", "3000", out ErrMsg) == false)
            {
                SBAlert.Append("「回覆內文」請輸入1 ~ 3000個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region >> 儲存名單 <<
            //設定轉寄名單 - 內部員工
            if (Set_DataRel(Param_thisID, this.val_EmpEmail.Text, this.val_EmpName.Text, "1") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 內部員工！", PageUrl);
                return;
            }

            //設定轉寄名單 - 自訂名單 
            if (Set_DataRel(Param_thisID, this.val_OtherEmail.Text, this.val_OtherName.Text, "2") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 自訂名單！", PageUrl);
                return;
            }

            //設定轉寄名單 - 承辦人員
            if (Set_DataRel(Param_thisID, this.val_SalesEmail.Text, this.val_SalesName.Text, "3") == false)
            {
                fn_Extensions.JsAlert("轉寄名單設定失敗 - 承辦人員！", "");
                return;
            }
            #endregion


            #region >> 寄信 - 回覆信轉寄對象 Start <<

            //回覆信郵件內容
            StringBuilder myMailBody = new StringBuilder();
            myMailBody.AppendLine("<div style=\"border-bottom:solid 1px #efefef;padding:10px 0;\">#Top_Logo#</div>");
            myMailBody.AppendLine("<div style=\"font-family: Segoe UI,Segoe UI Web Regular,Segoe UI Symbol,Helvetica Neue,BBAlpha Sans,S60 Sans,Microsoft JhengHei,Microsoft YaHei,sans-serif\">");

            //回覆訊息
            myMailBody.Append("<div style=\"border-bottom:solid 1px #efefef;padding:20px 0px 20px 0px;\"><br/>{0}</div>".FormatThis(
                this.tb_Reply_Message.Text.Replace("\n", "<br/>")));

            //留言訊息
            myMailBody.Append("<div style=\"margin:20px 30px;padding-left:5px;border-left: solid 3px #CCCCCC;\">{0}</div>".FormatThis(
                this.lt_Message.Text.Replace("\n", "<br/>")));

            myMailBody.AppendLine("</div>");

            //發信
            sendMail_transfer(
                this.tb_Subject.Text
                , myMailBody
                , this.lt_ReplyMailSender.Text
                , this.lt_ReplyMailSender.Text
                , this.val_EmpEmail.Text
                , this.val_OtherEmail.Text);

            #endregion >> 寄信 - 回覆信轉寄對象 End <<


            #region >> 寄信 - 承辦人員 Start <<

            //郵件內容
            myMailBody.Clear();

            //客戶資料
            myMailBody.AppendLine("<div style=\"border-bottom:solid 1px #efefef;padding:10px 0;\">#Top_Logo#</div>");
            myMailBody.AppendLine("<div style=\"font-family: Segoe UI,Segoe UI Web Regular,Segoe UI Symbol,Helvetica Neue,BBAlpha Sans,S60 Sans,Microsoft JhengHei,Microsoft YaHei,sans-serif\">");
            myMailBody.AppendLine("<div><h3>【留言訊息】</h3><span>{0}</span></div>".FormatThis(
                this.lt_Message.Text.Replace("\n", "<br/>")));

            myMailBody.AppendLine("<div><h3>【客戶資料】</h3></div>");
            myMailBody.Append("<table width=\"80%\" border=\"1\" cellpadding=\"8\" style=\"border-collapse: collapse\">");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">Email</td>");
            myMailBody.Append("    <td colspan=\"3\">{0}</td>".FormatThis(this.modal_Email.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">公司</td>");
            myMailBody.Append("    <td colspan=\"3\">{0}</td>".FormatThis(this.modal_Company.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td width=\"15%\" align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">生日</td>");
            myMailBody.Append("    <td width=\"35%\">{0}</td>".FormatThis(this.modal_Birthday.Text));
            myMailBody.Append("    <td width=\"15%\" align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">性別</td>");
            myMailBody.Append("    <td width=\"35%\">{0}</td>".FormatThis(this.modal_Sex.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">姓</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_FirstName.Text));
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">名</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_LastName.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">電話</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Tel.Text));
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">手機</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Mobile.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("<tr>");
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">國家</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Country.Text));
            myMailBody.Append("    <td align=\"right\" bgcolor=\"#9f9f9f\" style=\"color: #ffffff\">地址</td>");
            myMailBody.Append("    <td>{0}</td>".FormatThis(this.modal_Address.Text));
            myMailBody.Append("</tr>");
            myMailBody.Append("</table>");
            myMailBody.AppendLine("</div>");

            //發信
            sendMail_transfer(
                "{0}-網站留言-客戶詳細資料 #{1}".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"), this.lt_TraceID.Text)
                , myMailBody
                , this.lt_ReplyMailSender.Text
                , System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"]
                , this.val_SalesEmail.Text, ""
                );

            #endregion >> 寄信 - 回覆信轉寄對象 End <<

            #region >> 資料儲存 <<
            //更新基本資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料更新, 5=結案
                SBSql.AppendLine(" UPDATE Inquiry ");
                SBSql.AppendLine(" SET Reply_Subject = @Reply_Subject, Status = 5 ");
                SBSql.AppendLine(" WHERE (InquiryID = @DataID); ");

                switch (this.hf_Status.Value)
                {
                    case "1":
                        //[SQL] - 新增回覆
                        SBSql.AppendLine(" DECLARE @New_ID AS INT ");
                        SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(ReplyID), 0) + 1 FROM Inquiry_Reply WHERE (InquiryID = @DataID)) ");
                        SBSql.AppendLine(" INSERT INTO Inquiry_Reply( ");
                        SBSql.AppendLine("  InquiryID, ReplyID, Reply_Message, Create_Who, Create_Time");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine("  @DataID, @New_ID, @Reply_Message, @Create_Who, GETDATE()");
                        SBSql.AppendLine(" ); ");

                        cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);

                        break;

                    default:
                        //[SQL] - 更新回覆
                        SBSql.AppendLine(" UPDATE Inquiry_Reply ");
                        SBSql.AppendLine(" SET Reply_Message = @Reply_Message, Update_Who = @Update_Who, Update_Time = GETDATE() ");
                        SBSql.AppendLine(" WHERE (InquiryID = @DataID) AND (ReplyID = @ReplyID); ");

                        cmd.Parameters.AddWithValue("ReplyID", this.hf_ReplyID.Value);
                        cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserGuid);

                        break;

                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Subject", this.tb_Subject.Text);
                cmd.Parameters.AddWithValue("Reply_Message", this.tb_Reply_Message.Text);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKWeb, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", PageUrl);
                    return;
                }

                //轉到檢視頁
                Response.Redirect("Msg_View.aspx?DataID={0}".FormatThis(Server.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey))));
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
    /// 發轉寄信
    /// </summary>
    /// <param name="mySubject"></param>
    /// <param name="myMailBody"></param>
    /// <param name="mySender"></param>
    /// <param name="mySendTo"></param>
    /// <param name="mailList1"></param>
    /// <param name="mailList2"></param>
    private void sendMail_transfer(string mySubject, StringBuilder myMailBody, string mySender, string mySendTo, string mailList1, string mailList2)
    {
        //過濾轉寄名單
        List<TempParam_Item> ITempList = new List<TempParam_Item>();
        List<string> ForwardTo = new List<string>();
        //取得陣列資料-內部員工
        string[] strAry_EmpMail = Regex.Split(mailList1, @"\,{1}");
        //存入暫存清單
        for (int row = 0; row < strAry_EmpMail.Length; row++)
        {
            if (fn_Extensions.IsEmail(strAry_EmpMail[row]))
            {
                ITempList.Add(new TempParam_Item(strAry_EmpMail[row], ""));
            }
        }

        //取得陣列資料-自定名單
        if (!string.IsNullOrEmpty(mailList2))
        {
            string[] strAry_OtherMail = Regex.Split(mailList2, @"\,{1}");
            //存入暫存清單
            for (int row = 0; row < strAry_OtherMail.Length; row++)
            {
                if (fn_Extensions.IsEmail(strAry_OtherMail[row]))
                {
                    ITempList.Add(new TempParam_Item(strAry_OtherMail[row], ""));
                }
            }
        }

        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID
                    };
        foreach (var item in query)
        {
            ForwardTo.Add(item.ID);
        }


        //發信
        if (false == SendMail(mySubject, myMailBody, mySender, mySendTo, ForwardTo, out ErrMsg))
        {
            fn_Extensions.JsAlert("回覆信發送失敗！請確認電子郵件是否正確.\\n訊息:{0}".FormatThis(ErrMsg), PageUrl);
            return;
        }
    }


    /// <summary>
    /// 按鈕 - 垃圾信
    /// </summary>
    protected void btn_Hide_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Reply_Message.Text, "1", "3000", out ErrMsg) == false)
            {
                SBAlert.Append("「回覆內文」請輸入1 ~ 3000個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            //資料儲存
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.AppendLine(" DECLARE @New_ID AS INT ");

                //[SQL] - 資料更新, 4=垃圾信
                SBSql.AppendLine(" UPDATE Inquiry ");
                SBSql.AppendLine(" SET Status = 4 ");
                SBSql.AppendLine(" WHERE (InquiryID = @DataID) ");

                //[SQL] - 新增回覆
                SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(ReplyID), 0) + 1 FROM Inquiry_Reply WHERE (InquiryID = @DataID)) ");
                SBSql.AppendLine(" INSERT INTO Inquiry_Reply( ");
                SBSql.AppendLine("  InquiryID, ReplyID, Reply_Message, Create_Who, Create_Time");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @New_ID, @Reply_Message, @Create_Who, GETDATE()");
                SBSql.AppendLine(" ); ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Message", this.tb_Reply_Message.Text);
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
                if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKWeb, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", "");
                    return;
                }

                //返回列表
                Response.Redirect(Session["BackListUrl"].ToString());
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 判斷為垃圾信", "");
            return;
        }

    }


    /// <summary>
    /// 關聯性設定
    /// </summary>
    /// <param name="DataID">資料編號</param>
    /// <param name="Get_EMail"></param>
    /// <param name="Get_Name"></param>
    /// <returns></returns>
    private bool Set_DataRel(string DataID, string Get_EMail, string Get_Name, string type)
    {
        //取得陣列資料
        string[] strAry_ID = Regex.Split(Get_EMail, @"\,{1}");
        string[] strAry_Name = Regex.Split(Get_Name, @"\,{1}");

        //宣告暫存清單
        List<TempParam_Item> ITempList = new List<TempParam_Item>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam_Item(strAry_ID[row], strAry_Name[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID,
                        Name = el.tmp_Name
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID,
                        Name = gp.Key.Name
                    };

        //處理資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            SBSql.AppendLine(" DELETE FROM Inquiry_CC WHERE (InquiryID = @DataID) AND (CC_Type = @CC_Type); ");

            //判斷是否為空
            if (!string.IsNullOrEmpty(Get_EMail))
            {
                int row = 0;
                foreach (var item in query)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO Inquiry_CC( ");
                    SBSql.AppendLine("  InquiryID, CC_Type, CC_EMail, CC_Who");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @DataID, @CC_Type, @CC_EMail_{0}, @CC_Who_{0}".FormatThis(row));
                    SBSql.AppendLine(" ); ");

                    cmd.Parameters.AddWithValue("CC_EMail_" + row, item.ID);
                    cmd.Parameters.AddWithValue("CC_Who_" + row, item.Name);
                }
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", DataID);
            cmd.Parameters.AddWithValue("CC_Type", type);
            if (dbConn.ExecuteSql(cmd, dbConn.DBS.PKWeb, out ErrMsg) == false)
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
    /// 暫存參數
    /// </summary>
    public class TempParam_Item
    {
        private string _tmp_ID;
        public string tmp_ID
        {
            get { return this._tmp_ID; }
            set { this._tmp_ID = value; }
        }

        private string _tmp_Name;
        public string tmp_Name
        {
            get { return this._tmp_Name; }
            set { this._tmp_Name = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        public TempParam_Item(string tmp_ID, string tmp_Name)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Name = tmp_Name;
        }
    }
    #endregion -- 資料編輯 End --

    #region -- 其他功能 --
    /// <summary>
    /// 寄回覆信
    /// </summary>
    /// <param name="DataID">追蹤編號</param>
    /// <param name="Subject">主旨</param>
    /// <param name="ReqEMail">需求者EMail</param>
    private bool SendMail(string Subject, StringBuilder mailBody, string emailFrom, string emailTo, List<string> ForwardTo, out string ErrMsg)
    {
        ErrMsg = "";

        if (emailFrom == null)
        {
            ErrMsg = "寄件者空白，無法寄送回覆信";
            return false;
        }
        if (emailTo == null)
        {
            ErrMsg = "收件者空白，無法寄送回覆信";
            return false;
        }

        //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = "PKWeb, Inquiry回覆";

        //[設定參數] - 寄件人
        fn_Mail.Sender = emailFrom;

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = "Pro'skit";

        //[設定參數] - 收件人
        List<string> myReciever = new List<string>();
        myReciever.Add(emailTo);
        fn_Mail.Reciever = myReciever;

        //[設定參數] - 轉寄人群組
        fn_Mail.CC = ForwardTo;

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = Subject;

        //[設定參數] - 郵件內容
        fn_Mail.MailBody = mailBody;

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();

        //[判斷參數] - 寄件是否成功
        if (!fn_Mail.MessageCode.Equals(200))
        {
            //fn_Mail.Message
            ErrMsg = "發生錯誤，無法寄送回覆信.\n{0}".FormatThis(fn_Mail.Message);
            return false;
        }

        return true;
    }

    #endregion

    #region -- 參數設定 --
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
            return string.IsNullOrEmpty(Request.QueryString["DataID"]) ? "" : Cryptograph.MD5Decrypt(Request.QueryString["DataID"].ToString(), DesKey);
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
            return string.Format(@"Msg_Edit.aspx?DataID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey)));
        }
        set
        {
            this._PageUrl = value;
        }
    }


    #endregion

}