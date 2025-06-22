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

    [HtmlTargetElement("critical-style-bundle-loader")]
    public class CriticalStyleBundleLoaderTagHelper : TagHelper
    {
        private readonly BundleCollection _bundleRegistry;

        public CriticalStyleBundleLoaderTagHelper(BundleCollection bundleRegistry)
        {
            _bundleRegistry = bundleRegistry;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var css = this._bundleRegistry.GetCriticalCss();
            if (!string.IsNullOrEmpty(css))
                output.Content.SetHtmlContent(@$"<!-- Injected Critical CSS Start --><style>{css}</style><!-- Injected Critical CSS End -->");
            else
                output.SuppressOutput();
        }
    }
}
