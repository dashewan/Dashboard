using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dashboard.Core.FrogEntities;
using Dashboard.Core.Log4netHelper;
using Dashboard.Core.Utility;
using Dashboard.Domain.BasicData;
using Dashboard.Domain.MVCExtension;
using Dashboard.Interface.BasicData;
using log4net;
using System.Data;
using Dashboard.Common.Util;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dashboard.Service.Services.BasicData
{
    public partial class CBasAdjustmentTableService : DBBase, IBasAdjustmentTable<CBasAdjustmentTable>
    {
        #region IBaseService<CBasAdjustmentTable> 成员
        #region Get
        public CBasAdjustmentTable Get(int id)
        {
            return null;
        }

        public CBasAdjustmentTable Get(string id)
        {
            return DataContext.CBasAdjustmentTable.FirstOrDefault(m => m.BasAdjustmentTableId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasAdjustmentTable> GetList()
        {
            return DataContext.CBasAdjustmentTable.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasAdjustmentTable> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasAdjustmentTable> originalSource = DataContext.CBasAdjustmentTable.Where(p => p.UserCode == Authentication.Authentication.CurrentUser.SysUserId);

            string adjustmentType = condition["AdjustmentType"];
            if (!string.IsNullOrEmpty(adjustmentType))
                originalSource = originalSource.Where(o => o.AdjustmentType == adjustmentType);

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasAdjustmentTable>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasAdjustmentTable BasAdjustmentTable, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasAdjustmentTable>(BasAdjustmentTable);
                if (string.IsNullOrEmpty(BasAdjustmentTable.BasAdjustmentTableId))
                {
                    BasAdjustmentTable.BasAdjustmentTableId = Guid.NewGuid().ToString();
                    DataContext.CBasAdjustmentTable.Add(BasAdjustmentTable);
                }
                else
                {

                }
                DataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("BasAdjustmentTable Save方法出错。Author:chenjm ", ex);
                return false;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return false;
        }

        public bool Delete(string id)
        {
            try
            {
                var BasAdjustmentTable = DataContext.CBasAdjustmentTable.FirstOrDefault(f => f.BasAdjustmentTableId == id);
                if (BasAdjustmentTable != null)
                {
                    DataContext.CBasAdjustmentTable.Remove(BasAdjustmentTable);
                    DataContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #endregion

        #region IBasSpecialSchedule<CBasSpecialSchedule> 成员
        #region AdjustmentTableImport
        /// <summary>
        /// 调整过滤条件表导入到数据库
        /// 每次导入会先删除 同用户 同类型 的已上传条目
        /// </summary>
        /// <param name="strPath">服务器上过滤条件excel文件路径</param>
        /// <param name="adjustmentType"></param>
        /// <param name="errMsg"></param>
        public void AdjustmentTableImport(string strPath, string adjustmentType, out string errMsg)
        {
            errMsg = "<=ImportSuccess>";
            try
            {
                DataContext.Configuration.AutoDetectChangesEnabled = false;
                DataContext.Configuration.ValidateOnSaveEnabled = false;

                var args = new DbParameter[] { 
                    new SqlParameter ("@userCode", Authentication.Authentication.CurrentUser.SysUserId) ,
                    new SqlParameter ("@adjustmentType",adjustmentType) ,
                 };
                DataContext.Database.ExecuteSqlCommand("delete BAS_ADJUSTMENT_TABLE where USER_CODE = @userCode AND ADJUSTMENT_TYPE = @adjustmentType", args);

                AdjustmentTableImport(strPath, adjustmentType);
                DataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                errMsg = "<=ImportError>:" + ex.Message;
            }
            finally
            {
                DataContext.Configuration.AutoDetectChangesEnabled = true;
                DataContext.Configuration.ValidateOnSaveEnabled = true;
            }
        }
        #endregion
        #endregion

        #region AdjustmentTableImport
        /// <summary>
        /// Special Schedule Import
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="adjustmentType"></param>
        private void AdjustmentTableImport(string strPath, string adjustmentType)
        {
            try
            {
                NpoiHelper npoiHelper = new NpoiHelper(strPath, false);
                DataTable dt = npoiHelper.ExcelToDataTable(0);
                Dictionary<string, string> dicDealerCode = new Dictionary<string, string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    int index = 2;
                    foreach (DataRow row in dt.Rows)
                    {
                        string printDate = row["Exclude STK of below TN print date"].ToString();
                        string tnNo = row["Exclude below TN number"].ToString();
                        string tnProperty = row["TN Property"].ToString();
                        string serial = row["Serial Number"].ToString();
                        if (!printDate.IsNullString() || !tnNo.IsNullString() || !tnProperty.IsNullString())
                        {
                            CBasAdjustmentTable item = new CBasAdjustmentTable();
                            Dashboard.Authentication.Authentication.UpdateEntity<CBasAdjustmentTable>(item);
                            item.BasAdjustmentTableId = Guid.NewGuid().ToString();

                            item.AdjustmentType = adjustmentType;
                            item.UserCode = Authentication.Authentication.CurrentUser.SysUserId;
                            if (!printDate.IsNullString())
                            {
                                DataValidator<DateTime>(row, "Exclude STK of below TN print date", index);
                                item.PrintDate = Convert.ToDateTime(printDate);
                            }
                            item.TnNo = tnNo;
                            item.TnProperty = tnProperty;
                            item.Serial = serial;//排序字段
                            DataContext.CBasAdjustmentTable.Add(item);
                        }
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DataValidator
        /// <summary>
        /// 数据验证:包括列名是否存在及非空
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="index"></param>
        private void DataValidator(DataRow row, string columnName, int index)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                throw new Exception("Excel格式错误,不存在列" + columnName);
            }
            if (row[columnName] == null || row[columnName].ToString().IsNullString())
            {
                throw new Exception("第" + index.ToString() + "行 " + columnName + "不允许为空");
            }
        }

        /// <summary>
        /// 数据验证:包括列名是否存在及非空、类型有效性
        /// </summary>
        /// <typeparam name="TResult">数据类型</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="columnName">列名</param>
        /// <param name="index">所在行</param>
        private void DataValidator<TResult>(DataRow row, string columnName, int index)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                throw new Exception("Excel格式错误,不存在列" + columnName);
            }
            if (row[columnName] == null || row[columnName].ToString().IsNullString())
            {
                throw new Exception("第" + index.ToString() + "行 " + columnName + "不允许为空");
            }
            var type = typeof(TResult);

            var method = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
            var parameters = new object[] { row[columnName].ToString(), default(TResult) };
            // 若转换失败，执行failed
            if (!(bool)method.Invoke(null, parameters))
            {
                throw new Exception("第" + index.ToString() + "行 " + columnName + "格式不正确");
            }
        }
        #endregion
    }
}
