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
    [HtmlTargetElement("product-select")]
    public class ProductSelectTagHelper : TagHelper
    {
        private SportsProContext _context;
        public ProductSelectTagHelper(SportsProContext context)
        {
            _context = context;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }
        [HtmlAttributeName("select-product-id")]
        public int? SelectedProductID { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var products = _context.Products
                .OrderBy(p => p.Name)
                .ToList();

            output.TagName = "select";
            output.Attributes.SetAttribute("class", "form-control");
            output.Attributes.SetAttribute("id", AspFor.Name);
            output.Attributes.SetAttribute("name", AspFor.Name);

            output.Content.AppendHtml("<option value\"\">Select a Product...</option>");

            foreach (var product in products)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = product.ProductID.ToString();
                if (SelectedProductID.HasValue && product.ProductID == SelectedProductID.Value)
                {
                    option.Attributes["selected"] = "selected";
                }
                option.InnerHtml.Append(product.Name);
                output.Content.AppendHtml(option);
            }
        }
    }
}
