using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class SessionTokenModel
    {
        public string? Token { get; set; }
        public List<int>? Permissions { get; set; }
    }
}
