//*******************************************
// ** Description:  Data Access Object for BasCodeType
// ** Author     :  Code generator
// ** Created    :  2011-11-28 14:17:21
// ** Modified   :
//*******************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Domain.MasterData
{
    [Table("BAS_CODE_TYPE")]
	/// <summary>
	/// Data Access Object for BasCodeType
	/// </summary>
	public class  CBasCodeType
	{
		/// <summary>
		/// Phisical Primary Key
		/// </summary>
		[Key]
		[Column("BAS_CODE_TYPE_ID",TypeName = "VARCHAR")]
		public string BasCodeTypeId{get;set;}

		/// <summary>
		/// ��������
		/// </summary>
		[Column("CODE_NAME",TypeName = "NVARCHAR")]
		public string CodeName{get;set;}

		/// <summary>
		/// ���ͱ���
		/// </summary>
		[Column("CODE_TYPE",TypeName = "VARCHAR")]
		public string CodeType{get;set;}

		/// <summary>
		/// ���͵ȼ�
		/// </summary>
		[Column("CODE_GRADE",TypeName = "NUMERIC")]
		public Decimal? CodeGrade{get;set;}

		/// <summary>
		/// ������
		/// </summary>
		[Column("CODE_WIDTH",TypeName = "INT")]
		public int? CodeWidth{get;set;}

		/// <summary>
		/// ��������
		/// </summary>
		[Column("CODE_DESC",TypeName = "VARCHAR")]
		public string CodeDesc{get;set;}

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
		public virtual ICollection<CBasCodeDef> BasCodeDef{get;set;}

	}
}
