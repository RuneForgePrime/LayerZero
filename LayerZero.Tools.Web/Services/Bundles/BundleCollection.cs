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
        private bool _isDev = false;

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

        public void SetEnv(bool IsDevelopment)
        {
            this._isDev = IsDevelopment;
        }


        public bool IsDev() => this._isDev;
    }
}
