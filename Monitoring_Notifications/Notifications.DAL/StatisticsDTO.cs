using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL
{
    public class StatisticsDTO
    {
        public string EnvironmentName { get; set; }
        public string MachineName { get; set; }
        public int Average { get; set; }
    }
    
    public class StatisticsDTO_FreeDisk  
    {
        public string EnvironmentName { get; set; }
        public string MachineName { get; set; }
        public long Average { get; set; }
        public string DriveName { get; set; }
         
    }
}
