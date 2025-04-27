

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace SportsPro.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asp-controller, asp-action")]
    public class ActiveTagHelper : TagHelper
    {
        private IHttpContextAccessor _httpContextAccessor;
        public ActiveTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();

            if (string.Equals(Controller, currentController) &&
                string.Equals(Action, currentAction))
            {
                var existingClass = output.Attributes["class"]?.Value?.ToString() ?? "";
                output.Attributes.SetAttribute("class", $"(existingClass) active".Trim());
            }
        }
    }
}
