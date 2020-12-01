namespace Microsoft.Extensions.DependencyInjection
{
    using Maincotech.Blazor;
    using Maincotech.Blazor.Routing;
    using Maincotech.Cms;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    public static class CmsBlazorExtensions
    {
        public static IServiceCollection AddCmsBlazor([NotNull] this IServiceCollection services, [NotNull] Action<IServiceProvider, CmsOptionsBuilder> optionAction)
        {
            var builder = new CmsOptionsBuilder();

            //ViewModels
            services.AddScoped<Maincotech.Cms.Pages.Category.CategoryViewModel>();
            services.AddScoped<Maincotech.Cms.Pages.Category.IndexViewModel>();
            services.AddScoped<Maincotech.Cms.Pages.Blog.IndexViewModel>();

            //We need add routes and layouts immediately. So we create service provider here.
            using var serviceProvider = services.BuildServiceProvider();
            optionAction.Invoke(serviceProvider, builder);
            services.AddSingleton(builder.Options);

            RegisterLayout(serviceProvider, builder.Options);
            RegisterRoutes(serviceProvider, builder.Options);
            return services;
        }

        private static void RegisterRoutes(IServiceProvider serviceProvider, CmsOptions options)
        {
            var routeManager = serviceProvider.GetRequiredService<RouteManager>();
            routeManager.RegisterRoutesInAssembly(typeof(CmsBlazorExtensions).Assembly, options.AreaName);
        }

        private static void RegisterLayout(IServiceProvider serviceProvider, CmsOptions options)
        {
            if (options.Layout != null)
            {
                var layoutProvider = serviceProvider.GetRequiredService<ILayoutProvider>();

                var appAssembly = Assembly.GetExecutingAssembly();

                var pageComponentTypes = appAssembly
                    .ExportedTypes
                    .Where(t => t.Namespace != null && (t.IsSubclassOf(typeof(ComponentBase))
                                                        && t.Namespace.Contains(".Pages")));

                foreach (var pageType in pageComponentTypes)
                {
                    if (pageType.FullName == null)
                        continue;
                    layoutProvider.Register(pageType.FullName, options.Layout);
                }
            }
        }
    }
}