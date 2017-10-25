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
using System.Data;
using Dashboard.Core.Utility.SqlHelper;

namespace Dashboard.Service.Services.MasterData.SysManagement
{
    #region SysRoleService
    public class SysRoleService : DBBase, ISysRole<SysRole>
    {
        #region IBaseService<SysRole> Members

        #region Get
        public SysRole Get(int id)
        {
            return null;
        }

        public SysRole Get(string id)
        {
            return DataContext.SysRole.FirstOrDefault(m => m.SysRoleId == id);
        }
        #endregion

        #region GetList
        public IQueryable<SysRole> GetList()
        {
            return DataContext.SysRole.OrderByDescending(r => r.RoleCode);
        }

        public IQueryable<SysRole> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<SysRole> originalSource = DataContext.SysRole.OrderByDescending(r => r.RoleCode);

            string RoleCode = condition.ContainsKey("RoleCode") == true ? condition["RoleCode"] : "";
            if (!RoleCode.IsNullString())
                originalSource = originalSource.Where(o => o.RoleCode.Contains(RoleCode));

            string RoleName = condition.ContainsKey("RoleName") == true ? condition["RoleName"] : "";
            if (!RoleName.IsNullString())
                originalSource = originalSource.Where(o => o.RoleName.Contains(RoleName));

            //string SysOrganizationId = condition.ContainsKey("SysOrganizationId") == true ? condition["SysOrganizationId"] : "";
            //if (!SysOrganizationId.IsNullString())
            //    originalSource = originalSource.Where(o => o.SysOrganizationId == SysOrganizationId);

            //string OrganizationName = condition.ContainsKey("OrganizationName") == true ? condition["OrganizationName"] : "";
            //if (!OrganizationName.IsNullString())
            //    originalSource = originalSource.Where(o => o.SysOrganization.OrganizationName.Contains(OrganizationName));

            pageCount = 0;
            totalCount = originalSource.Count();
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);

