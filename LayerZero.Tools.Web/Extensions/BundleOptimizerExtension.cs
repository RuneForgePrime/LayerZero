using LayerZero.Tools.Web.Bundles;
using LayerZero.Tools.Web.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
                var store = new BundleStore(env.WebRootPath);
                DynamicBundleMapper.Register(store, Config);
                if (Config.EnableBenchmark)
                    DynamicBundleMapper.RegisterBulk(store, Config);
                return store;
            });

            return Services;
        }
    }
}
