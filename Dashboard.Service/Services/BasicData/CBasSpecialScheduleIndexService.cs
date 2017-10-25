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
using Dashboard.Common.Util;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;


public partial class CBasSpecialScheduleIndexService : DBBase, IBasSpecialScheduleIndex<CBasSpecialScheduleIndex>
{
    #region IBaseService<CBasSpecialScheduleIndex> 成员
    #region Get
    public CBasSpecialScheduleIndex Get(int id)
    {
        return null;
    }

    public CBasSpecialScheduleIndex Get(string id)
    {
        return DataContext.CBasSpecialScheduleIndex.FirstOrDefault(m => m.BasSpecialScheduleIndexId == id);
    }
    #endregion

    #region GetList
    public IQueryable<CBasSpecialScheduleIndex> GetList()
    {
        return DataContext.CBasSpecialScheduleIndex.OrderByDescending(r => r.CreatedDate);
    }

    public IQueryable<CBasSpecialScheduleIndex> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
    {
        IQueryable<CBasSpecialScheduleIndex> originalSource = DataContext.CBasSpecialScheduleIndex;

        string scheduleNo = condition["ScheduleNo"];
        if (!string.IsNullOrEmpty(scheduleNo))
            originalSource = originalSource.Where(o => o.ScheduleNo.Contains(scheduleNo));

        pageCount = 0;
        originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
        totalCount = originalSource.Count();
        var dataSouce = Paging<CBasSpecialScheduleIndex>(originalSource, gp.rows, gp.page, out pageCount);
        return dataSouce;
    }
    #endregion

    #region Save
    public bool Save(CBasSpecialScheduleIndex CBasSpecialScheduleIndex, out string errMsg)
    {
        errMsg = "";
        try
        {
            if (CBasSpecialScheduleIndex.EffectiveDate > CBasSpecialScheduleIndex.ExpirationDate)
            {
                errMsg = "<=EffectiveDateGreaterThanExpirationDate>";
                return false;
            }
            Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialScheduleIndex>(CBasSpecialScheduleIndex);
            if (string.IsNullOrEmpty(CBasSpecialScheduleIndex.BasSpecialScheduleIndexId))
            {
                if (DataContext.CBasSpecialScheduleIndex.Any(c => c.ScheduleNo == CBasSpecialScheduleIndex.ScheduleNo))
                {
                    errMsg = "<=ScheduleNoRepeat>";
                    return false;
                }
                //判断生效与失效日期是否有重叠
                if (DataContext.CBasSpecialScheduleIndex.Any(c => ((c.EffectiveDate >= CBasSpecialScheduleIndex.EffectiveDate
                                                          && c.EffectiveDate <= CBasSpecialScheduleIndex.ExpirationDate)
                                                         || (c.EffectiveDate <= CBasSpecialScheduleIndex.EffectiveDate
                                                          && c.ExpirationDate >= CBasSpecialScheduleIndex.ExpirationDate)
                                                          || (c.ExpirationDate >= CBasSpecialScheduleIndex.EffectiveDate
                                                          && c.ExpirationDate <= CBasSpecialScheduleIndex.ExpirationDate))))
                {
                    errMsg = "<=ScheduleRepeat>";
                    return false;
                }
                CBasSpecialScheduleIndex.BasSpecialScheduleIndexId = Guid.NewGuid().ToString();
                DataContext.CBasSpecialScheduleIndex.Add(CBasSpecialScheduleIndex);
            }
            else
            {
                if (DataContext.CBasSpecialScheduleIndex.Any(c => c.ScheduleNo == CBasSpecialScheduleIndex.ScheduleNo && c.BasSpecialScheduleIndexId != CBasSpecialScheduleIndex.BasSpecialScheduleIndexId))
                {
                    errMsg = "<=ScheduleNoRepeat>";
                    return false;
                }
                if (DataContext.CBasSpecialScheduleIndex.Any(c => ((c.EffectiveDate >= CBasSpecialScheduleIndex.EffectiveDate
                                                          && c.EffectiveDate <= CBasSpecialScheduleIndex.ExpirationDate)
                                                         || (c.EffectiveDate <= CBasSpecialScheduleIndex.EffectiveDate
                                                          && c.ExpirationDate >= CBasSpecialScheduleIndex.ExpirationDate)
                                                          || (c.ExpirationDate >= CBasSpecialScheduleIndex.EffectiveDate
                                                          && c.ExpirationDate <= CBasSpecialScheduleIndex.ExpirationDate))
                                                          && c.BasSpecialScheduleIndexId != CBasSpecialScheduleIndex.BasSpecialScheduleIndexId))
                {
                    errMsg = "<=ScheduleRepeat>";
                    return false;
                }
            }
            DataContext.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            errMsg = "<=SaveError>";
            Log4netHelper.LoadADONetAppender();
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("CBasSpecialScheduleIndex Save方法出错。Author:chenjm ", ex);
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
            var CBasSpecialScheduleIndex = DataContext.CBasSpecialScheduleIndex.FirstOrDefault(f => f.BasSpecialScheduleIndexId == id);
            if (CBasSpecialScheduleIndex != null)
            {
                DataContext.CBasSpecialScheduleIndex.Remove(CBasSpecialScheduleIndex);
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
            throw new Exception("<=ExcelFormatError>" + columnName);
        }
        if (row[columnName] == null || row[columnName].ToString().IsNullString())
        {
            throw new Exception("<=Di>" + index.ToString() + "<=Row> " + columnName + "<=IsRequired>");
        }
    }

    /// <summary>
    /// 验证是否为空,为空则返回True，不存在列默认为空
    /// </summary>
    /// <param name="row"></param>
    /// <param name="columnName"></param>
    /// <returns></returns>
    private bool DataIsNullValidator(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
        {
            return true;
        }
        if (row[columnName] == null || row[columnName].ToString().IsNullString())
        {
            return true;
        }
        return false;
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
            throw new Exception("<=ExcelFormatError>" + columnName);
        }
        if (row[columnName] == null || row[columnName].ToString().IsNullString())
        {
            throw new Exception("<=Di>" + index.ToString() + "<=Row> " + columnName + "<=IsRequired>");
        }
        var type = typeof(TResult);

        var method = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
        var parameters = new object[] { row[columnName].ToString(), default(TResult) };
        // 若转换失败，执行failed
        if (!(bool)method.Invoke(null, parameters))
        {
            throw new Exception("<=Di>" + index.ToString() + "<=Row> " + columnName + "<=FormatError>");
        }
    }
    #endregion
}
    