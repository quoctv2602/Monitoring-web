using log4net;
using Monitoring_Notifications.Jobs;
using Notifications.Options;
using Quartz;
using System.Reflection;
using Microsoft.Extensions.Logging.Log4Net.AspNetCore;
using Microsoft.Extensions.Hosting;
using Notifications.EmailService;
using Notifications.DAL;

namespace Monitoring_Notifications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.Sources.Clear();
                    configurationBuilder
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                        .AddXmlFile("appsettings.xml", optional: true, reloadOnChange: false);
                })
                .ConfigureServices((hostingBuilderContext, services) =>
                {
                    IConfiguration configuration = hostingBuilderContext.Configuration;
                    Configs configs = Configuration.Set(configuration);

                    services.AddServiceConnector();
                    services.AddReposioryConnector();

                    services.AddSingleton(configs);
                    //services.AddHostedService<Worker>();

                    services.Configure<QuartzOptions>(options =>
                    {
                        options.Scheduling.IgnoreDuplicates = true; // default: false
                        options.Scheduling.OverWriteExistingData = true; // default: true
                    });
                    services.AddQuartz(q =>
                    {
                        q.SchedulerId = "Notification Scheduler";
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        // these are the defaults
                        q.UseSimpleTypeLoader();
                        q.UseInMemoryStore();
                        q.UseDefaultThreadPool(tp =>
                        {
                            tp.MaxConcurrency = configs.AppSettings.NumThreadsForJob;
                        });

                        var notificationJobKey = new JobKey("Notification Job", "Notification Group");
                        q.AddJob<NotificationJob>(j => j
                            .StoreDurably()
                            .WithIdentity(notificationJobKey)
                            .WithDescription("Notification Job")
                        );
                        q.AddTrigger(t => t
                            .WithIdentity("Notification Trigger")
                            .ForJob(notificationJobKey)
                            .StartNow()
                            .WithCronSchedule(configs.AppSettings.NotificationJobCron)
                            .WithDescription("Notification Trigger")
                        );

                        var summaryReportJobKey = new JobKey("Summary report Job", "Summary report Group");
                        q.AddJob<SummaryReportJob>(j => j
                            .StoreDurably()
                            .WithIdentity(summaryReportJobKey)
                            .WithDescription("Daily report Job")
                        );
                        q.AddTrigger(t => t
                            .WithIdentity("Summary report Trigger")
                            .ForJob(summaryReportJobKey)
                            .StartNow()
                            .WithCronSchedule(configs.AppSettings.SummaryReportJobCron)
                            .WithDescription("Summary report Trigger")
                        );

                        var ArchiveDataJobKey = new JobKey("Archive data Job", "Archive data Group");
                        q.AddJob<HistoryArchive>(j => j
                            .StoreDurably()
                            .WithIdentity(ArchiveDataJobKey)
                            .WithDescription("Archive data Job")
                        );
                        q.AddTrigger(t => t
                            .WithIdentity("Archive data Trigger")
                            .ForJob(ArchiveDataJobKey)
                            .StartNow()
                            .WithCronSchedule(configs.AppSettings.ArchiveDataJobCron)
                            .WithDescription("Archive data Trigger")
                        );

                    });
                    services.AddQuartzHostedService(options =>
                    {
                        // when shutting down we want jobs to complete gracefully
                        options.WaitForJobsToComplete = true;

                        // when we need to init another IHostedServices first
                        options.StartDelay = TimeSpan.FromSeconds(10);
                    });
                })
                .ConfigureLogging((logging) =>
                {
                    logging.ClearProviders();
                    logging.AddLog4Net("log4net.config");
                })
                .UseWindowsService();
    }
}