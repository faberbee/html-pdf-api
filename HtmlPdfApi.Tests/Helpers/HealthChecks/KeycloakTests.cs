using Xunit;
using Moq;
using System.Net.Http;
using HtmlPdfApi.Helpers.HealthChecks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Moq.Protected;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HtmlPdfApi.Tests.Helpers.HealthChecks
{
    public class KeycloakTests
    {
        public IConfiguration config;
        public Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();

        public KeycloakTests()
        {
            config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"OIDC_IDP_BASE_URL", "http://www.keycloak.it/"},
                })
                .Build();
        }

        [Fact]
        public async Task CheckHealthAsync_ReturnsHealthy_WhenStatusCodeIsSuccess()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                });
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockMessageHandler.Object));
            KeycloakHealthCheck service = new KeycloakHealthCheck(config, clientFactory.Object);
            HealthCheckResult response = await service.CheckHealthAsync(new HealthCheckContext());
            Assert.Equal(HealthStatus.Healthy, response.Status);
        }

        [Fact]
        public async Task Compile_ReturnsUnhealthy_WhenDataIsWrong()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadGateway,
                });
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(mockMessageHandler.Object));
            KeycloakHealthCheck service = new KeycloakHealthCheck(config, clientFactory.Object);
            HealthCheckResult response = await service.CheckHealthAsync(new HealthCheckContext());
            Assert.Equal(HealthStatus.Unhealthy, response.Status);
        }
    }
}
