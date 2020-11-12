using System;
using Microsoft.Extensions.DependencyInjection;

namespace JustActors.Microsoft.DependencyInjection
{
    public class DIBeeResolver : IBeeResolver
    {
        private readonly IServiceProvider _provider;

        public DIBeeResolver(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        public T Resolve<T>() where T : IBee
        {
            return _provider.GetRequiredService<T>();
        }
    }
}