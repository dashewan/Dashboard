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
using Dashboard.Domain.ValidationAttributes;
namespace Dashboard.Service.Services.MasterData
{
    public class CSbCodeDefService : DBBase, IBaseService<CBasCodeDef>
    {

        public CBasCodeDef Get(int id)
        {
            return null;
        }

        public CBasCodeDef Get(string id)
        {
            return DataContext.CBasCodeDef.FirstOrDefault(m => m.BasCodeDefId == id);
        }

        public IQueryable<CBasCodeDef> GetList()
        {
            return DataContext.CBasCodeDef.OrderByDescending(r => r.UpdatedDate);
        }

        private void transaction(CBasCodeDef cbascodedef)
        {
            if (string.IsNullOrEmpty(cbascodedef.BasCodeDefId))
            {
                cbascodedef.BasCodeDefId = Guid.NewGuid().ToString();
                cbascodedef.CreatedDate = DateTime.Now;
                DataContext.CBasCodeDef.Add(cbascodedef);
            }
            DataContext.SaveChanges();

           // var entity = DataContext.CSbCodeType.FirstOrDefault(c => c.SbCodeTypeId == csbcodedef.SbCodeDefId);
        }

        public bool Save(CBasCodeDef cbascodedef, out string errMsg)
        {
            errMsg = "";
            try
            {
                Dashboard.Authentication.Authentication.UpdateEntity<CBasCodeDef>(cbascodedef);
                if (string.IsNullOrEmpty(cbascodedef.BasCodeDefId))
                {
                    if (DataContext.CBasCodeDef.Any(c => c.CodeValue == cbascodedef.CodeValue && c.BasCodeTypeId==cbascodedef.BasCodeTypeId))
                    {
                        errMsg = "<=CodeTypeRepeat>";
                        return false;
                    }
                    cbascodedef.BasCodeDefId = Guid.NewGuid().ToString();

                    cbascodedef.CreatedDate = DateTime.Now;
                    cbascodedef.UpdatedDate = DateTime.Now;

                    DataContext.CBasCodeDef.Add(cbascodedef);
                }
                else
                {
                    if (DataContext.CBasCodeDef.Any(c => c.CodeValue == cbascodedef.CodeValue &&
                                                     c.BasCodeDefId != cbascodedef.BasCodeDefId && c.BasCodeTypeId == cbascodedef.BasCodeTypeId))
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
                var csbcodedef = DataContext.CBasCodeDef.FirstOrDefault(f => f.BasCodeDefId == id);
                DataContext.CBasCodeDef.Remove(csbcodedef);
                DataContext.SaveChanges();
                return true;
            }
            catch
            {
                
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("Delete方法出错。Author:huangbx ");
                return false;
            }
        }
        public IQueryable<comboDescriptor> GetList(string typeCode)
        {
            IQueryable<comboDescriptor> defSource = DataContext.CBasCodeDef.Join(DataContext.CBasCodeType, d => d.BasCodeTypeId, t => t.BasCodeTypeId, (d, t) => new { d.CodeValue, t.BasCodeDef, t.CodeType, d.DisplayValue, d.DisplayValueEn, d.Active }).Where(e => e.CodeType == typeCode && e.Active == true).Select(e => new comboDescriptor { Id = e.CodeValue, Text = e.DisplayValue }).OrderBy(f => f.Text);
            return defSource;
            
        }

        public IQueryable<CBasCodeDef> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasCodeDef> originalSource = DataContext.CBasCodeDef.OrderByDescending(r => r.UpdatedDate);
            string CodeDef = condition["SbCodeTypeId"];
            if (!string.IsNullOrEmpty(CodeDef))
                originalSource = originalSource.Where(o => o.BasCodeTypeId.Contains(CodeDef));

            ////是否有效
            //string Active = condition["Active"];
            //if (!string.IsNullOrEmpty(Active))
            //{
            //    if (Active.ToUpper() == "true")
            //    {
            //        originalSource = originalSource.Where(o => o.Active == true);
            //    }
            //    else
            //    {
            //        originalSource = originalSource.Where(o => o.Active == false);
            //    }
            //}
            //else
            //{
            //    originalSource = originalSource.Where(o => o.Active == true);
            //}

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            //originalSource = originalSource.FilterSource().OrderBy(c=>c.UpdatedDate);
            var dataSouce = Paging<CBasCodeDef>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }

        public bool Exists(string val)
        {
            int count= DataContext.CBasCodeDef.Join(DataContext.CBasCodeType, d => d.BasCodeTypeId, t => t.BasCodeTypeId, (d, t) => new { d.CodeValue,t.CodeType,d.DisplayValue,d.DisplayValueEn }).Where(e=>e.CodeType == "ITMember" && (e.DisplayValue==val||e.DisplayValueEn==val)).Count();
            return count>0?true:false;
        }
    }
}
