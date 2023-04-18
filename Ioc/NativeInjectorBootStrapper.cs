using WhatsappAPI.Controllers;
using WhatsappAPI.Ioc;

namespace WhatsappAPI.Ioc
{
    public class NativeInjectorBootStrapper
    {
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IBrowserController, BrowserController>();
            services.AddTransient<IAuthController, AuthController>();

            return services;
        }
    }
    

    public static class NativeInjectorBootStrapperExtensions
    {
        public static IServiceCollection NativeInjector(this IServiceCollection services)
        {
            return NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}
