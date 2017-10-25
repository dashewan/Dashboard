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
    public partial class CBasNormalScheduleVCtService : DBBase, IBasNormalScheduleVCt<CBasNormalScheduleVCt>
    {
        #region IBaseService<CBasNormalScheduleVCt> 成员
        #region Get
        public CBasNormalScheduleVCt Get(int id)
        {
            return null;
        }

        public CBasNormalScheduleVCt Get(string id)
        {
            return DataContext.CBasNormalScheduleVCt.FirstOrDefault(m => m.BasNormalScheduleVCtId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasNormalScheduleVCt> GetList()
        {
            return DataContext.CBasNormalScheduleVCt.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasNormalScheduleVCt> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasNormalScheduleVCt> originalSource = DataContext.CBasNormalScheduleVCt;
            string basNormalScheduleId = condition["BasNormalScheduleId"];
            if (!string.IsNullOrEmpty(basNormalScheduleId))
                originalSource = originalSource.Where(o => o.BasNormalScheduleId == basNormalScheduleId);

            string dealerCode = condition["DealerCode"];
            if (!string.IsNullOrEmpty(dealerCode))
                originalSource = originalSource.Where(o => o.DealerCode.Contains(dealerCode));

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasNormalScheduleVCt>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasNormalScheduleVCt BasNormalScheduleVCt, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (!((BasNormalScheduleVCt.FirstVCutoffTime == null && BasNormalScheduleVCt.FirstVPickupTime == null
                    && BasNormalScheduleVCt.FirstVLeadtime == null && BasNormalScheduleVCt.FirstVLeadtimeAmPm.IsNullString()
                    && BasNormalScheduleVCt.FirstVLeadtimeStart == null && BasNormalScheduleVCt.FirstVLeadtimeEnd == null
                    && BasNormalScheduleVCt.FirstVTransMode.IsNullString()) ||
                    (BasNormalScheduleVCt.FirstVCutoffTime != null && BasNormalScheduleVCt.FirstVPickupTime != null
                    && BasNormalScheduleVCt.FirstVLeadtime != null && !BasNormalScheduleVCt.FirstVLeadtimeAmPm.IsNullString()
                    && BasNormalScheduleVCt.FirstVLeadtimeStart != null && BasNormalScheduleVCt.FirstVLeadtimeEnd != null
                    && !BasNormalScheduleVCt.FirstVTransMode.IsNullString())))
                {

                    errMsg = "<=NormalScheduleVCtFirstVInfoIncomplete>";
                    return false;
                }
                else
                {
                    if (BasNormalScheduleVCt.FirstVLeadtimeStart.HasValue
                        && BasNormalScheduleVCt.FirstVLeadtimeEnd.HasValue
                        && BasNormalScheduleVCt.FirstVLeadtimeStart.Value > BasNormalScheduleVCt.FirstVLeadtimeEnd)
                    {
                        errMsg = "<=StartTimeGreaterThanEndTime>";
                        return false;
                    }
                }
                if (BasNormalScheduleVCt.FirstVCutoffTime.HasValue)
                {
                    BasNormalScheduleVCt.FirstVCutoffDay = "Working day";
                }
                if (BasNormalScheduleVCt.FirstVPickupTime.HasValue)
                {
                    BasNormalScheduleVCt.FirstVPickupDay = "Working day";
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVCt>(BasNormalScheduleVCt);
                if (string.IsNullOrEmpty(BasNormalScheduleVCt.BasNormalScheduleVCtId))
                {
                    if (DataContext.CBasNormalScheduleVCt.Any(c => c.DealerCode == BasNormalScheduleVCt.DealerCode
                                                                    && c.BasNormalScheduleId == BasNormalScheduleVCt.BasNormalScheduleId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVCt>";
                        return false;
                    }
                    BasNormalScheduleVCt.SecondVCutoffDay = "Working day";
                    BasNormalScheduleVCt.SecondVPickupDay = "Working day";
                    BasNormalScheduleVCt.BasNormalScheduleVCtId = Guid.NewGuid().ToString();
                    DataContext.CBasNormalScheduleVCt.Add(BasNormalScheduleVCt);
                }
                else
                {
                    if (DataContext.CBasNormalScheduleVCt.Any(c => c.DealerCode == BasNormalScheduleVCt.DealerCode
                                                                    && c.BasNormalScheduleId == BasNormalScheduleVCt.BasNormalScheduleId
                                                                    && c.BasNormalScheduleVCtId != BasNormalScheduleVCt.BasNormalScheduleVCtId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleVCt>";
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
                log.Info("BasNormalScheduleVCt Save方法出错。Author:chenjm ", ex);
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
                var BasNormalScheduleVCt = DataContext.CBasNormalScheduleVCt.FirstOrDefault(f => f.BasNormalScheduleVCtId == id);
                if (BasNormalScheduleVCt != null)
                {
                    DataContext.CBasNormalScheduleVCt.Remove(BasNormalScheduleVCt);
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
