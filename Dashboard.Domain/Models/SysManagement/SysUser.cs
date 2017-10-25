using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Dashboard.Domain.ValidationAttributes.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Domain.SysManagement
{
    [Table("SYS_USER")]
    public class SysUser
    {
        [Key]
        [Column("SYS_USER_ID", TypeName = "VARCHAR")]
        public string SysUserId { get; set; }

        [Column("User_Name", TypeName = "VARCHAR")]
        [Remote("Exist","MasterData")]
       
        public string UserName { get; set; }

        [Column("User_CODE", TypeName = "VARCHAR")]
        public string UserCode { get; set; }

        [Column("PASSWORD", TypeName = "VARCHAR")]
        //[PassWord(ErrorMessage = "<=PasswordError>")]
        public string Pwd { get; set; }

        [Column("EMAIL", TypeName = "VARCHAR")]
        [Email(ErrorMessage = "<=EmailFormatError>")]
        public string Email { get; set; }

        [Column("PASS_CODE", TypeName = "VARCHAR")]
        public string PassCode { get; set; }

        [Column("Valid_Date_From", TypeName = "DATETIME")]
        public DateTime ValidDateFrom { get; set; }

        [Column("Valid_Date_To", TypeName = "DATETIME")]
        public DateTime ValidDateTo { get; set; }

        [Column("Language_Type", TypeName = "int")]
        public int LanguageType { get; set; }

        [Column("ACTIVE", TypeName = "BIT")]
        public bool Active { get; set; }

        [Column("Remarks", TypeName = "VARCHAR")]
        public string Remarks { get; set; }

        [Column("SYS_ROLE_ID", TypeName = "VARCHAR")]
        [ForeignKey("SysRole")]
        public string RoleId { get; set; }

        public virtual SysRole SysRole { get; set; }

        [Column("PASSWORD_DATE", TypeName = "DATETIME")]
        public DateTime? PasswordDate { get; set; }

        public string Team { get; set; }

        public virtual ICollection<SysUserRole> SysUserRole { get; set; }

        //[NotMapped()]
        //public SysOrganization Organization { get; set; }
    }
}
