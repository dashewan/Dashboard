//*******************************************
// ** Description:  Data Access Object for BasCity
// ** Author     :  Code generator
// ** Created    :  2011-11-21 9:00:00
// ** Modified   :
//*******************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dashboard.Domain.ValidationAttributes.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Domain.MasterData
{
    //BAS_CITY
    [Table("BAS_CITY")]
    public class CBasCity
    {
        /// <summary>
        /// BAS_CITY_ID
        /// </summary>
        [Key]
        [Column("BAS_CITY_ID", TypeName = "VARCHAR")]
        public string BasCityId { get; set; }

        /// <summary>
        /// PROVINCE_CODE
        /// </summary>
        [Column("PROVINCE_CODE", TypeName = "VARCHAR")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// PROVINCE_NAME
        /// </summary>
        [Column("PROVINCE_NAME", TypeName = "NVARCHAR")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// ZIPCODE
        /// </summary>
        [Column("ZIPCODE", TypeName = "VARCHAR")]
        [Zip(ErrorMessage = "<=IsZip>")]
        public string Zipcode { get; set; }

        /// <summary>
        /// DISTRICT_NUM
        /// </summary>
        [Column("DISTRICT_NUM", TypeName = "VARCHAR")]
        [DistrictNum(ErrorMessage = "<=IsDistrictNum>")]
        public string DistrictNum { get; set; }

        /// <summary>
        /// ACTIVE
        /// </summary>
        [Column("ACTIVE", TypeName = "BIT")]
        public bool Active { get; set; }

        /// <summary>
        /// CREATED_USER_ID
        /// </summary>
        [Column("CREATED_USER_ID", TypeName = "VARCHAR")]
        public string CreatedUserId { get; set; }

        /// <summary>
        /// CREATED_USER_CODE
        /// </summary>
        [Column("CREATED_USER_CODE", TypeName = "VARCHAR")]
        public string CreatedUserCode { get; set; }

        /// <summary>
        /// CREATED_USER_NAME
        /// </summary>
        [Column("CREATED_USER_NAME", TypeName = "NVARCHAR")]
        public string CreatedUserName { get; set; }

        /// <summary>
        /// CREATED_OFFICE_ID
        /// </summary>
        [Column("CREATED_OFFICE_ID", TypeName = "VARCHAR")]
        public string CreatedOfficeId { get; set; }

        /// <summary>
        /// CREATED_OFFICE_CODE
        /// </summary>
        [Column("CREATED_OFFICE_CODE", TypeName = "VARCHAR")]
        public string CreatedOfficeCode { get; set; }

        /// <summary>
        /// CITY_CODE
        /// </summary>
        [Required(ErrorMessage = "<=CityCodeNotNull>")]
        [Column("CITY_CODE", TypeName = "VARCHAR")]
        public string CityCode { get; set; }

        /// <summary>
        /// CREATED_OFFICE_NAME
        /// </summary>
        [Column("CREATED_OFFICE_NAME", TypeName = "NVARCHAR")]
        public string CreatedOfficeName { get; set; }

        /// <summary>
        /// CREATED_DATE
        /// </summary>
        [Column("CREATED_DATE", TypeName = "DATETIME")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// UPDATED_USER_ID
        /// </summary>
        [Column("UPDATED_USER_ID", TypeName = "VARCHAR")]
        public string UpdatedUserId { get; set; }

        /// <summary>
        /// UPDATED_USER_CODE
        /// </summary>
        [Column("UPDATED_USER_CODE", TypeName = "VARCHAR")]
        public string UpdatedUserCode { get; set; }

        /// <summary>
        /// UPDATED_USER_NAME
        /// </summary>
        [Column("UPDATED_USER_NAME", TypeName = "NVARCHAR")]
        public string UpdatedUserName { get; set; }

        /// <summary>
        /// UPDATED_OFFICE_ID
        /// </summary>
        [Column("UPDATED_OFFICE_ID", TypeName = "VARCHAR")]
        public string UpdatedOfficeId { get; set; }

        /// <summary>
        /// UPDATED_OFFICE_CODE
        /// </summary>
        [Column("UPDATED_OFFICE_CODE", TypeName = "VARCHAR")]
        public string UpdatedOfficeCode { get; set; }

        /// <summary>
        /// UPDATED_OFFICE_NAME
        /// </summary>
        [Column("UPDATED_OFFICE_NAME", TypeName = "NVARCHAR")]
        public string UpdatedOfficeName { get; set; }

        /// <summary>
        /// UPDATED_DATE
        /// </summary>
        [Column("UPDATED_DATE", TypeName = "DATETIME")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// CITY_NAME
        /// </summary>
        [Required(ErrorMessage = "<=CityNameNotNull>")]
        [Column("CITY_NAME", TypeName = "NVARCHAR")]
        public string CityName { get; set; }

        /// <summary>
        /// CITY_NAME_EN
        /// </summary>
        [Column("CITY_NAME_EN", TypeName = "VARCHAR")]
        public string CityNameEn { get; set; }

        /// <summary>
        /// SUPERCITY_ID
        /// </summary>
        [Column("SUPERCITY_ID", TypeName = "VARCHAR")]
        public string SupercityId { get; set; }

        /// <summary>
        /// SUPERCITY_CODE
        /// </summary>
        [Column("SUPERCITY_CODE", TypeName = "VARCHAR")]
        public string SupercityCode { get; set; }

        /// <summary>
        /// CITY_TYPE_CODE
        /// </summary>
        [Column("CITY_TYPE_CODE", TypeName = "VARCHAR")]
        public string CityTypeCode { get; set; }

        /// <summary>
        /// CITY_TYPE_NAME
        /// </summary>
        [Column("CITY_TYPE_NAME", TypeName = "NVARCHAR")]
        public string CityTypeName { get; set; }

        /// <summary>
        /// PROVINCE_ID
        /// </summary>
        [Column("PROVINCE_ID", TypeName = "VARCHAR")]
        public string ProvinceId { get; set; }

    }
}

