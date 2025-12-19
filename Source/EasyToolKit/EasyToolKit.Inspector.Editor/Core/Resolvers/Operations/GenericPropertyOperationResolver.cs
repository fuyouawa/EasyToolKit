using System;
using System.Collections;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericPropertyOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolve(InspectorProperty property)
        {
            return !property.ValueEntry.ValueType.IsInheritsFrom<IEnumerable>();
        }

        protected override void Initialize()
        {
            if (Property.Info.IsLogicRoot)
            {
                _operation = new GenericPropertyOperation(
                    null, Property.ValueEntry.ValueType,
                    (ref object index) => Property.Tree.Targets[(int)index],
                    (ref object index, object value) => throw new InvalidOperationException("Cannot set logic root"));
            }
            else
            {
                var ownerType = Property.Parent.ValueEntry.ValueType;
                var operationType = typeof(MemberPropertyOperation<,>).MakeGenericType(ownerType, Property.ValueEntry.ValueType);
                _operation = (IPropertyOperation)Activator.CreateInstance(operationType);
            }
        }

        public override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}
