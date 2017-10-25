using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Core.Enumeration
{
    public enum RoleType
    {
        Warehouse = 0,
        Forwarder = 1,
        WTM = 2,
        Dealer = 3,
        Finance = 4,
        Cim = 5
    }

    public enum AuthType
    {
        Operator = 0,
        Supervisor = 1
    }

    [Flags]
    public enum RegionType
    {
        Beijing = 1,
        Shanghai = 2,
        Yangzhou = 4,
        Guangzhou = 8
    }

    public enum Language
    {
        /// <summary>
        /// 英文
        /// </summary>
        English = 0,

        /// <summary>
        ///简体中文
        /// </summary>
        SimplifiedChinese = 1
    }

    /// <summary>
    /// 主页面左侧功能菜单
    /// </summary>
    public enum FunctionGroupType
    {
        /// <summary>
        /// 业务信息
        /// </summary>
        BusinessInfo = 1,

        /// <summary>
        /// 图表信息
        /// </summary>
        ChartInfo = 2,

        /// <summary>
        /// 首页管理
        /// </summary>
        HomeManagement = 3,

        /// <summary>
        /// 系统管理
        /// </summary>
        System = 9
    }

    public enum FuntionType
    {
        Function = 0,
        Page = 1
    }
}
