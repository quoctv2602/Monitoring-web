using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class IntegrationAPI
    {
        public int ID { get; set; }
        public int? NodeID { get; set; }
        public string? NodeName { get; set; }
        public string? ServiceList { get; set; }
        public List<ListService>? Service { get; set; }
    }
    public class ListService
    {
        public int ID { get; set; }
        public string? name { get; set; }
    }
}
