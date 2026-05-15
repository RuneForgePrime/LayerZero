using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using NUglify;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace LayerZero.Tools.Web.Bundles
{
    public class BundleStore : IBundleBuilder
    {
        private readonly ConcurrentDictionary<string, BundleDescriptor> _descriptors = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, CachedBundle> _contentCache = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterBundle(string route, string[] sourceGlobs, BundleType type, bool minify)
        {
            _descriptors[route] = new BundleDescriptor(sourceGlobs, type, minify);
        }

        public bool TryGetOrBuild(string route, out CachedBundle bundle)
        {
            if (_contentCache.TryGetValue(route, out bundle!))
                return true;

            if (!_descriptors.TryGetValue(route, out var descriptor))
                return false;

            var built = Build(descriptor);
            bundle = _contentCache.GetOrAdd(route, built);
            return true;
        }

        public bool IsRegistered(string route) => _descriptors.ContainsKey(route);

        private static CachedBundle Build(BundleDescriptor descriptor)
        {
            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var raw = new StringBuilder();

            var matcher = new Matcher();
            foreach (var glob in descriptor.Globs)
                matcher.AddInclude(glob);

            var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(wwwroot)));
            foreach (var file in result.Files.OrderBy(f => f.Path))
                raw.AppendLine(File.ReadAllText(Path.Combine(wwwroot, file.Path)));

            var content = raw.ToString();

            if (descriptor.Minify && !string.IsNullOrWhiteSpace(content))
            {
                var minified = descriptor.Type == BundleType.Css
                    ? Uglify.Css(content)
                    : Uglify.Js(content);

                if (!minified.HasErrors)
                    content = minified.Code;
            }

            var contentType = descriptor.Type == BundleType.Css
                ? "text/css; charset=utf-8"
                : "application/javascript; charset=utf-8";

            var etag = $"\"{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)))}\"";

            return new CachedBundle(content, contentType, etag);
        }
    }
}
