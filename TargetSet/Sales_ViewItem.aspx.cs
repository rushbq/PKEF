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

public partial class Sales_ViewItem : SecurityIn
{
    //總計
    int totalAmount = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 業務目標
                if (fn_CheckAuth.CheckAuth_User("110", out ErrMsg) == false)
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
                SBSql.AppendLine(" SELECT SetMonth ");
                SBSql.AppendLine(string.Format(",{0} AS Amount", Param_Column));
                SBSql.AppendLine(" FROM Target_Sales ");
                SBSql.AppendLine(" WHERE (ShipFrom = @ShipFrom) AND (DeptID = @DeptID) AND (SetYear = @SetYear) AND (StaffID = @StaffID) AND (TargetType = @TargetType)");
                SBSql.AppendLine(" ORDER BY SetMonth ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ShipFrom", Param_ShipFrom);
                cmd.Parameters.AddWithValue("DeptID", Param_DeptID);
                cmd.Parameters.AddWithValue("SetYear", Param_SetYear);
                cmd.Parameters.AddWithValue("StaffID", Param_StaffID);
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

                totalAmount += Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "Amount"));
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
                Label lb_Total = (Label)lvDataList.FindControl("lb_Total");
                lb_Total.Text = fn_stringFormat.C_format(totalAmount.ToString());
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
            /* 判斷權限 
            *   管理者權限(111):帶出所有部門
            *   一般權限(112):帶出所屬部門
            */
            string ErrMsg;
            string deptID = "";

            //無:管理者權限，有:一般權限
            if (fn_CheckAuth.CheckAuth_User("111", out ErrMsg) == false
                && fn_CheckAuth.CheckAuth_User("112", out ErrMsg))
            {
                deptID = ADService.getDepartmentFromGUID(fn_Params.UserGuid);
            }

            //若deptID為空，則判斷是否有request
            return (string.IsNullOrEmpty(deptID)) ?
                string.IsNullOrEmpty(Request.QueryString["DeptID"]) ? "" : Request.QueryString["DeptID"].ToString()
                : deptID;
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

    /// <summary>
    /// 人員
    /// </summary>
    private string _Param_StaffID;
    public string Param_StaffID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["StaffID"]) ? "" : Request.QueryString["StaffID"].ToString();
        }
        set
        {
            this._Param_StaffID = value;
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
