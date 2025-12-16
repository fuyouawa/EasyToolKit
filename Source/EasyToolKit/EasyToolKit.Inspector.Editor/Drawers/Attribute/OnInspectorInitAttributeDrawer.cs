using System;

namespace EasyToolKit.Inspector.Editor
{
    public class OnInspectorInitAttributeDrawer : EasyMethodAttributeDrawer<OnInspectorInitAttribute>
    {
        protected override void Initialize()
        {
            Action<object, object> action;
            var parameters = MethodInfo.GetParameters();
            if (parameters.Length == 0)
            {
                action = (target, referencedObject) => MethodInfo.Invoke(target, null);
            }
            else if (parameters.Length == 1)
            {
                action = (target, referencedObject) => MethodInfo.Invoke(target, new object[] { referencedObject });
            }
            else
            {
                throw new Exception($"The OnInspectorInit method '{MethodInfo}' only supports 0 or 1 parameters.");
            }

            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                var target = Property.Parent.ValueEntry.WeakValues[i];
                if (target == null)
                    continue;
                var referencedObject = Property.Tree.Targets[i];
                action(target, referencedObject);
            }
        }
    }
}
