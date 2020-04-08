using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;

public partial class Recording_Req_Process : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //判斷參數是否為空
                Check_Params();

                //取得資料
                LookupData();
            }


        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料讀取 --

    /// <summary>
    /// 判斷參數是否為空
    /// </summary>
    private void Check_Params()
    {
        if (string.IsNullOrEmpty(Req_DataID))
        {
            this.ph_Message.Visible = true;
            this.ph_Content.Visible = false;
            this.ph_Buttons.Visible = false;
        }
        else
        {
            this.ph_Message.Visible = false;
            this.ph_Content.Visible = true;
            this.ph_Buttons.Visible = true;
        }
    }


    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT Base.TraceID, Base.Help_Subject, Base.Help_Content");
            SBSql.AppendLine("  , Prof.Account_Name, Prof.Display_Name");
            SBSql.AppendLine(" FROM IT_Help Base");
            SBSql.AppendLine("  INNER JOIN PKSYS.dbo.User_Profile Prof ON Base.Req_Who = Prof.Account_Name");
            SBSql.AppendLine(" WHERE (Base.TraceID = @TraceID) AND (Base.Agree_Who IS NULL)");

            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("TraceID", Req_DataID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    this.ph_Message.Visible = true;
                    this.ph_Content.Visible = false;
                    this.ph_Buttons.Visible = false;
                    return;
                }
                else
                {
                    //[填入資料]
                    this.lt_TraceID.Text = DT.Rows[0]["TraceID"].ToString().Insert(2, "-").Insert(11, "-");
                    this.lt_ReqWho.Text = "({0}) {1}".FormatThis(DT.Rows[0]["Account_Name"].ToString(), DT.Rows[0]["Display_Name"].ToString());
                    this.lt_Help_Subject.Text = DT.Rows[0]["Help_Subject"].ToString();
                    this.lt_Help_Content.Text = DT.Rows[0]["Help_Content"].ToString().Replace("\r", "<br/>");
                  
                }
            }
        }
    }


    #endregion

    #region -- 按鈕事件 --

    /// <summary>
    /// No
    /// </summary>
    protected void lbtn_No_Click(object sender, EventArgs e)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" UPDATE IT_Help SET IsAgree = 'N', Agree_Who = @Agree_Who, Agree_Time = GETDATE()");
            SBSql.AppendLine(" WHERE (TraceID = @TraceID)");

            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("TraceID", Req_DataID);
            cmd.Parameters.AddWithValue("Agree_Who", fn_Params.UserAccount);
            if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
            {
                this.ph_Error.Visible = true;
                return;
            }
            else
            {
                this.ph_Message.Visible = true;
                this.ph_Content.Visible = false;
                this.ph_Buttons.Visible = false;
                this.ph_Error.Visible = false;
                return;
            }
        }
    }

    /// <summary>
    /// OK
    /// </summary>
    protected void lbtn_Yes_Click(object sender, EventArgs e)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" UPDATE IT_Help SET IsAgree = 'Y', Agree_Who = @Agree_Who, Agree_Time = GETDATE()");
            SBSql.AppendLine(" WHERE (TraceID = @TraceID)");

            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("TraceID", Req_DataID);
            cmd.Parameters.AddWithValue("Agree_Who", fn_Params.UserAccount);
            if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
            {
                this.ph_Error.Visible = true;
                return;
            }
            else
            {
                this.ph_Message.Visible = true;
                this.ph_Content.Visible = false;
                this.ph_Buttons.Visible = false;
                this.ph_Error.Visible = false;
                return;
            }
        }
    }


    #endregion

    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            string data = Request.QueryString["dataID"];

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;

    #endregion
}