using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring_Common.Service
{

    public class WindowCounters
    {
        public static async Task<int> GetCpuCounterAsync()
        {
            await Task.Yield();
            PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            int cpuCounter = (int)Math.Ceiling(theCPUCounter.NextValue());
            if (cpuCounter == 0)
            {
                await Task.Delay(500).ConfigureAwait(false);
                cpuCounter = (int)Math.Ceiling(theCPUCounter.NextValue());
            }
            return cpuCounter;
        }
        public static async Task<int> GetCpuCounterProcessAsync(string processName)
        {
            await Task.Yield();
            PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time", processName, true);
            int cpuCounter = (int)Math.Ceiling(theCPUCounter.NextValue() / 10);
            if (cpuCounter == 0)
            {
                await Task.Delay(500).ConfigureAwait(false);
                cpuCounter = (int)Math.Ceiling(theCPUCounter.NextValue() / 10);
            }
            return cpuCounter;
        }
        public static async Task<int> GetRAMCounterAsync()
        {
            await Task.Yield();
            PerformanceCounter ramCounter = new PerformanceCounter();
            ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            await Task.Delay(500).ConfigureAwait(false);
            dynamic value = (int)Math.Ceiling(ramCounter.NextValue());
            return value;
        }

        public static async Task<int> GetRAMCounterProcessAsync(string processName)
        {
            await Task.Yield();
            PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set - Private", processName, true);
            await Task.Delay(500).ConfigureAwait(false);
            dynamic value = (int)Math.Ceiling(ramCounter.NextValue() / 1024 / 1024);
            return value;
        }

        public static async Task<int> GetDiskCounterAsync()
        {
            await Task.Yield();
            PerformanceCounter diskCounter = new PerformanceCounter();
            diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            var firstValue = (int)Math.Ceiling(diskCounter.NextValue());
            await Task.Delay(500).ConfigureAwait(false);
            var secondValue = (int)Math.Ceiling(diskCounter.NextValue());
            return secondValue;
        }

        public static async Task<int> GetDiskReadAsync()
        {
            await Task.Yield();
            PerformanceCounter diskRead = new PerformanceCounter();
            diskRead = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
            var firstValue = (int)Math.Ceiling(diskRead.NextValue());
            await Task.Delay(500).ConfigureAwait(false);
            var secondValue = (int)Math.Ceiling(diskRead.NextValue());
            return secondValue;
        }

        public static async Task<int> GetDiskWriteAsync()
        {
            await Task.Yield();
            PerformanceCounter diskWrite = new PerformanceCounter();
            diskWrite = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
            var firstValue = (int)Math.Ceiling(diskWrite.NextValue());
            await Task.Delay(500).ConfigureAwait(false);
            var secondValue = (int)Math.Ceiling(diskWrite.NextValue());
            return secondValue;
        }

        public static async Task<int> GetDiskCounterProcessAsync(string processName)
        {
            await Task.Yield();
            PerformanceCounter theDiskCounter = new PerformanceCounter("Process", "% Processor Time", processName, true);
            await Task.Delay(500).ConfigureAwait(false);
            int diskCounter = (int)Math.Ceiling(theDiskCounter.NextValue());
            return diskCounter;
        }


        public class TransferModel
        {
            public int miliseconds { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public int Status { get; set; }
            public string Error { get; set; }
        }
        public class TransactionEDItoASCIIModel
        {
            public int miliseconds { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }

            public int Status { get; set; }
            public string Error { get; set; }
        }
        private static DateTime convertIntToDatetime(string DateTimeInt)
        {
            //2022110714089977
            int year = Convert.ToInt32(DateTimeInt.Substring(0, 4));
            int Month = Convert.ToInt32(DateTimeInt.Substring(4, 2));
            int Day = Convert.ToInt32(DateTimeInt.Substring(6, 2));
            int Hour = Convert.ToInt32(DateTimeInt.Substring(8, 2));
            int Minute = Convert.ToInt32(DateTimeInt.Substring(10, 2));
            int Second = Convert.ToInt32(DateTimeInt.Substring(12, 2));
            int MiliSecond = Convert.ToInt32(DateTimeInt.Substring(14,3));

            return new DateTime(year, Month, Day, Hour, Minute, Second, MiliSecond);
        }
        public static TransferModel GetTimeTransferFile()
        {
            TransferModel respose = new TransferModel();
            try
            {
                PerformanceCounter Counter_Timer = new PerformanceCounter();
                PerformanceCounter Counter_Start = new PerformanceCounter();
                PerformanceCounter Counter_End = new PerformanceCounter();


                Counter_Timer = new PerformanceCounter(".Shared Storage Running Time", "Counter_Timer");
                Counter_Start = new PerformanceCounter(".Shared Storage Running Time", "Counter_Start");
                Counter_End = new PerformanceCounter(".Shared Storage Running Time", "Counter_End");
                respose.Status = 1;

                respose.miliseconds = Convert.ToInt32(Counter_Timer.RawValue);
                int check_error = 0;
                
                if(Counter_Start.RawValue == 0)
                {
                    respose.start = DateTime.Now;
                    check_error++;
                     

                }
                else
                {
                    respose.start = convertIntToDatetime(Convert.ToString(Counter_Start.RawValue));

                }
                if (Counter_End.RawValue == 0)
                {
                    respose.end = DateTime.Now;
                    check_error++;
                }
                else
                {
                    respose.end = convertIntToDatetime(Convert.ToString(Counter_End.RawValue));

                }
                
                if(check_error > 0)
                {
                    respose.Error = "Shared Storage Running Time Nothing";
                }
                else
                {
                    respose.Error = null;

                }

            }
            catch (Exception ex)
            {
                respose.Status = 0;
                respose.miliseconds = 0;
                respose.start = DateTime.Now;
                respose.end = DateTime.Now;
                respose.Error = ex.Message;

            }
            
            return respose;
        }
        public static TransferModel AddTimeTransferFile(string sourceDir, string destinationDir)
        {

            TransferModel respose = new TransferModel();

            try
            {

                destinationDir = destinationDir + "/Monitoring";
                if (!Directory.Exists(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                Stopwatch stopWatch = new Stopwatch();
                respose.start = DateTime.Now;
                stopWatch.Start();

                var allFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
                foreach (string newPath in allFiles)
                {
                    File.Copy(newPath, newPath.Replace(sourceDir, destinationDir), true);
                }
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;



                respose.end = DateTime.Now;
                long realAppLiveTime = stopWatch.ElapsedMilliseconds;
                respose.miliseconds = Convert.ToInt32(realAppLiveTime);

                DirectoryInfo di = new DirectoryInfo(destinationDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                respose.Status = 1;
                return respose;
            }
            catch (Exception ex)
            {

                //  _logger.LogError(" SetupCategory : GetTimeTransferFile ", ex);
                respose.Status = 0;
                respose.Error = ex.Message;
                return respose;
            }
        }
        public static TransactionEDItoASCIIModel GetAppAndWarningEDItoASCII()
        {
            TransactionEDItoASCIIModel respose = new TransactionEDItoASCIIModel();
            try
            {
                PerformanceCounter Counter_Timer = new PerformanceCounter();
                PerformanceCounter Counter_Start = new PerformanceCounter();
                PerformanceCounter Counter_End = new PerformanceCounter();


                Counter_Timer = new PerformanceCounter(".Process Time EDItoASCII", "Counter_Timer");
                Counter_Start = new PerformanceCounter(".Process Time EDItoASCII", "Counter_Start");
                Counter_End = new PerformanceCounter(".Process Time EDItoASCII", "Counter_End");
                respose.Status = 1;
                respose.miliseconds = Convert.ToInt32(Counter_Timer.RawValue);
                int check_error = 0;

                if (Counter_Start.RawValue == 0)
                {
                    respose.start = DateTime.Now;
                    check_error++;
                }
                else
                {
                    respose.start = convertIntToDatetime(Convert.ToString(Counter_Start.RawValue));
                }
                if (Counter_End.RawValue == 0)
                {
                    respose.end = DateTime.Now;
                    check_error++;
                }
                else
                {
                    respose.end = convertIntToDatetime(Convert.ToString(Counter_End.RawValue));
                }
                 

                if (check_error > 0)
                {
                    respose.Error = "Process Time EDItoASCII Nothing";
                }
                else
                {
                    respose.Error = null;

                }

            }
            catch (Exception ex)
            {
                respose.Status = 0;
                respose.miliseconds = 0;
                respose.start = DateTime.Now;
                respose.end = DateTime.Now;
                respose.Error = ex.Message;

            }
            return respose;
        }
        public static TransactionEDItoASCIIModel CallAppAndWarningEDItoASCII(string EDItoASCIIConfigApp, string EDItoASCIIConfigData)
        {
            TransactionEDItoASCIIModel respose = new TransactionEDItoASCIIModel();

            respose.start = DateTime.Now;
            string AppProcess = EDItoASCIIConfigApp;
            string Argument = EDItoASCIIConfigData;

            List<Process> processList = new List<Process>();
            Stopwatch sw = new Stopwatch();
            string aResult = "";
            string aError = "";
            sw.Start();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = AppProcess;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.Arguments = Argument;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                using (Process exeProcess = Process.Start(startInfo))
                {
                    using (StreamReader aStreamReader = exeProcess.StandardOutput)
                    {
                        aResult = aStreamReader.ReadToEnd();
                    }
                    using (StreamReader aStreamReader = exeProcess.StandardError)
                    {
                        aError = aStreamReader.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {

                respose.Error = ex.Message;
                respose.Status = 0;
            }
            sw.Stop();
            respose.end = DateTime.Now;
            long realAppLiveTime = sw.ElapsedMilliseconds;
            respose.miliseconds = Convert.ToInt32(realAppLiveTime);
            respose.Status = 1;

            return respose;

        }



    }
}
