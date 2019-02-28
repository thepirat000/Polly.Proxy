using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Polly.Proxy
{
    /// <summary>
    /// Helper to create a Dynamic Proxies on interfaces methods or virtual methods 
    /// </summary>
    public class PollyProxy
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        /// <summary>
        /// Creates a proxy of a given class/interface instance so the virtual calls can be intercepted and made using a Polly policy.
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="instance">The instance to proxy</param>
        /// <param name="config">The configuration for this instance.</param>
        /// <returns>The proxied instance of T</returns>
        public static T Create<T>(T instance, Action<IProxyConfig> config)
            where T : class
        {
            var cfg = new ProxyConfig();
            config.Invoke(cfg);
            var interceptor = new PollyProxyInterceptorAsync(cfg);
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
        /// <param name="policyBuilder">A function to map virtual methods of T with Polly policies to use. Can return NULL to bypass Polly.</param>
        /// <returns>The proxied new instance of T</returns>
        public static T Create<T>(Action<IProxyConfig> config)
            where T : class, new()
        {
            return Create<T>(new T(), config);
        }
    }
}
