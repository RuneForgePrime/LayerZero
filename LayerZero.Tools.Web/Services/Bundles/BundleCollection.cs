
namespace LayerZero.Tools.Web.Services.Bundles
{
    public class BundleCollection
    {
        private readonly HashSet<string> _bundlesCss = new (StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _bundlesJs = new (StringComparer.OrdinalIgnoreCase);

        private bool _isCacheBustingActive = false;
        private bool _isBulkActive = false;
        private bool _isDevEnv = false;
        private bool _isMinified = false;
        private bool _isCommonJsAvailable = false;
        private bool _isCommonCssAvailable = false;
        private bool _isCriticalCssAvailable = false;
        private bool _isCriticalJsAvailable = false;

        public void RegisterJsBundle(string Controller, string? Action = null)
        {
            var path = !string.IsNullOrEmpty(Action) ? $"{Controller}/{Action}" : Controller;
            this._bundlesJs.Add(path);
        }

        public void RegisterCssBundle(string Controller, string? Action = null)
        {
            var path = !string.IsNullOrEmpty(Action) ? $"{Controller}/{Action}" : Controller;
            this._bundlesCss.Add(path);
        }

        public bool IsJsBundleRegistered(string Controller, string? Action = null)
        {
            var path = !string.IsNullOrEmpty(Action) ? $"{Controller}/{Action}" : Controller;
            return this._bundlesJs.Contains(path);
        }

        public bool IsCssBundleRegistered(string Controller, string? Action = null)
        {
            var path = !string.IsNullOrEmpty(Action) ? $"{Controller}/{Action}" : Controller;
            return this._bundlesCss.Contains(path);
        }

        public void SetIsCommonJsAvailable(bool IsCommonJsAvailable) => _isCommonJsAvailable = IsCommonJsAvailable;
        public void SetIsCommonCssAvailable(bool IsCommonCssAvailable) => _isCommonCssAvailable = IsCommonCssAvailable;
        public bool IsCommonJsAvailable() => _isCommonJsAvailable;
        public bool IsCommonCssAvailable() => _isCommonCssAvailable;

        public void SetIsCriticalCssAvailable(bool value) => _isCriticalCssAvailable = value;
        public void SetIsCriticalJsAvailable(bool value) => _isCriticalJsAvailable = value;
        public bool IsCriticalCssAvailable() => _isCriticalCssAvailable;
        public bool IsCriticalJsAvailable() => _isCriticalJsAvailable;

        public void SetCacheBusting(bool IsCacheBustingActive) => this._isCacheBustingActive = IsCacheBustingActive;
        public bool IsCacheBustingActive() => this._isCacheBustingActive;

        public void SetBulkMode(bool IsBulkActive) => _isBulkActive = IsBulkActive;
        public bool IsBulkActive() => this._isBulkActive;

        public HashSet<string> GetAllCss() => this._bundlesCss;
        public HashSet<string> GetAllJs() => this._bundlesJs;

        public void SetIsDevEnv(bool IsDevEnv) => this._isDevEnv = IsDevEnv;
        public void SetIsMinified(bool IsMinified) => this._isMinified = IsMinified;

        public string GetExtension() => _isDevEnv ? ".dev." : _isMinified ? ".min." : ".";
        public bool IsMinified() => this._isMinified;
    }
}
