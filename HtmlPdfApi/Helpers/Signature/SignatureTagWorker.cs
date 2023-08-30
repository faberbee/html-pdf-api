using iText.Html2pdf.Attach;
using iText.Layout;
using iText.Layout.Element;
using iText.StyledXmlParser.Node;
using iText.Html2pdf.Attach.Impl.Tags;

namespace HtmlPdfApi.Helpers.Signature
{
    public class SignatureTagWorker : DivTagWorker
    {
        private readonly IAttributes attributes;

        public SignatureTagWorker(IElementNode element, ProcessorContext context) : base(element, context)
        {
            attributes = element.GetAttributes();
        }

        public override IPropertyContainer GetElementResult()
        {
            // Restituisco l'elemento dopo tutto il processo dato dagli step sopra
            IPropertyContainer baseResult = base.GetElementResult();
            if (baseResult is Div div)
                div.SetNextRenderer(new SignatureTagRenderer(div, attributes));
            return baseResult;
        }
    }
}