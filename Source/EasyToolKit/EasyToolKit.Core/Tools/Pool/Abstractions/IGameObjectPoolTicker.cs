namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a mechanism for updating GameObject pools at regular intervals.
    /// </summary>
    public interface IGameObjectPoolTicker
    {
        /// <summary>
        /// Updates all managed pools, processing object lifetimes and recycling.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update (in seconds).</param>
        void Tick(float deltaTime);
    }
}
