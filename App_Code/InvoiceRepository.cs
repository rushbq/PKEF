﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ExtensionUI;
using Invoice.Models;
using PKLib_Method.Methods;

namespace Invoice.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        Status = 3,
        StartDate = 4,
        EndDate = 5,
        CheckRel = 6
    }

    public class InvoiceRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<InvoiceData> GetDataList(out string ErrMsg)
        {
            return GetDataList(null, out ErrMsg);
        }


        /// <summary>
        /// 取得所有資料 - 發票維護查詢
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<InvoiceData> GetDataList(Dictionary<int, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<InvoiceData> dataList = new List<InvoiceData>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search, out ErrMsg))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new InvoiceData
                    {
                        SerialNo = item.Field<Int64>("SerialNo"),
                        OrderID = item.Field<string>("OrderID"),
                        TraceID = item.Field<string>("TraceID"),
                        NickName = item.Field<string>("NickName"),
                        SO_FID = item.Field<string>("SO_FID"),
                        SO_SID = item.Field<string>("SO_SID"),
                        SO_Date = item.Field<string>("SO_Date"),
                        CustID = item.Field<string>("CustID"),
                        CustName = item.Field<string>("CustName"),
                        TotalPrice = item.Field<double>("TotalPrice"),
                        BI_FID = item.Field<string>("BI_FID"),
                        BI_SID = item.Field<string>("BI_SID"),
                        InvoiceNo = item.Field<string>("InvoiceNo"),
                        InvoiceDate = item.Field<string>("InvoiceDate"),
                        InvTitle = item.Field<string>("InvTitle"),
                        InvType = item.Field<string>("InvType"),
                        InvNumber = item.Field<string>("InvNumber"),
                        InvAddrInfo = item.Field<string>("InvAddrInfo"),
                        InvBankInfo = item.Field<string>("InvBankInfo"),
                        InvMessage = item.Field<string>("InvMessage"),
                        InvRemark = item.Field<string>("InvRemark"),
                        InvStatus = item.Field<string>("InvStatus")
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得銷貨明細資料 - 單身
        /// 功能位置:發票維護列表, 明細按鈕
        /// </summary>
        /// <param name="FID">銷貨單別</param>
        /// <param name="SID">銷貨單號</param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetDetailList(string FID, string SID)
        {
            //----- 宣告 -----
            List<RefColumn> dataList = new List<RefColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(COPTH.TH001) AS SO_FID, RTRIM(COPTH.TH002) AS SO_SID, RTRIM(COPTH.TH003) AS SO_No ");
                sql.AppendLine("  , RTRIM(COPTH.TH004) ModelNo, RTRIM(COPTH.TH005) ModelName ");
                sql.AppendLine("  , CONVERT(INT, COPTH.TH008) Qty, CONVERT(FLOAT, COPTH.TH035) Price, CONVERT(FLOAT, COPTH.TH036) TaxPrice ");
                sql.AppendLine("  , (CASE WHEN Rel.SNO IS NULL THEN 'Y' ELSE 'N' END) AS IsInvoice ");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTH WITH(NOLOCK) ");
                sql.AppendLine("  LEFT JOIN BBC_NotInvoiceItem Rel ON COPTH.TH001 = Rel.FID COLLATE Chinese_Taiwan_Stroke_BIN AND COPTH.TH002 = Rel.SID COLLATE Chinese_Taiwan_Stroke_BIN AND COPTH.TH003 = Rel.SNO COLLATE Chinese_Taiwan_Stroke_BIN ");
                sql.AppendLine(" WHERE (COPTH.TH001 = @SO_FID) AND (COPTH.TH002 = @SO_SID) ");
                sql.AppendLine(" ORDER BY COPTH.TH003, COPTH.TH004 ");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("SO_FID", FID);
                cmd.Parameters.AddWithValue("SO_SID", SID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefColumn
                        {
                            SO_FID = item.Field<string>("SO_FID"),
                            SO_SID = item.Field<string>("SO_SID"),
                            SO_No = item.Field<string>("SO_No"),
                            ModelNo = item.Field<string>("ModelNo"),
                            ModelName = item.Field<string>("ModelName"),
                            Qty = item.Field<int>("Qty"),
                            Price = item.Field<double>("Price"),
                            TaxPrice = item.Field<double>("TaxPrice"),
                            IsInvoice = item.Field<string>("IsInvoice")

                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        ///// <summary>
        ///// 匯出清單 List
        ///// </summary>
        ///// <param name="search"></param>
        ///// <returns></returns>
        //public IQueryable<ExportBase> GetExportList(Dictionary<int, string> search)
        //{
        //    //----- 宣告 -----
        //    List<ExportBase> dataList = new List<ExportBase>();
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----

        //        sql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY Base.Create_Time DESC) AS SerialNo ");
        //        sql.AppendLine("  , Base.Data_ID, Base.Subject, Base.IsSend");
        //        sql.AppendLine("  , Base.Create_Time, Base.Update_Time, Base.Send_Time");
        //        sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE (Guid = Base.Create_Who)) AS Create_Name ");
        //        sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE (Guid = Base.Update_Who)) AS Update_Name ");
        //        sql.AppendLine("  , (SELECT COUNT(*) FROM BBC_InvoiceMail_RelItem WITH(NOLOCK) WHERE (MailID = Base.Data_ID)) AS RelCnt ");
        //        sql.AppendLine(" FROM BBC_InvoiceMail Base WITH(NOLOCK) ");
        //        sql.AppendLine(" WHERE (1=1) ");

        //        /* Search */
        //        if (search != null)
        //        {
        //            foreach (var item in search)
        //            {
        //                switch (item.Key)
        //                {
        //                    case (int)mySearch.DataID:
        //                        if (!string.IsNullOrEmpty(item.Value))
        //                        {
        //                            //銷貨單號
        //                            sql.Append(" AND (Base.Data_ID = @DataID)");

        //                            cmd.Parameters.AddWithValue("DataID", item.Value);
        //                        }

        //                        break;

        //                    case (int)mySearch.Keyword:
        //                        if (!string.IsNullOrEmpty(item.Value))
        //                        {
        //                            sql.Append(" AND ( ");
        //                            sql.Append("	(Base.Subject LIKE '%' + @Keyword + '%') ");
        //                            sql.Append(" ) ");

        //                            cmd.Parameters.AddWithValue("Keyword", item.Value);
        //                        }

        //                        break;

        //                }
        //            }
        //        }

        //        sql.AppendLine(" ORDER BY Base.Create_Time DESC ");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();


        //        //----- 資料取得 -----
        //        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
        //        {
        //            //LinQ 查詢
        //            var query = DT.AsEnumerable();

        //            //資料迴圈
        //            foreach (var item in query)
        //            {
        //                //加入項目
        //                var data = new ExportBase
        //                {
        //                    DataID = item.Field<Guid>("Data_ID"),
        //                    SerialNo = item.Field<Int64>("SerialNo"),
        //                    Subject = item.Field<string>("Subject"),
        //                    IsSend = item.Field<string>("IsSend"),
        //                    Create_Name = item.Field<string>("Create_Name"),
        //                    Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
        //                    Update_Name = item.Field<string>("Update_Name"),
        //                    Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
        //                    Send_Time = item.Field<DateTime?>("Send_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
        //                    RelCnt = item.Field<int>("RelCnt")
        //                };

        //                //將項目加入至集合
        //                dataList.Add(data);

        //            }
        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}


        ///// <summary>
        ///// 取得所有資料 - 匯出過程使用
        ///// </summary>
        ///// <param name="search"></param>
        ///// <returns></returns>
        //public IQueryable<InvoiceData> GetDataList_withExport(Dictionary<int, string> search)
        //{
        //    //----- 宣告 -----
        //    List<InvoiceData> dataList = new List<InvoiceData>();

        //    //----- 資料取得 -----
        //    using (DataTable DT = LookupRawData_withExport(search))
        //    {
        //        //LinQ 查詢
        //        var query = DT.AsEnumerable();

        //        //資料迴圈
        //        foreach (var item in query)
        //        {
        //            //加入項目
        //            var data = new InvoiceData
        //            {
        //                SerialNo = item.Field<Int64>("SerialNo"),
        //                OrderID = item.Field<string>("OrderID"),
        //                NickName = item.Field<string>("NickName"),
        //                SO_FID = item.Field<string>("SO_FID"),
        //                SO_SID = item.Field<string>("SO_SID"),
        //                SO_Date = item.Field<string>("SO_Date"),
        //                CustID = item.Field<string>("CustID"),
        //                CustName = item.Field<string>("CustName"),
        //                TotalPrice = item.Field<double>("TotalPrice"),
        //                BI_FID = item.Field<string>("BI_FID"),
        //                BI_SID = item.Field<string>("BI_SID"),
        //                InvTitle = item.Field<string>("InvTitle"),
        //                InvType = item.Field<string>("InvType"),
        //                InvNumber = item.Field<string>("InvNumber"),
        //                InvAddrInfo = item.Field<string>("InvAddrInfo"),
        //                InvBankInfo = item.Field<string>("InvBankInfo"),
        //                InvMessage = item.Field<string>("InvMessage"),
        //                InvRemark = item.Field<string>("InvRemark"),
        //                Item_DataID = item.Field<int>("Item_DataID")
        //            };

        //            //將項目加入至集合
        //            dataList.Add(data);

        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}


        ///// <summary>
        ///// 取得所有資料 - 發送郵件使用
        ///// </summary>
        ///// <param name="search"></param>
        ///// <returns></returns>
        //public IQueryable<ExportDT> GetDataList_withEmail(Dictionary<int, string> search)
        //{
        //    //----- 宣告 -----
        //    List<ExportDT> dataList = new List<ExportDT>();

        //    //----- 資料取得 -----
        //    using (DataTable DT = LookupRawData_withEmail(search))
        //    {
        //        //LinQ 查詢
        //        var query = DT.AsEnumerable();

        //        //資料迴圈
        //        foreach (var item in query)
        //        {
        //            //加入項目
        //            var data = new ExportDT
        //            {
        //                CustID = item.Field<string>("CustID"),
        //                CustName = item.Field<string>("CustName"),
        //                ModelNo = item.Field<string>("ModelNo"),
        //                ModelName = item.Field<string>("ModelName"),
        //                Qty = item.Field<int>("Qty"),
        //                Unit = item.Field<string>("Unit").Replace("PCE", "个").Replace("SET", "套"),
        //                UnitPrice = item.Field<double>("UnitPrice"),
        //                Price = item.Field<double>("Price"),
        //                TaxPrice = item.Field<double>("TaxPrice"),
        //                TotalPrice = item.Field<double>("Price") + item.Field<double>("TaxPrice"),
        //                SO_No = "{0}-{1}-{2}".FormatThis(item.Field<string>("SO_FID"), item.Field<string>("SO_SID"), item.Field<string>("SO_Num")),
        //                SO_Date = item.Field<string>("SO_Date"),
        //                Inv_Type = item.Field<string>("Inv_Type"),
        //                Inv_Title = item.Field<string>("Inv_Title"),
        //                Inv_Number = item.Field<string>("Inv_Number"),
        //                Inv_AddrInfo = fn_CustomUI.ReplaceLowOrderASCIICharacters(item.Field<string>("Inv_AddrInfo")),
        //                Inv_BankInfo = fn_CustomUI.ReplaceLowOrderASCIICharacters(item.Field<string>("Inv_BankInfo")),
        //                Acc_ClassNo = "108040412",
        //                Acc_VerNo = "1.0",
        //                Acc_Taxlaw = "0",
        //                Inv_Remark = fn_CustomUI.ReplaceLowOrderASCIICharacters(item.Field<string>("Inv_Remark"))
        //            };

        //            //將項目加入至集合
        //            dataList.Add(data);

        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}


        ///// <summary>
        ///// 取得收件人清單
        ///// </summary>
        ///// <returns></returns>
        //public IQueryable<MailtoList> GetMailList()
        //{
        //    //----- 宣告 -----
        //    List<MailtoList> dataList = new List<MailtoList>();
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----

        //        sql.AppendLine(" SELECT MailAddress, MailName ");
        //        sql.AppendLine(" FROM BBC_InvoiceMail_Receiver WITH(NOLOCK) ");
        //        sql.AppendLine(" WHERE (Display = 'Y') ");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();


        //        //----- 資料取得 -----
        //        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
        //        {
        //            //LinQ 查詢
        //            var query = DT.AsEnumerable();

        //            //資料迴圈
        //            foreach (var item in query)
        //            {
        //                //加入項目
        //                var data = new MailtoList
        //                {
        //                    mailAddr = item.Field<string>("MailAddress"),
        //                    mailName = item.Field<string>("MailName")

        //                };

        //                //將項目加入至集合
        //                dataList.Add(data);

        //            }
        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}



        /// <summary>
        /// 發票子檔清單 - 維護頁子查詢用(歷史記錄)
        /// 功能位置:發票維護,歷史資料按鈕
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IQueryable<InvoiceColumn> GetInvItemList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<InvoiceColumn> dataList = new List<InvoiceColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY Base.Inv_Title, Base.Inv_Number) AS SerialNo ");
                sql.AppendLine("  , ISNULL(Base.Inv_Type, '') InvType, ISNULL(Base.Inv_Title, '') InvTitle, ISNULL(Base.Inv_Number, '') InvNumber ");
                sql.AppendLine("  , ISNULL(Base.Inv_AddrInfo, '') InvAddrInfo, ISNULL(Base.Inv_BankInfo, '') InvBankInfo ");
                sql.AppendLine("  , ISNULL(Base.Inv_Message, '') InvMessage ");
                sql.AppendLine(" FROM BBC_InvoiceItem Base WITH(NOLOCK)");
                sql.AppendLine(" WHERE (1=1) ");

                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.AppendLine(" AND ( ");
                                    sql.AppendLine("    (Base.Inv_Title LIKE '%' + @Keyword + '%') ");
                                    sql.AppendLine("    OR (Base.Inv_Number LIKE '%' + @Keyword + '%') ");
                                    sql.AppendLine("    OR (Base.OrderID IN (");
                                    sql.AppendLine("     SELECT DT.OrderID FROM BBC_ImportData_DT DT WITH(NOLOCK)");
                                    sql.AppendLine("     WHERE (NickName IS NOT NULL AND NickName <> '') AND (DT.NickName LIKE '%' + @Keyword + '%')");
                                    sql.AppendLine("      )");
                                    sql.AppendLine("    )");
                                    sql.AppendLine(") ");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                        }
                    }
                }

                sql.AppendLine(" GROUP BY Base.Inv_Type, Base.Inv_Title, Base.Inv_Number, Base.Inv_AddrInfo, Base.Inv_BankInfo, Base.Inv_Message ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new InvoiceColumn
                        {
                            SerialNo = item.Field<Int64>("SerialNo"),
                            InvTitle = item.Field<string>("InvTitle"),
                            InvType = item.Field<string>("InvType"),
                            InvNumber = item.Field<string>("InvNumber"),
                            InvAddrInfo = item.Field<string>("InvAddrInfo"),
                            InvBankInfo = item.Field<string>("InvBankInfo"),
                            InvMessage = item.Field<string>("InvMessage")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        #endregion


        #region  -----// Create //-----

        ///// <summary>
        ///// 建立匯出檔 - Step1
        ///// </summary>
        ///// <param name="baseData"></param>
        ///// <returns></returns>
        //public bool Create_ExportData(ExportBase baseData)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" INSERT INTO BBC_InvoiceMail(");
        //        sql.AppendLine(" Data_ID, Subject, IsSend, Create_Who, Create_Time");
        //        sql.AppendLine(" ) VALUES (");
        //        sql.AppendLine(" @DataID, @Subject, 'N', @Create_Who, GETDATE()");
        //        sql.AppendLine(" )");


        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", baseData.DataID);
        //        cmd.Parameters.AddWithValue("Subject", baseData.Subject);
        //        cmd.Parameters.AddWithValue("Create_Who", baseData.Create_Who);

        //        return dbConn.ExecuteSql(cmd, out ErrMsg);
        //    }
        //}


        ///// <summary>
        ///// 建立匯出關聯 - Step2
        ///// </summary>
        ///// <param name="dataID"></param>
        ///// <param name="aryID"></param>
        ///// <returns></returns>
        //public bool Create_ExportItem(ExportBase baseData, ArrayList aryID, out string ErrMsg)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----

        //        sql.AppendLine(" UPDATE BBC_InvoiceMail SET Update_Who = @Update_Who, Update_Time = GETDATE() ");
        //        sql.AppendLine(" WHERE (Data_ID = @DataID);");

        //        sql.AppendLine(" DELETE BBC_InvoiceMail_RelItem WHERE (MailID = @DataID); ");

        //        for (int row = 0; row < aryID.Count; row++)
        //        {
        //            sql.AppendLine(" INSERT INTO BBC_InvoiceMail_RelItem(");
        //            sql.AppendLine(" MailID, ItemID");
        //            sql.AppendLine(" ) VALUES (");
        //            sql.AppendLine(" @DataID, @ids_{0}".FormatThis(row));
        //            sql.AppendLine(" );");


        //            cmd.Parameters.AddWithValue("ids_{0}".FormatThis(row), aryID[row]);
        //        }


        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", baseData.DataID);
        //        cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

        //        return dbConn.ExecuteSql(cmd, out ErrMsg);
        //    }
        //}


        #endregion


        #region -----// Update & Set //-----

        /// <summary>
        /// 指定品項是否發開票 - 發票維護
        /// </summary>
        /// <param name="FID"></param>
        /// <param name="SID"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Update_Status(string FID, string SID, string SNO, string val)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 語法 -----
                if (val.Equals("N"))
                {
                    //INSERT(不開發票)
                    sql.AppendLine(" IF (SELECT COUNT(*) FROM BBC_NotInvoiceItem Base WHERE (Base.FID = @FID) AND (Base.SID = @SID) AND (Base.SNO = @SNO)) = 0 ");
                    sql.AppendLine(" BEGIN ");
                    sql.AppendLine("  INSERT INTO BBC_NotInvoiceItem (FID, SID, SNO) ");
                    sql.AppendLine("  VALUES (@FID, @SID, @SNO)");
                    sql.AppendLine(" END ");
                }
                else
                {
                    //DELETE
                    sql.AppendLine(" DELETE FROM BBC_NotInvoiceItem WHERE (FID = @FID) AND (SID = @SID) AND (SNO = @SNO)");
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("FID", FID);
                cmd.Parameters.AddWithValue("SID", SID);
                cmd.Parameters.AddWithValue("SNO", SNO);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 更新發票資料 - 發票維護
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool SetInvoice(InvoiceData baseData, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" IF (SELECT COUNT(*) FROM BBC_InvoiceItem WHERE (OrderID = @OrderID) AND (TraceID = @TraceID)) = 0 ");
                sql.AppendLine(" BEGIN ");
                sql.AppendLine(" DECLARE @DataID AS INT ");
                sql.AppendLine(" SET @DataID = (SELECT ISNULL(MAX(Data_ID), 0) + 1 FROM BBC_InvoiceItem) ");
                sql.AppendLine("    INSERT INTO BBC_InvoiceItem ( ");
                sql.AppendLine("    Data_ID, OrderID, TraceID ");
                sql.AppendLine("    , Inv_Type, Inv_Title, Inv_Number ");
                sql.AppendLine("    , Inv_AddrInfo, Inv_BankInfo, Inv_Message, Inv_Remark ");
                sql.AppendLine("    , Create_Who, Create_Time ");
                sql.AppendLine("    ) VALUES ( ");
                sql.AppendLine("    @DataID, @OrderID, @TraceID ");
                sql.AppendLine("    , @Inv_Type, @Inv_Title, @Inv_Number ");
                sql.AppendLine("    , @Inv_AddrInfo, @Inv_BankInfo, @Inv_Message, @Inv_Remark ");
                sql.AppendLine("    , @Create_Who, GETDATE() ");
                sql.AppendLine("    ); ");
                sql.AppendLine(" END ");
                sql.AppendLine("  ELSE ");
                sql.AppendLine(" BEGIN ");
                sql.AppendLine("    UPDATE BBC_InvoiceItem ");
                sql.AppendLine("    SET Inv_Type = @Inv_Type, Inv_Title = @Inv_Title, Inv_Number = @Inv_Number ");
                sql.AppendLine("    , Inv_AddrInfo = @Inv_AddrInfo, Inv_BankInfo = @Inv_BankInfo, Inv_Message = @Inv_Message, Inv_Remark = @Inv_Remark ");
                sql.AppendLine("    , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                sql.AppendLine("    WHERE (OrderID = @OrderID) AND (TraceID = @TraceID); ");
                sql.AppendLine(" END ");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("OrderID", baseData.OrderID);
                cmd.Parameters.AddWithValue("TraceID", baseData.TraceID);
                cmd.Parameters.AddWithValue("Inv_Type", baseData.InvType);
                cmd.Parameters.AddWithValue("Inv_Title", baseData.InvTitle);
                cmd.Parameters.AddWithValue("Inv_Number", baseData.InvNumber);
                cmd.Parameters.AddWithValue("Inv_AddrInfo", baseData.InvAddrInfo);
                cmd.Parameters.AddWithValue("Inv_BankInfo", baseData.InvBankInfo);
                cmd.Parameters.AddWithValue("Inv_Message", baseData.InvMessage);
                cmd.Parameters.AddWithValue("Inv_Remark", baseData.InvRemark);
                cmd.Parameters.AddWithValue("Create_Who", baseData.Create_Who);
                cmd.Parameters.AddWithValue("Update_Who", baseData.Update_Who);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }


        }


        ///// <summary>
        ///// 更新匯出主檔 - 設為已發送
        ///// </summary>
        ///// <param name="baseData"></param>
        ///// <returns></returns>
        //public bool Update_Export(ExportBase baseData)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" UPDATE BBC_InvoiceMail SET IsSend = 'Y', Send_Who = @Send_Who, Send_Time = GETDATE()");
        //        sql.AppendLine(" WHERE (Data_ID = @DataID)");


        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", baseData.DataID);
        //        cmd.Parameters.AddWithValue("Send_Who", baseData.Send_Who);

        //        return dbConn.ExecuteSql(cmd, out ErrMsg);
        //    }
        //}


        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// 取得原始資料 - 發票維護查詢
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                //** WITH AS ---
                sql.AppendLine("WITH TblBase AS (");
                sql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY COPTG.TG001, COPTG.TG002) AS SerialNo");
                sql.AppendLine("  , COPTC.TC012 AS OrderID ");
                sql.AppendLine("  , COPTC.TC202 AS TraceID ");
                sql.AppendLine(", ISNULL((");
                sql.AppendLine("  SELECT TOP 1 ISNULL(DT.NickName,'')");
                sql.AppendLine("  FROM [PKEF].dbo.BBC_ImportData AS Base WITH(NOLOCK)");
                sql.AppendLine("   INNER JOIN [PKEF].dbo.BBC_ImportData_DT AS DT WITH(NOLOCK) ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  WHERE (Base.TraceID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC202) AND (DT.OrderID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC012)");
                sql.AppendLine(" ), '') AS NickName ");
                sql.AppendLine("  , RTRIM(COPTG.TG001) SO_FID, RTRIM(COPTG.TG002) SO_SID, COPTG.TG003 SO_Date ");
                sql.AppendLine("  , RTRIM(COPMA.MA001) CustID, RTRIM(COPMA.MA002) CustName ");
                sql.AppendLine("  , ROUND(SUM(CONVERT(FLOAT, COPTH.TH035) + CONVERT(FLOAT, COPTH.TH036)), 2) TotalPrice");
                sql.AppendLine("  , RTRIM(ACRTA.TA001) BI_FID, RTRIM(ACRTA.TA002) BI_SID ");
                sql.AppendLine("  , ACRTA.TA036 InvoiceNo, ACRTA.TA200 InvoiceDate ");
                sql.AppendLine(" FROM [ProUnion].dbo.COPTG WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTH.TH014 = COPTD.TD001 AND COPTH.TH015 = COPTD.TD002 AND COPTH.TH016 = COPTD.TD003 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTC WITH(NOLOCK) ON COPTD.TD001 = COPTC.TC001 AND COPTD.TD002 = COPTC.TC002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTG.TG004 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTA WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002 ");
                sql.AppendLine(" WHERE (COPTC.TC200 = 'Y') AND (COPTC.TC201 IN (N'京東POP',N'京東VC',N'京東廠送',N'唯品會',N'天貓'))");
                
                /* Search */
                #region search1
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //銷貨單號
                                    sql.Append(" AND (RTRIM(COPTG.TG001) + RTRIM(COPTG.TG002) = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;


                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //ERP日期格式YYYYMMDD
                                    sql.Append(" AND (COPTG.TG003 >= @sDate)");

                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //ERP日期格式YYYYMMDD
                                    sql.Append(" AND (COPTG.TG003 <= @eDate)");

                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;

                        }
                    }
                }
                #endregion

                //Group by
                sql.AppendLine(" GROUP BY COPTC.TC012, COPTC.TC202, COPTG.TG001, COPTG.TG002, COPTG.TG003 ");
                sql.AppendLine(" , COPMA.MA001, COPMA.MA002, ACRTA.TA001, ACRTA.TA002, ACRTA.TA036, ACRTA.TA200 ");
                sql.AppendLine(") ");
                //** WITH AS ---

                sql.AppendLine(" SELECT TblBase.* ");
                sql.AppendLine("  , ISNULL(Item.Inv_Title, '') InvTitle, ISNULL(Item.Inv_Type, '') InvType, ISNULL(Item.Inv_Number, '') InvNumber ");
                sql.AppendLine("  , ISNULL(Item.Inv_AddrInfo, '') InvAddrInfo, ISNULL(Item.Inv_BankInfo, '') InvBankInfo ");
                sql.AppendLine("  , ISNULL(Item.Inv_Message, '') InvMessage, ISNULL(Item.Inv_Remark, '') InvRemark ");
                sql.AppendLine("  , (CASE ");
                sql.AppendLine("    WHEN Item.Inv_Title IS NULL AND TblBase.InvoiceNo = '' THEN '1' ");
                sql.AppendLine("    WHEN TblBase.InvoiceNo <> '' THEN '3' ");
                sql.AppendLine("    ELSE '2' ");
                sql.AppendLine("  END) InvStatus ");
                sql.AppendLine(" FROM TblBase ");
                sql.AppendLine("  LEFT JOIN BBC_InvoiceItem Item WITH(NOLOCK) ON Item.OrderID COLLATE Chinese_Taiwan_Stroke_BIN = TblBase.OrderID AND Item.TraceID COLLATE Chinese_Taiwan_Stroke_BIN = TblBase.TraceID");
                sql.AppendLine(" WHERE (1=1) ");

                #region search2
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //Keyword:客戶單號/銷貨單號/訂單單號/客戶暱稱/姓名/發票抬頭
                                    sql.Append("AND ( ");
                                    sql.Append("	(TblBase.OrderID LIKE '%' + @Keyword + '%') ");
                                    sql.Append("	OR (Item.Inv_Title LIKE '%' + @Keyword + '%') ");
                                    sql.Append("	OR (RTRIM(TblBase.SO_FID) + RTRIM(TblBase.SO_SID) LIKE '%' + REPLACE(UPPER(@Keyword),'-','') + '%') ");
                                    sql.Append("	OR (RTRIM(TblBase.BI_FID) + RTRIM(TblBase.BI_SID) LIKE '%' + REPLACE(UPPER(@Keyword),'-','') + '%') ");
                                    sql.Append("	OR (UPPER((TblBase.CustID)) LIKE '%' + UPPER(@Keyword) + '%') ");
                                    sql.Append("	OR (UPPER((TblBase.CustName)) LIKE '%' + UPPER(@Keyword) + '%') ");
                                    sql.Append("    OR (UPPER((TblBase.NickName)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(") ");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)mySearch.Status:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //判斷狀態
                                    switch (item.Value)
                                    {
                                        case "1":
                                            sql.Append(" AND (Item.Inv_Title IS NULL) AND (TblBase.InvoiceNo = '') ");
                                            break;

                                        case "2":
                                            sql.Append(" AND (Item.Inv_Title <> '') AND (TblBase.InvoiceNo = '') ");
                                            break;

                                        case "3":
                                            sql.Append(" AND (TblBase.InvoiceNo <> '') ");
                                            break;
                                    }
                                }

                                break;


                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY TblBase.SerialNo");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.CommandTimeout = 90;

                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 取得原始資料 - 匯出發票Step2, Step3, List Detail
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData_withExport(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY COPTG.TG001, COPTG.TG002) AS SerialNo ");
                sql.AppendLine(", COPTC.TC012 AS OrderID");
                sql.AppendLine(", ISNULL((SELECT TOP 1 ISNULL(NickName,'') FROM BBC_ImportData_DT WHERE (OrderID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC012) AND (NickName IS NOT NULL AND NickName <> '')), '') AS NickName ");
                sql.AppendLine(", RTRIM(COPTG.TG001) SO_FID, RTRIM(COPTG.TG002) SO_SID, COPTG.TG003 SO_Date ");
                sql.AppendLine(", RTRIM(COPMA.MA001) CustID, RTRIM(COPMA.MA002) CustName ");
                sql.AppendLine(", ROUND(SUM(CONVERT(FLOAT, COPTH.TH035) + CONVERT(FLOAT, COPTH.TH036)), 2) TotalPrice");
                sql.AppendLine(", RTRIM(ACRTA.TA001) BI_FID, RTRIM(ACRTA.TA002) BI_SID ");
                sql.AppendLine(", ISNULL(Item.Inv_Title, '') InvTitle, ISNULL(Item.Inv_Type, '') InvType, ISNULL(Item.Inv_Number, '') InvNumber ");
                sql.AppendLine(", ISNULL(Item.Inv_AddrInfo, '') InvAddrInfo, ISNULL(Item.Inv_BankInfo, '') InvBankInfo ");
                sql.AppendLine(", ISNULL(Item.Inv_Message, '') InvMessage, ISNULL(Item.Inv_Remark, '') InvRemark ");
                sql.AppendLine(", Item.Data_ID AS Item_DataID ");

                sql.AppendLine(" FROM [ProUnion].dbo.COPTG WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTH.TH014 = COPTD.TD001 AND COPTH.TH015 = COPTD.TD002 AND COPTH.TH016 = COPTD.TD003 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTC WITH(NOLOCK) ON COPTD.TD001 = COPTC.TC001 AND COPTD.TD002 = COPTC.TC002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPMA.MA001 = COPTG.TG004 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTB WITH(NOLOCK) ON ACRTB.TB005 = COPTG.TG001 AND ACRTB.TB006 = COPTG.TG002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.ACRTA WITH(NOLOCK) ON ACRTA.TA001 = ACRTB.TB001 AND ACRTA.TA002 = ACRTB.TB002 ");
                sql.AppendLine("  INNER JOIN BBC_InvoiceItem Item WITH(NOLOCK) ON Item.OrderID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC012 AND Item.TraceID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC202 ");
                sql.AppendLine(" WHERE (COPTG.TG200 = 'Y') AND (COPTC.TC200 = 'Y') AND (Item.Inv_Title <> '') ");

                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //BBC_InvoiceMail.Data_ID
                                    sql.Append(" AND (Item.Data_ID IN (");
                                    sql.Append("  SELECT ItemID FROM BBC_InvoiceMail_RelItem WHERE (MailID = @DataID)");
                                    sql.Append(")) ");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.CheckRel:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //排除已匯出過的發票
                                    sql.Append(" AND (ACRTA.TA036 = '') AND (Item.Data_ID NOT IN ( ");
                                    sql.Append("	SELECT ItemID FROM BBC_InvoiceMail_RelItem");
                                    sql.Append(" )) ");
                                }

                                break;


                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //ERP日期格式YYYYMMDD
                                    sql.Append(" AND (COPTG.TG003 >= @sDate) ");

                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //ERP日期格式YYYYMMDD
                                    sql.Append(" AND (COPTG.TG003 <= @eDate) ");

                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;

                        }
                    }
                }

                sql.AppendLine(" GROUP BY COPTC.TC012, COPTG.TG001, COPTG.TG002, COPTG.TG003 ");
                sql.AppendLine(", COPMA.MA001, COPMA.MA002, ACRTA.TA001, ACRTA.TA002, ACRTA.TA036, ACRTA.TA200 ");
                sql.AppendLine(", Item.Inv_Title, Item.Inv_Type, Item.Inv_Number, Item.Inv_AddrInfo, Item.Inv_BankInfo, Item.Inv_Message, Item.Inv_Remark, Item.Data_ID ");
                sql.AppendLine(" ORDER BY COPTG.TG001, COPTG.TG002 ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 取得原始資料 - Excel & Email用
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData_withEmail(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT ");
                sql.AppendLine(" RTRIM(COPMA.MA001) CustID, RTRIM(COPMA.MA002) CustName ");
                sql.AppendLine(" , RTRIM(COPTH.TH004) ModelNo, RTRIM(COPTH.TH005) ModelName, CONVERT(INT, COPTH.TH008) Qty, COPTH.TH009 Unit ");
                sql.AppendLine(" , CONVERT(FLOAT, COPTH.TH012) UnitPrice, CONVERT(FLOAT, COPTH.TH036) TaxPrice, CONVERT(FLOAT, COPTH.TH035) Price ");
                sql.AppendLine(" , RTRIM(COPTG.TG001) SO_FID, RTRIM(COPTG.TG002) SO_SID, COPTH.TH003 SO_Num, COPTG.TG003 SO_Date ");
                sql.AppendLine(" , Inv.Inv_Type, Inv.Inv_Title, Inv.Inv_Number, Inv.Inv_AddrInfo, Inv.Inv_BankInfo ");
                sql.AppendLine(" , (CASE WHEN NotInv.SNO IS NOT NULL THEN '不開發票 ' + Inv.Inv_Remark ELSE Inv.Inv_Remark END) Inv_Remark ");
                sql.AppendLine(" FROM BBC_InvoiceMail Base WITH(NOLOCK) ");
                sql.AppendLine("  INNER JOIN BBC_InvoiceMail_RelItem Rel WITH(NOLOCK) ON Base.Data_ID = Rel.MailID ");
                sql.AppendLine("  INNER JOIN BBC_InvoiceItem Inv WITH(NOLOCK) ON Rel.ItemID = Inv.Data_ID ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTC WITH(NOLOCK) ON Inv.OrderID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC012 AND Inv.TraceID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC202 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTD.TD001 = COPTC.TC001 AND COPTD.TD002 = COPTC.TC002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTH.TH014 = COPTD.TD001 AND COPTH.TH015 = COPTD.TD002 AND COPTH.TH016 = COPTD.TD003 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTG WITH(NOLOCK) ON COPTG.TG001 = COPTH.TH001 AND COPTG.TG002 = COPTH.TH002 ");
                sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPMA WITH(NOLOCK) ON COPTG.TG004 = COPMA.MA001 ");
                sql.AppendLine("  LEFT JOIN BBC_NotInvoiceItem NotInv ON COPTH.TH001 = NotInv.FID COLLATE Chinese_Taiwan_Stroke_BIN AND COPTH.TH002 = NotInv.SID COLLATE Chinese_Taiwan_Stroke_BIN AND COPTH.TH003 = NotInv.SNO COLLATE Chinese_Taiwan_Stroke_BIN ");
                sql.AppendLine(" WHERE (1=1) ");

                /* Search */
                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //BBC_InvoiceMail.Data_ID
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                        }
                    }
                }
                sql.AppendLine(" ORDER BY COPMA.MA001, COPTG.TG001, COPTG.TG002, COPTH.TH003, COPTH.TH004 ");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }

        #endregion
    }

}