namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving post processor chains for <see cref="IElement"/>
    /// </summary>
    public interface IPostProcessorChainResolver : IResolver
    {
        /// <summary>
        /// Gets the post processor chain for the element
        /// </summary>
        /// <returns>The post processor chain</returns>
        PostProcessorChain GetPostProcessorChain();
    }
}
