using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class EnvironmentModel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Comment { get; set; }
        public List<IntegrationAPI> ListIntegrationAPI { get; set; }
    }
}
