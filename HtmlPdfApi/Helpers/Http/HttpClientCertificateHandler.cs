
using System.Net.Http;

namespace HtmlPdfApi.Helpers.Http
{
    public class HttpClientCertificateHandler : HttpClientHandler
    {
        public bool ValidationEnabled { get; }

        public HttpClientCertificateHandler(bool ValidationEnabled) : base()
        {
            this.ValidationEnabled = ValidationEnabled;
            if (!ValidationEnabled)
            {
                this.ClientCertificateOptions = ClientCertificateOption.Manual;
                this.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };
            }
        }
    }
}
