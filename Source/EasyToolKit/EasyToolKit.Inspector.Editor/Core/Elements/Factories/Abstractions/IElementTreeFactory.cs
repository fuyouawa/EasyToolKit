using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Defines a factory interface for creating <see cref="IElementTree"/> instances in the EasyToolKit inspector system.
    /// </summary>
    public interface IElementTreeFactory
    {
        /// <summary>
        /// Creates an element tree for a single serialized object.
        /// </summary>
        /// <param name="serializedObject">The serialized object to create a tree for. Must not be null.</param>
        /// <returns>A new <see cref="IElementTree"/> instance representing the complete inspector element hierarchy.</returns>
        IElementTree CreateTree([NotNull] SerializedObject serializedObject);

        /// <summary>
        /// Creates an element tree for multiple target objects with an optional serialized object.
        /// </summary>
        /// <param name="targets">The array of target objects to create a tree for. Must not be null.</param>
        /// <param name="serializedObject">The optional serialized object containing shared serialized data. Can be null.</param>
        /// <returns>A new <see cref="IElementTree"/> instance representing the multi-object inspector element hierarchy.</returns>
        /// <remarks>
        /// If <paramref name="serializedObject"/> is not null, all objects in <paramref name="targets"/>
        /// must match the objects in <see cref="SerializedObject.targetObjects"/>.
        /// </remarks>
        IElementTree CreateTree([NotNull] object[] targets, [CanBeNull] SerializedObject serializedObject);
    }
}
