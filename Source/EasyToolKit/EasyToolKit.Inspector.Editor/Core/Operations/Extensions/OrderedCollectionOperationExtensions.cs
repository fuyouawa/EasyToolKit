using System;

namespace EasyToolKit.Inspector.Editor
{
    public static class OrderedCollectionOperationExtensions
    {
        public static void InsertWeakElementAt(this IOrderedCollectionOperation collectionOperation, InspectorProperty property, int targetIndex, int elementIndex, object value)
        {
            if (property.GetOperation() != collectionOperation)
            {
                throw new ArgumentException("Property operation does not match collection operation", nameof(property));
            }

            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.InsertWeakElementAt(ref collection, elementIndex, value);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }

        public static void RemoveWeakElementAt(this IOrderedCollectionOperation collectionOperation, InspectorProperty property, int targetIndex, int elementIndex)
        {
            if (property.GetOperation() != collectionOperation)
            {
                throw new ArgumentException("Property operation does not match collection operation", nameof(property));
            }

            var collection = property.ValueEntry.WeakValues[targetIndex];
            collectionOperation.RemoveWeakElementAt(ref collection, elementIndex);
            property.ValueEntry.WeakValues[targetIndex] = collection;
        }
    }
}
