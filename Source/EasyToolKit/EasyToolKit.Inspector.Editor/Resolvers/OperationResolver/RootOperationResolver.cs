using System;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverPriority(100000.0)]
    public class RootOperationResolver : ValueOperationResolverBase
    {
        private IValueOperation _operation;

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Roles.IsRoot();
        }

        protected override void Initialize()
        {
            var method = GetType()
                .GetMethod(nameof(CreateOperationWrapper), BindingFlagsHelper.AllStatic)!
                .MakeGenericMethod(Element.ValueEntry.ValueType);
            _operation = (IValueOperation)method.Invoke(null, new object[] { Element });
        }

        protected override IValueOperation GetOperation()
        {
            return _operation;
        }

        private static IValueOperation CreateOperationWrapper<TValue>(IValueElement element)
        {
            return new GenericValueOperation<TValue>(
                typeof(int),
                (ref object instance) => (TValue)element.SharedContext.Tree.Targets[(int)instance],
                (ref object instance, TValue value) => throw new InvalidOperationException("Cannot set logic root"));
        }
    }
}
