using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web;

namespace Dashboard.Core.Utility
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString DatePicker(this HtmlHelper htmlHelper, string name, Object value,
                        IDictionary<string, object> htmlAttributes, string datePickerParameters)
        {
            string strLang = string.Empty;
            if (HttpContext.Current.Request.Cookies["Lang"] == null)
                strLang = "en";
            else
                strLang = HttpContext.Current.Request.Cookies["Lang"].Value == "English" ? "en" : "zh-cn";

            if (String.IsNullOrEmpty(datePickerParameters))
                datePickerParameters = String.Format("lang:'{0}'", strLang);
            else if (datePickerParameters.ToLower().IndexOf("lang") < 0)
                datePickerParameters += String.Format(",lang:'{0}'", strLang);
            datePickerParameters += String.Format(",el:$dp.$('{0}')", name);
            string onClickAtteibute = "WdatePicker({" + datePickerParameters + "})";

            KeyValuePair<string, object> keyValue = htmlAttributes.FirstOrDefault(d => string.Compare(d.Key, "class", true) == 0);
            htmlAttributes["class"] = "text Wdate";

            htmlAttributes.Add("onFocus", onClickAtteibute);
            try
            {

                //return MvcHtmlString.Create(String.Format("{0}{1}{2}{3}",
                //    InputExtensions.TextBox(htmlHelper, name, (value != null ? ((DateTime)value).ToString("yyyy-MM-dd") : value), htmlAttributes),
                //    "<em class='date' onclick = ", onClickAtteibute, "></em>"));

                return MvcHtmlString.Create(
                  InputExtensions.TextBox(htmlHelper, name, (value != null ? ((DateTime)value).ToString("yyyy-MM-dd") : value), htmlAttributes).ToString()
                  );
            }
            catch { return null; }
        }
    }
}
