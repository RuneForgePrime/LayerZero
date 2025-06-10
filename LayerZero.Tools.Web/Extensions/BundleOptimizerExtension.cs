using LayerZero.Tools.Web.Bundles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.Extensions
{
    public static class BundleOptimizerExtension
    {
        public static IServiceCollection AddDynamicBundle(this IServiceCollection Services) {

            Services.AddSingleton(DynamicBundleMapper._bundles);

            Services.AddWebOptimizer(pipeline =>
            {
                DynamicBundleMapper.Register(pipeline);
            });

            return Services;
        }
    }
}
