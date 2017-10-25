//*******************************************
// ** Description:  Data Access Object for SysOrganization
// ** Author     :  Code generator
// ** Created    :  2011-11-30 14:07:03
// ** Modified   :
//*******************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Domain.SysManagement
{

    public class MyClass
    {
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public int Count { get; set; }
    }


    /// <summary>
    /// Data Access Object for SysOrganization
    /// </summary>
    [Table("SYS_ORGANIZATION")]
    public class SysOrganization
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Column("SYS_ORGANIZATION_ID", TypeName = "VARCHAR")]
        public string SysOrganizationId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column("ORGANIZATION_NAME", TypeName = "NVARCHAR")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ORGANIZATION_CODE", TypeName = "VARCHAR")]
        public string OrganizationCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ACTIVE", TypeName = "BIT")]
        public bool Active { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("PARENT_ID", TypeName = "VARCHAR")]
        public string ParentId { get; set; }


        [Column("CREATED_DATE", TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("REMARKS", TypeName = "NVARCHAR")]
        public string Remarks { get; set; }

        [Column("IS_INTERIOR", TypeName = "BIT")]
        public bool IsInterior { get; set; }


        [Column("IS_WAREHOUSE", TypeName = "BIT")]
        public bool IsWarehouse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<SysRole> SysRole { get; set; }

        public virtual ICollection<SysDataAuth> SysDataAuth { get; set; }

    }
}
