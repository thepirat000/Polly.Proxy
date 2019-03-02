using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Polly.Proxy
{
    /// <summary>
    /// Helper to create a Dynamic Proxies on interfaces methods or virtual methods 
    /// </summary>
    public class PollyProxy<T>
        where T : class
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        /// <summary>
        /// Creates a proxy of a given class/interface instance so the virtual calls can be intercepted and made using a Polly policy.
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="instance">The instance to proxy</param>
        /// <param name="config">The configuration for this instance.</param>
        /// <returns>The proxied instance of T</returns>
        public static T Create(T instance, Action<IProxyConfig<T>> config)
        {
            var cfg = new ProxyConfig<T>();
            config.Invoke(cfg);
            var interceptor = new PollyProxyInterceptorAsync<T>(cfg);
            if (typeof(T).GetTypeInfo().IsInterface)
            {
                return Generator.CreateInterfaceProxyWithTarget<T>(instance, new[] { interceptor });
            }
            else
            {
                return Generator.CreateClassProxyWithTarget<T>(instance, new[] { interceptor });
            }
        }

        /// <summary>
        /// Creates a proxy of a given class/interface so its virtual calls can be intercepted and made using a Polly policy.
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="config">The configuration for this instance.</param>
        /// <returns>The proxied new instance of T</returns>
        public static T Create(Action<IProxyConfig<T>> config)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            return Create(instance, config);
        }
    }
}
