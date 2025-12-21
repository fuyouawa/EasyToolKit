using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Exception thrown when a service cannot be resolved.
    /// </summary>
    public class DependencyResolutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DependencyResolutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DependencyResolutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
