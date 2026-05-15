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
    public class BundleStore : IBundleBuilder, IDisposable
    {
        private readonly string _webRootPath;
        private readonly ILogger<BundleStore> _logger;
        private readonly ConcurrentDictionary<string, BundleDescriptor> _descriptors = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, CachedBundle> _contentCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<FileSystemWatcher> _watchers = new();
        private int _watchersStarted = 0;

        public BundleStore(string webRootPath, ILogger<BundleStore>? logger = null)
        {
            _webRootPath = webRootPath;
            _logger = logger ?? NullLogger<BundleStore>.Instance;
        }

        public void StartWatchers()
        {
            if (Interlocked.CompareExchange(ref _watchersStarted, 1, 0) != 0) return;
            if (!Directory.Exists(_webRootPath)) return;

            var dirToRoutes = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var (route, descriptor) in _descriptors)
            {
                foreach (var glob in descriptor.Globs)
                {
                    var relBase = GetGlobBaseDir(glob);
                    var absBase = Path.GetFullPath(Path.Combine(_webRootPath, relBase.Replace('/', Path.DirectorySeparatorChar)));
                    if (!Directory.Exists(absBase)) continue;

                    if (!dirToRoutes.TryGetValue(absBase, out var routes))
                        dirToRoutes[absBase] = routes = new List<string>();
                    if (!routes.Contains(route))
                        routes.Add(route);
                }
            }

            foreach (var (dir, routes) in dirToRoutes)
            {
                var capturedRoutes = routes;
                var watcher = new FileSystemWatcher(dir)
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                    EnableRaisingEvents = true
                };
                void OnChanged(object s, FileSystemEventArgs e)
                {
                    foreach (var route in capturedRoutes)
                    {
                        if (_contentCache.TryRemove(route, out _))
                            _logger.LogInformation("Bundle cache evicted: {Route} (file changed: {File})", route, e.Name);
                    }
                }
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Renamed += (s, e) => OnChanged(s, e);
                _watchers.Add(watcher);
            }

            _logger.LogInformation("Bundle watchers started: {Count} director{Suffix} monitored",
                dirToRoutes.Count, dirToRoutes.Count == 1 ? "y" : "ies");
        }

        private static string GetGlobBaseDir(string glob)
        {
            var star = glob.IndexOf('*');
            var dir = star < 0 ? glob : glob[..star];
            return dir.TrimEnd('/').TrimEnd('\\');
        }

        public void Dispose()
        {
            foreach (var w in _watchers)
                w.Dispose();
        }

        public void RegisterBundle(string route, string[] sourceGlobs, BundleType type, bool minify)
        {
            _descriptors[route] = new BundleDescriptor(sourceGlobs, type, minify);
            _contentCache.TryRemove(route, out _);
        }

        public bool TryGetOrBuild(string route, out CachedBundle bundle)
        {
            if (_watchersStarted == 0) StartWatchers();

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