            var dataSouce = Paging<SysRole>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(SysRole sysRole, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (!string.IsNullOrEmpty(sysRole.SysRoleId))
                {
                    if (DataContext.SysOrganization.Count(o => o.SysOrganizationId == sysRole.SysOrganizationId && o.IsInterior == false) > 0)
                    {
                        if (DataContext.SysDataAuth.Count(u => u.SysRoleId == sysRole.SysRoleId && u.SysOrganization.IsInterior == true) > 0)
                        {
                            errMsg = "<=AuthDataExistInOrg>";
                            return false;
                        }
                    }
                }

                if (string.IsNullOrEmpty(sysRole.SysRoleId))
                {
                    if (DataContext.SysRole.Any(c => c.RoleCode == sysRole.RoleCode))
                    {
                        errMsg = "<=RoleCodeRepeat>";
                        return false;
                    }
                    sysRole.SysRoleId = Guid.NewGuid().ToString();
                    DataContext.SysRole.Add(sysRole);
                }
                else
                {
                    if (DataContext.SysRole.Any(c => c.RoleCode == sysRole.RoleCode && c.SysRoleId != sysRole.SysRoleId))
                    {
                        errMsg = "<=RoleCodeRepeat>";
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
                log.Info("SysRole Save方法出错。Author:huangyc ", ex);
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
                var role = DataContext.SysRole.FirstOrDefault(f => f.SysRoleId == id);
                DataContext.SysRole.Remove(role);
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

        #region GetRoleFunctionExport
        /// <summary>
        /// 获取角色功能数据源导出Excel
        /// </summary>
        /// <returns></returns>
        public List<CRoleFunctionExport> GetRoleFunctionExport()
        {
            try
            {
                DataSet ds = SpExecution.ExecuteDataset("SP_ROLE_FUNCTION_EXPORT");

                List<CRoleFunctionExport> lstRoleFunctionExport = new List<CRoleFunctionExport>();
                if (ds.Tables.Count > 0)
                    lstRoleFunctionExport = ConvertHelper<CRoleFunctionExport>.ConvertToList(ds.Tables[0], true);
                return lstRoleFunctionExport;
            }
            catch (Exception ex)
            {
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("GetRoleFunctionExport方法出错。Author:chenjm ", ex);
                return null;
            }
        }
        #endregion

        #region GetRoleUserExport
        /// <summary>
        /// 获取角色用户数据源导出Excel
        /// </summary>
        /// <returns></returns>
        public List<CRoleUserExport> GetRoleUserExport()
        {
            try
            {
                DataSet ds = SpExecution.ExecuteDataset("SP_ROLE_USER_EXPORT");

                List<CRoleUserExport> lstRoleUserExport = new List<CRoleUserExport>();
                if (ds.Tables.Count > 0)
                    lstRoleUserExport = ConvertHelper<CRoleUserExport>.ConvertToList(ds.Tables[0], true);
                return lstRoleUserExport;
            }
            catch (Exception ex)
            {
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("GetRoleUserExport方法出错。Author:chenjm ", ex);
                return null;
            }
        }
        #endregion
    }
    #endregion

    #region SysUserRoleService
    public class SysUserRoleService : DBBase, ISysUserRole<SysUserRole>
    {
        #region IBaseService<SysUserRole> Members

        #region Get
        public SysUserRole Get(int id)
        {
            return null;
        }

        public SysUserRole Get(string id)
        {
            return DataContext.SysUserRole.FirstOrDefault(m => m.SysUserRoleId == id);
        }
        #endregion

        #region GetList
        public IQueryable<SysUserRole> GetList()
        {
            return DataContext.SysUserRole.OrderByDescending(r => r.SysUserRoleId);
        }

        public IQueryable<SysUserRole> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<SysUserRole> originalSource = DataContext.SysUserRole.OrderByDescending(r => r.SysUserRoleId);

            string SysRoleId = condition.ContainsKey("SysRoleId") == true ? condition["SysRoleId"] : "";
            if (!SysRoleId.IsNullString())
                originalSource = originalSource.Where(o => o.SysRoleId == SysRoleId);

            string SysUserId = condition.ContainsKey("SysUserId") == true ? condition["SysUserId"] : "";
            if (!SysUserId.IsNullString())
                originalSource = originalSource.Where(o => o.SysUserId == SysUserId);

            pageCount = 0;
            totalCount = originalSource.Count();

            var dataSouce = Paging<SysUserRole>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(SysUserRole sysUserRole, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (string.IsNullOrEmpty(sysUserRole.SysUserRoleId))
                {
                    if (DataContext.SysUserRole.Any(c => c.SysRoleId == sysUserRole.SysRoleId && c.SysUserId == sysUserRole.SysUserId))
                    {
                        errMsg = "<=SysUserRoleRepeat>";
                        return false;
                    }
                    sysUserRole.SysUserRoleId = Guid.NewGuid().ToString();
                    DataContext.SysUserRole.Add(sysUserRole);
                }

                DataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("SysUserRole Save方法出错。Author:huangyc ", ex);
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
                var userRole = DataContext.SysUserRole.FirstOrDefault(f => f.SysUserRoleId == id);
                DataContext.SysUserRole.Remove(userRole);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string id, out string errMsg)
        {
            try
            {
                errMsg = "";
                var userRole = DataContext.SysUserRole.FirstOrDefault(f => f.SysUserRoleId == id);

                if (DataContext.SysUser.Count(m => m.RoleId == userRole.SysRoleId && m.SysUserId == userRole.SysUserId) > 0)
                {
                    errMsg = "<=DefaultRoleCannotBeDelete>";
                    return false;
                }

                DataContext.SysUserRole.Remove(userRole);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                errMsg = "<=DeleteFail>";
                return false;
            }
        }

        /// <summary>
        /// 根据RoleId删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteByRoleId(string id) 
        {
            try
            {
                var userRole = DataContext.SysUserRole.FirstOrDefault(f => f.SysRoleId == id);
                if (userRole == null)
                    return true;
                DataContext.SysUserRole.Remove(userRole);
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

        #region GetList
        public IQueryable<SysUserRole> GetList(Dictionary<string, string> condition)
        {
            IQueryable<SysUserRole> originalSource = DataContext.SysUserRole.OrderByDescending(r => r.SysUserRoleId);

            string SysRoleId = condition.ContainsKey("SysRoleId") == true ? condition["SysRoleId"] : "";
            if (!SysRoleId.IsNullString())
                originalSource = originalSource.Where(o => o.SysRoleId == SysRoleId);

            string SysUserId = condition.ContainsKey("SysUserId") == true ? condition["SysUserId"] : "";
            if (!SysUserId.IsNullString())
                originalSource = originalSource.Where(o => o.SysUserId == SysUserId);

            return originalSource;
        }
        #endregion
    }
    #endregion

    #region SysRoleFunctionService
    public class SysRoleFunctionService : DBBase, ISysRoleFunction<SysRoleFunction>
    {
        #region IBaseService<SysRoleFunction> Members

        #region Get
        public SysRoleFunction Get(int id)
        {
            return null;
        }

        public SysRoleFunction Get(string id)
        {
            return DataContext.SysRoleFunction.FirstOrDefault(m => m.SysRoleFunctionId == id);
        }
        #endregion

        #region GetList
        public IQueryable<SysRoleFunction> GetList()
        {
            return DataContext.SysRoleFunction.OrderByDescending(r => r.SysRoleFunctionId);
        }

        public IQueryable<SysRoleFunction> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<SysRoleFunction> originalSource = DataContext.SysRoleFunction.OrderByDescending(r => r.SysRoleFunctionId);

            string SysRoleId = condition["SysRoleId"];
            if (!SysRoleId.IsNullString())
                originalSource = originalSource.Where(o => o.SysRoleId.Contains(SysRoleId));

            pageCount = 0;
            totalCount = originalSource.Count();

            var dataSouce = Paging<SysRoleFunction>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(SysRoleFunction roleFunc, out string errMsg)
        {
            throw new NotImplementedException();
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
                var roleFunction = DataContext.SysRoleFunction.FirstOrDefault(f => f.SysRoleFunctionId == id);
                DataContext.SysRoleFunction.Remove(roleFunction);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteByRoleId(string id) 
        {
            try
            {
                var roleFunction = DataContext.SysRoleFunction.FirstOrDefault(f => f.SysRoleId == id);
                if (roleFunction == null)
                    return true;
                DataContext.SysRoleFunction.Remove(roleFunction);
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

        #region GetRoleFunc
        public string GetRoleFunc(string sFunctionId, string sRoleId)
        {
            var sRoleFunctionId = "";
            SysRoleFunction roleFunction = DataContext.SysRoleFunction.FirstOrDefault(m => m.SysFunctionId == sFunctionId && m.SysRoleId == sRoleId);
            if (roleFunction != null)
            {
                sRoleFunctionId = roleFunction.SysRoleFunctionId;
            }
            return sRoleFunctionId;
        }
        #endregion

        #region Transaction
        private string Transaction(List<SysRoleFunction> roleFunc, string MenuId, bool deleteAllFlag)
        {
            try
            {
                SysRoleFunction tmpRoleFunc = null;

                if (roleFunc.Count > 0)
                {
                    string tmp = roleFunc[0].SysRoleId;
                    tmpRoleFunc = DataContext.SysRoleFunction.FirstOrDefault(n => n.SysFunctionId == MenuId && n.SysRoleId == tmp);
                    if (deleteAllFlag)
                    {
                        if (tmpRoleFunc != null)
                        {
                            DataContext.SysRoleFunction.Remove(tmpRoleFunc);
                        }
                    }
                    else
                    {
                        if (tmpRoleFunc == null)
                        {
                            tmpRoleFunc = new SysRoleFunction();
                            tmpRoleFunc.SysFunctionId = MenuId;
                            tmpRoleFunc.SysRoleId = roleFunc[0].SysRoleId;
                            tmpRoleFunc.SysRoleFunctionId = Guid.NewGuid().ToString();
                            DataContext.SysRoleFunction.Add(tmpRoleFunc);
                        }
                    }
                }

                foreach (SysRoleFunction item in roleFunc)
                {
                    DataContext.SysRoleFunction.Attach(item);
                    if (item.SysFunctionId != "")
                    {
                        DataContext.Entry(item).State = string.IsNullOrEmpty(item.SysRoleFunctionId) ? EntityState.Added : EntityState.Modified;
                        if (string.IsNullOrEmpty(item.SysRoleFunctionId))
                        {
                            item.SysRoleFunctionId = Guid.NewGuid().ToString();
                        }
                    }
                    else
                    {
                        DataContext.SysRoleFunction.Remove(item);
                    }
                }

                DataContext.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Save
        public bool Save(List<SysRoleFunction> roleFunc, string MenuId, bool deleteAllFlag, out string errMsg)
        {
            errMsg = "";
            try
            {
                string strError = "";
                bool result = DBTransactionExtension.Excute(out errMsg, () =>
                {
                    strError = Transaction(roleFunc, MenuId, deleteAllFlag);
                });
                if (strError != "")
                {
                    errMsg = strError;
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";

                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("Save方法出错。Author:huangyc ", ex);

                return false;
            }
        }
        #endregion
    }
    #endregion

    #region SysDataAuthService
    public class SysDataAuthService : DBBase, ISysDataAuth<SysDataAuth>
    {
        #region IBaseService<SysDataAuth> Members

        #region Get
        public SysDataAuth Get(int id)
        {
            return null;
        }

        public SysDataAuth Get(string id)
        {
            return DataContext.SysDataAuth.FirstOrDefault(m => m.SysDataAuthId == id);
        }
        #endregion

        #region GetList
        public IQueryable<SysDataAuth> GetList()
        {
            return DataContext.SysDataAuth.OrderByDescending(r => r.SysDataAuthId);
        }

        public IQueryable<SysDataAuth> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<SysDataAuth> originalSource = DataContext.SysDataAuth.OrderByDescending(r => r.SysDataAuthId);

            string SysRoleId = condition["SysRoleId"];
            if (!SysRoleId.IsNullString())
                originalSource = originalSource.Where(o => o.SysRoleId.Contains(SysRoleId));

            pageCount = 0;
            totalCount = originalSource.Count();

            var dataSouce = Paging<SysDataAuth>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(SysDataAuth dataAuth, out string errMsg)
        {
            throw new NotImplementedException();
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
                var dataAuth = DataContext.SysDataAuth.FirstOrDefault(f => f.SysDataAuthId == id);
                DataContext.SysDataAuth.Remove(dataAuth);
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

        #region GetList
        public IQueryable<SysDataAuth> GetList(Dictionary<string, string> condition)
        {
            IQueryable<SysDataAuth> originalSource = DataContext.SysDataAuth.OrderByDescending(r => r.SysDataAuthId);
            string SysRoleId = condition["SysRoleId"];
            originalSource = originalSource.Where(o => o.SysRoleId == SysRoleId);
            return originalSource;
        }
        #endregion

        #region Transaction
        private string Transaction(List<SysDataAuth> lstdataAuth)
        {
            try
            {
                bool isInOrg = true;
                SysRole role = null;
                if (lstdataAuth.Count > 0)
                {
                    string sysRoleId = lstdataAuth[0].SysRoleId;
                    role = DataContext.SysRole.FirstOrDefault(p => p.SysRoleId == sysRoleId);
                    if (role != null)
                    {
                        isInOrg = role.SysOrganization.IsInterior;
                    }
                }

                foreach (SysDataAuth item in lstdataAuth)
                {
                    if (!isInOrg)
                    {
                        string orgId = item.SysOrganizationId;
                        if (DataContext.SysOrganization.Count(g => g.SysOrganizationId == orgId && g.IsInterior == true) > 0)
                        {
                            return "<=OrgOfAuthDataIsError>";
                        }
                    }

                    DataContext.SysDataAuth.Attach(item);
                    DataContext.Entry(item).State = string.IsNullOrEmpty(item.SysDataAuthId) ? EntityState.Added : EntityState.Modified;
                    if (string.IsNullOrEmpty(item.SysDataAuthId))
                    {
                        item.SysDataAuthId = Guid.NewGuid().ToString();
                    }
                    DataContext.SaveChanges();
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Save
        public bool Save(List<SysDataAuth> lstdataAuth, out string errMsg)
        {
            errMsg = "";
            try
            {
                string strError = "";
                bool result = DBTransactionExtension.Excute(out errMsg, () =>
                {
                    strError = Transaction(lstdataAuth);
                });
                if (strError != "")
                {
                    errMsg = strError;
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";

                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("Save方法出错。Author:huangyc ", ex);

                return false;
            }
        }
        #endregion
    }
    #endregion

}
