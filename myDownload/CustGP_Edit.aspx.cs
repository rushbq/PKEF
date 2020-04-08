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
                //[參數判斷] - 判斷是否有資料編號
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData();
                }

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT GP.Group_ID, GP.Group_Name, GP.Display, GP.Sort ");
                SBSql.AppendLine("  , Base.Cust_ERPID AS CustID, Cust.MA002 AS CustName");
                SBSql.AppendLine("  FROM File_CustGroup GP ");
                SBSql.AppendLine("   INNER JOIN File_CustList Base ON GP.Group_ID = Base.Group_ID ");
                SBSql.AppendLine("   INNER JOIN PKSYS.dbo.Customer Cust ON Base.Cust_ERPID = Cust.MA001 AND Cust.DBS = Cust.DBC");
                SBSql.AppendLine(" WHERE (GP.Group_ID = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "");
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Group_Name.Text = DT.Rows[0]["Group_Name"].ToString();
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.lt_Save.Text = "修改資料";

                        //填入客戶 Html
                        StringBuilder itemHtml = new StringBuilder();

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //取得參數
                            string myID = DT.Rows[row]["CustID"].ToString();
                            string myName = DT.Rows[row]["CustName"].ToString();

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

                        this.lt_ViewList.Text = itemHtml.ToString();

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }

    #endregion -- 資料顯示 End --

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
            if (fn_Extensions.String_資料長度Byte(this.tb_Group_Name.Text, "1", "150", out ErrMsg) == false)
            {
                SBAlert.Append("「群組名稱」請輸入1 ~ 70個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            
            #region "..資料儲存.."
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data();
                    break;

                case "EDIT":
                    Edit_Data();
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
    private void Add_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            int NewID;

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Group_ID) ,0) + 1 FROM File_CustGroup ");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" SELECT @NewID AS NewID");

            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                NewID = Convert.ToInt32(DT.Rows[0]["NewID"]);
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO File_CustGroup( ");
            SBSql.AppendLine("  Group_ID, Group_Name, Display, Sort");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @NewID, @Group_Name, @Display, @Sort");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" )");
            
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("Group_Name", this.tb_Group_Name.Text);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
                return;
            }

            //設定關聯
            if (false == Set_CustRel(NewID.ToString()))
            {
                fn_Extensions.JsAlert("經銷名單設定失敗", "");
                return;
            }

            //導向列表頁
            Response.Redirect("CustGP_Search.aspx");

        }

    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE File_CustGroup ");
            SBSql.AppendLine(" SET Group_Name = @Group_Name, Display = @Display, Sort = @Sort");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Group_ID = @DataID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("Group_Name", this.tb_Group_Name.Text);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Update_Who", fn_Params.UserGuid);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }

            //設定關聯
            if (false == Set_CustRel(Param_thisID))
            {
                fn_Extensions.JsAlert("經銷商名單設定失敗", "");
                return;
            }

            //導向列表頁
            Response.Redirect("CustGP_Search.aspx");
        }
    }

    /// <summary>
    /// 關聯設定 
    /// </summary>
    /// <param name="DataID">單頭資料編號</param>
    /// <returns></returns>
    private bool Set_CustRel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.myValues.Text;

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

            SBSql.AppendLine(" DELETE FROM File_CustList WHERE (Group_ID = @DataID); ");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                SBSql.AppendLine(" INSERT INTO File_CustList( ");
                SBSql.AppendLine("  Group_ID, Cust_ERPID");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @Cust_ERPID_{0}".FormatThis(row));
                SBSql.AppendLine(" ); ");

                cmd.Parameters.AddWithValue("Cust_ERPID_" + row, item.ID);
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