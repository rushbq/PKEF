using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
  SH-開票平台
 */
namespace SH_Invoice.Models
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

}
