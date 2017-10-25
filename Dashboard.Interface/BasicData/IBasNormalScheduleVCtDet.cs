using System.Linq;
using Dashboard.Interface.BaseService;

namespace Dashboard.Interface.BasicData
{
    public partial interface IBasNormalScheduleVCtDet<T> : IBaseService<T> where T : class
    {
        IQueryable<T> GetList(string basNormalScheduleVCtId);
    }
}
