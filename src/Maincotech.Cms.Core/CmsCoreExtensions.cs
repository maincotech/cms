namespace Microsoft.Extensions.DependencyInjection
{
    using Maincotech.Caching;
    using Maincotech.Cms;
    using Maincotech.Cms.Services;
    using Maincotech.Localization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public static class CmsCoreExtensions
    {
        public static IServiceCollection AddLocalizationCore(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("cmsDb");
            if (connectionString.IsNullOrEmpty())
            {
                throw new NotSupportedException("ResourceCore.ConnectionStringNotConfigured");
            }

            //add database context
            services.AddDbContext<CmsDbContext>(options => options.UseSqlServer(connectionString));

            //add repositories

            //add service
            //add service
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();

            //Setup database
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
            dbContext.EnsureCreated();

            return services;
        }

        public static IServiceCollection AddCmsCore([NotNull] this IServiceCollection services, [NotNull] Action<DbContextOptionsBuilder> optionsAction, ServiceLifetime dbContextLifetime = ServiceLifetime.Transient)
        {
            //add database context
            services.AddDbContext<CmsDbContext>(optionsAction, dbContextLifetime);

            //add repositories

            //add service
            services.AddScoped<IAdminService, AdminService>(sp =>
            {
                var dbContext = sp.GetRequiredService<CmsDbContext>();
                var cachedManager = sp.GetService<ICacheManager>();
                return new AdminService(dbContext, cachedManager);
            });

            services.AddScoped<IUserService, UserService>(sp =>
            {
                var dbContext = sp.GetRequiredService<CmsDbContext>();
                var cachedManager = sp.GetService<ICacheManager>();
                var localizationService = sp.GetService<ILocalizationService>();
                return new UserService(dbContext, localizationService, cachedManager);
            });

            //Setup database
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
            dbContext.EnsureCreated();

            return services;
        }
    }
}