using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Data;
using System.Collections;
using System.Reflection;

namespace Dashboard.Core.Utility
{
    public static class StaticExtension
    {
        public static bool IsNullString(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        public static bool IsPhone(this string value)
        {
            Regex reg = new Regex(@"(-|\s|\d|\*)$");
            return reg.IsMatch(value);
        }

        /// <summary>
        /// 两位小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFloat(this string value)
        {
            Regex reg = new Regex(@"\d+(\.\d{1,6})?$");
            return reg.IsMatch(value);
        }

        /// <summary>
        /// 邮编格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsZip(this string value)
        {
            Regex reg = new Regex(@"^[1-9]\d{5}(?!\d)$");
            return reg.IsMatch(value);
        }

        /// <summary>
        /// 区号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDistrictNum(this string value)
        {
            Regex reg = new Regex(@"^\d{3,4}$");
            return reg.IsMatch(value);
        }


        /// <summary>
        /// 用户密码验证：必须同时包含数字、大小写字母、特殊字符且长度为8
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidPassWord(this string value)
        {
            Regex lowCaseReg = new Regex(@"[a-z]+");
            bool lowCase = lowCaseReg.IsMatch(value);

            Regex upperCaseReg = new Regex(@"[A-Z]+");
            bool upperCase = upperCaseReg.IsMatch(value);

            Regex numberReg = new Regex(@"[0-9]+");
            bool number = numberReg.IsMatch(value);

            Regex specialCaseReg = new Regex(@"\W+|_");
            bool specialCase = specialCaseReg.IsMatch(value);

            return lowCase && upperCase && specialCase && number && value.Length == 8;
        }

    }

    public static class StaticIQueryableExtension
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, string sOrd, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            string orderDirection = string.Compare(sOrd, "asc", true) == 0 ? "OrderBy" : "OrderByDescending";
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), orderDirection, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }
    }

}
