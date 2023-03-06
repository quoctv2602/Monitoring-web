using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring_wsGetHealth.App_Code
{
    public class GlobalSettings
    {

        public static string ConnectionStrings()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ConnectionStrings"];
        }
        
        public static string DayCount()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["DayCount"];
        }
        public static string ProcessRunJobCron()
        { 
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ProcessRunJobCron"];
        } 
        

    }
}
