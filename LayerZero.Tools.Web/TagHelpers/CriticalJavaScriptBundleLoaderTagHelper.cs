using LayerZero.Tools.Web.Bundles;
using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LayerZero.Tools.Web.TagHelpers
{
    [HtmlTargetElement("critical-script-bundle-loader")]
    public class CriticalJavaScriptBundleLoaderTagHelper : TagHelper
    {
        private readonly BundleCollection _bundleRegistry;
        private readonly BundleStore _store;

        public CriticalJavaScriptBundleLoaderTagHelper(BundleCollection bundleRegistry, BundleStore store)
        {
            _bundleRegistry = bundleRegistry;
            _store = store;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            if (!_bundleRegistry.IsCriticalJsAvailable()) { output.SuppressOutput(); return; }

            var route = $"/bundles/z-Critical{_bundleRegistry.GetExtension()}js";
            if (_store.TryGetOrBuild(route, out var bundle) && !string.IsNullOrEmpty(bundle.Content))
                output.Content.SetHtmlContent($"<!-- Critical Js Start --><script>{bundle.Content}</script><!-- Critical Js End -->");
            else
                output.SuppressOutput();
        }
    }
}
