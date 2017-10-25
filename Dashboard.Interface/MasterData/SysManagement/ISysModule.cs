using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Domain.SysManagement;
using Dashboard.Interface.BaseService;

namespace Dashboard.Interface.MasterData.SysManagement
{
    public interface ISysModule<T> : IBaseService<T> where T : class
    {
         void YourMethod();
    }
}
