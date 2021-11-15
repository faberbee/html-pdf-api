using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HtmlPdfApi.Helpers.HealthChecks
{
    public class KeycloakHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public KeycloakHealthCheck(IConfiguration config, IHttpClientFactory clientFactory)
        {
            _config = config;
            _clientFactory = clientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string keycloakUrl = _config.GetValue<string>("OIDC_IDP_BASE_URL");
            var httpClient = _clientFactory.CreateClient("default");
            var response = await httpClient.GetAsync(keycloakUrl);
            string apiResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy();
            return HealthCheckResult.Unhealthy(apiResponse);
        }
    }
}