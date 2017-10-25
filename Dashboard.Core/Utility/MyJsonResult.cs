using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Dashboard.Core.Utility
{
    public sealed class MyJsonResult : System.Web.Mvc.JsonResult
    {
        /// <summary>
        /// 已重写，将返回内容写入响应流中，并使用自定义的序列化方法 
        /// </summary>
        /// <param name="context">当前请求的上下文</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="context"/> parameter is null.</exception>
        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            var data = this.Data;

            this.Data = null;
            base.ExecuteResult(context);
            var js = new JavaScriptSerializer();
            js.RegisterConverters(new List<EFJavaScriptConverter> { new EFJavaScriptConverter(1) });
            string jsonString = js.Serialize(data);
            if (data != null)
            {
                this.Data = data;
                context.HttpContext.Response.Write(jsonString);
            }
        }
    }

}
