using HealthMeasurement.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Monitoring_Common.Service;
using static Monitoring_Common.Service.WindowCounters;
using System.Net.NetworkInformation;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HealthMeasurement.Api.Service
{
    public class HealthMeasurementService : IHealthMeasurementService
    {
        public MonitoringSystem GetDataMonitor()
        {
            try
            {
                var cpuCounterTask = GetCpuCounterAsync();
                var ramCounterTask = GetRAMCounterAsync();
                var diskStorageTask = GetDiskCounterAsync();
                Task.WhenAll(cpuCounterTask, ramCounterTask, diskStorageTask);
                var (CpuCounter, RamCounter, DiskStorage) = (cpuCounterTask.Result, ramCounterTask.Result, diskStorageTask.Result);

                string hostName = Dns.GetHostName();
                MonitoringSystem monitoringSystem = new MonitoringSystem();
                monitoringSystem.MarchineName = hostName;
                monitoringSystem.IpAddress = GetCurrentIpV4(hostName);
                monitoringSystem.CPUInfo = CpuCounter;
                monitoringSystem.MemoryInfo = RamCounter;
                monitoringSystem.StorageInfo = DiskStorage;
               
                return monitoringSystem;
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        public static string GetCurrentIpV4(string hostName)
        {
            string IPv4 = "";
            try
            {
             

                // Find host by name
                IPHostEntry iphostentry = Dns.GetHostByName(hostName);

                // Enumerate IP addresses
                Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                MatchCollection result = null;
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    result = ip.Matches(Convert.ToString(ipaddress));
                    if (result.Count() > 0)
                    {
                        IPv4 = result[0].Value;
                        
                        return IPv4;
                    }
                }
            }
            catch (Exception ex)
            {
                return IPv4;
            }
            return IPv4;
             


        }
        private string GetStatusService(string processName)
        {
            try
            {
                ServiceController sc = new ServiceController(processName);

                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.StopPending:
                        return "Stopping";
                    case ServiceControllerStatus.StartPending:
                        return "Starting";
                    default:
                        return "Status Changing";
                }
            }
            catch (Exception)
            {

                return "";
               
                 
            }
        }
        public List<MonitoringDetail> GetDetailDataMonitor(List<ProcessModel> ListProcess)
        {
            try 
            {
                

                List<MonitoringDetail> result = new List<MonitoringDetail>();

                string hostName = Dns.GetHostName();
                string IpAddress = GetCurrentIpV4(hostName);
                if (ListProcess != null)
                {
                    foreach (var item in ListProcess)
                    {
                        MonitoringDetail itemData = new MonitoringDetail();
                     //   itemData.MarchineName = hostName;
                     //   itemData.IpAddress = IpAddress;
                        itemData.ProcessName = item.Name.ToString();
                        try
                        {

                            #region remove - dont use function
                            //try
                            //{
                            //   itemData.CPUInfo = WindowCounters.getCPUCounterProcess(item.Name.ToString());
                            //}
                            //catch (Exception)
                            //{
                            //    itemData.CPUInfo = 0;
                            //}
                            //try
                            //{
                            //    itemData.MemoryInfo = WindowCounters.getRAMCounterProcess(item.Name.ToString());
                            //}
                            //catch (Exception)
                            //{
                            //    itemData.MemoryInfo = 0;
                            //}
                            //try
                            //{
                            //    itemData.StorageInfo = WindowCounters.getDiskCounterProcess(item.Name.ToString());
                            //}
                            //catch (Exception)
                            //{
                            //    itemData.StorageInfo = 0;
                            //} 
                            #endregion

                            //itemData.CPUInfo = 0;
                            //itemData.MemoryInfo = 0;
                            //itemData.StorageInfo = 0;


                            itemData.Status = GetStatusService(Convert.ToString(item.Name));
                            Process[] theProcesses = Process.GetProcessesByName(Convert.ToString(item.Name));
                            int LengthofProcesses = theProcesses.Length;

                           // itemData.CountInstance = LengthofProcesses;
                            if (itemData.Status == "")
                            {
                                itemData.Status = LengthofProcesses > 0 ? "Running" : "Nothing";
                            }
                        }
                        catch (Exception ex)
                        {

                            //itemData.CPUInfo = 0;
                            //itemData.MemoryInfo = 0;
                            //itemData.StorageInfo = 0;
                            //itemData.CountInstance = 0;
                            itemData.Status = "Nothing";
                        }

                        result.Add(itemData);
                    }
                }



 

               


                return result;
            }
            catch (Exception ex)
            {
                return new List<MonitoringDetail> { null };
            }
           
            
        }


        public TransferModel GetTransfer()
        {
            try
            {
               
                TransferModel Timer = new TransferModel();
                Timer = WindowCounters.GetTimeTransferFile();

                return Timer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TransactionEDItoASCIIModel GetAppAndWarningEDItoASCII()
        {
            try
            {

                TransactionEDItoASCIIModel Timer = new TransactionEDItoASCIIModel();
                Timer = WindowCounters.GetAppAndWarningEDItoASCII();

                return Timer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
