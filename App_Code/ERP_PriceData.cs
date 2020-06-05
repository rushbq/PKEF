using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * 報價單匯入
 */
namespace ERP_PriceData.Models
{
    /// <summary>
    /// 匯入單頭
    /// </summary>
    public class ImportData
    {
        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public string TraceID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }

        /// <summary>
        /// 單別(User自選)
        /// </summary>
        public string OrderType { get; set; }
        /// <summary>
        /// 單號
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// DBS
        /// </summary>
        public string DBS { get; set; }

        /// <summary>
        /// 生效日
        /// </summary>
        public string ValidDate { get; set; }

        /// <summary>
        /// 10:匯入中 / 20:轉入完成
        /// </summary>
        public Int16 Status { get; set; }
        public string StatusName { get; set; }
        public string Upload_File { get; set; }
        public string Sheet_Name { get; set; }

        /// <summary>
        /// 匯入完成時間
        /// </summary>
        public string Import_Time { get; set; }
        public string Create_Who { get; set; }
        public string Create_Name { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Name { get; set; }
        public string Update_Time { get; set; }

        /// <summary>
        /// 報錯時的訊息
        /// </summary>
        public string ErrMessage { get; set; }

        /// <summary>
        /// 報錯時間
        /// </summary>
        public string ErrTime { get; set; }

    }


    /// <summary>
    /// 匯入單身
    /// </summary>
    public class ImportDataDT
    {
        public Guid Parent_ID { get; set; }
        public int Data_ID { get; set; }

        /// <summary>
        /// EXCEL中的品號
        /// </summary>
        public string ProdID { get; set; }

        /// <summary>
        /// 客戶品號:對應EDI欄位:XA026
        /// </summary>
        public string Cust_ModelNo { get; set; }

        /// <summary>
        /// 品號:對應EDI欄位:XA011
        /// </summary>
        public string ERP_ModelNo { get; set; }

        /// <summary>
        /// 金額:對應EDI欄位:XA013
        /// </summary>
        public double? InputPrice { get; set; }
        
        /// <summary>
        /// 是否通過檢查
        /// </summary>
        public string IsPass { get; set; }

        /// <summary>
        /// 說明在幹嘛(系統)
        /// </summary>
        public string doWhat { get; set; }

    }

    
    /// <summary>
    /// EDIXA轉入失敗記錄或排程中
    /// </summary>
    public class EDILog
    {
        public string OrderID { get; set; }
        public string ModelNo { get; set; }
        public string Why { get; set; }
    }


    /// <summary>
    /// 類別欄位
    /// </summary>
    public class RefClass
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string NoType { get; set; }
    }


}