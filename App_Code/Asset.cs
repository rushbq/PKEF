using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetData.Models
{
    /// <summary>
    /// 基本資料欄位
    /// </summary>
    public class AssetBase
    {
        public Int32 SeqNo { get; set; }
        public string DBS { get; set; }

        /// <summary>
        /// 類別
        /// </summary>
        public string ClassLv1 { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string ClassLv2 { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string AName { get; set; }

        /// <summary>
        /// 上線日期
        /// </summary>
        public string OnlineDate { get; set; }

        /// <summary>
        /// 維護日期(S ~ E)
        /// </summary>
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IPAddr { get; set; }

        /// <summary>
        /// 網址
        /// </summary>
        public string WebUrl { get; set; }
        public string Remark { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Create_Name { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
        public string Update_Name { get; set; }
    }

}