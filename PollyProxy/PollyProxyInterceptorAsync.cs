using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Polly.Proxy
{
    /// <summary>
    /// Castle core interceptor using Castle.Core.AsyncInterceptor extension
    /// </summary>
    internal class PollyProxyInterceptorAsync<T> : IAsyncInterceptor
        where T : class
    {
        private ProxyConfig<T> _config;

        public PollyProxyInterceptorAsync(ProxyConfig<T> config)
        {
            _config = config;
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            var policy = _config.GetPolicy(invocation.Method);
            if (policy == null)
            {
                invocation.Proceed();
            }
            else
            {
                if (policy is Policy pol)
                {
                    // sync policy and sync method
                    pol.Execute(() => invocation.Proceed());
                }
                else if (policy is AsyncPolicy asyncPol)
                {
                    // async policy for a sync method call
                    asyncPol.ExecuteAsync(async () => { invocation.Proceed(); await Task.FromResult(false); })
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            var policy = _config.GetPolicy(invocation.Method);
            if (policy == null)
            {
                invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
            }
            else
            {
                if (policy is Policy pol)
                {
                    // sync policy for an async method call
                    pol.Execute(() =>
                    {
                        var task = InternalInterceptAsynchronous(invocation);
                        if (task.IsFaulted)
                        {
                            throw task.Exception.InnerException ?? task.Exception;
                        }
                        invocation.ReturnValue = task;
                    });
                }
                else if (policy is AsyncPolicy asyncPol)
                {
                    // async policy and async method
                    asyncPol
                        .ExecuteAsync(() => {
                            var result = InternalInterceptAsynchronous(invocation);
                            invocation.ReturnValue = result;
                            return result;
                        })
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var policy = _config.GetPolicy(invocation.Method);
            if (policy == null)
            {
                invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
            }
            else
            {
                if (policy is Policy pol)
                {
                    // sync policy for an async method call
                    pol.Execute(() => 
                    {
                        var task = InternalInterceptAsynchronous<TResult>(invocation);
                        if (task.IsFaulted)
                        {
                            throw task.Exception.InnerException ?? task.Exception;
                        }
                        invocation.ReturnValue = task;
                    });
                }
                else if (policy is AsyncPolicy asyncPol)
                {
                    // async policy and async method
                    asyncPol
                        .ExecuteAsync(() => {
                            var result = InternalInterceptAsynchronous<TResult>(invocation);
                            invocation.ReturnValue = result;
                            return result;
                        })
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            TResult result = await task;
            return result;
        }

    }
}
