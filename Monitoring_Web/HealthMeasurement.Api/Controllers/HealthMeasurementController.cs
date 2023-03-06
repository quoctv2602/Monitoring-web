using HealthMeasurement.Api.Constants;
using HealthMeasurement.Api.Models;
using HealthMeasurement.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Security.Cryptography.Xml;
using static Monitoring_Common.Service.WindowCounters;

namespace HealthMeasurement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthMeasurementController : ControllerBase
    {
        private readonly IHealthMeasurementService _healthMeasurementService;
        private readonly IConfiguration _configuration;
        public HealthMeasurementController(IHealthMeasurementService healthMeasurementService, IConfiguration configuration)
        {
            _healthMeasurementService = healthMeasurementService;
            _configuration = configuration;
        }


        [HttpPost("getMonitor")]
        [SecretKey]
        public IActionResult GetDataMonitor(string UUID, string ServiceList)
        {

            MonitoringSystem resultComputer = _healthMeasurementService.GetDataMonitor();


           // _configuration.GetSection("MyConfig")["ProcessList"].Replace(",", ";"); 
            List<MonitoringDetail> result;
            if (!string.IsNullOrEmpty(ServiceList))
            {
                string itemList = ServiceList.Replace(",", ";");
                List<ProcessModel> subSettings = new List<ProcessModel>();
                foreach (var item in itemList.Split(';'))
                {
                    var i = new ProcessModel();
                    i.Name = item;
                    subSettings.Add(i);
                }

                 result = _healthMeasurementService.GetDetailDataMonitor(subSettings);
            }
            else
            {
                result = null;
            }
           

            List<DiskModel> drive = new List<DiskModel>();
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.IsReady == true)
                {
                    var i = new DiskModel();
                    i.DriveName = d.Name;
                    i.VolumeLabel = d.VolumeLabel;
                    i.TotalSize = d.TotalSize;
                    i.TotalFreeSpace = d.TotalFreeSpace;
                    drive.Add(i);
                }
            }

            TransferModel timer = new TransferModel();

            timer.Error = "Nothing";
             
            
            timer.start = DateTime.Now;
            timer.end = DateTime.Now;
            timer = _healthMeasurementService.GetTransfer();
               
           

            TransactionEDItoASCIIModel EDItoASCIIResponse = new TransactionEDItoASCIIModel();
            string EDItoASCIICheck = _configuration.GetSection("MyConfig")["EDItoASCII"];
            EDItoASCIIResponse.Error = "Nothing";
            EDItoASCIIResponse.start = DateTime.Now;
            EDItoASCIIResponse.end = DateTime.Now;
       
            EDItoASCIIResponse = _healthMeasurementService.GetAppAndWarningEDItoASCII();
           


            MonitoringRespone respone = new MonitoringRespone();
            respone.RequestID = UUID;
            respone.result = resultComputer;
            respone.detail = result;
            respone.disk = drive;
            respone.Transfer = timer;
            respone.EDItoASCII = EDItoASCIIResponse;

            if (respone == null)
            {
                return BadRequest(respone);
            }
            return Ok(respone);
        }

        #region not use function
        //[HttpPost("getListProcess")]
        //[SecretKey]
        //public IActionResult getListProcess()
        //{

        //    string itemList = _configuration.GetSection("MyConfig")["ProcessList"].Replace(",", ";");

        //    List<ProcessModel> subSettings = new List<ProcessModel>();
        //    foreach (var item in itemList.Split(';'))
        //    {
        //        var i = new ProcessModel();
        //        i.Name = item;
        //        subSettings.Add(i);
        //    }



        //    if (subSettings == null)
        //    {
        //        return BadRequest(subSettings);
        //    }
        //    return Ok(subSettings);
        //} 
        #endregion

        [HttpPost("getListDriveInfo")]
        [SecretKey]
        public IActionResult getListDriveInfo()
        {
            List<DiskModel> drive = new List<DiskModel>();
            try
            {

                foreach (DriveInfo d in DriveInfo.GetDrives())
                {
                    if (d.IsReady == true)
                    {
                        var i = new DiskModel();
                        i.DriveName = d.Name;
                        i.VolumeLabel = d.VolumeLabel;
                        i.TotalSize = d.TotalSize;
                        i.TotalFreeSpace = d.TotalFreeSpace;
                        drive.Add(i);
                    }

                }


                if (drive == null)
                {
                    return BadRequest(drive);
                }
                return Ok(drive);
            }
            catch (Exception ex)
            {
                var i = new DiskModel();
                i.error = Convert.ToString(ex);
                drive.Add(i);
                if (drive == null)
                {
                    return BadRequest(drive);
                }
                return Ok(drive);

            }
        }



    }
}
