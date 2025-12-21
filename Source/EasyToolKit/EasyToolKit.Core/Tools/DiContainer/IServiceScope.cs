using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a scope for service resolution.
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        /// <summary>
        /// The service provider for this scope.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}
