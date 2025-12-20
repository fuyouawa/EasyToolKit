using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IInspectorHandler
    {
        /// <summary>
        /// Gets or sets the <see cref="InspectorProperty"/> that this element is associated with.
        /// </summary>
        InspectorProperty Property { get; set; }

        bool CanHandle(InspectorProperty property);
    }
}
