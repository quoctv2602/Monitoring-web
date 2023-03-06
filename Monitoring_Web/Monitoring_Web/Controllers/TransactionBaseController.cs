using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring.Service.Services;
using Monitoring_Common.Common;
using Monitoring_Web.HubConfig;
using Monitoring_Web.TimerFeatures;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Monitoring_Web.Filter;
using static Monitoring_Common.Enum;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using DiCentral.RetrySupport._6._0.ServiceHelper;

namespace Monitoring_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionBaseController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransactionBaseController> _logger;
        private readonly ITransactionBaseService _transactionBaseService;


        public TransactionBaseController(IConfiguration configuration, ILogger<TransactionBaseController> logger, ITransactionBaseService transactionBaseService)
        {
            _logger = logger;
            _configuration = configuration;
            _transactionBaseService = transactionBaseService;
        }

        [HttpPost]
        [Route("GetCIPReporting")]
        public async Task<IActionResult> GetCIPReporting([FromBody] CIPReportingModel Param, int EnvironmentID)
        {
            try
            {
                _logger.LogInformation("---Begin method GETCIPReporting in TransactionBaseController---");
                var listData = await _transactionBaseService.GetCIPReporting(Param, EnvironmentID);
                _logger.LogInformation("---End method GETCIPReporting in TransactionBaseController---");
                return Ok(new { Message = "Request Get CIP Reporting  Completed ", Data = listData });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetReportByTransactionKey")]
        public async Task<IActionResult> GetReportByTransactionKey([FromBody] ReportbyTransactionModel Param, int EnvironmentID)
        {
            try
            {

                var listData = _transactionBaseService.GetReportByTransactionKey(Param, EnvironmentID).Result;
                return Ok(new { Message = "Request Get Report By TransactionKey Completed", Data = listData });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId =(int)ActionEnum.transactionsViewLog)]
        [HttpPost]
        [Route("GetViewLogs")]
        public async Task<IActionResult> GetViewLogs([FromBody] GetViewLogsModel Param, int EnvironmentID)
        {
            try
            {

                var listData = _transactionBaseService.GetViewLogs(Param, EnvironmentID).Result;
                return Ok(new { Message = "Request GetViewLogs Completed", Data = listData });


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.transactionsViewDataContent)]
        [HttpPost]
        [Route("GetContentView")]
        public async Task<IActionResult> GetContentView([FromBody] GetContentViewModel Param, int EnvironmentID)
        {
            try
            {

                var listData = _transactionBaseService.GetContentView(Param, EnvironmentID).Result;


                
                return Ok(new { Message = "Request GetContentView Completed", Data = listData });


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.transactionsViewConfig)]
        [HttpPost]
        [Route("ViewCIPConfiguration")]
        public async Task<IActionResult> ViewCIPConfiguration([FromBody] CIPConfig Param, int EnvironmentID)
        {
            try
            {

                var listData = _transactionBaseService.ViewCIPConfiguration(Param, EnvironmentID).Result;
                return Ok(new { Message = "Request ViewCIPConfiguration Completed", Data = listData });


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.transactionsMonitoringAction)]
        [HttpPost]
        [Route("createTransDataIntegration")]
        public async Task<IActionResult> CreateTransDataIntegration([FromBody] List<TransDataIntegrationModel> param, int? MonitoredStatus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _transactionBaseService.CreateTransDataIntegration(param, MonitoredStatus);
                if (!result)
                {
                    return Ok(new ApiErrorResult<string>("Create Trans Data Integration error"));
                }
                return Ok(new ApiSuccessResult<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.transactionsMonitoringAction)]
        [HttpPost]
        [Route("exportFileExcel")]
        public async Task<ActionResult> ExportFileExcel(List<ListDataExportExcel> _data)
        {
            try
            {
                var fileName = "FileExcelExport_FromMornitoring" + "_" + DateTime.Now.ToString().Replace(":", "").Replace("/", "").Replace(" ", "") + ".xlsx";
                var stream = new MemoryStream();
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Transactions");
                    var namedStyle = xlPackage.Workbook.Styles.CreateNamedStyle("HyperLink");
                    namedStyle.Style.Font.UnderLine = true;
                    namedStyle.Style.Font.Color.SetColor(Color.Blue);
                    const int startRow = 5;
                    var row = startRow;

                    //Create Headers and format them
                    worksheet.Cells["A1"].Value = "Transactions";
                    using (var r = worksheet.Cells["A1:L1"])
                    {
                        r.Merge = true;
                        r.Style.Font.Color.SetColor(Color.White);
                        r.Style.Font.Size = 30;
                        r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                    }

                    worksheet.Cells["A4"].Value = "Note";
                    worksheet.Cells["B4"].Value = "Environment";
                    worksheet.Cells["C4"].Value = "Transaction Key";
                    //worksheet.Cells["D4"].Value = "Total of Docs";
                    worksheet.Cells["D4"].Value = "Doc Type";
                    worksheet.Cells["E4"].Value = "Document";
                    worksheet.Cells["F4"].Value = "Start Date";
                    worksheet.Cells["G4"].Value = "End Date";
                    worksheet.Cells["H4"].Value = "Sender";
                    worksheet.Cells["I4"].Value = "Receiver";
                    worksheet.Cells["J4"].Value = "Error Status";
                    worksheet.Cells["K4"].Value = "Re-processed";
                    worksheet.Cells["L4"].Value = "Monitored Status";
                    worksheet.Cells["A4:L4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A4:L4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    worksheet.Column(1).Width = 20;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 50;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 30;
                    worksheet.Column(7).Width = 25;
                    worksheet.Column(8).Width = 25;
                    worksheet.Column(9).Width = 25;
                    worksheet.Column(10).Width = 25;
                    worksheet.Column(11).Width = 30;
                    worksheet.Column(12).Width = 30;
                    row = 5;
                    foreach (var item in _data)
                    {
                        worksheet.Cells[row, 1].Value = item.Note;
                        worksheet.Cells[row, 2].Value = item.Environment;
                        worksheet.Cells[row, 3].Value = item.TransactionKey;
                        //worksheet.Cells[row, 4].Value = item.TotalOfDocs;
                        worksheet.Cells[row, 4].Value = item.DocType;
                        worksheet.Cells[row, 5].Value = item.Document;
                        worksheet.Cells[row, 6].Value = item.StartDate.ToString("MM/dd/yyyy HH:mm:ss");
                        worksheet.Cells[row, 7].Value = item.EndDate.ToString("MM/dd/yyyy HH:mm:ss");
                        worksheet.Cells[row, 8].Value = item.Sender;
                        worksheet.Cells[row, 9].Value = item.Receiver;
                        worksheet.Cells[row, 10].Value = item.ErrorStatus;
                        worksheet.Cells[row, 11].Value = item.ReProcessed;
                        worksheet.Cells[row, 12].Value = item.MonitoredStatus;
                        row++;
                    }

                    // set some core property values
                    xlPackage.Workbook.Properties.Title = "User List";
                    xlPackage.Workbook.Properties.Author = "Mohamad Lawand";
                    xlPackage.Workbook.Properties.Subject = "User List";
                    // save the new spreadsheet
                    xlPackage.Save();
                    // Response.Clear();
                }
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.transactionsViewDataContent)]
        [HttpPost]
        [Route("DownloadFileContent")]
        public async Task<IActionResult> DownloadFileContent([FromBody] GetFileContentViewModel Param, int EnvironmentID)
        {
            try
            {
                var listData = _transactionBaseService.DownloadFileContent(EnvironmentID).Result;
                string postbody = JsonConvert.SerializeObject(Param).ToString();
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(listData.URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", listData.token);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new System.Net.Http.StringContent(postbody, Encoding.UTF8, "application/json");
                    var response = await WebAPIHelper.Default.ExecuteAsync(() => client.PostAsync(listData.URL, data), CancellationToken.None);
                    var result = await response.Content.ReadAsStringAsync();
                    byte[] memoryStream = Encoding.ASCII.GetBytes(result);
                    return File(memoryStream, "application/octet-stream", listData.RequestID + ".zip");
                }
                //using (var stream = response.GetResponseStream())
                //{
                //    var memoryStream = new MemoryStream();
                //    await stream.CopyToAsync(memoryStream);

                //    return File(memoryStream.ToArray(), "application/octet-stream", "filename.ext");
                //}

                //var listData = _transactionBaseService.DownloadFileContent(EnvironmentID).Result;
                //return Ok(new { Message = "Request DownloadFileContent Completed", Data = listData });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
