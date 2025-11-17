

namespace LayerZero.Tools.Web.Configuration
{
    public class BundleCollectionConfig
    {
        public string? JsRoot { get; set; } = "js/controller";
        public string? CssRoot { get; set; } = "css/controller";
        public string? CriticalJsRoot { get; set; } = "js/critical";
        public string? CriticalCssRoot { get; set; } = "css/critical";
        public string? CommonCssRoot { get; set; } = "css/Common";
        public string? CommonJsRoot { get; set; } = "js/Common";
        public bool EnableCacheBusting { get; set; } = false;
        public bool EnableBenchmark { get; set; } = false;
        public bool IsEnvironmentDev {  get; set; } = false;
        public bool IsMinified { get; set; } = false;

    }
}
