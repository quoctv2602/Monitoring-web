using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class ActionModel
    {
        public int ActionId { get; set; }
        public string? ActionName { get; set; }
        public bool? IsSelected { get; set; }
    }
}
