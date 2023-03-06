using Monitoring_wsGetHealth.DAILY;
using Monitoring_wsGetHealth.INTERVAL;
using System.Reflection;
using System.Runtime.InteropServices;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        // services.AddHostedService<WorkerDaily>();
    })
      .ConfigureLogging(builder =>
      {   // Configuration here:
          builder.SetMinimumLevel(LogLevel.Error);
          builder.AddLog4Net("log4net.config");
      }).UseWindowsService()
       .Build();
Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
await host.RunAsync();

