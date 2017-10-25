using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Dashboard.Domain.SysManagement;
using System.Web;
using Dashboard.Interface.MasterData.SysManagement;
using System.Web.Mvc;
using Dashboard.Core.Enumeration;
using System.Web.Mvc.Html;
using Dashboard.Core.Utility;


namespace Dashboard.Authentication
{
    public static class Authentication
    {
        #region Fields
        #endregion

        #region Properties
        public static SysUser CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                {
                    string s = "Dashboard.Service.Services.MasterData.SysManagement.SysUserService";
                    ISysUser<SysUser> service = (ISysUser<SysUser>)Assembly.Load("Dashboard.Service").CreateInstance(s);
                    HttpContext _context = HttpContext.Current;
                    var userCode = _context.User.Identity.Name;
                    userCode = System.Web.HttpContext.Current.User.Identity.Name;
                    //userCode = userCode.Split('\\')[1].ToString();
                    HttpContext.Current.Session["User"] = service.GetUserByCode(userCode);
                }

                return (SysUser)HttpContext.Current.Session["User"];
            }
        }

        public static SysOrganization Organization
        {
            get
            {
                if (HttpContext.Current.Session["Organization"] == null)
                {
                    HttpContext.Current.Session["Organization"] = CurrentUser.SysRole.SysOrganization;
                }
                return (SysOrganization)HttpContext.Current.Session["Organization"];
            }
        }

        public static List<string> CurrentOrgIds
        {
            get
            {
                if (HttpContext.Current.Session["CurrentOrgIds"] == null)
                {
                    string s = "Dashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
                    ISysFunction<SysFunction> service = (ISysFunction<SysFunction>)Assembly.Load("Dashboard.Service").CreateInstance(s);
                    //HttpContext.Current.Session["CurrentOrgIds"] = service.GetOrganizationIds(Organization.SysOrganizationId);
                    HttpContext.Current.Session["CurrentOrgIds"] = service.GetOrganizationIds(CurrentUser.RoleId);
                }
                return (List<string>)HttpContext.Current.Session["CurrentOrgIds"];
            }
        }

        public static List<string> CurrentForwarderCodes
        {
            get
            {
                if (HttpContext.Current.Session["ForwarderCodes"] == null)
                {
                    string s = "Dashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
                    ISysFunction<SysFunction> service = (ISysFunction<SysFunction>)Assembly.Load("Dashboard.Service").CreateInstance(s);
                    HttpContext.Current.Session["ForwarderCodes"] = service.GetForwarderCodes(CurrentUser.RoleId);
                }
                return (List<string>)HttpContext.Current.Session["ForwarderCodes"];
            }
        }

        public static SysRole CurrentUserRole
        {
            get
            {
                return CurrentUser.SysRole;
            }
        }

        #endregion

        #region Methods

        #region Private Methods
        private static Expression<Func<T, bool>> SetEqualCondition<T>(string field, string value)
        {

            var methods = typeof(string).GetMethods();
            var equalMethod = methods.Where(c => c.Name == "Equals").ToArray()[1];

            var parameter = Expression.Parameter(typeof(T), "p");
            var left = Expression.Call(
                Expression.Property(parameter, field),
                equalMethod,
                Expression.Constant(value));
            var lambda = Expression.Lambda<Func<T, bool>>(left, parameter);

            return lambda;
        }

        private static Expression<Func<T, bool>> SetContainCondition<T>(string field, List<string> ids)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var left = Expression.Call(
                Expression.Constant(ids),
                typeof(List<string>).GetMethod("Contains"),
                Expression.Property(parameter, field)
            );
            var lambda = Expression.Lambda<Func<T, bool>>(left, parameter);
            return lambda;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 更新实体的UserID ,OfficeID 等信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void UpdateEntity<T>(T obj)
        {
            var props = typeof(T).GetProperties();

            var keyProp = (from p in props
                           where p.GetIndexParameters().Length == 0 &
                           p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() != null
                           select p).FirstOrDefault();

            object keyValue = keyProp.GetValue(obj, null);

            if (keyValue == null || keyValue.ToString().IsNullString())
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty("CreatedUserId");
                propertyInfo.SetValue(obj, CurrentUser.SysUserId, null);

                propertyInfo = typeof(T).GetProperty("CreatedUserName");
                propertyInfo.SetValue(obj, CurrentUser.UserName, null);

                propertyInfo = typeof(T).GetProperty("CreatedUserCode");
                propertyInfo.SetValue(obj, CurrentUser.UserCode, null);

                propertyInfo = typeof(T).GetProperty("CreatedOfficeId");
                propertyInfo.SetValue(obj, Organization.SysOrganizationId, null);

                propertyInfo = typeof(T).GetProperty("CreatedOfficeCode");
                propertyInfo.SetValue(obj, Organization.OrganizationCode, null);

                propertyInfo = typeof(T).GetProperty("CreatedOfficeName");
                propertyInfo.SetValue(obj, Organization.OrganizationName, null);

                propertyInfo = typeof(T).GetProperty("CreatedDate");
                propertyInfo.SetValue(obj, DateTime.Now, null);

            }
            else
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty("UpdatedUserId");
                propertyInfo.SetValue(obj, CurrentUser.SysUserId, null);

                propertyInfo = typeof(T).GetProperty("UpdatedUserCode");
                propertyInfo.SetValue(obj, CurrentUser.UserCode, null);

                propertyInfo = typeof(T).GetProperty("UpdatedUserName");
                propertyInfo.SetValue(obj, CurrentUser.UserName, null);

                propertyInfo = typeof(T).GetProperty("UpdatedOfficeId");
                propertyInfo.SetValue(obj, Organization.SysOrganizationId, null);

                propertyInfo = typeof(T).GetProperty("UpdatedOfficeCode");
                propertyInfo.SetValue(obj, Organization.OrganizationCode, null);

                propertyInfo = typeof(T).GetProperty("UpdatedOfficeName");
                propertyInfo.SetValue(obj, Organization.OrganizationName, null);

                propertyInfo = typeof(T).GetProperty("UpdatedDate");
                propertyInfo.SetValue(obj, DateTime.Now, null);
            }
        }

        public static IQueryable<T> FilterSource<T>(this IQueryable<T> dataSource, string exteriorCode = "ForwarderCode") where T : class
        {
            IQueryable<T> result = null;
            if (CurrentUser.SysRole.SysOrganization.IsInterior)
            {
                if (CurrentOrgIds != null && CurrentOrgIds.Count > 0)
                    result = dataSource.Where(SetContainCondition<T>("CreatedOfficeId", CurrentOrgIds)).Union
                                        (dataSource.Where(SetEqualCondition<T>("CreatedUserId", CurrentUser.SysUserId)));
                else
                    result = dataSource.Where(SetEqualCondition<T>("CreatedUserId", CurrentUser.SysUserId));
            }
            else
            {
                if (CurrentForwarderCodes != null && CurrentForwarderCodes.Count > 0)
                    result = dataSource.Where(SetContainCondition<T>(exteriorCode, CurrentForwarderCodes));
                else
                    result = dataSource.Where(d => 1 != 1);
            }

            return result;
        }

        public static bool HasPermission(string controller, string action)
        {
            StringBuilder sb = new StringBuilder();
            string s = "Dashboard.Service.Services.MasterData.SysManagement.SysFunctionService";
            ISysFunction<SysFunction> service = (ISysFunction<SysFunction>)Assembly.Load("Dashboard.Service").CreateInstance(s);
            var userRoleId = CurrentUser.RoleId;

            if (service.GetUserPermission(userRoleId).Any(c => string.Compare(c.ControllerName, controller, true) == 0
                                                          && string.Compare(c.ActionName, action, true) == 0))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取当前用户所有角色
        /// </summary>
        /// <returns></returns>
        public static List<SysRole> GetUserRoles()
        {
            List<SysRole> result = (from ur in CurrentUser.SysUserRole
                                    select ur.SysRole).ToList();
            return result;
        }

        public static void SetCurrentUserRole(string sysRoleId)
        {
            CurrentUser.RoleId = sysRoleId;
            CurrentUser.SysRole = GetUserRoles().FirstOrDefault(r => r.SysRoleId == sysRoleId);
            HttpContext.Current.Session["Organization"] = null;
            HttpContext.Current.Session["ForwarderCodes"] = null;
            HttpContext.Current.Session["CurrentOrgIds"] = null;
        }

        public static void LogOff()
        {
            HttpContext.Current.Session["User"] = null;
            HttpContext.Current.Session["Organization"] = null;
            HttpContext.Current.Session["CurrentOrgIds"] = null;
        }

        #endregion

        #endregion
    }
}
