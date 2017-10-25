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
    public partial class CBasSpecialDayService : DBBase, IBasSpecialDay<CBasSpecialDay>
    {
        #region IBaseService<CBasSpecialDay> 成员
        #region Get
        public CBasSpecialDay Get(int id)
        {
            return null;
        }

        public CBasSpecialDay Get(string id)
        {
            return DataContext.CBasSpecialDay.FirstOrDefault(m => m.BasSpecialDayId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasSpecialDay> GetList()
        {
            return DataContext.CBasSpecialDay.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasSpecialDay> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasSpecialDay> originalSource = DataContext.CBasSpecialDay;

            if (condition.ContainsKey("SpecialDayFrom"))
            {
                string startdate = condition["SpecialDayFrom"];
                if (!string.IsNullOrEmpty(startdate))
                {
                    DateTime startDate = Convert.ToDateTime(startdate);
                    if (startDate != null)
                        originalSource = originalSource.Where(o => o.SpecialDay >= startDate);
                }
            }
            if (condition.ContainsKey("SpecialDayTo"))
            {
                string endingdate = condition["SpecialDayTo"];
                if (!string.IsNullOrEmpty(endingdate))
                {
                    DateTime endingDate = Convert.ToDateTime(endingdate);
                    if (endingDate != null)
                        originalSource = originalSource.Where(o => o.SpecialDay <= endingDate);
                }
            }

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasSpecialDay>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasSpecialDay BasSpecialDay, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialDay>(BasSpecialDay);
                if (string.IsNullOrEmpty(BasSpecialDay.BasSpecialDayId))
                {
                    if (DataContext.CBasSpecialDay.Any(c => c.SpecialDay == BasSpecialDay.SpecialDay))
                    {
                        errMsg = "<=DuplicatedSpecialDay>";
                        return false;
                    }
                    BasSpecialDay.BasSpecialDayId = Guid.NewGuid().ToString();
                    DataContext.CBasSpecialDay.Add(BasSpecialDay);
                }
                else
                {
                    if (DataContext.CBasSpecialDay.Any(c => c.SpecialDay == BasSpecialDay.SpecialDay && c.BasSpecialDayId != BasSpecialDay.BasSpecialDayId))
                    {
                        errMsg = "<=DuplicatedSpecialDay>";
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
                log.Info("BasSpecialDay Save方法出错。Author:chenjm ", ex);
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
                var BasSpecialDay = DataContext.CBasSpecialDay.FirstOrDefault(f => f.BasSpecialDayId == id);
                if (BasSpecialDay != null)
                {
                    DataContext.CBasSpecialDay.Remove(BasSpecialDay);
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
