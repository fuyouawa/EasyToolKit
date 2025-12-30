using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="ElementPhases"/> to provide convenient phase checking operations.
    /// </summary>
    public static class ElementPhaseExtensions
    {
        /// <summary>
        /// Determines if the element is currently being drawn in the inspector UI.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the Drawing phase set.</returns>
        public static bool IsDrawing(this ElementPhases phases)
        {
            return (phases & ElementPhases.Drawing) == ElementPhases.Drawing;
        }

        /// <summary>
        /// Determines if the element is waiting to be refreshed.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the PendingRefresh phase set.</returns>
        public static bool IsPendingRefresh(this ElementPhases phases)
        {
            return (phases & ElementPhases.PendingRefresh) == ElementPhases.PendingRefresh;
        }

        /// <summary>
        /// Determines if the element is currently being refreshed.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the Refreshing phase set.</returns>
        public static bool IsRefreshing(this ElementPhases phases)
        {
            return (phases & ElementPhases.Refreshing) == ElementPhases.Refreshing;
        }

        /// <summary>
        /// Determines if the element has just completed a refresh operation.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the JustRefreshed phase set.</returns>
        public static bool IsJustRefreshed(this ElementPhases phases)
        {
            return (phases & ElementPhases.JustRefreshed) == ElementPhases.JustRefreshed;
        }

        /// <summary>
        /// Determines if the element is currently being updated.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the Updating phase set.</returns>
        public static bool IsUpdating(this ElementPhases phases)
        {
            return (phases & ElementPhases.Updating) == ElementPhases.Updating;
        }

        public static bool IsPendingPostProcess(this ElementPhases phases)
        {
            return (phases & ElementPhases.PendingPostProcess) == ElementPhases.PendingPostProcess;
        }

        public static bool IsPostProcessing(this ElementPhases phases)
        {
            return (phases & ElementPhases.PostProcessing) == ElementPhases.PostProcessing;
        }

        /// <summary>
        /// Determines if the element is waiting to be destroyed.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the PendingDestroy phase set.</returns>
        public static bool IsPendingDestroy(this ElementPhases phases)
        {
            return (phases & ElementPhases.PendingDestroy) == ElementPhases.PendingDestroy;
        }

        /// <summary>
        /// Determines if the element is currently being destroyed.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the Destroying phase set.</returns>
        public static bool IsDestroying(this ElementPhases phases)
        {
            return (phases & ElementPhases.Destroying) == ElementPhases.Destroying;
        }

        /// <summary>
        /// Determines if the element has been destroyed.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if the element has the Destroyed phase set.</returns>
        public static bool IsDestroyed(this ElementPhases phases)
        {
            return (phases & ElementPhases.Destroyed) == ElementPhases.Destroyed;
        }

        /// <summary>
        /// Determines if the element has no phases set.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <returns>True if no phases are set.</returns>
        public static bool IsNone(this ElementPhases phases)
        {
            return phases == ElementPhases.None;
        }

        /// <summary>
        /// Checks if the element has any of the specified phases set.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <param name="phasesToCheck">The phases to check against.</param>
        /// <returns>True if any of the specified phases are set.</returns>
        public static bool HasAny(this ElementPhases phases, ElementPhases phasesToCheck)
        {
            return (phases & phasesToCheck) != ElementPhases.None;
        }

        /// <summary>
        /// Checks if the element has all of the specified phases set.
        /// </summary>
        /// <param name="phases">The element phase to check.</param>
        /// <param name="phasesToCheck">The phases that must all be set.</param>
        /// <returns>True if all specified phases are set.</returns>
        public static bool Has(this ElementPhases phases, ElementPhases phasesToCheck)
        {
            return (phases & phasesToCheck) == phasesToCheck;
        }

        /// <summary>
        /// Sets the specified phase(s) on the element.
        /// </summary>
        /// <param name="phases">The original element phase.</param>
        /// <param name="phasesToSet">The phase(s) to set.</param>
        /// <returns>New element phase with the specified phases set.</returns>
        public static ElementPhases Add(this ElementPhases phases, ElementPhases phasesToSet)
        {
            return phases | phasesToSet;
        }

        /// <summary>
        /// Removes the specified phase(s) from the element.
        /// </summary>
        /// <param name="phases">The original element phase.</param>
        /// <param name="phasesToRemove">The phase(s) to remove.</param>
        /// <returns>New element phase with the specified phases removed.</returns>
        public static ElementPhases Remove(this ElementPhases phases, ElementPhases phasesToRemove)
        {
            return phases & ~phasesToRemove;
        }

        /// <summary>
        /// Toggles the specified phase(s) on the element.
        /// </summary>
        /// <param name="phases">The original element phase.</param>
        /// <param name="phasesToToggle">The phase(s) to toggle.</param>
        /// <returns>New element phase with the specified phases toggled.</returns>
        public static ElementPhases Toggle(this ElementPhases phases, ElementPhases phasesToToggle)
        {
            return phases ^ phasesToToggle;
        }
    }
}
