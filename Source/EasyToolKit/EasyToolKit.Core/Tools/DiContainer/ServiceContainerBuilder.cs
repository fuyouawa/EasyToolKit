using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Builder for creating service containers.
    /// </summary>
    public static class ServiceContainerBuilder
    {
        /// <summary>
        /// Creates a new service container with the specified registrations.
        /// </summary>
        public static IServiceContainer Build(IEnumerable<ServiceDescriptor> descriptors)
        {
            return new ServiceContainer(descriptors);
        }

        /// <summary>
        /// Creates a new service container from a collection of descriptors.
        /// </summary>
        public static IServiceContainer Build(params ServiceDescriptor[] descriptors)
        {
            return new ServiceContainer(descriptors);
        }
    }
}
