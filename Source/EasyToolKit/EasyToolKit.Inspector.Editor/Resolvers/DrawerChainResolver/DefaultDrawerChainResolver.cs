using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Default implementation of drawer chain resolver for <see cref="InspectorProperty"/>
    /// </summary>
    public class DefaultDrawerChainResolver : DrawerChainResolverBase
    {
        private DrawerChain _chain;

        protected override void Initialize()
        {
            // Get default property drawer types for the property
            var drawerTypes = EasyDrawerUtility.GetDrawerTypes(Element);
            var drawers = new List<IEasyDrawer>();

            // Create and initialize drawer instances
            foreach (var drawerType in drawerTypes)
            {
                var drawer = drawerType.CreateInstance<IEasyDrawer>();
                drawers.Add(drawer);
            }

            // Create and cache the drawer chain
            _chain = new DrawerChain(Element, drawers);
        }

        /// <summary>
        /// Gets the drawer chain for the property by discovering and initializing appropriate drawers
        /// </summary>
        /// <returns>The drawer chain</returns>
        protected override DrawerChain GetDrawerChain()
        {
            return _chain;
        }

        /// <summary>
        /// Clears the cached drawer chain when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _chain = null;
        }
    }
}
