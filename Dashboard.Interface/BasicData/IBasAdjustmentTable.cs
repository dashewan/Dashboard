using Dashboard.Interface.BaseService;

namespace Dashboard.Interface.BasicData
{
    public partial interface IBasAdjustmentTable<T> : IBaseService<T> where T : class
    {
        void AdjustmentTableImport(string strPath, string adjustmentType, out string errMsg);
    }
}
