using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
  SZ-開票平台 for 航天中繼Table
 */
namespace SZ_Invoice.Aisino.Models
{
    /// <summary>
    /// [一般紙本發票] Step1 檢查欄位
    /// </summary>
    public class DT_Check
    {
        public string Erp_SO_ID { get; set; }
        public string Erp_AR_ID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public double TotalPrice { get; set; }
    }


    /// <summary>
    /// BBC關聯資料
    /// </summary>
    public class BBC_Data
    {
        public Guid Parent_ID { get; set; }
        public int Data_ID { get; set; }
        public string Erp_SO_ID { get; set; }
        public string Erp_AR_ID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public double TotalPrice { get; set; }
        public int MallID { get; set; }
        public string InvType { get; set; }
        public string InvTypeName { get; set; }
        public string TraceID { get; set; }
        public string OrderID { get; set; }
        public string vendeename { get; set; }
        public string vendeetax { get; set; }
        public string vendeeadress { get; set; }
        public string vendeebnkno { get; set; }
    }


    /// <summary>
    /// 本機檢查檔-單頭
    /// </summary>
    public class DT_Base
    {
        public Guid Data_ID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string BuyerName { get; set; }
        public string InvType { get; set; }
        public string Inv_UID { get; set; }
        public string Inv_Subject { get; set; }
        public string Inv_NO { get; set; }
        public string Inv_Date { get; set; }
        public string IsInsert { get; set; }
        public string IsUpdate { get; set; }
        public int IsRel { get; set; }  //發票系統是否已回寫,可UPDATE發票資料
        public Int16 DataType { get; set; } //1:一般紙本發票 2:電商紙本發票
        public string vendeename { get; set; }

        public string Create_Time { get; set; }
        public string Update_Time { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }

    }


    /// <summary>
    /// 本機檢查檔-單身
    /// </summary>
    public class DT_Detail
    {
        public Guid Parent_ID { get; set; }
        public int Data_ID { get; set; }
        public string Erp_SO_ID { get; set; }
        public string Erp_AR_ID { get; set; }
    }


    /// <summary>
    /// Step2 中繼檔欄位-單頭
    /// </summary>
    public class DT_Header
    {
        public string unino { get; set; }
        public string erp_unino { get; set; }
        public string vendeename { get; set; }
        public string vendeetax { get; set; }
        public string vendeeadress { get; set; }
        public string vendeebnkno { get; set; }
        public string billdate { get; set; }
        public string remark { get; set; }
        public int invoicekind { get; set; }
        public string sta { get; set; }
        public string negativesign { get; set; }
    }


    /// <summary>
    /// Step2 中繼檔欄位-單身
    /// </summary>
    public class DT_Lines
    {
        public string unino { get; set; }
        public Int16 serial { get; set; }
        public string linenumber { get; set; }
        public double qty { get; set; }
        public double price { get; set; }
        public double shpamt { get; set; }
        public double taxrate { get; set; }
        public double taxation { get; set; }
        public string tradename { get; set; }
        public string model { get; set; }
        public string unit { get; set; }
        public double oldprice { get; set; }
        public double disamt { get; set; }
        public string taxprice { get; set; }
        public string bmbbh { get; set; }
        public string ssflbm { get; set; }
        public string xsyhzc { get; set; }

    }


    /// <summary>
    /// ERP未開票資料
    /// </summary>
    public class ERP_Invoice
    {
        //銷貨/退貨單號
        public string Erp_SO_ID { get; set; }
        //結帳單號
        public string Erp_AR_ID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        //結帳日
        public string ArDate { get; set; }
        //發票號碼
        public string InvNo { get; set; }
        public Int64 SerialNo { get; set; }
    }

}
