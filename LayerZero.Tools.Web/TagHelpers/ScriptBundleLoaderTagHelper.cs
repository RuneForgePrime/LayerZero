using LayerZero.Tools.Web.Bundles;
using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LayerZero.Tools.Web.TagHelpers
{
    [HtmlTargetElement("script-bundle-loader")]
    public class ScriptBundleLoaderTagHelper : TagHelper
    {
        private readonly BundleCollection _bundleRegistry;
        private readonly BundleStore _store;

        public ScriptBundleLoaderTagHelper(BundleCollection bundleRegistry, BundleStore store)
        {
            _bundleRegistry = bundleRegistry;
            _store = store;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private string CacheBust(string route)
        {
            if (!_bundleRegistry.IsCacheBustingActive()) return string.Empty;
            return _store.TryGetOrBuild(route, out var bundle)
                ? $"?v={bundle.ETag.Trim('"')}"
                : string.Empty;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var controller = ViewContext.RouteData.Values["controller"]?.ToString()?.ToLowerInvariant();
            var action = ViewContext.RouteData.Values["action"]?.ToString()?.ToLowerInvariant();

            var extension = _bundleRegistry.GetExtension();

            output.TagName = null;

            if (controller == null) return;

            string html = string.Empty;

            if (_bundleRegistry.IsCommonJsAvailable())
            {
                var route = $"/bundles/z-Shared{extension}js";
                html += $"<script src=\"{route}{CacheBust(route)}\"></script>";
            }

            if (_bundleRegistry.IsJsBundleRegistered(controller))
            {
                var route = $"/bundles/{controller}{extension}js";
                html += $"<script src=\"{route}{CacheBust(route)}\"></script>";
            }

            if (_bundleRegistry.IsJsBundleRegistered(controller, action))
            {
                var route = $"/bundles/{controller}/{action}{extension}js";
                html += $"<script src=\"{route}{CacheBust(route)}\"></script>";
            }

            if (!string.IsNullOrEmpty(html))
                output.Content.SetHtmlContent(html);
        }
    }
}
