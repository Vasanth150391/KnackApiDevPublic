using Knack.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace Knack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportBuilder _reportBuilder;

        public ReportController(IReportBuilder reportBuilder)
        {
            _reportBuilder = reportBuilder;
        }
        [HttpGet]
        [Route("GlobalReadinessReport")]
        public async Task<IActionResult> DownloadGlobalReadinessReportFile()
        {
            var encoding = Encoding.UTF8;
           
            var resultString = await _reportBuilder.GetTextReport();

            var stream = new MemoryStream(encoding.GetBytes(resultString));


            MemoryStream memoryStream = new MemoryStream();
            TextWriter textWriter = new StreamWriter(memoryStream);
            textWriter.WriteLine(resultString);
            textWriter.Flush();
            byte[] bytesInStream = memoryStream.ToArray();
            memoryStream.Close();

            return File(bytesInStream, "application/txt", "globalreadinessreport.txt");
        }
    }
}
