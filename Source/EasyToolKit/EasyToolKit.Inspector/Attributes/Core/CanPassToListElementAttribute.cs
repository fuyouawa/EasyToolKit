using System;

namespace EasyToolKit.Inspector
{
    public abstract class CanPassToListElementAttribute : InspectorAttribute
    {
        public bool PassToListElements { get; set; }
    }
}
