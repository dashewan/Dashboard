using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dashboard.Core.FrogEntities;
using Dashboard.Core.Log4netHelper;
using Dashboard.Core.Utility;
using Dashboard.Domain.MasterData;
using Dashboard.Domain.MVCExtension;
using Dashboard.Interface.BaseService;
using log4net;

namespace Dashboard.Service.Services.MasterData
{
    public class CBasCityService : DBBase, IBaseService<CBasCity>
    {
        #region IBaseService<CBasCity> 成员

        #region Get
        public CBasCity Get(int id)
        {
            return null;
        }

        public CBasCity Get(string id)
        {
            return DataContext.BasCity.FirstOrDefault(m => m.BasCityId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasCity> GetList()
        {
            return DataContext.BasCity.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasCity> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasCity> originalSource = DataContext.BasCity;
            string CityCode = condition["CityCode"];
            if (!CityCode.IsNullString())
                originalSource = originalSource.Where(o => o.CityCode.Contains(CityCode));

            string CityName = condition["CityName"];
            if (!CityName.IsNullString())
                originalSource = originalSource.Where(o => o.CityName.Contains(CityName));

            string CityTypeCode = condition["CityTypeCode"];
            if (!CityTypeCode.IsNullString())
                originalSource = originalSource.Where(o => o.CityTypeCode == CityTypeCode);

            string ProvinceId = condition["ProvinceId"];
            if (!ProvinceId.IsNullString())
                originalSource = originalSource.Where(o => o.ProvinceId == ProvinceId);

            string Active = condition["Active"];
            if (Active=="Y")
                originalSource = originalSource.Where(o => o.Active);
            else if (Active == "N")
                originalSource = originalSource.Where(o => !o.Active);

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasCity>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasCity BasCity, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasCity>(BasCity);
                if (string.IsNullOrEmpty(BasCity.BasCityId))
                {
                    if (DataContext.BasCity.Any(c => c.CityCode == BasCity.CityCode))
                    {
                        errMsg = "<=CityCodeRepeat>";
                        return false;
                    }
                    BasCity.BasCityId = Guid.NewGuid().ToString();
                    DataContext.BasCity.Add(BasCity);
                }
                else
                {
                    if (DataContext.BasCity.Any(c => c.CityCode == BasCity.CityCode && c.BasCityId != BasCity.BasCityId))
                    {
                        errMsg = "<=CityCodeRepeat>";
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
                log.Info("BasCity Save方法出错。Author:chenjm ", ex);
                return false;
            }
            #region MyRegion
            /*
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }
            */

            #endregion
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
                var city = DataContext.BasCity.FirstOrDefault(f => f.BasCityId == id);
                if (city != null)
                {
                    DataContext.BasCity.Remove(city);
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
