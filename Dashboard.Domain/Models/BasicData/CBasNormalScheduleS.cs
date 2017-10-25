//*******************************************
// ** Description:  Data Access Object for BasNormalScheduleS
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

    [Table("BAS_NORMAL_SCHEDULE_S")]
    /// <summary>
    /// Data Access Object for BasNormalScheduleS
    /// </summary>
    public class CBasNormalScheduleS
    {
        #region Property of BasNormalScheduleSId
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        [Column("BAS_NORMAL_SCHEDULE_S_ID", TypeName = "VARCHAR")]
        public string BasNormalScheduleSId { get; set; }
        #endregion

        #region Property of BasNormalScheduleId
        /// <summary>
        /// Schedule ����ID
        /// </summary>
        [Required(ErrorMessage = "<=NormalScheduleIsRequired>")]
        [Column("BAS_NORMAL_SCHEDULE_ID", TypeName = "VARCHAR")]
        public string BasNormalScheduleId { get; set; }

        #endregion

        #region Property of DealerCode
        /// <summary>
        /// �����̴���
        /// </summary>
        [Column("DEALER_CODE", TypeName = "VARCHAR")]
        public string DealerCode { get; set; }

        #endregion

        #region Property of FacingPdc
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
        [Column("FACING_PDC", TypeName = "VARCHAR")]
        public string FacingPdc { get; set; }

        #endregion

        #region Property of Definition
        /// <summary>
        /// ����Ƶ��
        ///Format:
        ///1st Stk
        ///2nd Stk
        ///Normal Stk--only once S
        /// </summary>
        [Column("DEFINITION", TypeName = "VARCHAR")]
        public string Definition { get; set; }

        #endregion

        #region Property of DealerType
        /// <summary>
        /// ����������
        ///Format:
        ///PC
        ///Van
        ///Truck
        ///VPC
        /// </summary>
        [Column("DEALER_TYPE", TypeName = "VARCHAR")]
        public string DealerType { get; set; }

        #endregion

        #region Property of ShortDealerName
        /// <summary>
        /// Dealer���
        ///
        /// </summary>
        [Column("SHORT_DEALER_NAME", TypeName = "NVARCHAR")]
        public string ShortDealerName { get; set; }

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

        #region Property of CutoffDay
        /// <summary>
        /// �ص�����
        ///Format:
        ///1��Monday
        ///2��Tuesday
        ///...
        ///7��Sunday
        /// </summary>
        [Column("CUTOFF_DAY", TypeName = "INT")]
        public int CutoffDay { get; set; }

        #endregion

        #region Property of CutoffTime
        /// <summary>
        /// �ص�ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("CUTOFF_TIME", TypeName = "DATETIME")]
        public DateTime CutoffTime { get; set; }

        #endregion

        #region Property of PickupDay
        /// <summary>
        /// �������=��ӡ����
        ///Format:
        ///1��Monday
        ///2��Tuesday
        ///...
        ///7��Sunday
        /// </summary>
        [Column("PICKUP_DAY", TypeName = "INT")]
        public int PickupDay { get; set; }

        #endregion

        #region Property of PickupTime
        /// <summary>
        /// ���ʱ��=TMS��ӡʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("PICKUP_TIME", TypeName = "DATETIME")]
        public DateTime PickupTime { get; set; }

        #endregion

        #region Property of TmsRouteName
        /// <summary>
        /// TMS·������
        ///Format:
        ///by TMS System
        /// </summary>
        [Column("TMS_ROUTE_NAME", TypeName = "VARCHAR")]
        public string TmsRouteName { get; set; }

        #endregion

        #region Property of StopOrder
        /// <summary>
        /// ж����˳��
        ///Format:
        ///����
        ///1,2,3,4��
        /// </summary>
        [Column("STOP_ORDER", TypeName = "INT")]
        public int? StopOrder { get; set; }

        #endregion

        #region Property of TransMode
        /// <summary>
        /// ����ģʽ
        ///Format:
        ///by TMS System
        /// </summary>
        [Column("TRANS_MODE", TypeName = "VARCHAR")]
        public string TransMode { get; set; }

        #endregion

        #region Property of Overweek
        /// <summary>
        /// ������
        ///Format:
        ///0������
        ///1����һ��
        ///...
        /// </summary>
        [Column("OVERWEEK", TypeName = "INT")]
        public int Overweek { get; set; }

        #endregion

        #region Property of ArrivalDay
        /// <summary>
        /// ��������
        ///Format:
        ///1��Monday
        ///2��Tuesday
        ///...
        ///7��Sunday
        /// </summary>
        [Column("ARRIVAL_DAY", TypeName = "INT")]
        public int ArrivalDay { get; set; }

        #endregion

        #region Property of Eta
        /// <summary>
        /// ����ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("ETA", TypeName = "DATETIME")]
        public DateTime? Eta { get; set; }

        #endregion

        #region Property of ArrivalTime
        /// <summary>
        /// ����ʱ��(������)
        ///Format:
        ///am/pm
        /// </summary>
        [Column("ARRIVAL_TIME", TypeName = "VARCHAR")]
        public string ArrivalTime { get; set; }

        #endregion

        #region Property of ArrivalTimeStart
        /// <summary>
        /// �����綨�忪ʼʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("ARRIVAL_TIME_START", TypeName = "DATETIME")]
        public DateTime ArrivalTimeStart { get; set; }

        #endregion

        #region Property of ArrivalTimeEnd
        /// <summary>
        /// �����綨�����ʱ��
        ///Format:
        ///HH:mm:ss
        ///12Сʱ��
        /// </summary>
        [Column("ARRIVAL_TIME_END", TypeName = "DATETIME")]
        public DateTime ArrivalTimeEnd { get; set; }

        #endregion

        #region Property of Forwarder
        /// <summary>
        /// ������
        ///Format:
        ///FC/KE/SF
        /// </summary>
        [Column("FORWARDER", TypeName = "VARCHAR")]
        public string Forwarder { get; set; }

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
