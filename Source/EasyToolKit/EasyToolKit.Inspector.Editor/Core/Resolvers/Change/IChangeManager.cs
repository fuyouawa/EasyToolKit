using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for managing property changes in the inspector system.
    /// Handles change queuing, application, and undo/redo operations.
    /// </summary>
    public interface IChangeManager
    {
        /// <summary>
        /// Enqueues a change action to be applied later
        /// </summary>
        /// <param name="action">The action representing the change to be applied</param>
        void EnqueueChange(Action action);

        /// <summary>
        /// Applies all queued changes
        /// </summary>
        /// <returns>True if changes were applied successfully, false if there were no pending changes</returns>
        bool ApplyChanges();

        /// <summary>
        /// Records an undo operation for the given action
        /// </summary>
        /// <param name="operationName">The name of the operation for undo/redo tracking</param>
        void RecordUndo(string operationName);
    }
}