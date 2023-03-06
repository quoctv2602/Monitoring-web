using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class ServiceListRequestModel
    {
        public int NodeID { get; set; }
        public int EnvironmentID { get; set; }
        public string EnviromentName { get; set; } = null!;
        public string MachineName { get; set; } = null!;
    }
}
