using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections;
using ExtensionMethods;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace MailMethods
{
    /// <summary>
    /// Email送信功能
    /// </summary>
    /// <remarks>
    /// 1. 僅適用一般寄信
    /// 2. 若電子報要使用，需另外修改
    ///    - 大量發送問題 
    ///    - 批次發送，每封收件對象只能有一人，避免其他email外流
    /// </remarks>
    public class fn_Mail
    {
        #region 參數設定
        /// <summary>
        /// 寄件人
        /// </summary>
        private static string _Sender;
        public static string Sender
        {
            get;
            set;
        }
        /// <summary>
        /// 寄件人顯示名稱
        /// </summary>
        private static string _SenderName;
        public static string SenderName
        {
            get;
            set;
        }
        /// <summary>
        /// 收件人群組
        /// </summary>
        private static List<string> _Reciever;
        public static List<string> Reciever
        {
            get;
            set;
        }
        /// <summary>
        /// 轉寄人群組
        /// </summary>
        private static List<string> _CC;
        public static List<string> CC
        {
            get;
            set;
        }
        /// <summary>
        /// 密件轉寄人群組
        /// </summary>
        private static List<string> _BCC;
        public static List<string> BCC
        {
            get;
            set;
        }

        /// <summary>
        /// 主旨
        /// </summary>
        private static string _Subject;
        public static string Subject
        {
            get;
            set;
        }
        /// <summary>
        /// 郵件內容
        /// </summary>
        private static StringBuilder _MailBody;
        public static StringBuilder MailBody
        {
            get;
            set;
        }
        /// <summary>
        /// 指定檔案 - 路徑
        /// </summary>
        private static string _FilePath;
        public static string FilePath
        {
            get;
            set;
        }
        /// <summary>
        /// 指定檔案 - 檔名
        /// </summary>
        private static string _FileName;
        public static string FileName
        {
            get;
            set;
        }
        /// <summary>
        /// 建立者
        /// </summary>
        private static string _Create_Who;
        public static string Create_Who
        {
            get;
            set;
        }
        /// <summary>
        /// 來源程式/功能
        /// </summary>
        private static string _FromFunc;
        public static string FromFunc
        {
            get;
            set;
        }
        /// <summary>
        /// 處理訊息
        /// </summary>
        private static string _Message;
        public static string Message
        {
            get;
            private set;
        }
        /// <summary>
        /// 處理訊息代碼
        /// </summary>
        private static int _MessageCode;
        public static int MessageCode
        {
            get;
            private set;
        }

        enum MCode
        {
            Success = 200, Fail = 999
        }

        #endregion

        /// <summary>
        /// 發送EMail
        /// </summary>
        public static void SendMail()
        {
            try
            {
                using (MailMessage Msg = new MailMessage())
                {
                    //[判斷&取得參數] - 建立者
                    if (string.IsNullOrEmpty(Create_Who) || Create_Who.Length > 50)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "建立者已失去連線或不存在,或字數超出限制";
                        return;
                    }

                    //[判斷&取得參數] - 來源程式/功能
                    if (string.IsNullOrEmpty(FromFunc) || FromFunc.Length > 50)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "來源程式/功能空白,或字數超出限制";
                        return;
                    }

                    #region ----- 郵件屬性 -----
                    //[判斷&取得參數] - 寄件人
                    if (string.IsNullOrEmpty(Sender) && Sender.IsEmail() == false)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "寄件人空白或格式錯誤";
                        return;
                    }
                    //[加入Mail屬性] - From
                    Msg.From = new MailAddress(Sender, SenderName);

                    //[判斷&取得參數] - 收件人
                    List<string> Get_receiver = Check_EmailAddress(Reciever);
                    if (Get_receiver == null || Get_receiver.Count == 0)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "收件人空白或格式錯誤";
                        return;
                    }
                    foreach (var item in Get_receiver)
                    {
                        //[加入Mail屬性] - To
                        Msg.To.Add(new MailAddress(item));
                    }

                    //[判斷&取得參數] - 轉寄
                    List<string> Get_cc = Check_EmailAddress(CC);
                    if (Get_cc != null && Get_cc.Count != 0)
                    {
                        foreach (var item in Get_cc)
                        {
                            //[加入Mail屬性] - CC
                            Msg.CC.Add(new MailAddress(item));
                        }
                    }

                    //[判斷&取得參數] - 密件轉寄
                    List<string> Get_Bcc = Check_EmailAddress(BCC);
                    if (Get_Bcc != null && Get_Bcc.Count != 0)
                    {
                        foreach (var item in Get_Bcc)
                        {
                            //[加入Mail屬性] - Bcc
                            Msg.Bcc.Add(new MailAddress(item));
                        }
                    }

                    #endregion

                    #region ----- 郵件內容 -----
                    //[加入Mail屬性] - Subject
                    Msg.Subject = Subject;
                    //[加入Mail屬性] - IsBodyHtml
                    Msg.IsBodyHtml = true;
                    //[加入Mail屬性] - Body
                    Msg.Body = MailBody.ToString();

                    //[加入Mail屬性]  - 插入指定圖片
                    if (MailBody.ToString().IndexOf("#Top_Logo#") != -1)
                    {
                        string ImgLogo = MailBody.ToString().Replace("#Top_Logo#"
                            , "<a href=\"{0}\"><img align=\"bottom\" src=\"cid:image_Logo\" width=\"220\" border=\"0\"></a>".FormatThis(
                                "http://www.prokits.com.tw/"
                            )); //將Body中的#Top_Logo# 取代成要顯示的圖片
                        AlternateView av = AlternateView.CreateAlternateViewFromString(ImgLogo, null, MediaTypeNames.Text.Html);
                        // -圖片來源
                        string ImgLogo_Src = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"images\PKlogo_email.gif";
                        LinkedResource lr = new LinkedResource(ImgLogo_Src, MediaTypeNames.Image.Gif);
                        lr.ContentId = "image_Logo"; //cid
                        av.LinkedResources.Add(lr);
                        Msg.AlternateViews.Add(av);
                    }

                    //[加入Mail屬性] - 指定上傳的檔案
                    if (!string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(FileName))
                    {
                        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                        {
                            using (Attachment att = new Attachment(fs, FileName, MediaTypeNames.Application.Octet))
                            {
                                Msg.Attachments.Add(att);
                            }
                        }
                    }
                    #endregion

                    //宣告SmtpClient (注意webconfig是否已設定..如下)
                    /*
                      <system.net>
                        <mailSettings>
                          <smtp deliveryMethod="Network">
                            <network defaultCredentials="false" host="smtp.prokits.com.tw" port="25" userName="pkmailman" password="PK!@#mail" />
                          </smtp>
                        </mailSettings>
                      </system.net>
                     */
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(Msg);
                    smtp.Dispose();

                    MessageCode = (int)MCode.Success;
                    Message = "寄送成功";

                    //寫入Log (Get_receiver - 篩選過的收件人)
                    EmailLog(Sender, Get_receiver, Get_cc, Get_Bcc
                        , FromFunc, DateTime.Now, Convert.ToString(MessageCode), Message, Create_Who);
                }
            }
            catch (Exception ex)
            {
                MessageCode = (int)MCode.Fail;
                Message = fn_stringFormat.Filter_Html(ex.Message.ToString());
                if (Message.IndexOf("無法使用信箱") != -1)
                    Message = Message + ",....格式正確的郵件可能已成功寄出。";

                //寫入Log (未篩選過的收件人)
                EmailLog(Sender, Reciever, CC, BCC
                      , FromFunc, DateTime.Now, Convert.ToString(MessageCode), Message, Create_Who);
            }
        }

        /// <summary>
        /// 發送EMail (可自行上傳一個檔案)
        /// </summary>
        public static void SendMail_WithFile(System.Web.UI.WebControls.FileUpload _files)
        {
            try
            {
                using (MailMessage Msg = new MailMessage())
                {
                    //[判斷&取得參數] - 建立者
                    if (string.IsNullOrEmpty(Create_Who) || Create_Who.Length > 50)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "建立者已失去連線或不存在,或字數超出限制";
                        return;
                    }

                    //[判斷&取得參數] - 來源程式/功能
                    if (string.IsNullOrEmpty(FromFunc) || FromFunc.Length > 50)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "來源程式/功能空白,或字數超出限制";
                        return;
                    }

                    #region ----- 郵件屬性 -----
                    //[判斷&取得參數] - 寄件人
                    if (string.IsNullOrEmpty(Sender) && Sender.IsEmail() == false)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "寄件人空白或格式錯誤";
                        return;
                    }
                    //[加入Mail屬性] - From
                    Msg.From = new MailAddress(Sender, SenderName);

                    //[判斷&取得參數] - 收件人
                    List<string> Get_receiver = Check_EmailAddress(Reciever);
                    if (Get_receiver == null || Get_receiver.Count == 0)
                    {
                        MessageCode = (int)MCode.Fail;
                        Message = "收件人空白或格式錯誤";
                        return;
                    }
                    foreach (var item in Get_receiver)
                    {
                        //[加入Mail屬性] - To
                        Msg.To.Add(new MailAddress(item));
                    }

                    //[判斷&取得參數] - 轉寄
                    List<string> Get_cc = Check_EmailAddress(CC);
                    if (Get_cc != null && Get_cc.Count != 0)
                    {
                        foreach (var item in Get_cc)
                        {
                            //[加入Mail屬性] - CC
                            Msg.CC.Add(new MailAddress(item));
                        }
                    }

                    //[判斷&取得參數] - 密件轉寄
                    List<string> Get_Bcc = Check_EmailAddress(BCC);
                    if (Get_Bcc != null && Get_Bcc.Count != 0)
                    {
                        foreach (var item in Get_Bcc)
                        {
                            //[加入Mail屬性] - Bcc
                            Msg.Bcc.Add(new MailAddress(item));
                        }
                    }

                    #endregion

                    #region ----- 郵件內容 -----
                    //[加入Mail屬性] - Subject
                    Msg.Subject = Subject;
                    //[加入Mail屬性] - IsBodyHtml
                    Msg.IsBodyHtml = true;
                    //[加入Mail屬性] - Body
                    Msg.Body = MailBody.ToString();

                    //[加入Mail屬性]  - 插入指定圖片
                    if (MailBody.ToString().IndexOf("#Top_Logo#") != -1)
                    {
                        string ImgLogo = MailBody.ToString().Replace("#Top_Logo#", "<img align=\"bottom\" src=\"cid:image_Logo\" width=\"220\">"); //將Body中的#Top_Logo# 取代成要顯示的圖片
                        AlternateView av = AlternateView.CreateAlternateViewFromString(ImgLogo, null, MediaTypeNames.Text.Html);
                        // -圖片來源
                        string ImgLogo_Src = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"images\PKlogo_email.gif";
                        LinkedResource lr = new LinkedResource(ImgLogo_Src, MediaTypeNames.Image.Gif);
                        lr.ContentId = "image_Logo"; //cid
                        av.LinkedResources.Add(lr);
                        Msg.AlternateViews.Add(av);
                    }

                    //[加入Mail屬性] - 指定上傳的檔案
                    if (!string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(FileName))
                    {
                        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                        {
                            using (Attachment att = new Attachment(fs, FileName, MediaTypeNames.Application.Octet))
                            {
                                Msg.Attachments.Add(att);
                            }
                        }
                    }

                    //[加入Mail屬性] - 判斷是否有附檔 (使用者上傳)
                    if (_files != null && _files.HasFile)
                    {
                        Msg.Attachments.Add(new Attachment(_files.PostedFile.InputStream, _files.FileName));
                    }
                    #endregion

                    //宣告SmtpClient
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(Msg);
                    smtp.Dispose();

                    MessageCode = (int)MCode.Success;
                    Message = "寄送成功";

                    //寫入Log (Get_receiver - 篩選過的收件人)
                    EmailLog(Sender, Get_receiver, Get_cc, Get_Bcc
                        , FromFunc, DateTime.Now, Convert.ToString(MessageCode), Message, Create_Who);
                }
            }
            catch (Exception ex)
            {
                MessageCode = (int)MCode.Fail;
                Message = fn_stringFormat.Filter_Html(ex.Message.ToString());
                if (Message.IndexOf("無法使用信箱") != -1)
                    Message = Message + ",....格式正確的郵件可能已成功寄出。";

                //寫入Log (未篩選過的收件人)
                EmailLog(Sender, Reciever, CC, BCC
                      , FromFunc, DateTime.Now, Convert.ToString(MessageCode), Message, Create_Who);
            }
        }

        /// <summary>
        /// 新增Email Log
        /// </summary>
        /// <param name="Sender">寄件人</param>
        /// <param name="Reciever">收件人</param>
        /// <param name="CC">轉寄人</param>
        /// <param name="Bcc">密件轉寄人</param>
        /// <param name="FromFunc">來源程式</param>
        /// <param name="Send_Time">寄件時間</param>
        /// <param name="Send_Status">寄件狀態</param>
        /// <param name="Send_Desc">寄件描述</param>
        /// <param name="Create_Who">建立者</param>
        /// <returns>bool</returns>
        private static bool EmailLog(string Sender, List<string> Reciever, List<string> CC, List<string> Bcc
            , string FromFunc, DateTime Send_Time, string Send_Status, string Send_Desc, string Create_Who)
        {
            string ErrMsg = "";
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("Declare @New_ID AS INT SET @New_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_SendMail) ");
                    SBSql.AppendLine(" INSERT INTO Log_SendMail( ");
                    SBSql.AppendLine("  Log_ID, Sender, Reciever, CC, Bcc ");
                    SBSql.AppendLine("  , FromFunc, Send_Time, Send_Status, Send_Desc ");
                    SBSql.AppendLine("  , Create_Who, Create_Time ");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Sender, @Reciever, @CC, @Bcc ");
                    SBSql.AppendLine("  , @FromFunc, @Send_Time, @Send_Status, @Send_Desc ");
                    SBSql.AppendLine("  , @Create_Who, GETDATE() ");
                    SBSql.AppendLine(" ) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Sender", Sender);
                    //Reciever
                    StringBuilder sb = new StringBuilder();
                    if (Reciever != null)
                    {
                        for (int i = 0; i < Reciever.Count; i++)
                        {
                            sb.Append(Reciever[i] + ";");
                        }

                        cmd.Parameters.AddWithValue("Reciever", sb.ToString());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("Reciever", DBNull.Value);
                    }

                    //CC
                    if (CC != null)
                    {
                        sb.Clear();
                        for (int i = 0; i < CC.Count; i++)
                        {
                            sb.Append(CC[i] + ";");
                        }
                        cmd.Parameters.AddWithValue("CC", sb.ToString());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("CC", DBNull.Value);
                    }

                    //Bcc
                    if (Bcc != null)
                    {
                        sb.Clear();
                        for (int i = 0; i < Bcc.Count; i++)
                        {
                            sb.Append(Bcc[i] + ";");
                        }
                        cmd.Parameters.AddWithValue("Bcc", sb.ToString());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("Bcc", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("FromFunc", FromFunc);
                    cmd.Parameters.AddWithValue("Send_Time", Send_Time);
                    cmd.Parameters.AddWithValue("Send_Status", Send_Status);
                    cmd.Parameters.AddWithValue("Send_Desc", Send_Desc);
                    cmd.Parameters.AddWithValue("Create_Who", Create_Who);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 判斷Email格式是否正確, 重新組合&回傳正確的Email
        /// 移除重複的Email
        /// </summary>
        /// <param name="_Email">Email</param>
        /// <returns>List<string></returns>
        private static List<string> Check_EmailAddress(List<string> _Email)
        {
            if (_Email == null)
                return null;

            //移除重複的Email
            List<string> validEmail = new List<string>();
            var query = from el in _Email
                        group el by el.ToString() into gp
                        select new
                        {
                            Text = gp.Key
                        };
            foreach (var item in query)
            {
                //判斷Email是否正確, 將正確的Email存至List
                if (item.Text.IsEmail())
                    validEmail.Add(item.Text);
            }

            return validEmail;
        }
    }


    /*使用範例
     * //[設定參數] - 建立者
        fn_Mail.Create_Who = fn_Params.UserAccount;

        //[設定參數] - 來源程式/功能
        fn_Mail.FromFunc = "測試區";

        //[設定參數] - 寄件人
        fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

        //[設定參數] - 寄件人顯示名稱
        fn_Mail.SenderName = "IronMan";

        //[設定參數] - 收件人
        List<string> emailTo = new List<string>();
        emailTo.Add("clyde@mail.prokits.com.tw");      
        emailTo.Add("clyde@mail.prokits.com.tw");

        fn_Mail.Reciever = emailTo;

        //[設定參數] - 轉寄人群組
        fn_Mail.CC = null;

        //[設定參數] - 密件轉寄人群組
        fn_Mail.BCC = null;

        //[設定參數] - 郵件主旨
        fn_Mail.Subject = "測試信0720";

        //[設定參數] - 郵件內容
        StringBuilder mailBody = new StringBuilder();
        mailBody.AppendLine("<html>");
        mailBody.AppendLine("<body>");
        mailBody.AppendLine("<p>#Top_Logo#</p>"); //指定附圖
        mailBody.AppendLine("<hr>");
        mailBody.AppendLine("我是html內容....");
        mailBody.AppendLine("<hr>");
        mailBody.AppendLine("</body>");
        mailBody.AppendLine("</html>");

        fn_Mail.MailBody = mailBody;

        //[設定參數] - 指定檔案 - 路徑
        fn_Mail.FilePath = "";

        //[設定參數] - 指定檔案 - 檔名
        fn_Mail.FileName = "";

        //發送郵件
        fn_Mail.SendMail();
        //fn_Mail.SendMail_WithFile(this.FileUpload1);

        //[讀取參數] 
        Response.Write(fn_Mail.Message);
     * 
     */
}
