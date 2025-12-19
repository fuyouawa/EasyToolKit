using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverPriority(-10000.0)]
    public class UnityCollectionStructureResolver<TCollection, TElement> : CollectionStructureResolverBase<TCollection>
        where TCollection : IList<TElement>
    {
        private SerializedProperty _serializedProperty;

        public override Type ElementType => typeof(TElement);

        protected override bool CanResolveProperty(InspectorProperty property)
        {
            if (Property.Info.IsLogicRoot)
            {
                _serializedProperty = Property.Tree.SerializedObject?.GetIterator();
            }
            else
            {
                _serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.UnityPath);
            }

            if (_serializedProperty == null)
            {
                return false;
            }

            return _serializedProperty.isArray;
        }

        protected override void Initialize()
        {
            base.Initialize();
            if (Property.Info.IsLogicRoot)
            {
                _serializedProperty = Property.Tree.SerializedObject.GetIterator();
            }
            else
            {
                _serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.UnityPath);
                if (_serializedProperty == null)
                {
                    throw new InvalidOperationException($"SerializedProperty is null for path: {Property.UnityPath}");
                }
            }
        }

        protected override int CalculateChildCount()
        {
            return _serializedProperty.arraySize;
        }
    }
}
