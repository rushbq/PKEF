using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MISData.Models
{
    /// <summary>
    /// Menu欄位
    /// </summary>
    public class WebMenu
    {
        public Int32 MenuID { get; set; }
        public Int32 ParentID { get; set; }
        public string MenuName { get; set; }
        public Int32 Level { get; set; }
        public Int32 ClickCnt { get; set; }
        public Int32 Sort { get; set; }
        public string Remark { get; set; }
    }

    public class WebMenuTable
    {
        public Int32 MenuID { get; set; }
        public string Lv1Name { get; set; }
        public string Lv2Name { get; set; }
        public string Lv3Name { get; set; }
        public string Lv4Name { get; set; }
        public Int32 ClickCnt { get; set; }
        public Int32 Lv { get; set; }
        public string Remark { get; set; }
    }
}