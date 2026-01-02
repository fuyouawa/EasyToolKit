using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class AttributeResolverBase : ResolverBase, IAttributeResolver
    {
        private bool _isInitialized;
        private Dictionary<Attribute, ElementAttributeInfo> _attributeInfoCache;

        /// <summary>
        /// Override this method to perform initialization logic
        /// </summary>
        protected virtual void Initialize()
        {
        }

        protected abstract ElementAttributeInfo[] GetAttributeInfos();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                BuildAttributeCache();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Builds a cache dictionary for fast attribute lookup.
        /// </summary>
        private void BuildAttributeCache()
        {
            var attributeInfos = GetAttributeInfos();
            if (attributeInfos.Length == 0)
            {
                _attributeInfoCache = null;
                return;
            }

            _attributeInfoCache = new Dictionary<Attribute, ElementAttributeInfo>(attributeInfos.Length);
            foreach (var attributeInfo in attributeInfos)
            {
                _attributeInfoCache[attributeInfo.Attribute] = attributeInfo;
            }
        }

        /// <summary>
        /// Tries to get the attribute info for the specified attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute to find.</param>
        /// <param name="attributeInfo">When this method returns, contains the attribute info if found; otherwise, null.</param>
        /// <returns>true if the attribute info was found; otherwise, false.</returns>
        public bool TryGetAttributeInfo(Attribute attribute, out ElementAttributeInfo attributeInfo)
        {
            EnsureInitialize();
            if (_attributeInfoCache == null)
            {
                attributeInfo = null;
                return false;
            }

            return _attributeInfoCache.TryGetValue(attribute, out attributeInfo);
        }

        ElementAttributeInfo[] IAttributeResolver.GetAttributeInfos()
        {
            EnsureInitialize();
            return GetAttributeInfos();
        }

        /// <summary>
        /// Resets the initialization flag and clears the cache when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _isInitialized = false;
            _attributeInfoCache?.Clear();
            _attributeInfoCache = null;
        }
    }
}
