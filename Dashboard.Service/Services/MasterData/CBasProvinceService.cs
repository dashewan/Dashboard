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
    public class CBasProvinceService : DBBase, IBaseService<CBasProvince>
    {

        public CBasProvince Get(int id)
        {
            return null;
        }

        public CBasProvince Get(string id)
        {
            return DataContext.CBasProvince.FirstOrDefault(m => m.BasProvinceId == id);
        }

        public IQueryable<CBasProvince> GetList()
        {
            return DataContext.CBasProvince.OrderByDescending(r => r.UpdatedDate);
        }

        private void transaction(CBasProvince cbasprovince)
        {
            if (string.IsNullOrEmpty(cbasprovince.BasProvinceId))
            {
                cbasprovince.BasProvinceId = Guid.NewGuid().ToString();
                cbasprovince.CreatedDate = DateTime.Now;
                DataContext.CBasProvince.Add(cbasprovince);
            }
            DataContext.SaveChanges();;
        }

        public bool Save(CBasProvince cbasprovince, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasProvince>(cbasprovince);
                if (string.IsNullOrEmpty(cbasprovince.BasProvinceId))
                {
                    if (DataContext.CBasProvince.Any(c => c.ProvinceCode == cbasprovince.ProvinceCode))
                    {
                        errMsg = "<=ProvincesCodeRepeat>";
                        return false;
                    }
                    cbasprovince.BasProvinceId = Guid.NewGuid().ToString();

                    cbasprovince.CreatedDate = DateTime.Now;
                    cbasprovince.UpdatedDate = DateTime.Now;
                    DataContext.CBasProvince.Add(cbasprovince);
                }
                else
                {
                    if (DataContext.CBasProvince.Any(c => c.ProvinceCode == cbasprovince.ProvinceCode &&
                                                     c.BasProvinceId != cbasprovince.BasProvinceId))
                    {
                        errMsg = "<=ProvincesCodeRepeat>";
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
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CBasProvince> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasProvince> originalSource = DataContext.CBasProvince.OrderByDescending(r => r.UpdatedDate);
            string ProvinceCode = condition["ProvinceCode"];
            if (!string.IsNullOrEmpty(ProvinceCode))
                originalSource = originalSource.Where(o => o.ProvinceCode.Contains(ProvinceCode));
            //省中文名称
            string ProvinceName = condition["ProvinceName"];
            if (!string.IsNullOrEmpty(ProvinceName))
                originalSource = originalSource.Where(o => o.ProvinceName.Contains(ProvinceName));
            //省会
            string ProvinceNameEn = condition["ProvinceNameEn"];
            if (!string.IsNullOrEmpty(ProvinceNameEn))
                originalSource = originalSource.Where(o => o.ProvinceNameEn.Contains(ProvinceNameEn));
            //是否有效
            string Actives = condition["Actives"];
            if (!string.IsNullOrEmpty(Actives))
            {
                if (Actives.ToUpper() == "TRUE")
                {
                    originalSource = originalSource.Where(o => o.Active == true);
                }
                else
                {
                    originalSource = originalSource.Where(o => o.Active == false);
                }
            }
            
            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();

            var dataSouce = Paging<CBasProvince>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;

        }
    }
}
