using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Caching;
using System.Xml.Linq;
using System.Web;

namespace Dashboard.Core.Localization
{
    public class LocalizationHandler : Stream
    {

        private Stream responseStream;

        private string _strLanguage;

        private StringBuilder responseHtml;

        public LocalizationHandler(Stream inputStream, string strLanguage)
        {
            responseStream = inputStream;
            _strLanguage = strLanguage;
            responseHtml = new StringBuilder();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            responseStream.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        long postion;
        public override long Position
        {
            get
            {
                return postion;
            }
            set
            {
                postion = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return responseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return responseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            responseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string sBuffer = System.Text.UTF8Encoding.UTF8.GetString(buffer, offset, count);
            //string pattern = @"(&lt;|<)=(.*?)(>|&gt;)";

            string pattern = @"(\\u003c|&lt;|<)=(.*?)(>|&gt;|\\u003e)";//正则替换类似页面格式为这样的字符串如：<=OtherContent>

            Dictionary<string, string> dicData = null;
            switch (_strLanguage)
            {
                case "SimplifiedChinese":
                    dicData = ReadSimpResource();
                    break;
                case "English":
                    dicData = ReadEnResource();
                    break;
                default:
                    dicData = ReadEnResource();
                    break;
            }

            if (HttpContext.Current.Response.ContentType == "text/html")
            {
                Regex eof = new Regex("</html>", RegexOptions.IgnoreCase);
                if (!eof.IsMatch(sBuffer))
                {

                    if (HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("print"))
                    {
                        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(sBuffer);
                        responseStream.Write(data, 0, data.Length);
                    }
                    else
                        responseHtml.Append(sBuffer);
                }
                else
                {
                    responseHtml.Append(sBuffer);

                    string finalHtml = responseHtml.ToString();

                    sBuffer = Regex.Replace(finalHtml, pattern, delegate(Match c)
                    {
                        string value = dicData.FirstOrDefault(d => d.Key == c.Groups[2].Value).Value;
                        return value;
                    });

                    byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(sBuffer);

                    responseStream.Write(data, 0, data.Length);
                }
            }
            else
            {

                sBuffer = Regex.Replace(sBuffer, pattern, delegate(Match c)
                {
                    string value = dicData.FirstOrDefault(d => d.Key == c.Groups[2].Value).Value;
                    return value;
                });

                byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(sBuffer);
                responseStream.Write(data, 0, data.Length);
            }
        }

        ObjectCache cache = MemoryCache.Default;
        private Dictionary<string, string> ReadEnResource()
        {
            string resourcePath = "";

            Dictionary<string, string> cacheData = null;
            if (cacheData != null)
            {
                return cacheData;
            }
            Dictionary<string, string> cachedData = new Dictionary<string, string>();

            System.Web.HttpContext.Current.Server.MapPath("~");
            string serverPath = System.Web.HttpContext.Current.Server.MapPath("~");

            string fileKey = GetResFileName();
            string resourceName = string.Format("{0}.en.xml", fileKey); //"EnResource.xml";

            resourcePath = Path.Combine(serverPath, string.Format(@"Resources\{0}", resourceName));

            if (!System.IO.File.Exists(resourcePath))
                resourcePath = Path.Combine(serverPath, string.Format(@"Resources\{0}", "Resource.en.xml"));

            //建立缓存（使用.net4.0最新缓存机制：System.Runtime.Caching;）
            if (cache[resourceName] == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = TimeSpan.FromMinutes(60);
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { resourcePath }));

                var items = XElement.Load(resourcePath).Elements("Module").Elements("item");
                foreach (var item in items)
                {
                    string key = item.Attribute("name").Value;
                    string value = item.Value;
                    cachedData.Add(key, value);
                }
                cache.Set(resourceName, cachedData, policy);

                return cachedData;

            }

            return (Dictionary<string, string>)cache[resourceName];
        }

        private Dictionary<string, string> ReadSimpResource()
        {
            string resourcePath = "";

            Dictionary<string, string> cacheData = null;
            if (cacheData != null)
            {
                return cacheData;
            }
            Dictionary<string, string> cachedData = new Dictionary<string, string>();

            string serverPath = System.Web.HttpContext.Current.Server.MapPath("~");

            //string fileName = GetResFileName();
            //string resourceName = "zh-CNResource.xml";

            string fileKey = GetResFileName();
            string resourceName = string.Format("{0}.zh-cn.xml", fileKey); //"EnResource.xml";

            resourcePath = Path.Combine(serverPath, string.Format(@"Resources\{0}", resourceName));

            if (!System.IO.File.Exists(resourcePath))
                resourcePath = Path.Combine(serverPath, string.Format(@"Resources\{0}", "Resource.zh-cn.xml"));

            //建立缓存（使用.net4.0最新缓存机制：System.Runtime.Caching;）
            if (cache[resourceName] == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = TimeSpan.FromMinutes(60);
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { resourcePath }));

                var items = XElement.Load(resourcePath).Elements("Module").Elements("item");
                foreach (var item in items)
                {
                    string key = item.Attribute("name").Value;
                    string value = item.Value;
                    cachedData.Add(key, value);
                }
                cache.Set(resourceName, cachedData, policy);

                return cachedData;

            }

            return (Dictionary<string, string>)cache[resourceName];
        }

        /// <summary>
        /// 获取资源文件
        /// </summary>
        /// <returns></returns>
        private string GetResFileName()
        {
            string filePath = HttpContext.Current.Request.FilePath;
            if (filePath[0] == '/') filePath = filePath.Substring(1, filePath.Length - 1);
            string[] words = filePath.Split(new char[] { '/' });

            if (words.Last().Contains("."))
                return "Resource";

            string filename = words[0];
            if (string.IsNullOrEmpty(filename))
                filename = "home";
            return filename;
        }
    }
}
