//*******************************************
// ** Description:  Data Access Object for BasAllDataFormConfig
// ** Author     :  Code generator
// ** Created    :  2014-08-29 09:05:06
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

    [Table("BAS_ALL_DATA_FORM_CONFIG")]
    /// <summary>
    /// Data Access Object for BasAllDataFormConfig
    /// </summary>
    public class CBasAllDataFormConfig
    {
        #region Property of BasAllDataFormConfigId
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        [Column("BAS_ALL_DATA_FORM_CONFIG_ID", TypeName = "VARCHAR")]
        public string BasAllDataFormConfigId { get; set; }
        #endregion

        #region Property of SysUserId
        /// <summary>
        /// �û�ID
        /// </summary>
        [Column("SYS_USER_ID", TypeName = "VARCHAR")]
        public string SysUserId { get; set; }

        #endregion

        #region Property of UserCode
        /// <summary>
        /// �û�����
        /// </summary>
        [Column("USER_CODE", TypeName = "VARCHAR")]
        public string UserCode { get; set; }

        #endregion

        #region Property of FieldName
        /// <summary>
        /// �ֶ���
        /// </summary>
        [Column("FIELD_NAME", TypeName = "VARCHAR")]
        public string FieldName { get; set; }

        #endregion

        #region Property of DisplayName
        /// <summary>
        /// ��ʾ����
        /// </summary>
        [Column("DISPLAY_NAME", TypeName = "VARCHAR")]
        public string DisplayName { get; set; }

        #endregion

        #region Property of SerialNo
        /// <summary>
        /// ���
        /// </summary>
        [Column("SERIAL_NO", TypeName = "INT")]
        public int SerialNo { get; set; }

        #endregion

        #region Property of UserSerialNo
        /// <summary>
        /// �û����
        /// </summary>
        [Column("USER_SERIAL_NO", TypeName = "INT")]
        public int UserSerialNo { get; set; }

        #endregion

        #region Property of IsDefault
        /// <summary>
        /// �Ƿ���Ĭ����
        /// </summary>
        [Column("IS_DEFAULT", TypeName = "BIT")]
        public bool IsDefault { get; set; }

        #endregion

    }
}
