using LayerZero.Tools.Web.Bundles;
using LayerZero.Tools.Web.Configuration;
using LayerZero.Tools.Web.Services.Bundles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LayerZero.Tools.Web.Extensions
{
    public static class BundleOptimizerExtension
    {
        public static IServiceCollection AddDynamicBundle(this IServiceCollection Services, BundleCollectionConfig Config)
        {
            var bundles = new BundleCollection();
            Services.AddSingleton(bundles);

            Services.AddSingleton(sp =>
            {
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                var logger = sp.GetService<ILogger<BundleStore>>();
                var store = new BundleStore(env.WebRootPath, logger);
                DynamicBundleMapper.Register(store, bundles, Config, env.WebRootPath);
                if (Config.EnableBenchmark)
                    DynamicBundleMapper.RegisterBulk(store, bundles, Config, env.WebRootPath);
                store.StartWatchers();
                return store;
            });

            return Services;
        }
    }
}
