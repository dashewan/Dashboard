using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Interface.BaseService;
using Dashboard.Domain.SysManagement;


namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysFunction<T> : IBaseService<T> where T : class
    {

        List<int> GetMenuGroup(string sysRoleId);
        List<SysFunction> GetMenuFromGroup(string sysRoleId, int intGroupType);
        List<SysFunction> GetUserFunction(string sysRoleId);
        List<SysFunction> GetUserPermission(string sysRoleId);
        List<string> GetOrganizationIds(string sysRoleId);

        bool funcHasChild(string id);
        IQueryable<T> GetList(Dictionary<string, string> condition);
        List<string> GetForwarderCodes(string sysRoleId);
    }
}
