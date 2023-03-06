using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class ErrorStatusModel
    {
        public int ErrorStatus { get; set; }

        public string ErrorName { get; set; }

        public string Description { get; set; }

        public int? GroupCode { get; set; }
    }
}
