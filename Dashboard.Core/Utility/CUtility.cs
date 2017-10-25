using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using log4net;
using System.Reflection;
using Dashboard.Core.Log4netHelper;
using System.Web;
using System.Runtime.Caching;

namespace Dashboard.Core.Utility
{
    public static class CUtility
    {
        #region GetFileName
        /// <summary>
        /// 得到一个文件名，如：CN_2011112216370099
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetFileName(string type)
        {
            Random rd = new Random();
            return type + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + rd.Next(1000);
        }
        #endregion

        #region GetJsonString
        /// <summary>
        /// Serialize返回JSON字符串
        /// </summary>
        /// <param name="eEntity"></param>
        /// <returns></returns>
        public static string GetJsonString(object eEntity)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            new JavaScriptSerializer().Serialize(eEntity, sb);
            return sb.ToString();
        }
        #endregion

        #region ConvertEnumToDictionary
        public static Dictionary<int, string> ConvertEnumToDictionary(Type type)
        {
            return Enum.GetValues(type).Cast<object>().ToDictionary(e => (int)e, e => Enum.GetName(type, e));
        }
        #endregion

        public static object OpenPDFInBrowser(byte[] streamBytes)
        {
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
            System.Web.HttpContext.Current.Response.BinaryWrite(streamBytes);//pdfPath
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.Close();
            return null;
        }

        public static bool WebServerExist(string url)
        {
            //try
            //{
            //    string refer = url.Substring(0, url.LastIndexOf("/") + 1);
            //    System.Net.HttpWebRequest req = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
            //    req.AllowAutoRedirect = true;
            //    req.Referer = refer;
            //    //req.Timeout = int.MaxValue;//Convert.ToInt32(AppConfig.GetConfigValue("Timeout"));
            //    req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
            //    System.Net.HttpWebResponse res = req.GetResponse() as System.Net.HttpWebResponse;
            //    System.IO.Stream stream = res.GetResponseStream();
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Log4netHelper.Log4netHelper.LoadADONetAppender();
            //    ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //    log.Info("CarrierServer is disabled ", ex);
            //    return false;
            //}
            return true;
        }

        public static string GetLocalizationByKey(string strKey, string strControllerName)
        {
            try
            {
                HttpCookie lang = System.Web.HttpContext.Current.Request.Cookies["Lang"];
                string strLang = string.Empty;
                if (lang == null)
                {
                    string langFromBrowser = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
                    if (string.Compare("zh-CN", langFromBrowser, true) == 0)
                    {
                        strLang = "SimplifiedChinese";
                    }
                    else if (langFromBrowser.Contains("en"))
                    {
                        strLang = "English";
                    }
                    else
                    {
                        strLang = "English";
                    }
                }
                else
                    strLang = lang.Value;

                if (strLang == "English")
                    strControllerName = string.Format("{0}.en.xml", strControllerName);
                else
                    strControllerName = string.Format("{0}.zh-cn.xml", strControllerName);

                ObjectCache cache = MemoryCache.Default;
                if (cache[strControllerName] != null)
                {
                    var resource = (Dictionary<string, string>)cache[strControllerName];
                    return resource.FirstOrDefault(r => r.Key.ToLower() == strKey.ToLower()).Value;
                }
                else
                    return strKey;
            }
            catch
            {
                return strKey;
            }
        }
    }
}
