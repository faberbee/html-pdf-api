
using System;
using iText.Html2pdf.Css.Apply;
using iText.Html2pdf.Css.Apply.Impl;
using iText.StyledXmlParser.Node;

namespace HtmlPdfApi.Helpers.Signature
{
    public class SignatureTagCssApplierFactory : DefaultCssApplierFactory
    {
        public override ICssApplier GetCustomCssApplier(IElementNode tag)
        {
            if (tag.Name().Equals("signature", StringComparison.OrdinalIgnoreCase))
            {
                return new BlockCssApplier();
            }
            return null;
        }
    }
}