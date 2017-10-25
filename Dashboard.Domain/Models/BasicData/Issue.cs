using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Domain.Models.BasicData
{
    public class Issue
    { 
        public decimal IssueNumber { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string SystemName { get; set; }
        public string CRNumber { get; set; }
        public string Category { get; set; }
        public string IssueDescription { get; set; }
        public string BusinessDepartment { get; set; }
        public string Owner { get; set; }
        public string OriginDate { get; set; }
        public string DueDate { get; set; }
        public string ClosedDate { get; set; }
        public string OCI { get; set; }
        public string ActionUpdates { get; set; }
        public string Raisedby { get; set; }
    }
}
