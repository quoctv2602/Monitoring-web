using Monitoring_Common.Service;
using Monitoring_wsAddCounters.App_Code;
using System.Diagnostics;
using static Monitoring_Common.Service.WindowCounters;

namespace Monitoring_wsAddCounters
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //const int SW_HIDE = 0;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        private static PerformanceCounter avgCounter64_CPU;
        private static PerformanceCounter avgCounter64_Ram;
        private static PerformanceCounter avgCounter64_Disk;

        private static PerformanceCounter Disk_read_write;
        private static PerformanceCounter Disk_start;
        private static PerformanceCounter Disk_end;

        private static PerformanceCounter EDItoASCII;
        private static PerformanceCounter EDItoASCII_start;
        private static PerformanceCounter EDItoASCII_end;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);
            try
            {
                // run Administrator
                _logger.LogInformation(" ExecuteAsync : Administrator");               
                SetupCategory();
                CreateCounters(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(" IJob.Add : PerfCounterCategory ", e);
            }

            return Task.CompletedTask;
        }

        private void SetupCategory()
        {
            try
            {
                var addCounterList = ((GlobalSettings.EDItoASCIICheck() == "1" ? "Process Time EDItoASCII;" : "") + (GlobalSettings.ShareFolderCheck() == "1" ? "Shared Storage Running Time;" : "") + GlobalSettings.ProcessList()).Replace(",", ";").Split(';');
                if (addCounterList != null)
                {
                    foreach (var item in addCounterList)
                    {
                        string categoryName = "." + item.ToString();
                        try
                        {
                            if (item.ToString() == "Shared Storage Running Time")
                            {
                                if (!PerformanceCounterCategory.Exists(categoryName))
                                {
                                    CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_Timer", "Sample Counter elapsedTime", PerformanceCounterType.NumberOfItems64));
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_Start", "Sample Counter elapsedTime", PerformanceCounterType.NumberOfItems64));
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_End", "Sample Counter elapsedTime", PerformanceCounterType.NumberOfItems64));
                                    PerformanceCounterCategory.Create(categoryName, "Category Help", PerformanceCounterCategoryType.SingleInstance, counterCreationDataCollection);
                                }
                            }
                            else if (item.ToString() == "Process Time EDItoASCII")
                            {
                                if (!PerformanceCounterCategory.Exists(categoryName))
                                {
                                    CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_Timer", "Sample Counter elapsedTime", PerformanceCounterType.NumberOfItems64));
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_Start", "Sample Counter elapsedTime", PerformanceCounterType.NumberOfItems64));
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_End", "Sample Counter elapsedTime", PerformanceCounterType.NumberOfItems64));
                                    PerformanceCounterCategory.Create(categoryName, "Category Help", PerformanceCounterCategoryType.SingleInstance, counterCreationDataCollection);
                                }
                            }
                            else
                            {
                                if (!PerformanceCounterCategory.Exists(categoryName))
                                {
                                    CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_CPU", "Sample Counter CPU", PerformanceCounterType.NumberOfItems64));
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_Ram", "Sample Counter Ram", PerformanceCounterType.NumberOfItems64));
                                    counterCreationDataCollection.Add(new CounterCreationData("Counter_Disk", "Sample Counter Disk", PerformanceCounterType.NumberOfItems64));
                                    PerformanceCounterCategory.Create(categoryName, "Category Help", PerformanceCounterCategoryType.SingleInstance, counterCreationDataCollection);
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(" SetupCategory : PerfCounterCategory for addCounterList " + categoryName, e);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation(" SetupCategory : PerfCounterCategory No data list <ProcessList>");

                }
            }
            catch (Exception e)
            {
                _logger.LogError(" SetupCategory : PerfCounterCategory ", e);
            }
        }

        private async Task CreateCounters(CancellationToken stoppingToken)
        {
            try
            {
                var tasks = new List<Task>();
                if (GlobalSettings.EDItoASCIICheck() == "1"){
                    tasks.Add(KPI_ProcessTimeEDItoASCII(stoppingToken));
                }
                else
                {
                    try
                    {
                        string categoryName = "." + "Process Time EDItoASCII";
                        _logger.LogInformation(" Reset Counter : " + categoryName);

                        EDItoASCII = new PerformanceCounter(categoryName, "Counter_Timer", false);
                        EDItoASCII_start = new PerformanceCounter(categoryName, "Counter_Start", false);
                        EDItoASCII_end = new PerformanceCounter(categoryName, "Counter_End", false);

                        EDItoASCII.RawValue = 0;
                        EDItoASCII_start.RawValue = 0;
                        EDItoASCII_end.RawValue = 0;
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(" CreateCounters : PerfCounterCategory Reset Process Time EDItoASCII value 0: {0}", ex.Message);

                    }
                }

                if (GlobalSettings.ShareFolderCheck() == "1")
                {
                    tasks.Add(KPI_SharedStorageRunningTime(stoppingToken));
                }
                else
                {
                    try
                    {

                        string categoryName = "." + "Shared Storage Running Time";
                        _logger.LogInformation(" Reset Counter : " + categoryName);
                        Disk_read_write = new PerformanceCounter(categoryName, "Counter_Timer", false);
                        Disk_start = new PerformanceCounter(categoryName, "Counter_Start", false);
                        Disk_end = new PerformanceCounter(categoryName, "Counter_End", false);

                        Disk_read_write.RawValue = 0;
                        Disk_start.RawValue = 0;
                        Disk_end.RawValue = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(" CreateCounters : PerfCounterCategory Reset Shared Storage Running Time value 0: {0}", ex.Message);
                    }
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {

                _logger.LogError(" CreateCounters : PerfCounterCategory ", e);
            }
        }

        private async Task KPIListService(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation(" ExecuteAsync : KPIListService");
                await Task.Yield();
                var addCounterList = GlobalSettings.ProcessList().Replace(",", ";").Split(';');
                if (addCounterList != null)
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                       
                        foreach (var item in addCounterList)
                        {
                            string categoryName = "." + item.ToString();

                            try
                            {
                                avgCounter64_CPU = new PerformanceCounter(categoryName, "Counter_CPU", false);
                                avgCounter64_Ram = new PerformanceCounter(categoryName, "Counter_Ram", false);
                                avgCounter64_Disk = new PerformanceCounter(categoryName, "Counter_Disk", false);

                                var ramTask = WindowCounters.GetRAMCounterProcessAsync(Convert.ToString(item));
                                var cpuTask = WindowCounters.GetCpuCounterProcessAsync(Convert.ToString(item));
                                var diskTask = WindowCounters.GetDiskCounterProcessAsync(Convert.ToString(item));
                                Task.WhenAll(ramTask, cpuTask, diskTask);
                                var (Cpu, Ram, Disk) = (ramTask.Result, cpuTask.Result, diskTask.Result);

                                avgCounter64_Ram.IncrementBy(Ram);                                
                                avgCounter64_CPU.IncrementBy(Cpu);                                
                                avgCounter64_Disk.IncrementBy(Disk);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(" CreateCounters : PerfCounterCategory for addCounterList " + categoryName, e);
                            }
                        }                        
                        await Task.Delay(GlobalSettings.Schedule_Service()).ConfigureAwait(false);
                    }
                }

            }
            catch (Exception e)
            {

                _logger.LogError(" CreateCounters : PerfCounterCategory KPIListService ", e);
            }
        }
        private async Task KPI_ProcessTimeEDItoASCII(CancellationToken stoppingToken)
        {
            string categoryName = "." + "Process Time EDItoASCII";
            try
            {
                _logger.LogInformation(" ExecuteAsync : KPI_ProcessTimeEDItoASCII");
                await Task.Yield();
                while (!stoppingToken.IsCancellationRequested)
                {
                   
                    EDItoASCII = new PerformanceCounter(categoryName, "Counter_Timer", false);
                    EDItoASCII_start = new PerformanceCounter(categoryName, "Counter_Start", false);
                    EDItoASCII_end = new PerformanceCounter(categoryName, "Counter_End", false);
                    var EDItoASCIIConfigApp = GlobalSettings.EDItoASCIIConfigApp();
                    var EDItoASCIIConfigData = GlobalSettings.EDItoASCIIConfigData();
                    TransactionEDItoASCIIModel Value = CallAppAndWarningEDItoASCII(EDItoASCIIConfigApp, EDItoASCIIConfigData);



                    EDItoASCII.RawValue = 0;
                    EDItoASCII_start.RawValue = 0;
                    EDItoASCII_end.RawValue = 0;
                    EDItoASCII.IncrementBy(Value.miliseconds);
                    string Start = Value.start.ToString("yyyyMMddHHmmssfff");
                    string End = Value.end.ToString("yyyyMMddHHmmssfff");

                    EDItoASCII_start.IncrementBy(Convert.ToInt64(Start));
                    EDItoASCII_end.IncrementBy(Convert.ToInt64(End));

                    await Task.Delay(GlobalSettings.Schedule_Service()).ConfigureAwait(false);
                }



            }
            catch (Exception ex)
            {

                _logger.LogError(" CreateCounters : PerfCounterCategory for addCounterList " + categoryName, ex);
            }
        }
        private async Task KPI_SharedStorageRunningTime(CancellationToken stoppingToken)
        {
            _logger.LogInformation(" ExecuteAsync : KPI_SharedStorageRunningTime");
            string categoryName = "." + "Shared Storage Running Time";
            await Task.Yield();
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Disk_read_write = new PerformanceCounter(categoryName, "Counter_Timer", false);
                    Disk_start = new PerformanceCounter(categoryName, "Counter_Start", false);
                    Disk_end = new PerformanceCounter(categoryName, "Counter_End", false);
                    var sourceDir = GlobalSettings.ShareFolderFrom();
                    var destinationDir = GlobalSettings.ShareFolderTo();
                    TransferModel Value = AddTimeTransferFile(sourceDir, destinationDir);


                    Disk_read_write.RawValue = 0;
                    Disk_start.RawValue = 0;
                    Disk_end.RawValue = 0;

                    Disk_read_write.IncrementBy(Value.miliseconds);
                    string Start = Value.start.ToString("yyyyMMddHHmmssfff");
                    string End = Value.end.ToString("yyyyMMddHHmmssfff");
                    Disk_start.IncrementBy(Convert.ToInt64(Start));
                    Disk_end.IncrementBy(Convert.ToInt64(End));

                    await Task.Delay(GlobalSettings.Schedule_Service()).ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {

                _logger.LogError(" CreateCounters : PerfCounterCategory for addCounterList " + categoryName, ex);
            }
        }

    }
}