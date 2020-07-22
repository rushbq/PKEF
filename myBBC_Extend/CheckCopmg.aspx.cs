using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using LinqToExcel;
using PKLib_Method.Methods;
using ERP_CheckModel.Controllers;

public partial class myBBC_Extend_CheckCopmg : SecurityIn
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (!fn_CheckAuth.CheckAuth_User("8101", out ErrMsg))
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 取得資料 --

    protected void btn_Check1_Click(object sender, EventArgs e)
    {
        string _custID = filter_Cust1.Text;
        //SH
        GetDataList("SH", _custID);
    }
    protected void btn_Check2_Click(object sender, EventArgs e)
    {
        string _custID = filter_Cust2.Text;
        //TW
        GetDataList("TW", _custID);
    }


    /// <summary>
    /// 取得資料
    /// </summary>
    private void GetDataList(string dbs, string custID)
    {
        //----- 宣告:資料參數 -----
        ERP_CheckProdDataRepository _data = new ERP_CheckProdDataRepository();

        try
        {
            lt_CustID.Text = custID;

            //----- 原始資料:取得所有資料 -----
            var data = _data.GetList(dbs, custID, out ErrMsg);

            //----- 資料整理:繫結 ----- 
            this.lvDataList.DataSource = data;
            this.lvDataList.DataBind();

        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            //release
            _data = null;
        }

    }


    #endregion


}