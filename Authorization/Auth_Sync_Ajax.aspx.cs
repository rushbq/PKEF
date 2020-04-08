using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using ExtensionMethods;

/// <summary>
/// Ajax 同步AD
/// </summary>
public partial class Authorization_Auth_Sync_Ajax : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[驗證] - MD5是否相同
            if (Request["ValidCode"] == null)
            {
                Response.Write("驗證失敗!");
                return;
            }
            if (!Request["ValidCode"].Equals(ValidCode))
            {
                Response.Write("驗證失敗!");
                return;
            }

            //執行同步資料
            string ErrMsg;
            int ErrNum = 0;
            if (SyncGroup(out ErrMsg, out ErrNum) == false)
            {
                Response.Write("群組同步失敗," + ErrMsg);
            }
            if (SyncUser(out ErrMsg, out ErrNum) == false)
            {
                Response.Write("使用者同步失敗," + ErrMsg);
            }
            if (ErrNum == 0)
            {
                Response.Write("同步完成," + DateTime.Now.ToString().ToDateString("yyyy-MM-dd HH:mm"));
                return;
            }
            else
            {
                Response.Write("---同步失敗");
                return;
            }
        }
    }

    /// <summary>
    /// [AD] - 同步群組資料 (OU=MIS_Management)
    /// </summary>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool SyncGroup(out string ErrMsg, out int ErrNum)
    {
        try
        {
            ErrMsg = "";
            ErrNum = 0;
            #region **資料整理比對**
            #region --取得&暫存群組資料--
            //[暫存資料] - AD群組
            List<TempADData> ITempAD = new List<TempADData>();
            //[取得資料] - 所有群組 from AD
            List<ADService.LookupLDAP> ListGroup = ADService.ListGroups("LDAP://OU=MIS_Management,DC=prokits,DC=com,DC=tw");
            if (ListGroup == null)
            {
                ErrMsg = ADService.ProcMessage;
                return false;
            }
            else
            {
                for (int i = 0; i < ListGroup.Count; i++)
                {
                    ITempAD.Add(new TempADData(ListGroup[i].GUID, ListGroup[i].Desc, ListGroup[i].AccountName));
                }
            }

            //[暫存資料] - DB群組
            List<TempADData> ITempDB = new List<TempADData>();
            //[取得資料] - 所有群組 from DB
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT Guid, Display_Name, Account_Name ");
                SBSql.AppendLine(" FROM User_Group WITH (NOLOCK)");
                cmd.CommandText = SBSql.ToString();
                //[參數宣告] - DataTable
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        ITempDB.Add(new TempADData(
                            DT.Rows[i]["Guid"].ToString()
                            , DT.Rows[i]["Display_Name"].ToString()
                            , DT.Rows[i]["Account_Name"].ToString()));
                    }
                }
            }
            #endregion

            //[暫存比對資料] - AD.GUID
            List<string> ListAD_GUID = new List<string>();
            for (int i = 0; i < ITempAD.Count; i++)
            {
                ListAD_GUID.Add(ITempAD[i].GUID);
            }
            //[暫存比對資料] - DB.GUID
            List<string> ListDB_GUID = new List<string>();
            for (int i = 0; i < ITempDB.Count; i++)
            {
                ListDB_GUID.Add(ITempDB[i].GUID);
            }
            //[暫存比對資料] - DB.DispName
            List<string> ListDB_DispName = new List<string>();
            for (int i = 0; i < ITempDB.Count; i++)
            {
                ListDB_DispName.Add(ITempDB[i].DisplayName);
            }

            //[宣告暫存] - 新增/更新/刪除
            List<TempADData> IListAdd = new List<TempADData>();
            List<TempADData> IListUpd = new List<TempADData>();
            List<TempADData> IListDel = new List<TempADData>();

            #region --Update判斷--
            //[資料篩選] - AD to DB
            //AD資料存在於DB -> Update, 暫存至Update參數區
            //1.搜尋GUID已存在於DB的資料
            var queryUpdate = from el in ITempAD
                              where ListDB_GUID.Contains(el.GUID)
                              select new
                              {
                                  GUID = el.GUID,
                                  DisplayName = el.DisplayName,
                                  AccountName = el.AccountName
                              };

            //2.比對DisplayName不相同的資料
            var compareUpdate = from el in queryUpdate
                                where !ListDB_DispName.Contains(el.DisplayName) && (!string.IsNullOrEmpty(el.DisplayName))
                                select new
                                {
                                    GUID = el.GUID,
                                    DisplayName = el.DisplayName,
                                    AccountName = el.AccountName
                                };
            foreach (var item in compareUpdate)
            {
                IListUpd.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName));
            }
            #endregion

            #region --Insert判斷--
            //AD資料不存在於DB -> Insert, 暫存至Insert參數區
            var queryAdd = from el in ITempAD
                           where !ListDB_GUID.Contains(el.GUID)
                           select new
                           {
                               GUID = el.GUID,
                               DisplayName = el.DisplayName,
                               AccountName = el.AccountName
                           };
            foreach (var item in queryAdd)
            {
                IListAdd.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName));
            }
            #endregion

            #region --Delete判斷--
            //[資料篩選] - DB to AD
            //DB資料不存在於AD -> Delete, 暫存至Delete參數區
            var queryDel = from el in ITempDB
                           where !ListAD_GUID.Contains(el.GUID)
                           select new
                           {
                               GUID = el.GUID,
                               DisplayName = el.DisplayName,
                               AccountName = el.AccountName
                           };
            foreach (var item in queryDel)
            {
                IListDel.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName));
            }
            #endregion

            #endregion

            #region **執行資料更新**
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 更新
                #region -SQL更新-
                if (IListUpd.Count > 0)
                {
                    //[初始化]
                    cmd.Parameters.Clear();
                    SBSql.Clear();

                    //[SQL] - 宣告New ID
                    SBSql.AppendLine(" Declare @Log_ID AS INT ");

                    for (int i = 0; i < IListUpd.Count; i++)
                    {
                        //[SQL] - 資料更新
                        SBSql.AppendLine(string.Format(
                            " UPDATE User_Group SET Display_Name = N'{0}' WHERE (Guid = '{1}'); "
                            , IListUpd[i].DisplayName
                            , IListUpd[i].GUID));

                        //[SQL] - 寫入Log
                        SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                        SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                        SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine(" @Log_ID, 'Group', 'Update'");
                        SBSql.Append(string.Format(
                            ", '{0}', N'修改全名, {2}', '{1}'"
                            , IListUpd[i].AccountName
                            , fn_Params.UserAccount
                            , IListUpd[i].DisplayName));
                        SBSql.AppendLine(" );");
                    }
                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    //[執行SQL]
                    if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        ErrNum++;
                    }
                }
                #endregion

                //[SQL] - 新增
                #region -SQL新增-
                if (IListAdd.Count > 0)
                {
                    //[初始化]
                    cmd.Parameters.Clear();
                    SBSql.Clear();

                    //[SQL] - 宣告New ID
                    SBSql.AppendLine(" Declare @Log_ID AS INT ");

                    for (int i = 0; i < IListAdd.Count; i++)
                    {
                        //[SQL] - 資料新增
                        SBSql.AppendLine(string.Format(
                            " INSERT INTO User_Group(Guid, Display_Name, Account_Name, Display, Sort)" +
                            " VALUES ('{0}', N'{1}', '{2}', 'Y', 999); "
                            , IListAdd[i].GUID
                            , IListAdd[i].DisplayName
                            , IListAdd[i].AccountName));

                        //[SQL] - 寫入Log
                        SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                        SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                        SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine(" @Log_ID, 'Group', 'Insert'");
                        SBSql.Append(string.Format(
                            ", '{0}', N'新增群組, {2}', '{1}'"
                            , IListAdd[i].AccountName
                            , fn_Params.UserAccount
                            , IListAdd[i].DisplayName));
                        SBSql.AppendLine(" );");
                    }
                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    //[執行SQL]
                    if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        ErrNum++;
                    }
                }
                #endregion

                //[SQL] - 刪除
                #region "SQL刪除"
                if (IListDel.Count > 0)
                {
                    //[初始化]
                    cmd.Parameters.Clear();
                    SBSql.Clear();

                    //[SQL] - 宣告New ID
                    SBSql.AppendLine(" Declare @Log_ID AS INT ");

                    for (int i = 0; i < IListDel.Count; i++)
                    {
                        //[SQL] - 關聯資料刪除(跨資料庫：PKSYS, PKEF)
                        SBSql.AppendLine(string.Format(
                            " DELETE FROM PKEF.dbo.User_Group_Rel_Program WHERE (Guid = '{0}');" +
                            " DELETE FROM User_Group WHERE (Guid = '{0}');"
                            , IListDel[i].GUID));

                        //[SQL] - 寫入Log
                        SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                        SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                        SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine(" @Log_ID, 'Group', 'Insert'");
                        SBSql.Append(string.Format(
                            ", '{0}', '刪除群組, {2}', '{1}'"
                            , IListDel[i].AccountName
                            , fn_Params.UserAccount
                            , IListDel[i].DisplayName));
                        SBSql.AppendLine(" );");
                    }
                    //[SQL] - CommandText
                    cmd.CommandText = SBSql.ToString();
                    //[執行SQL]
                    if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                    {
                        ErrNum++;
                    }
                }
                #endregion
            }
            #endregion

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            ErrNum = 1;
            return false;
        }

    }

    /// <summary>
    /// [AD] - 同步使用者資料 (OU=Prokits_Users)
    /// </summary>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool SyncUser(out string ErrMsg, out int ErrNum)
    {
        try
        {
            ErrMsg = "";
            ErrNum = 0;
            #region **資料整理比對**

            #region --取得&暫存使用者資料--
            //[暫使用者料] - AD使用者
            List<TempADData> ITempAD = new List<TempADData>();
            //[取得資料] - 所有使用者 from AD
            List<ADService.LookupLDAP> ListUsers = ADService.ListUsers("LDAP://OU=Prokits_Users,DC=prokits,DC=com,DC=tw");
            if (ListUsers == null)
            {
                ErrMsg = ADService.ProcMessage;
                return false;
            }
            else
            {
                for (int i = 0; i < ListUsers.Count; i++)
                {
                    ITempAD.Add(new TempADData(ListUsers[i].GUID, ListUsers[i].DisplayName, ListUsers[i].AccountName
                        , ListUsers[i].Department
                        , (ListUsers[i].userAccountControl == 514) ? "N" : "Y"
                        , ListUsers[i].Email
                        ));
                }
            }

            //[暫使用者料] - DB使用者
            List<TempADData> ITempDB = new List<TempADData>();
            //[取得資料] - 所有使用者 from DB (條件:Display = Y)
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT Guid, Display_Name, Account_Name, DeptID, Display, Email ");
                SBSql.AppendLine(" FROM User_Profile WITH (NOLOCK) WHERE (Display = 'Y') ");
                cmd.CommandText = SBSql.ToString();
                //[參數宣告] - DataTable
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        ITempDB.Add(new TempADData(
                            DT.Rows[i]["Guid"].ToString()
                            , DT.Rows[i]["Display_Name"].ToString()
                            , DT.Rows[i]["Account_Name"].ToString()
                            , DT.Rows[i]["DeptID"].ToString()
                            , DT.Rows[i]["Display"].ToString()
                            , DT.Rows[i]["Email"].ToString()
                            ));
                    }
                }
            }
            #endregion

            //[暫使用者AD資料] - AD.GUID
            List<string> ListAD_GUID = new List<string>();
            for (int i = 0; i < ITempAD.Count; i++)
            {
                ListAD_GUID.Add(ITempAD[i].GUID);
            }
            //[暫使用者AD資料] - DB.GUID
            List<string> ListDB_GUID = new List<string>();
            for (int i = 0; i < ITempDB.Count; i++)
            {
                ListDB_GUID.Add(ITempDB[i].GUID);
            }
            ////[暫使用者AD資料] - DB.DisplayName
            //List<string> ListDB_DispName = new List<string>();
            //for (int i = 0; i < ITempDB.Count; i++)
            //{
            //    ListDB_DispName.Add(ITempDB[i].DisplayName);
            //}
            ////[暫使用者AD資料] - DB.Dept
            //List<string> ListDB_Dept = new List<string>();
            //for (int i = 0; i < ITempDB.Count; i++)
            //{
            //    ListDB_Dept.Add(ITempDB[i].Department);
            //}
            ////[暫使用者AD資料] - DB.userAccountControl
            //List<string> ListDB_AccCtrl = new List<string>();
            //for (int i = 0; i < ITempDB.Count; i++)
            //{
            //    ListDB_AccCtrl.Add(ITempDB[i].userAccountControl);
            //}

            //[宣告暫存] - 新增/更新/刪除
            List<TempADData> IListAdd = new List<TempADData>();
            List<TempADData> IListUpd = new List<TempADData>();
            List<TempADData> IListDel = new List<TempADData>();
            List<TempADData> IListUpd_Dept = new List<TempADData>();
            List<TempADData> IListUpd_AccCtrl = new List<TempADData>();
            List<TempADData> IListUpd_Email = new List<TempADData>();

            #region --Update判斷--
            //[資料篩選] - AD to DB
            //AD資料存在於DB -> Update, 暫使用者Update參數區
            //1.搜尋GUID已存在於DB的資料, 為比對資料的Base
            var queryUpdate = from el in ITempAD
                              where ListDB_GUID.Contains(el.GUID)
                              select new
                              {
                                  GUID = el.GUID,
                                  DisplayName = el.DisplayName,
                                  AccountName = el.AccountName,
                                  Department = el.Department,
                                  userAccountControl = el.userAccountControl,
                                  Email = string.IsNullOrEmpty(el.Email) ? "" : el.Email
                              };

            //1-1.比對DisplayName(名稱)不相同的資料
            for (int i = 0; i < ITempDB.Count; i++)
            {
                //AD人員Loop
                var compareUpdate = from el in queryUpdate
                                    where (el.GUID.Equals(ITempDB[i].GUID) && !el.DisplayName.Equals(ITempDB[i].DisplayName))
                                    select new
                                    {
                                        GUID = el.GUID,
                                        DisplayName = el.DisplayName,
                                        AccountName = el.AccountName,
                                        Email = el.Email
                                    };
                foreach (var item in compareUpdate)
                {
                    IListUpd.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName));
                }
            }

            //1-2.比對Department(部門)不相同的資料
            for (int i = 0; i < ITempDB.Count; i++)
            {
                //AD人員Loop
                var compareUpdate_Dept = from el in queryUpdate
                                         where (el.GUID.Equals(ITempDB[i].GUID) && !el.Department.Equals(ITempDB[i].Department))
                                         select new
                                         {
                                             GUID = el.GUID,
                                             DisplayName = el.DisplayName,
                                             AccountName = el.AccountName,
                                             Department = el.Department
                                         };
                foreach (var item in compareUpdate_Dept)
                {
                    IListUpd_Dept.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName, item.Department));
                }
            }

            //1-3.比對userAccountControl(帳戶類型)不相同的資料
            for (int i = 0; i < ITempDB.Count; i++)
            {
                //AD人員Loop
                var compareUpdate_AccCtrl = from el in queryUpdate
                                            where (el.GUID.Equals(ITempDB[i].GUID) && !el.userAccountControl.Equals(ITempDB[i].userAccountControl))
                                            select new
                                            {
                                                GUID = el.GUID,
                                                DisplayName = el.DisplayName,
                                                AccountName = el.AccountName,
                                                Department = el.Department,
                                                userAccountControl = el.userAccountControl
                                            };
                foreach (var item in compareUpdate_AccCtrl)
                {
                    IListUpd_AccCtrl.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName, item.Department, item.userAccountControl));
                }
            }

            //1-4.比對Email不相同的資料
            for (int i = 0; i < ITempDB.Count; i++)
            {
                //AD人員Loop
                var compareUpdate_Email = from el in queryUpdate
                                          where (el.GUID.Equals(ITempDB[i].GUID) && !el.Email.Equals(ITempDB[i].Email))
                                          select new
                                          {
                                              GUID = el.GUID,
                                              Email = el.Email
                                          };
                foreach (var item in compareUpdate_Email)
                {
                    IListUpd_Email.Add(new TempADData(item.GUID, item.Email));
                }
            }
            #endregion

            #region --Insert判斷--
            //AD資料不存在於DB -> Insert, 暫使用者Insert參數區
            var queryAdd = from el in ITempAD
                           where !(ListDB_GUID.Contains(el.GUID))
                           select new
                           {
                               GUID = el.GUID,
                               DisplayName = el.DisplayName,
                               AccountName = el.AccountName,
                               Department = el.Department,
                               userAccountControl = el.userAccountControl,
                               Email = el.Email
                           };
            foreach (var item in queryAdd)
            {
                IListAdd.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName, item.Department, item.userAccountControl, item.Email));
            }
            #endregion

            #region --Delete判斷--
            //[資料篩選] - DB to AD
            //DB資料不存在於AD -> Delete, 暫使用者Delete參數區
            var queryDel = from el in ITempDB
                           where !ListAD_GUID.Contains(el.GUID)
                           select new
                           {
                               GUID = el.GUID,
                               DisplayName = el.DisplayName,
                               AccountName = el.AccountName
                           };
            foreach (var item in queryDel)
            {
                IListDel.Add(new TempADData(item.GUID, item.DisplayName, item.AccountName));
            }
            #endregion

            #endregion

            #region **開始執行資料更新**
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                #region - [SQL]:更新 Display_Name -
                if (IListUpd.Count > 0)
                {
                    try
                    {
                        //[初始化]
                        cmd.Parameters.Clear();
                        SBSql.Clear();

                        //[SQL] - 宣告New ID
                        SBSql.AppendLine(" Declare @Log_ID AS INT ");

                        for (int i = 0; i < IListUpd.Count; i++)
                        {
                            //[SQL] - 資料更新
                            SBSql.AppendLine(string.Format(
                                " UPDATE User_Profile SET Display_Name = N'{1}', Update_Time = GETDATE() WHERE (Guid = '{0}'); "
                                , IListUpd[i].GUID
                                , IListUpd[i].DisplayName
                                ));

                            //[SQL] - 寫入Log
                            SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                            SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                            SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                            SBSql.AppendLine(" ) VALUES ( ");
                            SBSql.Append(" @Log_ID, 'User', 'Update'");
                            SBSql.Append(string.Format(
                                ", '{0}', N'修改全名, {2}', N'{1}'"
                                , IListUpd[i].AccountName
                                , fn_Params.UserAccount
                                , IListUpd[i].DisplayName));
                            SBSql.AppendLine(" );");
                        }
                        //[SQL] - CommandText
                        cmd.CommandText = SBSql.ToString();
                        //[執行SQL]
                        if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                        {
                            ErrNum++;
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("無法更新Display_Name");
                    }

                }
                #endregion

                #region - [SQL]:更新 Department -
                if (IListUpd_Dept.Count > 0)
                {
                    try
                    {
                        //[初始化]
                        cmd.Parameters.Clear();
                        SBSql.Clear();

                        //[SQL] - 宣告New ID
                        SBSql.AppendLine(" Declare @Log_ID AS INT ");

                        for (int i = 0; i < IListUpd_Dept.Count; i++)
                        {
                            //[SQL] - 資料更新
                            SBSql.AppendLine(string.Format(
                                " UPDATE User_Profile SET DeptID = '{1}', Update_Time = GETDATE() WHERE (Guid = '{0}'); "
                                , IListUpd_Dept[i].GUID
                                , IListUpd_Dept[i].Department
                                ));

                            //[SQL] - 寫入Log
                            SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                            SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                            SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                            SBSql.AppendLine(" ) VALUES ( ");
                            SBSql.Append(" @Log_ID, 'User', 'Update'");
                            SBSql.Append(string.Format(
                                ", '{0}', N'修改部門, {2} ({3})', '{1}'"
                                , IListUpd_Dept[i].AccountName
                                , fn_Params.UserAccount
                                , IListUpd_Dept[i].Department
                                , IListUpd_Dept[i].DisplayName));
                            SBSql.AppendLine(" );");
                        }
                        //[SQL] - CommandText
                        cmd.CommandText = SBSql.ToString();

                        //[執行SQL]
                        if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                        {
                            ErrNum++;
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("無法更新Department");
                    }

                }
                #endregion

                #region - [SQL]:更新 userAccountControl -
                if (IListUpd_AccCtrl.Count > 0)
                {
                    try
                    {
                        //[初始化]
                        cmd.Parameters.Clear();
                        SBSql.Clear();

                        //[SQL] - 宣告New ID
                        SBSql.AppendLine(" Declare @Log_ID AS INT ");

                        for (int i = 0; i < IListUpd_AccCtrl.Count; i++)
                        {
                            //[SQL] - 資料更新
                            SBSql.AppendLine(string.Format(
                                " UPDATE User_Profile SET Display = '{1}', Update_Time = GETDATE() WHERE (Guid = '{0}'); "
                                , IListUpd_AccCtrl[i].GUID
                                , IListUpd_AccCtrl[i].userAccountControl
                                ));

                            //[SQL] - 寫入Log
                            SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                            SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                            SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                            SBSql.AppendLine(" ) VALUES ( ");
                            SBSql.Append(" @Log_ID, 'User', 'Update'");
                            SBSql.Append(string.Format(
                                ", '{0}', '設定帳戶狀態, {2} ({3})', '{1}'"
                                , IListUpd_AccCtrl[i].AccountName
                                , fn_Params.UserAccount
                                , (IListUpd_AccCtrl[i].userAccountControl.Equals("N")) ? "停用" : "啟用"
                                , IListUpd_AccCtrl[i].DisplayName));
                            SBSql.AppendLine(" );");
                        }
                        //[SQL] - CommandText
                        cmd.CommandText = SBSql.ToString();
                        //[執行SQL]
                        if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                        {
                            ErrNum++;
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("無法更新userAccountControl");
                    }

                }
                #endregion

                #region - [SQL]:更新 Email -
                if (IListUpd_Email.Count > 0)
                {
                    try
                    {
                        //[初始化]
                        cmd.Parameters.Clear();
                        SBSql.Clear();

                        for (int i = 0; i < IListUpd_Email.Count; i++)
                        {
                            string email = IListUpd_Email[i].Email;
                            string getMailName = IListAdd[i].AccountName;

                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                string[] strAry = email.Split('@');
                                getMailName = strAry[0];
                            }

                            //[SQL] - 資料更新
                            SBSql.AppendLine(string.Format(
                                " UPDATE User_Profile SET Email = '{1}', NickName = '{2}', Update_Time = GETDATE() WHERE (Guid = '{0}'); "
                                , IListUpd_Email[i].GUID
                                , email
                                , getMailName
                                ));
                        }

                        //[SQL] - CommandText
                        cmd.CommandText = SBSql.ToString();
                        //[執行SQL]
                        if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                        {
                            ErrNum++;
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("無法更新Email");
                    }

                }
                #endregion

                #region - [SQL]:新增Account -
                if (IListAdd.Count > 0)
                {
                    string accoutName = "";
                    string debugstr = "";

                    try
                    {
                        //[初始化]
                        cmd.Parameters.Clear();
                        SBSql.Clear();

                        //[SQL] - 宣告New ID
                        SBSql.AppendLine(" Declare @Log_ID AS INT ");

                        for (int i = 0; i < IListAdd.Count; i++)
                        {
                            string getMailName = IListAdd[i].AccountName;
                            string email = IListAdd[i].Email;
                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                string[] strAry = email.Split('@');
                                getMailName = strAry[0];
                            }

                            accoutName += IListAdd[i].AccountName + ",";

                            //[SQL] - 資料新增
                            SBSql.AppendLine(string.Format(
                                " IF (SELECT COUNT(*) FROM User_Profile WHERE (Guid = '{0}')) = 0 " +
                                " BEGIN " +
                                "  INSERT INTO User_Profile(Guid, Display_Name, Account_Name, DeptID, Display, Email, NickName)" +
                                "  VALUES ('{0}', N'{1}', '{2}', '{3}', '{4}', N'{5}', N'{6}'); "
                                , IListAdd[i].GUID
                                , IListAdd[i].DisplayName
                                , IListAdd[i].AccountName
                                , IListAdd[i].Department
                                , IListAdd[i].userAccountControl
                                , email
                                , getMailName
                                ));
                            debugstr = "{0}, {1}, {2}, {3}, {4}, {5}, {6}".FormatThis(
                                IListAdd[i].GUID
                                , IListAdd[i].DisplayName
                                , IListAdd[i].AccountName
                                , IListAdd[i].Department
                                , IListAdd[i].userAccountControl
                                , email
                                , getMailName
                                );
                            //[SQL] - 寫入Log
                            SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                            SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                            SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                            SBSql.AppendLine(" ) VALUES ( ");
                            SBSql.Append(" @Log_ID, 'User', 'Insert'");
                            SBSql.Append(string.Format(
                                ", '{0}', N'新增使用者, {2}', '{1}'"
                                , IListAdd[i].AccountName
                                , fn_Params.UserAccount
                                , IListAdd[i].DisplayName));
                            SBSql.AppendLine(" );");
                            SBSql.AppendLine("END");
                        }

                        //[SQL] - CommandText
                        cmd.CommandText = SBSql.ToString();

                        //[執行SQL]
                        if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                        {
                            ErrNum++;
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("無法新增Account:" + debugstr);
                    }


                }
                #endregion


                #region - [SQL]:刪除(停用) -
                if (IListDel.Count > 0)
                {
                    try
                    {
                        //[初始化]
                        cmd.Parameters.Clear();
                        SBSql.Clear();

                        //[SQL] - 宣告New ID
                        SBSql.AppendLine(" Declare @Log_ID AS INT ");

                        for (int i = 0; i < IListDel.Count; i++)
                        {
                            //[SQL] - 資料刪除, 刪除相關權限, 更新為已停用
                            SBSql.AppendLine(string.Format(
                                " DELETE FROM PKEF.dbo.User_Profile_Rel_Program WHERE (Guid = '{0}');" +
                                " UPDATE User_Profile SET Display = 'N', Update_Time = GETDATE() WHERE (Guid = '{0}');"
                                , IListDel[i].GUID));

                            //[SQL] - 寫入Log
                            SBSql.AppendLine(" SET @Log_ID = (SELECT ISNULL(MAX(Log_ID), 0) + 1 FROM Log_ADSync) ");
                            SBSql.AppendLine(" INSERT INTO Log_ADSync( ");
                            SBSql.AppendLine("  Log_ID, Proc_Type, Proc_Action, Proc_Account, Proc_Desc, Create_Who");
                            SBSql.AppendLine(" ) VALUES ( ");
                            SBSql.AppendLine(" @Log_ID, 'User', 'Delete'");
                            SBSql.Append(string.Format(
                                ", '{0}', '停用使用者, {2} (若要啟用需至權限設定手動啟用)', '{1}'"
                                , IListDel[i].AccountName
                                , fn_Params.UserAccount
                                , IListDel[i].DisplayName));
                            SBSql.AppendLine(" );");
                        }
                        //[SQL] - CommandText
                        cmd.CommandText = SBSql.ToString();
                        //[執行SQL]
                        if (false == dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                        {
                            ErrNum++;
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("無法刪除(停用)");
                    }

                }
                #endregion
            }
            #endregion

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            ErrNum = 1;
            return false;
        }

    }

    /// <summary>
    /// 暫存資料
    /// </summary>
    public class TempADData
    {
        #region "參數設定"
        /// <summary>
        /// GUID
        /// </summary>
        private string _GUID;
        public string GUID
        {
            get { return this._GUID; }
            set { this._GUID = value; }
        }

        /// <summary>
        /// DisplayName (名稱)
        /// </summary>
        private string _DisplayName;
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        /// <summary>
        /// AccountName (工號)
        /// </summary>
        private string _AccountName;
        public string AccountName
        {
            get { return this._AccountName; }
            set { this._AccountName = value; }
        }

        /// <summary>
        /// department (部門)
        /// </summary>
        private string _Department;
        public string Department
        {
            get { return this._Department; }
            set { this._Department = value; }
        }

        /// <summary>
        /// userAccountControl (帳戶類型)
        /// </summary>
        /// [2+512]=已停用, 512=預設類型, xxx=其他
        /// 因 514 為確定的類型，故以 514 為判斷依據
        private string _userAccountControl;
        public string userAccountControl
        {
            get { return this._userAccountControl; }
            set { this._userAccountControl = value; }
        }

        /// <summary>
        /// mail
        /// </summary>
        private string _Email;
        public string Email
        {
            get { return this._Email; }
            set { this._Email = value; }
        }
        #endregion

        public TempADData(string GUID, string Email)
        {
            this._GUID = GUID;
            this._Email = Email;
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="GUID">AD GUID</param>
        /// <param name="DisplayName">AD DisplayName</param>
        /// <param name="AccountName">AD saMaccountName</param>
        /// <param name="Department">AD department</param>
        public TempADData(string GUID, string DisplayName, string AccountName)
        {
            this._GUID = GUID;
            this._DisplayName = DisplayName;
            this._AccountName = AccountName;
        }

        public TempADData(string GUID, string DisplayName, string AccountName, string Department)
        {
            this._GUID = GUID;
            this._DisplayName = DisplayName;
            this._AccountName = AccountName;
            this._Department = Department;
        }

        public TempADData(string GUID, string DisplayName, string AccountName, string Department, string userAccountControl)
        {
            this._GUID = GUID;
            this._DisplayName = DisplayName;
            this._AccountName = AccountName;
            this._Department = Department;
            this._userAccountControl = userAccountControl;
        }
        public TempADData(string GUID, string DisplayName, string AccountName, string Department, string userAccountControl, string Email)
        {
            this._GUID = GUID;
            this._DisplayName = DisplayName;
            this._AccountName = AccountName;
            this._Department = Department;
            this._userAccountControl = userAccountControl;
            this._Email = Email;
        }
    }

    /// <summary>
    /// 產生MD5驗証碼
    /// SessionID + 登入帳號 + 自訂字串
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get
        {
            return Cryptograph.MD5(
                Session.SessionID + Session["Login_UserID"] + System.Web.Configuration.WebConfigurationManager.AppSettings["ValidCode_Pwd"]);
        }
        private set
        {
            this._ValidCode = value;
        }
    }
}