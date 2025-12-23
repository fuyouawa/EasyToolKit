using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyMethodAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : MethodAttribute
    {
        private MethodInfo _methodInfo;

        public new IMethodElement Element => base.Element as IMethodElement;

        public MethodInfo MethodInfo => Element.Definition.MethodInfo;

        protected override bool CanDraw(IElement element)
        {
            if (element is IMethodElement methodElement)
            {
                return CanDrawElement(methodElement);
            }

            return false;
        }

        protected virtual bool CanDrawElement(IMethodElement element)
        {
            return true;
        }
    }
}
