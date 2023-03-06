using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring_wsAddCounters.App_Code
{
    public class GlobalSettings
    {

        public static string ProcessList()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ProcessList"];
        }
        public static string ShareFolderCheck()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ShareFolderCheck"];
        }
        public static string ShareFolderFrom()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ShareFolderFrom"];
        }
        public static string ShareFolderTo()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["ShareFolderTo"];
        }

        public static string EDItoASCIICheck()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["EDItoASCII"];
        }



        public static string EDItoASCIIConfigApp()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["EDItoASCIIConfigApp"];
        }

        public static string EDItoASCIIConfigData()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["EDItoASCIIConfigData"];
        }
        public static int Schedule_Service()
        {
            return 1000 * Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("MyConfig")["Schedule_Service"]);
        }
       


    }
}
