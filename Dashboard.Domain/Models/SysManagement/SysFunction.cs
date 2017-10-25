//*******************************************
// ** Description:  Data Access Object for SysFunction
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
	/// Data Access Object for SysFunction
	/// </summary>
    [Table("SYS_FUNCTION")]
	public class  SysFunction
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		[Column("SYS_FUNCTION_ID",TypeName = "VARCHAR")]
		public string SysFunctionId{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("FUNCTION_NAME",TypeName = "NVARCHAR")]
		public string FunctionName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("ACTION_NAME",TypeName = "NVARCHAR")]
		public string ActionName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("CONTROLLER_NAME",TypeName = "NVARCHAR")]
		public string ControllerName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("AREA_NAME",TypeName = "NVARCHAR")]
		public string AreaName{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("FUNCTION_TYPE",TypeName = "INT")]
		public int FunctionType{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("ACTIVE",TypeName = "BIT")]
		public bool Active{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("SORTID",TypeName = "INT")]
		public int Sortid{get;set;}

		/// <summary>
		/// 
		/// </summary>
		[Column("REMARKS",TypeName = "NVARCHAR")]
		public string Remarks{get;set;}

        /// <summary>
        /// 
        /// </summary>
        [Column("PARENT_ID", TypeName = "VARCHAR")]
        public string ParentId { get; set; }


        [Column("GROUP_TYPE", TypeName = "INT")]
        public int GroupType { get; set; }

      
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<SysRoleFunction> SysRoleFunction { get; set; }



	}
}
