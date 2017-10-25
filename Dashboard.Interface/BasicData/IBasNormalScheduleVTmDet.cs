using System.Linq;
using Dashboard.Interface.BaseService;

namespace Dashboard.Interface.BasicData
{
    public partial interface IBasNormalScheduleVTmDet<T> : IBaseService<T> where T : class
    {
        IQueryable<T> GetList(string basNormalScheduleVTmId);
    }
}
