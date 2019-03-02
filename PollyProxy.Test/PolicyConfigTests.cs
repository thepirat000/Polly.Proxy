using NUnit.Framework;
using System;

namespace Polly.Proxy.Test
{
    public class PolicyConfigTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestConfig_General()
        {
            var policy1 = Policy.Handle<Exception>().Retry(2);
            var policy2 = Policy.Handle<Exception>().Retry(4);
            var policy3 = Policy.Handle<Exception>().Retry(6);

            var cfg = new ProxyConfig<object>();
            cfg.Default(policy3);
            cfg.For("GetInfo1", policy1);
            cfg.When(mi => mi.Name == "GetInfo2", policy2);

            var m1 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo1"));
            var m2 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo2"));
            var m3 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo3"));

            Assert.AreEqual(policy1, m1);
            Assert.AreEqual(policy2, m2);
            Assert.AreEqual(policy3, m3);
        }

        [Test]
        public void TestConfig_NoDefault()
        {
            var policy1 = Policy.Handle<Exception>().Retry(2);
            var policy2 = Policy.Handle<Exception>().Retry(4);

            var cfg = new ProxyConfig<object>();
            cfg.For("GetInfo1", policy1);
            cfg.When(mi => mi.Name == "GetInfo2", policy2);
            cfg.Map(mi => mi.Name == "GetInfo3" ? policy1 : null);

            var m1 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo1"));
            var m2 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo2"));
            var m3 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo3"));
            var m4 = cfg.GetPolicy(typeof(SomeRepository).GetMethod("GetInfo4"));

            Assert.AreEqual(policy1, m1);
            Assert.AreEqual(policy2, m2);
            Assert.AreEqual(policy1, m3);
            Assert.IsNull(m4);
        }
    }

}