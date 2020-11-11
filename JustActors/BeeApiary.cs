using System;
using System.Collections.Concurrent;

namespace JustActors
{
    public class BeeApiary
    {
        private readonly IBeeResolver _beeResolver;
        private readonly ConcurrentDictionary<(Type, string), IBee> _state;
        
        public BeeApiary(IBeeResolver beeResolver)
        {
            _beeResolver = beeResolver;
            _state = new ConcurrentDictionary<(Type, string), IBee>();
        }

        public void RegisterBee<T>(string id, T bee) where T : IBee
        {
            _state.AddOrUpdate((typeof(T), id), bee, (tuple, bee1) => bee1);
        }

        public T GetBee<T>(string id) where T : IBee
        {
            if (_state.TryGetValue((typeof(T), id), out var bee))
                return (T)bee;
            
            var newBee = _beeResolver.Resolve<T>();
            RegisterBee(id, newBee);

            return newBee;
        }
    }
}