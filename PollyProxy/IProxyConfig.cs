using System;
using System.Reflection;

namespace Polly.Proxy
{
    /// <summary>
    /// Fluent API to define the Polly policies for a proxied instance
    /// </summary>
    public interface IProxyConfig
    {
        /// <summary>
        /// Maps a policy to 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        IProxyConfig When(Func<MethodInfo, bool> selector, IsPolicy policy);
        IProxyConfig When(Func<MethodInfo, bool> selector, Action<IPolicyConfig> policyConfig);

        IProxyConfig For(string methodName, IsPolicy policy);
        IProxyConfig For(string methodName, Action<IPolicyConfig> policyConfig);

        IProxyConfig Default(IsPolicy policy);

        IProxyConfig Map(Func<MethodInfo, IsPolicy> policyBuilder);
        
    }
}
