using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for drawer chain resolution in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class DrawerChainResolverBase : InspectorResolverBase, IDrawerChainResolver
    {
        /// <summary>
        /// Gets the drawer chain for the property
        /// </summary>
        /// <returns>The drawer chain</returns>
        public abstract DrawerChain GetDrawerChain();
    }
}
