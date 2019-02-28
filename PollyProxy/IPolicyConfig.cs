namespace Polly.Proxy
{
    public interface IPolicyConfig
    {
        /// <summary>
        /// Uses the given sync policy
        /// </summary>
        /// <param name="policy">The policy to use</param>
        void Use(Policy policy);
        /// <summary>
        /// Uses the given async policy
        /// </summary>
        /// <param name="policy">The async policy to use</param>
        void Use(AsyncPolicy policy);
    }
}
