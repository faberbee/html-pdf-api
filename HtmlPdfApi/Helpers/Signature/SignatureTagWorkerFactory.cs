using iText.Html2pdf.Attach;
using iText.Html2pdf.Attach.Impl;
using iText.StyledXmlParser.Node;

namespace HtmlPdfApi.Helpers.Signature
{
    /// <summary>
    /// Example of a custom tagworkerfactory for pdfHTML
    /// </summary>
    /// <remarks>
    /// The tag <bold>signature</bold> is mapped on a Signature tagworker. Every other tag is mapped to the default.
    /// </remarks>
    public class SignatureTagWorkerFactory : DefaultTagWorkerFactory
    {
        public override ITagWorker GetCustomTagWorker(IElementNode tag, ProcessorContext context)
        {
            if (tag.Name().Equals("signature"))
            {
                return new SignatureTagWorker(tag, context);
            }
            return base.GetCustomTagWorker(tag, context);
        }
    }
}

