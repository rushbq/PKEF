using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZ_BBC.Models
{
    #region >> BBC匯入 <<
    /// <summary>
    /// 單頭資料欄位
    /// </summary>
    public class ImportData
    {
        #region -- 資料庫欄位 --

        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public string TraceID { get; set; }
        public int MallID { get; set; }
        public string CustID { get; set; }
        public string SalesID { get; set; }
        public decimal Data_Type { get; set; }
        public decimal Status { get; set; }
        public string Upload_File { get; set; }
        public string Sheet_Name { get; set; }
        public string Import_Time { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }

        public string MallName { get; set; }
        public string CustName { get; set; }
        public string StatusName { get; set; }
        public string Data_TypeName { get; set; }
        public string Remark { get; set; }

        public int LogCnt { get; set; }

        #endregion


        #region -- 關聯欄位 --

        public string Create_Name { get; set; }
        public string Update_Name { get; set; }

        #endregion

    }

    /// <summary>
    /// 類別欄位
    /// </summary>
    public class RefClass
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public string IsStock { get; set; }
    }


    /// <summary>
    /// 單身資料欄
    /// </summary>
    public class RefColumn
    {
        public Guid Parent_ID { get; set; }
        public int Data_ID { get; set; }
        public string OrderID { get; set; }
        public string ProdID { get; set; }
        public string ProdSpec { get; set; }
        public string ProdName { get; set; }
        public int BuyCnt { get; set; }
        public double? BuyPrice { get; set; }
        public double? TotalPrice { get; set; }
        public double? Freight { get; set; }
        public string ShipmentNo { get; set; }
        public string IsPass { get; set; }
        public string doWhat { get; set; }
        public string ERP_ModelNo { get; set; }
        public double? ERP_Price { get; set; }
        public string Currency { get; set; }
        public string ShipWho { get; set; }
        public string ShipAddr { get; set; }
        public string ShipTel { get; set; }
        public int StockNum { get; set; }
        public string IsGift { get; set; }
        public string RemarkTitle { get; set; }
        public string RemarkID { get; set; }
        public string Remark { get; set; }
        public string StockReback { get; set; }
        public string NickName { get; set; }

        public string InvType { get; set; }
        public string InvTitle { get; set; }
        public string InvNumber { get; set; }
        public string InvAddrInfo { get; set; }
        public string InvBankInfo { get; set; }
        public string InvMessage { get; set; }
        public string InvRemark { get; set; }

        public string Buy_ProdName { get; set; }
        public string Buy_Place { get; set; }
        public string Buy_Warehouse { get; set; }
        public string Buy_Sales { get; set; }
        public string Buy_Time { get; set; }
        public string BuyRemark { get; set; }
        public string SellRemark { get; set; }
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


    /// <summary>
    /// ERP 訂單/銷貨單資料 - 單身
    /// </summary>
    public class ERPData
    {
        public string OrderID { get; set; }
        public string ModelNo { get; set; }

        //--訂單資訊
        public string TC001 { get; set; }   //單別
        public string TC002 { get; set; }   //單號
        public string TC003 { get; set; }   //訂單日
        public string TD004 { get; set; }   //品號
        public string TD005 { get; set; }   //品名
        public int TD008 { get; set; }   //數量
        public string TD007 { get; set; }   //庫別

        //--銷貨單資訊
        public string TH001 { get; set; }   //單別
        public string TH002 { get; set; }   //單號
        public string TH004 { get; set; }   //品號
        public string TH005 { get; set; }   //品名
        public string TH007 { get; set; }   //庫別
        public int TH008 { get; set; }   //數量

    }


    /// <summary>
    /// EDIXA轉入失敗記錄
    /// </summary>
    public class EDILog
    {
        public string OrderID { get; set; }
        public string ModelNo { get; set; }
        public string Why { get; set; }
    }


    /// <summary>
    /// ERP銷退單
    /// </summary>
    public class RebackData
    {
        public string TI001 { get; set; }   //單別
        public string TI002 { get; set; }   //單號
        public string TI003 { get; set; }   //銷退日
        public string TJ004 { get; set; }   //品號
        public string TJ005 { get; set; }   //品名
        public int TJ007 { get; set; }   //數量
        public string TI020 { get; set; }   //單頭備註
        public string TJ023 { get; set; }   //單身備註
        public string TJ052 { get; set; }   //銷退原因代號
        public string TJ025 { get; set; }   //結帳單別
        public string TJ026 { get; set; }   //結帳單號

    }


    /// <summary>
    /// ERP借出單
    /// </summary>
    public class InvData
    {
        public string TG001 { get; set; }   //異動單別
        public string TG002 { get; set; }   //異動單號
        public string TC012 { get; set; }   //原始單號
        public string TG004 { get; set; }   //品號
        public string TG005 { get; set; }   //品名
        public string TG007 { get; set; }   //轉出庫別
        public string TG008 { get; set; }   //轉入庫別
        public int TG009 { get; set; }   //數量
        public string TG014 { get; set; }   //來源單別
        public string TG015 { get; set; }   //來源單號

    }


    /// <summary>
    /// 客戶對應表
    /// </summary>
    public class CustRef
    {
        public string Platform_ID { get; set; }
        public string ERP_ID { get; set; }
        public string DispType { get; set; }
    }


    /// <summary>
    /// 促銷活動-單頭
    /// </summary>
    public class PromoBase
    {
        public Guid Data_ID { get; set; }
        public string PromoName { get; set; }
        public int MallID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Int16 PromoType { get; set; }
        public double? TargetMoney { get; set; }
        public string TargetItem { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }
        public string MallName { get; set; }
        public string TypeName { get; set; }
        public Int32 ChildCnt { get; set; }

    }

    /// <summary>
    /// 促銷活動-單身
    /// </summary>
    public class PromoDT
    {
        public Int32 Data_ID { get; set; }
        public string ModelNo { get; set; }
        public Int32 Qty { get; set; }
    }


    /// <summary>
    /// ERP COPMG自訂對應
    /// 天貓商品SKU為一對多, 在ERP上無法設定
    /// </summary>
    public class RefModel
    {
        public Int64 Data_ID { get; set; } //(bigint)
        public int MallID { get; set; }
        public string MallName { get; set; }
        public string CustID { get; set; } //MG001
        public string ModelNo { get; set; } //MG002
        public string CustModelNo { get; set; } //MG003
        public string CustSpec { get; set; } //MG006
    }
    #endregion
    

    #region >> 出貨明細表 <<
    /// <summary>
    /// 出貨明細
    /// </summary>
    public class Shipment
    {
        public Int64 RowRank { get; set; }
        public Guid SrcParentID { get; set; }
        public int SrcDataID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string MallName { get; set; }
        public string OrderID { get; set; }
        public string ModelNo { get; set; }
        public int BuyCnt { get; set; }
        public string Erp_SO_ID { get; set; }
        public string ShipNo { get; set; }
        public string ShipWho { get; set; }
        public string ShipAddr { get; set; }
        public string ShipTel { get; set; }
        public string InvType { get; set; }
        public double InvPrice { get; set; }
        public string InvTitle { get; set; }
        public string InvNumber { get; set; }
        public string InvAddrInfo { get; set; }
        public string InvBankInfo { get; set; }
    }


    /// <summary>
    /// 出貨明細匯出-單頭
    /// </summary>
    public class ShipExport
    {
        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Create_Name { get; set; }

    }

    /// <summary>
    /// 出貨明細匯出-單身
    /// </summary>
    public class ShipExportDT
    {
        public int Data_ID { get; set; }
        public string OrderID { get; set; }
        public string ModelNo { get; set; }
        public Guid SrcParentID { get; set; }
        public int SrcDataID { get; set; }
    }
    #endregion


    #region >> 電商庫存 <<
    /// <summary>
    /// 電商庫存
    /// </summary>
    public class StockData
    {
        public string ModelNo { get; set; }
        public string EC1_SKU { get; set; }
        public int EC1_StockNum { get; set; }
        public string EC2_SKU { get; set; }
        public int EC2_StockNum { get; set; }
        public string EC3_SKU { get; set; }
        public int EC3_StockNum { get; set; }
        public string EC4_SKU { get; set; }
        public int EC4_StockNum { get; set; }
        public int StockQty_B01 { get; set; }
        public int PreSell_B01 { get; set; }
        public int PreIN_B01 { get; set; }
    }


    /// <summary>
    /// 電商庫存-匯入欄位-單頭
    /// </summary>
    public class StockImportData
    {
        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public int MallID { get; set; }
        public string CustID { get; set; }
        public string StockDate { get; set; }
        public string Upload_File { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }

        public string MallName { get; set; }
        public string CustName { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }
    }


    /// <summary>
    /// 電商庫存-匯入欄位-單身
    /// </summary>
    public class StockImportDataDT
    {
        public string ProdID { get; set; }
        public int StockNum { get; set; }
        public string ERP_ModelNo { get; set; }
        public string IsPass { get; set; }
        public string doWhat { get; set; }
    }
    #endregion


    #region >> 平台對應 <<

    /// <summary>
    /// [平台對應] ERP 訂單/銷貨單資料 - 單頭
    /// </summary>
    public class ERPDataList
    {
        public string OrderID { get; set; }
        public string MallName { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string ShipmentNo { get; set; }
        public string ShipTel { get; set; }

        //--訂單資訊
        public string TC001 { get; set; }   //單別
        public string TC002 { get; set; }   //單號

        //--銷貨單資訊
        public string TH001 { get; set; }   //單別
        public string TH002 { get; set; }   //單號
        //--結帳單資訊
        public string TA001 { get; set; }   //單別
        public string TA002 { get; set; }   //單號
        public double TotalPrice { get; set; }

    }
    #endregion


    #region *** 未使用 ***
    /// <summary>
    /// 物流單頭欄位
    /// </summary>
    public class ShippingBase
    {
        public int Data_ID { get; set; }
        public string Label { get; set; }
    }


    /// <summary>
    /// 物流單身欄位
    /// </summary>
    public class ShippingDT
    {
        public int Data_ID { get; set; }
        public string ShipNo { get; set; }
        public string OrderID { get; set; }
        public string IsUse { get; set; }
        public string MallName { get; set; }
        public string CustName { get; set; }

        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Create_Name { get; set; }

    }
    #endregion

   
}