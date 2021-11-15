using HtmlPdfApi.Controllers;
using Xunit;
using Moq;
using System.Threading.Tasks;
using HtmlPdfApi.Models.Browser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using HtmlPdfApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Net.Http;
using System.Collections.Generic;
using HtmlPdfApi.Helpers.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HtmlPdfApi.Tests
{
    public class BrowserControllerTests
    {
        public Mock<ILogger<BrowserController>> logger = new Mock<ILogger<BrowserController>>();
        public Mock<IConfiguration> config = new Mock<IConfiguration>();
        public Mock<ResourceService> resourceService;
        public HandlebarsService handlebarsService;
        public HtmlPdfService htmlPdfService;

        public BrowserControllerTests()
        {
            resourceService = new Mock<ResourceService>(MockBehavior.Strict, new Mock<ILogger<ResourceService>>().Object, new Mock<IHttpClientFactory>().Object);
            handlebarsService = new HandlebarsService(new Mock<ILogger<HandlebarsService>>().Object);
            htmlPdfService = new HtmlPdfService(new Mock<ILogger<HtmlPdfService>>().Object);
        }

        [Fact]
        public async Task Print_ReturnsDocumentWithoutAuth_WhenEverythingOk()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"OIDC_AUTH_ENABLED", "false"},
                })
                .Build();

            resourceService
                .Setup(x => x.GetDocumentResourceAsync(It.IsAny<string>()))
                .ReturnsAsync("<html><body>Hello {{name}}!</body></html>");

            BrowserController controller = new BrowserController(logger.Object, configuration, resourceService.Object, handlebarsService, htmlPdfService);
            dynamic data = new ExpandoObject();
            data.name = "Alice";
            var result = await controller.Print(new BrowserDto() { resourceUrl = "http://www.faberbee.com/test.html", data = data });

            Assert.IsType<FileContentResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Print_ReturnsDocumentWithAuth_WhenEverythingOk()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"OIDC_AUTH_ENABLED", "true"},
                    {"REQUIRED_SCOPE", "test-scope"},
                })
                .Build();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("scope", "test-scope"),
            }, "mock"));

            resourceService
                .Setup(x => x.GetDocumentResourceAsync(It.IsAny<string>()))
                .ReturnsAsync("<html><body>Hello {{name}}!</body></html>");

            BrowserController controller = new BrowserController(logger.Object, configuration, resourceService.Object, handlebarsService, htmlPdfService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            dynamic data = new ExpandoObject();
            data.name = "Alice";
            var result = await controller.Print(new BrowserDto() { resourceUrl = "http://www.faberbee.com/test.html", data = data });

            Assert.IsType<FileContentResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Print_ThrowsUnauthorized_WhenUserHasNotTheScope()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"OIDC_AUTH_ENABLED", "true"},
                    {"REQUIRED_SCOPE", "test-scope"},
                })
                .Build();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("scope", "another-scope"),
            }, "mock"));

            BrowserController controller = new BrowserController(logger.Object, configuration, resourceService.Object, handlebarsService, htmlPdfService);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await Assert.ThrowsAsync<ApiExceptions.ForbiddenException>(async () => await controller.Print(new BrowserDto() { resourceUrl = "http://www.faberbee.com/test.html", data = new ExpandoObject() }));
        }
    }
}