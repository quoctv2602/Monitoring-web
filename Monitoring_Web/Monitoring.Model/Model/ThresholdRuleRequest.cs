using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class ThresholdRuleRequest
    {

        public int MonitoringType { get; set; }
        public string? MonitoringName { get; set; }
        public byte? Condition { get; set; }
        public int? Threshold { get; set; }
        public int? ThresholdCounter { get; set; }
    }
    public class MoniorType
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }
}
