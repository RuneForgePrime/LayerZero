using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.Services.Bundles
{
    public class BundleCollection
    {
        private readonly HashSet<string> _bundlesCss = new (StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _bundlesJs = new (StringComparer.OrdinalIgnoreCase);


        private string _criticalCss = string.Empty;
        private string _criticalJs = string.Empty;
        private bool _isCacheBustingActive = false;
        private bool _isBulkActive = false;
        private bool _isDevEnv = false;
        private bool _isMinified = false;
        private bool _isCommonCssActive = false;
        private bool _isCommonJsActive = false;

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

        public void SetCriticalCss(string Rules)
        {
            this._criticalCss = Rules;
        }

        public string GetCriticalCss() => this._criticalCss;

        public void SetCriticalJs(string Scripts)
        {
            this._criticalJs  = Scripts;
        }

        public string GetCriticalJs() => this._criticalJs;

        public void SetCacheBusting(bool IsCacheBustingActive)
        {
            this._isCacheBustingActive = IsCacheBustingActive;
        }

        public bool IsCacheBustingActive() => this._isCacheBustingActive;

        public void SetBulkMode(bool IsBulkActive) => _isBulkActive = IsBulkActive;

        public bool IsBulkActive() => this._isBulkActive;

        public HashSet<string> GetAllCss() => this._bundlesCss;

        public HashSet<string?> GetAllJs() => this._bundlesJs;

        public void SetIsDevEnv(bool IsDevEnv) => this._isDevEnv = IsDevEnv;

        public void SetIsMinified(bool IsMinified) => this._isMinified = IsMinified;

        public string GetExtension() => _isDevEnv ? ".dev." : _isMinified ? ".min." : ".";

        public void SetIsCommonCssActive(bool IsActive) => this._isCommonCssActive = IsActive;
        public bool IsCommonCssActive() => this._isCommonCssActive;

        public void SetIsCommonJsActive(bool IsActive) => this._isCommonJsActive = IsActive;
        public bool IsCommonJsActive() => this._isCommonJsActive;
    }
}
