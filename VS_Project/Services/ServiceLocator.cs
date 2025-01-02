
using System;
using System.Collections.Generic;
using Sand_Breaker;

namespace Sand_Breaker.Services
{
    public static class ServiceLocator
    {

        private static Dictionary<Type, object> serviceMap = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            if (serviceMap.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Service of type {typeof(T)} already registered");
            serviceMap[typeof(T)] = service;
        }

        public static T Get<T>()
        {
            if (!serviceMap.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Service of type {typeof(T)} does not exist");
            else
                return (T)serviceMap[typeof(T)];
        }
    }
}
