using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SportsPro.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "active-when")]
    public class ActiveRouteTagHelper : TagHelper
    {
        private IHttpContextAccessor _httpContextAccessor;
        public ActiveRouteTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        [HtmlAttributeName("active-when")]
        public string ActiveWhen { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var requestPath = _httpContextAccessor.HttpContext.Request.Path.Value ?? "";

            if (requestPath.Contains(ActiveWhen))
            {
                var existingClass = output.Attributes["class"]?.Value?.ToString() ?? "";
                output.Attributes.SetAttribute("class", $"{existingClass} active".Trim());
            }
        }

    }
}
