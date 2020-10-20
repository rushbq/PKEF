using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using ClosedXML.Excel;
using CustomController;
using ExtensionMethods;

namespace ExtensionUI
{
    /// <summary>
    /// 自訂常用的UI
    /// 換網站時注意DB名前置詞
    /// </summary>
    public class fn_CustomUI
    {
        private static string ErrMsg;


        #region -- 目標設定 --

        /// <summary>
        /// 產生Tab Html
        /// </summary>
        /// <param name="typeID">目前的ID</param>
        /// <param name="url">目標網址</param>
        /// <returns></returns>
        public static string GetTabHtml(string typeID, string url)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<ul>");
            html.Append("<li class=\"{0}\"><a href=\"{1}?t=1\">工具</a></li>".FormatThis(
                        typeID.Equals("1") ? "TabAc" : ""
                        , url
                    ));
            html.Append("<li class=\"{0}\"><a href=\"{1}?t=2\">玩具</a></li>".FormatThis(
                        typeID.Equals("2") ? "TabAc" : ""
                        , url
                    ));
            html.Append("</ul>");

            return html.ToString();
        }
        #endregion

        #region -- EXCEL匯出 --

        public static void ExportExcel(DataTable DT, string fileName)
        {
            //default
            ExportExcel(DT, fileName, true);
        }

        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="fileName"></param>
        /// <remarks>
        /// 使用元件:ClosedXML
        /// </remarks>
        /// <seealso cref="https://github.com/ClosedXML/ClosedXML/wiki"/>
        public static void ExportExcel(DataTable DT, string fileName, bool setPassword)
        {
            //宣告
            XLWorkbook wbook = new XLWorkbook();

            //-- 工作表設定 Start --
            var ws = wbook.Worksheets.Add(DT, "PKDataList");

            if (setPassword)
            {
                //鎖定工作表, 並設定密碼
                ws.Protect("iLoveProkits25")    //Set Password
                    .SetFormatCells(true)   // Cell Formatting
                    .SetInsertColumns() // Inserting Columns
                    .SetDeleteColumns() // Deleting Columns
                    .SetDeleteRows();   // Deleting Rows
            }

            //細項設定
            ws.Tables.FirstOrDefault().ShowAutoFilter = false;  //停用自動篩選
            ws.Style.Font.FontName = "Microsoft JhengHei";  //字型名稱
            ws.Style.Font.FontSize = 10;

            //修改標題列
            var header = ws.FirstRowUsed(false);
            //header.Style.Fill.BackgroundColor = XLColor.Green;
            //header.Style.Font.FontColor = XLColor.Yellow;
            header.Style.Font.FontSize = 12;
            header.Style.Font.Bold = true;
            header.Height = 22;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //-- 工作表設定 End --

            //Http Response & Request
            var resp = HttpContext.Current.Response;
            var req = HttpContext.Current.Request;
            HttpResponse httpResponse = resp;
            httpResponse.Clear();
            // 編碼
            httpResponse.ContentEncoding = Encoding.UTF8;
            // 設定網頁ContentType
            httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // 匯出檔名
            var browser = req.Browser.Browser;
            var exportFileName = browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : HttpUtility.UrlEncode(fileName, Encoding.UTF8);

            resp.AddHeader(
                "Content-Disposition",
                string.Format("attachment;filename={0}", exportFileName));

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wbook.SaveAs(memoryStream);
                memoryStream.WriteTo(httpResponse.OutputStream);
                memoryStream.Close();
            }

