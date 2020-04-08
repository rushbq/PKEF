using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Security.Principal;
using System.Net;

namespace ReportConn
{
    public sealed class MyReportServerConn : IReportServerConnection2
    {
        //AD - 帳號
        private string _userName = System.Web.Configuration.WebConfigurationManager.AppSettings["ReportADUser"];
        //AD - 密碼
        private string _userPWD = System.Web.Configuration.WebConfigurationManager.AppSettings["ReportADPwd"];
        //AD網域
        private string _domain = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_Domain"];
        //ReportServer路徑
        private string _reportServerUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServerUrl"];


        #region IReportServerConnection2 成員

        public IEnumerable<System.Net.Cookie> Cookies
        {
            get { return null; }
        }

        public IEnumerable<string> Headers
        {
            get { return null; }
        }

        #endregion


        #region IReportServerConnection 成員

        public Uri ReportServerUrl
        {
            get
            {
                string tmpUrl = _reportServerUrl;

                return new Uri(tmpUrl);
            }
        }

        public int Timeout
        {
            get
            {
                return 60000; // 60 seconds
            }
        }

        #endregion


        #region IReportServerCredentials 成員

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;

            userName = null;

            password = null;

            authority = null;

            // Not using form credentials
            return false;
        }

        public WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                string tmpUserName = _userName;

                // Password
                string tmpPassword = _userPWD;

                // Domain
                string tmpDomain = _domain;

                return new NetworkCredential(tmpUserName, tmpPassword, tmpDomain);

            }
        }

        #endregion

    }
}