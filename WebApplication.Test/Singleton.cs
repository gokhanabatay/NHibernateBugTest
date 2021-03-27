using System;
using System.Threading;

namespace WebApplication.Test
{
    public class Singleton<T> where T : class
    {
        private Singleton() { }

        private static readonly Lazy<T> instance = new Lazy<T>(CreateInstance, LazyThreadSafetyMode.ExecutionAndPublication);

        private static T CreateInstance()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        public static T Instance => instance.Value;
    }
}
