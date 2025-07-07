using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.Configuration
{
    public class BundleCollectionConfig
    {
        public string JsRoot { get; set; } = "js/controller";
        public string CssRoot { get; set; } = "css/controller";
        public string CriticalJsRoot { get; set; } = "js/critical";
        public string CriticalCssRoot { get; set; } = "css/critical";
        public bool EnableCacheBusting { get; set; } = false;
        public bool EnableBenchmark { get; set; } = false;
    }
}
