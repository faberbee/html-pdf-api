using HtmlPdfApi.Helpers.Exceptions;
using HtmlPdfApi.Models.Browser;
using HtmlPdfApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HtmlPdfApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/browsers")]
    public class BrowserController : ControllerBase
    {
        private readonly ILogger<BrowserController> _logger;
        private readonly IConfiguration _config;
        private readonly ResourceService _resourceService;
        private readonly HandlebarsService _handlebarsService;
        private readonly HtmlPdfService _htmlPdfService;

        public BrowserController(ILogger<BrowserController> logger, IConfiguration config, ResourceService resourceService, HandlebarsService handlebarsService, HtmlPdfService htmlPdfService)
        {
            _logger = logger;
            _config = config;
            _resourceService = resourceService;
            _handlebarsService = handlebarsService;
            _htmlPdfService = htmlPdfService;
        }

        [HttpPost("print")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ExceptionsInterceptor(typeof(ApiExceptions.ForbiddenException), HttpStatusCode.Forbidden)]
        [ExceptionsInterceptor(typeof(ApiExceptions.ResourceException), HttpStatusCode.InternalServerError)]
        [ExceptionsInterceptor(typeof(ApiExceptions.ResourceNotFoundException), HttpStatusCode.NotFound)]
        [ExceptionsInterceptor(typeof(ApiExceptions.TemplateCompilationException), HttpStatusCode.InternalServerError)]
        [ExceptionsInterceptor(typeof(ApiExceptions.PdfCreationException), HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Print([FromBody] BrowserDto body)
        {
            _logger.LogInformation($"Try to print resource {body.resourceUrl} ... ");

            // Validate scopes
            if (_config.GetValue<bool>("OIDC_AUTH_ENABLED"))
            {
                string[] scopes = HttpContext.User.FindFirst("scope").Value.Split(" ");
                if (!scopes.Contains(_config.GetValue<string>("REQUIRED_SCOPE")))
                    throw new ApiExceptions.ForbiddenException();
            }

            // Get template
            string rawTemplate = await _resourceService.GetDocumentResourceAsync(body.resourceUrl);
            if (String.IsNullOrEmpty(rawTemplate))
                throw new ApiExceptions.ResourceException(body.resourceUrl);

            // Add static resources url
            Uri uri = new Uri(body.resourceUrl);
            string staticPath = uri.AbsoluteUri.Replace("/" + uri.Segments[uri.Segments.Count() - 1], "");
            IDictionary<string, object> data = body.data;
            data.Add("STATIC_PATH", staticPath);

            // Compile template with data            
            string compiledTemplate = _handlebarsService.Compile(rawTemplate, data);
            if (String.IsNullOrEmpty(compiledTemplate))
                throw new ApiExceptions.TemplateCompilationException();

            // Generate PDF
            var pdf = _htmlPdfService.GetPdf(compiledTemplate);
            if (pdf == null)
                throw new ApiExceptions.PdfCreationException();

            // Retrun stream
            return File(pdf, "application/pdf", "content.pdf");
        }
    }
}
