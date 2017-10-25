using Dashboard.Interface.BaseService;

namespace Dashboard.Interface.BasicData
{
    public partial interface IBasNormalSchedule<T> : IBaseService<T> where T : class
    {
        void NormalScheduleImport(string basNormalScheduleId, string strPath, string type, out string errMsg);
    }
}
