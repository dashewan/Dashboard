using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dashboard.Domain.SysManagement
{
    [Table("SYS_ROLE")]
    public class SysRole
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Column("SYS_ROLE_ID", TypeName = "VARCHAR")]
        public string SysRoleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ROLE_CODE", TypeName = "NVARCHAR")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ROLE_NAME", TypeName = "NVARCHAR")]
        public string RoleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("SysOrganization")]
        [Column("SYS_ORGANIZATION_ID", TypeName = "VARCHAR")]
        public string SysOrganizationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("REMARKS", TypeName = "NVARCHAR")]
        public string Remarks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<SysRoleFunction> SysRoleFunction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<SysUserRole> SysUserRole { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<SysDataAuth> SysDataAuth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual SysOrganization SysOrganization { get; set; }

        public virtual ICollection<SysUser> SysUser { get; set; }

      
    }
}
