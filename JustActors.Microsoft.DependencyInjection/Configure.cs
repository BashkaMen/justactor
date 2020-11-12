using Microsoft.Extensions.DependencyInjection;

namespace JustActors.Microsoft.DependencyInjection
{
    public static class Configure
    {
        public static void AddJustActors(this IServiceCollection services)
        {
            services.AddTransient<DIBeeResolver>();
            services.AddSingleton<BeeApiary>();
        }
    }
}