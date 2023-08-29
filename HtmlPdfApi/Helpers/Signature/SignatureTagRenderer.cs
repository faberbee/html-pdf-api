
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.StyledXmlParser.Node;

namespace HtmlPdfApi.Helpers.Signature
{
    public class SignatureTagRenderer : DivRenderer
    {
        private IAttributes attributes;

        public SignatureTagRenderer(Div modelElement, IAttributes _attributes) : base(modelElement)
        {
            attributes = _attributes;
        }

        public override IRenderer GetNextRenderer()
        {
            return new SignatureTagRenderer((Div)modelElement, attributes);
        }

        public override void Draw(DrawContext drawContext)
        {
            base.Draw(drawContext);

            Rectangle occupiedArea = this.GetOccupiedAreaBBox();

            // Retrieve all necessary properties to create the signature
            string nameAttr = attributes.GetAttribute("name");
            string reasonAttr = attributes.GetAttribute("reason");
            string requiredAttr = attributes.GetAttribute("required");

            // Create field
            var signatureField = PdfSignatureFormField.CreateSignature(drawContext.GetDocument(), occupiedArea);
            signatureField.SetFieldName(nameAttr);
            signatureField.SetRequired(requiredAttr == "" || requiredAttr == "true");

            // New implementation for iText7 v8
            // PdfFormField signatureField = new SignatureFormFieldBuilder(drawContext.GetDocument(), nameAttr)
            //     .SetWidgetRectangle(occupiedArea)
            //     .CreateSignature();

            // Set flags
            signatureField.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE).SetFlags(PdfAnnotation.PRINT);

            // Add to acro form
            PdfAcroForm.GetAcroForm(drawContext.GetDocument(), true).AddField(signatureField);
        }
    }
}
