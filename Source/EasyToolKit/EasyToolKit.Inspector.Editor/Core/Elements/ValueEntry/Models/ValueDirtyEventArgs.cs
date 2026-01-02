using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides data for the value dirty event.
    /// </summary>
    [MustDisposeResource]
    public class ValueDirtyEventArgs : EventArgs, IPoolItem, IDisposable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ValueDirtyEventArgs"/> class from the object pool.
        /// </summary>
        /// <returns>A new or reused instance of <see cref="ValueDirtyEventArgs"/>.</returns>
        public static ValueDirtyEventArgs Create()
        {
            return EditorPoolUtility.Rent<ValueDirtyEventArgs>();
        }

        /// <summary>
        /// Releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            EditorPoolUtility.Release(this);
        }

        void IPoolItem.Rent()
        {
        }

        void IPoolItem.Release()
        {
        }
    }
}
