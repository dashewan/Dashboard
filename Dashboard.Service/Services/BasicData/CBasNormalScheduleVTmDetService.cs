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

namespace Dashboard.Service.Services.BasicData
{
    public partial class CBasNormalScheduleVTmDetService : DBBase, IBasNormalScheduleVTmDet<CBasNormalScheduleVTmDet>
    {
        #region IBaseService<CBasNormalScheduleVTmDet> 成员
        #region Get
        public CBasNormalScheduleVTmDet Get(int id)
        {
            return null;
        }

        public CBasNormalScheduleVTmDet Get(string id)
        {
            return DataContext.CBasNormalScheduleVTmDet.FirstOrDefault(m => m.BasNormalScheduleVTmDetId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasNormalScheduleVTmDet> GetList()
        {
            return DataContext.CBasNormalScheduleVTmDet.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasNormalScheduleVTmDet> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasNormalScheduleVTmDet> originalSource = DataContext.CBasNormalScheduleVTmDet;

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasNormalScheduleVTmDet>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasNormalScheduleVTmDet BasNormalScheduleVTmDet, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (BasNormalScheduleVTmDet.SecondVLeadtimeStart > BasNormalScheduleVTmDet.SecondVLeadtimeEnd)
                {
                    errMsg = "<=StartTimeGreaterThanEndTime>";
                    return false;
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVTmDet>(BasNormalScheduleVTmDet);
                if (string.IsNullOrEmpty(BasNormalScheduleVTmDet.BasNormalScheduleVTmDetId))
                {
                    if (DataContext.CBasNormalScheduleVTmDet.Any(c => c.Pdc == BasNormalScheduleVTmDet.Pdc
                                                                          && c.TransMode == BasNormalScheduleVTmDet.TransMode
                                                                          && c.BasNormalScheduleVTmId == BasNormalScheduleVTmDet.BasNormalScheduleVTmId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVTmDet>";
                        return false;
                    }
                    BasNormalScheduleVTmDet.BasNormalScheduleVTmDetId = Guid.NewGuid().ToString();
                    DataContext.CBasNormalScheduleVTmDet.Add(BasNormalScheduleVTmDet);
                }
                else
                {
                    if (DataContext.CBasNormalScheduleVTmDet.Any(c => c.Pdc == BasNormalScheduleVTmDet.Pdc
                                                                       && c.TransMode == BasNormalScheduleVTmDet.TransMode
                                                                       && c.BasNormalScheduleVTmId == BasNormalScheduleVTmDet.BasNormalScheduleVTmId
                                                                       && c.BasNormalScheduleVTmDetId != BasNormalScheduleVTmDet.BasNormalScheduleVTmDetId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVTmDet>";
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
                log.Info("BasNormalScheduleVTmDet Save方法出错。Author:chenjm ", ex);
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
                var BasNormalScheduleVTmDet = DataContext.CBasNormalScheduleVTmDet.FirstOrDefault(f => f.BasNormalScheduleVTmDetId == id);
                if (BasNormalScheduleVTmDet != null)
                {
                    DataContext.CBasNormalScheduleVTmDet.Remove(BasNormalScheduleVTmDet);
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

        #region IBaseService<CBasNormalScheduleVTmDet> 成员
        #region GetList
        public IQueryable<CBasNormalScheduleVTmDet> GetList(string basNormalScheduleVTmId)
        {
            IQueryable<CBasNormalScheduleVTmDet> originalSource = DataContext.CBasNormalScheduleVTmDet.Where(p => p.BasNormalScheduleVTmId == basNormalScheduleVTmId);
            return originalSource;
        }
        #endregion
        #endregion
    }
}
