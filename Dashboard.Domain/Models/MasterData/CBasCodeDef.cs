//*******************************************
// ** Description:  Data Access Object for BasCodeDef
// ** Author     :  Code generator
// ** Created    :  2011-11-28 14:17:21
// ** Modified   :
//*******************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dashboard.Domain.MasterData;
using System.ComponentModel.DataAnnotations;
using Dashboard.Domain.MVCExtension;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dashboard.Domain.MasterData
{
    [Table("BAS_CODE_DEF")]
	/// <summary>
	/// Data Access Object for BasCodeDef
	/// </summary>
	public class  CBasCodeDef
	{
		/// <summary>
		/// ����ID
		/// </summary>
		[Key]
		[Column("BAS_CODE_DEF_ID",TypeName = "VARCHAR")]
		public string BasCodeDefId{get;set;}

		/// <summary>
		/// ���
		/// </summary>
		[ForeignKey("CBasCodeType")]
		[Column("BAS_CODE_TYPE_ID",TypeName = "VARCHAR")]
		public string BasCodeTypeId{get;set;}

		/// <summary>
		/// ����ֵ
		///
		/// </summary>
		[Column("CODE_VALUE",TypeName = "VARCHAR")]
		public string CodeValue{get;set;}

		/// <summary>
		/// �������ݣ����ģ�
		///
		/// </summary>
		[Column("DISPLAY_VALUE",TypeName = "NVARCHAR")]
		public string DisplayValue{get;set;}

		/// <summary>
		/// �������ݣ�Ӣ�ģ�
		///
		/// </summary>
		[Column("DISPLAY_VALUE_EN",TypeName = "VARCHAR")]
		public string DisplayValueEn{get;set;}

		/// <summary>
		/// ��Ч���
		/// </summary>
		[Column("ACTIVE",TypeName = "BIT")]
		public bool Active{get;set;}

		/// <summary>
		/// ������ID
		/// </summary>
		[Column("CREATED_USER_ID",TypeName = "VARCHAR")]
		public string CreatedUserId{get;set;}

		/// <summary>
		/// �����˱���
		/// </summary>
		[Column("CREATED_USER_CODE",TypeName = "VARCHAR")]
		public string CreatedUserCode{get;set;}

		/// <summary>
		/// ����������
		/// </summary>
		[Column("CREATED_USER_NAME",TypeName = "NVARCHAR")]
		public string CreatedUserName{get;set;}

		/// <summary>
		/// �������´�ID
		/// </summary>
		[Column("CREATED_OFFICE_ID",TypeName = "VARCHAR")]
		public string CreatedOfficeId{get;set;}

		/// <summary>
		/// �������´�����
		/// </summary>
		[Column("CREATED_OFFICE_CODE",TypeName = "VARCHAR")]
		public string CreatedOfficeCode{get;set;}

		/// <summary>
		/// �������´�����
		/// </summary>
		[Column("CREATED_OFFICE_NAME",TypeName = "NVARCHAR")]
		public string CreatedOfficeName{get;set;}

		/// <summary>
		/// ����ʱ��
		/// </summary>
		[Column("CREATED_DATE",TypeName = "DATETIME")]
		public DateTime? CreatedDate{get;set;}

		/// <summary>
		/// ������ID
		/// </summary>
		[Column("UPDATED_USER_ID",TypeName = "VARCHAR")]
		public string UpdatedUserId{get;set;}

		/// <summary>
		/// �����˴���
		/// </summary>
		[Column("UPDATED_USER_CODE",TypeName = "VARCHAR")]
		public string UpdatedUserCode{get;set;}

		/// <summary>
		/// ����������
		/// </summary>
		[Column("UPDATED_USER_NAME",TypeName = "NVARCHAR")]
		public string UpdatedUserName{get;set;}

		/// <summary>
		/// ���°��´�ID
		/// </summary>
		[Column("UPDATED_OFFICE_ID",TypeName = "VARCHAR")]
		public string UpdatedOfficeId{get;set;}

		/// <summary>
		/// ���°��´�����
		/// </summary>
		[Column("UPDATED_OFFICE_CODE",TypeName = "VARCHAR")]
		public string UpdatedOfficeCode{get;set;}

		/// <summary>
		/// ���°��´�����
		/// </summary>
		[Column("UPDATED_OFFICE_NAME",TypeName = "NVARCHAR")]
		public string UpdatedOfficeName{get;set;}

		/// <summary>
		/// ��������
		/// </summary>
		[Column("UPDATED_DATE",TypeName = "DATETIME")]
		public DateTime? UpdatedDate{get;set;}

		/// <summary>
		/// 
		/// </summary>
        public virtual CBasCodeType CBasCodeType{ get; set; }

	}
}
