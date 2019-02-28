namespace Polly.Proxy
{
    public interface IPolicyConfig
    {
        void Use(Policy policy);
        void Use(AsyncPolicy policy);
    }
}
