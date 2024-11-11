
using System.Reflection;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Services;

namespace WorkManagementPortal.Backend.API.Extensions
{
    public static class APIRegistration
    {
        public static IServiceCollection APIConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


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

    }
}
