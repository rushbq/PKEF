using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MISData.Models
{
    #region *** 網站功能點擊 ***
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

    #endregion


    #region *** 資訊需求 ***
    public class ItHelpData
    {
        public Int64 SeqNo { get; set; }
        public Guid DataID { get; set; }
        public string TraceID { get; set; }
        public Int16 Apply_Type { get; set; }
        public Int32 Req_Class { get; set; }
        public string Req_ClassName { get; set; }
        public string Req_Who { get; set; }
        public string Req_WhoName { get; set; }
        public string Req_NickName { get; set; }
        public string Req_Email { get; set; }
        public string Req_TelExt { get; set; }
        public string Req_Dept { get; set; }
        public string Req_DeptName { get; set; }
        public string Help_Subject { get; set; }
        public string Help_Content { get; set; }
        public string Help_Benefit { get; set; }
        public Int32 Help_Status { get; set; }
        public string Help_StatusName { get; set; }
        public Int16 Help_Way { get; set; }
        public string Reply_Content { get; set; }
        public string ProcInfo { get; set; }
        public string onTop { get; set; }
        public string onTopWho { get; set; }
        public string IsRate { get; set; }
        public Int16 RateQ1 { get; set; }
        public Int16 RateQ2 { get; set; }
        public Int16 RateScore { get; set; }
        public string RateContent { get; set; }
        public string RateWhoName { get; set; }
        public double? Finish_Hours { get; set; }
        public string Finish_Time { get; set; }
        public string Finish_WhoName { get; set; }
        public string CreateDay { get; set; }
        public string Create_Name { get; set; }
        public string Create_Time { get; set; }
        public string Update_Name { get; set; }
        public string Update_Time { get; set; }
        public string Agree_WhoName { get; set; }
        public string Agree_Time { get; set; }
        public string IsAgree { get; set; }
        public string Wish_Time { get; set; }
        public Int32 IsDeptManager { get; set; }
        public Int32 dfDay { get; set; }
        
    }


    /// <summary>
    /// 資訊需求:附件
    /// </summary>
    public class ITHelpAttachment
    {
        public int AttachID { get; set; }
        public string AttachFile { get; set; }
        public string AttachFile_Org { get; set; }
        public string Create_Who { get; set; }

        /// <summary>
        /// 關聯用
        /// </summary>
        public Guid ParentID { get; set; }
        /// <summary>
        /// 關聯用
        /// </summary>
        public int DetailID { get; set; }

    }

    /// <summary>
    /// 資訊需求:轉寄通知
    /// </summary>
    public class ITHelpCC
    {
        public string CC_Who { get; set; }
        public string CC_Email { get; set; }

    }

    /// <summary>
    /// 資訊需求:處理進度
    /// </summary>
    public class ITHelpProc
    {
        //public Guid Parent_ID { get; set; }
        public int DetailID { get; set; }
        public int Class_ID { get; set; }
        public string Class_Name { get; set; }
        public string Proc_Desc { get; set; }
        public string Proc_Time { get; set; }
        public string Confirm_Time { get; set; }
        public string Wish_Time { get; set; }
        public string Create_Time { get; set; }
        public string Create_WhoName { get; set; }
    }

    /// <summary>
    /// 資訊需求:固定收信清單
    /// </summary>
    public class ITHelpReceiver
    {
        public string Email { get; set; }
    }
    #endregion

}