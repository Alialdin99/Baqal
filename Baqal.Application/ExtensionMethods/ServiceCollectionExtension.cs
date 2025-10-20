using Baqal.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace Baqal.Application.ExtensionMethods
{
    public static class ServiceCollectionExtention
    {
        public static IServiceCollection AddService(this IServiceCollection service)
            => service
                .AddScoped<StoreService>()
                .AddScoped<ProductService>()
                .AddScoped<CartService>();
    }
}
