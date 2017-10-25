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


namespace Dashboard.Service.Services.MasterData.SysManagement
{
    public class SysUserService : DBBase, ISysUser<SysUser>
    {
        #region ISysUser<SysUser> Members

        public SysUser Login(string userCode, string password)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;
            return DataContext.SysUser.FirstOrDefault(u => u.UserCode == userCode);
            //return DataContext.SysUser.FirstOrDefault(u => u.UserCode == userCode && u.Pwd == password);
        }

        public SysUser GetUserByCode(string userCode)
        {
            return DataContext.SysUser.FirstOrDefault(u => u.UserCode == userCode && u.Active == true);
        }

        #endregion

        #region IBaseService<SysUser> Members

        public SysUser Get(int id)
        {
            return null;
        }

        public SysUser Get(string id)
        {
            return DataContext.SysUser.FirstOrDefault(m => m.SysUserId == id);
        }

        #region GetList
        public IQueryable<SysUser> GetList()
        {
            return DataContext.SysUser.OrderByDescending(r => r.UserCode);
        }

        public IQueryable<SysUser> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<SysUser> originalSource = DataContext.SysUser.OrderByDescending(r => r.UserCode);
            string UserCode = condition["UserCode"];
            if (!UserCode.IsNullString())
                originalSource = originalSource.Where(o => o.UserCode.Contains(UserCode));

            string UserName = condition["UserName"];
            if (!UserName.IsNullString())
                originalSource = originalSource.Where(o => o.UserName.Contains(UserName));

            string Active = condition["Active"];
            if (!Active.IsNullString())
            {
                DateTime dt = DateTime.Today;
                if (Active == "0")
                {
                    originalSource = originalSource.Where(o => o.Active == false || o.ValidDateTo < dt || o.ValidDateFrom > dt);
                }
                if (Active == "1")
                {
                    originalSource = originalSource.Where(o => o.Active == true && o.ValidDateFrom <= dt && o.ValidDateTo >= dt);
                }
            }

            pageCount = 0;
            totalCount = originalSource.Count();
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);

            var dataSouce = Paging<SysUser>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        private string Transaction(SysUser sysUser)
        {
            try
            {
                if (sysUser.ValidDateTo < sysUser.ValidDateFrom)
                {
                    return "<=ValidDateFromMoreThanValidDateTo>";
                }
                if (string.IsNullOrEmpty(sysUser.SysUserId))
                {
                    if (DataContext.SysUser.Any(c => c.UserCode == sysUser.UserCode))
                    {
                        return "<=UserCodeRepeat>";
                    }
                    sysUser.SysUserId = Guid.NewGuid().ToString();
                    DataContext.SysUser.Add(sysUser);
                }

                if (DataContext.SysUserRole.Count(m => m.SysUserId == sysUser.SysUserId) <= 0)
                {
                    SysUserRole userRole = new SysUserRole();
                    userRole.SysUserRoleId = Guid.NewGuid().ToString();
                    userRole.SysUserId = sysUser.SysUserId;
                    userRole.SysRoleId = sysUser.RoleId;
                    DataContext.SysUserRole.Add(userRole);
                }

                DataContext.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public bool Save(SysUser sysUser, out string errMsg)
        {

            errMsg = "";
            try
            {
                string strError = "";
                bool result = DBTransactionExtension.Excute(out errMsg, () =>
                {
                    strError = Transaction(sysUser);
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

        #region Delete
        public bool Delete(int id)
        {
            return false;
        }

        public bool Delete(string id)
        {
            try
            {
                var user = DataContext.SysUser.FirstOrDefault(f => f.SysUserId == id);
                DataContext.SysUser.Remove(user);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        //public DataTable sysTeam(string sql)
        //{
 
        //}
        #endregion
    }
}
