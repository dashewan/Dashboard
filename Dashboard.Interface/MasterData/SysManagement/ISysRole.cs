using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Interface.BaseService;
using Dashboard.Domain.SysManagement;


namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysRole<T> : IBaseService<T> where T : class
    {
        List<CRoleFunctionExport> GetRoleFunctionExport();
        List<CRoleUserExport> GetRoleUserExport();
    }
}
