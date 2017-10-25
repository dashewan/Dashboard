using Dashboard.Interface.BaseService;
using System.Data;
using System.Linq;
using Dashboard.Domain.BasicData;
using System.Collections.Generic;
using Dashboard.Domain.MVCExtension;

namespace Dashboard.Interface.BasicData
{
    public partial interface IBasNormalScheduleS<T> : IBaseService<T> where T : class
    {
    }
}
