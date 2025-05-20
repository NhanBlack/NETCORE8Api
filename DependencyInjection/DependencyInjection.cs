using Application.Customer;
using Application;

namespace CleanSolution.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddSingleton<DapperContext>();

            return services;
        }
    }
}
