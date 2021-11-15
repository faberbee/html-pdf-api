namespace HtmlPdfApi.Helpers.Exceptions
{
    public static class ApiExceptions
    {
        public class BaseException : System.Exception
        {
            public string type { get; internal set; }
            public string subType { get; internal set; }
            public string message { get; internal set; }
            public string internalMessage { get; internal set; }
            public BaseException(string type, string subType, string message, string internalMessage = null)
            {
                this.type = type;
                this.subType = subType;
                this.message = message;
                this.internalMessage = internalMessage;
            }
        }

        public class ForbiddenException : BaseException
        {
            public ForbiddenException() : base("ForbiddenException", null, "Missing or wrong scope") { }
        }

        public class ResourceException : BaseException
        {
            public ResourceException(string resource, string message = null) : base("ResourceException", null, $"ERROR while try to retrieve {resource}", message) { }
        }

        public class ResourceNotFoundException : BaseException
        {
            public ResourceNotFoundException(string resource, string message = null) : base("ResourceNotFoundException", null, $"ERROR while try to retrieve {resource}: Not found", message) { }
        }

        public class TemplateCompilationException : BaseException
        {
            public TemplateCompilationException(string message = null) : base("TemplateCompilationException", null, $"ERROR while try to compile template", message) { }
        }

        public class PdfCreationException : BaseException
        {
            public PdfCreationException(string message = null) : base("PdfCreationException", null, "ERROR while try to create PDF file", message) { }
        }
    }
}