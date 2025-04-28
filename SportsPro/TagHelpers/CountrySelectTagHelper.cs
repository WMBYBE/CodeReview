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
    [HtmlTargetElement("Country-select")]
    public class CountrySelectTagHelper : TagHelper
    {
        private SportsProContext _context;
        public CountrySelectTagHelper(SportsProContext context)
        {
            _context = context;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }
        [HtmlAttributeName("select-country-id")]
        public string SelectedCountryId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var countries = _context.Countries
                .OrderBy(t => t.Name)
                .ToList();

            output.TagName = "select";
            output.Attributes.SetAttribute("class", "form-control");
            output.Attributes.SetAttribute("id", AspFor.Name);
            output.Attributes.SetAttribute("name", AspFor.Name);

            output.Content.AppendHtml("<option value\"\">Select a Country...</option>");

            foreach (var country in countries)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = country.CountryID.ToString();
                if (!string.IsNullOrEmpty(SelectedCountryId) && country.CountryID == SelectedCountryId)
                {
                    option.Attributes["selected"] = "selected";
                }
                option.InnerHtml.Append(country.Name);
                output.Content.AppendHtml(option);
            }
        }
    }
}
