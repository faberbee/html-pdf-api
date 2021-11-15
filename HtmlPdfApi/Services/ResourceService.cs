using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HtmlPdfApi.Helpers.Exceptions;

namespace HtmlPdfApi.Services
{
    public class ResourceService
    {
        private readonly ILogger<ResourceService> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public ResourceService(ILogger<ResourceService> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public virtual async Task<string> GetDocumentResourceAsync(string resourceUrl)
        {
            try
            {
                _logger.LogInformation("Getting resource from url");

                var httpClient = _clientFactory.CreateClient("default");
                var response = await httpClient.GetAsync(resourceUrl);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ApiExceptions.ResourceNotFoundException(resourceUrl, apiResponse);

                if (!response.IsSuccessStatusCode)
                    throw new ApiExceptions.ResourceException(resourceUrl, apiResponse);

                _logger.LogInformation($"Resource: [ {resourceUrl} ] successfully retrieved.");

                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
