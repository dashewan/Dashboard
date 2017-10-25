using Dashboard.Interface.BaseService;

namespace Dashboard.Interface.BasicData
{
    public partial interface IBasSpecialSchedule<T> : IBaseService<T> where T : class
    {
        void SpecialScheduleImport(string BasSpecialScheduleIndexId, string strPath, out string errMsg);
    }
}
