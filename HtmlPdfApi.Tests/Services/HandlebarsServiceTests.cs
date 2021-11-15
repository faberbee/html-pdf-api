using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using HtmlPdfApi.Services;
using HtmlPdfApi.Helpers.Exceptions;

namespace HtmlPdfApi.Tests.Services
{
    public class HandlebarsServiceTests
    {
        public Mock<ILogger<HandlebarsService>> logger = new Mock<ILogger<HandlebarsService>>();

        [Fact]
        public void Compile_ReturnsCompiledHtml_WhenEverythingOk()
        {
            HandlebarsService service = new HandlebarsService(logger.Object);
            string response = service.Compile("Hello {{name}}!", new { name = "Alice" });
            Assert.Equal("Hello Alice!", response);
        }

        [Fact]
        public void Compile_ThrowException_WhenDataIsWrong()
        {
            HandlebarsService service = new HandlebarsService(logger.Object);
            Assert.Throws<ApiExceptions.TemplateCompilationException>(() => service.Compile(null, null));
        }
    }
}
