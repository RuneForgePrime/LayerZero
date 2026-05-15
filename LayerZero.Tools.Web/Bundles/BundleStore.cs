using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUglify;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace LayerZero.Tools.Web.Bundles
{
    public class BundleStore : IBundleBuilder
    {
        private readonly string _webRootPath;
        private readonly ILogger<BundleStore> _logger;
        private readonly ConcurrentDictionary<string, BundleDescriptor> _descriptors = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, CachedBundle> _contentCache = new(StringComparer.OrdinalIgnoreCase);

        public BundleStore(string webRootPath, ILogger<BundleStore>? logger = null)
        {
            _webRootPath = webRootPath;
            _logger = logger ?? NullLogger<BundleStore>.Instance;
        }

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

            bundle = _contentCache.GetOrAdd(route, _ => Build(descriptor));
            return true;
        }

        public bool IsRegistered(string route) => _descriptors.ContainsKey(route);

        private CachedBundle Build(BundleDescriptor descriptor)
        {
            var raw = new StringBuilder();

            var matcher = new Matcher();
            foreach (var glob in descriptor.Globs)
                matcher.AddInclude(glob);

            var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(_webRootPath)));
            foreach (var file in result.Files.OrderBy(f => f.Path))
            {
                try
                {
                    raw.AppendLine(File.ReadAllText(Path.Combine(_webRootPath, file.Path)));
                }
                catch (IOException)
                {
                    // file disappeared or is unreadable between glob resolution and read; skip
                }
            }

            var content = raw.ToString();

            if (descriptor.Minify && !string.IsNullOrWhiteSpace(content))
            {
                var minified = descriptor.Type == BundleType.Css
                    ? Uglify.Css(content)
                    : Uglify.Js(content);

                if (!minified.HasErrors)
                {
                    content = minified.Code;
                }
                else
                {
                    var errors = string.Join("\n", minified.Errors.Select(e => $"   - {e.Message}"));
                    content = $"/* LayerZero: minification failed — serving raw content\n{errors}\n*/\n{content}";
                    _logger.LogWarning("Bundle minification failed for descriptor with {ErrorCount} error(s):\n{Errors}",
                        minified.Errors.Count, errors);
                }
            }

            var contentType = descriptor.Type == BundleType.Css
                ? "text/css; charset=utf-8"
                : "application/javascript; charset=utf-8";

            var etag = $"\"{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)))}\"";

            return new CachedBundle(content, contentType, etag);
        }
    }
}
