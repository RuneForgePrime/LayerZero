using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.TagHelpers
{

    [HtmlTargetElement("bundle-benchmark-loader")]
    public class BenchmarkLoaderTagHelper : TagHelper
    {
        [HtmlAttributeName("filter")]
        public string Filter { get; set; }

        [HtmlAttributeName("type")]
        public string AssetType { get; set; } // css, js, or both (null)

        private readonly BundleCollection _bundleRegistry;
        public BenchmarkLoaderTagHelper(BundleCollection bundleRegistry)
        {
            _bundleRegistry = bundleRegistry;
        }
        bool Match(string name)
        {
            return string.IsNullOrEmpty(Filter) || name.Contains(Filter, StringComparison.OrdinalIgnoreCase);
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = null;

            string resource = string.Empty;
            var extension = _bundleRegistry.GetExtension();

            if (AssetType is null || AssetType.Contains("css", StringComparison.OrdinalIgnoreCase))
            {
                if (_bundleRegistry.IsBulkActive() && Filter is null)
                {
                    resource += @$"<li> <a href='/bundles/bulk{extension}css'> /bundles/bulk{extension}css </a> </li>";
                }
                _bundleRegistry.GetAllCss()
                    .Where(css => Match(css))
                    .OrderBy(x => x)
                    .ToList()
                    .ForEach(css => {
                        resource += $"<li> <a href='/bundles/{css}{extension}css'> /bundles/{css}{extension}css </a> </li>";
                    });
            }

            if (AssetType is null || AssetType.Contains("js", StringComparison.OrdinalIgnoreCase))
            {

                if (_bundleRegistry.IsBulkActive() && Filter is null)
                {
                    resource += @$" <li> <a href='/bundles/bulk{extension}js'> /bundles/bulk{extension}js  </a> </li>";
                }

                _bundleRegistry.GetAllJs()
                    .Where(js => Match(js))
                    .OrderBy(x => x)
                    .ToList()
                    .ForEach(js => {
                        resource += $"<li> <a href='/bundles/{js}{extension}js'> /bundles/{js}{extension}js  </a> </li>";
                    });
            }



            if (!string.IsNullOrEmpty(resource))
            {
                var html = $@"<ul id=""{Guid.NewGuid().ToString()}"" class=""benchmark-module"">
                            {resource}
                            </ul>

                            <script>
                                    async function benchmarkAsset(url, liElement) {{
                                  const startTime = performance.now();

                                  try {{
                                    const response = await fetch(url, {{ cache: ""no-store"" }});

                                    const endTime = performance.now();
                                    const duration = (endTime - startTime).toFixed(2); // ms

                                    const contentType = response.headers.get(""content-type"");

                                    const buffer = await response.arrayBuffer();
                                    const sizeBytes = buffer.byteLength;
                                    const sizeKB = (sizeBytes / 1024).toFixed(2) + "" KB"";

                                    // Create and append result element
                                    const resultDiv = document.createElement(""div"");
                                    resultDiv.className = ""benchmark-results"";
                                    resultDiv.style.fontSize = ""0.85rem"";
                                    resultDiv.style.marginTop = ""4px"";
                                    resultDiv.innerHTML = `
                                      <strong>Type:</strong> ${{contentType}}<br>
                                      <strong>Load time:</strong> ${{duration}} ms<br>
                                      <strong>Size:</strong> ${{sizeKB}}
                                    `;
                                    liElement.appendChild(resultDiv);

                                    return {{
                                      url,
                                      duration: Number(duration),
                                      size: sizeKB,
                                      type: contentType
                                    }};
                                  }} catch (err) {{
                                    console.warn(`Failed to load ${{url}}:`, err);

                                    // Add error info to UI
                                    const errorDiv = document.createElement(""div"");
                                    errorDiv.className = ""benchmark-results error"";
                                    errorDiv.style.color = ""red"";
                                    errorDiv.style.marginTop = ""4px"";
                                    errorDiv.textContent = `⚠️ Failed to load: ${{err.message}}`;
                                    liElement.appendChild(errorDiv);

                                    return null;
                                  }}
                                }}

                                async function benchmarkAll() {{
                                  const links = document.querySelectorAll(""ul.benchmark-module li > a"");

                                  for (const link of links) {{
                                    const li = link.closest(""li"");
                                    await benchmarkAsset(link.href, li);
                                  }}
                                }}

                                  if (!window.__benchmarkInitialized) {{

                                        window.__benchmarkInitialized = true;
                                        // benchmarkAll and asset logic here
                                        window.onload = benchmarkAll;
                                  }}


                            </script>


                            ";
                output.Content.SetHtmlContent(html);
            }
        }
    }
}
