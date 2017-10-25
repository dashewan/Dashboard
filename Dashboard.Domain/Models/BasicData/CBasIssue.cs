using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Dashboard.Domain.Models.BasicData
{
    [Table("ISSUE")]
    public class CBasIssue
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("IssueNumber", TypeName = "NUMERIC")]
        public decimal IssueNumber { get; set; }

        [Column("Type", TypeName = "NVARCHAR")]
        public string Type { get; set; }

        [Column("Priority", TypeName = "NVARCHAR")]
        public string Priority { get; set; }

        [Column("SystemName", TypeName = "NVARCHAR")]
        public string SystemName { get; set; }

        [Column("CRNumber", TypeName = "NVARCHAR")]
        public string CRNumber { get; set; }

        [Column("Category", TypeName = "NCHAR")]
        public string Category { get; set; }

        [Column("IssueDescription", TypeName = "NCHAR")]
        public string IssueDescription { get; set; }

        [Column("Raisedby", TypeName = "NVARCHAR")]
        public string Raisedby { get; set; }

        [Column("Owner", TypeName = "NVARCHAR")]
        public string Owner { get; set; }

        [Column("OriginDate", TypeName = "DATE")]
        public DateTime? OriginDate { get; set; }

        [Column("DueDate", TypeName = "DATE")]
        public DateTime? DueDate { get; set; }

        [Column("ClosedDate", TypeName = "DATE")]
        public DateTime? ClosedDate { get; set; }

        [Column("OCI", TypeName = "NVARCHAR")]
        public string OCI { get; set; }

        [Column("ActionUpdates", TypeName = "NCHAR")]
        public string ActionUpdates { get; set; }
        
    }
}
