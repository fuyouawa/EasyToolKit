using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for all EasyToolKit drawers that handle property drawing in the Unity Inspector.
    /// Drawers are responsible for rendering custom UI for properties and attributes.
    /// </summary>
    public interface IEasyDrawer : IInitializable
    {
        /// <summary>
        /// Gets or sets whether this drawer should be skipped during the drawing process.
        /// When true, the drawer will not render its UI.
        /// </summary>
        bool SkipWhenDrawing { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="InspectorProperty"/> that this drawer is responsible for rendering.
        /// </summary>
        InspectorProperty Property { get; set; }

        /// <summary>
        /// Draws the property UI using the specified label.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
        void DrawProperty(GUIContent label);

        /// <summary>
        /// Determines whether this drawer can draw the specified property.
        /// </summary>
        /// <param name="property">The <see cref="InspectorProperty"/> to check.</param>
        /// <returns>True if this drawer can handle the property, false otherwise.</returns>
        bool CanDrawProperty(InspectorProperty property);
    }

    /// <summary>
    /// Base abstract class for all EasyToolKit drawers.
    /// Provides common functionality for property drawing and lifecycle management.
    /// </summary>
    public abstract class EasyDrawer : IEasyDrawer
    {
        /// <summary>
        /// Gets or sets whether this drawer should be skipped during the drawing process.
        /// </summary>
        public bool SkipWhenDrawing { get; set; }

        /// <summary>
        /// Gets the InspectorProperty that this drawer is responsible for rendering.
        /// </summary>
        public InspectorProperty Property { get; private set; }

        /// <summary>
        /// Gets whether this drawer has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Called when the drawer is being initialized.
        /// Override this method to perform custom initialization logic.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Called when the drawer is being deinitialized.
        /// Override this method to perform cleanup operations.
        /// </summary>
        protected virtual void Deinitialize()
        {
        }

        /// <summary>
        /// Determines whether this drawer can draw the specified property.
        /// Override this method to implement custom property filtering logic.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the property, false otherwise.</returns>
        protected virtual bool CanDrawProperty(InspectorProperty property)
        {
            return true;
        }

        /// <summary>
        /// Calls the next drawer in the drawer chain for the current property.
        /// This allows multiple drawers to process the same property in sequence.
        /// </summary>
        /// <param name="label">The <see cref="GUIContent"/> label to pass to the next drawer.</param>
        /// <returns>True if there was a next drawer that was called, false otherwise.</returns>
        protected bool CallNextDrawer(GUIContent label)
        {
            var chain = Property.GetDrawerChain();
            if (chain.MoveNext())
            {
                chain.Current.DrawProperty(label);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Main method for drawing the property UI.
        /// Override this method to implement custom drawing logic.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
        protected virtual void DrawProperty(GUIContent label)
        {
        }

        /// <summary>
        /// Explicit interface implementation for IEasyDrawer.Property.
        /// Gets or sets the InspectorProperty for this drawer.
        /// </summary>
        InspectorProperty IEasyDrawer.Property
        {
            get => Property;
            set => Property = value;
        }

        /// <summary>
        /// Explicit interface implementation for IInitializable.IsInitialized.
        /// Gets whether this drawer has been initialized.
        /// </summary>
        bool IInitializable.IsInitialized => IsInitialized;

        /// <summary>
        /// Explicit interface implementation for IInitializable.Initialize.
        /// Initializes the drawer if it hasn't been initialized already.
        /// </summary>
        void IInitializable.Initialize()
        {
            if (IsInitialized) return;
            Initialize();
            IsInitialized = true;
        }

        /// <summary>
        /// Explicit interface implementation for IInitializable.Deinitialize.
        /// Deinitializes the drawer if it has been initialized.
        /// </summary>
        void IInitializable.Deinitialize()
        {
            if (!IsInitialized) return;
            Deinitialize();
            IsInitialized = false;
        }

        /// <summary>
        /// Explicit interface implementation for IEasyDrawer.DrawProperty.
        /// Draws the property UI using the specified label.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
        void IEasyDrawer.DrawProperty(GUIContent label)
        {
            DrawProperty(label);
        }

        /// <summary>
        /// Explicit interface implementation for IEasyDrawer.CanDrawProperty.
        /// Determines whether this drawer can draw the specified property.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the property, false otherwise.</returns>
        bool IEasyDrawer.CanDrawProperty(InspectorProperty property)
        {
            return CanDrawProperty(property);
        }
    }
}
