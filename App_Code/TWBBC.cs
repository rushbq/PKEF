using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TW_BBC.Models
{
    #region >> BBC匯入 <<
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
        /// 訂單單別(User自選)
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 幣別(User自選)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 匯入類型(TW/SH/Prod)
        /// </summary>
        public string Data_Type { get; set; }
        public string Data_TypeName { get; set; }

        /// <summary>
        /// 狀態-Ref:TWBBC_RefClass
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
        /// 出貨地
        /// </summary>
        public string ShipFrom { get; set; }

        /// <summary>
        /// 對應資料庫名
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// ERP公司別:對應EDI欄位XA001
        /// </summary>
        public string Company { get; set; }

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
        /// 訂單數量
        /// </summary>
        public int BuyCnt { get; set; }

        /// <summary>
        /// 修改數量(預設為訂單數量):對應EDI欄位:XA012
        /// </summary>
        public int InputCnt { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        public double? BuyPrice { get; set; }

        /// <summary>
        /// ERP價格:對應EDI欄位:XA013,(使用TWBBC_GetQuotePrice取得)
        /// </summary>
        public double? ERP_Price { get; set; }

        /// <summary>
        /// 贈品(Y/N),Y:XA012=0, XA020=1, XA021=InputCnt
        /// </summary>
        public string IsGift { get; set; }

        /// <summary>
        /// 出貨庫別:對應EDI欄位XA014(取INVMB主要庫別MB017)
        /// </summary>
        public string StockType { get; set; }

        /// <summary>
        /// 內盒產品數量(MB201)
        /// </summary>
        public int InnerBox { get; set; }

        /// <summary>
        /// 外包裝含內盒數(MB200)
        /// </summary>
        public int OuterBox { get; set; }

        /// <summary>
        /// 產銷訊息(MB202)
        /// </summary>
        public string ProdMsg { get; set; }

        /// <summary>
        /// 核價日,(使用TWBBC_GetQuotePrice取得)
        /// </summary>
        public string QuoteDate { get; set; }

        /// <summary>
        /// 上次銷貨日
        /// </summary>
        public string LastSalesDay { get; set; }

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
    /// 類別欄位
    /// </summary>
    public class RefClass
    {
        public int ID { get; set; }
        public string Label { get; set; }
    }


    /// <summary>
    /// ERP 訂單/銷貨單資料 - 單身
    /// </summary>
    public class ERPOrderData
    {
        public string OrderID { get; set; }
        public string ModelNo { get; set; }

        //--訂單資訊
        public string TC001 { get; set; }   //單別
        public string TC002 { get; set; }   //單號
        public string TC003 { get; set; }   //訂單日
        public string TD004 { get; set; }   //品號
        public string TD005 { get; set; }   //品名
        public string TD007 { get; set; }   //庫別
        public int TD008 { get; set; }   //數量

        //--銷貨單資訊
        public string TH001 { get; set; }   //單別
        public string TH002 { get; set; }   //單號
        public string TH004 { get; set; }   //品號
        public string TH005 { get; set; }   //品名
        public string TH007 { get; set; }   //庫別
        public int TH008 { get; set; }   //數量

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



    #endregion


}