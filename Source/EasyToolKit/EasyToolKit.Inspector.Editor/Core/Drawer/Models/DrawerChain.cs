using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a chain of drawers that can be iterated over during property drawing.
    /// This class implements both <see cref="IEnumerable{T}"/> and <see cref="IEnumerator{T}"/> interfaces to allow
    /// sequential iteration through drawers while automatically skipping drawers
    /// that should be skipped during the drawing process.
    /// </summary>
    public class DrawerChain : IEnumerable<IEasyDrawer>, IEnumerator<IEasyDrawer>
    {
        private readonly IEasyDrawer[] _drawers;
        private int _index = -1;

        /// <summary>
        /// Gets the current drawer in the iteration.
        /// Returns null if the iteration is before the first drawer or after the last drawer.
        /// </summary>
        public IEasyDrawer Current
        {
            get
            {
                if (_index >= 0 && _index < _drawers.Length)
                {
                    var result = _drawers[_index];
                    result.Element = Element;
                    result.Chain = this;
                    return result;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all drawers in this chain, including those that may be skipped during iteration.
        /// </summary>
        public IEasyDrawer[] Drawers => _drawers;

        /// <summary>
        /// Gets the inspector element associated with this drawer chain.
        /// </summary>
        public IElement Element { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DrawerChain class.
        /// </summary>
        /// <param name="element">The inspector property this drawer chain is associated with.</param>
        /// <param name="drawers">The collection of drawers to include in the chain.</param>
        public DrawerChain(IElement element, IEnumerable<IEasyDrawer> drawers)
        {
            Element = element;
            _drawers = drawers.ToArray();
        }

        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next drawer in the chain.
        /// Automatically skips drawers that have SkipWhenDrawing set to true.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next drawer; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            do
            {
                _index++;
            } while (Current != null && this.Current.SkipWhenDrawing);

            return Current != null;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first drawer in the chain.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through all drawers in the chain.
        /// This includes drawers that would normally be skipped during drawing.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through all drawers.</returns>
        public IEnumerator<IEasyDrawer> GetEnumerator()
        {
            return ((IEnumerable<IEasyDrawer>)_drawers).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all drawers in the chain.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through all drawers.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Resets the enumerator position.
        /// </summary>
        void IDisposable.Dispose()
        {
            Reset();
        }
    }
}
