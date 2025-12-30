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
    public class ValueElement : LogicalElementBase, IValueElement
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
                    PostProcessBaseValueEntry(_baseValueEntry);
                }

                return _baseValueEntry;
            }
        }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// This is built based on the runtime type of the value in <see cref="BaseValueEntry"/>.
        /// </summary>
        /// <remarks>
        /// <para>When the runtime type equals the declared type, this is the same as <see cref="BaseValueEntry"/>.</para>
        /// <para>When the runtime type is a derived type, this is a type wrapper around <see cref="BaseValueEntry"/>.</para>
        /// </remarks>
        public IValueEntry ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    return BaseValueEntry;
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
            [CanBeNull] ILogicalElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        protected override bool CanHaveChildren()
        {
            var valueType = Definition.ValueType;
            // Check if it's a plain value type or UnityEngine.Object reference
            return valueType.IsStructuralType() && base.CanHaveChildren();
        }

        protected override void OnUpdate(bool forceUpdate)
        {
            _baseValueEntry.Update();

            if (_baseValueEntry.State == ValueEntryState.TypeConsistent || _baseValueEntry.State == ValueEntryState.Consistent)
            {
                var runtimeType = _baseValueEntry.RuntimeValueType;
                Assert.IsTrue(runtimeType != null,
                    () => $"Runtime type of value entry is null, element is '{this}'");
                if (runtimeType != _baseValueEntry.ValueType)
                {
                    if (_valueEntry == null ||
                        (_valueEntry is IValueEntryWrapper && _valueEntry.RuntimeValueType != runtimeType))
                    {
                        _valueEntry = CreateWrapperValueEntry();
                        RequestRefresh();
                    }
                }
            }

            if (_valueEntry == null)
            {
                _valueEntry = _baseValueEntry;
                RequestRefresh();
            }

            base.OnUpdate(forceUpdate);
        }

        /// <summary>
        /// Creates the value entry for managing the underlying value storage.
        /// </summary>
        /// <returns>A value entry instance for this element.</returns>
        protected virtual IValueEntry CreateBaseValueEntry()
        {
            var valueEntryType = typeof(ValueEntry<>).MakeGenericType(Definition.ValueType);
            return valueEntryType.CreateInstance<IValueEntry>(this);
        }

        protected virtual void PostProcessBaseValueEntry(IValueEntry baseValueEntry)
        {
            baseValueEntry.AfterValueChanged += OnValueChanged;
        }

        /// <summary>
        /// Creates a wrapper value entry for managing the underlying value storage.
        /// </summary>
        /// <returns>A value entry instance for this element.</returns>
        protected virtual IValueEntry CreateWrapperValueEntry()
        {
            var valueEntryType = typeof(ValueEntryWrapper<,>).MakeGenericType(_baseValueEntry.RuntimeValueType, _baseValueEntry.ValueType);
            return valueEntryType.CreateInstance<IValueEntry>(_baseValueEntry);
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs eventArgs)
        {
            if (eventArgs.OldValue?.GetType() != eventArgs.NewValue?.GetType())
            {
                RequestRefresh();
            }
        }
    }
}
