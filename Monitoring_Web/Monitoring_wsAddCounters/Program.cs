using Monitoring_wsAddCounters;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
      .ConfigureLogging(builder =>
      {   // Configuration here:
          builder.SetMinimumLevel(LogLevel.Trace);
          builder.AddLog4Net("log4net.config");
      }).UseWindowsService()
       .Build();
Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
await host.RunAsync();
