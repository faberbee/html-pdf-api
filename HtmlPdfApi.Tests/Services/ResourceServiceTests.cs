using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using HtmlPdfApi.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Moq.Protected;
using HtmlPdfApi.Helpers.Exceptions;

namespace HtmlPdfApi.Tests.Services
{
    public class ResourceServiceTests
    {
        public Mock<ILogger<ResourceService>> logger = new Mock<ILogger<ResourceService>>();
        public Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();

        [Fact]
        public async Task Compile_ReturnsCompiledHtml_WhenEverythingOk()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test")
                });
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockMessageHandler.Object));
            ResourceService service = new ResourceService(logger.Object, clientFactory.Object);
            string response = await service.GetDocumentResourceAsync("http://www.faberbee.com/test.html");
            Assert.Equal("test", response);
        }

        [Fact]
        public async Task Compile_ThrowResourceNotFoundException_WhenResponseIs404()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockMessageHandler.Object));
            ResourceService service = new ResourceService(logger.Object, clientFactory.Object);
            await Assert.ThrowsAsync<ApiExceptions.ResourceNotFoundException>(async () => await service.GetDocumentResourceAsync("http://www.faberbee.com/test.html"));
        }

        [Fact]
        public async Task Compile_ThrowResourceException_WhenResponseIsBad()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadGateway
                });
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockMessageHandler.Object));
            ResourceService service = new ResourceService(logger.Object, clientFactory.Object);
            await Assert.ThrowsAsync<ApiExceptions.ResourceException>(async () => await service.GetDocumentResourceAsync("http://www.faberbee.com/test.html"));
        }
    }
}
