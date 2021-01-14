using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections;

public partial class Dept_ViewAll : SecurityIn
{
    //總計
    int totalAmount_NTD = 0;
    int totalAmount_USD = 0;
    int totalAmount_RMB = 0;
    int totalAmountOrd_NTD = 0;
    int totalAmountOrd_USD = 0;
    int totalAmountOrd_RMB = 0;
    int totalAmountCh_NTD = 0;
    int totalAmountCh_USD = 0;
    int totalAmountCh_RMB = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 部門目標
                if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //顯示資料
                View_DataList();
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 顯示資料 - 本年
    /// </summary>
    private void View_DataList()
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT UID, ShipFrom, DeptID, SetYear, SetMonth");
                SBSql.AppendLine("  , Amount_NTD, Amount_USD, Amount_RMB");
                SBSql.AppendLine("  , OrdAmount_NTD, OrdAmount_USD, OrdAmount_RMB");
                SBSql.AppendLine("  , ChAmount_NTD, ChAmount_USD, ChAmount_RMB");
                SBSql.AppendLine(" FROM Target_Dept ");
                SBSql.AppendLine(" WHERE (ShipFrom = @ShipFrom) AND (DeptID = @DeptID) AND (SetYear = @SetYear) AND (TargetType = @TargetType)");
                SBSql.AppendLine(" ORDER BY SetMonth ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ShipFrom", Param_ShipFrom);
                cmd.Parameters.AddWithValue("DeptID", Param_DeptID);
                cmd.Parameters.AddWithValue("SetYear", Param_SetYear);
                cmd.Parameters.AddWithValue("TargetType", Param_Type);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.EFLocal, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 顯示資料！", "");
            return;
        }
    }

    //金額加總
    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                totalAmount_NTD += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "Amount_NTD"));
                totalAmount_USD += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "Amount_USD"));
                totalAmount_RMB += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "Amount_RMB"));
                totalAmountOrd_NTD += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "OrdAmount_NTD"));
                totalAmountOrd_USD += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "OrdAmount_USD"));
                totalAmountOrd_RMB += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "OrdAmount_RMB"));
                totalAmountCh_NTD += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ChAmount_NTD"));
                totalAmountCh_USD += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ChAmount_USD"));
                totalAmountCh_RMB += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ChAmount_RMB"));
            }
        }
        catch (Exception)
        {
            throw new Exception("ItemDataBound");
        }
    }

    //顯示總計金額
    protected void lvDataList_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (lvDataList.Items.Count > 0)
            {
                //銷售
                Label lb_Total_NTD = (Label)lvDataList.FindControl("lb_Total_NTD");
                lb_Total_NTD.Text = fn_stringFormat.C_format(totalAmount_NTD.ToString());
                Label lb_Total_USD = (Label)lvDataList.FindControl("lb_Total_USD");
                lb_Total_USD.Text = fn_stringFormat.C_format(totalAmount_USD.ToString());
                Label lb_Total_RMB = (Label)lvDataList.FindControl("lb_Total_RMB");
                lb_Total_RMB.Text = fn_stringFormat.C_format(totalAmount_RMB.ToString());

                //接單
                Label lb_TotalOrd_NTD = (Label)lvDataList.FindControl("lb_TotalOrd_NTD");
                lb_TotalOrd_NTD.Text = fn_stringFormat.C_format(totalAmountOrd_NTD.ToString());
                Label lb_TotalOrd_USD = (Label)lvDataList.FindControl("lb_TotalOrd_USD");
                lb_TotalOrd_USD.Text = fn_stringFormat.C_format(totalAmountOrd_USD.ToString());
                Label lb_TotalOrd_RMB = (Label)lvDataList.FindControl("lb_TotalOrd_RMB");
                lb_TotalOrd_RMB.Text = fn_stringFormat.C_format(totalAmountOrd_RMB.ToString());

                //挑戰
                Label lb_TotalCh_NTD = (Label)lvDataList.FindControl("lb_TotalCh_NTD");
                lb_TotalCh_NTD.Text = fn_stringFormat.C_format(totalAmountCh_NTD.ToString());
                Label lb_TotalCh_USD = (Label)lvDataList.FindControl("lb_TotalCh_USD");
                lb_TotalCh_USD.Text = fn_stringFormat.C_format(totalAmountCh_USD.ToString());
                Label lb_TotalCh_RMB = (Label)lvDataList.FindControl("lb_TotalCh_RMB");
                lb_TotalCh_RMB.Text = fn_stringFormat.C_format(totalAmountCh_RMB.ToString());
            }
        }
        catch (Exception)
        {
            throw new Exception("DataBound");
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 出貨地
    /// </summary>
    private string _Param_ShipFrom;
    public string Param_ShipFrom
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["ShipFrom"]) ? "" : Request.QueryString["ShipFrom"].ToString();
        }
        set
        {
            this._Param_ShipFrom = value;
        }
    }

    /// <summary>
    /// 部門
    /// </summary>
    private string _Param_DeptID;
    public string Param_DeptID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["DeptID"]) ? "" : Request.QueryString["DeptID"].ToString();
        }
        set
        {
            this._Param_DeptID = value;
        }
    }

    /// <summary>
    /// 年份
    /// </summary>
    private string _Param_SetYear;
    public string Param_SetYear
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["SetYear"]) ? "" : Request.QueryString["SetYear"].ToString();
        }
        set
        {
            this._Param_SetYear = value;
        }
    }

    private string _Param_Column;
    public string Param_Column
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["Column"]) ? "0" : Request.QueryString["Column"].ToString();
        }
        set
        {
            this._Param_Column = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Tab ID
    /// </summary>
    private string _Param_Type;
    public string Param_Type
    {
        get
        {
            string _id = Request.QueryString["t"];
            string _checkID = _id;

            //若為空值,帶預設值
            if (string.IsNullOrWhiteSpace(_id) || _id.Equals("0"))
            {
                _checkID = "1";
            }

            return _checkID;

        }
        set
        {
            this._Param_Type = value;
        }
    }
    #endregion
}
