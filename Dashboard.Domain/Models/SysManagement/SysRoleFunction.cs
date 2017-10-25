//*******************************************
// ** Description:  Data Access Object for SysRoleFunction
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
	/// Data Access Object for SysRoleFunction
	/// </summary>
    [Table("SYS_ROLE_FUNCTION")]
	public class  SysRoleFunction
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		[Column("SYS_ROLE_FUNCTION_ID",TypeName = "VARCHAR")]
		public string SysRoleFunctionId{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[ForeignKey("SysFunction")]
		[Column("SYS_FUNCTION_ID",TypeName = "VARCHAR")]
		public string SysFunctionId{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[ForeignKey("SysRole")]
		[Column("SYS_ROLE_ID",TypeName = "VARCHAR")]
		public string SysRoleId{get;set;}

        /// <summary>
        /// 
        /// </summary>
        public virtual SysFunction SysFunction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual SysRole SysRole { get; set; }

	}

    public class CRoleFunctionExport
    {
        public string ROLE_CODE { get; set; }
        public string GROUP_TYPE { get; set; }
        public string PAGE_NAME { get; set; }
        public string FUNCTION_NAME { get; set; }
        public int SORTID { get; set; }
        public int GROUP_SORTID { get; set; }
    }
}
