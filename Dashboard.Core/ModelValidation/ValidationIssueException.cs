using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Core.ModelValidation
{
    public class ValidationIssueException :Exception 
    {
        public ValidationIssueException(List<ValidationIssue> validationIssues)
        {
            this.ValidationIssues = validationIssues;
        }

        public List<ValidationIssue> ValidationIssues { get; set; }
    }
}
