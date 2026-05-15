using LayerZero.Tools.Guard;
using LayerZero.Tools.IO;
using LayerZero.Tools.Web.Configuration;
using LayerZero.Tools.Web.Parser;
using LayerZero.Tools.Web.Services.Bundles;
using System.Text;

namespace LayerZero.Tools.Web.Bundles
{
    public static class DynamicBundleMapper
    {
        public static BundleCollection _bundles { get; } = new();

        public static void Register(IBundleBuilder builder, BundleCollectionConfig Cfg)
        {
            _bundles.SetCacheBusting(Cfg.EnableCacheBusting);
            _bundles.SetIsDevEnv(Cfg.IsEnvironmentDev);
            _bundles.SetIsMinified(Cfg.IsMinified);

            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";


            var rootDirectory = @"wwwroot/";

            var rootFolderJs = GenerateFullPath(rootDirectory,Cfg.JsRoot);
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
                    builder.RegisterBundle($"/bundles/{name}{extension}js", [$"{_item}/*.js"], BundleType.Js, Cfg.IsMinified);
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.AllDirectories, [".js"]))
                        continue;

                    _bundles.RegisterJsBundle(name);
                    builder.RegisterBundle($"/bundles/{name}{extension}js", [$"{_item}/**/*.js"], BundleType.Js, Cfg.IsMinified);
                }
            }


            var rootFolderCss = GenerateFullPath(rootDirectory,Cfg.CssRoot);
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
                    builder.RegisterBundle($"/bundles/{name}{extension}css", [$"{_item}/*.css"], BundleType.Css, Cfg.IsMinified);
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.AllDirectories, [".css"]))
                        continue;
                    _bundles.RegisterCssBundle(name);
                    builder.RegisterBundle($"/bundles/{name}{extension}css", [$"{_item.Replace("\\", "/")}/**/*.css"], BundleType.Css, Cfg.IsMinified);
                }
            }

            var rootFolderCssCritical = GenerateFullPath(rootDirectory,Cfg.CriticalCssRoot);

            var cssCriticalFiles = SpindleTree.GetAllFilesPath(rootFolderCssCritical, FileExtensions: [".css"]);

            var criticalCss = new StringBuilder();
            cssCriticalFiles?.ForEach(f =>
            {
                var rules = CssFileParser.Analyse(f);
                criticalCss.AppendLine(rules);
            });

            if(!string.IsNullOrEmpty(criticalCss.ToString()))
                _bundles.SetCriticalCss(criticalCss.ToString());


            var rootFolderJsCritical = GenerateFullPath(rootDirectory,Cfg.CriticalJsRoot);
            var jsCriticalFiles = SpindleTree.GetAllFilesPath(rootFolderJsCritical, FileExtensions: [".js"]);

            var criticalJs = new StringBuilder();
            jsCriticalFiles?.ForEach(f =>
            {
                var script = JsFileParser.Analyse(f);
                criticalJs.AppendLine(script);
            });

            if (!string.IsNullOrEmpty(criticalJs.ToString()))
                _bundles.SetCriticalJs(criticalJs.ToString());


            if (SpindleTree.GetAllFilesPath(GenerateFullPath(rootDirectory,Cfg.CommonCssRoot), FileExtensions: [".css"])?.Count > 0)
            {
                _bundles.SetIsCommonCssAvailable(true);
                builder.RegisterBundle($"/bundles/z-Shared{extension}css", [$"{Cfg.CommonCssRoot.Replace("\\", "/")}/**/*.css"], BundleType.Css, Cfg.IsMinified);
            }


            if (SpindleTree.GetAllFilesPath(GenerateFullPath(rootDirectory,Cfg.CommonJsRoot), FileExtensions: [".js"])?.Count > 0)
            {
                _bundles.SetIsCommonJsAvailable(true);
                builder.RegisterBundle($"/bundles/z-Shared{extension}js", [$"{Cfg.CommonJsRoot.Replace("\\", "/")}/**/*.js"], BundleType.Js, Cfg.IsMinified);
            }

        }

        private static string? GenerateFullPath(string Root, string relative)
        {
            if (string.IsNullOrEmpty(relative))
                return null;
            var path = $"{Root}{relative}";
            return Directory.Exists(path) ? path : null;
        }


        public static void RegisterBulk(IBundleBuilder builder, BundleCollectionConfig Cfg)
        {

            _bundles.SetBulkMode(true);
            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";
            var rootDirectory = @"wwwroot/";


            List<string> cssAssets = new List<string>();


            var cssAssetsPath = GenerateFullPath(rootDirectory, Cfg.CssRoot);
            if(cssAssets != null)
                cssAssets.Add($"{Cfg.CssRoot.Replace("\\", "/")}/**/*.css");


            if(!string.IsNullOrEmpty(GenerateFullPath(rootDirectory, Cfg.CriticalCssRoot)))
                cssAssets.Add($"{Cfg.CriticalCssRoot.Replace("\\", "/")}/**/*.css");

            if (!string.IsNullOrEmpty(GenerateFullPath(rootDirectory, Cfg.CommonCssRoot)))
                cssAssets.Add($"{Cfg.CommonCssRoot.Replace("\\", "/")}/**/*.css");


            if (cssAssets.Any())
                builder.RegisterBundle($"/bundles/bulk{extension}css", cssAssets.ToArray(), BundleType.Css, Cfg.IsMinified);


            List<string> jsAssets = new List<string>();


            var jsAssetsPath = GenerateFullPath(rootDirectory, Cfg.JsRoot);
            if (jsAssetsPath != null)
                jsAssets.Add($"{Cfg.JsRoot.Replace("\\", "/")}/**/*.js");


            if (!string.IsNullOrEmpty(GenerateFullPath(rootDirectory, Cfg.CriticalJsRoot)))
                jsAssets.Add($"{Cfg.CriticalJsRoot.Replace("\\", "/")}/**/*.js");

            if (!string.IsNullOrEmpty(GenerateFullPath(rootDirectory, Cfg.CommonJsRoot)))
                jsAssets.Add($"{Cfg.CommonJsRoot.Replace("\\", "/")}/**/*.js");

            if (jsAssets.Any())
                builder.RegisterBundle($"/bundles/bulk{extension}js", jsAssets.ToArray(), BundleType.Js, Cfg.IsMinified);
        }
    }
}
