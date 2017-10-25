using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Interface.BaseService;

using Dashboard.Domain.SysManagement;

namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysOrganization<T> : IBaseService<T> where T : class
    {
        bool HasChild(string id);
        bool Delete(String id, out string errMsg);
        IQueryable<MyClass> TestMethod();
    }
}
