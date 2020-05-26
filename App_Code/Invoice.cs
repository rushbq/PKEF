using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Invoice.Models
{

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

    public class InvData_Base
    {
        public Guid Data_ID { get; set; }
        public string CompID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string erp_sDate { get; set; }
        public string erp_eDate { get; set; }
        public string InvDate { get; set; }
        public string InvNo { get; set; }
        public string Staus { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }



    }

    public class InvData_DT
    {
        public Guid Parent_ID { get; set; }
        public Int64 Data_ID { get; set; }
        public string Erp_AR_ID { get; set; }
    }


    /// <summary>
    /// 發票欄位
    /// </summary>
    public class InvoiceData
    {
        public Int64 SerialNo { get; set; }
        public string OrderID { get; set; }
        public string TraceID { get; set; }
        public string NickName { get; set; }
        public string SO_FID { get; set; }
        public string SO_SID { get; set; }
        public string SO_Date { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public double TotalPrice { get; set; }  //SQL float資料型態 要使用 double
        public string BI_FID { get; set; }
        public string BI_SID { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string InvTitle { get; set; }
        public string InvType { get; set; }
        public string InvNumber { get; set; }
        public string InvAddrInfo { get; set; }
        public string InvBankInfo { get; set; }
        public string InvMessage { get; set; }
        public string InvRemark { get; set; }
        public string InvStatus { get; set; }


        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }

        public int? Item_DataID { get; set; }
    }


    /// <summary>
    /// 銷貨明細資料欄
    /// </summary>
    public class RefColumn
    {
        public string SO_FID { get; set; }
        public string SO_SID { get; set; }
        public string SO_No { get; set; }
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public double TaxPrice { get; set; }
        public string IsInvoice { get; set; }

    }


    /// <summary>
    /// 匯出欄位-主檔
    /// </summary>
    public class ExportBase
    {
        public Int64 SerialNo { get; set; }
        public Guid DataID { get; set; }
        public string Subject { get; set; }
        public string IsSend { get; set; }
        public string Create_Name { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Name { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
        public string Send_Who { get; set; }
        public string Send_Time { get; set; }

        public int RelCnt { get; set; }
    }


    /// <summary>
    /// 匯出檔 - 明細(EXCEL)
    /// </summary>
    public class ExportDT
    {
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public double TaxPrice { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public string SO_No { get; set; }
        public string SO_Date { get; set; }
        public string Inv_Type { get; set; }
        public string Inv_Title { get; set; }
        public string Inv_Number { get; set; }
        public string Inv_AddrInfo { get; set; }
        public string Inv_BankInfo { get; set; }

        //--會計固定欄位(值)
        public string Acc_ClassNo { get; set; }
        public string Acc_VerNo { get; set; }
        public string Acc_Taxlaw { get; set; }
        public string Inv_Remark { get; set; }
    }


    /// <summary>
    /// 發票匯出檔收件者
    /// </summary>
    public class MailtoList
    {
        public string mailAddr { get; set; }
        public string mailName { get; set; }
    }


    /// <summary>
    /// 發票子檔欄位
    /// </summary>
    public class InvoiceColumn
    {
        public Int64 SerialNo { get; set; }
        public string InvTitle { get; set; }
        public string InvType { get; set; }
        public string InvNumber { get; set; }
        public string InvAddrInfo { get; set; }
        public string InvBankInfo { get; set; }
        public string InvMessage { get; set; }
    }
}