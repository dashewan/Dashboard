//*******************************************
// ** Description:  Data Access Object for CBasCR
// ** Author     :  Code generator
// ** Created    :  2014-08-18 14:36:46
// ** Modified   :  CR表的实体类
//*******************************************
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Dashboard.Domain.Models.BasicData
{
    [Table("CR")]
    public class CBasCR
    {
        [Key]
        [Column("PCRNumber", TypeName = "NVARCHAR")]
        public string PCRNumber { get; set; }

        [Column("CRName", TypeName = "NVARCHAR")]
        public string CRName { get; set; }

        [Column("Type", TypeName = "NVARCHAR")]
        public string Type { get; set; }

        [Column("Priority", TypeName = "NVARCHAR")]
        public string Priority { get; set; }

        [Column("ChangeRequestDescription", TypeName = "NVARCHAR")]
        public string ChangeRequestDescription { get; set; }

        [Column("ROIAnalysis", TypeName = "NVARCHAR")]
        public string ROIAnalysis { get; set; }

        [Column("ITOwner", TypeName = "NVARCHAR")]
        public string ITOwner { get; set; }

        [Column("BusinessDepartment", TypeName = "NVARCHAR")]
        public string BusinessDepartment { get; set; }

        [Column("Status", TypeName = "NVARCHAR")]
        public string Status { get; set; }

        [Column("ITPCID", TypeName = "NVARCHAR")]
        public string ITPCID { get; set; }

        [Column("ProductName", TypeName = "NVARCHAR")]
        public string ProductName { get; set; }

        [Column("KickedOffDate", TypeName = "DATE")]
        public DateTime? KickedOffDate { get; set; }

        [Column("TargetCompletionDate", TypeName = "DATE")]
        public DateTime? TargetCompletionDate { get; set; }

        [Column("ActualCompletionDate", TypeName = "DATE")]
        public DateTime? ActualCompletionDate { get; set; }

        [Column("EstBudget", TypeName = "FLOAT")]
        public float? EstBudget { get; set; }

        [Column("ActBudget", TypeName = "FLOAT")]
        public float? ActBudget { get; set; }

        [Column("PaymentStatus", TypeName = "NVARCHAR")]
        public string PaymentStatus { get; set; }

        [Column("RevisedTarget", TypeName = "FLOAT")]
        public float? RevisedTarget { get; set; }

        [Column("RevisedCompletionDate", TypeName = "DATE")]
        public DateTime? RevisedCompletionDate { get; set; }

        [Column("ActualstartDate", TypeName = "DATE")]
        public DateTime? ActualstartDate { get; set; }

        [Column("ProjectID", TypeName = "NVARCHAR")]
        public string ProjectID { get; set; }

        [Column("CurrencyCode", TypeName = "NVARCHAR")]
        public string CurrencyCode { get; set; }
    }
}
