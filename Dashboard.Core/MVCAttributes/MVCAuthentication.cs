using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using System.Web;

namespace Dashboard.Core.MVCAttributes
{
    //public class CustomAuthorizeAttribute : AuthorizeAttribute
    //{
    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        bool result = false;
    //        if (httpContext.User.Identity.IsAuthenticated)
    //        {
    //            result = true;
    //        }
    //        else
    //        {
    //            httpContext.Response.StatusCode = 403;
    //            return false;
    //        }

    //        return result;
    //    }

    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        base.OnAuthorization(filterContext);
    //        if (filterContext.HttpContext.Response.StatusCode == 403)
    //        {
    //            //filterContext.HttpContext.Response.Write("无权限。。。");
    //            filterContext.Result = new RedirectResult("/Account/LogOn");
    //        }
    //    }

    //}
   
}
