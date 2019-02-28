namespace Polly.Proxy
{
    internal class PolicyConfig : IPolicyConfig
    {
        internal IsPolicy _policy = null;

        public void Use(Policy policy)
        {
            _policy = policy;
        }

        public void Use(AsyncPolicy policy)
        {
            _policy = policy;
        }
    }
}
