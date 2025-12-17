using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Null implementation of change management for testing purposes.
    /// Does nothing when changes are queued or applied.
    /// </summary>
    public class NullChangeManager : IChangeManager
    {
        /// <summary>
        /// Enqueues a change action to be applied later (no-op in null implementation)
        /// </summary>
        /// <param name="action">The action representing the change to be applied</param>
        public void EnqueueChange(Action action)
        {
            // No-op for testing
        }

        /// <summary>
        /// Applies all queued changes (always returns false in null implementation)
        /// </summary>
        /// <returns>False - no changes applied in null implementation</returns>
        public bool ApplyChanges()
        {
            return false;
        }

        /// <summary>
        /// Records an undo operation (no-op in null implementation)
        /// </summary>
        /// <param name="operationName">The name of the operation for undo/redo tracking</param>
        public void RecordUndo(string operationName)
        {
            // No-op for testing
        }
    }
}