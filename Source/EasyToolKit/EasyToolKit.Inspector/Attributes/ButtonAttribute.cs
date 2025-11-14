using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ButtonAttribute : MethodAttribute
    {
        public string Label { get; set; }

        public ButtonAttribute()
        {
            Label = null;
        }

        public ButtonAttribute(string label)
        {
            Label = label;
        }
    }
}
