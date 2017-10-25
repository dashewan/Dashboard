using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Domain.SysManagement;
using System.Web.Mvc;
using Dashboard.Domain.MVCExtension;

namespace Dashboard.Interface.BaseService
{

    public interface IBaseService<T> where T : class
    {
        T Get(int id);
        T Get(String id);
        IQueryable<T> GetList();
        bool Save(T entity, out string errMsg);
        bool Delete(int id);
        bool Delete(String id);
        IQueryable<T> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount);
    }

}
