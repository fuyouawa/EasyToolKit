using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionResolver
    {
        bool IsReadOnly { get; }
        Type CollectionType { get; }
        Type ElementType { get; }

        void QueueAddElement(int targetIndex, object value);
        void QueueRemoveElement(int targetIndex, object value);
    }

    public abstract class CollectionResolverBase<TCollection, TElement> : PropertyResolver, ICollectionResolver
        where TCollection : IEnumerable<TElement>
    {
        private Action _changeAction;
        private int _lastIsReadOnlyUpdateId;
        private bool _isReadOnly;
        private IPropertyValueEntry<TCollection> _valueEntry;

        public IPropertyValueEntry<TCollection> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IPropertyValueEntry<TCollection>;
                    if (_valueEntry == null)
                    {
                        Property.Update(true);
                        _valueEntry = Property.ValueEntry as IPropertyValueEntry<TCollection>;
                    }
                }
                return _valueEntry;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (_lastIsReadOnlyUpdateId != Property.Tree.UpdateId)
                {
                    _lastIsReadOnlyUpdateId = Property.Tree.UpdateId;
                    _isReadOnly = false;

                    for (int i = 0; i < ValueEntry.Values.Count; i++)
                    {
                        var collection = ValueEntry.Values[i];
                        if (IsReadOnlyCollection(collection))
                        {
                            _isReadOnly = true;
                            break;
                        }
                    }
                }

                return _isReadOnly;
            }
        }

        public Type CollectionType => typeof(TCollection);
        public Type ElementType => typeof(TElement);

        public void QueueAddElement(int targetIndex, object value)
        {
            EnqueueChange(() => AddElement(targetIndex, (TElement)value));
        }

        public void QueueRemoveElement(int targetIndex, object value)
        {
            EnqueueChange(() => RemoveElement(targetIndex, (TElement)value));
        }


        protected abstract void AddElement(int targetIndex, TElement value);
        protected abstract void RemoveElement(int targetIndex, TElement value);

        protected void EnqueueChange(Action action)
        {
            _changeAction += action;
            Property.Tree.QueueCallbackUntilRepaint(() =>
            {
                Property.Tree.SetPropertyDirty(Property);
            });
        }

        protected override bool ApplyChanges()
        {
            if (_changeAction != null)
            {
                foreach (var target in Property.Tree.Targets)
                {
                    Undo.RecordObject(target, $"Change {Property.Path} on {target.name}");
                }

                _changeAction();
                _changeAction = null;

                Property.Update();
                foreach (var child in Property.Children.Recurse())
                {
                    child.Update();
                }

                Property.Children.Clear();
                return true;
            }

            return false;
        }

        protected virtual bool IsReadOnlyCollection(TCollection collection) => false;
    }
}
