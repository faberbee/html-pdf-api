using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace HtmlPdfApi.Models.Browser
{
    public class BrowserDto
    {
        [Required]
        public string resourceUrl { get; set; }

        [Required]
        public ExpandoObject data { get; set; }
    }
}
