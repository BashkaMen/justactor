using System;
using System.Collections.Concurrent;

namespace JustActors
{
    public class BeeApiary
    {
        private readonly IBeeResolver _beeResolver;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, IBee>> _state;
        
        public BeeApiary(IBeeResolver beeResolver)
        {
            _beeResolver = beeResolver;
            _state = new ConcurrentDictionary<Type, ConcurrentDictionary<string, IBee>>();
        }

        public void RegisterBee<T>(string id, T bee) where T : IBee
        {
            _state.AddOrUpdate(typeof(T), t =>
            {
                var dict = new ConcurrentDictionary<string, IBee> {[id] = bee};
                return dict;
            }, (t, v) =>
            {
                v[id] = bee;
                return v;
            });
        }

        public T GetBee<T>(string id) where T : IBee
        {
            if (_state.TryGetValue(typeof(T), out var bees))
                if (bees.TryGetValue(id, out var bee))
                    return (T)bee;


            var newBee = _beeResolver.Resolve<T>();
            RegisterBee(id, newBee);

            return newBee;
        }
    }
}