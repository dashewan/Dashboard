//*******************************************
// ** Description:  Data Access Object for SysDataAuth
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
	/// Data Access Object for SysDataAuth
	/// </summary>
    [Table("SYS_DATA_AUTH")]
	public class  SysDataAuth
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		[Column("SYS_DATA_AUTH_ID",TypeName = "VARCHAR")]
		public string SysDataAuthId{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[ForeignKey("SysRole")]
		[Column("SYS_ROLE_ID",TypeName = "VARCHAR")]
		public string SysRoleId{get;set;}

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("SysOrganization")]
        [Column("SYS_ORGANIZATION_ID", TypeName = "VARCHAR")]
        public string SysOrganizationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual SysRole SysRole { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual SysOrganization SysOrganization { get; set; }

	}
}
