using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Polly.Proxy
{
    internal class ProxyConfig<T> : IProxyConfig<T>
        where T : class
    {
        internal List<Func<MethodInfo, IsPolicy>> _policyMapping = new List<Func<MethodInfo, IsPolicy>>();
        internal IsPolicy _defaultPolicy = null;

        public IProxyConfig<T> For(string methodName, IsPolicy policy)
        {
            _policyMapping.Add(mi => mi.Name == methodName ? policy : null);
            return this;
        }

        public IProxyConfig<T> Map(Func<MethodInfo, IsPolicy> policyBuilder)
        {
            _policyMapping.Add(policyBuilder);
            return this;
        }

        public IProxyConfig<T> For(string methodName, Action<IPolicyConfig> policyConfig)
        {
            var cfg = new PolicyConfig();
            policyConfig.Invoke(cfg);
            _policyMapping.Add(mi => mi.Name == methodName ? cfg._policy : null);
            return this;
        }

        public IProxyConfig<T> For<TResult>(Expression<Func<T, TResult>> methodExpression, IsPolicy policy) 
        {
            var methodCall = methodExpression.Body as MethodCallExpression;
            if (methodCall != null)
            {
                _policyMapping.Add(mi => mi == methodCall.Method ? policy : null);
            }
            return this;
        }

        public IProxyConfig<T> For<TResult>(Expression<Func<T, TResult>> methodExpression, Action<IPolicyConfig> policyConfig) 
        {
            var cfg = new PolicyConfig();
            policyConfig.Invoke(cfg);
            var methodCall = methodExpression.Body as MethodCallExpression;
            if (methodCall != null)
            {
                _policyMapping.Add(mi => mi == methodCall.Method ? cfg._policy : null);
            }
            return this;
        }

        public IProxyConfig<T> For(Expression<Action<T>> methodExpression, IsPolicy policy)
        {
            var methodCall = methodExpression.Body as MethodCallExpression;
            if (methodCall != null)
            {
                _policyMapping.Add(mi => mi == methodCall.Method ? policy : null);
            }
            return this;
        }

        public IProxyConfig<T> For(Expression<Action<T>> methodExpression, Action<IPolicyConfig> policyConfig)
        {
            var cfg = new PolicyConfig();
            policyConfig.Invoke(cfg);
            var methodCall = methodExpression.Body as MethodCallExpression;
            if (methodCall != null)
            {
                _policyMapping.Add(mi => mi == methodCall.Method ? cfg._policy : null);
            }
            return this;

        }

        public IProxyConfig<T> When(Func<MethodInfo, bool> predicate, IsPolicy policy)
        {
            _policyMapping.Add(mi => predicate.Invoke(mi) ? policy : null);
            return this;
        }

        public IProxyConfig<T> When(Func<MethodInfo, bool> predicate, Action<IPolicyConfig> policyConfig)
        {
            var cfg = new PolicyConfig();
            policyConfig.Invoke(cfg);
            _policyMapping.Add(mi => predicate.Invoke(mi) ? cfg._policy : null);
            return this;
        }

        public IProxyConfig<T> Default(IsPolicy policy)
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
