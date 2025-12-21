using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Describes a service registration.
    /// </summary>
    public class ServiceDescriptor
    {
        /// <summary>
        /// The service type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// The implementation instance (for singleton registrations).
        /// </summary>
        public object ImplementationInstance { get; internal set; }

        /// <summary>
        /// The implementation factory (for factory-based registrations).
        /// </summary>
        public Func<IServiceProvider, object> ImplementationFactory { get; }

        private ServiceDescriptor(
            Type serviceType,
            ServiceLifetime lifetime,
            Type implementationType = null,
            object implementationInstance = null,
            Func<IServiceProvider, object> implementationFactory = null)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            Lifetime = lifetime;
            ImplementationType = implementationType;
            ImplementationInstance = implementationInstance;
            ImplementationFactory = implementationFactory;
        }

        /// <summary>
        /// Creates a transient service descriptor.
        /// </summary>
        public static ServiceDescriptor Transient<TService, TImplementation>()
            where TImplementation : TService
        {
            return Transient(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Creates a transient service descriptor.
        /// </summary>
        public static ServiceDescriptor Transient(Type serviceType, Type implementationType)
        {
            return new ServiceDescriptor(serviceType, ServiceLifetime.Transient, implementationType);
        }

        /// <summary>
        /// Creates a transient service descriptor using a factory.
        /// </summary>
        public static ServiceDescriptor Transient<TService>(Func<IServiceProvider, TService> factory)
        {
            return new ServiceDescriptor(typeof(TService), ServiceLifetime.Transient, implementationFactory: provider => factory(provider));
        }

        /// <summary>
        /// Creates a singleton service descriptor.
        /// </summary>
        public static ServiceDescriptor Singleton<TService, TImplementation>()
            where TImplementation : TService
        {
            return Singleton(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Creates a singleton service descriptor.
        /// </summary>
        public static ServiceDescriptor Singleton(Type serviceType, Type implementationType)
        {
            return new ServiceDescriptor(serviceType, ServiceLifetime.Singleton, implementationType);
        }

        /// <summary>
        /// Creates a singleton service descriptor using a factory.
        /// </summary>
        public static ServiceDescriptor Singleton<TService>(Func<IServiceProvider, TService> factory)
        {
            return new ServiceDescriptor(typeof(TService), ServiceLifetime.Singleton, implementationFactory: provider => factory(provider));
        }

        /// <summary>
        /// Creates a singleton service descriptor using an existing instance.
        /// </summary>
        public static ServiceDescriptor Singleton<TService>(TService instance)
        {
            return new ServiceDescriptor(typeof(TService), ServiceLifetime.Singleton, implementationInstance: instance);
        }

        /// <summary>
        /// Creates a scoped service descriptor.
        /// </summary>
        public static ServiceDescriptor Scoped<TService, TImplementation>()
            where TImplementation : TService
        {
            return Scoped(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Creates a scoped service descriptor.
        /// </summary>
        public static ServiceDescriptor Scoped(Type serviceType, Type implementationType)
        {
            return new ServiceDescriptor(serviceType, ServiceLifetime.Scoped, implementationType);
        }

        /// <summary>
        /// Creates a scoped service descriptor using a factory.
        /// </summary>
        public static ServiceDescriptor Scoped<TService>(Func<IServiceProvider, TService> factory)
        {
            return new ServiceDescriptor(typeof(TService), ServiceLifetime.Scoped, implementationFactory: provider => factory(provider));
        }
    }
}