            httpResponse.End();
        }

        /// <summary>
        /// Linq查詢結果轉Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// 此方法僅可接受IEnumerable<T>泛型物件
        /// DataTable dt = LINQToDataTable(query);
        /// </remarks>
        public static DataTable LINQToDataTable<T>(IEnumerable<T> query)
        {
            //宣告一個datatable
            DataTable tbl = new DataTable();
            //宣告一個propertyinfo為陣列的物件，此物件需要import reflection才可以使用
            //使用 ParameterInfo 的執行個體來取得有關參數的資料型別、預設值等資訊

            PropertyInfo[] props = null;
            //使用型別為T的item物件跑query的內容
            foreach (T item in query)
            {
                if (props == null) //尚未初始化
                {
                    //宣告一型別為T的t物件接收item.GetType()所回傳的物件
                    Type t = item.GetType();
                    //props接收t.GetProperties();所回傳型別為props的陣列物件
                    props = t.GetProperties();
                    //使用propertyinfo物件針對propertyinfo陣列的物件跑迴圈
                    foreach (PropertyInfo pi in props)
                    {
                        //將pi.PropertyType所回傳的物件指給型別為Type的coltype物件
                        Type colType = pi.PropertyType;
                        //針對Nullable<>特別處理
                        if (colType.IsGenericType
                            && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            colType = colType.GetGenericArguments()[0];
                        //建立欄位
                        tbl.Columns.Add(pi.Name, colType);
                    }
                }
                //宣告一個datarow物件
                DataRow row = tbl.NewRow();
                //同樣利用PropertyInfo跑迴圈取得props的內容，並將內容放進剛所宣告的datarow中
                //接著在將該datarow加到datatable (tb1) 當中
                foreach (PropertyInfo pi in props)
                    row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
                tbl.Rows.Add(row);
            }
            //回傳tb1的datatable物件
            return tbl;
        }


        /// <summary>
        /// 排除使用者KEY在輸入欄裡特殊字元
        /// </summary>
        /// <param name="tmp">
        /// <returns></returns>
        /// <remarks>
        /// ex:(十六進位值 0x0B) 是無效的字元
        /// </remarks>
        public static string ReplaceLowOrderASCIICharacters(string tmp)
        {
            StringBuilder info = new StringBuilder();
            foreach (char cc in tmp)
            {
                int ss = (int)cc;
                if (((ss >= 0) && (ss <= 8)) || ((ss >= 11) && (ss <= 12)) || ((ss >= 14) && (ss <= 32)))
                    info.AppendFormat(" ", ss);//&#x{0:X};
                else info.Append(cc);
            }
            return info.ToString();
        }

        #endregion

        #region -- 資訊需求 --
        /// <summary>
        /// 取得指定收信人
        /// </summary>
        /// <returns></returns>
        public static List<string> emailReceiver(string mailType)
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT MailAddress ");
                SBSql.AppendLine(" FROM IT_Help_Receiver WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (MailType = @MailType) AND (Display = 'Y')");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("MailType", mailType);

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    List<string> GetEmail = new List<string>();

                    //若無資料
                    if (DT.Rows.Count == 0)
                    {
                        GetEmail.Add("ITInform@mail.prokits.com.tw");
                    }
                    else
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                        }
                    }

                    return GetEmail;

                }
            }
        }


        /// <summary>
        /// 取得部門主管Email
        /// </summary>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public static List<string> emailReceiver_Supervisor(string deptID)
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Prof.Email MailAddress");
                SBSql.AppendLine(" FROM User_Dept_Supervisor Info WITH (NOLOCK)");
                SBSql.AppendLine("  INNER JOIN User_Profile Prof WITH (NOLOCK) ON Info.Account_Name = Prof.Account_Name");
                SBSql.AppendLine(" WHERE (Info.DeptID = @DeptID) AND (Prof.Display = 'Y')");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DeptID", deptID);

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    List<string> GetEmail = new List<string>();

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                    }

                    return GetEmail;

                }
            }
        }


        /// <summary>
        /// 取得CC名單
        /// </summary>
        /// <param name="traceID"></param>
        /// <returns></returns>
        public static List<string> emailCC(string traceID)
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Item_Email AS MailAddress ");
                SBSql.AppendLine(" FROM IT_Help_CC WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("TraceID", traceID);

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    List<string> GetEmail = new List<string>();

                    //若無資料則塞mis@mail.prokits.com.tw
                    if (DT.Rows.Count == 0)
                    {
                        GetEmail.Add("mis@mail.prokits.com.tw");
                    }
                    else
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                        }
                    }

                    return GetEmail;

                }
            }
        }

        #endregion

        #region -- OP需求 --
        /// <summary>
        /// 取得指定收信人
        /// </summary>
        /// <returns></returns>
        public static List<string> OP_emailReceiver(string mailType)
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT MailAddress ");
                SBSql.AppendLine(" FROM OP_Help_Receiver WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (MailType = @MailType) AND (Display = 'Y')");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("MailType", mailType);

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    List<string> GetEmail = new List<string>();

                    //若無資料則塞mis@mail.prokits.com.tw
                    if (DT.Rows.Count == 0)
                    {
                        GetEmail.Add("opteam@mail.prokits.com.tw");
                    }
                    else
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                        }
                    }

                    return GetEmail;

                }
            }
        }


        /// <summary>
        /// 取得CC名單
        /// </summary>
        /// <param name="traceID"></param>
        /// <returns></returns>
        public static List<string> OP_emailCC(string traceID)
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Item_Email AS MailAddress ");
                SBSql.AppendLine(" FROM OP_Help_CC WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (TraceID = @TraceID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("TraceID", traceID);

                // SQL查詢執行
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    List<string> GetEmail = new List<string>();

                    //若無資料則塞mis@mail.prokits.com.tw
                    if (DT.Rows.Count == 0)
                    {
                        GetEmail.Add("opteam@mail.prokits.com.tw");
                    }
                    else
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                        }
                    }

                    return GetEmail;

                }
            }
        }

        #endregion
    }
}
