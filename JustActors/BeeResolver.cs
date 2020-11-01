using System;

namespace JustActors
{
    public interface IBeeResolver
    {
        T Resolve<T>() where T : IBee;
    }

    public class DefaultBeeResolver : IBeeResolver
    {
        public T Resolve<T>() where T : IBee
            => Activator.CreateInstance<T>();
    }
}