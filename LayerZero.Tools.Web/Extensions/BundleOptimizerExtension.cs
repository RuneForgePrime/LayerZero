using LayerZero.Tools.Web.Bundles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.Extensions
{
    public static class BundleOptimizerExtension
    {
        public static IServiceCollection AddDynamicBundle(this IServiceCollection Services, IWebHostEnvironment? Env = null) {

            Services.AddSingleton(DynamicBundleMapper._bundles);

            Services.AddWebOptimizer(pipeline =>
            {
                DynamicBundleMapper.Register(pipeline, isDevelopment: Env?.IsDevelopment() ?? false);
            });

            return Services;
        }
    }
}
