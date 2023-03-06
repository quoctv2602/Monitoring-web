using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class UserLoginModel
    {
        public string? Email { get; set; }
        public string? DefaultPage { get; set; }
        public List<int>? Permission { get; set; }
        public bool? IsDelete { get; set; }
    }
}
