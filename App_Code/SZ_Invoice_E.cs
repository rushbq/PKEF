using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZ_Invoice_E.Models
{
    /// <summary>
    /// 單頭資料欄位
    /// </summary>
    public class ImportData
    {
        #region -- 資料庫欄位 --

        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public string TraceID { get; set; }
        public decimal Status { get; set; }
        public string Upload_File { get; set; }
        public string Sheet_Name { get; set; }
        public string Import_Time { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }

        public string StatusName { get; set; }
        public string Remark { get; set; }

        public int LogCnt { get; set; }

        #endregion


        #region -- 關聯欄位 --

        public string Create_Name { get; set; }
        public string Update_Name { get; set; }
        public string Import_Name { get; set; }

        #endregion

    }

    /// <summary>
    /// ERP 結帳資料
    /// </summary>
    public class ERPData
    {
        public string OrderID { get; set; }
        public string ErpID { get; set; }
        public string ErpSOID { get; set; }
        public double ErpPrice { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public double InvPrice { get; set; }
    }

    /// <summary>
    /// 類別欄位
    /// </summary>
    public class RefClass
    {
        public int ID { get; set; }
        public string Label { get; set; }
    }


    /// <summary>
    /// 單身資料欄
    /// </summary>
    public class RefColumn
    {
        public int Data_ID { get; set; }
        public string OrderID { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public double InvPrice { get; set; }

        public string IsPass { get; set; }
        public string doWhat { get; set; }
        public string Erp_AR_ID { get; set; }
        public string Erp_SO_ID { get; set; }

    }


    /// <summary>
    /// Log欄位
    /// </summary>
    public class RefLog
    {
        public Int64 Log_ID { get; set; }
        public string TraceID { get; set; }
        public string Log_Desc { get; set; }
        public string Create_Time { get; set; }
    }

}