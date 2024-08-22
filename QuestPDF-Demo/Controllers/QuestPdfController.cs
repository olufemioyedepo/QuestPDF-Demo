using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF_Demo.Services;

namespace QuestPDF_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestPdfController : ControllerBase
    {
        [HttpGet]
        [Route("sample1")]
        public async Task<ActionResult> GenerateSample1Pdf()
        {
            var pdfReportInfo = await QuestPdfService.GenerateSample1Pdf();
            return File(pdfReportInfo.ByteArray, pdfReportInfo.MimeType, pdfReportInfo.FileName);
        }

        [HttpGet]
        [Route("generate-pharmacy-report")]
        public ActionResult GeneratePharmacyReport()
        {
            var byteArray = QuestPdfService.GeneratePharmacyReportBytes();
            return File(byteArray, "application/pdf", "PharmacyReport.pdf");
        }
    }
}
