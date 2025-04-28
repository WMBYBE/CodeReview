using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using SportsPro.Models;
using System.Linq;

namespace SportsPro.TagHelpers
{
    [HtmlTargetElement("technician-select")]
    public class TechnicianSelectTagHelpercs : TagHelper
    {
        private SportsProContext _context;
        public TechnicianSelectTagHelpercs(SportsProContext context)
        {
            _context = context;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }
        [HtmlAttributeName("select-technician-id")]
        public int? SelectedTechnicianId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var technicians = _context.Technicians
                .OrderBy(t => t.Name)
                .ToList();

            output.TagName = "select";
            output.Attributes.SetAttribute("class", "form-control");
            output.Attributes.SetAttribute("id", AspFor.Name);
            output.Attributes.SetAttribute("name", AspFor.Name);

            output.Content.AppendHtml("<option value\"\">Select a Technician...</option>");

            foreach (var technician in technicians)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = technician.TechnicianID.ToString();
                if (SelectedTechnicianId.HasValue && technician.TechnicianID == SelectedTechnicianId.Value)
                {
                    option.Attributes["selected"] = "selected";
                }
                option.InnerHtml.Append(technician.Name);
                output.Content.AppendHtml(option);
            }
        }
    }
}
