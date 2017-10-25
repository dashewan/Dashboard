//*******************************************
// ** Description:  Data Access Object for BasProvince
// ** Author     :  Code generator
// ** Created    :  2011-11-22 18:56:56
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
    [Table("BAS_PROVINCE")]
	/// <summary>
	/// Data Access Object for BasProvince
	/// </summary>
	public class  CBasProvince
	{
		/// <summary>
		/// Physical Primary Key
		/// </summary>
		[Key]
		[Column("BAS_PROVINCE_ID",TypeName = "VARCHAR")]
		public string BasProvinceId{get;set;}

		/// <summary>
		/// ʡ�ݴ���
		/// </summary>
        ///[Required(ErrorMessage = "<=CityCodeNotNull>")]
		[Column("PROVINCE_CODE",TypeName = "VARCHAR")]
		public string ProvinceCode{get;set;}

		/// <summary>
		/// ʡ����������
		/// </summary>
		[Column("PROVINCE_NAME",TypeName = "NVARCHAR")]
		public string ProvinceName{get;set;}

		/// <summary>
		/// ʡ��Ӣ������
		/// </summary>
		[Column("PROVINCE_NAME_EN",TypeName = "VARCHAR")]
		public string ProvinceNameEn{get;set;}

		/// <summary>
		/// �������
		/// </summary>
		[Column("REGION_CODE",TypeName = "VARCHAR")]
		public string RegionCode{get;set;}

		/// <summary>
		/// ʡ��
		/// </summary>
		[Column("PROVINCE_CAPITAL",TypeName = "VARCHAR")]
		public string ProvinceCapital{get;set;}

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

	}
}
