using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JustActors
{
    public static class CommonExt
    {
        private static Random _random = new Random();
        
        public static T DeepCopy<T>(this T item)
        {
            var json = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(json);
        }


        public static T Random<T>(this T[] source)
        {
            return source[_random.Next(source.Length)];
        }

        public static void SwarmPost<T>(this AbstractBee<T>[] source, T message) 
            => source.Random().Post(message);
    }
}