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
using SZ_BBC.Controllers;
using SZ_BBC.Models;

/*
 * [Step4]
 * - EDI匯入
 * - 整理EDI所需欄位
 * - 若為退貨單, 則顯示退貨單頭備註欄位, 並Update BBC_ImportData.Remark
 * - 計算預交日
 * 
 */

public partial class mySZBBC_ImportStep4 : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("810", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

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
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                MallID = fld.MallID,
                MallName = fld.MallName,
                DataType = fld.Data_Type,
                DataTypeName = fld.Data_TypeName,
                Status = fld.Status

            }).FirstOrDefault();

        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string CustID = query.CustID;
        string CustName = query.CustName;

        this.lt_TraceID.Text = TraceID;
        this.lt_CustName.Text = "{0} ({1})".FormatThis(CustName, CustID);
        this.lt_MallName.Text = query.MallName;
        this.lt_TypeName.Text = query.DataTypeName;
        this.hf_TraceID.Value = TraceID;
        this.hf_MallID.Value = query.MallID.ToString();
        this.hf_Type.Value = query.DataType.ToString();

        
        //已完成匯入, 不可停留
        if (query.Status.Equals(13))
        {
            Response.Redirect("{0}mySZBBC/ImportList.aspx".FormatThis(Application["WebUrl"]));
        }

        query = null;

    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 下一步
    /// </summary>
    protected void lbtn_Next_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        SZBBCRepository _data = new SZBBCRepository();

        //建立基本資料參數
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = this.hf_TraceID.Value,
            Data_Type = Convert.ToDecimal(this.hf_Type.Value),
            MallID = Convert.ToInt16(this.hf_MallID.Value)
        };

        //建立EDI
        if (!_data.Create_EDI(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "EDI匯入失敗(Step4)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            //Show Error
            //Response.Write(ErrMsg);
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            this.ph_Message.Visible = false;
        }


        //匯入完成, 更新狀態
        if (!_data.Update_Status(Req_DataID, out ErrMsg))
        {
            //導至完成頁
            Response.Redirect("{0}mySZBBC/ImportStep5.aspx?dataID={1}&st=500".FormatThis(
                Application["WebUrl"]
                , Req_DataID));
        }
        else
        {
            //清空暫存
            _data.Delete_Temp(Req_DataID);

            //導至完成頁
            Response.Redirect("{0}mySZBBC/ImportStep5.aspx?dataID={1}&st=200".FormatThis(
                Application["WebUrl"]
                , Req_DataID));
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