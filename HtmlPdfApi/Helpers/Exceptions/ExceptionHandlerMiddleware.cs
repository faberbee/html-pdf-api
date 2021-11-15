using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace HtmlPdfApi.Helpers.Exceptions
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HttpResponse response = context.Response;
                if (response.HasStarted)
                    return;

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "application/json";
                await response.WriteAsync(JsonSerializer.Serialize(new
                {
                    type = "UnmanagedException",
                    subType = "",
                    message = ex?.Message ?? "Internal server error",
                }));
            }
        }
    }
}