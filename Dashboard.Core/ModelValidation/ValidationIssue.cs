using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.Core.ModelValidation
{
   public  class ValidationIssue
    {
        public ValidationIssue(string key, object attemptedValue, string errorMessage)
        {
            this.Key = key;
            this.AttemptedValue = attemptedValue;
            this.ErrorMessage = errorMessage;
        }

        public string Key { get; set; }

        public object AttemptedValue { get; set; }

        public string ErrorMessage { get; set; }
    }
}
