//*******************************************
// ** Description:  Data Access Object for SysUserRole
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

	/// <summary>
	/// Data Access Object for SysUserRole
	/// </summary>
    [Table("SYS_USER_ROLE")]
	public class  SysUserRole
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		[Column("SYS_User_ROLE_ID",TypeName = "NVARCHAR")]
		public string SysUserRoleId{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[ForeignKey("SysUser")]
		[Column("SYS_USER_ID",TypeName = "VARCHAR")]
		public string SysUserId{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[ForeignKey("SysRole")]
		[Column("SYS_ROLE_ID",TypeName = "VARCHAR")]
		public string SysRoleId{get;set;}

        /// <summary>
        /// 
        /// </summary>
        public virtual SysUser SysUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual SysRole SysRole { get; set; }

	}

    public class CRoleUserExport
    {
        public string SYS_ROLE_ID { get; set; }
        public string ROLE_CODE { get; set; }
        public string User_CODE { get; set; }
        public string User_Name { get; set; }
        public string Remark { get; set; }
    }
}
