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
using Dashboard.Core.Enumeration;

namespace Dashboard.Service.Services.MasterData.SysManagement
{
    public class SysFunctionService : DBBase, ISysFunction<SysFunction>
    {

        #region Fields
        List<SysOrganization> _resultEntities = new List<SysOrganization>();
        List<SysOrganization> _dataSource = null;
        #endregion

        #region Properties
        public List<SysOrganization> OrgSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = DataContext.SysOrganization.ToList();
                }
                return _dataSource;
            }

        }
        #endregion

        #region Private Methods
        private void GetIds(string id)
        {
            var tempEntities = OrgSource.Where(d => d.ParentId == id).ToList();
            if (tempEntities != null)
            {
                _resultEntities.AddRange(tempEntities);
                foreach (var data in tempEntities)
                {
                    GetIds(data.SysOrganizationId);
                }
            }

            _resultEntities.Add(OrgSource.FirstOrDefault(o => o.SysOrganizationId == id));
        }
        #endregion

        #region IBaseService<SysFunction> Members

        #region Get

        public SysFunction Get(int id)
        {
            return null;
        }

        public SysFunction Get(string id)
        {
            return DataContext.SysFunction.FirstOrDefault(m => m.SysFunctionId == id);
        }

        #endregion

        #region GetList
        public IQueryable<SysFunction> GetList()
        {
            return DataContext.SysFunction.OrderByDescending(m => m.SysFunctionId);
        }

        public IQueryable<SysFunction> GetList(GridParam gp, Dictionary<string, string> dicCondtion,
                                                 out int pageCount, out int totalCount)
        {

            IQueryable<SysFunction> originalSource = DataContext.SysFunction.OrderByDescending(r => r.SysFunctionId);

            string FunctionName = dicCondtion.ContainsKey("FunctionName") == true ? dicCondtion["FunctionName"] : "";
            if (!string.IsNullOrEmpty(FunctionName))
                originalSource = originalSource.Where(o => o.FunctionName.Contains(FunctionName));

            string ActionName = dicCondtion.ContainsKey("ActionName") == true ? dicCondtion["ActionName"] : "";
            if (!string.IsNullOrEmpty(ActionName))
                originalSource = originalSource.Where(o => o.ActionName.Contains(ActionName));

            string ControllerName = dicCondtion.ContainsKey("ControllerName") == true ? dicCondtion["ControllerName"] : "";
            if (!string.IsNullOrEmpty(ControllerName))
                originalSource = originalSource.Where(o => o.ControllerName.Contains(ControllerName));

            string ParentId = dicCondtion.ContainsKey("ParentId") == true ? dicCondtion["ParentId"] : "";
            if (!string.IsNullOrEmpty(ParentId))
                originalSource = originalSource.Where(o => o.ParentId.Contains(ParentId));

            string FunctionType = dicCondtion.ContainsKey("FunctionType") == true ? dicCondtion["FunctionType"] : "";
            if (!string.IsNullOrEmpty(FunctionType))
            {
                int iFuncType = Convert.ToInt16(FunctionType);
                originalSource = originalSource.Where(o => o.FunctionType == iFuncType);
            }

            string GroupType = dicCondtion.ContainsKey("GroupType") == true ? dicCondtion["GroupType"] : "";
            if (!string.IsNullOrEmpty(GroupType))
            {
                int igroupType = Convert.ToInt16(GroupType);
                originalSource = originalSource.Where(o => o.GroupType == igroupType);
            }

            pageCount = 0;
            totalCount = originalSource.Count();
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);

            var dataSouce = Paging<SysFunction>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(SysFunction sysFunction, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (string.IsNullOrEmpty(sysFunction.SysFunctionId))
                {
                    if (DataContext.SysFunction.Any(c => c.ControllerName == sysFunction.ControllerName && c.ActionName == sysFunction.ActionName && c.AreaName == sysFunction.AreaName))
                    {
                        errMsg = "<=FunctionNameRepeat>";
                        return false;
                    }
                    sysFunction.SysFunctionId = Guid.NewGuid().ToString();
                    DataContext.SysFunction.Add(sysFunction);
                }
                DataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("SysFunction Save方法出错。Author:huangyc ", ex);
                return false;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int functionId)
        {
            return false;
        }

        public bool Delete(string functionId)
        {
            try
            {
                var function = DataContext.SysFunction.FirstOrDefault(f => f.SysFunctionId == functionId);
                DataContext.SysFunction.Remove(function);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #endregion

        private IQueryable<SysFunction> GetFunctions(string sysRoleId)
        {
            var ids = DataContext.SysRoleFunction.Where(r => r.SysRoleId == sysRoleId).Select(rf => rf.SysFunctionId);
            var data = DataContext.SysFunction.Where(f => ids.Contains(f.SysFunctionId));
            return data;
        }

        public List<int> GetMenuGroup(string sysRoleId)
        {
            var menus = GetFunctions(sysRoleId);
            var groups = menus.OrderBy(o => o.GroupType).Select(m => m.GroupType).Distinct().ToList();

            var result = (from g in groups
                          select g).ToList();
            return result;
        }

        public List<SysFunction> GetMenuFromGroup(string sysRoleId, int intGroupType)
        {
            var menus = GetFunctions(sysRoleId);
            int intFunType = (int)FuntionType.Page;
            menus = menus.Where(m => m.GroupType == intGroupType && m.FunctionType == intFunType).OrderBy(o => o.Sortid);
            return menus.ToList();
        }

        public List<SysFunction> GetUserFunction(string sysRoleId)
        {
            int intType = (int)FuntionType.Function;
            List<SysFunction> result = GetFunctions(sysRoleId).Where(f => f.FunctionType == intType).ToList();
            return result;
        }

        public List<SysFunction> GetUserPermission(string sysRoleId)
        {
            List<SysFunction> result = GetFunctions(sysRoleId).ToList();
            return result;
        }

        public bool funcHasChild(string id)
        {
            return DataContext.SysFunction.Any(o => o.ParentId == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysRoleId">SYS_ROLE_ID:角色名称</param>
        /// <returns></returns>
        public List<string> GetOrganizationIds(string sysRoleId)
        {

            List<string> orgIds = DataContext.SysDataAuth.Where(a => a.SysRoleId == sysRoleId)
                              .Select(aa => aa.SysOrganizationId).ToList();
            List<string> result = null;

            _resultEntities = new List<SysOrganization>();
            foreach (string id in orgIds)
            {
                GetIds(id);
            }

            //result = _resultEntities.Select(r => r.SysOrganizationId).ToList();
            result = _resultEntities.Where(p => p.IsInterior).Select(r => r.SysOrganizationId).ToList();

            if (result != null)
                return result.Distinct().ToList();
            else
                return result;

        }

        public List<string> GetForwarderCodes(string sysRoleId)
        {
            List<string> orgIds = DataContext.SysDataAuth.Where(a => a.SysRoleId == sysRoleId)
                                   .Select(aa => aa.SysOrganizationId).ToList();
            List<string> result = null;

            _resultEntities = new List<SysOrganization>();
            foreach (string id in orgIds)
            {
                GetIds(id);
            }

            //result = _resultEntities.Select(r => r.OrganizationCode).ToList();
            result = _resultEntities.Where(p => !p.IsInterior).Select(r => r.OrganizationCode).ToList();

            if (result != null)
                return result.Distinct().ToList();
            else
                return result;

        }

        public IQueryable<SysFunction> GetList(Dictionary<string, string> condition)
        {
            IQueryable<SysFunction> originalSource = DataContext.SysFunction.Where(o => o.Active).OrderByDescending(r => r.Sortid);

            string ParentId = condition.ContainsKey("ParentId") == true ? condition["ParentId"] : "";
            if (!string.IsNullOrEmpty(ParentId))
                originalSource = originalSource.Where(o => o.ParentId.Contains(ParentId));

            string FunctionType = condition.ContainsKey("FunctionType") == true ? condition["FunctionType"] : "";
            if (!string.IsNullOrEmpty(FunctionType))
            {
                int iFuncType = Convert.ToInt16(FunctionType);
                originalSource = originalSource.Where(o => o.FunctionType == iFuncType);
            }

            return originalSource;
        }
    }
}
