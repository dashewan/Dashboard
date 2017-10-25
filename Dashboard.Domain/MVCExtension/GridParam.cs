using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Domain.MVCExtension
{
    public class GridParam
    {
        /// <summary>
        /// 当排序时定义排序字段名称的索引，参数名为sidx
        /// </summary>
        public string sidx{get;set;}
        /// <summary>
        /// 升序还是降序
        /// </summary>
        public string sord { get; set; }
        /// <summary>
        /// 传入的页数
        /// </summary>
        public int page{get;set;} 
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int rows{get;set;}
    }
}
