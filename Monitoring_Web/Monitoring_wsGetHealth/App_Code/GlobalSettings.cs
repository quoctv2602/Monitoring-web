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
        
        public static int ScheduleCronExpression()
        {
            return Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ScheduleCronExpression"])*1000;
        } 
        public static int ScheduleGetSummaryErrors()
        {
            return Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("DiConnectConfig")["ScheduleGetSummaryErrors"])*1000;
        }
        public static int ScheduleGetTopErrors()
        {
            return Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("DiConnectConfig")["ScheduleGetTopErrors"])*1000;
        } 
        public static int TopErrors()
        {
            return Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("DiConnectConfig")["TopErrors"]);
        }
        public static string ScheduledTimeDaily()
        { 
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ScheduledTimeDaily"];
        } 
        

    }
}
