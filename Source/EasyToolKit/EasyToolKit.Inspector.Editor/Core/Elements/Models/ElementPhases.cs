using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the processing phases of an element during its lifecycle in the inspector.
    /// Elements can be in multiple phases simultaneously to represent concurrent states.
    /// </summary>
    [Flags]
    public enum ElementPhases
    {
        /// <summary>
        /// No phase set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The element is currently being drawn in the inspector UI.
        /// </summary>
        Drawing = 1 << 0,

        /// <summary>
        /// The element is waiting to be refreshed.
        /// </summary>
        PendingRefresh = 1 << 1,

        /// <summary>
        /// The element is currently being refreshed.
        /// </summary>
        Refreshing = 1 << 2,

        /// <summary>
        /// The element has just completed a refresh operation.
        /// </summary>
        JustRefreshed = 1 << 3,

        /// <summary>
        /// The element is currently being updated.
        /// </summary>
        Updating = 1 << 4,

        PendingPostProcess = 1 << 5,

        PostProcessing = 1 << 6,

        /// <summary>
        /// The element is waiting to be destroyed.
        /// </summary>
        PendingDestroy = 1 << 7,

        Destroying = 1 << 8,

        /// <summary>
        /// The element has been destroyed.
        /// </summary>
        Destroyed = 1 << 9,
    }
}
