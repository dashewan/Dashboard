using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Dashboard.Authentication.MvcAttribute
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = false;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                result = true;
            }
            else
            {
                httpContext.Response.StatusCode = 403;
                return false;
            }

            return result;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            //= filterContext.HttpContext.Request.IsAjaxRequest();
            bool isAjax = filterContext.HttpContext.Request.IsAjaxRequest();

            if (filterContext.HttpContext.Response.StatusCode == 403)
            {
                if (!isAjax)
                {
                    //filterContext.Result = new RedirectResult("/Shared/Error");
                    return;
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.HttpContext.Response.Write("sorry,timeout!");
                    filterContext.HttpContext.Response.End();
                    return;
                }
            }

            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            if (string.Compare(controller, "home", true) == 0 && string.Compare(action, "index", true) == 0)
                return;
            if (string.Compare(filterContext.HttpContext.User.Identity.Name, "admin", true) == 0)
                return;
            //if (!Authentication.HasPermission(controller, action))
            //{
            //    filterContext.Result = new RedirectResult("/Account/LogOn");
            //}
        }
    }
}
