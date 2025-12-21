using System;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Manages service registrations and resolutions.
    /// </summary>
    internal class ServiceRegistry
    {
        private readonly Dictionary<Type, ServiceDescriptor> _descriptors = new Dictionary<Type, ServiceDescriptor>();
        private readonly Dictionary<Type, Func<IServiceProvider, object>> _factories = new Dictionary<Type, Func<IServiceProvider, object>>();

        /// <summary>
        /// Adds a service descriptor to the registry.
        /// </summary>
        public void Add(ServiceDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            _descriptors[descriptor.ServiceType] = descriptor;
        }

        /// <summary>
        /// Gets the service descriptor for the specified type.
        /// </summary>
        public ServiceDescriptor GetDescriptor(Type serviceType)
        {
            return _descriptors.TryGetValue(serviceType, out var descriptor) ? descriptor : null;
        }

        /// <summary>
        /// Gets all service descriptors.
        /// </summary>
        public IEnumerable<ServiceDescriptor> GetDescriptors()
        {
            return _descriptors.Values;
        }

        /// <summary>
        /// Caches a factory for a service type.
        /// </summary>
        public void CacheFactory(Type serviceType, Func<IServiceProvider, object> factory)
        {
            _factories[serviceType] = factory;
        }

        /// <summary>
        /// Gets the cached factory for a service type.
        /// </summary>
        public Func<IServiceProvider, object> GetCachedFactory(Type serviceType)
        {
            return _factories.TryGetValue(serviceType, out var factory) ? factory : null;
        }
    }
}
