

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
    [HtmlTargetElement("customer-select")]
    public class CustomerSelectTagHelper : TagHelper
    {
        private SportsProContext _context;
        public CustomerSelectTagHelper(SportsProContext context)
        {
            _context = context;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }
        [HtmlAttributeName("select-customer-id")]
        public int? SelectedCustomerID { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var customers = _context.Customers
                .OrderBy(c => c.FirstName)
                .Select(c => new { c.CustomerID, c.FullName })
                .ToList();

            output.TagName = "select";
            output.Attributes.SetAttribute("class", "form-control");
            output.Attributes.SetAttribute("id", AspFor.Name);
            output.Attributes.SetAttribute("name", AspFor.Name);

            output.Content.AppendHtml("<option value\"\">Select a Customer...</option>");

            foreach(var customer in customers)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = customer.CustomerID.ToString();
                if (SelectedCustomerID.HasValue && customer.CustomerID== SelectedCustomerID.Value)
                {
                    option.Attributes["selected"] = "selected";
                }
                option.InnerHtml.Append(customer.FullName);
                output.Content.AppendHtml(option);
            }
        }
    }
}
