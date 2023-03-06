using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class UserProfileFilterRequestModel
    {
        public string? Email { get; set; }
        public int? UserType { get; set; }
        public bool? IsDelete { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
