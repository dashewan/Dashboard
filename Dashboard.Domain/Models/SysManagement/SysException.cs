using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Domain.SysManagement
{
    public class SysException
    {
        [Key]
        public Guid SysExceptionId { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
        public string HttpReferrer { get; set; }
        public string HttpVerb { get; set; }
        public string HttpPathAndQuery { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
