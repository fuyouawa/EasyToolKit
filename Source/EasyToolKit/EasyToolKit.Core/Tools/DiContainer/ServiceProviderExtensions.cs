using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IServiceProvider"/> providing generic typed APIs.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The service provider.</param>
        /// <returns>A service object of type <typeparamref name="T"/>, or null if there is no such service.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is null.</exception>
        public static T GetService<T>(this IServiceProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            return (T)provider.GetService(typeof(T));
        }
    }
}
