using Monitoring_HistoryArchive;

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

await host.RunAsync();
