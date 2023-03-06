using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.DAL.EFModel;
using Notifications.DAL.Repository;

namespace Notifications.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReposioryConnector(this IServiceCollection services)
        {
            services.AddTransient<INotificationDataFacade, NotificationDataFacade>();
            return services;
        }
    }
}
