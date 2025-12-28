using System;

namespace EasyToolKit.Inspector.Editor
{
    public class CustomValueOperationResolver : ValueOperationResolverBase
    {
        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Roles.IsCustomValue();
        }

        protected override IValueOperation GetOperation()
        {
            throw new NotImplementedException("Get operation for dynamically created custom value is not implemented.");
        }
    }
}
