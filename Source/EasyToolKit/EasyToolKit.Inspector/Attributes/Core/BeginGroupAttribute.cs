using System;

namespace EasyToolKit.Inspector
{
    public abstract class BeginGroupAttribute : InspectorAttribute
    {
        public string GroupCatalogue { get; set; }
        public bool EndAfterThisProperty { get; set; }

        public virtual string GroupName => GroupCatalogue;

        protected BeginGroupAttribute()
        {
        }
    }
}
