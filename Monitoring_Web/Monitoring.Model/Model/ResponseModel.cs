using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Model.Model
{
    public class ResponseModel<T>
    {
        public int ResponseStatus { get; set; }
        public string Description { get; set; }
        public T Data { get; set; }
    }
}
