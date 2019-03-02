using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Polly.Proxy.Test
{
    public class ProxyTests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void TestProxy_ByInterface_ForExpression()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(2);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For(_ => _.NotVirtualFailFirst(null), policy));
            var ret = proxy.NotVirtualFailFirst("y");
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_ByClass_ForExpression()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(2);
            var proxy = PollyProxy<ClientTest>.Create(config => config
                .For(_ => _.FailFirst(null), _ => _.Use(policy)));
            var ret = proxy.FailFirst("y");
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_ByInterface_ForExpressionVoid()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(2);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For(_ => _.FailFirstVoid(""), policy));
            proxy.FailFirstVoid("y");
            Assert.Pass();
        }

        [Test]
        public void TestProxy_ByClass_ForExpressionVoid()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(2);
            var proxy = PollyProxy<ClientTest>.Create(config => config
                .For(_ => _.FailFirstVoid(null), _ => _.Use(policy)));
            proxy.FailFirstVoid("y");
            Assert.Pass();
        }

        [Test]
        public void TestProxy_ByInterface_SyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("NotVirtualFailFirst", pol => pol.Use(policy)));
            var ret = proxy.NotVirtualFailFirst("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .When(mi => mi.Name == "NotVirtualFailFirstAsync", policy));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("NotVirtualFailFirstAsync", policy));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_ByInterface_SyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("NotVirtualFailFirst", pol => pol.Use(policy)));
            var ret = proxy.NotVirtualFailFirst("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncVoidMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var cli = new ClientTest();
            var proxy = PollyProxy<IClientTest>.Create(cli, config => config
                .For("NotVirtualFailFirstVoidAsync", policy));
            await proxy.NotVirtualFailFirstVoidAsync("y"); 
            Assert.IsFalse(cli._fail);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncVoidMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var cli = new ClientTest();
            var proxy = PollyProxy<IClientTest>.Create(cli, config => config
                .For("NotVirtualFailFirstVoidAsync", policy));
            await proxy.NotVirtualFailFirstVoidAsync("y"); 
            Assert.IsFalse(cli._fail);
        }



        [Test]
        public void TestProxy_ByClass_SyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy<ClientTest>.Create(config => config
                .For("FailFirst", policy));
            var ret = proxy.FailFirst("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByClass_AsyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy<ClientTest>.Create(config => config
                .When(mi => mi.Name == "FailFirstAsync", _ => _.Use(policy)));
            var ret = await proxy.FailFirstAsync("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByClass_AsyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy<ClientTest>.Create(config => config
                .For("FailFirstAsync", policy));
            var ret = await proxy.FailFirstAsync("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_ByClass_SyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy<ClientTest>.Create(config => config
                .For("FailFirst", pol => pol.Use(policy)));
            var ret = proxy.FailFirst("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByClass_AsyncVoidMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var cli = new ClientTest();
            var proxy = PollyProxy<ClientTest>.Create(cli, config => config
                .When(mi => mi.Name == "FailFirstVoidAsync", policy));
            await proxy.FailFirstVoidAsync("y"); 
            Assert.AreEqual(false, cli._fail);
        }
        [Test]
        public async Task TestProxy_ByClass_AsyncVoidMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var cli = new ClientTest();
            var proxy = PollyProxy<ClientTest>.Create(cli, config => config
                .When(mi => mi.Name == "FailFirstVoidAsync", policy));
            await proxy.FailFirstVoidAsync("y"); 
            Assert.AreEqual(false, cli._fail);
        }


        [Test]
        public void TestProxy_Default_SyncMethod()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var policyNA = Policy.Handle<NullReferenceException>().Retry(2); 
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("XXXX", policyNA)
                .Default(policy));
            var ret = proxy.NotVirtualFailFirst("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_Default_AsyncMethod()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("XXXX", policyNA)
                .Default(policy));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_NoDefault_AsyncVoidMethod()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var cli = new ClientTest();
            var proxy = PollyProxy<IClientTest>.Create(cli, config => config
                .For("XXXX", policyNA)
                .Default(policy));
            await proxy.NotVirtualFailFirstVoidAsync("y"); 
            Assert.AreEqual(false, cli._fail);
        }

        [Test]
        public void TestProxy_NoPolicy_SyncMethod()
        {
            var policyNA = Policy.Handle<NullReferenceException>().Retry(2); 
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("XXXX", policyNA));
            Assert.Throws<ArgumentException>(() =>
            {
                var ret = proxy.NotVirtualFailFirst("y"); 
            });
        }

        [Test]
        public async Task TestProxy_NoPolicy_AsyncMethod()
        {
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("XXXX", policyNA));
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var ret = await proxy.NotVirtualFailFirstAsync("y"); 
            });
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestProxy_NoPolicy_AsyncVoidMethod()
        {
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2); 
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .For("XXXX", policyNA));
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await proxy.NotVirtualFailFirstVoidAsync("y"); 
            });
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestProxy_MapRule()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var cli = new ClientTest();
            var proxy = PollyProxy<IClientTest>.Create(cli, config => config
                .Map(mi => mi.Name == "NotVirtualFailFirstAsync" ? policy : null)
                .Default(policyNA));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); 
            Assert.IsFalse(cli._fail);
            cli._fail = true;
            Assert.Throws<ArgumentException>(() =>
            {
                var ret2 = proxy.NotVirtualFailFirst("z");  // should not retry because of handling wrong exception
            });
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_NotMatchingPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(3);
            var cli = new ClientTest();
            var proxy = PollyProxy<IClientTest>.Create(cli, config => config
                .Map(mi => mi.Name == "XXXXX" ? policy : null)
                .When(mi => mi.GetCustomAttribute<IgnoreAttribute>() != null, policy)
                .For("XX", policy)
                .Default(null));
            Assert.Throws<ArgumentException>(() =>
            {
                var ret2 = proxy.NotVirtualFailFirst("z");  // should not retry because no matching policy
            });
        }

        [Test]
        public void TestProxy_CustomAttributeRule()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(2);
            var proxy = PollyProxy<IClientTest>.Create(new ClientTest(), config => config
                .When(mi => mi.GetCustomAttribute<MarkAttribute>(true) != null, policy));
            var ret = proxy.FailFirst("y"); 
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_AsyncBy_AsyncStateMachineAttribute()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var cli = new ClientTest();
            var proxy = PollyProxy<ClientTest>.Create(cli, config => config
                .When(mi => mi.GetCustomAttribute<AsyncStateMachineAttribute>() != null, policy));
            var ret = await proxy.FailFirstAsync("y"); 
            cli._fail = true;
            await proxy.FailFirstVoidAsync("y"); 
            cli._fail = true;
            Assert.Throws<ArgumentException>(() =>
            {
                proxy.FailFirst("y");
            });
            Assert.AreEqual("y", ret);
        }

    }
}
