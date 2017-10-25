using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Core.Const
{
    public class Consts
    {
        public const string IBaseService = "IBaseService";

        public const string BasCity = "BasCity";//城市信息
        public const string SbProvince = "SbProvince";//省份信息
        public const string SbCodeType = "SbCodeType";//数据字典
        public const string CSbCodeDef = "CSbCodeDef";//数据字典明细信息
        public const string CBasFrtType = "CBasFrtType";//费用类型 
        public const string CBasFrtDef = "CBasFrtDef";//费用名称
        public const string BasAttachment = "BasAttachment";//附件

        #region 客户、承运商
        public const string CimCust = "CimCust";
        public const string CimCustAdd = "CimCustAdd";
        public const string CimRelatedOffice = "CimRelatedOffice";
        #endregion

        #region 运价合同
        public const string CimCustContract = "CimCustContract";
        public const string CimCustRoute = "CimCustRoute";
        public const string CimCustRoutePoint = "CimCustRoutePoint";
        public const string CimFreightProtocol = "CimFreightProtocol";
        public const string CimFreightRate = "CimFreightRate";
        #endregion

        #region 运单管理
        public const string TmsOrder = "TmsOrder";
        public const string TmsOrderDetail = "TmsOrderDetail";
        public const string TmsOrderSign = "TmsOrderSign";
        public const string TmsDispatch = "TmsDispatch";
        #endregion

        public const string BasVehicle = "BasVehicle";
        public const string BasPackage = "BasPackage";
        public const string BasCurrency = "BasCurrency";

        public const string AccFreight = "AccFreight";

        public const string AccLedgerComp = "AccLedgerComp";
        public const string AccLedgerCompDetail = "AccLedgerCompDetail";
        //public const string TmsOrder = "TmsOrder";

        public const string CostReport = "CostReport";//成本报告
        public const string CAccBudgetControl = "CAccBudgetControl";//预算控制
    }
}
