using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Interface.BaseService;


namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysRoleFunction<T> : IBaseService<T> where T : class
    {
        string GetRoleFunc(string sFunctionId, string sRoleId);
        bool Save(List<T> roleFunc, string MenuId, bool deleteAllFlag, out string errMsg);
        bool DeleteByRoleId(string id);
    }
}
