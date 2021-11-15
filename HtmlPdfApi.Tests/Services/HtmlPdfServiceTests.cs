using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using HtmlPdfApi.Services;
using HtmlPdfApi.Helpers.Exceptions;
using System.Text;

namespace HtmlPdfApi.Tests.Services
{
    public class HtmlPdfServiceTests
    {
        public Mock<ILogger<HtmlPdfService>> logger = new Mock<ILogger<HtmlPdfService>>();

        [Fact]
        public void Compile_ReturnsCompiledHtmlWithoutSignature_WhenEverythingOk()
        {
            HtmlPdfService service = new HtmlPdfService(logger.Object);
            byte[] response = service.GetPdf("<html><body>test</body></html>");
            Assert.NotNull(response);
            string utfString = Encoding.UTF8.GetString(response, 0, response.Length);
            Assert.DoesNotContain("/Sig/", utfString);
            Assert.DoesNotContain("/AcroForm", utfString);
        }

        [Fact]
        public void Compile_ReturnsCompiledHtmlWithSignature_WhenEverythingOk()
        {
            HtmlPdfService service = new HtmlPdfService(logger.Object);
            byte[] response = service.GetPdf("<html><body>Sign:<signature name=\"test\" reason=\"test\">_____</signature></body></html>");
            Assert.NotNull(response);
            string utfString = Encoding.UTF8.GetString(response, 0, response.Length);
            Assert.Contains("/Sig/", utfString);
            Assert.Contains("/AcroForm", utfString);
        }

        [Fact]
        public void Compile_ThrowException_WhenDataIsWrong()
        {
            HtmlPdfService service = new HtmlPdfService(logger.Object);
            Assert.Throws<ApiExceptions.PdfCreationException>(() => service.GetPdf(null));
        }
    }
}
