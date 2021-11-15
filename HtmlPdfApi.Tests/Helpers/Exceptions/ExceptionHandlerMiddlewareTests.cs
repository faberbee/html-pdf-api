using System;
using System.Threading.Tasks;
using HtmlPdfApi.Helpers.Exceptions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace HtmlPdfApi.Tests.Helpers.Exceptions
{
    public class ExceptionHandlerMiddlewareTests
    {
        [Fact]
        public async Task Invoke_ReturnsDesiredResponse_WhenMatchesException()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => Task.FromException(new Exception("generic"));
            ExceptionHandlerMiddleware middleware = new ExceptionHandlerMiddleware(next);
            await middleware.Invoke(httpContext);
            var response = httpContext.Response;
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async Task OnExceptionAsync_Returns200Response_WhenDoesNotMatchException()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
            ExceptionHandlerMiddleware middleware = new ExceptionHandlerMiddleware(next);
            await middleware.Invoke(httpContext);
            var response = httpContext.Response;
            Assert.Equal(200, response.StatusCode);
        }
    }
}