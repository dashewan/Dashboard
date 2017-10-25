//*******************************************
// ** Description:  Data Access Object for BasNormalScheduleVTm
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

    [Table("BAS_NORMAL_SCHEDULE_V_TM")]
    /// <summary>
    /// Data Access Object for BasNormalScheduleVTm
    /// </summary>
    public class CBasNormalScheduleVTm
    {
        #region Property of BasNormalScheduleVTmId
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        [Column("BAS_NORMAL_SCHEDULE_V_TM_ID", TypeName = "VARCHAR")]
        public string BasNormalScheduleVTmId { get; set; }
        #endregion

        #region Property of BasNormalScheduleId
        /// <summary>
        /// Schedule ����ID
        /// </summary>
        [Required(ErrorMessage = "<=NormalScheduleIsRequired>")]
        [Column("BAS_NORMAL_SCHEDULE_ID", TypeName = "VARCHAR")]
        public string BasNormalScheduleId { get; set; }

        #endregion

        #region Property of Destination
        /// <summary>
        /// Dealer���ڳ���
        ///Format:
        ///����ƴ��
        /// </summary>
        [Column("DESTINATION", TypeName = "VARCHAR")]
        public string Destination { get; set; }

        #endregion

        #region Property of Province
        /// <summary>
        /// Dealer����ʡ��
        ///Format:
        ///ʡ��ƴ��
        /// </summary>
        [Column("PROVINCE", TypeName = "VARCHAR")]
        public string Province { get; set; }

        #endregion

        #region Property of FirstVCutoffDay
        /// <summary>
        /// ��һ��V�ص�����
        ///Format:
        ///Working day
        /// </summary>
        [Column("FIRST_V_CUTOFF_DAY", TypeName = "VARCHAR")]
        public string FirstVCutoffDay { get; set; }

        #endregion

        #region Property of FirstVCutoffTime
        /// <summary>
        /// ��һ��V�ص�ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("FIRST_V_CUTOFF_TIME", TypeName = "DATETIME")]
        public DateTime? FirstVCutoffTime { get; set; }

        #endregion

        #region Property of FirstVPickupDay
        /// <summary>
        /// ��һ��V�������=��ӡ����
        ///Format:
        ///Working day
        /// </summary>
        [Column("FIRST_V_PICKUP_DAY", TypeName = "VARCHAR")]
        public string FirstVPickupDay { get; set; }

        #endregion

        #region Property of FirstVPickupTime
        /// <summary>
        /// ��һ��V���ʱ��=TMS��ӡʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("FIRST_V_PICKUP_TIME", TypeName = "DATETIME")]
        public DateTime? FirstVPickupTime { get; set; }

        #endregion

        #region Property of FirstVTransMode
        /// <summary>
        /// ��һ��V ���䷽ʽ
        ///Format:
        ///FTL DTD
        /// </summary>
        [Column("FIRST_V_TRANS_MODE", TypeName = "VARCHAR")]
        public string FirstVTransMode { get; set; }

        #endregion

        #region Property of FirstVLeadtime
        /// <summary>
        /// ��һ��VʱЧ
        ///format:
        ///0:����
        ///1:�ڶ���
        ///..
        /// </summary>
        [Column("FIRST_V_LEADTIME", TypeName = "INT")]
        public int? FirstVLeadtime { get; set; }

        #endregion

        #region Property of FirstVLeadtimeAmPm
        /// <summary>
        /// ��һ��VʱЧ������
        ///format:
        ///am/pm
        /// </summary>
        [Column("FIRST_V_LEADTIME_AM_PM", TypeName = "VARCHAR")]
        public string FirstVLeadtimeAmPm { get; set; }

        #endregion

        #region Property of FirstVLeadtimeStart
        /// <summary>
        /// �����綨�忪ʼʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("FIRST_V_LEADTIME_START", TypeName = "DATETIME")]
        public DateTime? FirstVLeadtimeStart { get; set; }

        #endregion

        #region Property of FirstVLeadtimeEnd
        /// <summary>
        /// �����綨�����ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("FIRST_V_LEADTIME_END", TypeName = "DATETIME")]
        public DateTime? FirstVLeadtimeEnd { get; set; }

        #endregion

        #region Property of SecondVCutoffDay
        /// <summary>
        /// �ڶ���V�ص�����
        ///Format:
        ///Working day
        /// </summary>
        [Column("SECOND_V_CUTOFF_DAY", TypeName = "VARCHAR")]
        public string SecondVCutoffDay { get; set; }

        #endregion

        #region Property of SecondVCutoffTime
        /// <summary>
        /// �ڶ���V�ص�ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("SECOND_V_CUTOFF_TIME", TypeName = "DATETIME")]
        public DateTime SecondVCutoffTime { get; set; }

        #endregion

        #region Property of SecondVPickupDay
        /// <summary>
        /// �ڶ���V�������=��ӡ����
        ///Format:
        ///Working day
        /// </summary>
        [Column("SECOND_V_PICKUP_DAY", TypeName = "VARCHAR")]
        public string SecondVPickupDay { get; set; }

        #endregion

        #region Property of SecondVPickupTime
        /// <summary>
        /// �ڶ���V���ʱ��=TMS��ӡʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("SECOND_V_PICKUP_TIME", TypeName = "DATETIME")]
        public DateTime SecondVPickupTime { get; set; }

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
