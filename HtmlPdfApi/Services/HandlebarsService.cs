using System;
using HandlebarsDotNet;
using HtmlPdfApi.Helpers.Exceptions;
using HtmlPdfApi.Helpers.Handlebars;
using Microsoft.Extensions.Logging;

namespace HtmlPdfApi.Services
{
    public class HandlebarsService
    {
        private readonly ILogger<HandlebarsService> _logger;

        public HandlebarsService(ILogger<HandlebarsService> logger)
        {
            _logger = logger;
            HandlebarsHelpers.RegisterHelper_IfCond();
        }

        public virtual string Compile(string rawTemplate, dynamic data)
        {
            try
            {
                _logger.LogInformation($"Try to compile template with custom data ... ");

                HandlebarsTemplate<object, object> template = Handlebars.Compile(rawTemplate);
                string compiledTemplate = template(data);

                _logger.LogInformation($"Template compiled successfully");

                return compiledTemplate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiExceptions.TemplateCompilationException(ex.Message);
            }
        }
    }
}
