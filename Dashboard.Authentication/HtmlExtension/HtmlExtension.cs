using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Dashboard.Interface.MasterData.SysManagement;
using Dashboard.Core.Enumeration;
using System.Reflection;
using Dashboard.Domain.SysManagement;
using System.Web.Mvc.Html;

namespace Dashboard.Authentication.HtmlExtension
{
    public static class HtmlExtension
    {
        public static MvcHtmlString GenerateMenu(this HtmlHelper htmlHelper)
        {
            StringBuilder sb = new StringBuilder();
            string s = "Dashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
            ISysFunction<SysFunction> service = (ISysFunction<SysFunction>)Assembly.Load("Dashboard.Service").CreateInstance(s);

            var userRoleId = Authentication.CurrentUser.SysRole.SysRoleId;

            List<FunctionGroupType> groupTypes = (from g in service.GetMenuGroup(userRoleId)
                                                  select (FunctionGroupType)g).ToList();

            foreach (var group in groupTypes)
            {
                sb.Append("<div class='accordionHeader'>");
                sb.Append("<h2>");
                sb.Append(string.Format("<span class='Folder_{0}'>Folder</span><={0}></h2>", group.ToString(), group.ToString()));
                sb.Append("</div>");
                /*分组结束*/
                sb.Append("<div class='accordionContent'>");
                sb.Append(" <ul class='tree'>");

                foreach (var m in service.GetMenuFromGroup(userRoleId, (int)group))
                {
                    string url = string.Format("<a href='/{0}/{1}' target='navTab' rel='{2}' fresh='false' external='true' ><div class='{3}'></div><={4}.{5}></a>"
                                            , m.ControllerName, m.ActionName, m.SysFunctionId, m.ControllerName + '_' + m.ActionName, m.ControllerName, m.ActionName);
                    sb.Append(string.Format("<li>{0}</li>", url));
                }
                sb.Append(" </ul></div>");
            }
            return MvcHtmlString.Create(sb.ToString());

        }

        /// <summary>
        /// 功能权限
        /// </summary>
        /// <param name="controller">按钮所在的controller</param>
        /// <param name="action">按钮所在的action</param>
        /// <param name="id">按钮的html id</param>
        /// <param name="value">按钮的名称</param>
        /// <returns></returns>
        public static MvcHtmlString GenerateButton(this HtmlHelper htmlHelper, string controller, string action,
                                                   string id, string value)
        {
            StringBuilder sb = new StringBuilder();
            string s = "Dashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
            ISysFunction<SysFunction> service = (ISysFunction<SysFunction>)Assembly.Load("Dashboard.Service").CreateInstance(s);

            var userRoleId = Authentication.CurrentUser.RoleId;
            if (service.GetUserFunction(userRoleId).Any(c => string.Compare(c.ControllerName, controller, true) == 0 &&
                           string.Compare(c.ActionName, action, true) == 0))
            {
                sb.Append(string.Format("<input type='button' class='button' id='{0}' value='{1}' />", id, value));
            }
            else
            {
                //sb.Append(string.Format("<input type='button' class='button' id='{0}' value='{1}' disabled= 'disabled'/>", id, value));
                sb.Append(string.Format("<input type='button' class='button' id='{0}' value='{1}' style= 'display:none;'/>", id, value));
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
