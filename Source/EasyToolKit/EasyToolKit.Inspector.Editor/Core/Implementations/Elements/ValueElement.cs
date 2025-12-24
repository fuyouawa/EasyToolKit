using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Provides a value element implementation that represents data-containing elements
    /// such as fields, properties, or dynamically created custom values.
    /// </summary>
    public class ValueElement : ElementBase, IValueElement
    {
        private IValueEntry _baseValueEntry;
        private IValueEntry _valueEntry;

        /// <summary>
        /// Gets the value definition that describes this value element.
        /// </summary>
        public new IValueDefinition Definition => (IValueDefinition)base.Definition;

        /// <summary>
        /// Gets the base value entry that is built directly from <see cref="IValueDefinition.ValueType"/>.
        /// This represents the declared type of the value.
        /// </summary>
        public IValueEntry BaseValueEntry
        {
            get
            {
                if (_baseValueEntry == null)
                {
                    _baseValueEntry = CreateBaseValueEntry();
                }

                return _baseValueEntry;
            }
        }

        public IValueEntry ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    return _baseValueEntry;
                }

                return _valueEntry;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueElement"/> class.
        /// </summary>
        /// <param name="definition">The value definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public ValueElement(
            [NotNull] IValueDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        protected override bool CanHaveChildren()
        {
            var valueType = Definition.ValueType;
            // Check if it's a plain value type or UnityEngine.Object reference
            return valueType.IsStructuralType() && base.CanHaveChildren();
        }

        protected override void Update(bool forceUpdate)
        {
            _baseValueEntry.Update();

            if (_baseValueEntry.State == ValueEntryState.TypeConsistent || _baseValueEntry.State == ValueEntryState.Consistent)
            {
                var runtimeType = _baseValueEntry.WeakSmartValue.GetType();
                if (_valueEntry == null ||
                    (_valueEntry is IValueEntryWrapper && _valueEntry.WeakSmartValue.GetType() != runtimeType) ||
                    runtimeType != _baseValueEntry.ValueType)
                {
                    _valueEntry = CreateWrapperValueEntry(runtimeType);
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Creates the value entry for managing the underlying value storage.
        /// </summary>
        /// <returns>A value entry instance for this element.</returns>
        protected virtual IValueEntry CreateBaseValueEntry()
        {
            var valueType = Definition.ValueType;
            var valueEntryType = typeof(ValueEntry<>).MakeGenericType(valueType);

            var valueEntry = valueEntryType.CreateInstance<IValueEntry>(this);
            valueEntry.AfterValueChanged += OnValueChanged;
            return valueEntry;
        }

        protected virtual IValueEntry CreateWrapperValueEntry(Type runtimeType)
        {
            var valueEntryType = typeof(ValueEntryWrapper<,>).MakeGenericType(runtimeType, _baseValueEntry.ValueType);
            var valueEntry = valueEntryType.CreateInstance<IValueEntry>(_baseValueEntry);
            return valueEntry;
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs eventArgs)
        {
            if (eventArgs.OldValue.GetType() != eventArgs.NewValue.GetType())
            {
                Refresh();
            }
        }
    }
}
