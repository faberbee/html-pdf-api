using System;
using System.Net.Http;
using System.Net.Security;
using HtmlPdfApi.Helpers.Http;
using Xunit;

namespace HtmlPdfApi.Tests.Helpers.Http
{
    public class HttpClientCertificateHandlerTests
    {
        [Fact]
        public void Creation_ReturnsInstanceWithCustomCallback_WhenValidationDisabled()
        {
            HttpClientCertificateHandler handler = new HttpClientCertificateHandler(false);
            Assert.False(handler.ValidationEnabled);
            bool result = handler.ServerCertificateCustomValidationCallback(null, null, null, SslPolicyErrors.None);
            Assert.True(result);
        }

        [Fact]
        public void Creation_ReturnsInstanceWithoutCustomCallback_WhenValidationEnabled()
        {
            HttpClientCertificateHandler handler = new HttpClientCertificateHandler(true);
            Assert.True(handler.ValidationEnabled);
            Assert.ThrowsAny<Exception>(() => handler.ServerCertificateCustomValidationCallback(new HttpRequestMessage(HttpMethod.Get, "https://www.faberbee.com"), null, null, SslPolicyErrors.None));
        }
    }
}