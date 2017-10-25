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
using Dashboard.Common.Util;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace Dashboard.Service.Services.BasicData
{
    public partial class CBasNormalScheduleService : DBBase, IBasNormalSchedule<CBasNormalSchedule>
    {
        #region IBaseService<CBasNormalSchedule> 成员
        #region Get
        public CBasNormalSchedule Get(int id)
        {
            return null;
        }

        public CBasNormalSchedule Get(string id)
        {
            return DataContext.CBasNormalSchedule.FirstOrDefault(m => m.BasNormalScheduleId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasNormalSchedule> GetList()
        {
            return DataContext.CBasNormalSchedule.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasNormalSchedule> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasNormalSchedule> originalSource = DataContext.CBasNormalSchedule;

            string scheduleNo = condition["ScheduleNo"];
            if (!string.IsNullOrEmpty(scheduleNo))
                originalSource = originalSource.Where(o => o.ScheduleNo.Contains(scheduleNo));

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasNormalSchedule>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasNormalSchedule BasNormalSchedule, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (BasNormalSchedule.EffectiveDate > BasNormalSchedule.ExpirationDate)
                {
                    errMsg = "<=EffectiveDateGreaterThanExpirationDate>";
                    return false;
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalSchedule>(BasNormalSchedule);
                if (string.IsNullOrEmpty(BasNormalSchedule.BasNormalScheduleId))
                {
                    if (DataContext.CBasNormalSchedule.Any(c => c.ScheduleNo == BasNormalSchedule.ScheduleNo))
                    {
                        errMsg = "<=ScheduleNoRepeat>";
                        return false;
                    }
                    //判断生效与失效日期是否有重叠
                    if (DataContext.CBasNormalSchedule.Any(c => ((c.EffectiveDate >= BasNormalSchedule.EffectiveDate
                                                              && c.EffectiveDate <= BasNormalSchedule.ExpirationDate)
                                                             || (c.EffectiveDate <= BasNormalSchedule.EffectiveDate
                                                              && c.ExpirationDate >= BasNormalSchedule.ExpirationDate)
                                                              || (c.ExpirationDate >= BasNormalSchedule.EffectiveDate
                                                              && c.ExpirationDate <= BasNormalSchedule.ExpirationDate))))
                    {
                        errMsg = "<=ScheduleRepeat>";
                        return false;
                    }
                    BasNormalSchedule.BasNormalScheduleId = Guid.NewGuid().ToString();
                    DataContext.CBasNormalSchedule.Add(BasNormalSchedule);
                }
                else
                {
                    if (DataContext.CBasNormalSchedule.Any(c => c.ScheduleNo == BasNormalSchedule.ScheduleNo && c.BasNormalScheduleId != BasNormalSchedule.BasNormalScheduleId))
                    {
                        errMsg = "<=ScheduleNoRepeat>";
                        return false;
                    }
                    if (DataContext.CBasNormalSchedule.Any(c => ((c.EffectiveDate >= BasNormalSchedule.EffectiveDate
                                                              && c.EffectiveDate <= BasNormalSchedule.ExpirationDate)
                                                             || (c.EffectiveDate <= BasNormalSchedule.EffectiveDate
                                                              && c.ExpirationDate >= BasNormalSchedule.ExpirationDate)
                                                              || (c.ExpirationDate >= BasNormalSchedule.EffectiveDate
                                                              && c.ExpirationDate <= BasNormalSchedule.ExpirationDate))
                                                              && c.BasNormalScheduleId != BasNormalSchedule.BasNormalScheduleId))
                    {
                        errMsg = "<=ScheduleRepeat>";
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
                log.Info("BasNormalSchedule Save方法出错。Author:chenjm ", ex);
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
                var BasNormalSchedule = DataContext.CBasNormalSchedule.FirstOrDefault(f => f.BasNormalScheduleId == id);
                if (BasNormalSchedule != null)
                {
                    DataContext.CBasNormalSchedule.Remove(BasNormalSchedule);
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

        #region IBasNormalSchedule<CBasNormalSchedule> 成员
        #region NormalScheduleImport
        public void NormalScheduleImport(string basNormalScheduleId, string strPath, string type, out string errMsg)
        {
            errMsg = "<=ImportSuccess>";
            var basNormalSchedule = DataContext.CBasNormalSchedule.FirstOrDefault(p => p.BasNormalScheduleId == basNormalScheduleId);

            if (basNormalSchedule == null)
            {
                errMsg = "<=NotExistsScheduleHeadInfo>";
                return;
            }
            try
            {
                DataContext.Configuration.AutoDetectChangesEnabled = false;
                DataContext.Configuration.ValidateOnSaveEnabled = false;

                var args = new DbParameter[] { 
                    new SqlParameter ("@id",basNormalScheduleId)
                 };

                switch (type)
                {
                    case "Normal Schedule S":
                        //存储文件路径到BAS_NORMAL_SCHEDULE表的NORMAL_SCHEDULE_S_FILE_NAME字段
                        basNormalSchedule.NormalScheduleSFileName = strPath;
                        DataContext.Database.ExecuteSqlCommand("delete BAS_NORMAL_SCHEDULE_S where BAS_NORMAL_SCHEDULE_ID = @id", args);
                        //导入
                        NormalScheduleSImport(basNormalScheduleId, strPath);
                        break;
                    case "Normal Schedule V Cargo Type":
                        //存储文件路径到BAS_NORMAL_SCHEDULE表的NORMAL_SCHEDULE_V_CT_FILE_NAME字段
                        basNormalSchedule.NormalScheduleVCtFileName = strPath;
                        
                        DataContext.Database.ExecuteSqlCommand(@"delete BAS_NORMAL_SCHEDULE_V_CT where BAS_NORMAL_SCHEDULE_ID = @id;
                                                                 delete BAS_NORMAL_SCHEDULE_V_CT_DET where BAS_NORMAL_SCHEDULE_ID = @id;", args);
                        NormalScheduleVCtImport(basNormalScheduleId, strPath);
                        break;
                    case "Normal Schedule V Trans Mode":
                        //存储文件路径到BAS_NORMAL_SCHEDULE表的NORMAL_SCHEDULE_V_TM_FILE_NAME字段
                        basNormalSchedule.NormalScheduleVTmFileName = strPath;
                        DataContext.Database.ExecuteSqlCommand(@"delete BAS_NORMAL_SCHEDULE_V_TM where BAS_NORMAL_SCHEDULE_ID = @id;
                                                                 delete BAS_NORMAL_SCHEDULE_V_TM_DET where BAS_NORMAL_SCHEDULE_ID = @id;", args);
                        NormalScheduleVTmImport(basNormalScheduleId, strPath);
                        break;
                    default:
                        break;
                }
                //需要手动标记basNormalSchedule的状态为modified,否则不能更新到
                DataContext.Entry<CBasNormalSchedule>(basNormalSchedule).State = System.Data.EntityState.Modified; 
                DataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                errMsg = "<=ImportError>:" + ex.Message;

            }
            finally
            {
                DataContext.Configuration.AutoDetectChangesEnabled = true;
                DataContext.Configuration.ValidateOnSaveEnabled = true;
            }
        }
        #endregion

        #endregion

        #region NormalScheduleSImport
        /// <summary>
        /// Normal Schedule S Import
        /// </summary>
        /// <param name="basNormalScheduleId"></param>
        /// <param name="strPath"></param>
        private void NormalScheduleSImport(string basNormalScheduleId, string strPath)
        {
            try
            {
                NpoiHelper npoiHelper = new NpoiHelper(strPath, false);
                DataTable dt = npoiHelper.ExcelToDataTable(0);
                Dictionary<string, string> dicDealerCode = new Dictionary<string, string>();
                Dictionary<string, string> dicDealerCodePickupDay = new Dictionary<string, string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    int index = 2;
                    foreach (DataRow row in dt.Rows)
                    {
                        NormalScheduleSImportCheckData(row, index);
                        string dealerCode = row["Dealer_Code"].ToString();
                        string definition = row["Definition"].ToString();
                        if (dicDealerCode.ContainsKey(dealerCode + "_" + definition))
                        {
                            throw new Exception(string.Format(@"Dealer Code ""{0}"" Definition ""{1}"" <=Duplicate>", dealerCode, definition));
                        }
                        dicDealerCode.Add(dealerCode + "_" + definition, dealerCode + "_" + definition);

                        string pickupDay = row["Pickup_Day"].ToString();
                        if (dicDealerCodePickupDay.ContainsKey(dealerCode + "_" + pickupDay))
                        {
                            throw new Exception(string.Format(@"Dealer Code ""{0}"",Pickup Day ""{1}"" <=Duplicate>", dealerCode, pickupDay));
                        }
                        dicDealerCodePickupDay.Add(dealerCode + "_" + pickupDay, dealerCode + "_" + pickupDay);

                        CBasNormalScheduleS item = new CBasNormalScheduleS();
                        Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleS>(item);
                        item.BasNormalScheduleId = basNormalScheduleId;
                        item.BasNormalScheduleSId = Guid.NewGuid().ToString();

                        item.DealerCode = row["Dealer_Code"].ToString();
                        item.FacingPdc = row["Facing_PDC"].ToString();
                        item.Definition = row["Definition"].ToString();
                        item.DealerType = row["Dealer_Type"].ToString();
                        item.ShortDealerName = row["Short_Dealer_Name"].ToString();
                        item.Destination = row["Destination"].ToString();
                        item.Province = row["Province"].ToString();
                        item.CutoffDay = Convert.ToInt32(row["Cutoff_Day"].ToString());
                        item.CutoffTime = Convert.ToDateTime(row["Cutoff_Time"].ToString());
                        item.PickupDay = Convert.ToInt32(row["Pickup_Day"].ToString());
                        item.PickupTime = Convert.ToDateTime(row["Pickup_Time"].ToString());
                        item.TmsRouteName = row["TMS_Route_Name"].ToString();
                        item.StopOrder = Convert.ToInt32(row["Stop_Order"].ToString());
                        item.TransMode = row["Trans_Mode"].ToString();
                        item.Overweek = Convert.ToInt32(row["Overweek"].ToString());
                        item.ArrivalDay = Convert.ToInt32(row["Arrival_Day"].ToString());
                        item.Eta = Convert.ToDateTime(row["ETA"].ToString());
                        item.ArrivalTime = row["Arrival_Time"].ToString();
                        item.ArrivalTimeStart = Convert.ToDateTime(row["Arrival_Time_Start"].ToString());
                        item.ArrivalTimeEnd = Convert.ToDateTime(row["Arrival_Time_End"].ToString());
                        item.Forwarder = row["Forwarder"].ToString();
                        DataContext.CBasNormalScheduleS.Add(item);

                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NormalScheduleSImportCheckData
        /// <summary>
        /// Normal Schedule S 导入数据校验
        /// </summary>
        /// <param name="row"></param>
        /// <param name="index"></param>
        private void NormalScheduleSImportCheckData(DataRow row, int index)
        {
            try
            {
                DataValidator(row, "Dealer_Code", index);
                DataValidator(row, "Definition", index);

                DataValidator<int>(row, "Cutoff_Day", index);
                DataValidator<int>(row, "Pickup_Day", index);
                DataValidator<int>(row, "Stop_Order", index);
                DataValidator<int>(row, "Overweek", index);
                DataValidator<int>(row, "Arrival_Day", index);

                DataValidator<DateTime>(row, "Pickup_Time", index);
                DataValidator<DateTime>(row, "Cutoff_Time", index);
                DataValidator<DateTime>(row, "ETA", index);
                DataValidator<DateTime>(row, "Arrival_Time_Start", index);
                DataValidator<DateTime>(row, "Arrival_Time_End", index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NormalScheduleVCtImport
        /// <summary>
        /// Normal Schedule V Cargo Type Import
        /// </summary>
        /// <param name="basNormalScheduleId"></param>
        /// <param name="strPath"></param>
        private void NormalScheduleVCtImport(string basNormalScheduleId, string strPath)
        {
            try
            {
                NpoiHelper npoiHelper = new NpoiHelper(strPath, false);
                DataTable dt = npoiHelper.ExcelToDataTable(0);
                //替换为从表里获取
                var warehouse = DataContext.CBasPdcSequence.Select(p => p.Pdc);
                //string[] warehouse = new string[] { "BJ", "SH", "GZ", "YZ", "CD" };

                Dictionary<string, string> dicDealerCode = new Dictionary<string, string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    int index = 2;
                    foreach (DataRow row in dt.Rows)
                    {
                        //验证指定字段不能为空
                        NormalScheduleVCtImportCheckData(row, index);
                        string dealerCode = row["Dealer_Code"].ToString();
                        if (dicDealerCode.ContainsKey(dealerCode))
                        {
                            throw new Exception(string.Format(@"Dealer Code ""{0}"" <=Duplicate>", dealerCode));
                        }
                        dicDealerCode.Add(dealerCode, dealerCode);
                        CBasNormalScheduleVCt item = new CBasNormalScheduleVCt();
                        Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVCt>(item);
                        item.BasNormalScheduleId = basNormalScheduleId;
                        item.BasNormalScheduleVCtId = Guid.NewGuid().ToString();

                        item.DealerCode = dealerCode;
                        item.FacingPdc = row["Facing_PDC"].ToString();
                        item.DealerType = row["Dealer_Type"].ToString();
                        item.DealerName = row["Dealer_Name"].ToString();
                        item.Destination = row["Destination"].ToString();
                        item.Province = row["Province"].ToString();

                        if (!DataIsNullValidator(row, "1st_V_Cutoff_Day") && !DataIsNullValidator(row, "1st_V_Cutoff_Time")
                            && !DataIsNullValidator(row, "1st_V_Pickup_Day") && !DataIsNullValidator(row, "1st_V_Pickup_Time")
                            && !DataIsNullValidator(row, "1st_V_Trans_Mode") && !DataIsNullValidator(row, "1st_V_Leadtime")
                            && !DataIsNullValidator(row, "1st_V_Leadtime_AM_PM") && !DataIsNullValidator(row, "1st_V_Leadtime_Start")
                            && !DataIsNullValidator(row, "1st_V_Leadtime_End"))
                        {
                            DataValidator<DateTime>(row, "1st_V_Cutoff_Time", index);
                            DataValidator<DateTime>(row, "1st_V_Pickup_Time", index);
                            DataValidator<int>(row, "1st_V_Leadtime", index);
                            DataValidator<DateTime>(row, "1st_V_Leadtime_Start", index);
                            DataValidator<DateTime>(row, "1st_V_Leadtime_End", index);

                            item.FirstVCutoffDay = row["1st_V_Cutoff_Day"].ToString();
                            item.FirstVCutoffTime = Convert.ToDateTime(row["1st_V_Cutoff_Time"].ToString());
                            item.FirstVPickupDay = row["1st_V_Pickup_Day"].ToString();
                            item.FirstVPickupTime = Convert.ToDateTime(row["1st_V_Pickup_Time"].ToString());
                            item.FirstVTransMode = row["1st_V_Trans_Mode"].ToString();
                            item.FirstVLeadtime = Convert.ToInt32(row["1st_V_Leadtime"].ToString());
                            item.FirstVLeadtimeAmPm = row["1st_V_Leadtime_AM_PM"].ToString();
                            item.FirstVLeadtimeStart = Convert.ToDateTime(row["1st_V_Leadtime_Start"].ToString());
                            item.FirstVLeadtimeEnd = Convert.ToDateTime(row["1st_V_Leadtime_End"].ToString());
                        }
                        item.SecondVCutoffDay = row["2nd_V_Cutoff_Day"].ToString();
                        item.SecondVCutoffTime = Convert.ToDateTime(row["2nd_V_Cutoff_Time"].ToString());
                        item.SecondVPickupDay = row["2nd_V_Pickup_Day"].ToString();
                        item.SecondVPickupTime = Convert.ToDateTime(row["2nd_V_Pickup_Time"].ToString());
                        DataContext.CBasNormalScheduleVCt.Add(item);
                        foreach (var strPdc in warehouse)
                        {
                            if (!dt.Columns.Contains(strPdc + "_2nd_V_Normal_Trans_Mode")) continue;

                            DataValidator<int>(row, strPdc + "_2nd_V_Normal_Leadtime", index);
                            DataValidator<DateTime>(row, strPdc + "_2nd_V_Normal_Leadtime_Start", index);
                            DataValidator<DateTime>(row, strPdc + "_2nd_V_Normal_Leadtime_End", index);

                            DataValidator<int>(row, strPdc + "_2nd_V_DG_Bulky_Leadtime", index);
                            DataValidator<DateTime>(row, strPdc + "_2nd_V_DG_Bulky_Leadtime_Start", index);
                            DataValidator<DateTime>(row, strPdc + "_2nd_V_DG_Bulky_Leadtime_End", index);

                            CBasNormalScheduleVCtDet itemNormal = new CBasNormalScheduleVCtDet();
                            Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVCtDet>(itemNormal);
                            itemNormal.BasNormalScheduleId = basNormalScheduleId;
                            itemNormal.BasNormalScheduleVCtId = item.BasNormalScheduleVCtId;
                            itemNormal.BasNormalScheduleVCtDetId = Guid.NewGuid().ToString();
                            itemNormal.Pdc = strPdc;
                            itemNormal.CargoType = "Normal";
                            itemNormal.SecondVTransMode = row[strPdc + "_2nd_V_Normal_Trans_Mode"].ToString();
                            itemNormal.SecondVLeadtime = Convert.ToInt32(row[strPdc + "_2nd_V_Normal_Leadtime"].ToString());
                            itemNormal.SecondVLeadtimeAmPm = row[strPdc + "_2nd_V_Normal_Leadtime_AM_PM"].ToString();
                            itemNormal.SecondVLeadtimeStart = Convert.ToDateTime(row[strPdc + "_2nd_V_Normal_Leadtime_Start"].ToString());
                            itemNormal.SecondVLeadtimeEnd = Convert.ToDateTime(row[strPdc + "_2nd_V_Normal_Leadtime_End"].ToString());
                            DataContext.CBasNormalScheduleVCtDet.Add(itemNormal);

                            CBasNormalScheduleVCtDet itemDgBulky = new CBasNormalScheduleVCtDet();
                            Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVCtDet>(itemDgBulky);
                            itemDgBulky.BasNormalScheduleId = basNormalScheduleId;
                            itemDgBulky.BasNormalScheduleVCtId = item.BasNormalScheduleVCtId;
                            itemDgBulky.BasNormalScheduleVCtDetId = Guid.NewGuid().ToString();
                            itemDgBulky.Pdc = strPdc;
                            itemDgBulky.CargoType = "DG/Bulky";
                            itemDgBulky.SecondVTransMode = row[strPdc + "_2nd_V_DG_Bulky_Trans_Mode"].ToString();
                            itemDgBulky.SecondVLeadtime = Convert.ToInt32(row[strPdc + "_2nd_V_DG_Bulky_Leadtime"].ToString());
                            itemDgBulky.SecondVLeadtimeAmPm = row[strPdc + "_2nd_V_DG_Bulky_Leadtime_AM_PM"].ToString();
                            itemDgBulky.SecondVLeadtimeStart = Convert.ToDateTime(row[strPdc + "_2nd_V_DG_Bulky_Leadtime_Start"].ToString());
                            itemDgBulky.SecondVLeadtimeEnd = Convert.ToDateTime(row[strPdc + "_2nd_V_DG_Bulky_Leadtime_End"].ToString());
                            DataContext.CBasNormalScheduleVCtDet.Add(itemDgBulky);
                        }
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NormalScheduleVCtImportCheckData
        /// <summary>
        /// Normal Schedule V Cargo Type 导入数据校验
        /// </summary>
        /// <param name="row"></param>
        /// <param name="index"></param>
        private void NormalScheduleVCtImportCheckData(DataRow row, int index)
        {
            try
            {
                DataValidator(row, "Dealer_Code", index);

                DataValidator<DateTime>(row, "2nd_V_Pickup_Time", index);
                DataValidator<DateTime>(row, "2nd_V_Cutoff_Time", index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NormalScheduleVTmImport
        /// <summary>
        /// Normal Schedule V Trans Mode Import
        /// </summary>
        /// <param name="basNormalScheduleId"></param>
        /// <param name="strPath"></param>
        private void NormalScheduleVTmImport(string basNormalScheduleId, string strPath)
        {
            try
            {
                NpoiHelper npoiHelper = new NpoiHelper(strPath, false);
                DataTable dt = npoiHelper.ExcelToDataTable(0);
                //替换为从表里获取
                var pdcs = DataContext.CBasPdcSequence.Select(p => p.Pdc);
                //string[] pdcs = new string[] { "BJ", "SH", "GZ", "YZ", "CD" };
                string[] transModes = new string[] { "FTL_Line", "FTL_DTD", "LTL", "Air", "Courier" };

                Dictionary<string, string> dicDestination = new Dictionary<string, string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    int index = 2;
                    foreach (DataRow row in dt.Rows)
                    {
                        NormalScheduleVTmImportCheckData(row, index);
                        string destination = row["Destination"].ToString();
                        if (dicDestination.ContainsKey(destination))
                        {
                            throw new Exception(string.Format(@"Destination ""{0}"" <=Duplicate>", destination));
                        }
                        dicDestination.Add(destination, destination);
                        CBasNormalScheduleVTm item = new CBasNormalScheduleVTm();
                        Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVTm>(item);
                        item.BasNormalScheduleId = basNormalScheduleId;
                        item.BasNormalScheduleVTmId = Guid.NewGuid().ToString();

                        item.Destination = row["Destination"].ToString();
                        item.Province = row["Province"].ToString();

                        if (!DataIsNullValidator(row, "1st_V_Cutoff_Day") && !DataIsNullValidator(row, "1st_V_Cutoff_Time")
                             && !DataIsNullValidator(row, "1st_V_Pickup_Day") && !DataIsNullValidator(row, "1st_V_Pickup_Time")
                             && !DataIsNullValidator(row, "1st_V_Trans_Mode") && !DataIsNullValidator(row, "1st_V_Leadtime")
                             && !DataIsNullValidator(row, "1st_V_Leadtime_AM_PM") && !DataIsNullValidator(row, "1st_V_Leadtime_Start")
                             && !DataIsNullValidator(row, "1st_V_Leadtime_End"))
                        {
                            DataValidator<DateTime>(row, "1st_V_Cutoff_Time", index);
                            DataValidator<DateTime>(row, "1st_V_Pickup_Time", index);
                            DataValidator<int>(row, "1st_V_Leadtime", index);
                            DataValidator<DateTime>(row, "1st_V_Leadtime_Start", index);
                            DataValidator<DateTime>(row, "1st_V_Leadtime_End", index);

                            item.FirstVCutoffDay = row["1st_V_Cutoff_Day"].ToString();
                            item.FirstVCutoffTime = Convert.ToDateTime(row["1st_V_Cutoff_Time"].ToString());
                            item.FirstVPickupDay = row["1st_V_Pickup_Day"].ToString();
                            item.FirstVPickupTime = Convert.ToDateTime(row["1st_V_Pickup_Time"].ToString());
                            item.FirstVTransMode = row["1st_V_Trans_Mode"].ToString();
                            item.FirstVLeadtime = Convert.ToInt32(row["1st_V_Leadtime"].ToString());
                            item.FirstVLeadtimeAmPm = row["1st_V_Leadtime_AM_PM"].ToString();
                            item.FirstVLeadtimeStart = Convert.ToDateTime(row["1st_V_Leadtime_Start"].ToString());
                            item.FirstVLeadtimeEnd = Convert.ToDateTime(row["1st_V_Leadtime_End"].ToString());
                        }
                        item.SecondVCutoffDay = row["2nd_V_Cutoff_Day"].ToString();
                        item.SecondVCutoffTime = Convert.ToDateTime(row["2nd_V_Cutoff_Time"].ToString());
                        item.SecondVPickupDay = row["2nd_V_Pickup_Day"].ToString();
                        item.SecondVPickupTime = Convert.ToDateTime(row["2nd_V_Pickup_Time"].ToString());
                        DataContext.CBasNormalScheduleVTm.Add(item);
                        foreach (var strPdc in pdcs)
                        {
                            foreach (var strTransMode in transModes)
                            {
                                if (!dt.Columns.Contains(strPdc + "_2nd_V_" + strTransMode + "_Leadtime")) continue;

                                if (row[strPdc + "_2nd_V_" + strTransMode + "_Leadtime"] != null && !row[strPdc + "_2nd_V_" + strTransMode + "_Leadtime"].ToString().IsNullString())
                                {
                                    DataValidator<int>(row, strPdc + "_2nd_V_" + strTransMode + "_Leadtime", index);
                                    DataValidator<DateTime>(row, strPdc + "_2nd_V_" + strTransMode + "_Leadtime_Start", index);
                                    DataValidator<DateTime>(row, strPdc + "_2nd_V_" + strTransMode + "_Leadtime_End", index);
                                    CBasNormalScheduleVTmDet itemDet = new CBasNormalScheduleVTmDet();
                                    Dashboard.Authentication.Authentication.UpdateEntity<CBasNormalScheduleVTmDet>(itemDet);
                                    itemDet.BasNormalScheduleId = basNormalScheduleId;
                                    itemDet.BasNormalScheduleVTmId = item.BasNormalScheduleVTmId;
                                    itemDet.BasNormalScheduleVTmDetId = Guid.NewGuid().ToString();
                                    itemDet.Pdc = strPdc;
                                    itemDet.TransMode = strTransMode;
                                    itemDet.SecondVLeadtime = Convert.ToInt32(row[strPdc + "_2nd_V_" + strTransMode + "_Leadtime"].ToString());
                                    itemDet.SecondVLeadtimeAmPm = row[strPdc + "_2nd_V_" + strTransMode + "_Leadtime_AM_PM"].ToString();
                                    itemDet.SecondVLeadtimeStart = Convert.ToDateTime(row[strPdc + "_2nd_V_" + strTransMode + "_Leadtime_Start"].ToString());
                                    itemDet.SecondVLeadtimeEnd = Convert.ToDateTime(row[strPdc + "_2nd_V_" + strTransMode + "_Leadtime_End"].ToString());
                                    DataContext.CBasNormalScheduleVTmDet.Add(itemDet);
                                }
                            }
                        }
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NormalScheduleVTmImportCheckData
        /// <summary>
        /// Normal Schedule V Trans Mode 导入数据校验
        /// </summary>
        /// <param name="row"></param>
        /// <param name="index"></param>
        private void NormalScheduleVTmImportCheckData(DataRow row, int index)
        {
            try
            {
                DataValidator(row, "Destination", index);

                DataValidator<DateTime>(row, "2nd_V_Pickup_Time", index);
                DataValidator<DateTime>(row, "2nd_V_Cutoff_Time", index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DataValidator
        /// <summary>
        /// 数据验证:包括列名是否存在及非空
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="index"></param>
        private void DataValidator(DataRow row, string columnName, int index)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                throw new Exception("<=ExcelFormatError>" + columnName);
            }
            if (row[columnName] == null || row[columnName].ToString().IsNullString())
            {
                throw new Exception("<=Di>" + index.ToString() + "<=Row> " + columnName + "<=IsRequired>");
            }
        }

        /// <summary>
        /// 验证是否为空,为空则返回True，不存在列默认为空
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private bool DataIsNullValidator(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return true;
            }
            if (row[columnName] == null || row[columnName].ToString().IsNullString())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 数据验证:包括列名是否存在及非空、类型有效性
        /// </summary>
        /// <typeparam name="TResult">数据类型</typeparam>
        /// <param name="row">DataRow</param>
        /// <param name="columnName">列名</param>
        /// <param name="index">所在行</param>
        private void DataValidator<TResult>(DataRow row, string columnName, int index)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                throw new Exception("<=ExcelFormatError>" + columnName);
            }
            if (row[columnName] == null || row[columnName].ToString().IsNullString())
            {
                throw new Exception("<=Di>" + index.ToString() + "<=Row> " + columnName + "<=IsRequired>");
            }
            var type = typeof(TResult);

            var method = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
            var parameters = new object[] { row[columnName].ToString(), default(TResult) };
            // 若转换失败，执行failed
            if (!(bool)method.Invoke(null, parameters))
            {
                throw new Exception("<=Di>" + index.ToString() + "<=Row> " + columnName + "<=FormatError>");
            }
        }
        #endregion
    }
}
