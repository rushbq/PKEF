using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProdExt.Models
{
    /// <summary>
    /// 欄位
    /// </summary>
    public class ItemData
    {
        public string Model_No { get; set; }
        public int SafeQty_SZEC { get; set; }
        public int SafeQty_A01 { get; set; }
        public int SafeQty_B01 { get; set; }

        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }
    }

}