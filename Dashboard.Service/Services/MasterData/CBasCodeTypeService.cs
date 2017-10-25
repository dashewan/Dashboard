using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Core.FrogEntities;
using Dashboard.Domain.MasterData;
using Dashboard.Interface.MasterData;
using Dashboard.Core.ModelValidation;
using System.Web.Mvc;
using Dashboard.Domain.MVCExtension;
using Dashboard.Interface.BaseService;
using Dashboard.Core.DBTransaction;
using Dashboard.Core.Log4netHelper;
using log4net;
using System.Reflection;
using Dashboard.Core.Utility;
using Dashboard.Authentication;

namespace Dashboard.Service.Services.MasterData
{
    public class CBasCodeTypeService : DBBase, IBaseService<CBasCodeType>
    {

        public CBasCodeType Get(int id)
        {
            return null;
        }

        public CBasCodeType Get(string id)
        {
            return DataContext.CBasCodeType.FirstOrDefault(m => m.BasCodeTypeId == id);
        }

        public IQueryable<CBasCodeType> GetList()
        {
            return DataContext.CBasCodeType.OrderByDescending(r => r.UpdatedDate);
        }

        private void transaction(CBasCodeType cbascodetype)
        {
            if (string.IsNullOrEmpty(cbascodetype.BasCodeTypeId))
            {
                cbascodetype.BasCodeTypeId = Guid.NewGuid().ToString();
                cbascodetype.CreatedDate = DateTime.Now;
                DataContext.CBasCodeType.Add(cbascodetype);
            }
            DataContext.SaveChanges();

            var entity = DataContext.CBasCodeType.FirstOrDefault(c => c.BasCodeTypeId == cbascodetype.BasCodeTypeId);
        }

        public bool Save(CBasCodeType cbascodetype, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasCodeType>(cbascodetype);
                if (string.IsNullOrEmpty(cbascodetype.BasCodeTypeId))
                {
                    if (DataContext.CBasCodeType.Any(c => c.CodeType == cbascodetype.CodeType))
                    {
                        errMsg = "<=CodeTypeRepeat>";
                        return false;
                    }
                    cbascodetype.BasCodeTypeId = Guid.NewGuid().ToString();
                    cbascodetype.CreatedDate = DateTime.Now;
                    cbascodetype.UpdatedDate = DateTime.Now;
                    DataContext.CBasCodeType.Add(cbascodetype);
                }
                else
                {
                    if (DataContext.CBasCodeType.Any(c => c.CodeType == cbascodetype.CodeType &&
                                                     c.BasCodeTypeId != cbascodetype.BasCodeTypeId))
                    {
                        errMsg = "<=CodeTypeRepeat>";
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
                log.Info("Save方法出错。Author:huangbx ", ex);
                return false;
            }
        }

        public bool Delete(int id)
        {
            return false;
        }

        public bool Delete(string id)
        {
            try
            {
                var codetype = DataContext.CBasCodeType.FirstOrDefault(f => f.BasCodeTypeId == id);
                DataContext.CBasCodeType.Remove(codetype);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
         //获取到千欧元的汇率
        public string GetTEURParities()
        {
            return DataContext.CBasCodeDef.Where(e => e.CodeValue == "TEUR（千欧元)").Select(e => e.DisplayValueEn).First();
            
        }

        public IQueryable<CBasCodeType> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasCodeType> originalSource = DataContext.CBasCodeType.OrderByDescending(r => r.UpdatedDate);
            string CodeType = condition["CodeType"];
            if (!string.IsNullOrEmpty(CodeType))
                originalSource = originalSource.Where(o => o.CodeType.Contains(CodeType));

            string CodeName = condition["CodeName"];
            if (!string.IsNullOrEmpty(CodeName))
                originalSource = originalSource.Where(o => o.CodeName.Contains(CodeName));

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            //originalSource = originalSource.FilterSource().OrderBy(c=>c.UpdatedDate);
            var dataSouce = Paging<CBasCodeType>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        
    }
}
