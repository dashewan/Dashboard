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
    public partial class CBasNormalScheduleVCtDetService : DBBase, IBasNormalScheduleVCtDet<CBasNormalScheduleVCtDet>
    {
        #region IBaseService<CBasNormalScheduleVCtDet> 成员
        #region Get
        public CBasNormalScheduleVCtDet Get(int id)
        {
            return null;
        }

        public CBasNormalScheduleVCtDet Get(string id)
        {
            return DataContext.CBasNormalScheduleVCtDet.FirstOrDefault(m => m.BasNormalScheduleVCtDetId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasNormalScheduleVCtDet> GetList()
        {
            return DataContext.CBasNormalScheduleVCtDet.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasNormalScheduleVCtDet> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasNormalScheduleVCtDet> originalSource = DataContext.CBasNormalScheduleVCtDet;

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasNormalScheduleVCtDet>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasNormalScheduleVCtDet BasNormalScheduleVCtDet, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (BasNormalScheduleVCtDet.SecondVLeadtimeStart > BasNormalScheduleVCtDet.SecondVLeadtimeEnd)
                {
                    errMsg = "<=StartTimeGreaterThanEndTime>";
                    return false;
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVCtDet>(BasNormalScheduleVCtDet);
                if (string.IsNullOrEmpty(BasNormalScheduleVCtDet.BasNormalScheduleVCtDetId))
                {
                    if (DataContext.CBasNormalScheduleVCtDet.Any(c => c.Pdc == BasNormalScheduleVCtDet.Pdc
                                                                       && c.CargoType == BasNormalScheduleVCtDet.CargoType
                                                                       && c.BasNormalScheduleVCtId == BasNormalScheduleVCtDet.BasNormalScheduleVCtId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVCtDet>";
                        return false;
                    }
                    BasNormalScheduleVCtDet.BasNormalScheduleVCtDetId = Guid.NewGuid().ToString();
                    DataContext.CBasNormalScheduleVCtDet.Add(BasNormalScheduleVCtDet);
                }
                else
                {
                    if (DataContext.CBasNormalScheduleVCtDet.Any(c => c.Pdc == BasNormalScheduleVCtDet.Pdc
                                                                       && c.CargoType == BasNormalScheduleVCtDet.CargoType
                                                                       && c.BasNormalScheduleVCtId == BasNormalScheduleVCtDet.BasNormalScheduleVCtId
                                                                       && c.BasNormalScheduleVCtDetId != BasNormalScheduleVCtDet.BasNormalScheduleVCtDetId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVCtDet>";
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
                log.Info("BasNormalScheduleVCtDet Save方法出错。Author:chenjm ", ex);
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
                var BasNormalScheduleVCtDet = DataContext.CBasNormalScheduleVCtDet.FirstOrDefault(f => f.BasNormalScheduleVCtDetId == id);
                if (BasNormalScheduleVCtDet != null)
                {
                    DataContext.CBasNormalScheduleVCtDet.Remove(BasNormalScheduleVCtDet);
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

        #region IBaseService<CBasNormalScheduleVCtDet> 成员
        #region GetList
        public IQueryable<CBasNormalScheduleVCtDet> GetList(string basNormalScheduleVCtId)
        {
            IQueryable<CBasNormalScheduleVCtDet> originalSource = DataContext.CBasNormalScheduleVCtDet.Where(p => p.BasNormalScheduleVCtId == basNormalScheduleVCtId);
            return originalSource;
        }
        #endregion
        #endregion
    }
}
