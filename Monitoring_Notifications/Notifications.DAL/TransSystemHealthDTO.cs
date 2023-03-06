using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL
{
    public class TransSystemHealthDTO
    {
        public int EnvironmentId {get;set;}
        public string MachineName { get;set;}
   
    }
    public class TransSystemHealthProcessService
    {
        public int EnvironmentId { get; set; }
        public string MachineName { get; set; }
       
        public string Service { get; set; }
    }
}
