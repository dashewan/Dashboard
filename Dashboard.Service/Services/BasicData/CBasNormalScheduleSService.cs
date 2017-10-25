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
using Dashboard.Core.Utility.SqlHelper;

namespace Dashboard.Service.Services.BasicData
{
    public partial class CBasNormalScheduleSService : DBBase, IBasNormalScheduleS<CBasNormalScheduleS>
    {
        #region IBaseService<CBasNormalScheduleS> 成员
        #region Get
        public CBasNormalScheduleS Get(int id)
        {
            return null;
        }

        public CBasNormalScheduleS Get(string id)
        {
            return DataContext.CBasNormalScheduleS.FirstOrDefault(m => m.BasNormalScheduleSId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasNormalScheduleS> GetList()
        {
            return DataContext.CBasNormalScheduleS.OrderByDescending(r => r.CreatedDate);
        }
        public IQueryable<CBasNormalScheduleS> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasNormalScheduleS> originalSource = DataContext.CBasNormalScheduleS;
            string basNormalScheduleId = condition["BasNormalScheduleId"];
            if (!string.IsNullOrEmpty(basNormalScheduleId))
                originalSource = originalSource.Where(o => o.BasNormalScheduleId == basNormalScheduleId);

            string dealerCode = condition["DealerCode"];
            if (!string.IsNullOrEmpty(dealerCode))
                originalSource = originalSource.Where(o => o.DealerCode.Contains(dealerCode));
            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasNormalScheduleS>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasNormalScheduleS BasNormalScheduleS, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (BasNormalScheduleS.ArrivalTimeStart > BasNormalScheduleS.ArrivalTimeEnd)
                {
                    errMsg = "<=StartTimeGreaterThanEndTime>";
                    return false;
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleS>(BasNormalScheduleS);
                if (string.IsNullOrEmpty(BasNormalScheduleS.BasNormalScheduleSId))
                {
                    if (DataContext.CBasNormalScheduleS.Any(c => c.DealerCode == BasNormalScheduleS.DealerCode && c.Definition == BasNormalScheduleS.Definition
                                                                 && c.BasNormalScheduleId == BasNormalScheduleS.BasNormalScheduleId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleS>";
                        return false;
                    }
                    if (DataContext.CBasNormalScheduleS.Any(c => c.DealerCode == BasNormalScheduleS.DealerCode && c.PickupDay == BasNormalScheduleS.PickupDay
                                                                 && c.BasNormalScheduleId == BasNormalScheduleS.BasNormalScheduleId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleS>";
                        return false;
                    }
                    BasNormalScheduleS.BasNormalScheduleSId = Guid.NewGuid().ToString();
                    DataContext.CBasNormalScheduleS.Add(BasNormalScheduleS);
                }
                else
                {
                    if (DataContext.CBasNormalScheduleS.Any(c => c.DealerCode == BasNormalScheduleS.DealerCode && c.Definition == BasNormalScheduleS.Definition
                                                                 && c.BasNormalScheduleId == BasNormalScheduleS.BasNormalScheduleId
                                                                 && c.BasNormalScheduleSId != BasNormalScheduleS.BasNormalScheduleSId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleS>";
                        return false;
                    }
                    if (DataContext.CBasNormalScheduleS.Any(c => c.DealerCode == BasNormalScheduleS.DealerCode && c.PickupDay == BasNormalScheduleS.PickupDay
                                                                  && c.BasNormalScheduleId == BasNormalScheduleS.BasNormalScheduleId
                                                                  && c.BasNormalScheduleSId != BasNormalScheduleS.BasNormalScheduleSId))
                    {
                        errMsg = "<=DuplicatedNormalScheduleS>";
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
                log.Info("BasNormalScheduleS Save方法出错。Author:chenjm ", ex);
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
                var BasNormalScheduleS = DataContext.CBasNormalScheduleS.FirstOrDefault(f => f.BasNormalScheduleSId == id);
                if (BasNormalScheduleS != null)
                {
                    DataContext.CBasNormalScheduleS.Remove(BasNormalScheduleS);
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
