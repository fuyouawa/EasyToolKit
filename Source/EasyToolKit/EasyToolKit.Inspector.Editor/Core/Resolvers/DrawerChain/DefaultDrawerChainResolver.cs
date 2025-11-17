using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Default implementation of drawer chain resolver for <see cref="InspectorProperty"/>
    /// </summary>
    public class DefaultDrawerChainResolver : DrawerChainResolver
    {
        private DrawerChain _chain;

        /// <summary>
        /// Gets the drawer chain for the property by discovering and initializing appropriate drawers
        /// </summary>
        /// <returns>The drawer chain</returns>
        public override DrawerChain GetDrawerChain()
        {
            // Return cached chain if available
            if (_chain != null)
            {
                return _chain;
            }

            // Get default property drawer types for the property
            var drawerTypeResults = InspectorDrawerUtility.GetDefaultPropertyDrawerTypes(Property);
            var drawers = new List<IEasyDrawer>();

            // Create and initialize drawer instances
            foreach (var drawerType in drawerTypeResults.Select(result => result.MatchedType).Distinct())
            {
                var drawer = drawerType.CreateInstance<IEasyDrawer>();
                drawer.Property = Property;
                drawer.Initialize();
                drawers.Add(drawer);
            }

            // Create and cache the drawer chain
            _chain = new DrawerChain(Property, drawers);
            return _chain;
        }

        /// <summary>
        /// Deinitializes the resolver by clearing the cached drawer chain
        /// </summary>
        protected override void Deinitialize()
        {
            _chain = null;
        }
    }
}
