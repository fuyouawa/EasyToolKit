using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides data for the element destroyed event.
    /// </summary>
    [MustDisposeResource]
    public class ElementDestroyedEventArgs : EventArgs, IPoolItem, IDisposable
    {
        /// <summary>
        /// Gets the element that was destroyed.
        /// </summary>
        public IElement Element { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ElementDestroyedEventArgs"/> class from the object pool.
        /// </summary>
        /// <param name="element">The element that was destroyed.</param>
        /// <returns>A new or reused instance of <see cref="ElementDestroyedEventArgs"/>.</returns>
        public static ElementDestroyedEventArgs Create(IElement element)
        {
            var args = EditorPoolUtility.Rent<ElementDestroyedEventArgs>();
            args.Element = element;
            return args;
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
            Element = null;
        }
    }
}
