using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public static class CollectionOperationExtensions
    {
        public static void AddWeakElement(this ICollectionOperation collectionOperation, InspectorProperty property, int targetIndex, object element)
        {
            if (property.GetOperation() != collectionOperation)
            {
                throw new ArgumentException("Property operation does not match collection operation", nameof(property));
            }

            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.AddWeakElement(ref collection, element);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }

        public static void RemoveWeakElement(this ICollectionOperation collectionOperation, InspectorProperty property, int targetIndex, object element)
        {
            if (property.GetOperation() != collectionOperation)
            {
                throw new ArgumentException("Property operation does not match collection operation", nameof(property));
            }

            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.RemoveWeakElement(ref collection, element);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }
    }
}
