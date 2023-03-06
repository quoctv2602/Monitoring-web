using Monitoring_Common.DataContext;
using Monitoring_wsGetHealth.App_Code;
using System.Data;

namespace Monitoring_wsGetHealth.Repository
{
    public class Monitoring_EnvironmentConfigRepository
    {
        public static DataTable Monitoring_EnvironmentConfig()
        {
            return SqlHelper.ExecuteQuery(GlobalSettings.ConnectionStrings(), " SELECT * FROM Sys_Integration_API AS sia  WITH (NOLOCK) WHERE sia.IsActive = 1 AND sia.NodeType = 2").Tables[0];
        }

        public static DataTable Monitoring_EnvironmentConfig_DiConnect()
        {
            return SqlHelper.ExecuteQuery(GlobalSettings.ConnectionStrings(), " SELECT * FROM Sys_Integration_API AS sia  WITH (NOLOCK) WHERE sia.IsActive = 1 AND sia.NodeType = 1").Tables[0];
        }
    }
}
