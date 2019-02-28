using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Polly.Proxy.Test
{
    public class YourAttribute : Attribute
    {

    }
public interface IYourInterface
{
    void YourMethod();
    Task YourMethodAsync();
}

    public class YourClass : IYourInterface
    {
        public void YourMethod()
        {
            throw new Exception();
        }
        public async Task YourMethodAsync()
        {
            throw new Exception();

        }
    }
    public class AnotherClass
    {
        private IYourInterface _instance = // <-- Will proxy this instance
            PollyProxy.Create<IYourInterface>(new YourClass(), p => p
                .Default(Policy.Handle<Exception>().Retry(2)));

        public async Task SomeMethod()
        {
            _instance.YourMethod(); // <-- This call will be handled by Polly
                                    // ...
        }
    }

    public class ProxyTests
    {
        [SetUp]
        public void Setup()
        {
            

var proxy = PollyProxy.Create<IYourInterface>(new YourClass(), _ => _
    .For("YourMethod1", Policy.Handle<TimeoutException>().Retry(2))
    .For("YourMethodAsync", Policy.Handle<Exception>().RetryForeverAsync())
    .When(mi => mi.GetCustomAttribute<YourAttribute>() != null, Policy.Timeout(10))
    .Default(Policy.NoOp()));

            
        }

        [Test]
        public void TestProxy_ByInterface_SyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("NotVirtualFailFirst", pol => pol.Use(policy)));
            var ret = proxy.NotVirtualFailFirst("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .When(mi => mi.Name == "NotVirtualFailFirstAsync", policy));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("NotVirtualFailFirstAsync", policy));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_ByInterface_SyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("NotVirtualFailFirst", pol => pol.Use(policy)));
            var ret = proxy.NotVirtualFailFirst("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncVoidMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<IClientTest>(cli, config => config
                .For("NotVirtualFailFirstVoidAsync", policy));
            await proxy.NotVirtualFailFirstVoidAsync("y"); // first fails
            Assert.IsFalse(cli._fail);
        }

        [Test]
        public async Task TestProxy_ByInterface_AsyncVoidMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<IClientTest>(cli, config => config
                .For("NotVirtualFailFirstVoidAsync", policy));
            await proxy.NotVirtualFailFirstVoidAsync("y"); // first fails
            Assert.IsFalse(cli._fail);
        }



        [Test]
        public void TestProxy_ByClass_SyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy.Create<ClientTest>(config => config
                .For("FailFirst", policy));
            var ret = proxy.FailFirst("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByClass_AsyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy.Create<ClientTest>(config => config
                .When(mi => mi.Name == "FailFirstAsync", _ => _.Use(policy)));
            var ret = await proxy.FailFirstAsync("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByClass_AsyncMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var proxy = PollyProxy.Create<ClientTest>(config => config
                .For("FailFirstAsync", policy));
            var ret = await proxy.FailFirstAsync("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_ByClass_SyncMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var proxy = PollyProxy.Create<ClientTest>(config => config
                .For("FailFirst", pol => pol.Use(policy)));
            var ret = proxy.FailFirst("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_ByClass_AsyncVoidMethod_SyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<ClientTest>(cli, config => config
                .When(mi => mi.Name == "FailFirstVoidAsync", policy));
            await proxy.FailFirstVoidAsync("y"); // first fails
            Assert.AreEqual(false, cli._fail);
        }
        [Test]
        public async Task TestProxy_ByClass_AsyncVoidMethod_AsyncPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<ClientTest>(cli, config => config
                .When(mi => mi.Name == "FailFirstVoidAsync", policy));
            await proxy.FailFirstVoidAsync("y"); // first fails
            Assert.AreEqual(false, cli._fail);
        }


        [Test]
        public void TestProxy_Default_SyncMethod()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(1);
            var policyNA = Policy.Handle<NullReferenceException>().Retry(2); 
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("XXXX", policyNA)
                .Default(policy));
            var ret = proxy.NotVirtualFailFirst("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_Default_AsyncMethod()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("XXXX", policyNA)
                .Default(policy));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); // first fails
            Assert.AreEqual("y", ret);
        }

        [Test]
        public async Task TestProxy_NoDefault_AsyncVoidMethod()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<IClientTest>(cli, config => config
                .For("XXXX", policyNA)
                .Default(policy));
            await proxy.NotVirtualFailFirstVoidAsync("y"); // first fails
            Assert.AreEqual(false, cli._fail);
        }

        [Test]
        public void TestProxy_NoPolicy_SyncMethod()
        {
            var policyNA = Policy.Handle<NullReferenceException>().Retry(2); 
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("XXXX", policyNA));
            Assert.Throws<ArgumentException>(() =>
            {
                var ret = proxy.NotVirtualFailFirst("y"); // first fails
            });
        }

        [Test]
        public async Task TestProxy_NoPolicy_AsyncMethod()
        {
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("XXXX", policyNA));
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var ret = await proxy.NotVirtualFailFirstAsync("y"); // first fails
            });
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestProxy_NoPolicy_AsyncVoidMethod()
        {
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2); 
            var proxy = PollyProxy.Create<IClientTest>(new ClientTest(), config => config
                .For("XXXX", policyNA));
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await proxy.NotVirtualFailFirstVoidAsync("y"); // first fails
            });
            await Task.CompletedTask;
        }

        [Test]
        public async Task TestProxy_Map()
        {
            var policy = Policy.Handle<ArgumentException>().RetryAsync(1);
            var policyNA = Policy.Handle<NullReferenceException>().RetryAsync(2);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<IClientTest>(cli, config => config
                .Map(mi => mi.Name == "NotVirtualFailFirstAsync" ? policy : null)
                .Default(policyNA));
            var ret = await proxy.NotVirtualFailFirstAsync("y"); // first fails
            Assert.IsFalse(cli._fail);
            cli._fail = true;
            Assert.Throws<ArgumentException>(() =>
            {
                var ret2 = proxy.NotVirtualFailFirst("z"); // first fails but should not retry because of handling wrong exception
            });
            Assert.AreEqual("y", ret);
        }

        [Test]
        public void TestProxy_NotMatchingPolicy()
        {
            var policy = Policy.Handle<ArgumentException>().Retry(3);
            var cli = new ClientTest();
            var proxy = PollyProxy.Create<IClientTest>(cli, config => config
                .Map(mi => mi.Name == "XXXXX" ? policy : null)
                .When(mi => mi.GetCustomAttribute<IgnoreAttribute>() != null, policy)
                .For("XX", policy)
                .Default(null));
            Assert.Throws<ArgumentException>(() =>
            {
                var ret2 = proxy.NotVirtualFailFirst("z"); // first fails but should not retry because no matching policy
            });
        }

    }
}
