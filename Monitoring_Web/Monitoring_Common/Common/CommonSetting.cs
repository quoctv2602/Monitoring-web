using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Monitoring_Common.Common
{
    public static class CommonSetting
    {
        public const string FullPermission = "Full";
        public const string DefaultPage = "dashboard";
        public const string EncryptKeySession = "user_login";
        public static int GetMaxWCFRetry()
        {
            int result = 5;
            var maxWCFRetry = GetValueFromAppSettings("AppSettings:WcfHelper.MaxRetry");
            if (StringValidate(maxWCFRetry))
            {
                int.TryParse(maxWCFRetry, out result);
            }
            return result;
        }
        public static List<string> GetWCFTransientExceptions()
        {
            var transientWCFException = GetValueFromAppSettings("AppSettings:WcfHelper.TransientExceptions");
            var transientWCFExceptions = new List<string>();
            if (StringValidate(transientWCFException))
            {
                transientWCFExceptions = Regex.Replace(GetValueFromAppSettings("AppSettings:WcfHelper.TransientExceptions"), @"\s+", "").Replace(" ", "").Split(',').ToList();
            }
            else
            {
                transientWCFExceptions = new List<string>(new[] { "ChannelTerminatedException", "CommunicationObjectAbortedException", "CommunicationObjectFaultedException",
                    "EndpointNotFoundException","FaultException","FaultException`1","ServerTooBusyException","ServiceActivationException","MessageFilterException",
                    "MessageSecurityException","SecurityAccessDeniedException","SecurityNegotiationException" });
            }
            return transientWCFExceptions;
        }
        public static int GetMaxServiceRetry()
        {
            int result = 5;
            var maxRetry = GetValueFromAppSettings("AppSettings:WebAPIHelper.MaxRetry");
            if (StringValidate(maxRetry))
            {
                int.TryParse(maxRetry, out result);
            }
            return result;
        }
        public static List<int> GetServiceErrorNumbers()
        {
            var connectionErrorNumber = GetValueFromAppSettings("AppSettings:WebAPIHelper.ErrorNumbers");
            var connectionErrorNumbers = new List<int>();
            if (StringValidate(connectionErrorNumber))
            {
                connectionErrorNumbers = GetValueFromAppSettings("AppSettings:WebAPIHelper.ErrorNumbers").Split(',').Select(Int32.Parse).ToList();
            }
            else
            {
                connectionErrorNumbers = new List<int>(new[] { 400, 403, 404, 406, 408, 429, 503 });
            }
            return connectionErrorNumbers;
        }
        public static int GetMaxDBRetryHelper()
        {
            int result = 5;
            var maxRetry = GetValueFromAppSettings("AppSettings:DBRetryHelper.MaxRetrytest");
            if (StringValidate(maxRetry))
            {
                int.TryParse(maxRetry, out result);
            }
            return result;
        }
        public static List<int> GetDBTransientErrorNumbers()
        {
            var transientErrorNumber = GetValueFromAppSettings("AppSettings:DBRetryHelper.TransientErrorNumbers");
            var transientErrorNumbers = new List<int>();
            if (StringValidate(transientErrorNumber))
            {
                transientErrorNumbers = GetValueFromAppSettings("AppSettings:DBRetryHelper.TransientErrorNumbers").Split(',').Select(Int32.Parse).ToList();
            }
            else
            {
                transientErrorNumbers = new List<int>(new[] { 1205, 4060, 40197, 40501, 40613, 49918, 49919, 49920, 4221, 41301, 41839 });
            }
            return transientErrorNumbers;
        }
        public static List<int> GetDBConnectionErrorNumbers()
        {
            var connectionErrorNumber = GetValueFromAppSettings("AppSettings:DBRetryHelper.ConnectionErrorNumbers");
            var connectionErrorNumbers = new List<int>();
            if (StringValidate(connectionErrorNumber))
            {
                connectionErrorNumbers = GetValueFromAppSettings("AppSettings:DBRetryHelper.ConnectionErrorNumbers").Split(',').Select(Int32.Parse).ToList();
            }
            else
            {
                connectionErrorNumbers = new List<int>(new[] { -2, -1, 2, 21, 53, 109, 121, 233, 49920, 4221, 10053, 10054, 10060, 10061, 10928, 10929, 11001, 17187, 17188, 17189, 17191, 17197, 17198, 17803, 17809, 17829, 17830, 17889, 18401, 18451 });
            }
            return connectionErrorNumbers;
        }
        public static int GetMaxIORetry()
        {

            int result = 5;
            var maxRetry = GetValueFromAppSettings("AppSettings:IOHelper.MaxRetry");
            if (StringValidate(maxRetry))
            {
                int.TryParse(maxRetry, out result);
            }
            return result;
        }
        public static List<int> GetIOErrorNumbers()
        {
            var errorNumber = GetValueFromAppSettings("AppSettings:IOHelper.ErrorNumbers");
            var errorNumbers = new List<int>();
            if (StringValidate(errorNumber))
            {
                errorNumbers = GetValueFromAppSettings("AppSettings:IOHelper.ErrorNumbers").Split(',').Select(Int32.Parse).ToList();
            }
            else
            {
                errorNumbers = new List<int>(new[] { 2, 3, 4, 8, 14, 15, 32, 33, 36, 53, 54, 55, 56, 57, 58, 59, 64, 65, 67, 69, 70, 71, 82, 84, 88, 89, 100, 101, 102, 103, 104, 105, 109, 110, 121, 148, 152, 155, 258 });
            }
            return errorNumbers;
        }
        public static List<string> GetIOTransientExceptions()
        {
            var transientWCFException = GetValueFromAppSettings("AppSettings:IOHelper.TransientExceptions");
            var transientWCFExceptions = new List<string>();
            if (StringValidate(transientWCFException))
            {
                transientWCFExceptions = Regex.Replace(GetValueFromAppSettings("AppSettings:IOHelper.TransientExceptions"), @"\s+", "").Replace(" ", "").Split(',').ToList();
            }
            else
            {
                transientWCFExceptions = new List<string>(new[] { "FileNotFoundException", "DirectoryNotFoundException", "DriveNotFoundException", "PathTooLongException",
                    "OperationCanceledException","UnauthorizedAccessException","ArgumentException","NotSupportedException","SecurityException" });
            }
            return transientWCFExceptions;
        }
        public static bool StringValidate(string value)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
            {
                result = true;
            }
            return result;
        }
        public static string GetConnectionString()
        {
            //var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ErrorCode")["ProductIntegrationId"];
            var config = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string connectionString = config["ConnectionStrings:ConnError"];
            return string.IsNullOrEmpty(connectionString) ? "" : connectionString;
        }
        public static string GetValueFromAppSettings(string key)
        {
            try
            {
                //var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ErrorCode")["ProductIntegrationId"];
                var config = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                return config[key];
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string EncryptEmail(string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(email);
                    var encryptString = System.Convert.ToBase64String(plainTextBytes);
                    return encryptString;
                }
                return "";
            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
