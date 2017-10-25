using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Interface.BaseService;


namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysUserRole<T> : IBaseService<T> where T : class
    {
        IQueryable<T> GetList(Dictionary<string, string> condition);
        bool Delete(string id, out string errMsg);
        bool DeleteByRoleId(string id);
    }
}
