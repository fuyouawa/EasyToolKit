namespace EasyToolKit.Inspector.Editor.Modules
{
    /// <summary>
    /// Base interface for editor window modules
    /// </summary>
    internal interface IEditorWindowModule
    {
        /// <summary>
        /// Initialize the module with the parent window
        /// </summary>
        /// <param name="window">The parent editor window</param>
        void Initialize(EasyEditorWindow window);

        /// <summary>
        /// Called when the module is being destroyed
        /// </summary>
        void Dispose();

        /// <summary>
        /// Called during OnGUI to handle module-specific GUI logic
        /// </summary>
        void OnGUI();

        /// <summary>
        /// Called when the window is enabled
        /// </summary>
        void OnEnable();

        /// <summary>
        /// Called when the window is disabled/destroyed
        /// </summary>
        void OnDisable();
    }

    /// <summary>
    /// Abstract base class for editor window modules providing common functionality
    /// </summary>
    internal abstract class EditorWindowModuleBase : IEditorWindowModule
    {
        protected EasyEditorWindow Window { get; private set; }

        public virtual void Initialize(EasyEditorWindow window)
        {
            Window = window;
        }

        public abstract void Dispose();

        public virtual void OnGUI() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }
    }
}
