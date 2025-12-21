using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a mechanism for retrieving service objects.
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        object GetService(Type serviceType);
    }
}
