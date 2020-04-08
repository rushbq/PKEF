using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;

public partial class Rate_Ajax : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[驗證] - MD5是否相同
                if (Request.Form["ValidCode"] == null)
                {
                    Response.Write("驗證失敗!");
                    return;
                }
                if (!Request.Form["ValidCode"].Equals(ValidCode))
                {
                    Response.Write("驗證失敗!");
                    return;
                }

                //[取得callback資料]
                if (string.IsNullOrEmpty(Request["score"]) || string.IsNullOrEmpty(Request["ratetype"]) || string.IsNullOrEmpty(Request["dataID"]))
                {
                    Response.Write("error");
                    return;
                }
                //[取得參數值]
                string dataID = Request["dataID"].ToString();     //資料編號
                Int16 score = Convert.ToInt16(Request["score"]);   //評分
                string ratetype = Request["ratetype"].ToString().ToUpper();     //類別
                string ErrMsg;

                //[SQL] - 資料新增/更新
                using (SqlCommand cmd = new SqlCommand())
                {
                    //[清除參數]
                    cmd.Parameters.Clear();

                    StringBuilder SBSql = new StringBuilder();

                    switch (ratetype)
                    {
                        case "ITHELP":
                            SBSql.AppendLine(" UPDATE IT_Help SET RateScore = @RateScore, IsRate = 'Y' ");
                            SBSql.AppendLine(" WHERE (TraceID = @DataID) ");
                            break;

                        case "OPHELP":
                            SBSql.AppendLine(" UPDATE OP_Help SET RateScore = @RateScore, IsRate = 'Y' ");
                            SBSql.AppendLine(" WHERE (TraceID = @DataID) ");
                            break;

                        default:
                            break;
                    }
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("RateScore", score);
                    cmd.Parameters.AddWithValue("DataID", dataID);
                    if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                    {
                        Response.Write("error:" + ErrMsg);
                        return;
                    }
                    else
                    {
                        Response.Write("OK");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("error:" + ex.Message.ToString());
                return;
            }
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
}