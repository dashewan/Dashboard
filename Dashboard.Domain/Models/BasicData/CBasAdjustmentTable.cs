//*******************************************
// ** Description:  Data Access Object for BasAdjustmentTable
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

    [Table("BAS_ADJUSTMENT_TABLE")]
    /// <summary>
    /// Data Access Object for BasAdjustmentTable
    /// </summary>
    public class CBasAdjustmentTable
    {
        #region Property of BasAdjustmentTableId
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        [Column("BAS_ADJUSTMENT_TABLE_ID", TypeName = "VARCHAR")]
        public string BasAdjustmentTableId { get; set; }
        #endregion

        #region Property of UserCode
        /// <summary>
        /// Adjustment Table�ϴ���
        /// </summary>
        [Column("USER_CODE", TypeName = "VARCHAR")]
        public string UserCode { get; set; }

        #endregion

        #region Property of AdjustmentType
        /// <summary>
        /// Adjustment Table ��������
        ///���ڵ����ĸ�����
        /// </summary>
        [Column("ADJUSTMENT_TYPE", TypeName = "VARCHAR")]
        public string AdjustmentType { get; set; }

        #endregion

        #region Property of PrintDate
        /// <summary>
        /// Exclude STK of below TN print date
        /// </summary>
        [Column("PRINT_DATE", TypeName = "DATE")]
        public DateTime? PrintDate { get; set; }

        #endregion

        #region Property of TnNo
        /// <summary>
        /// Exclude below TN number
        /// </summary>
        [Column("TN_NO", TypeName = "VARCHAR")]
        public string TnNo { get; set; }

        #endregion

        #region Property of TnProperty
        /// <summary>
        /// Exclude below TN Property
        /// </summary>
        [Column("TN_PROPERTY", TypeName = "VARCHAR")]
        public string TnProperty { get; set; }

        #endregion

        #region Property of Serial
        /// <summary>
        /// Serial�����ֶ�
        /// </summary>
        [Column("SERIAL", TypeName = "VARCHAR")]
        public string Serial { get; set; }

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
