using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for group property resolver in the <see cref="InspectorProperty"/> system
    /// </summary>
    public abstract class GroupResolverBase : InspectorResolverBase, IGroupResolver
    {
        private bool _isInitialized;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Gets all properties that belong to the same group as the current property
        /// </summary>
        /// <param name="beginGroupAttributeType">The type of the begin group attribute</param>
        /// <returns>Array of properties in the group</returns>
        protected abstract InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        InspectorProperty[] IGroupResolver.GetGroupProperties(Type beginGroupAttributeType)
        {
            EnsureInitialize();
            return GetGroupProperties(beginGroupAttributeType);
        }
    }
}
