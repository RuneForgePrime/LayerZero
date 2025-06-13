using LayerZero.Tools.Guard;
using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebOptimizer;

namespace LayerZero.Tools.Web.Bundles
{
    public static class DynamicBundleMapper
    {
        public static BundleCollection _bundles { get; } = new();

        public static void Register(IAssetPipeline pipeline, 
            string JsRoot = "js/controller", 
            string CssRoot = "css/controller",
            bool isDevelopment = false)
        {
            var rootDirectory = @"wwwroot/";

            var rootFolderJs = @$"{rootDirectory}{JsRoot}";
            var JsFolders = Directory.GetDirectories(rootFolderJs, "*", SearchOption.AllDirectories);


            foreach (var item in JsFolders)
            {
                var relativePath = item.Replace(rootFolderJs, string.Empty);
                var nodes = relativePath.Split("\\").Where(s => !string.IsNullOrEmpty(s)).ToList();
                var _item = item.Replace(rootDirectory, string.Empty).Replace("\\", "/").TrimEnd('/');
                if (nodes.Count > 2)
                {
                    continue;
                }
                else if (nodes.Count == 1)
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item, SearchOption.TopDirectoryOnly, [".js"]))
                        continue;
                    _bundles.RegisterJsBundle(nodes[0]);

                    if(isDevelopment)
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}.min.js", $"{_item}/*.js");
                    else
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}.min.js", $"{_item}/*.js").MinifyJavaScript();
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item, SearchOption.AllDirectories, [".js"]))
                        continue;

                    _bundles.RegisterJsBundle(nodes[0], nodes[1]);
                    if (isDevelopment)
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}/{nodes[1]}.min.js", $"{_item}/**/*.js");
                    else
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}/{nodes[1]}.min.js", $"{_item}/**/*.js").MinifyJavaScript();
                }
            }


            var rootFolderCss = @$"{rootDirectory}{CssRoot}";
            var CsssFolders = Directory.GetDirectories(rootFolderCss, "*", SearchOption.AllDirectories);


            foreach (var item in CsssFolders)
            {
                var relativePath = item.Replace(rootFolderCss, string.Empty);
                var nodes = relativePath.Split("\\").Where(s => !string.IsNullOrEmpty(s)).ToList();
                var _item = item.Replace(rootDirectory, string.Empty).Replace("\\", "/").TrimEnd('/');
                if (nodes.Count > 2)
                {
                    continue;
                }
                else if (nodes.Count == 1)
                {

                    if (SpindleTreeGuard.IsDirectoryEmpty(item, SearchOption.TopDirectoryOnly, [".css"]))
                        continue;

                    _bundles.RegisterCssBundle(nodes[0]);

                    if (isDevelopment)
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}.min.css", $"{_item}/*.css");
                    else
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}.min.css", $"{_item}/*.css").MinifyCss();
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item, SearchOption.AllDirectories, [".css"]))
                        continue;
                    _bundles.RegisterCssBundle(nodes[0], nodes[1]);

                    if (isDevelopment)
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}/{nodes[1]}.min.css", $"{_item.Replace("\\", "/")}/**/*.css");
                    else
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}/{nodes[1]}.min.css", $"{_item.Replace("\\", "/")}/**/*.css").MinifyCss();
                }
            }
        }
    }
}
