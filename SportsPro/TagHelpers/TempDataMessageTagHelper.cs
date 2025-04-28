using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SportsPro.TagHelpers
{
    [HtmlTargetElement("temp-message")]
    public class TempDataMessageTagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tempdata = ViewContext.TempData;
            if (tempdata.ContainsKey("message"))
            {
                output.TagName = "div";
                output.Attributes.SetAttribute("class", "alert alert-success");
                output.Content.SetContent(tempdata["message"]?.ToString());
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}
