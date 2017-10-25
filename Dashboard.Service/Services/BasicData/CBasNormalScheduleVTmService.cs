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
    public partial class CBasNormalScheduleVTmService : DBBase, IBasNormalScheduleVTm<CBasNormalScheduleVTm>
    {
        #region IBaseService<CBasNormalScheduleVTm> 成员
        #region Get
        public CBasNormalScheduleVTm Get(int id)
        {
            return null;
        }

        public CBasNormalScheduleVTm Get(string id)
        {
            return DataContext.CBasNormalScheduleVTm.FirstOrDefault(m => m.BasNormalScheduleVTmId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasNormalScheduleVTm> GetList()
        {
            return DataContext.CBasNormalScheduleVTm.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasNormalScheduleVTm> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasNormalScheduleVTm> originalSource = DataContext.CBasNormalScheduleVTm;
            string basNormalScheduleId = condition["BasNormalScheduleId"];
            if (!string.IsNullOrEmpty(basNormalScheduleId))
                originalSource = originalSource.Where(o => o.BasNormalScheduleId == basNormalScheduleId);

            string destination = condition["Destination"];
            if (!string.IsNullOrEmpty(destination))
                originalSource = originalSource.Where(o => o.Destination.Contains(destination));

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasNormalScheduleVTm>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasNormalScheduleVTm BasNormalScheduleVTm, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (!((BasNormalScheduleVTm.FirstVCutoffTime == null && BasNormalScheduleVTm.FirstVPickupTime == null
                       && BasNormalScheduleVTm.FirstVLeadtime == null && BasNormalScheduleVTm.FirstVLeadtimeAmPm.IsNullString()
                       && BasNormalScheduleVTm.FirstVLeadtimeStart == null && BasNormalScheduleVTm.FirstVLeadtimeEnd == null
                       && BasNormalScheduleVTm.FirstVTransMode.IsNullString()) ||
                       (BasNormalScheduleVTm.FirstVCutoffTime != null && BasNormalScheduleVTm.FirstVPickupTime != null
                       && BasNormalScheduleVTm.FirstVLeadtime != null && !BasNormalScheduleVTm.FirstVLeadtimeAmPm.IsNullString()
                       && BasNormalScheduleVTm.FirstVLeadtimeStart != null && BasNormalScheduleVTm.FirstVLeadtimeEnd != null
                       && !BasNormalScheduleVTm.FirstVTransMode.IsNullString())))
                {

                    errMsg = "<=NormalScheduleVTmFirstVInfoIncomplete>";
                    return false;
                }
                else
                {
                    if (BasNormalScheduleVTm.FirstVLeadtimeStart.HasValue
                        && BasNormalScheduleVTm.FirstVLeadtimeEnd.HasValue
                        && BasNormalScheduleVTm.FirstVLeadtimeStart.Value > BasNormalScheduleVTm.FirstVLeadtimeEnd)
                    {
                        errMsg = "<=StartTimeGreaterThanEndTime>";
                        return false;
                    }
                }
                if (BasNormalScheduleVTm.FirstVCutoffTime.HasValue)
                {
                    BasNormalScheduleVTm.FirstVCutoffDay = "Working day";
                }
                if (BasNormalScheduleVTm.FirstVPickupTime.HasValue)
                {
                    BasNormalScheduleVTm.FirstVPickupDay = "Working day";
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVTm>(BasNormalScheduleVTm);
                if (string.IsNullOrEmpty(BasNormalScheduleVTm.BasNormalScheduleVTmId))
                {
                    if (DataContext.CBasNormalScheduleVTm.Any(c => c.Destination == BasNormalScheduleVTm.Destination
                                                                       && c.BasNormalScheduleId == BasNormalScheduleVTm.BasNormalScheduleId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVTm>";
                        return false;
                    }
                    BasNormalScheduleVTm.SecondVCutoffDay = "Working day";
                    BasNormalScheduleVTm.SecondVPickupDay = "Working day";
                    BasNormalScheduleVTm.BasNormalScheduleVTmId = Guid.NewGuid().ToString();
                    DataContext.CBasNormalScheduleVTm.Add(BasNormalScheduleVTm);
                }
                else
                {
                    if (DataContext.CBasNormalScheduleVTm.Any(c => c.Destination == BasNormalScheduleVTm.Destination
                                                                    && c.BasNormalScheduleId == BasNormalScheduleVTm.BasNormalScheduleId
                                                                    && c.BasNormalScheduleVTmId != BasNormalScheduleVTm.BasNormalScheduleVTmId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVTm>";
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
                log.Info("BasNormalScheduleVTm Save方法出错。Author:chenjm ", ex);
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
                var BasNormalScheduleVTm = DataContext.CBasNormalScheduleVTm.FirstOrDefault(f => f.BasNormalScheduleVTmId == id);
                if (BasNormalScheduleVTm != null)
                {
                    DataContext.CBasNormalScheduleVTm.Remove(BasNormalScheduleVTm);
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
    }
}
