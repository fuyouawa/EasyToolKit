using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class OnInspectorGUIAttributeDrawer : EasyMethodAttributeDrawer<OnInspectorGUIAttribute>
    {
        protected override void Initialize()
        {
            if (MethodInfo.GetParameters().Length != 0)
            {
                throw new ArgumentException($"The OnInspectorGUI method '{MethodInfo}' must have no parameters.");
            }
        }

        protected override void Draw(GUIContent label)
        {
            foreach (var target in Element.LogicalParent.CastValue().ValueEntry.EnumerateWeakValues())
            {
                if (target == null)
                    continue;
                MethodInfo.Invoke(target, null);
            }
        }
    }
}
