using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a chain of post processors that can be iterated over during element processing.
    /// This class implements both <see cref="IEnumerable{T}"/> and <see cref="IEnumerator{T}"/> interfaces to allow
    /// sequential iteration through post processors while automatically skipping post processors
    /// that should be skipped during the processing.
    /// </summary>
    public class PostProcessorChain : IEnumerable<IPostProcessor>, IEnumerator<IPostProcessor>
    {
        private readonly IPostProcessor[] _postProcessors;
        private int _index = -1;

        /// <summary>
        /// Gets the current post processor in the iteration.
        /// Returns null if the iteration is before the first post processor or after the last post processor.
        /// </summary>
        public IPostProcessor Current
        {
            get
            {
                if (_index >= 0 && _index < _postProcessors.Length)
                {
                    var result = _postProcessors[_index];
                    result.Element = Element;
                    result.Chain = this;
                    return result;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all post processors in this chain, including those that may be skipped during iteration.
        /// </summary>
        public IPostProcessor[] PostProcessors => _postProcessors;

        /// <summary>
        /// Gets the inspector element associated with this post processor chain.
        /// </summary>
        public IElement Element { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostProcessorChain"/> class.
        /// </summary>
        /// <param name="element">The inspector element this post processor chain is associated with.</param>
        /// <param name="postProcessors">The collection of post processors to include in the chain.</param>
        public PostProcessorChain(IElement element, IEnumerable<IPostProcessor> postProcessors)
        {
            Element = element;
            _postProcessors = postProcessors.ToArray();
        }

        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next post processor in the chain.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next post processor; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            _index++;
            return Current != null;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first post processor in the chain.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through all post processors in the chain.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through all post processors.</returns>
        public IEnumerator<IPostProcessor> GetEnumerator()
        {
            return ((IEnumerable<IPostProcessor>)_postProcessors).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all post processors in the chain.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through all post processors.</returns>
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
