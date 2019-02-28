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
        /// Specifies a policy to be used under certain condition on the Method being called.
        /// </summary>
        /// <param name="selector">A function of the MethodInfo that indicates a condition for matching this rule</param>
        /// <param name="policy">The policy to use when the selector condition is satisfied</param>
        IProxyConfig When(Func<MethodInfo, bool> selector, IsPolicy policy);
        /// <summary>
        /// Specifies a policy to be used under certain condition on the Method being called.
        /// </summary>
        /// <param name="selector">A function of the MethodInfo that indicates a condition for matching this rule</param>
        /// <param name="policyConfig">The policy configuration to use when the selector condition is satisfied</param>
        IProxyConfig When(Func<MethodInfo, bool> selector, Action<IPolicyConfig> policyConfig);
        /// <summary>
        /// Maps a policy for a method by its name.
        /// </summary>
        /// <param name="methodName">The method name for matching this rule (case-sensitive)</param>
        /// <param name="policy">The policy to use</param>
        IProxyConfig For(string methodName, IsPolicy policy);
        /// <summary>
        /// Maps a policy for a method by its name.
        /// </summary>
        /// <param name="methodName">The method name for matching this rule (case-sensitive)</param>
        /// <param name="policyConfig">The policy configuration</param>
        IProxyConfig For(string methodName, Action<IPolicyConfig> policyConfig);
        /// <summary>
        /// Defines a default Policy to use for any other method not mapped by other rules.
        /// </summary>
        /// <param name="policy">The policy to use as default</param>
        IProxyConfig Default(IsPolicy policy);
        /// <summary>
        /// Specifies the policies to be used as a function of the MethodInfo being called that returns the policy to use (or NULL).
        /// </summary>
        /// <param name="policyBuilder">A function of the MethodInfo that returns the policy to use. Can return NULL to indicate no-match.</param>
        /// <returns></returns>
        IProxyConfig Map(Func<MethodInfo, IsPolicy> policyBuilder);
    }
}
