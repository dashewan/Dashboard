//*******************************************
// ** Description:  Data Access Object for BasNormalSchedule
// ** Author     :  Code generator
// ** Created    :  2014-08-18 14:36:46
// ** Modified   :
//*******************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Domain.BasicData
{

    [Table("BAS_SPECIAL_SCHEDULE_INDEX")]
    /// <summary>
    /// Data Access Object for BasNormalSchedule
    /// </summary>
    public class CBasSpecialScheduleIndex
    {
        #region Property of BasSpecialScheduleIndexId
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("BAS_SPECIAL_SCHEDULE_INDEX_ID", TypeName = "VARCHAR")]
        public string BasSpecialScheduleIndexId { get; set; }
        #endregion

        #region Property of ScheduleNo
        /// <summary>
        /// Schedule 编号
        /// </summary>
        [Column("SCHEDULE_NO", TypeName = "VARCHAR")]
        public string ScheduleNo { get; set; }

        #endregion

        #region Property of EffectiveDate
        /// <summary>
        /// 生效日期
        /// </summary>
        [Column("EFFECTIVE_DATE", TypeName = "DATETIME")]
        public DateTime EffectiveDate { get; set; }

        #endregion

        #region Property of ExpirationDate
        /// <summary>
        /// 失效日期
        /// </summary>
        [Column("EXPIRATION_DATE", TypeName = "DATETIME")]
        public DateTime ExpirationDate { get; set; }

        #endregion

        #region Property of NormalScheduleSFileName
        /// <summary>
        /// Normal Schedule S 文件名
        /// </summary>
        [Column("NORMAL_SCHEDULE_S_FILE_NAME", TypeName = "VARCHAR")]
        public string NormalScheduleSFileName { get; set; }

        #endregion

        #region Property of NormalScheduleVCtFileName
        /// <summary>
        /// Normal Schedule V Cargo Type 文件名
        /// </summary>
        [Column("NORMAL_SCHEDULE_V_CT_FILE_NAME", TypeName = "VARCHAR")]
        public string NormalScheduleVCtFileName { get; set; }

        #endregion

        #region Property of NormalScheduleVTmFileName
        /// <summary>
        /// Normal Schedule V Trans. Mode 文件名
        /// </summary>
        [Column("NORMAL_SCHEDULE_V_TM_FILE_NAME", TypeName = "VARCHAR")]
        public string NormalScheduleVTmFileName { get; set; }

        #endregion

        #region Property of Remark
        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK", TypeName = "NVARCHAR")]
        public string Remark { get; set; }

        #endregion

        #region Property of SpecilScheduleFile
        /// <summary>
        /// SpecilScheduleFile的上传路径保存
        /// </summary>
        [Column("SPECIL_SCHEDULE_FILE_NAME", TypeName = "VARCHAR")]
        public string SpecilScheduleFile { get; set; }

        #endregion

        #region Property of CreatedUserId
        /// <summary>
        /// 创建人ID
        /// </summary>
        [Column("CREATED_USER_ID", TypeName = "VARCHAR")]
        public string CreatedUserId { get; set; }

        #endregion

        #region Property of CreatedUserCode
        /// <summary>
        /// 创建人编码
        /// </summary>
        [Column("CREATED_USER_CODE", TypeName = "VARCHAR")]
        public string CreatedUserCode { get; set; }

        #endregion

        #region Property of CreatedUserName
        /// <summary>
        /// 创建人名称
        /// </summary>
        [Column("CREATED_USER_NAME", TypeName = "NVARCHAR")]
        public string CreatedUserName { get; set; }

        #endregion

        #region Property of CreatedOfficeId
        /// <summary>
        /// 创建办事处ID
        /// </summary>
        [Column("CREATED_OFFICE_ID", TypeName = "VARCHAR")]
        public string CreatedOfficeId { get; set; }

        #endregion

        #region Property of CreatedOfficeCode
        /// <summary>
        /// 创建办事处编码
        /// </summary>
        [Column("CREATED_OFFICE_CODE", TypeName = "VARCHAR")]
        public string CreatedOfficeCode { get; set; }

        #endregion

        #region Property of CreatedOfficeName
        /// <summary>
        /// 创建办事处名称
        /// </summary>
        [Column("CREATED_OFFICE_NAME", TypeName = "NVARCHAR")]
        public string CreatedOfficeName { get; set; }

        #endregion

        #region Property of CreatedDate
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CREATED_DATE", TypeName = "DATETIME")]
        public DateTime CreatedDate { get; set; }

        #endregion

        #region Property of UpdatedUserId
        /// <summary>
        /// 更新人ID
        /// </summary>
        [Column("UPDATED_USER_ID", TypeName = "VARCHAR")]
        public string UpdatedUserId { get; set; }

        #endregion

        #region Property of UpdatedUserCode
        /// <summary>
        /// 更新人代码
        /// </summary>
        [Column("UPDATED_USER_CODE", TypeName = "VARCHAR")]
        public string UpdatedUserCode { get; set; }

        #endregion

        #region Property of UpdatedUserName
        /// <summary>
        /// 更新人名称
        /// </summary>
        [Column("UPDATED_USER_NAME", TypeName = "NVARCHAR")]
        public string UpdatedUserName { get; set; }

        #endregion

        #region Property of UpdatedOfficeId
        /// <summary>
        /// 更新办事处ID
        /// </summary>
        [Column("UPDATED_OFFICE_ID", TypeName = "VARCHAR")]
        public string UpdatedOfficeId { get; set; }

        #endregion

        #region Property of UpdatedOfficeCode
        /// <summary>
        /// 更新办事处代码
        /// </summary>
        [Column("UPDATED_OFFICE_CODE", TypeName = "VARCHAR")]
        public string UpdatedOfficeCode { get; set; }

        #endregion

        #region Property of UpdatedOfficeName
        /// <summary>
        /// 更新办事处名称
        /// </summary>
        [Column("UPDATED_OFFICE_NAME", TypeName = "NVARCHAR")]
        public string UpdatedOfficeName { get; set; }

        #endregion

        #region Property of UpdatedDate
        /// <summary>
        /// 更新日期
        /// </summary>
        [Column("UPDATED_DATE", TypeName = "DATETIME")]
        public DateTime? UpdatedDate { get; set; }

        #endregion

    }
}
