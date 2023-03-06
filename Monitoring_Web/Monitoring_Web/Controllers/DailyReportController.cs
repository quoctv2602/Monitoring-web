using Microsoft.AspNetCore.Mvc;
using Monitoring.Service.IService;
using Monitoring.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.IO;

namespace Monitoring_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyReportController : Controller
    {
        private readonly IMonitoringSystemService _monitoringSystemService;
        private readonly ILogger<DailyReportController> _logger;
        public DailyReportController(IMonitoringSystemService monitoringSystemService, ILogger<DailyReportController> logger)
        {
            this._monitoringSystemService = monitoringSystemService;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetMonitoringSystem")]
        public ActionResult<IList<MonitoringSystem>> GetMonitoringSystem()
        {
            _logger.LogInformation("---Begin GetMonitoringSystem method in DailyReportController---");
            try
            {
                var list = _monitoringSystemService.GetMonitoringSystems();
                DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(list), (typeof(DataTable)));
                var folderPath = System.IO.Directory.CreateDirectory("DailyReport/" + DateTime.Today.ToString("yyyyMMdd") + "/");
                var fileName = "Daily_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(table, "Monitoring System");
                    using (var fs = new FileStream(folderPath + fileName, FileMode.Create, FileAccess.Write))
                    {
                        wb.SaveAs(fs);

                    }
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            _logger.LogInformation("---End GetMonitoringSystem method in DailyReportController---");
        }
    }
}
