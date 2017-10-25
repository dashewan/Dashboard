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
    public partial class CBasPdcSequenceService : DBBase, IBasPdcSequence<CBasPdcSequence>
    {
        #region IBaseService<CBasPdcSequence> 成员
        #region Get
        public CBasPdcSequence Get(int id)
        {
            return null;
        }

        public CBasPdcSequence Get(string id)
        {
            return DataContext.CBasPdcSequence.FirstOrDefault(m => m.BasPdcSequenceId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasPdcSequence> GetList()
        {
            return DataContext.CBasPdcSequence.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasPdcSequence> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasPdcSequence> originalSource = DataContext.CBasPdcSequence;

            string pdc = condition["Pdc"];
            if (!pdc.IsNullString())
                originalSource = originalSource.Where(o => o.Pdc.Contains(pdc));

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasPdcSequence>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasPdcSequence BasPdcSequence, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasPdcSequence>(BasPdcSequence);
                if (string.IsNullOrEmpty(BasPdcSequence.BasPdcSequenceId))
                {
                    if (DataContext.CBasPdcSequence.Any(c => c.Pdc == BasPdcSequence.Pdc))
                    {
                        errMsg = "<=DuplicatedPdcSequence>";
                        return false;
                    }
                    BasPdcSequence.BasPdcSequenceId = Guid.NewGuid().ToString();
                    DataContext.CBasPdcSequence.Add(BasPdcSequence);
                }
                else
                {
                    if (DataContext.CBasPdcSequence.Any(c => c.Pdc == BasPdcSequence.Pdc
                                                              && c.BasPdcSequenceId != BasPdcSequence.BasPdcSequenceId))
                    {
                        errMsg = "<=DuplicatedPdcSequence>";
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
                log.Info("BasPdcSequence Save方法出错。Author:chenjm ", ex);
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
                var BasPdcSequence = DataContext.CBasPdcSequence.FirstOrDefault(f => f.BasPdcSequenceId == id);
                if (BasPdcSequence != null)
                {
                    DataContext.CBasPdcSequence.Remove(BasPdcSequence);
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
