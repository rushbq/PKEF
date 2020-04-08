using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Security.Cryptography;
using System.Collections;
using CustomController;

namespace ExtensionMethods
{
    public static class fn_Extensions
    {
        #region "一般功能"
        /// <summary>
        /// 簡化string.format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatThis(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 取得Right字串
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="length">取得長度</param>
        /// <returns>string</returns>
        /// <example>
        /// string str = "12345";
        /// str = str.Right(3);  //345
        /// </example>
        public static string Right(this string inputValue, int length)
        {
            length = Math.Max(length, 0);

            if (inputValue.Length > length)
            {
                return inputValue.Substring(inputValue.Length - length, length);
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// 取得Left字串
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="length">取得長度</param>
        /// <returns>string</returns>
        /// <example>
        /// string str = "12345";
        /// str = str.Left(3);  //123
        /// </example>
        public static string Left(this string inputValue, int length)
        {
            length = Math.Max(length, 0);

            if (inputValue.Length > length)
            {
                return inputValue.Substring(0, length);
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// 取得各參數串的值
        /// </summary>
        /// <param name="str">String to process</param>
        /// <param name="OuterSeparator">Separator for each "NameValue"</param>
        /// <param name="NameValueSeparator">Separator for Name/Value splitting</param>
        /// <returns></returns>
        /// <example>
        /// string para = "param1=value1;param2=value2";
        /// NameValueCollection nv = para.ToNameValueCollection(';', '=');
        /// foreach (var item in nv)
        /// {
        ///     Response.Write(item + "<BR>");
        /// }
        /// </example>
        public static NameValueCollection ToNameValueCollection(this String inputValue, Char OuterSeparator, Char NameValueSeparator)
        {
            NameValueCollection nvText = null;
            inputValue = inputValue.TrimEnd(OuterSeparator);
            if (!String.IsNullOrEmpty(inputValue))
            {
                String[] arrStrings = inputValue.TrimEnd(OuterSeparator).Split(OuterSeparator);

                foreach (String s in arrStrings)
                {
                    Int32 posSep = s.IndexOf(NameValueSeparator);
                    String name = s.Substring(0, posSep);
                    String value = s.Substring(posSep + 1);
                    if (nvText == null)
                        nvText = new NameValueCollection();
                    nvText.Add(name, value);
                }
            }
            return nvText;
        }

        /// <summary>
        /// 檢查格式 - 是否為日期
        /// </summary>
        /// <param name="inputValue">日期</param>
        /// <returns>bool</returns>
        /// <example>
        /// string someDate = "2010/1/5";
        /// bool isDate = nonDate.IsDate();
        /// </example>
        public static bool IsDate(this string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                DateTime dt;
                return (DateTime.TryParse(inputValue, out dt));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查格式 - 是否為網址
        /// </summary>
        /// <param name="inputValue">網址字串</param>
        /// <returns>bool</returns>
        public static bool IsUrl(this string inputValue)
        {
            return Regex.IsMatch(inputValue, "http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&amp;=]*)?");
        }

        /// <summary>
        /// 檢查格式 - 是否為座標
        /// </summary>
        /// <param name="Lat">座標-Lat字串</param>
        /// <param name="Lng">座標-Lng字串</param>
        /// <returns>Boolean</returns>
        public static bool IsLatLng(string Lat, string Lng)
        {
            if (IsNumeric(Lat) & IsNumeric(Lng))
            {
                if (Math.Abs(Convert.ToDouble(Lat)) >= 0 & Math.Abs(Convert.ToDouble(Lat)) < 180 & Math.Abs(Convert.ToDouble(Lng)) >= 0 & Math.Abs(Convert.ToDouble(Lng)) < 180)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 檢查格式 - 是否為數字
        /// </summary>
        /// <param name="Expression">輸入值</param>
        /// <returns>bool</returns>
        /// <see cref="http://support.microsoft.com/kb/329488/zh-tw"/>
        public static bool IsNumeric(this object Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;
            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;
            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        /// <summary>
        /// 檢查格式 - EMail
        /// </summary>
        /// <param name="inputValue">Email</param>
        /// <returns>bool</returns>
        public static bool IsEmail(this string inputValue)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(inputValue,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// 日期格式化
        /// </summary>
        /// <param name="inputValue">日期字串</param>
        /// <param name="formatValue">要輸出的格式</param>
        /// <returns>string</returns>
        public static string ToDateString(this string inputValue, string formatValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return "";
            }
            else
            {
                return String.Format("{0:" + formatValue + "}", Convert.ToDateTime(inputValue));
            }

        }

        /// <summary>
        /// 日期格式化 - ERP
        /// </summary>
        /// <param name="inputValue">日期字串</param>
        /// <param name="formatValue">日期間隔符號</param>
        /// <returns>string</returns>
        /// <example>原始日期:20101215</example>
        public static string ToDateString_ERP(this string inputValue, string formatValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return "";
            }
            else
            {
                return String.Format("{1}{0}{2}{0}{3}"
                    , formatValue
                    , inputValue.Substring(0, 4)
                    , inputValue.Substring(4, 2)
                    , inputValue.Substring(6, 2));
            }
        }

        /// <summary>
        /// 產生隨機英數字
        /// </summary>
        /// <param name="VcodeNum">顯示幾碼</param>
        /// <returns>string</returns>
        public static string RndNum(int VcodeNum)
        {
            string Vchar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了
            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数
            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);
                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }

        /// <summary>
        /// 取得IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip;
            string trueIP = string.Empty;
            HttpRequest req = HttpContext.Current.Request;

            //先取得是否有經過代理伺服器
            ip = req.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ip))
            {
                //將取得的 IP 字串存入陣列
                string[] ipRange = ip.Split(',');

                //比對陣列中的每個 IP
                for (int i = 0; i < ipRange.Length; i++)
                {
                    //剔除內部 IP 及不合法的 IP 後，取出第一個合法 IP
                    if (ipRange[i].Trim().Substring(0, 3) != "10." &&
                        ipRange[i].Trim().Substring(0, 7) != "192.168" &&
                        ipRange[i].Trim().Substring(0, 7) != "172.16." &&
                        CheckIP(ipRange[i].Trim()))
                    {
                        trueIP = ipRange[i].Trim();
                        break;
                    }
                }

            }
            else
            {
                //沒經過代理伺服器，直接使用 ServerVariables["REMOTE_ADDR"]
                //並經過 CheckIP( ) 的驗證
                trueIP = CheckIP(req.ServerVariables["REMOTE_ADDR"]) ?
                    req.ServerVariables["REMOTE_ADDR"] : "";
            }

            return trueIP;
        }

        /// <summary>
        /// 檢查 IP 是否合法
        /// </summary>
        /// <param name="strPattern">需檢測的 IP</param>
        /// <returns>true:合法 false:不合法</returns>
        private static bool CheckIP(string strPattern)
        {
            // 繼承自：System.Text.RegularExpressions
            // regular: ^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$
            Regex regex = new Regex("^\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}$");
            Match m = regex.Match(strPattern);

            return m.Success;
        }

        /// <summary>
        /// 建立Url
        /// </summary>
        /// <param name="Uri">網址</param>
        /// <param name="ParamName">參數名稱(Array)(String)</param>
        /// <param name="ParamVal">參數值(Array)(String)</param>
        /// <returns>string</returns>
        public static string CreateUrl(string Uri, Array ParamName, Array ParamVal)
        {
            //判斷網址是否為空
            if (string.IsNullOrEmpty(Uri))
            {
                return "";
            }

            //產生完整網址
            StringBuilder url = new StringBuilder();
            url.Append(Uri);

            //判斷陣列索引數是否相同
            if (ParamName.Length == ParamVal.Length)
            {
                for (int row = 0; row < ParamName.Length; row++)
                {
                    url.Append(string.Format("{0}{1}={2}"
                        , (row == 0) ? "?" : "&"
                        , ParamName.GetValue(row).ToString()
                        , HttpUtility.UrlEncode(ParamVal.GetValue(row).ToString())
                        ));
                }
            }

            return url.ToString();
        }

        /// <summary>
        /// 判斷字串內是否包含某字詞
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="strPool">要判斷的字詞</param>
        /// <param name="splitSymbol">Array的分割符號</param>
        /// <param name="splitNum">分割符號的數量</param>
        /// <returns></returns>
        /// <example>
        ///     string strTmp = ".jpg||.png||.pdf||.bmp";
        ///     Response.Write(fn_Extensions.CheckStrWord(src, strTmp, "|", 2));        
        /// </example>
        public static bool CheckStrWord(string inputValue, string strPool, string splitSymbol, int splitNum)
        {
            string[] strAry = Regex.Split(strPool, @"\" + splitSymbol + "{" + splitNum + "}");
            foreach (string item in strAry)
            {
                if (inputValue.IndexOf(item.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 產生GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            return System.Guid.NewGuid().ToString();
        }
        #endregion

        #region "字串驗証"

        //================================= 字串 =================================
        public enum InputType
        {
            英文,
            數字,
            小寫英文,
            小寫英文混數字,
            小寫英文開頭混數字,
            大寫英文,
            大寫英文混數字,
            大寫英文開頭混數字
        }

        /// <summary>
        /// 驗証 - 輸入類型(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="InputType">輸入類型</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_輸入限制(string value, InputType InputType, string minLength, string maxLength
            , out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //判斷輸入限制種類 - InputType
                switch (InputType)
                {
                    case InputType.數字:

                        return IsNumeric(value);

                    case InputType.英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                & System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                                & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文開頭混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (i == 0)
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                                    & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                                {
                                    return false;
                                }
                            }
                        }

                        break;

                    case InputType.大寫英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.大寫英文混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.大寫英文開頭混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (i == 0)
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                    & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                                {
                                    return false;
                                }
                            }
                        }

                        break;
                }

                //檢查字數是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (value.Length < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "字數小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查字數是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (value.Length > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "字數大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 輸入字數(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_字數(string value, string minLength, string maxLength, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查字數是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (value.Length < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "字數小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查字數是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (value.Length > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "字數大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 驗証 - 輸入字數(byte)(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_資料長度Byte(string value, string minLength, string maxLength, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                double valueByteLength = System.Text.Encoding.Default.GetBytes(value).Length;
                //檢查資料長度(Byte)是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (valueByteLength < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "資料長度(Byte)小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查資料長度(Byte)是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (valueByteLength > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "資料長度(Byte)大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        //================================ 日期時間 ==============================
        /// <summary>
        /// 驗証 - 日期
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minDate">最小日期</param>
        /// <param name="maxDate">最大日期</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool DateTime_日期(string value, string minDate, string maxDate, out string ErrMsg)
        {
            try
            {
                DateTime dtResult;
                ErrMsg = "";
                value = value.Trim();
                minDate = minDate.Trim();
                maxDate = maxDate.Trim();
                //檢查是不是時間
                if (DateTime.TryParse(value, out dtResult) == false | string.IsNullOrEmpty(value))
                {
                    ErrMsg = "不是日期資料";
                    return false;
                }
                //檢查是不是小於 minDate
                if (DateTime.TryParse(minDate, out dtResult) & !string.IsNullOrEmpty(minDate))
                {
                    if (Convert.ToDateTime(value) < Convert.ToDateTime(minDate))
                    {
                        ErrMsg = "小於 minDate：" + string.Format(minDate, "yyyy-MM-dd");
                        return false;
                    }
                }
                //檢查是不是小於 maxDate
                if (DateTime.TryParse(maxDate, out dtResult) & !string.IsNullOrEmpty(maxDate))
                {
                    if (Convert.ToDateTime(value) > Convert.ToDateTime(maxDate))
                    {
                        ErrMsg = "大於 maxDate：" + string.Format(maxDate, "yyyy-MM-dd");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 時間
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minDateTime">最小時間</param>
        /// <param name="maxDateTime">最大時間</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool DateTime_時間(string value, string minDateTime, string maxDateTime, out string ErrMsg)
        {
            try
            {
                DateTime dtResult;
                ErrMsg = "";
                value = value.Trim();
                minDateTime = minDateTime.Trim();
                maxDateTime = maxDateTime.Trim();
                //檢查是不是時間
                if (DateTime.TryParse(value, out dtResult) == false | string.IsNullOrEmpty(value))
                {
                    ErrMsg = "不是時間資料";
                    return false;
                }
                //檢查是不是小於 minDateTime
                if (DateTime.TryParse(minDateTime, out dtResult) & !string.IsNullOrEmpty(minDateTime))
                {
                    if (Convert.ToDateTime(value) < Convert.ToDateTime(minDateTime))
                    {
                        ErrMsg = "小於 minDateTime：" + string.Format(minDateTime, "yyyy-MM-dd HH:mm:ss.fff");
                        return false;
                    }
                }
                //檢查是不是小於 maxDateTime
                if (DateTime.TryParse(maxDateTime, out dtResult) & !string.IsNullOrEmpty(maxDateTime))
                {
                    if (Convert.ToDateTime(value) > Convert.ToDateTime(maxDateTime))
                    {
                        ErrMsg = "大於 maxDateTime：" + string.Format(maxDateTime, "yyyy-MM-dd HH:mm:ss.fff");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        //================================= 數值 =================================
        /// <summary>
        /// 驗証 - 數字(正整數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_正整數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) < 0)
                {
                    ErrMsg = "小於 0";
                    return false;
                }
                //檢查是不是整數
                if (Convert.ToDouble(value) != Math.Floor(Convert.ToDouble(value)))
                {
                    ErrMsg = "正數非正整數";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(負整數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_負整數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) > 0)
                {
                    ErrMsg = "大於 0";
                    return false;
                }
                //檢查是不是整數
                if (Convert.ToDouble(value) != Math.Floor(Convert.ToDouble(value)))
                {
                    ErrMsg = "負數非負整數";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(正數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_正數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) < 0)
                {
                    ErrMsg = "小於 0";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(負數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_負數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) > 0)
                {
                    ErrMsg = "大於 0";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }


        #endregion

        #region "常用功能"
        /// <summary>
        /// 使用HttpWebRequest取得網頁資料
        /// </summary>
        /// <param name="url">網址</param>
        /// <returns>string</returns>
        public static string WebRequest_GET(string url)
        {
            try
            {
                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                //IIS為AD驗証時加入此段 Start
                req.UseDefaultCredentials = true;
                req.PreAuthenticate = true;
                req.Credentials = CredentialCache.DefaultCredentials;
                //IIS為AD驗証時加入此段 End

                req.Method = "GET";
                using (WebResponse wr = req.GetResponse())
                {
                    using (StreamReader myStreamReader = new StreamReader(wr.GetResponseStream(), myEncoding))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 使用HttpWebRequest POST取得網頁資料
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="param">參數 (a=123&b=456)</param>
        /// <returns></returns>
        public static string WebRequest_POST(string url, string param, bool ADMode)
        {
            try
            {
                byte[] bs = Encoding.ASCII.GetBytes(param);

                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                //IIS為AD驗証時加入此段 Start
                if (ADMode)
                {
                    req.UseDefaultCredentials = true;
                    req.PreAuthenticate = true;
                    req.Credentials = CredentialCache.DefaultCredentials;
                }
                //IIS為AD驗証時加入此段 End

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = bs.Length;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }
                using (WebResponse wr = req.GetResponse())
                {
                    using (StreamReader myStreamReader = new StreamReader(wr.GetResponseStream(), myEncoding))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 使用FileStream取得資料
        /// </summary>
        /// <param name="path">磁碟路徑</param>
        /// <returns>string</returns>
        public static string IORequest_GET(string path)
        {
            try
            {
                if (false == System.IO.File.Exists(path)) return "";
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader sw = new StreamReader(fs, System.Text.Encoding.UTF8))
                    {
                        return sw.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region "IO"
        /// <summary>
        /// IO - 判斷目標資料夾是否存在
        /// </summary>
        /// <param name="folder">目標資料夾</param>
        /// <returns>bool</returns>
        public static bool CheckFolder(string folder)
        {
            try
            {
                DirectoryInfo CheckFolder = new DirectoryInfo(folder);
                if (CheckFolder.Exists == false)
                    CheckFolder.Create();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region "其他"
        /// <summary>
        /// jQuery - BlockUI
        /// </summary>
        /// <param name="validGroup">驗証控制項的群組名稱</param>
        /// <param name="inputTxt">輸入的Html</param>
        /// <returns>string</returns>
        public static string BlockJs(string validGroup, string inputTxt)
        {
            StringBuilder BlockJs = new StringBuilder();
            BlockJs.AppendLine("if(Page_ClientValidate_AllPass('" + validGroup + "')) {");
            BlockJs.AppendLine("$(function () {");
            BlockJs.AppendLine("    $.blockUI({");
            BlockJs.AppendLine("        message: '" + inputTxt + "',");
            BlockJs.AppendLine("        css: {");
            BlockJs.AppendLine("            width: '200px',");
            BlockJs.AppendLine("            border: 'none',");
            BlockJs.AppendLine("            padding: '5px',");
            BlockJs.AppendLine("            backgroundColor: '#000',");
            BlockJs.AppendLine("            '-webkit-border-radius': '10px',");
            BlockJs.AppendLine("            '-moz-border-radius': '10px',");
            BlockJs.AppendLine("            opacity: .8,");
            BlockJs.AppendLine("            color: '#fff'");
            BlockJs.AppendLine("        }");
            BlockJs.AppendLine("    });");
            BlockJs.AppendLine("});");
            BlockJs.AppendLine("}");
            return BlockJs.ToString();
        }

        /// <summary>
        /// Javascript - Alert與Redirect
        /// </summary>
        /// <param name="alertMessage">警示訊息</param>
        /// <param name="hrefUrl">導向頁面或JS語法</param>
        /// <remarks>
        /// 使用JS語法須加入 script: 以判斷
        /// </remarks>
        public static void JsAlert(string alertMessage, string hrefUrl)
        {
            try
            {
                StringBuilder sbJs = new StringBuilder();
                //警示訊息
                if (false == string.IsNullOrEmpty(alertMessage))
                {
                    sbJs.Append(string.Format("alert('{0}');", alertMessage));
                }
                //判斷是頁面還是語法
                if (false == string.IsNullOrEmpty(hrefUrl))
                {
                    if (hrefUrl.IndexOf("script:") != -1)
                    {
                        sbJs.Append(hrefUrl.Replace("script:", ""));
                    }
                    else
                    {
                        sbJs.Append(string.Format("location.href=\'{0}\';", hrefUrl));
                    }
                }
                ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", sbJs.ToString(), true);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// SQL參數組合 - Where IN
        /// </summary>
        /// <param name="listSrc">來源資料(List)</param>
        /// <param name="paramName">參數名稱</param>
        /// <returns>參數字串</returns>
        public static string GetSQLParam(List<string> listSrc, string paramName)
        {
            if (listSrc.Count == 0)
            {
                return "";
            }

            //組合參數字串
            ArrayList aryParam = new ArrayList();
            for (int row = 0; row < listSrc.Count; row++)
            {
                aryParam.Add(string.Format("@{0}{1}", paramName, row));
            }
            //回傳以 , 為分隔符號的字串
            return string.Join(",", aryParam.ToArray());
        }
        #endregion

        #region "產生選單"
        /// <summary>
        /// 選單 - 部門(預設)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Dept(DropDownListGP setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            return Menu_Dept(setMenu, inputValue, showRoot, false, out ErrMsg);
        }

        public static bool Menu_Dept(DropDownListGP setMenu, string inputValue, bool showRoot, bool isSales, out string ErrMsg)
        {
            return Menu_Dept(setMenu, inputValue, showRoot, isSales, null, out ErrMsg);
        }

        /// <summary>
        /// 選單 - 部門
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <param name="isSales">是否為業務單位</param>
        /// <param name="AreaCode">區域別</param>
        /// <returns></returns>
        public static bool Menu_Dept(DropDownListGP setMenu, string inputValue, bool showRoot, bool isSales
            , List<string> AreaCode, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 部門資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.Clear();

                    StringBuilder SBSql = new StringBuilder();

                    SBSql.AppendLine(" SELECT Shipping.SName, User_Dept.DeptID, User_Dept.DeptName ");
                    SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Shipping.SID ORDER BY Shipping.Sort, User_Dept.Sort ASC) AS GP_Rank");
                    SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                    SBSql.AppendLine("   INNER JOIN Shipping WITH (NOLOCK) ON User_Dept.Area = Shipping.SID ");
                    SBSql.AppendLine(" WHERE (User_Dept.Display = 'Y') ");
                    //判斷是否為業務單位(for 目標設定)(20200109取消)
                    //if (isSales)
                    //{
                    //    SBSql.Append(" AND (User_Dept.TargetDisplay = 'Y')");
                        
                    //}
                    //判斷是否有區域別條件
                    if (AreaCode != null && AreaCode.Count > 0)
                    {
                        SBSql.Append(" AND (User_Dept.Area IN ({0})) ".FormatThis(
                                fn_Extensions.GetSQLParam(AreaCode, "Area")
                            ));
                    }
                    SBSql.AppendLine(" ORDER BY Shipping.Sort, User_Dept.Sort ");
                    cmd.CommandText = SBSql.ToString();
                    //判斷是否有區域別條件
                    if (AreaCode != null && AreaCode.Count > 0)
                    {
                        for (int row = 0; row < AreaCode.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("Area{0}".FormatThis(row), AreaCode[row].ToString());
                        }
                    }

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //判斷GP_Rank, 若為第一項，則輸出群組名稱
                            if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                            {
                                setMenu.AddItemGroup(DT.Rows[row]["SName"].ToString());

                            }

                            setMenu.Items.Add(new ListItem(DT.Rows[row]["DeptName"].ToString()
                                         , DT.Rows[row]["DeptID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇部門 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }


        public static bool Menu_DeptForTarget(DropDownListGP setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 部門資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.Clear();

                    StringBuilder SBSql = new StringBuilder();

                    SBSql.AppendLine(" SELECT Shipping.SName, User_Dept.DeptID, User_Dept.DeptName ");
                    SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Shipping.SID ORDER BY Shipping.Sort, User_Dept.Sort ASC) AS GP_Rank");
                    SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                    SBSql.AppendLine("   INNER JOIN Shipping WITH (NOLOCK) ON User_Dept.Area = Shipping.SID ");
                    SBSql.AppendLine(" WHERE (User_Dept.TargetDisplay = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Shipping.Sort, User_Dept.Sort ");
                    cmd.CommandText = SBSql.ToString();
                   

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //判斷GP_Rank, 若為第一項，則輸出群組名稱
                            if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                            {
                                setMenu.AddItemGroup(DT.Rows[row]["SName"].ToString());

                            }

                            setMenu.Items.Add(new ListItem(DT.Rows[row]["DeptName"].ToString()
                                         , DT.Rows[row]["DeptID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇部門 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 人員(預設)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="filterDept">篩選-部門</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Employee(DropDownListGP setMenu, string inputValue, bool showRoot, string filterDept, out string ErrMsg)
        {
            return Menu_Employee(setMenu, inputValue, showRoot, filterDept, false, out ErrMsg);
        }

        /// <summary>
        /// 選單 - 人員
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="filterDept">篩選-部門</param>
        /// <param name="isSales">是否為業務單位</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Employee(DropDownListGP setMenu, string inputValue, bool showRoot, string filterDept, bool isSales, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 人員資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Staff.Account_Name, Staff.Display_Name, User_Dept.DeptID, User_Dept.DeptName ");
                    SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY User_Dept.DeptID ORDER BY User_Dept.Sort, Staff.ERP_UserID ASC) AS GP_Rank ");
                    SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                    SBSql.AppendLine("  INNER JOIN User_Profile Staff WITH (NOLOCK) ON User_Dept.DeptID = Staff.DeptID");
                    SBSql.AppendLine(" WHERE (User_Dept.Display = 'Y') AND (Staff.Display = 'Y') ");
                    //判斷部門
                    if (false == string.IsNullOrEmpty(filterDept))
                    {
                        SBSql.AppendLine(" AND (User_Dept.DeptID = @DeptID) ");

                        cmd.Parameters.AddWithValue("DeptID", filterDept);
                    }
                    //判斷是否為業務單位(20200109取消)
                    //if (isSales)
                    //{
                    //    SBSql.AppendLine(" AND (User_Dept.DeptID IN ('100', '120', '130', '200', '220', '311', '312', '313', '314')) ");
                    //}
                    SBSql.AppendLine(" ORDER BY User_Dept.DeptID, User_Dept.Sort, Staff.ERP_UserID ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //判斷GP_Rank, 若為第一項，則輸出群組名稱
                            if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                            {
                                setMenu.AddItemGroup(DT.Rows[row]["DeptName"].ToString());

                            }

                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Display_Name"].ToString()
                                         , DT.Rows[row]["Account_Name"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇人員 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - AD人員
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="filterDept">篩選-部門</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_ADUser(DropDownListGP setMenu, string inputValue, bool showRoot, string filterDept, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 人員資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Staff.Account_Name, Staff.Display_Name, User_Dept.DeptID, User_Dept.DeptName ");
                    SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY User_Dept.DeptID ORDER BY User_Dept.Sort, Staff.Account_Name ASC) AS GP_Rank ");
                    SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                    SBSql.AppendLine("  INNER JOIN User_Profile Staff WITH (NOLOCK) ON User_Dept.DeptID = Staff.DeptID");
                    SBSql.AppendLine(" WHERE (User_Dept.Display = 'Y') AND (Staff.Display = 'Y')");
                    //判斷部門
                    if (false == string.IsNullOrEmpty(filterDept))
                    {
                        SBSql.AppendLine(" AND (User_Dept.DeptID = @DeptID) ");

                        cmd.Parameters.AddWithValue("DeptID", filterDept);
                    }
                    SBSql.AppendLine(" ORDER BY User_Dept.Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //判斷GP_Rank, 若為第一項，則輸出群組名稱
                            if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                            {
                                setMenu.AddItemGroup(DT.Rows[row]["DeptName"].ToString());

                            }

                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Display_Name"].ToString()
                                         , DT.Rows[row]["Account_Name"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇人員 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - TTD類別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="filterDept">篩選-部門</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_TTDClass(DropDownList setMenu, string inputValue, bool showRoot, string filterDept, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - TTD類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM TTD_Class WITH (NOLOCK) WHERE (Display = 'Y') AND (DeptID = @DeptID) ");
                    SBSql.AppendLine(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("DeptID", filterDept);
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇類別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - IT 類別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="filterType">篩選-類型</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_ITClass(DropDownList setMenu, string inputValue, bool showRoot, List<string> IType, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - IT類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM IT_ParamClass WITH (NOLOCK) WHERE (Display = 'Y') ");
                    //判斷是否有類型條件
                    if (IType != null && IType.Count > 0)
                    {
                        SBSql.Append(" AND (Class_Type IN ({0})) ".FormatThis(
                                fn_Extensions.GetSQLParam(IType, "ClsType")
                            ));

                        for (int row = 0; row < IType.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("ClsType{0}".FormatThis(row), IType[row].ToString());
                        }
                    }
                    SBSql.AppendLine(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇類別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        public static bool Menu_ITClass(RadioButtonList setMenu, string inputValue, List<string> IType, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - IT類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name, (Class_Type + '-' + Class_Name) AS Label ");
                    SBSql.AppendLine(" FROM IT_ParamClass WITH (NOLOCK) WHERE (Display = 'Y') ");
                    //判斷是否有類型條件
                    if (IType != null && IType.Count > 0)
                    {
                        SBSql.Append(" AND (Class_Type IN ({0})) ".FormatThis(
                                fn_Extensions.GetSQLParam(IType, "ClsType")
                            ));

                        for (int row = 0; row < IType.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("ClsType{0}".FormatThis(row), IType[row].ToString());
                        }
                    }
                    SBSql.AppendLine(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }


        /// <summary>
        /// 選單 - OP 類別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="filterType">篩選-類型</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_OPClass(DropDownList setMenu, string inputValue, bool showRoot, List<string> IType, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - IT類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM OP_ParamClass WITH (NOLOCK) WHERE (Display = 'Y') ");
                    //判斷是否有類型條件
                    if (IType != null && IType.Count > 0)
                    {
                        SBSql.Append(" AND (Class_Type IN ({0})) ".FormatThis(
                                fn_Extensions.GetSQLParam(IType, "ClsType")
                            ));

                        for (int row = 0; row < IType.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("ClsType{0}".FormatThis(row), IType[row].ToString());
                        }
                    }
                    SBSql.AppendLine(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇類別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        public static bool Menu_OPClass(RadioButtonList setMenu, string inputValue, List<string> IType, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - IT類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name, (Class_Type + '-' + Class_Name) AS Label ");
                    SBSql.AppendLine(" FROM OP_ParamClass WITH (NOLOCK) WHERE (Display = 'Y') ");
                    //判斷是否有類型條件
                    if (IType != null && IType.Count > 0)
                    {
                        SBSql.Append(" AND (Class_Type IN ({0})) ".FormatThis(
                                fn_Extensions.GetSQLParam(IType, "ClsType")
                            ));

                        for (int row = 0; row < IType.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("ClsType{0}".FormatThis(row), IType[row].ToString());
                        }
                    }
                    SBSql.AppendLine(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }


        /// <summary>
        /// 選單 - 出貨地
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_ShipFrom(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 出貨地資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT SID, SName ");
                    SBSql.AppendLine(" FROM Shipping WITH (NOLOCK) WHERE (Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["SName"].ToString()
                                         , DT.Rows[row]["SID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇區域 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 通路別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Channel(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 通路別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT CID, CName ");
                    SBSql.AppendLine(" FROM Channel WITH (NOLOCK) WHERE (Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY CID ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["CName"].ToString()
                                         , DT.Rows[row]["CID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇通路別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 年份
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="addNum">增量值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Year(DropDownList setMenu, string inputValue, bool showRoot, int addNum, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                int bgYear = DateTime.Now.AddYears(-1).Year;
                int edYear = DateTime.Now.AddYears(addNum).Year;
                for (int intY = bgYear; intY <= edYear; intY++)
                {
                    setMenu.Items.Add(new ListItem(intY.ToString(), intY.ToString()));
                }

                //判斷是否有已選取的項目
                if (false == string.IsNullOrEmpty(inputValue))
                {
                    setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                }
                //判斷是否要顯示索引文字
                if (showRoot)
                {
                    setMenu.Items.Insert(0, new ListItem("-- 選擇年 --", ""));
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 月份
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Month(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                for (int intM = 1; intM <= 12; intM++)
                {
                    setMenu.Items.Add(new ListItem(intM.ToString(), intM.ToString()));
                }

                //判斷是否有已選取的項目
                if (false == string.IsNullOrEmpty(inputValue))
                {
                    setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                }
                //判斷是否要顯示索引文字
                if (showRoot)
                {
                    setMenu.Items.Insert(0, new ListItem("-- 選擇月 --", ""));
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 問卷狀態
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_QuesStatus(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 問卷狀態
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Status_ID, Status_Name ");
                    SBSql.AppendLine(" FROM Questionnaire_Status WITH (NOLOCK) ");
                    SBSql.AppendLine(" ORDER BY Status_ID ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Status_Name"].ToString()
                                         , DT.Rows[row]["Status_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇狀態 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 公司別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="showOutside">是否只顯示境外</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Corp(RadioButtonList setMenu, string inputValue, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (DataTable DT = Data_Corp())
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        setMenu.Items.Add(new ListItem(DT.Rows[row]["Corp_Name"].ToString()
                                     , DT.Rows[row]["Corp_ID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (false == string.IsNullOrEmpty(inputValue))
                    {
                        setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        public static bool Menu_Corp(CheckBoxList setMenu, string[] inputValues, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (DataTable DT = Data_Corp())
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        setMenu.Items.Add(new ListItem(DT.Rows[row]["Corp_Name"].ToString() + "&nbsp;&nbsp;&nbsp;"
                                     , DT.Rows[row]["Corp_ID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (inputValues != null)
                    {
                        foreach (string item in inputValues)
                        {
                            for (int col = 0; col < setMenu.Items.Count; col++)
                            {
                                if (setMenu.Items[col].Value.Equals(item.ToString()))
                                {
                                    setMenu.Items[col].Selected = true;
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        public static bool Menu_Corp(DropDownList setMenu, string inputValue, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (DataTable DT = Data_Corp())
                {
                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        setMenu.Items.Add(new ListItem(DT.Rows[row]["Corp_Name"].ToString()
                                     , DT.Rows[row]["Corp_ID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (false == string.IsNullOrEmpty(inputValue))
                    {
                        setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 公司別選單資料
        /// </summary>
        /// <returns></returns>
        private static DataTable Data_Corp()
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT RTRIM(Corp_ID) AS Corp_ID, Corp_Name ");
                SBSql.AppendLine(" FROM Param_Corp WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort ");
                cmd.CommandText = SBSql.ToString();

                return dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg);
            }
        }

        /// <summary>
        /// 選單 - 物流資料
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_OrderClass(DropDownList setMenu, string inputValue, bool showRoot, string inputType, string inputArea
            , out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID AS id, Class_Name AS label");
                    SBSql.AppendLine(" FROM Order_ParamClass WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Class_Type = @Class_Type) AND (Display = 'Y') AND (AreaCode = @AreaCode)");
                    SBSql.AppendLine(" ORDER BY Sort, Class_ID");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Class_Type", inputType);
                    cmd.Parameters.AddWithValue("AreaCode", string.IsNullOrEmpty(inputArea) ? "TW" : inputArea);

                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["label"].ToString()
                                         , DT.Rows[row]["id"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇項目 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得資料庫名稱
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 1=台灣寶工 / 2=深圳寶工
        /// 3=上海寶工
        /// 4=LEONARD
        /// 5=SHINYPOWER
        /// 6=SUNGLOW
        /// </remarks>
        public static string Get_DBName(string uid, out string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT DB_Name ");
                    SBSql.AppendLine(" FROM Param_Corp WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Corp_UID = @Corp_UID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Corp_UID", uid);
                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return "";
                        }

                        return DT.Rows[0]["DB_Name"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return "";
            }
        }
        #endregion

        #region -- 官網資料 --

        /// <summary>
        /// 選單 - 訊息類別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_InqClass(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM Inquiry_Class WITH (NOLOCK) WHERE (Display = 'Y') AND (LangCode = 'zh-TW') ");
                    SBSql.AppendLine(" ORDER BY Sort, Class_ID");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇類別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 訊息狀態
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_InqStatus(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 狀態資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM Inquiry_Status WITH (NOLOCK) ");
                    SBSql.AppendLine(" ORDER BY Sort, Class_ID");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇狀態 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 官網區域
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_Area(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT AreaCode AS id, AreaName AS label");
                    SBSql.AppendLine(" FROM Param_Area WITH (NOLOCK) WHERE (LangCode = 'zh-tw') AND (Display = 'Y')");
                    SBSql.AppendLine(" ORDER BY Sort");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["label"].ToString()
                                         , DT.Rows[row]["id"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 所有資料 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - ERP地區別(3) / 國家別(4)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="inputType">類別</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_ERP_Param(DropDownList setMenu, string inputValue, string inputType, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT RTRIM(MR002) AS id, RTRIM(MR003) AS label");
                    SBSql.AppendLine(" FROM [prokit2].dbo.CMSMR WITH (NOLOCK) WHERE (MR001 = @type)");
                    SBSql.AppendLine(" ORDER BY MR003");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("type", inputType);

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["label"].ToString()
                                         , DT.Rows[row]["id"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 所有資料 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 出貨庫別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_SWID(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT SWID AS id, (SW_Name_zh_TW + ' (' + StockType + ')') AS label");
                    SBSql.AppendLine(" FROM ShippingWarehouse WITH (NOLOCK)");
                    SBSql.AppendLine(" ORDER BY Sort");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["label"].ToString()
                                         , DT.Rows[row]["id"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇庫別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }


        #endregion


        #region -- 玩具網站資料 --


        /// <summary>
        /// 選單 - 訊息類別
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_ToyInqClass(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 類別資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM Inquiry_Class WITH (NOLOCK) WHERE (Display = 'Y') AND (LangCode = 'zh-TW') ");
                    SBSql.AppendLine(" ORDER BY Sort, Class_ID");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Science, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇類別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 選單 - 訊息狀態
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Menu_ToyInqStatus(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 狀態資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Class_ID, Class_Name ");
                    SBSql.AppendLine(" FROM Inquiry_Status WITH (NOLOCK) ");
                    SBSql.AppendLine(" ORDER BY Sort, Class_ID");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Science, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Class_Name"].ToString()
                                         , DT.Rows[row]["Class_ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇狀態 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        #endregion


        #region -- 下載 --

        /// <summary>
        /// 下載分類選單 (Radio)
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Menu_FileClass(RadioButtonList setMenu, string inputValue, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.Append(" SELECT Class_ID AS ID, Class_Name AS Label");
                    SBSql.Append(" FROM File_Class WITH (NOLOCK) WHERE (Display = 'Y') AND (LOWER(LangCode) = 'zh-tw') AND (ClassType = 1) ");
                    SBSql.Append(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }


                        //加入bootstrap btn css
                        setMenu.CssClass = "btn-group btn-group-justified";
                        setMenu.Attributes.Add("data-toggle", "buttons");

                        for (int row = 0; row < setMenu.Items.Count; row++)
                        {
                            setMenu.Items[row].Attributes.Add("class", "btn btn-danger");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 下載語言分類選單 (Radio)
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Menu_FileLangType(RadioButtonList setMenu, string inputValue, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.Append(" SELECT Class_ID AS ID, Class_Name AS Label");
                    SBSql.Append(" FROM File_LangType WITH (NOLOCK) WHERE (Display = 'Y') ");
                    SBSql.Append(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }


                        //加入bootstrap btn css
                        setMenu.CssClass = "btn-group btn-group-justified";
                        setMenu.Attributes.Add("data-toggle", "buttons");

                        for (int row = 0; row < setMenu.Items.Count; row++)
                        {
                            setMenu.Items[row].Attributes.Add("class", "btn btn-warning");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 下載對象選單 (Radio)
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Menu_FileTarget(RadioButtonList setMenu, string inputValue, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.Append(" SELECT Class_ID AS ID, Class_Name AS Label");
                    SBSql.Append(" FROM File_Target WITH (NOLOCK) WHERE (Display = 'Y') ");
                    SBSql.Append(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }

                        //加入bootstrap btn css
                        setMenu.CssClass = "btn-group";
                        setMenu.Attributes.Add("data-toggle", "buttons");

                        for (int row = 0; row < setMenu.Items.Count; row++)
                        {
                            setMenu.Items[row].Attributes.Add("class", "btn btn-default");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 下載對象選單 (Dropdown)
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Menu_FileTarget(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.Append(" SELECT Class_ID AS ID, Class_Name AS Label");
                    SBSql.Append(" FROM File_Target WITH (NOLOCK) WHERE (Display = 'Y') ");
                    SBSql.Append(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }

                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇對象 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 檔案分類選單
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Menu_FileType(DropDownList setMenu, string inputValue, bool showRoot, string upClass, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.Append(" SELECT Class_ID AS ID, Class_Name AS Label");
                    SBSql.Append(" FROM File_Type WITH (NOLOCK) WHERE (Display = 'Y') AND (LOWER(LangCode) = 'zh-tw') AND (Up_ClassID = @Up_ClassID)");
                    SBSql.Append(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Up_ClassID", upClass);
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }

                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇檔案分類 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 客戶群組選單
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Menu_CustGroup(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.Append(" SELECT Group_ID AS ID, Group_Name AS Label");
                    SBSql.Append(" FROM File_CustGroup WITH (NOLOCK) WHERE (Display = 'Y')");
                    SBSql.Append(" ORDER BY Sort ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }

                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇群組 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        #endregion

        #region -- 權限 --
        /// <summary>
        /// 取得區域別權限
        /// </summary>
        /// <param name="IDs">程式代號</param>
        /// <param name="LoginGuid">AD Guid</param>
        /// <returns>區域別代碼</returns>
        /// <remarks>
        /// 若未設定區域別權限, 則自動帶所屬部門的區域別
        /// </remarks>
        public static List<string> GetAreaCode(string IDs, string LoginGuid, out string ErrMsg)
        {
            try
            {
                //判斷空值
                if (string.IsNullOrEmpty(IDs) || string.IsNullOrEmpty(LoginGuid))
                {
                    ErrMsg = "參數傳遞不正確";
                    return null;
                }

                //折解傳入的ID
                string[] strAry = System.Text.RegularExpressions.Regex.Split(IDs, @"\#{1}");
                //暫存值
                List<string> myVal = new List<string>();
                /*
                 * 查詢是否有關聯權限, 並回傳相關區域別
                 * 若無個人權限, 則依部門所屬區域
                 */
                using (SqlCommand cmd = new SqlCommand())
                {
                    //[清除參數]
                    cmd.Parameters.Clear();
                    //[SQL] - 資料查詢
                    string strSQL = @"
                    IF (  
                        SELECT COUNT(Program.AreaCode)
                        FROM User_Profile_Rel_Program Rel
                            INNER JOIN Program ON Rel.Prog_ID = Program.Prog_ID
                        WHERE (Guid = @Login_Guid)
                        AND (Rel.Prog_ID IN ({0}))
                    ) > 0
                     BEGIN
                        SELECT Program.AreaCode AS AreaCode
                        FROM User_Profile_Rel_Program Rel
                            INNER JOIN Program ON Rel.Prog_ID = Program.Prog_ID
                        WHERE (Guid = @Login_Guid)
                        AND (Rel.Prog_ID IN ({0}))
                     END
                    ELSE
                     BEGIN
                        SELECT Dept.Area AS AreaCode
                        FROM PKSYS.dbo.User_Profile Prof
                            INNER JOIN PKSYS.dbo.User_Dept Dept ON Prof.DeptID = Dept.DeptID
                        WHERE (Prof.Guid = @Login_Guid)
                     END".FormatThis(
                               fn_Extensions.GetSQLParam(strAry.ToList(), "ProgID")
                            );

                    cmd.CommandText = strSQL;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Login_Guid", LoginGuid);
                    //傳入多筆參數
                    int col = 0;
                    foreach (string item in strAry)
                    {
                        cmd.Parameters.AddWithValue("ProgID{0}".FormatThis(col), item.ToString());

                        col++;
                    }
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return null;
                        }

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            myVal.Add(DT.Rows[row]["AreaCode"].ToString());
                        }
                    }
                }

                return myVal;
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

    }

    public static class IOManage
    {
        //副檔名
        private static string _FileExtend;
        public static string FileExtend
        {
            get;
            private set;
        }
        //檔案名稱 - 原始檔名
        private static string _FileFullName;
        public static string FileFullName
        {
            get;
            private set;
        }
        //檔案名稱 - 系統命名
        private static string _FileNewName;
        public static string FileNewName
        {
            get;
            private set;
        }
        //檔案真實路徑
        private static string _FileRealName;
        private static string FileRealName
        {
            get;
            set;
        }
        //處理訊息
        private static string _Message;
        public static string Message
        {
            get;
            private set;
        }

        private static int idx = 0;

        /// <summary>
        /// 取得相關檔案名稱
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        public static void GetFileName(HttpPostedFile hpFile)
        {
            try
            {
                if (hpFile.ContentLength != 0)
                {
                    //[IO] - 檔案真實路徑
                    FileRealName = hpFile.FileName;
                    //[IO] - 取得副檔名(.xxx)
                    FileExtend = Path.GetExtension(FileRealName);
                    //[IO] - 檔案重新命名
                    idx += 1;
                    FileNewName = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + Convert.ToString(idx) + FileExtend;
                    //[IO] - 取得完整檔名
                    FileFullName = Path.GetFileName(FileRealName);

                    Message = "OK";
                }
                else
                {
                    FileExtend = null;
                    FileFullName = null;
                    FileNewName = null;
                    FileRealName = null;
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - GetFileName";
            }
        }

        /// <summary>
        /// 儲存檔案
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                {
                    //判斷資料夾是否存在
                    if (fn_Extensions.CheckFolder(FileFolder))
                    {
                        hpFile.SaveAs(FileFolder + newFileName);
                        Message = "OK";
                    }
                    else
                    {
                        Message = "資料夾無法建立，檔案上傳失敗。";
                    }
                }
                else
                {
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - Save";
            }

        }

        /// <summary>
        /// 儲存檔案, 使用縮圖
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        /// <param name="intWidth">指定寬度</param>
        /// <param name="intHeight">指定高度</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName, int intWidth, int intHeight)
        {
            try
            {
                if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                {
                    string fileUrl = FileFolder + newFileName;

                    //判斷資料夾是否存在
                    if (fn_Extensions.CheckFolder(FileFolder))
                    {
                        //儲存原始圖檔
                        hpFile.SaveAs(fileUrl);
                        //產生縮圖並覆蓋原始圖檔
                        renderThumb(fileUrl, fileUrl, intWidth, intHeight);

                        Message = "OK";
                    }
                    else
                    {
                        Message = "資料夾無法建立，檔案上傳失敗。";
                    }
                }
                else
                {
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - Save";
            }

        }

        /// <summary>
        /// 產生並儲存縮圖
        /// </summary>
        /// <param name="inputImg">來源圖檔路徑(磁碟路徑)</param>
        /// <param name="outputImg">輸出圖檔路徑(磁碟路徑)</param>
        /// <param name="w">寬</param>
        /// <param name="h">高</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static void renderThumb(string inputImg, string outputImg, int w, int h)
        {
            int width = 0;
            int height = 0;

            System.Drawing.Image image = new System.Drawing.Bitmap(inputImg);

            //取得圖檔寬高
            width = image.Width;
            height = image.Height;

            //重新設定寬高 (等比例)
            if (!(width < w & height < h))
            {
                if (width > height)
                {
                    h = w * height / width;
                }
                else
                {
                    w = h * width / height;
                }
            }
            else
            {
                h = height;
                w = width;
            }

            //產生縮圖
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(w, h);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(img);
            //將品質設定為HighQuality
            graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //重畫縮圖
            graphic.DrawImage(image, 0, 0, w, h);

            image.Dispose();

            //取得附檔名
            string myExt = Path.GetExtension(outputImg);
            switch (myExt.ToLower())
            {
                case "jpg":
                case "jpeg":
                    //輸出縮圖, 格式為Jpeg
                    img.Save(outputImg, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;

                case "gif":
                    //輸出縮圖, 格式為gif
                    img.Save(outputImg, System.Drawing.Imaging.ImageFormat.Gif);
                    break;

                case "bmp":
                    //輸出縮圖, 格式為bmp
                    img.Save(outputImg, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;

                default:
                    //輸出縮圖, 格式為Png
                    img.Save(outputImg, System.Drawing.Imaging.ImageFormat.Png);
                    break;
            }

            //釋放資源
            img.Dispose();
            graphic.Dispose();

        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="oldFileName">檔案名稱</param>
        public static void DelFile(string FileFolder, string oldFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(FileFolder) || string.IsNullOrEmpty(oldFileName))
                {
                    Message = "傳入參數空白";
                    return;
                }
                FileInfo FileDelete = new FileInfo(FileFolder + oldFileName);
                if (FileDelete.Exists)
                    FileDelete.Delete();

                Message = "OK";
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - DelFile";
            }
        }

        /// <summary>
        /// 刪除資料夾
        /// </summary>
        /// <param name="FileFolder">資料夾名稱</param>
        public static void DelFolder(string FileFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(FileFolder))
                {
                    Message = "傳入參數空白";
                    return;
                }

                string[] strTemp = null;
                int idx = 0;
                // 刪除檔案
                strTemp = Directory.GetFiles(FileFolder);
                for (idx = 0; idx < strTemp.Length; idx++)
                {
                    if (File.Exists(strTemp[idx]))
                        File.Delete(strTemp[idx]);
                }
                // 刪除子目錄
                strTemp = Directory.GetDirectories(FileFolder);
                for (idx = 0; idx < strTemp.Length; idx++)
                {
                    //呼叫 DelFolder
                    DelFolder(strTemp[idx]);
                }
                // 刪除該目錄
                System.IO.Directory.Delete(FileFolder);

                Message = "OK";
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - DelFolder";
            }
        }
    }

}