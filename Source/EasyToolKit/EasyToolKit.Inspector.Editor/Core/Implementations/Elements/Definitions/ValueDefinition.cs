using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base implementation of <see cref="IValueDefinition"/> for all data-containing elements.
    /// </summary>
    public class ValueDefinition : ElementDefinition, IValueDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefinition"/> class.
        /// </summary>
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="valueType">The type of the value.</param>
        public ValueDefinition(ElementFlags flags, string name, Type valueType)
            : base(flags, name)
        {
            ValueType = valueType;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        public Type ValueType { get; }
    }
}
