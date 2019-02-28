using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Polly.Proxy.Test
{
    public class SomeRepository
    {
        public string GetInfo1(int id)
        {
            return id.ToString() + 1;
        }
        public string GetInfo2(int id)
        {
            return id.ToString() + 2;
        }
        public string GetInfo3(int id)
        {
            return id.ToString() + 3;
        }
        public string GetInfo4(int id)
        {
            return id.ToString() + 4;
        }
    }


    public interface IClientTest
    {
        string FailFirst(string id);
        Task<string> FailFirstAsync(string id);
        Task FailFirstVoidAsync(string id);

        object NotVirtualFailFirst(string id);
        Task<object> NotVirtualFailFirstAsync(string id);
        Task NotVirtualFailFirstVoidAsync(string id);
    }
    public class ClientTest : IClientTest
    {
        internal bool _fail = true;

        public virtual string FailFirst(string id)
        {
            if (_fail)
            {
                _fail = false;
                throw new ArgumentException("this is a test exception");
            }
            return id;
        }
        public virtual async Task<string> FailFirstAsync(string id)
        {
            if (_fail)
            {
                _fail = false;
                throw new ArgumentException("this is a test exception");
            }
            await Task.Delay(0);
            return id;
        } 
        public virtual async Task FailFirstVoidAsync(string id)
        {
            await Task.Delay(0);
            if (_fail)
            {
                _fail = false;
                throw new ArgumentException("this is a test exception");
            }
        }
        public object NotVirtualFailFirst(string id)
        {
            if (_fail)
            {
                _fail = false;
                throw new ArgumentException("this is a test exception");
            }
            return id;
        }

        public async Task<object> NotVirtualFailFirstAsync(string id)
        {
            await Task.Delay(0);
            if (_fail)
            {
                _fail = false;
                throw new ArgumentException("this is a test exception");
            }
            return id;
        }

        public async Task NotVirtualFailFirstVoidAsync(string id)
        {
            if (_fail)
            {
                _fail = false;
                throw new ArgumentException("this is a test exception");
            }
            await Task.Delay(0);
        }

    }
}
