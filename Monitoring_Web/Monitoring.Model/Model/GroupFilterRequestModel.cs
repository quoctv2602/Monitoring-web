using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class GroupFilterRequestModel
    {
        public string? GroupName { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
    }
}
