using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using ExtensionMethods;

public partial class ToyMsg_View : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 網站訊息
                if (fn_CheckAuth.CheckAuth_User("615", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "myMarket/ToyMsg_Search.aspx";

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

                SBSql.AppendLine("SELECT ");
                SBSql.AppendLine("  Cls.Class_Name, Base.InquiryID, Base.Message, Base.Status, St.Class_Name AS StName, Base.TraceID ");
                SBSql.AppendLine("  , Base.Reply_Subject, Base.Create_Time ");
                SBSql.AppendLine("  , (SELECT Email FROM PKSYS.dbo.User_Profile WHERE (Guid = @LoginGuid)) AS PubSender_Email ");
                SBSql.AppendLine("  , Base.Email AS MemberMail, Base.LastName, Base.FirstName, GC.Country_Name, Base.Tel ");
                SBSql.AppendLine("  , (SELECT Email FROM PKSYS.dbo.User_Profile WHERE (Guid IN ( ");
                SBSql.AppendLine("      SELECT TOP 1 Reply.Create_Who FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC ");
                SBSql.AppendLine("   ) ");
                SBSql.AppendLine("  )) AS Reply_Email ");
                SBSql.AppendLine("  , (SELECT TOP 1 Reply.Create_Time FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS Reply_Time ");
                SBSql.AppendLine("  , (SELECT TOP 1 Reply.Reply_Message FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS Reply_Message ");
                SBSql.AppendLine("  , (SELECT TOP 1 Reply.ReplyID FROM Inquiry_Reply Reply WHERE (Reply.InquiryID = Base.InquiryID) ORDER BY Create_Time DESC) AS ReplyID ");
                SBSql.AppendLine("FROM Inquiry Base ");
                SBSql.AppendLine("  INNER JOIN Inquiry_Class Cls ON Base.Class_ID = Cls.Class_ID AND Cls.LangCode = 'zh-TW' ");
                SBSql.AppendLine("  INNER JOIN Inquiry_Status St ON Base.Status = St.Class_ID ");
                SBSql.AppendLine("  LEFT JOIN PKWeb.dbo.Geocode_CountryName GC ON Base.CountryCode = GC.Country_Code AND GC.LangCode = 'zh-tw' ");
                SBSql.AppendLine(" WHERE (Base.InquiryID = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("LoginGuid", fn_Params.UserGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Science, out ErrMsg))
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
                        this.lt_Message.Text = DT.Rows[0]["Message"].ToString().Replace("\n", "<br/>");
                        this.lt_ReplyMailSender.Text = DT.Rows[0]["Reply_Email"].ToString();
                        this.lt_MemberMail.Text = DT.Rows[0]["MemberMail"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Reply_Time.Text = DT.Rows[0]["Reply_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Subject.Text = DT.Rows[0]["Reply_Subject"].ToString();
                        this.lt_Reply_Message.Text = DT.Rows[0]["Reply_Message"].ToString().Replace("\n", "<br/>");

                        //會員資料
                        this.modal_Email.Text = DT.Rows[0]["MemberMail"].ToString();
                        this.modal_LastName.Text = DT.Rows[0]["LastName"].ToString();
                        this.modal_FirstName.Text = DT.Rows[0]["FirstName"].ToString();
                        this.modal_Country.Text = DT.Rows[0]["Country_Name"].ToString();
                        this.modal_Tel.Text = DT.Rows[0]["Tel"].ToString();

                        //取得狀態
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
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Science, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        StringBuilder itemHtml = new StringBuilder();

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //取得參數
                            string myLabel = DT.Rows[row]["myLabel"].ToString();
                            string myValue = DT.Rows[row]["myValue"].ToString();

                            //組合Html
                            itemHtml.AppendLine("<li style=\"padding-top:5px;\">");
                            itemHtml.Append("<a class=\"btn btn-success\" title=\"{1}\">{0}</a>".FormatThis(myLabel, myValue));
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
    #endregion

}