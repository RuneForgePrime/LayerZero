using LayerZero.Tools.Guard;
using LayerZero.Tools.IO;
using LayerZero.Tools.Web.Configuration;
using LayerZero.Tools.Web.Parser;
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

        public static void Register(IAssetPipeline pipeline, BundleCollectionConfig Cfg)
        {
            _bundles.SetCacheBusting(Cfg.EnableCacheBusting);
            _bundles.SetIsDevEnv(Cfg.IsEnvironmentDev);
            _bundles.SetIsMinified(Cfg.IsMinified);

            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";


            var rootDirectory = @"wwwroot/";

            var rootFolderJs = @$"{rootDirectory}{Cfg.JsRoot}";
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

                    if(Cfg.EnableCacheBusting)
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}{extension}js", $"{_item}/*.js");
                    else
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}{extension}js", $"{_item}/*.js").MinifyJavaScript();
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item, SearchOption.AllDirectories, [".js"]))
                        continue;

                    _bundles.RegisterJsBundle(nodes[0], nodes[1]);
                    if (Cfg.EnableCacheBusting)
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}/{nodes[1]}{extension}js", $"{_item}/**/*.js");
                    else
                        pipeline.AddJavaScriptBundle($"/bundles/{nodes[0]}/{nodes[1]}{extension}js", $"{_item}/**/*.js").MinifyJavaScript();
                }
            }


            var rootFolderCss = @$"{rootDirectory}{Cfg.CssRoot}";
            var CssFolders = Directory.GetDirectories(rootFolderCss, "*", SearchOption.AllDirectories);


            foreach (var item in CssFolders)
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

                    if (Cfg.EnableCacheBusting)
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}{extension}css", $"{_item}/*.css");
                    else
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}{extension}css", $"{_item}/*.css").MinifyCss();
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item, SearchOption.AllDirectories, [".css"]))
                        continue;
                    _bundles.RegisterCssBundle(nodes[0], nodes[1]);

                    if (Cfg.EnableCacheBusting)
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}/{nodes[1]}{extension}css", $"{_item.Replace("\\", "/")}/**/*.css");
                    else
                        pipeline.AddCssBundle($"/bundles/{nodes[0]}/{nodes[1]}{extension}css", $"{_item.Replace("\\", "/")}/**/*.css").MinifyCss();
                }
            }

            var rootFolderCssCritical = $@"{rootDirectory}{Cfg.CriticalCssRoot}";

            var cssCriticalFiles = SpindleTree.GetAllFilesPath(rootFolderCssCritical, FileExtensions: [".css"]);

            var criticalCss = new StringBuilder();
            cssCriticalFiles?.ForEach(f =>
            {
                var rules = CssFileParser.Analyse(f);
                criticalCss.AppendLine(rules);
            });

            if(!string.IsNullOrEmpty(criticalCss.ToString()))
                _bundles.SetCriticalCss(criticalCss.ToString());


            var rootFolderJsCritical = $@"{rootDirectory}{Cfg.CriticalJsRoot}";
            var jsCriticalFiles = SpindleTree.GetAllFilesPath(rootFolderJsCritical, FileExtensions: [".js"]);

            var criticalJs = new StringBuilder();
            jsCriticalFiles?.ForEach(f =>
            {
                var script = JsFileParser.Analyse(f);
                criticalJs.AppendLine(script);
            });

            if (!string.IsNullOrEmpty(criticalJs.ToString()))
                _bundles.SetCriticalJs(criticalJs.ToString());

        }


        public static void RegisterBulk(IAssetPipeline pipeline, BundleCollectionConfig Cfg)
        {
            _bundles.SetBulkMode(true);

            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";

            pipeline.AddCssBundle($"/bundles/bulk{extension}css", $"{Cfg.CssRoot.Replace("\\", "/")}/**/*.css", $"{Cfg.CriticalCssRoot.Replace("\\", "/")}/**/*.css").MinifyCss();

            pipeline.AddJavaScriptBundle($"/bundles/bulk{extension}js", $"{Cfg.JsRoot.Replace("\\", "/")}/**/*.js", $"{Cfg.CriticalJsRoot.Replace("\\", "/")}/**/*.js").MinifyJavaScript();
        }
    }
}
