using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;
using LogRecord;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using ExtensionUI;
using Microsoft.Reporting.WebForms;

public partial class myPrice_PriceCompare_View : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷] - 客戶報價
                if (fn_CheckAuth.CheckAuth_User("630", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    Response.Redirect(Page_SearchUrl);
                }
                else
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

                //[SQL] - 資料查詢
                SBSql.Append(" SELECT Base.Subject AS Label");
                SBSql.Append("  , (SELECT COUNT(*) FROM Price_Rel_CustID WHERE (Sheet_ID = Base.Sheet_ID)) AS CustCnt");
                SBSql.Append("  , (SELECT COUNT(*) FROM Price_Rel_ModelNo WHERE (Sheet_ID = Base.Sheet_ID)) AS ItemCnt");
                SBSql.Append(" FROM Price_CompareSheet Base");
                SBSql.Append(" WHERE (Base.Sheet_ID = @DataID) AND (Base.Create_Who = @Create_Who)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                cmd.Parameters.AddWithValue("Create_Who", fn_Params.UserGuid);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("No Data..", Page_SearchUrl);
                        return;
                    }
                    else
                    {
                        //判斷條件是否設定完整
                        int CustCnt = Convert.ToInt16(DT.Rows[0]["CustCnt"]);
                        int ItemCnt = Convert.ToInt16(DT.Rows[0]["ItemCnt"]);
                        if (CustCnt.Equals(0) || ItemCnt.Equals(0))
                        {
                            fn_Extensions.JsAlert("條件設定不完整,請確認", Page_PrevUrl);
                            return;
                        }

                        //[填入資料]
                        this.lt_TableName.Text = DT.Rows[0]["Label"].ToString();

                        //報表資料處理
                        DataProcess("/OpenJun/10-AbroadSalesManager/"
                           , "SYS_SAL_A_001D"
                           , "N"
                           , "N"
                           , "N");
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 報表資料處理
    /// </summary>
    /// <param name="myFolder">Report路徑</param>
    /// <param name="myReportName">Report名稱</param>
    /// <param name="myPDF">可否匯出PDF</param>
    /// <param name="myExcel">可否匯出Excel</param>
    /// <param name="myPrint">可否列印(限IE)</param>
    private void DataProcess(string myFolder, string myReportName, string myPDF, string myExcel, string myPrint)
    {

        #region ** Filter參數處理 **
        //暫存參數
        List<TempParam> ITempList = new List<TempParam>();

        //取得客戶
        ITempList.Add(new TempParam("para_Customer", Get_CustIDs()));

        //取得品號
        ITempList.Add(new TempParam("para_ModelNo", Get_ModelNos()));

        //丟空值給Msg
        ITempList.Add(new TempParam("Msg", ""));
        #endregion


        // 設定參數
        ReportParameter[] _params = new ReportParameter[ITempList.Count];
        for (int row = 0; row < ITempList.Count; row++)
        {
            _params[row] = new ReportParameter(ITempList[row].Param_Name, ITempList[row].Param_Value);
        }

        // 帶出報表(Viewer名稱, 參數集合)
        SetReportViewerAuth(RptData, _params
            , myFolder
            , myReportName
            , myPDF
            , myExcel
            , myPrint);
    }

    /// <summary>
    /// 報表設定
    /// </summary>
    /// <param name="sender">ReportViewer控制項名稱</param>
    /// <param name="_params">報表參數集合</param>
    /// <param name="myFolder">Report路徑</param>
    /// <param name="myReportName">Report名稱</param>
    /// <param name="myPDF">可否匯出PDF</param>
    /// <param name="myExcel">可否匯出Excel</param>
    /// <param name="myPrint">可否列印(限IE)</param>
    public void SetReportViewerAuth(ReportViewer sender, ReportParameter[] _params
        , string myFolder, string myReportName, string myPDF, string myExcel, string myPrint)
    {
        //設定ReportViewer處理模式
        sender.ProcessingMode = ProcessingMode.Remote;

        //** 設定報表屬性 **
        var rpt_with1 = sender.ServerReport;

        // 報表資料夾路徑 (ex:/LockJun/00-Chief/Rpt_Payment)
        rpt_with1.ReportPath = myFolder + myReportName;

        // 報表參數
        rpt_with1.SetParameters(_params);

        /*
         * 若已在web.config設定 ReportViewerServerConnection,
         * 則不要設定 ServerReport.Timeout、ServerReport.ReportServerUrl、ServerReport.ReportServerCredentials、ServerReport.Cookies 或 ServerReport.Headers 屬性
         */
        ////取得報表伺服器路徑
        //string strReportsServer = System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServerUrl"];
        ////指定Uri
        //Uri reportUri = new Uri(strReportsServer);

        // 報表Url (ex:http://pkrpcenter.prokits.com.tw/Report/)
        //rpt_with1.ReportServerUrl = reportUri;
        //取得認證
        //IReportServerCredentials mycred = new MyReportServerConn();
        //rpt_with1.ReportServerCredentials = mycred;


        //關閉報表預設查詢條件
        sender.ShowParameterPrompts = false;
        sender.Visible = true;
        sender.ZoomPercent = 100;
        sender.ShowZoomControl = true;

        //關閉內建的匯出鈕, 用自訂匯出按鈕
        sender.ShowExportControls = false;

        //判斷是否有列印權限(僅限IE)
        sender.ShowPrintButton = (myPrint.Equals("Y") ? true : false);

    }


    /// <summary>
    /// 回傳客戶編號
    /// </summary>
    /// <returns>string</returns>
    /// <example>
    /// 編號1, 編號2, 編號3
    /// </example>
    private string Get_CustIDs()
    {
        string strFullid;

        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 資料查詢
            SBSql.Append(" SELECT CustID AS ID");
            SBSql.Append(" FROM Price_Rel_CustID");
            SBSql.Append(" WHERE (Sheet_ID = @DataID)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    strFullid = "";
                }
                else
                {
                    //字串組合
                    ArrayList ary = new ArrayList();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        ary.Add(DT.Rows[row]["ID"].ToString());
                    }

                    strFullid = string.Join(",", ary.ToArray());
                                        
                }
            }
        }

        return strFullid;
    }

    /// <summary>
    /// 回傳品號
    /// </summary>
    /// <returns>string</returns>
    /// <example>
    /// 編號1, 編號2, 編號3
    /// </example>
    private string Get_ModelNos()
    {
        string strFullid;

        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 資料查詢
            SBSql.Append(" SELECT Model_No AS ID");
            SBSql.Append(" FROM Price_Rel_ModelNo");
            SBSql.Append(" WHERE (Sheet_ID = @DataID)");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    strFullid = "";
                }
                else
                {
                    //字串組合
                    ArrayList ary = new ArrayList();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        ary.Add(DT.Rows[row]["ID"].ToString());
                    }

                    strFullid = string.Join(",", ary.ToArray());

                }
            }
        }

        return strFullid;
    }
    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得傳遞參數 - DataID
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : data;
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    /// <summary>
    /// 設定參數 - 上一頁
    /// </summary>
    private string _Page_PrevUrl;
    public string Page_PrevUrl
    {
        get
        {
            return "{0}myPrice/PriceCompare_Create.aspx?DataID={1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._Page_PrevUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - Search Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            return "{0}myPrice/index.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }
    #endregion


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 名稱
        /// </summary>
        private string _Param_Name;
        public string Param_Name
        {
            get { return this._Param_Name; }
            set { this._Param_Name = value; }
        }

        /// <summary>
        /// [參數] - 值
        /// </summary>
        private string _Param_Value;
        public string Param_Value
        {
            get { return this._Param_Value; }
            set { this._Param_Value = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Name">名稱</param>
        /// <param name="Param_Value">值</param>
        public TempParam(string Param_Name, string Param_Value)
        {
            this._Param_Name = Param_Name;
            this._Param_Value = Param_Value;
        }
    }
}