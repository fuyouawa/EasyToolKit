namespace EasyToolKit.Inspector.Editor
{
    public abstract class StructureResolverBase : ResolverBase, IStructureResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;
        private bool _isInitialized;

        public int ChildCount
        {
            get
            {
                EnsureInitialize();
                if (_lastChildCountUpdateId != Element.SharedContext.UpdateId)
                {
                    _lastChildCountUpdateId = Element.SharedContext.UpdateId;
                    _childCount = CalculateChildCount();
                }
                return _childCount;
            }
        }

        /// <summary>
        /// Gets the definition of a child at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child</param>
        /// <returns>Definition of the child</returns>
        protected abstract IElementDefinition GetChildDefinition(int childIndex);

        /// <summary>
        /// Converts a child name to its index
        /// </summary>
        /// <param name="name">The name of the child</param>
        /// <returns>The index of the child, or -1 if not found</returns>
        protected abstract int ChildNameToIndex(string name);

        /// <summary>
        /// Calculates the number of children
        /// </summary>
        /// <returns>The number of children</returns>
        protected abstract int CalculateChildCount();

        protected virtual void Initialize()
        {
        }

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        IElementDefinition IStructureResolver.GetChildDefinition(int childIndex)
        {
            EnsureInitialize();
            return GetChildDefinition(childIndex);
        }

        int IStructureResolver.ChildNameToIndex(string name)
        {
            EnsureInitialize();
            return ChildNameToIndex(name);
        }
    }
}
