using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Dashboard.Core.ExceptionLog
{
    public class ExceptionLog : ApplicationException
    {
        public ExceptionLog(string message)
            : base(message)
        {
            Init();
            Log();
        }

        public ExceptionLog(Exception inner)
        {
            ex = inner;
            Init();
            Log();
        }

        public static void RecordExceptionLog(Exception inner)
        {
            ExceptionLog exceptionLog = new ExceptionLog(inner);
            exceptionLog.ex = inner;
            exceptionLog.Init();
            exceptionLog.Log();
        }

        public Exception ex = null;

        public override string Message
        {
            get
            {
                string msg = base.Message;
                if (ex != null)
                {
                    msg = ex.Message;
                }
                return msg;
            }

        }

        public string UserName
        {
            get
            {
                string username = HttpContext.Current.User == null ?
                          string.Empty : HttpContext.Current.User.Identity.Name;
                return username;
            }

        }

        string userAgent = string.Empty;
        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        string ipAddress = string.Empty;
        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        string httpReferrer = string.Empty;
        public string HttpReferrer
        {
            get { return httpReferrer; }
            set { httpReferrer = value; }
        }

        string httpVerb = string.Empty;
        public string HttpVerb
        {
            get { return httpVerb; }
            set { httpVerb = value; }
        }

        string httpPathAndQuery = string.Empty;
        public string HttpPathAndQuery
        {
            get { return httpPathAndQuery; }
            set { httpPathAndQuery = value; }
        }

        DateTime dateCreated;
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

        void Init()
        {
            DateCreated = DateTime.Now;
            if (HttpContext.Current.Request.UrlReferrer != null)
                httpReferrer = HttpContext.Current.Request.UrlReferrer.ToString();

            if (HttpContext.Current.Request.UserAgent != null)
                userAgent = HttpContext.Current.Request.UserAgent;

            if (HttpContext.Current.Request.UserHostAddress != null)
                ipAddress = HttpContext.Current.Request.UserHostAddress;

            try
            {
                if (HttpContext.Current.Request != null
                 && HttpContext.Current.Request.RequestType != null)
                    httpVerb = HttpContext.Current.Request.RequestType;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            if (HttpContext.Current.Request != null
             && HttpContext.Current.Request.Url != null
             && HttpContext.Current.Request.Url.PathAndQuery != null)
                httpPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;

            if (HttpContext.Current.Request != null
             && HttpContext.Current.Request.UrlReferrer != null
             && HttpContext.Current.Request.Url.PathAndQuery != null)
                httpReferrer = HttpContext.Current.Request.UrlReferrer.ToString();
        }

        public void Log()
        {
            //var log = new SystemErrorLog
            //{
            //    DateTime = this.DateCreated,
            //    HttpPathAndQuery = this.HttpPathAndQuery,
            //    HttpReferrer = this.HttpReferrer,
            //    HttpVerb = this.HttpVerb,
            //    IPAddress = this.IPAddress,
            //    Message = this.Message,
            //    UserAgent = this.UserAgent,
            //    UserName = this.UserName,
            //};
            //new SystemErrorLogDBController().SaveSystemLogToDabase(log);
        }
    }
}
