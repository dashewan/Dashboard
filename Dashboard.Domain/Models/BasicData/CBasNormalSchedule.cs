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

    [Table("BAS_NORMAL_SCHEDULE")]
    /// <summary>
    /// Data Access Object for BasNormalSchedule
    /// </summary>
    public class CBasNormalSchedule
    {
        #region Property of BasNormalScheduleId
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        [Column("BAS_NORMAL_SCHEDULE_ID", TypeName = "VARCHAR")]
        public string BasNormalScheduleId { get; set; }
        #endregion

        #region Property of ScheduleNo
        /// <summary>
        /// Schedule ���
        /// </summary>
        [Column("SCHEDULE_NO", TypeName = "VARCHAR")]
        public string ScheduleNo { get; set; }

        #endregion

        #region Property of EffectiveDate
        /// <summary>
        /// ��Ч����
        /// </summary>
        [Column("EFFECTIVE_DATE", TypeName = "DATETIME")]
        public DateTime EffectiveDate { get; set; }

        #endregion

        #region Property of ExpirationDate
        /// <summary>
        /// ʧЧ����
        /// </summary>
        [Column("EXPIRATION_DATE", TypeName = "DATETIME")]
        public DateTime ExpirationDate { get; set; }

        #endregion

        #region Property of NormalScheduleSFileName
        /// <summary>
        /// Normal Schedule S �ļ���
        /// </summary>
        [Column("NORMAL_SCHEDULE_S_FILE_NAME", TypeName = "VARCHAR")]
        public string NormalScheduleSFileName { get; set; }

        #endregion

        #region Property of NormalScheduleVCtFileName
        /// <summary>
        /// Normal Schedule V Cargo Type �ļ���
        /// </summary>
        [Column("NORMAL_SCHEDULE_V_CT_FILE_NAME", TypeName = "VARCHAR")]
        public string NormalScheduleVCtFileName { get; set; }

        #endregion

        #region Property of NormalScheduleVTmFileName
        /// <summary>
        /// Normal Schedule V Trans. Mode �ļ���
        /// </summary>
        [Column("NORMAL_SCHEDULE_V_TM_FILE_NAME", TypeName = "VARCHAR")]
        public string NormalScheduleVTmFileName { get; set; }

        #endregion

        #region Property of Remark
        /// <summary>
        /// ��ע
        /// </summary>
        [Column("REMARK", TypeName = "NVARCHAR")]
        public string Remark { get; set; }

        #endregion

        #region Property of CreatedUserId
        /// <summary>
        /// ������ID
        /// </summary>
        [Column("CREATED_USER_ID", TypeName = "VARCHAR")]
        public string CreatedUserId { get; set; }

        #endregion

        #region Property of CreatedUserCode
        /// <summary>
        /// �����˱���
        /// </summary>
        [Column("CREATED_USER_CODE", TypeName = "VARCHAR")]
        public string CreatedUserCode { get; set; }

        #endregion

        #region Property of CreatedUserName
        /// <summary>
        /// ����������
        /// </summary>
        [Column("CREATED_USER_NAME", TypeName = "NVARCHAR")]
        public string CreatedUserName { get; set; }

        #endregion

        #region Property of CreatedOfficeId
        /// <summary>
        /// �������´�ID
        /// </summary>
        [Column("CREATED_OFFICE_ID", TypeName = "VARCHAR")]
        public string CreatedOfficeId { get; set; }

        #endregion

        #region Property of CreatedOfficeCode
        /// <summary>
        /// �������´�����
        /// </summary>
        [Column("CREATED_OFFICE_CODE", TypeName = "VARCHAR")]
        public string CreatedOfficeCode { get; set; }

        #endregion

        #region Property of CreatedOfficeName
        /// <summary>
        /// �������´�����
        /// </summary>
        [Column("CREATED_OFFICE_NAME", TypeName = "NVARCHAR")]
        public string CreatedOfficeName { get; set; }

        #endregion

        #region Property of CreatedDate
        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Column("CREATED_DATE", TypeName = "DATETIME")]
        public DateTime CreatedDate { get; set; }

        #endregion

        #region Property of UpdatedUserId
        /// <summary>
        /// ������ID
        /// </summary>
        [Column("UPDATED_USER_ID", TypeName = "VARCHAR")]
        public string UpdatedUserId { get; set; }

        #endregion

        #region Property of UpdatedUserCode
        /// <summary>
        /// �����˴���
        /// </summary>
        [Column("UPDATED_USER_CODE", TypeName = "VARCHAR")]
        public string UpdatedUserCode { get; set; }

        #endregion

        #region Property of UpdatedUserName
        /// <summary>
        /// ����������
        /// </summary>
        [Column("UPDATED_USER_NAME", TypeName = "NVARCHAR")]
        public string UpdatedUserName { get; set; }

        #endregion

        #region Property of UpdatedOfficeId
        /// <summary>
        /// ���°��´�ID
        /// </summary>
        [Column("UPDATED_OFFICE_ID", TypeName = "VARCHAR")]
        public string UpdatedOfficeId { get; set; }

        #endregion

        #region Property of UpdatedOfficeCode
        /// <summary>
        /// ���°��´�����
        /// </summary>
        [Column("UPDATED_OFFICE_CODE", TypeName = "VARCHAR")]
        public string UpdatedOfficeCode { get; set; }

        #endregion

        #region Property of UpdatedOfficeName
        /// <summary>
        /// ���°��´�����
        /// </summary>
        [Column("UPDATED_OFFICE_NAME", TypeName = "NVARCHAR")]
        public string UpdatedOfficeName { get; set; }

        #endregion

        #region Property of UpdatedDate
        /// <summary>
        /// ��������
        /// </summary>
        [Column("UPDATED_DATE", TypeName = "DATETIME")]
        public DateTime? UpdatedDate { get; set; }

        #endregion

    }
}
