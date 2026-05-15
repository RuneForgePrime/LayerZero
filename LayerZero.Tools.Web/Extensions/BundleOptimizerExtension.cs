using LayerZero.Tools.Web.Bundles;
using LayerZero.Tools.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LayerZero.Tools.Web.Extensions
{
    public static class BundleOptimizerExtension
    {
        public static IServiceCollection AddDynamicBundle(this IServiceCollection Services, BundleCollectionConfig Config)
        {
            Services.AddSingleton(DynamicBundleMapper._bundles);

            var store = new BundleStore();
            DynamicBundleMapper.Register(store, Config);

            if (Config.EnableBenchmark)
                DynamicBundleMapper.RegisterBulk(store, Config);

            Services.AddSingleton(store);

            return Services;
        }
    }
}
