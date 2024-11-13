﻿
using System.Reflection;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Services;

namespace WorkManagementPortal.Backend.API.Extensions
{
    public static class APIRegistration
    {
        public static IServiceCollection APIConfiguration(this IServiceCollection services)
        {
            //Configure Auto Mapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISeedData, SeedData>();

            //Configure and Enable CORS
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
            return services;
        }
        public static async void RolesSeedingConfiguration(this IApplicationBuilder app)
        {
            // Run role seeding asynchronously before starting the application
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var seedData = serviceProvider.GetRequiredService<ISeedData>();
                await seedData.SeedRoles(serviceProvider);  // Seed roles before app starts
            }
        }

    }
}