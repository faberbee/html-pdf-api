using Microsoft.Extensions.Logging;
using System.IO;
using iText.Html2pdf;
using System;
using HtmlPdfApi.Helpers.Signature;
using System.Text;
using HtmlPdfApi.Helpers.Exceptions;
using iText.Kernel.Pdf;

namespace HtmlPdfApi.Services
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HtmlPdfService
    {
        private readonly ILogger<HtmlPdfService> _logger;

        public HtmlPdfService(ILogger<HtmlPdfService> logger)
        {
            _logger = logger;
        }

        public virtual byte[] GetPdf(string template)
        {
            try
            {
                _logger.LogInformation($"Template compiled successfully");

                string OUTPUT_FOLDER = Path.Combine(Directory.GetCurrentDirectory(), "assets");
                string pdfDest = Path.Combine(OUTPUT_FOLDER, "output.pdf");

                // Create output folder
                if (Directory.Exists(OUTPUT_FOLDER) == false)
                    Directory.CreateDirectory(OUTPUT_FOLDER);

                // Init streams
                using var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(template));
                using var pdfStream = new MemoryStream();
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(pdfStream));

                // Set orientation if exists custom orientation tag
                if (template.Contains($"<meta name=\"orientation\" content=\"landscape\" />"))
                    pdfDocument.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate());

                // Create custom tag
                ConverterProperties converterProperties = new ConverterProperties()
                    .SetCssApplierFactory(new SignatureTagCssApplierFactory())
                    .SetTagWorkerFactory(new SignatureTagWorkerFactory());

                // Convert html to pdf and save to file
                HtmlConverter.ConvertToPdf(htmlStream, pdfDocument, converterProperties);

                _logger.LogInformation("Completed");
                return pdfStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiExceptions.PdfCreationException(ex.Message);
            }
        }
    }
}
