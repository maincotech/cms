namespace Microsoft.Extensions.DependencyInjection
{
    using Maincotech.Blazor;
    using Maincotech.Blazor.Routing;
    using Maincotech.Cms;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public static class CmsBlazorExtensions
    {
        public static IServiceCollection AddCmsBlazor([NotNull] this IServiceCollection services, [NotNull] Action<IServiceProvider, CmsOptionsBuilder> optionAction)
        {
            var builder = new CmsOptionsBuilder();

            //ViewModels
            services.AddScoped<Maincotech.Cms.Pages.Admin.Category.IndexViewModel>();
            services.AddScoped<Maincotech.Cms.Pages.Admin.Blog.IndexViewModel>();
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
            //   routeManager.RegisterRoutesInAssembly(typeof(CmsBlazorExtensions).Assembly, options.AreaName);

            //register admin routes
            routeManager.RegisterRoute(typeof(Maincotech.Cms.Pages.Admin.Blog.Index), options.AdminAreaName);
            routeManager.RegisterRoute(typeof(Maincotech.Cms.Pages.Admin.Blog.Edit), options.AdminAreaName);
            routeManager.RegisterRoute(typeof(Maincotech.Cms.Pages.Admin.Category.Index), options.AdminAreaName);
            routeManager.RegisterRoute(typeof(Maincotech.Cms.Pages.Admin.Category.Edit), options.AdminAreaName);

            //register user routes
            routeManager.RegisterRoute(typeof(Maincotech.Cms.Pages.Blog.View), options.UserAreaName);
            routeManager.RegisterRoute(typeof(Maincotech.Cms.Pages.Blog.Index), options.UserAreaName);
        }

        private static void RegisterLayout(IServiceProvider serviceProvider, CmsOptions options)
        {
            var layoutProvider = serviceProvider.GetRequiredService<ILayoutProvider>();
            if (options.AdminLayout != null)
            {
                layoutProvider.Register(typeof(Maincotech.Cms.Pages.Admin.Blog.Index).FullName, options.AdminLayout);
                layoutProvider.Register(typeof(Maincotech.Cms.Pages.Admin.Blog.Edit).FullName, options.AdminLayout);
                layoutProvider.Register(typeof(Maincotech.Cms.Pages.Admin.Category.Index).FullName, options.AdminLayout);
                layoutProvider.Register(typeof(Maincotech.Cms.Pages.Admin.Category.Edit).FullName, options.AdminLayout);
            }
            if (options.UserLayout != null)
            {
                layoutProvider.Register(typeof(Maincotech.Cms.Pages.Blog.Index).FullName, options.UserLayout);
                layoutProvider.Register(typeof(Maincotech.Cms.Pages.Blog.View).FullName, options.UserLayout);
            }
            //if (options.Layout != null)
            //{
            //    var layoutProvider = serviceProvider.GetRequiredService<ILayoutProvider>();

            //    var appAssembly = Assembly.GetExecutingAssembly();

            //    var pageComponentTypes = appAssembly
            //        .ExportedTypes
            //        .Where(t => t.Namespace != null && (t.IsSubclassOf(typeof(ComponentBase))
            //                                            && t.Namespace.Contains(".Pages")));

            //    foreach (var pageType in pageComponentTypes)
            //    {
            //        if (pageType.FullName == null)
            //            continue;
            //        layoutProvider.Register(pageType.FullName, options.Layout);
            //    }
            //}
        }
    }
}