using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HtmlPdfApi.Helpers.Exceptions
{
    public class ExceptionsInterceptorAttribute : ExceptionFilterAttribute, IAsyncExceptionFilter
    {
        private Type exceptionType { get; set; }
        private HttpStatusCode statusCode { get; set; }

        public ExceptionsInterceptorAttribute(Type exceptionType, HttpStatusCode statusCode)
        {
            this.exceptionType = exceptionType;
            this.statusCode = statusCode;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception.GetType() == exceptionType)
            {
                HttpResponse response = context.HttpContext.Response;
                if (response.HasStarted)
                    return;

                response.StatusCode = (int)this.statusCode;
                response.ContentType = "application/json";
                await response.WriteAsync(JsonSerializer.Serialize(new
                {
                    type = ((dynamic)context.Exception).type,
                    subType = ((dynamic)context.Exception).subType,
                    message = ((dynamic)context.Exception).message,
                }));
            }
        }
    }
}