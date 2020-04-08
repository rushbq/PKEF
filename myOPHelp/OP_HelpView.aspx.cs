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

public partial class OP_HelpView : SecurityIn
{
    //回覆權限
    public bool ReplyAuth = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 品號修改需求登記
                if (fn_CheckAuth.CheckAuth_User("520", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //取得回覆權限
                ReplyAuth = fn_CheckAuth.CheckAuth_User("521", out ErrMsg);

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Recording/OP_HelpSearch.aspx";


                //[參數判斷] - 是否有編號
                if (string.IsNullOrEmpty(Param_thisID))
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                    return;
                }
                else
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

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                SBSql.AppendLine(" SELECT Main.* ");
                SBSql.AppendLine("  , ReqClass.Class_Name AS HClass, HelpStatus.Class_Name AS HStatus ");
                SBSql.AppendLine("  , Prof.Account_Name, Prof.Display_Name, ReqClass.Notify ");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Create_Who)) AS Create_Name ");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Update_Who)) AS Update_Name ");
                SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Agree_Who)) AS Agree_Name");
                SBSql.AppendLine(" FROM OP_Help Main ");
                SBSql.AppendLine("  INNER JOIN OP_ParamClass ReqClass ON Main.Req_Class = ReqClass.Class_ID ");
                SBSql.AppendLine("  INNER JOIN OP_ParamClass HelpStatus ON Main.Help_Status = HelpStatus.Class_ID ");
                SBSql.AppendLine("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Main.Req_Who = Prof.Account_Name ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID); ");

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
                        //判斷狀態, 若是自己的單子 & 狀態不為已結案，則顯示按鈕(自行解決的按鈕)
                        string Help_Status = DT.Rows[0]["Help_Status"].ToString();
                        if (!Help_Status.Equals("11") && DT.Rows[0]["Req_Who"].ToString().Equals(fn_Params.UserAccount))
                        {
                            this.btn_Done.Visible = true;
                        }

                        //[填入資料]
                        this.lt_TraceID.Text = DT.Rows[0]["TraceID"].ToString().Insert(2, "-").Insert(11, "-");
                        this.lt_ReqWho.Text = "({0}) {1}".FormatThis(DT.Rows[0]["Account_Name"].ToString(), DT.Rows[0]["Display_Name"].ToString());
                        this.lt_Req_Class.Text = DT.Rows[0]["HClass"].ToString();
                        this.lt_Help_Subject.Text = DT.Rows[0]["Help_Subject"].ToString();
                        this.lt_Help_Content.Text = DT.Rows[0]["Help_Content"].ToString().Replace("\r", "<br/>");
                        this.lt_Reply_Hours.Text = DT.Rows[0]["Reply_Hours"].ToString();
                        this.lt_Reply_Content.Text = DT.Rows[0]["Reply_Content"].ToString().Replace("\r", "<br/>");
                        this.lt_Reply_Date.Text = DT.Rows[0]["Reply_Date"].ToString().ToDateString("yyyy/MM/dd");
                        this.lb_IsAgree.Text = DT.Rows[0]["IsAgree"].ToString();

                        //取得處理狀態
                        string helpStatus = DT.Rows[0]["Help_Status"].ToString();
                        string setCss;
                        //判斷處理狀態, 給予不同的顏色
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
                                this.ph_Showrate.Visible = true;
                                break;
                        }
                        this.lb_Help_Status.Text = DT.Rows[0]["HStatus"].ToString();
                        this.lb_Help_Status.CssClass = setCss;

                        //主管核示(權限申請)
                        string IsNotify = DT.Rows[0]["Notify"].ToString();
                        this.ph_Agree.Visible = IsNotify.Equals("Y");

                        //填入建立 & 修改資料
                        this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                        this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Agree_Who.Text = DT.Rows[0]["Agree_Name"].ToString();
                        this.lt_Agree_Time.Text = DT.Rows[0]["Agree_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");


                        //評分判斷
                        if (DT.Rows[0]["IsRate"].ToString().Equals("Y"))
                        {
                            this.pl_Rate.Visible = false;
                            this.pl_ShowRate.Visible = true;
                            Param_Score = Convert.ToInt16(DT.Rows[0]["RateScore"]);
                        }
                        else
                        {
                            this.pl_Rate.Visible = true;
                            this.pl_ShowRate.Visible = false;
                            Param_Score = 0;
                        }

                        //回覆鈕
                        if (ReplyAuth)
                        {
                            this.ph_Reply.Visible = true;
                        }

                        //填入圖片資料
                        LookupDataList();
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

    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 結案
    /// </summary>
    protected void btn_Done_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" UPDATE OP_Help SET Help_Status = 11 ");
                SBSql.AppendLine("  , Reply_Hours = @Reply_Hours, Reply_Content = @Reply_Content, Reply_Date = @Reply_Date ");
                SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TraceID", Param_thisID);
                cmd.Parameters.AddWithValue("Reply_Hours", 0);
                cmd.Parameters.AddWithValue("Reply_Content", "需求者已自行解決。");
                cmd.Parameters.AddWithValue("Reply_Date", DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserAccount);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("結案失敗！", PageUrl);
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
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 結案", "");
            return;
        }

    }

    #endregion -- 資料編輯 End --

    #region -- 其他功能 --
    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <returns>string</returns>
    public string PicUrl(string PicName)
    {
        string preView = "";

        if (PicName.ToLower().IndexOf(".jpg") > 0 || PicName.ToLower().IndexOf(".png") > 0)
        {
            //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
            preView = string.Format(
                "<a class=\"PicGroup L2Img\" rel=\"PicGroup\" href=\"{0}\"><img src=\"{0}\" border=\"0\" style=\"width:120px\" alt=\"Pic\"></a>"
                , Param_WebFolder + PicName);
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

    #endregion

    #region -- 參數設定 --
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
            return string.Format(@"OP_HelpView.aspx?TraceID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, DesKey)));
        }
        set
        {
            this._PageUrl = value;
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
    /// 產生MD5驗証碼
    /// SessionID + 登入帳號 + 自訂字串
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get { return Cryptograph.MD5(Session.SessionID + Session["Login_UserID"] + System.Web.Configuration.WebConfigurationManager.AppSettings["ValidCode_Pwd"]); }
        private set { this._ValidCode = value; }
    }

    /// <summary>
    /// 分數
    /// </summary>
    private Int16 _Param_Score;
    public Int16 Param_Score
    {
        get;
        set;
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
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