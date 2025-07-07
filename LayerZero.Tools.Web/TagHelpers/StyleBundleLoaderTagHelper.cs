using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.TagHelpers
{
    [HtmlTargetElement("style-bundle-loader")]
    public class StyleBundleLoaderTagHelper : TagHelper
    {
        private readonly BundleCollection _bundleRegistry;

        public StyleBundleLoaderTagHelper(BundleCollection bundleRegistry)
        {
            _bundleRegistry = bundleRegistry;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var controller = ViewContext.RouteData.Values["controller"]?.ToString()?.ToLowerInvariant();
            var action = ViewContext.RouteData.Values["action"]?.ToString()?.ToLowerInvariant();

            output.TagName = null;

            if (controller == null) return;

            string html = string.Empty;
            var cacheBusting = string.Empty;
            if(_bundleRegistry.IsCacheBustingActive())
                cacheBusting = $"?v={Guid.NewGuid().ToString()}";


            if (_bundleRegistry.IsCssBundleRegistered(controller))
                html += $"<link rel=\"stylesheet\" href=\"/bundles/{controller}.min.css{cacheBusting}\" />";

            if (_bundleRegistry.IsCssBundleRegistered(controller, action))
                html += $"<link rel=\"stylesheet\" href=\"/bundles/{controller}/{action}.min.css{cacheBusting}\" />";

            if (!string.IsNullOrEmpty(html))
            {
                output.Content.SetHtmlContent(html);
            }
        }
    }
}
