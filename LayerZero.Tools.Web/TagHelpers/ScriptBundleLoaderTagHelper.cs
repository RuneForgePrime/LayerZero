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
    [HtmlTargetElement("script-bundle-loader")]
    public class ScriptBundleLoaderTagHelper : TagHelper
    {
        private readonly BundleCollection _bundleRegistry;

        public ScriptBundleLoaderTagHelper(BundleCollection bundleRegistry)
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

            var extension = _bundleRegistry.GetExtension();

            output.TagName = null;

            if (controller == null) return;

            string html = string.Empty;
            var cacheBusting = string.Empty;
            if (_bundleRegistry.IsCacheBustingActive())
                cacheBusting = $"?v={Guid.NewGuid().ToString()}";

            if (_bundleRegistry.IsJsBundleRegistered(controller))
                html += $"<script src=\"/bundles/{controller}{extension}js{cacheBusting}\"></script>";

            if (_bundleRegistry.IsJsBundleRegistered(controller, action))
                html += $"<script src=\"/bundles/{controller}/{action}{extension}js{cacheBusting}\"></script>";

            if (!string.IsNullOrEmpty(html))
            {
                output.Content.SetHtmlContent(html);
            }
        }
    }
}
