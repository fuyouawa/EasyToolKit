namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Manages focus state for editor windows.
    /// </summary>
    internal interface IFocusManager
    {
        /// <summary>
        /// Processes focus events to handle clearing focus when clicking on empty areas.
        /// </summary>
        void ProcessFocusEvents();
    }
}
