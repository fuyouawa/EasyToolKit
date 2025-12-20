namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving drawer chains for <see cref="InspectorProperty"/>
    /// </summary>
    public interface IDrawerChainResolver : IInspectorHandler
    {
        /// <summary>
        /// Gets the drawer chain for the property
        /// </summary>
        /// <returns>The drawer chain</returns>
        DrawerChain GetDrawerChain();
    }
}
