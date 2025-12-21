using System;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverPriority(100000.0)]
    public class RootPropertyOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Flags.IsRoot();
        }

        protected override void Initialize()
        {
            _operation = new GenericPropertyOperation(
                null, Element.ValueEntry.ValueType,
                (ref object index) => Element.SharedContext.Tree.Targets[(int)index],
                (ref object index, object value) => throw new InvalidOperationException("Cannot set logic root"));
        }

        protected override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}
