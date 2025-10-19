using LayerZero.Tools.Guard;
using LayerZero.Tools.IO;
using LayerZero.Tools.Web.Configuration;
using LayerZero.Tools.Web.Parser;
using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
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
            var JsFolders = SpindleTree.GetDirectories(rootFolderJs, 2);


            foreach (var item in JsFolders)
            {
                var relativePath = item.Path.Replace(rootFolderJs, string.Empty);
                var _item = item.Path.Replace(rootDirectory, string.Empty).Replace("\\", "/").TrimEnd('/');
                var name = _item.Replace(Cfg.JsRoot, string.Empty).TrimStart('/');

                if (item.Depth == 1)
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.TopDirectoryOnly, [".js"]))
                        continue;
                    _bundles.RegisterJsBundle(name);

                    if(!_bundles.IsMinified())
                        pipeline.AddJavaScriptBundle($"/bundles/{name}{extension}js", $"{_item}/*.js");
                    else
                        pipeline.AddJavaScriptBundle($"/bundles/{name}{extension}js", $"{_item}/*.js").MinifyJavaScript();
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.AllDirectories, [".js"]))
                        continue;

                    _bundles.RegisterJsBundle(name);
                    if (!_bundles.IsMinified())
                        pipeline.AddJavaScriptBundle($"/bundles/{name}{extension}js", $"{_item}/**/*.js");
                    else
                        pipeline.AddJavaScriptBundle($"/bundles/{name}{extension}js", $"{_item}/**/*.js").MinifyJavaScript();
                }
            }


            var rootFolderCss = @$"{rootDirectory}{Cfg.CssRoot}";
            var CssFolders = SpindleTree.GetDirectories(rootFolderCss, 2);


            foreach (var item in CssFolders)
            {
                var relativePath = item.Path.Replace(rootFolderCss, string.Empty);
                var _item = item.Path.Replace(rootDirectory, string.Empty).Replace("\\", "/").TrimEnd('/');
                var name = _item.Replace(Cfg.CssRoot, string.Empty).TrimStart('/');

                if (item.Depth == 1)
                {

                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.TopDirectoryOnly, [".css"]))
                        continue;

                    _bundles.RegisterCssBundle(name);

                    if (!_bundles.IsMinified())
                        pipeline.AddCssBundle($"/bundles/{name}{extension}css", $"{_item}/*.css");
                    else
                        pipeline.AddCssBundle($"/bundles/{name}{extension}css", $"{_item}/*.css").MinifyCss();
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.AllDirectories, [".css"]))
                        continue;
                    _bundles.RegisterCssBundle(name);

                    if (!_bundles.IsMinified())
                        pipeline.AddCssBundle($"/bundles/{name}{extension}css", $"{_item.Replace("\\", "/")}/**/*.css");
                    else
                        pipeline.AddCssBundle($"/bundles/{name}{extension}css", $"{_item.Replace("\\", "/")}/**/*.css").MinifyCss();
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


            if (!string.IsNullOrEmpty(Cfg.CommonCssRoot) && SpindleTree.GetAllFilesPath($@"{rootDirectory}{Cfg.CommonCssRoot}", FileExtensions: [".css"]).Any())
            {
                _bundles.SetIsCommonCssAvailable(true);
                pipeline.AddCssBundle($"/bundles/z-Shared{extension}css", $"{Cfg.CommonCssRoot.Replace("\\", "/")}/**/*.css");
            }


            if (!string.IsNullOrEmpty(Cfg.CommonJsRoot) && SpindleTree.GetAllFilesPath($@"{rootDirectory}{Cfg.CommonJsRoot}", FileExtensions: [".js"]).Any())
            {
                _bundles.SetIsCommonJsAvailable(true);
                pipeline.AddJavaScriptBundle($"/bundles/z-Shared{extension}js", $"{Cfg.CommonJsRoot.Replace("\\", "/")}/**/*.js");
            }

        }


        public static void RegisterBulk(IAssetPipeline pipeline, BundleCollectionConfig Cfg)
        {
            _bundles.SetBulkMode(true);

            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";

            List<string> cssAssets = new List<string> { $"{Cfg.CssRoot.Replace("\\", "/")}/**/*.css" };

            if(!string.IsNullOrEmpty(Cfg.CriticalCssRoot))
                cssAssets.Add($"{Cfg.CriticalCssRoot.Replace("\\", "/")}/**/*.css");

            if (!string.IsNullOrEmpty(Cfg.CommonCssRoot))
                cssAssets.Add($"{Cfg.CommonCssRoot.Replace("\\", "/")}/**/*.css");


            if (_bundles.IsMinified())
            {
                pipeline.AddCssBundle($"/bundles/bulk{extension}css", cssAssets.ToArray()).MinifyCss();
            }
            else
            {
                pipeline.AddCssBundle($"/bundles/bulk{extension}css", cssAssets.ToArray());
            }



            List<string> jsAssets = new List<string> { $"{Cfg.JsRoot.Replace("\\", "/")}/**/*.js" };

            if (!string.IsNullOrEmpty(Cfg.CriticalJsRoot))
                jsAssets.Add($"{Cfg.CriticalJsRoot.Replace("\\", "/")}/**/*.js");

            if (!string.IsNullOrEmpty(Cfg.CommonJsRoot))
                jsAssets.Add($"{Cfg.CommonJsRoot.Replace("\\", "/")}/**/*.js");

            if (_bundles.IsMinified())
            {
                pipeline.AddJavaScriptBundle($"/bundles/bulk{extension}js", jsAssets.ToArray()).MinifyJavaScript();
            }
            else
            {
                pipeline.AddJavaScriptBundle($"/bundles/bulk{extension}js", jsAssets.ToArray());
            }
        }
    }
}
