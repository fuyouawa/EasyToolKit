namespace EasyToolKit.Core
{
    /// <summary>
    /// Interface for pooled objects that need to be notified of rent and release events.
    /// </summary>
    /// <remarks>
    /// <para>If implemented by a regular class, the instance's methods will be called directly
    /// when the object is rented or released.</para>
    /// <para>If implemented by a Unity component, all components implementing this interface
    /// on the GameObject will be notified.</para>
    /// <para>This interface is optional; implement it only when you need to listen to pool events.</para>
    /// </remarks>
    public interface IPoolItem
    {
        /// <summary>
        /// Called when the object is rented from the pool.
        /// </summary>
        void Rent();

        /// <summary>
        /// Called when the object is released back to the pool.
        /// </summary>
        void Release();
    }
}
