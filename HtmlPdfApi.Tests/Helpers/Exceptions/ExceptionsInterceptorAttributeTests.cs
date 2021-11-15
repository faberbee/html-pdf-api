using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HtmlPdfApi.Helpers.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace HtmlPdfApi.Tests.Helpers.Exceptions
{
    public class ExceptionsInterceptorAttributeTests
    {
        private ExceptionsInterceptorAttribute interceptorAttribute;
        public ExceptionsInterceptorAttributeTests()
        {
            interceptorAttribute = new ExceptionsInterceptorAttribute(typeof(ApiExceptions.ResourceException), HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task OnExceptionAsync_ReturnsDesiredResponse_WhenMatchesException()
        {
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            exceptionContext.Exception = new ApiExceptions.ResourceException("test");

            await interceptorAttribute.OnExceptionAsync(exceptionContext);
            var response = exceptionContext.HttpContext.Response;
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task OnExceptionAsync_Returns200Response_WhenDoesNotMatchException()
        {
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            exceptionContext.Exception = new ApiExceptions.TemplateCompilationException("test");

            await interceptorAttribute.OnExceptionAsync(exceptionContext);
            var response = exceptionContext.HttpContext.Response;
            Assert.Equal(200, response.StatusCode);
        }
    }
}