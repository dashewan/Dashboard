using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using log4net;
using System.Reflection;


namespace Dashboard.Core.MVCAttributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext Context)
        {
            Dashboard.Core.Log4netHelper.Log4netHelper.LoadADONetAppender();

            base.OnException(Context);
            Exception ex = Context.Exception;
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            string controller = Context.RouteData.Values["controller"].ToString();
            string action = Context.RouteData.Values["action"].ToString();
            log.Info(string.Format("[controller:]{0}[action:]{1}", controller, action), ex);

            //if (!Context.ExceptionHandled)
            //    return;
            ////记录错误信息
            //Dashboard.Core.ExceptionLog.ExceptionLog.RecordExceptionLog(ex);
        }
    }
}
