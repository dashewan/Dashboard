using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Interface.BaseService;
using System.Data;


namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysUser<T> : IBaseService<T> where T : class
    {
        T Login(string userCode, string userPassword);
        T GetUserByCode(string userCode);
        //DataTable sysTeam(string sql);
    }
}
