using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Domain.SysManagement
{

    [Table("SYS_USER_ROLE")]
    public class UserRole
    {
        [Column("SYS_User_ROLE_ID", TypeName = "VARCHAR")]
        public string UserRoleId { get; set; }

        [ForeignKey("SysUser")]
        [Column("SYS_USER_ID", TypeName = "VARCHAR")]
        public string SysUserId { get; set; }

        [ForeignKey("SysRole")]
        [Column("SYS_ROLE_ID", TypeName = "INT")]
        public string SysRoleId { get; set; }

        public virtual SysUser SysUser { get; set; }
        public virtual SysRole SysRole { get; set; }
    }
}
