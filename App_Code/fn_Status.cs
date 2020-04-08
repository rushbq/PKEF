using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 各種狀態的名稱
/// </summary>
public class fn_Status
{
    /// <summary>
    /// 上下架名稱
    /// </summary>
    /// <param name="val"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string showDisp(string val, string type)
    {
        switch (type)
        {
            case "word":
                switch (val.ToUpper())
                {
                    case "N":
                        return "已下架";

                    case "Y":
                        return "上架中";

                    default:
                        return "";
                }

            default:
                switch (val.ToUpper())
                {
                    case "N":
                        return "grey";

                    case "Y":
                        return "blue";

                    default:
                        return "";
                }
        }

    }


    /// <summary>
    /// Lang Name
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string showLang(string val)
    {
        switch (val.ToUpper())
        {
            case "TW":
                return "繁中";

            case "CN":
                return "簡中";

            default:
                return "英文";
        }
    }


    /// <summary>
    /// TWBBC DataType Name
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string showTWBBC_DataTypeName(string val)
    {
        switch (val.ToUpper())
        {
            case "TW":
                return "台灣資料庫";

            case "SH":
                return "上海資料庫";

            default:
                return "產品出貨地";
        }
    }
}