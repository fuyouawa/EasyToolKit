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
        private IValueEntry _valueEntry;

        /// <summary>
        /// Gets the value definition that describes this value element.
        /// </summary>
        public new IValueDefinition Definition => (IValueDefinition)base.Definition;


        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// </summary>
        public IValueEntry ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = CreateValueEntry();
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
            return !valueType.IsBasicValueType() && !typeof(UnityEngine.Object).IsAssignableFrom(valueType) && base.CanHaveChildren();
        }

        protected override void Update(bool forceUpdate)
        {
            ValueEntry.Update();
        }

        /// <summary>
        /// Creates the value entry for managing the underlying value storage.
        /// </summary>
        /// <returns>A value entry instance for this element.</returns>
        protected virtual IValueEntry CreateValueEntry()
        {
            var valueType = Definition.ValueType;
            var valueEntryType = typeof(ValueEntry<>).MakeGenericType(valueType);

            var valueEntry = valueEntryType.CreateInstance<IValueEntry>(this);
            valueEntry.AfterValueChanged += OnValueChanged;
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
