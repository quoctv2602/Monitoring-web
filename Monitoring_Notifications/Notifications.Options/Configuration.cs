using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Options
{
    public class Configuration
    {
        public static Configs Set(IConfiguration configuration)
        {
            Configs configs = new Configs();
            configs.AppSettings = SetAppSettings(configuration);
            configs.DatabaseSettings = SetDatabaseSettings(configuration);
            return configs;
        }
        private static AppSettings SetAppSettings(IConfiguration configuration)
        {
            return configuration.GetSection("appsettings").Get<AppSettings>() ?? new AppSettings();
        }
        private static DatabaseSettings SetDatabaseSettings(IConfiguration configuration)
        {
            return configuration.GetSection("databases").Get<DatabaseSettings>() ?? new DatabaseSettings();
        }
    }
}
