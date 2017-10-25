//*******************************************
// ** Description:  Data Access Object for BasNormalScheduleVTmDet
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

    [Table("BAS_NORMAL_SCHEDULE_V_TM_DET")]
    /// <summary>
    /// Data Access Object for BasNormalScheduleVTmDet
    /// </summary>
    public class CBasNormalScheduleVTmDet
    {
        #region Property of BasNormalScheduleVTmDetId
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        [Column("BAS_NORMAL_SCHEDULE_V_TM_DET_ID", TypeName = "VARCHAR")]
        public string BasNormalScheduleVTmDetId { get; set; }
        #endregion

        #region Property of BasNormalScheduleVTmId
        /// <summary>
        /// Normal Schedule V by Cargo Type ID
        /// </summary>
        [Column("BAS_NORMAL_SCHEDULE_V_TM_ID", TypeName = "VARCHAR")]
        [Required(ErrorMessage = "<=NormalScheduleVTmIsRequired>")]
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

        #region Property of Pdc
        /// <summary>
        /// �ֿ����
        ///Format:
        ///BJ
        ///SH
        ///GZ
        ///YZ
        ///CD
        ///WJ
        ///BW
        ///KS
        /// </summary>
        [Column("PDC", TypeName = "VARCHAR")]
        public string Pdc { get; set; }

        #endregion

        #region Property of TransMode
        /// <summary>
        /// ����ģʽ
        ///format:
        ///FTL LINE
        ///FTL DTD
        ///AIR
        ///LTL
        /// </summary>
        [Column("TRANS_MODE", TypeName = "VARCHAR")]
        public string TransMode { get; set; }

        #endregion

        #region Property of SecondVLeadtime
        /// <summary>
        /// �ڶ���V ��ʱЧ
        ///format:
        ///0:����
        ///1:�ڶ���
        ///..
        /// </summary>
        [Column("SECOND_V_LEADTIME", TypeName = "INT")]
        public int SecondVLeadtime { get; set; }

        #endregion

        #region Property of SecondVLeadtimeAmPm
        /// <summary>
        /// �ڶ���V ������
        ///format:
        ///am/pm
        /// </summary>
        [Column("SECOND_V_LEADTIME_AM_PM", TypeName = "VARCHAR")]
        public string SecondVLeadtimeAmPm { get; set; }

        #endregion

        #region Property of SecondVLeadtimeStart
        /// <summary>
        /// �����綨�忪ʼʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("SECOND_V_LEADTIME_START", TypeName = "DATETIME")]
        public DateTime SecondVLeadtimeStart { get; set; }

        #endregion

        #region Property of SecondVLeadtimeEnd
        /// <summary>
        /// �����綨�����ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("SECOND_V_LEADTIME_END", TypeName = "DATETIME")]
        public DateTime SecondVLeadtimeEnd { get; set; }

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
