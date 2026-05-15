using LayerZero.Tools.Web.Bundles;
using LayerZero.Tools.Web.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LayerZero.Tools.Web.Extensions
{
    public static class BundleOptimizerExtension
    {
        public static IServiceCollection AddDynamicBundle(this IServiceCollection Services, BundleCollectionConfig Config)
        {
            Services.AddSingleton(DynamicBundleMapper._bundles);

            Services.AddSingleton(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                var logger = sp.GetService<ILogger<BundleStore>>();
                var store = new BundleStore(env.WebRootPath, logger);
                DynamicBundleMapper.Register(store, Config);
                if (Config.EnableBenchmark)
                    DynamicBundleMapper.RegisterBulk(store, Config);
                return store;
            });

            return Services;
        }
    }
}
