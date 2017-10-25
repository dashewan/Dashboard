using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Core.FrogEntities;
using Dashboard.Domain.SysManagement;
using Dashboard.Interface.MasterData.SysManagement;
using Dashboard.Core.ModelValidation;
using System.Web.Mvc;
using Dashboard.Domain.MVCExtension;
using Dashboard.Core.DBTransaction;
using Dashboard.Core.Utility;
using Dashboard.Core.Log4netHelper;
using log4net;
using System.Reflection;
using Dashboard.Interface.BaseService;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace Dashboard.Service.Services.MasterData.SysManagement
{
    public class SysOrgService : DBBase, ISysOrganization<SysOrganization>
    {

        #region Private Methods
        private bool UsedByOtherModule(string orgId, out string msg)
        {
            bool result = false;
            msg = string.Empty;
            result =DataContext.SysDataAuth.Any(d => d.SysOrganizationId == orgId);
            if (result)
            {
                msg = "<=AuthOfOrgHaveBeenUsed>";
                return true;
            }

            result = DataContext.SysRole.Any(r => r.SysOrganizationId == orgId);
            if (result)
            {
                msg = "<=RoleOfOrgHaveBeenUsed>";
                return true;
            }

            return result;
        }
        #endregion

        #region IBaseService<Organization> Members

        public SysOrganization Get(int id)
        {
            throw new NotImplementedException();
        }

        public SysOrganization Get(string id)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;
            return DataContext.SysOrganization.FirstOrDefault(c => c.SysOrganizationId == id);
        }

        public static Func<T, string> GetField<T>(string field)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(field);
            return obj => Convert.ToString(propertyInfo.GetValue(obj, null));
        }

        public IQueryable<SysOrganization> GetList()
        {
            //IQueryable<Organization> dataSource = DataContext.SysOrganization.Where(c => 1 == 1).OrderBy(c => c.CreateDate);
            //dataSource = Authentication.FilterSource<Organization>(dataSource);
            IQueryable<SysOrganization> dataSource = DataContext.SysOrganization.Where(c => 1 == 1).OrderBy(c => c.CreateDate);
            return dataSource;
        }

        public bool Save(SysOrganization entity, out string errMsg)
        {
            errMsg = "<=SaveSuccess>";
            try
            {
                if (entity.SysOrganizationId.IsNullString())
                {
                    if (DataContext.SysOrganization.Any(o => o.OrganizationName == entity.OrganizationName))
                    {
                        errMsg = "<=OrgNameExist>";
                        return false;
                    }

                    if (DataContext.SysOrganization.Any(o => o.OrganizationCode == entity.OrganizationCode))
                    {
                        errMsg = "<=OrgCodeExist>";
                        return false;
                    }

                    //if (entity.IsInterior == false && !DataContext.CimCust.Any(o => o.CustCode == entity.OrganizationCode && o.CustAttribute == "2"))
                    //{
                    //    errMsg = "<=OutInteriorOrganizationCodeError>";
                    //    return false;
                    //}

                    entity.SysOrganizationId = Guid.NewGuid().ToString();
                    entity.CreateDate = DateTime.Now;
                    DataContext.SysOrganization.Add(entity);
                }
                else
                {
                    bool exit = DataContext.SysOrganization.Any(c => c.OrganizationCode == entity.OrganizationCode
                                                         && c.SysOrganizationId != entity.SysOrganizationId);
                    if (exit)
                    {
                        errMsg = "<=OrgCodeExist>";
                        return false;
                    }

                    exit = DataContext.SysOrganization.Any(c => c.OrganizationName == entity.OrganizationName
                                                        && c.SysOrganizationId != entity.SysOrganizationId);
                    if (exit)
                    {
                        errMsg = "<=OrgNameExist>";
                        return false;
                    }
                }

                DataContext.SaveChanges();
                return true;
            }
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
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("CBasCurrency Save方法出错。Author:ryanding ", ex);
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

        public bool Delete(String id, out string errMsg)
        {
            try
            {
                if (UsedByOtherModule(id, out errMsg))
                {
                    return false;
                }

                if (DataContext.SysOrganization.FirstOrDefault(c => c.SysOrganizationId == id).ParentId == null)
                {
                    errMsg = "<=RootDeleteFail>";
                    return false;
                }

                if (DataContext.SysOrganization.Any(c => c.ParentId == id))
                {
                    errMsg = "<=NodeDeleteFail>";
                    return false;
                }

                var org = DataContext.SysOrganization.FirstOrDefault(c => c.SysOrganizationId == id);
                DataContext.SysOrganization.Remove(org);
                DataContext.SaveChanges();
                errMsg = "<=DeleteSuccess>";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public IQueryable<SysOrganization> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool HasChild(string id)
        {
            return DataContext.SysOrganization.Any(o => o.ParentId == id);
        }

        public IQueryable<MyClass> TestMethod()
        {
            var result = (from d in DataContext.SysOrganization
                          select new MyClass
                          {
                              OrgId = d.SysOrganizationId,
                              OrgName = d.OrganizationName,
                              Count = DataContext.SysRole.Where(s => s.SysOrganizationId == d.SysOrganizationId).Count()
                          }).OrderBy(c => c.OrgId);

            return result;

        }
    }
}
