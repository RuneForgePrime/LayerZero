using LayerZero.Tools.Guard;
using LayerZero.Tools.IO;
using LayerZero.Tools.Web.Configuration;
using LayerZero.Tools.Web.Services.Bundles;

namespace LayerZero.Tools.Web.Bundles
{
    public static class DynamicBundleMapper
    {
        public static void Register(IBundleBuilder builder, BundleCollection bundles, BundleCollectionConfig Cfg, string webRootPath)
        {
            bundles.SetCacheBusting(Cfg.EnableCacheBusting);
            bundles.SetIsDevEnv(Cfg.IsEnvironmentDev);
            bundles.SetIsMinified(Cfg.IsMinified);

            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";

            var rootFolderJs = GenerateFullPath(webRootPath, Cfg.JsRoot);
            var JsFolders = rootFolderJs != null ? SpindleTree.GetDirectories(rootFolderJs, 2) : [];


            foreach (var item in JsFolders)
            {
                var _item = Path.GetRelativePath(webRootPath, item.Path).Replace('\\', '/').TrimEnd('/');
                var name = _item.Replace(Cfg.JsRoot!.Replace('\\', '/').TrimStart('/'), string.Empty).TrimStart('/');

                if (item.Depth == 1)
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.TopDirectoryOnly, [".js"]))
                        continue;
                    bundles.RegisterJsBundle(name);
                    builder.RegisterBundle($"/bundles/{name}{extension}js", [$"{_item}/*.js"], BundleType.Js, Cfg.IsMinified);
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.AllDirectories, [".js"]))
                        continue;

                    bundles.RegisterJsBundle(name);
                    builder.RegisterBundle($"/bundles/{name}{extension}js", [$"{_item}/**/*.js"], BundleType.Js, Cfg.IsMinified);
                }
            }


            var rootFolderCss = GenerateFullPath(webRootPath, Cfg.CssRoot);
            var CssFolders = rootFolderCss != null ? SpindleTree.GetDirectories(rootFolderCss, 2) : [];


            foreach (var item in CssFolders)
            {
                var _item = Path.GetRelativePath(webRootPath, item.Path).Replace('\\', '/').TrimEnd('/');
                var name = _item.Replace(Cfg.CssRoot!.Replace('\\', '/').TrimStart('/'), string.Empty).TrimStart('/');

                if (item.Depth == 1)
                {

                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.TopDirectoryOnly, [".css"]))
                        continue;

                    bundles.RegisterCssBundle(name);
                    builder.RegisterBundle($"/bundles/{name}{extension}css", [$"{_item}/*.css"], BundleType.Css, Cfg.IsMinified);
                }
                else
                {
                    if (SpindleTreeGuard.IsDirectoryEmpty(item.Path, SearchOption.AllDirectories, [".css"]))
                        continue;
                    bundles.RegisterCssBundle(name);
                    builder.RegisterBundle($"/bundles/{name}{extension}css", [$"{_item.Replace("\\", "/")}/**/*.css"], BundleType.Css, Cfg.IsMinified);
                }
            }

            var rootFolderCssCritical = GenerateFullPath(webRootPath, Cfg.CriticalCssRoot);
            if (rootFolderCssCritical != null && SpindleTree.GetAllFilesPath(rootFolderCssCritical, FileExtensions: [".css"])?.Count > 0)
            {
                bundles.SetIsCriticalCssAvailable(true);
                builder.RegisterBundle($"/bundles/z-Critical{extension}css",
                    [$"{Cfg.CriticalCssRoot!.Replace("\\", "/")}/**/*.css"], BundleType.Css, Cfg.IsMinified);
            }

            var rootFolderJsCritical = GenerateFullPath(webRootPath, Cfg.CriticalJsRoot);
            if (rootFolderJsCritical != null && SpindleTree.GetAllFilesPath(rootFolderJsCritical, FileExtensions: [".js"])?.Count > 0)
            {
                bundles.SetIsCriticalJsAvailable(true);
                builder.RegisterBundle($"/bundles/z-Critical{extension}js",
                    [$"{Cfg.CriticalJsRoot!.Replace("\\", "/")}/**/*.js"], BundleType.Js, Cfg.IsMinified);
            }

            if (SpindleTree.GetAllFilesPath(GenerateFullPath(webRootPath, Cfg.CommonCssRoot), FileExtensions: [".css"])?.Count > 0)
            {
                bundles.SetIsCommonCssAvailable(true);
                builder.RegisterBundle($"/bundles/z-Shared{extension}css", [$"{Cfg.CommonCssRoot!.Replace("\\", "/")}/**/*.css"], BundleType.Css, Cfg.IsMinified);
            }

            if (SpindleTree.GetAllFilesPath(GenerateFullPath(webRootPath, Cfg.CommonJsRoot), FileExtensions: [".js"])?.Count > 0)
            {
                bundles.SetIsCommonJsAvailable(true);
                builder.RegisterBundle($"/bundles/z-Shared{extension}js", [$"{Cfg.CommonJsRoot!.Replace("\\", "/")}/**/*.js"], BundleType.Js, Cfg.IsMinified);
            }

        }

        private static string? GenerateFullPath(string webRootPath, string? relative)
        {
            if (string.IsNullOrEmpty(relative))
                return null;
            var path = Path.Combine(webRootPath, relative);
            return Directory.Exists(path) ? path : null;
        }


        public static void RegisterBulk(IBundleBuilder builder, BundleCollection bundles, BundleCollectionConfig Cfg, string webRootPath)
        {
            bundles.SetBulkMode(true);
            var extension = Cfg.IsEnvironmentDev ? ".dev." : Cfg.IsMinified ? ".min." : ".";

            List<string> cssAssets = new List<string>();

            if (!string.IsNullOrEmpty(GenerateFullPath(webRootPath, Cfg.CssRoot)))
                cssAssets.Add($"{Cfg.CssRoot!.Replace("\\", "/")}/**/*.css");

            if (!string.IsNullOrEmpty(GenerateFullPath(webRootPath, Cfg.CriticalCssRoot)))
                cssAssets.Add($"{Cfg.CriticalCssRoot!.Replace("\\", "/")}/**/*.css");

            if (!string.IsNullOrEmpty(GenerateFullPath(webRootPath, Cfg.CommonCssRoot)))
                cssAssets.Add($"{Cfg.CommonCssRoot!.Replace("\\", "/")}/**/*.css");

            if (cssAssets.Any())
                builder.RegisterBundle($"/bundles/bulk{extension}css", cssAssets.ToArray(), BundleType.Css, Cfg.IsMinified);

            List<string> jsAssets = new List<string>();

            if (GenerateFullPath(webRootPath, Cfg.JsRoot) != null)
                jsAssets.Add($"{Cfg.JsRoot!.Replace("\\", "/")}/**/*.js");

            if (!string.IsNullOrEmpty(GenerateFullPath(webRootPath, Cfg.CriticalJsRoot)))
                jsAssets.Add($"{Cfg.CriticalJsRoot!.Replace("\\", "/")}/**/*.js");

            if (!string.IsNullOrEmpty(GenerateFullPath(webRootPath, Cfg.CommonJsRoot)))
                jsAssets.Add($"{Cfg.CommonJsRoot!.Replace("\\", "/")}/**/*.js");

            if (jsAssets.Any())
                builder.RegisterBundle($"/bundles/bulk{extension}js", jsAssets.ToArray(), BundleType.Js, Cfg.IsMinified);
        }
    }
}
