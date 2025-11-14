using System;

namespace EasyToolKit.Inspector
{
    public abstract class CanPassToListElementAttribute : Attribute
    {
        public bool PassToListElements { get; set; }
    }
}
