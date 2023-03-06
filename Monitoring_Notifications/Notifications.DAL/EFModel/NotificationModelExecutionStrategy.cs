using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Notifications.DAL.EFModel
{
    public class NotificationModelExecutionStrategy : ExecutionStrategy
    {
        private readonly ILogger _logger;

        public NotificationModelExecutionStrategy(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay, ILogger logger)
            : base(dependencies, maxRetryCount, maxRetryDelay )
        {
            _logger = logger;
        }
        public NotificationModelExecutionStrategy(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay, ILogger logger)
            : base(context, maxRetryCount, maxRetryDelay)
        {
            _logger = logger;
        }

        protected override bool ShouldRetryOn(Exception ex)
        {
            //bool isRetry = false;
            //isRetry = (exception.GetType() == typeof(InvalidOperationException));
            ////... add more
            _logger.LogError(ex.Message + " - Stacktrace: " + ex.StackTrace);
            return true;
        }
    }
}
