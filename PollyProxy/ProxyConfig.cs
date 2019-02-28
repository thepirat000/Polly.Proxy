using System;
using System.Collections.Generic;
using System.Reflection;

namespace Polly.Proxy
{
    internal class ProxyConfig : IProxyConfig
    {
        internal List<Func<MethodInfo, IsPolicy>> _policyMapping = new List<Func<MethodInfo, IsPolicy>>();
        internal IsPolicy _defaultPolicy = null;

        public IProxyConfig For(string methodName, IsPolicy policy)
        {
            _policyMapping.Add(mi => mi.Name == methodName ? policy : null);
            return this;
        }

        public IProxyConfig Map(Func<MethodInfo, IsPolicy> policyBuilder)
        {
            _policyMapping.Add(policyBuilder);
            return this;
        }

        public IProxyConfig For(string methodName, Action<IPolicyConfig> policyConfig)
        {
            var cfg = new PolicyConfig();
            policyConfig.Invoke(cfg);
            _policyMapping.Add(mi => mi.Name == methodName ? cfg._policy : null);
            return this;
        }

        public IProxyConfig When(Func<MethodInfo, bool> predicate, IsPolicy policy)
        {
            _policyMapping.Add(mi => predicate.Invoke(mi) ? policy : null);
            return this;
        }

        public IProxyConfig When(Func<MethodInfo, bool> predicate, Action<IPolicyConfig> policyConfig)
        {
            var cfg = new PolicyConfig();
            policyConfig.Invoke(cfg);
            _policyMapping.Add(mi => predicate.Invoke(mi) ? cfg._policy : null);
            return this;
        }

        public IProxyConfig Default(IsPolicy policy)
        {
            _defaultPolicy = policy;
            return this;
        }

        public IsPolicy GetPolicy(MethodInfo methodInfo)
        {
            foreach(var map in _policyMapping)
            {
                var policy = map.Invoke(methodInfo);
                if (policy != null)
                {
                    return policy;
                }
            }
            return _defaultPolicy;
        }
    }
}
