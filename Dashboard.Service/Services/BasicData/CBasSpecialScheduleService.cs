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
using System.Data;
using Dashboard.Common.Util;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dashboard.Service.Services.BasicData
{
    public partial class CBasSpecialScheduleService : DBBase, IBasSpecialSchedule<CBasSpecialSchedule>
    {
        #region IBaseService<CBasSpecialSchedule> 成员
        #region Get
        public CBasSpecialSchedule Get(int id)
        {
            return null;
        }

        public CBasSpecialSchedule Get(string id)
        {
            return DataContext.CBasSpecialSchedule.FirstOrDefault(m => m.BasSpecialScheduleId == id);
        }
        #endregion

        #region GetList
        public IQueryable<CBasSpecialSchedule> GetList()
        {
            return DataContext.CBasSpecialSchedule.OrderByDescending(r => r.CreatedDate);
        }

        public IQueryable<CBasSpecialSchedule> GetList(GridParam gp, Dictionary<string, string> condition, out int pageCount, out int totalCount)
        {
            IQueryable<CBasSpecialSchedule> originalSource = DataContext.CBasSpecialSchedule;
            string BasSpecialScheduleIndexId = condition["BasSpecialScheduleIndexId"];

            if (!string.IsNullOrEmpty(BasSpecialScheduleIndexId))
                originalSource = originalSource.Where(o => o.BasSpecialScheduleIndexId == BasSpecialScheduleIndexId);

            string dealerCode = condition["DealerCode"];
            if (!string.IsNullOrEmpty(dealerCode))
                originalSource = originalSource.Where(o => o.DealerCode.Contains(dealerCode));

            if (condition.ContainsKey("PickupDayFrom"))
            {
                string startdate = condition["PickupDayFrom"];
                if (!string.IsNullOrEmpty(startdate))
                {
                    DateTime startDate = Convert.ToDateTime(startdate);
                    if (startDate != null)
                        originalSource = originalSource.Where(o => o.PickupDay >= startDate);
                }
            }
            if (condition.ContainsKey("PickupDayTo"))
            {
                string endingdate = condition["PickupDayTo"];
                if (!string.IsNullOrEmpty(endingdate))
                {
                    DateTime endingDate = Convert.ToDateTime(endingdate);
                    if (endingDate != null)
                        originalSource = originalSource.Where(o => o.PickupDay <= endingDate);
                }
            }

            pageCount = 0;
            originalSource = originalSource.OrderBy(gp.sidx, gp.sord);
            totalCount = originalSource.Count();
            var dataSouce = Paging<CBasSpecialSchedule>(originalSource, gp.rows, gp.page, out pageCount);
            return dataSouce;
        }
        #endregion

        #region Save
        public bool Save(CBasSpecialSchedule BasSpecialSchedule, out string errMsg)
        {
            errMsg = "";
            try
            {
                if (BasSpecialSchedule.ArrivalTimeStart > BasSpecialSchedule.ArrivalTimeEnd)
                {
                    errMsg = "<=StartTimeGreaterThanEndTime>";
                    return false;
                }
                var basSpecialScheduleIndex = DataContext.CBasSpecialScheduleIndex.FirstOrDefault(p=>p.BasSpecialScheduleIndexId == BasSpecialSchedule.BasSpecialScheduleIndexId);
                if (basSpecialScheduleIndex != null)
                {
                    if (BasSpecialSchedule.PickupDay < basSpecialScheduleIndex.EffectiveDate || BasSpecialSchedule.PickupDay > basSpecialScheduleIndex.ExpirationDate)
                    {
                        errMsg = "<=PickupDayError>";
                        return false;
                    }
                }
                Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialSchedule>(BasSpecialSchedule);
                if (string.IsNullOrEmpty(BasSpecialSchedule.BasSpecialScheduleId))
                {
                    if (DataContext.CBasSpecialSchedule.Any(c => c.DealerCode == BasSpecialSchedule.DealerCode && c.PickupDay == BasSpecialSchedule.PickupDay && c.BasSpecialScheduleIndexId == BasSpecialSchedule.BasSpecialScheduleIndexId))
                    {
                        errMsg = "<=DuplicatedSpecialSchedule>";
                        return false;
                    }
                    BasSpecialSchedule.BasSpecialScheduleId = Guid.NewGuid().ToString();
                    DataContext.CBasSpecialSchedule.Add(BasSpecialSchedule);
                }
                else
                {
                    if (DataContext.CBasSpecialSchedule.Any(c => c.DealerCode == BasSpecialSchedule.DealerCode && c.PickupDay == BasSpecialSchedule.PickupDay && c.BasSpecialScheduleId != BasSpecialSchedule.BasSpecialScheduleId && c.BasSpecialScheduleIndexId == BasSpecialSchedule.BasSpecialScheduleIndexId))
                    {
                        errMsg = "<=DuplicatedSpecialSchedule>";
                        return false;
                    }
                }
                if (!DataContext.CBasSpecialDay.Any(p => p.SpecialDay == BasSpecialSchedule.PickupDay))
                {
                    CBasSpecialDay BasSpecialDay = new CBasSpecialDay();
                    Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialDay>(BasSpecialDay);
                    BasSpecialDay.SpecialDay = BasSpecialSchedule.PickupDay;
                    BasSpecialDay.BasSpecialScheduleIndexId = BasSpecialSchedule.BasSpecialScheduleIndexId;
                    BasSpecialDay.BasSpecialDayId = Guid.NewGuid().ToString();
                    DataContext.CBasSpecialDay.Add(BasSpecialDay);
                }
                DataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errMsg = "<=SaveError>";
                Log4netHelper.LoadADONetAppender();
                ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                log.Info("BasSpecialSchedule Save方法出错。Author:chenjm ", ex);
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
                var BasSpecialSchedule = DataContext.CBasSpecialSchedule.FirstOrDefault(f => f.BasSpecialScheduleId == id);
                if (BasSpecialSchedule != null)
                {
                    DataContext.CBasSpecialSchedule.Remove(BasSpecialSchedule);
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

        #region IBasSpecialSchedule<CBasSpecialSchedule> 成员
        #region SpecialScheduleImport
        public void SpecialScheduleImport(string BasSpecialScheduleIndexId, string strPath, out string errMsg)
        {
            errMsg = "<=ImportSuccess>";
            try
            {
                DataContext.Configuration.AutoDetectChangesEnabled = false;
                DataContext.Configuration.ValidateOnSaveEnabled = false;

                SpecialScheduleImport(BasSpecialScheduleIndexId, strPath);
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

        #region SpecialScheduleImport
        /// <summary>
        /// Special Schedule Import
        /// </summary>
        /// <param name="strPath"></param>
        private void SpecialScheduleImport(string BasSpecialScheduleIndexId, string strPath)
        {
            try
            {
                //清除
                var basSpecialScheduleIndex = DataContext.CBasSpecialScheduleIndex.FirstOrDefault(p => p.BasSpecialScheduleIndexId == BasSpecialScheduleIndexId);

                //保存上传的文件路径
                basSpecialScheduleIndex.SpecilScheduleFile = strPath;
                DataContext.Entry(basSpecialScheduleIndex).State = System.Data.EntityState.Modified;

                var args = new DbParameter[] { new SqlParameter("@id", BasSpecialScheduleIndexId) };
                DataContext.Database.ExecuteSqlCommand("DELETE BAS_SPECIAL_DAY WHERE BAS_SPECIAL_SCHEDULE_INDEX_ID=@id", args);
                var argst = new DbParameter[] { new SqlParameter("@id", BasSpecialScheduleIndexId) };
                DataContext.Database.ExecuteSqlCommand("DELETE BAS_SPECIAL_SCHEDULE WHERE BAS_SPECIAL_SCHEDULE_INDEX_ID= @id", argst);

                //自动抓取的特殊日列表
                List<DateTime> ArrBasSpecialDay = new List<DateTime>();

                NpoiHelper npoiHelper = new NpoiHelper(strPath, false);
                DataTable dt = npoiHelper.ExcelToDataTable(0);
                Dictionary<string, string> dicDealerCode = new Dictionary<string, string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    int index = 2;
                    foreach (DataRow row in dt.Rows)
                    {
                        DataValidator(row, "Dealer_Code", index);
                        string dealerCode = row["Dealer_Code"].ToString();
                        string pickupTime = "";
                        #region 1st Stk
                        if (row["1st_S_Pickup_Date"] != null && !row["1st_S_Pickup_Date"].ToString().IsNullString())
                        {
                            DataValidator<DateTime>(row, "1st_S_Cutoff_Day", index);
                            DataValidator<DateTime>(row, "1st_S_Cutoff_Time", index);
                            DataValidator<DateTime>(row, "1st_S_Pickup_Date", index);
                            DataValidator<DateTime>(row, "1st_S_Pickup_Time", index);
                            DataValidator<DateTime>(row, "1st_S_Arrival_Date", index);
                            DataValidator(row, "1st_S_Arrival_Time", index);
                            DataValidator<DateTime>(row, "1st_S_Arrival_Time_Start", index);
                            DataValidator<DateTime>(row, "1st_S_Arrival_Time_End", index);

                            DateTime pickupDay = Convert.ToDateTime(row["1st_S_Pickup_Date"].ToString());
                            pickupTime = pickupDay.ToString("yyyy-MM-dd");
                            if (dicDealerCode.ContainsKey(dealerCode + "_" + pickupTime))
                            {
                                throw new Exception(string.Format(@"Dealer Code ""{0}"" 1st_S_Pickup_Date ""{1}"", <=Duplicate>", dealerCode, pickupDay.ToString("yyyy-MM-dd")));
                            }
                            dicDealerCode.Add(dealerCode + "_" + pickupTime, dealerCode + "_" + pickupTime);
                            var oldFirstSpecialSchedule = DataContext.CBasSpecialSchedule.Where(p => p.DealerCode == dealerCode && p.PickupDay == pickupDay).FirstOrDefault();
                            if (oldFirstSpecialSchedule == null)
                            {
                                oldFirstSpecialSchedule = new CBasSpecialSchedule();
                                Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialSchedule>(oldFirstSpecialSchedule);
                                oldFirstSpecialSchedule.BasSpecialScheduleId = Guid.NewGuid().ToString();
                                DataContext.CBasSpecialSchedule.Add(oldFirstSpecialSchedule);
                            }
                            else
                            {
                                Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialSchedule>(oldFirstSpecialSchedule);
                                DataContext.CBasSpecialSchedule.Attach(oldFirstSpecialSchedule);
                                DataContext.Entry(oldFirstSpecialSchedule).State = EntityState.Modified;
                            }
                            //标记特殊时效的归属关系
                            oldFirstSpecialSchedule.BasSpecialScheduleIndexId = BasSpecialScheduleIndexId;
                            oldFirstSpecialSchedule.DealerCode = row["Dealer_Code"].ToString();
                            oldFirstSpecialSchedule.Definition = "1st Stk";
                            oldFirstSpecialSchedule.FacingPdc = row["Facing_PDC"].ToString();
                            oldFirstSpecialSchedule.DealerType = row["Dealer_Type"].ToString();
                            oldFirstSpecialSchedule.DealerName = row["Dealer_Name"].ToString();
                            oldFirstSpecialSchedule.Destination = row["Destination"].ToString();
                            oldFirstSpecialSchedule.Province = row["Province"].ToString();
                            oldFirstSpecialSchedule.CutoffDay = Convert.ToDateTime(row["1st_S_Cutoff_Day"].ToString());
                            oldFirstSpecialSchedule.CutoffTime = Convert.ToDateTime(row["1st_S_Cutoff_Time"].ToString());
                            oldFirstSpecialSchedule.PickupDay = Convert.ToDateTime(row["1st_S_Pickup_Date"].ToString());
                            oldFirstSpecialSchedule.PickupTime = Convert.ToDateTime(row["1st_S_Pickup_Time"].ToString());
                            oldFirstSpecialSchedule.TransMode = row["1st_S_Trans_Mode"].ToString();
                            oldFirstSpecialSchedule.ArrivalDay = Convert.ToDateTime(row["1st_S_Arrival_Date"].ToString());
                            oldFirstSpecialSchedule.ArrivalTime = row["1st_S_Arrival_Time"].ToString();
                            oldFirstSpecialSchedule.ArrivalTimeStart = Convert.ToDateTime(row["1st_S_Arrival_Time_Start"].ToString());
                            oldFirstSpecialSchedule.ArrivalTimeEnd = Convert.ToDateTime(row["1st_S_Arrival_Time_End"].ToString());
                            if (oldFirstSpecialSchedule.PickupDay < basSpecialScheduleIndex.EffectiveDate || oldFirstSpecialSchedule.PickupDay > basSpecialScheduleIndex.ExpirationDate)
                            {
                                throw new Exception(string.Format(@"Dealer Code ""{0}"", ""{1}"" <=PickupDayError>, <=Duplicate>", dealerCode, pickupDay.ToString("yyyy-MM-dd")));
                            }

                            #region 自动抓取特殊日
                            ArrBasSpecialDay.Add(oldFirstSpecialSchedule.PickupDay);
                            #endregion
                        }
                        #endregion
                        #region 2nd Stk
                        if (row["2nd_S_Pickup_Date"] != null && !row["2nd_S_Pickup_Date"].ToString().IsNullString())
                        {
                            DataValidator<DateTime>(row, "2nd_S_Cutoff_Day", index);
                            DataValidator<DateTime>(row, "2nd_S_Cutoff_Time", index);
                            DataValidator<DateTime>(row, "2nd_S_Pickup_Date", index);
                            DataValidator<DateTime>(row, "2nd_S_Pickup_Time", index);
                            DataValidator<DateTime>(row, "2nd_S_Arrival_Date", index);
                            DataValidator(row, "2nd_S_Arrival_Time", index);
                            DataValidator<DateTime>(row, "2nd_S_Arrival_Time_Start", index);
                            DataValidator<DateTime>(row, "2nd_S_Arrival_Time_End", index);

                            DateTime pickupDay = Convert.ToDateTime(row["2nd_S_Pickup_Date"].ToString());
                            pickupTime = pickupDay.ToString("yyyy-MM-dd");
                            if (dicDealerCode.ContainsKey(dealerCode + "_" + pickupTime))
                            {
                                throw new Exception(string.Format(@"Dealer Code ""{0}"" 2nd_S_Pickup_Date ""{1}"", <=Duplicate>", dealerCode, pickupDay.ToString("yyyy-MM-dd")));
                            }
                            dicDealerCode.Add(dealerCode + "_" + pickupTime, dealerCode + "_" + pickupTime);
                            var oldFirstSpecialSchedule = DataContext.CBasSpecialSchedule.Where(p => p.DealerCode == dealerCode && p.PickupDay == pickupDay).FirstOrDefault();
                            if (oldFirstSpecialSchedule == null)
                            {
                                oldFirstSpecialSchedule = new CBasSpecialSchedule();
                                Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialSchedule>(oldFirstSpecialSchedule);
                                oldFirstSpecialSchedule.BasSpecialScheduleId = Guid.NewGuid().ToString();
                                DataContext.CBasSpecialSchedule.Add(oldFirstSpecialSchedule);
                            }
                            else
                            {
                                Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialSchedule>(oldFirstSpecialSchedule);
                                DataContext.CBasSpecialSchedule.Attach(oldFirstSpecialSchedule);
                                DataContext.Entry(oldFirstSpecialSchedule).State = EntityState.Modified;
                            }
                            oldFirstSpecialSchedule.BasSpecialScheduleIndexId = BasSpecialScheduleIndexId;
                            oldFirstSpecialSchedule.DealerCode = row["Dealer_Code"].ToString();
                            oldFirstSpecialSchedule.Definition = "2nd Stk";
                            oldFirstSpecialSchedule.FacingPdc = row["Facing_PDC"].ToString();
                            oldFirstSpecialSchedule.DealerType = row["Dealer_Type"].ToString();
                            oldFirstSpecialSchedule.DealerName = row["Dealer_Name"].ToString();
                            oldFirstSpecialSchedule.Destination = row["Destination"].ToString();
                            oldFirstSpecialSchedule.Province = row["Province"].ToString();
                            oldFirstSpecialSchedule.CutoffDay = Convert.ToDateTime(row["2nd_S_Cutoff_Day"].ToString());
                            oldFirstSpecialSchedule.CutoffTime = Convert.ToDateTime(row["2nd_S_Cutoff_Time"].ToString());
                            oldFirstSpecialSchedule.PickupDay = Convert.ToDateTime(row["2nd_S_Pickup_Date"].ToString());
                            oldFirstSpecialSchedule.PickupTime = Convert.ToDateTime(row["2nd_S_Pickup_Time"].ToString());
                            oldFirstSpecialSchedule.TransMode = row["2nd_S_Trans_Mode"].ToString();
                            oldFirstSpecialSchedule.ArrivalDay = Convert.ToDateTime(row["2nd_S_Arrival_Date"].ToString());
                            oldFirstSpecialSchedule.ArrivalTime = row["2nd_S_Arrival_Time"].ToString();
                            oldFirstSpecialSchedule.ArrivalTimeStart = Convert.ToDateTime(row["2nd_S_Arrival_Time_Start"].ToString());
                            oldFirstSpecialSchedule.ArrivalTimeEnd = Convert.ToDateTime(row["2nd_S_Arrival_Time_End"].ToString());
                            if (oldFirstSpecialSchedule.PickupDay < basSpecialScheduleIndex.EffectiveDate || oldFirstSpecialSchedule.PickupDay > basSpecialScheduleIndex.ExpirationDate)
                            {
                                throw new Exception(string.Format(@"Dealer Code ""{0}"", ""{1}"" <=PickupDayError>, <=Duplicate>", dealerCode, pickupDay.ToString("yyyy-MM-dd")));
                            }
                            #region 自动抓取特殊日
                            ArrBasSpecialDay.Add(oldFirstSpecialSchedule.PickupDay);
                            #endregion
                        }
                        #endregion
                        index++;
                    }
                    //添加自动抓取的特殊日到数据库
                    var ReArrBasSpecialDay=ArrBasSpecialDay.Distinct();
                    foreach (var item in ReArrBasSpecialDay)
                    {
                        CBasSpecialDay BasSpecialDay = new CBasSpecialDay();
                        Dashboard.Authentication.Authentication.UpdateEntity<CBasSpecialDay>(BasSpecialDay);
                        BasSpecialDay.SpecialDay = item;
                        BasSpecialDay.BasSpecialScheduleIndexId = BasSpecialScheduleIndexId;
                        BasSpecialDay.BasSpecialDayId = Guid.NewGuid().ToString();
                        DataContext.CBasSpecialDay.Add(BasSpecialDay);
                    }
                    
                }
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
