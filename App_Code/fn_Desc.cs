using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 各功能的中英文描述轉換
/// </summary>
public class fn_Desc
{
    /// <summary>
    /// 問卷 & 投票
    /// </summary>
    public class QnA
    {
        /// <summary>
        /// 範本類型
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string qType(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "QA":
                    return "問卷";

                case "TEST":
                    return "測驗卷";

                default:
                    return "";
            }
        }

        /// <summary>
        /// 問題類型
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string optType(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";
            switch (inputValue.ToUpper())
            {
                case "YESNO":
                    return "是非";

                case "RADIO":
                    return "單選";

                case "CHECK":
                    return "複選";

                case "ESSAY":
                    return "問答";

                case "RADIOPIC":
                    return "圖片單選";

                default:
                    return "";
            }
        }
    }


    /// <summary>
    /// 共用類
    /// </summary>
    public class PubAll
    {
        /// <summary>
        /// 是否
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <param name="lang">語系別</param>
        /// <returns>string</returns>
        public static string YesNo(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "Y":
                    return "是";

                case "N":
                    return "否";

                default:
                    return "";
            }
        }


        /// <summary>
        /// 公司區域碼 - 中文描述
        /// </summary>
        /// <param name="areaCode">區域碼</param>
        /// <returns></returns>
        public static string AreaDesc(string areaCode)
        {
            if (string.IsNullOrEmpty(areaCode))
            {
                return "";
            }
            else
            {
                switch (areaCode.ToUpper())
                {
                    case "TW":
                        return "台灣";
                    case "SH":
                        return "上海";
                    case "SZ":
                        return "深圳";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 性別
        /// </summary>
        /// <param name="inputValue">1/2</param>
        /// <returns></returns>
        public static string Sex(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "1":
                    return "男";

                case "2":
                    return "女";

                default:
                    return "";
            }
        }

        /// <summary>
        /// 性別
        /// </summary>
        /// <param name="inputValue">M/F</param>
        /// <returns></returns>
        public static string Gender(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "M":
                    return "先生";

                case "F":
                    return "小姐";

                default:
                    return "";
            }
        }
    }


    public class BBC
    {
        /// <summary>
        /// 取得模式名稱
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static string Get_ModeName(string type)
        {
            switch (type)
            {
                case "1":
                    return "未出貨訂單";

                case "2":
                    return "退貨單匯入";

                case "3":
                    return "已出貨訂單";

                default:
                    return "未出貨訂單";
            }
        }

        /// <summary>
        /// 狀態名稱
        /// </summary>
        /// <param name="inputVal"></param>
        /// <returns></returns>
        public static string Get_StatusName(string inputVal)
        {
            switch (inputVal)
            {
                case "1":
                    return "未開票";

                case "2":
                    return "開票中";

                case "3":
                    return "已開票";

                default:
                    return "";
            }
        }


        /// <summary>
        /// 發票類型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetInvTypeName(string type)
        {
            switch (type)
            {
                case "1":
                    return "普票";

                default:
                    return "专票";
            }
        }
    }
}