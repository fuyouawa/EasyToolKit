using System;
using System.Reflection;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a property in the Unity inspector, providing access to property information,
    /// value management, drawing capabilities, and hierarchical property structure.
    ///
    /// This class manages the lifecycle of inspector properties including initialization,
    /// value updates, drawing, and disposal. It supports property resolvers, attribute
    /// resolution, drawer chains, and grouping for complex inspector layouts.
    /// </summary>
    public class InspectorProperty : IDisposable
    {
        private string _niceName;
        private PropertyState _state;
        private bool _isSelfReadOnly;
        private long _lastSelfReadOnlyUpdateId;
        private bool _isAllowChildren;
        private long _lastAllowChildrenUpdateId;
        private string _unityPath;

        private IPropertyStructureResolver _childrenResolver;
        private IGroupResolver _groupResolver;
        private IDrawerChainResolver _drawerChainResolver;
        private IAttributeResolver _attributeResolver;
        private long _lastUpdateId;

        /// <summary>
        /// Gets the parent property in the inspector hierarchy, or null if this is a root property.
        /// </summary>
        [CanBeNull] public InspectorProperty Parent { get; private set; }

        /// <summary>
        /// Gets the property tree that this property belongs to, which manages the entire inspector property hierarchy.
        /// </summary>
        public PropertyTree Tree { get; private set; }

        /// <summary>
        /// Gets the collection of child properties for this property, or null if this property has no children.
        /// </summary>
        [CanBeNull] public PropertyChildren Children { get; private set; }

        /// <summary>
        /// Gets the metadata information about this property, including type information, member accessors, and resolver configuration.
        /// </summary>
        public InspectorPropertyInfo Info { get; private set; }

        /// <summary>
        /// Gets the base value entry that provides access to the underlying property value, or null if this property has no value accessor.
        /// This entry represents the raw value without any type conversion or wrapping.
        /// </summary>
        [CanBeNull] public IPropertyValueEntry BaseValueEntry { get; private set; }

        /// <summary>
        /// Gets the current value entry that provides access to the property value, which may be a wrapper around the base value entry
        /// for type conversion scenarios, or null if this property has no value accessor.
        /// </summary>
        [CanBeNull] public IPropertyValueEntry ValueEntry { get; private set; }

        /// <summary>
        /// Gets the hierarchical path of this property within the inspector tree, using dot notation for nested properties.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets or sets the GUI content used for displaying this property's label in the inspector.
        /// This includes the display text, tooltip, and optional icon for the property.
        /// </summary>
        public GUIContent Label { get; set; }

        /// <summary>
        /// Gets the index of this property within its parent's children collection.
        /// For root properties, this value is typically 0.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets or sets the number of times this property should be skipped during drawing.
        /// This is used for advanced drawing scenarios where properties need to be conditionally hidden.
        /// </summary>
        public int SkipDrawCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorProperty"/> class.
        /// </summary>
        /// <param name="tree">The property tree this property belongs to</param>
        /// <param name="parent">The parent property, or null if this is a root property</param>
        /// <param name="info">Information about the property being represented</param>
        /// <param name="index">The index of this property within its parent's children collection</param>
        /// <exception cref="ArgumentNullException">Thrown when tree or info is null</exception>
        /// <exception cref="ArgumentException">Thrown when tree and parent's tree don't match</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when index is out of bounds</exception>
        internal InspectorProperty(PropertyTree tree, InspectorProperty parent, InspectorPropertyInfo info,
            int index)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (parent != null)
            {
                if (tree != parent.Tree)
                {
                    throw new ArgumentException("The given tree and the given parent's tree are not the same tree.");
                }

                if (index < 0 || index >= parent.Children!.Count)
                {
                    throw new IndexOutOfRangeException("The given index for the property to create is out of bounds.");
                }
            }

            Tree = tree;
            Parent = parent;
            Info = info;
            Index = index;

            if (parent != null)
            {
                Path = parent.Children.GetPath(index);
            }
            else
            {
                Path = info.PropertyName;
            }

            Label = new GUIContent(NiceName);

            _drawerChainResolver = new DefaultDrawerChainResolver();
            _attributeResolver = new DefaultAttributeResolver();
            _groupResolver = new DefaultGroupResolver();
            Refresh();

            if (info.ValueAccessor != null)
            {
                var entryType = typeof(PropertyValueEntry<>).MakeGenericType(info.ValueAccessor.ValueType);
                BaseValueEntry = (IPropertyValueEntry)entryType.CreateInstance(this);
            }
        }

        /// <summary>
        /// Gets the state of this property, including validation state, visibility, and other runtime properties.
        /// The state is lazily initialized when first accessed.
        /// </summary>
        public PropertyState State
        {
            get
            {
                if (_state == null)
                {
                    _state = new PropertyState(this);
                }

                return _state;
            }
        }

        /// <summary>
        /// Gets the raw name of the property as defined in the source code.
        /// </summary>
        public string Name => Info.PropertyName;

        /// <summary>
        /// Gets the Unity serialization path for this property.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessed on a logic root property</exception>
        public string UnityPath
        {
            get
            {
                if (Info.IsLogicRoot)
                {
                    throw new InvalidOperationException("Logic root property does not support unity path.");
                }

                if (_unityPath.IsNullOrEmpty())
                {
                    _unityPath = Path["$ROOT$.".Length..];
                }

                return _unityPath;
            }
        }

        /// <summary>
        /// Gets the display-friendly name of the property, formatted for Unity inspector display.
        /// This name is automatically nicified and method parentheses are removed if present.
        /// </summary>
        public string NiceName
        {
            get
            {
                if (_niceName == null)
                {
                    _niceName = ObjectNames.NicifyVariableName(Name);

                    if (_niceName.EndsWith("()"))
                    {
                        _niceName = _niceName[..^2];
                    }
                }

                return _niceName;
            }
        }

        /// <summary>
        /// Gets whether this property is read-only, either due to its own configuration
        /// or because it inherits read-only state from its parent.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (IsSelfReadOnly)
                {
                    return true;
                }

                if (Parent != null && Parent.IsReadOnly)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets whether this specific property is read-only, not considering parent state.
        /// A property is self-read-only if its value accessor is readonly or it has a ReadOnlyAttribute.
        /// </summary>
        public bool IsSelfReadOnly
        {
            get
            {
                if (_lastSelfReadOnlyUpdateId != Tree.UpdateId)
                {
                    if (Info.ValueAccessor != null && Info.ValueAccessor.IsReadonly)
                    {
                        _isSelfReadOnly = true;
                    }
                    else if (GetAttribute<ReadOnlyAttribute>() != null)
                    {
                        _isSelfReadOnly = true;
                    }
                    else
                    {
                        _isSelfReadOnly = false;
                    }

                    _lastSelfReadOnlyUpdateId = Tree.UpdateId;
                }

                return _isSelfReadOnly;
            }
        }

        /// <summary>
        /// Gets whether this property can have child properties in the inspector hierarchy.
        /// A property can have children if it represents a complex type (class, struct, collection)
        /// that can be expanded to show its members or elements.
        /// </summary>
        public bool IsAllowChildren
        {
            get
            {
                if (_lastAllowChildrenUpdateId != Tree.UpdateId)
                {
                    _isAllowChildren = IsAllowChildrenImpl();
                    _lastAllowChildrenUpdateId = Tree.UpdateId;
                }

                return _isAllowChildren;
            }
        }

        /// <summary>
        /// Gets or sets the resolver responsible for providing child properties for this property.
        /// Setting this value triggers a refresh of the property tree.
        /// </summary>
        [CanBeNull]
        public IPropertyStructureResolver ChildrenResolver
        {
            get => _childrenResolver;
            set
            {
                _childrenResolver = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the resolver responsible for providing the drawer chain for this property.
        /// Setting this value triggers a refresh of the property tree.
        /// </summary>
        [CanBeNull]
        public IDrawerChainResolver DrawerChainResolver
        {
            get => _drawerChainResolver;
            set
            {
                _drawerChainResolver = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the resolver responsible for providing attributes for this property.
        /// Setting this value triggers a refresh of the property tree.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when setting to null</exception>
        public IAttributeResolver AttributeResolver
        {
            get => _attributeResolver;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _attributeResolver = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the resolver responsible for providing grouped properties for this property.
        /// Setting this value triggers a refresh of the property tree.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when setting to null</exception>
        public IGroupResolver GroupResolver
        {
            get => _groupResolver;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _groupResolver = value;
                Refresh();
            }
        }

        /// <summary>
        /// Updates the property state, including value entries and child properties.
        /// This method is called automatically during drawing but can be forced manually.
        /// </summary>
        /// <param name="force">Whether to force an update even if the property is already up to date</param>
        internal void Update(bool force = false)
        {
            if (_lastUpdateId == Tree.UpdateId && !force)
            {
                return;
            }

            _lastUpdateId = Tree.UpdateId;

            UpdateValueEntry();

            if (Children == null && IsAllowChildren)
            {
                ChildrenResolver = Info.PropertyResolverLocator.GetResolver(this);
                Children = new PropertyChildren(this);
            }

            if (Children != null)
            {
                Children.Update();
            }
        }

        private void UpdateValueEntry()
        {
            if (BaseValueEntry == null)
                return;
            BaseValueEntry.Update();

            if (!Info.PropertyType.IsValueType)
            {
                var valueType = BaseValueEntry.ValueType;
                if (valueType != BaseValueEntry.BaseValueType)
                {
                    if (ValueEntry == null ||
                        (ValueEntry is IPropertyValueEntryWrapper && ValueEntry.RuntimeValueType != valueType) ||
                        (!(ValueEntry is IPropertyValueEntryWrapper) && ValueEntry.RuntimeValueType != ValueEntry.BaseValueType))
                    {
                        if (ValueEntry != null && ValueEntry != BaseValueEntry)
                        {
                            ValueEntry.Dispose();
                        }

                        var wrapperType = typeof(PropertyValueEntryWrapper<,>).MakeGenericType(valueType, BaseValueEntry.BaseValueType);
                        ValueEntry = wrapperType.CreateInstance<IPropertyValueEntry>(BaseValueEntry);
                        Refresh();
                    }
                }
                else if (ValueEntry != BaseValueEntry)
                {
                    if (ValueEntry != null)
                    {
                        ValueEntry.Dispose();
                    }

                    ValueEntry = BaseValueEntry;
                    Refresh();
                }
            }
            else if (ValueEntry == null)
            {
                ValueEntry = BaseValueEntry;
                Refresh();
            }

            ValueEntry.Update();
        }

        /// <summary>
        /// Refreshes the property by reinitializing all resolvers and clearing child properties.
        /// This method should be called when the property configuration changes significantly.
        /// </summary>
        public void Refresh()
        {
            if (_childrenResolver != null)
            {
                if (_childrenResolver.IsInitialized)
                {
                    _childrenResolver.Deinitialize();
                }

                _childrenResolver = Info.PropertyResolverLocator.GetResolver(this);
            }

            ReinitializeResolver(_childrenResolver);
            ReinitializeResolver(_drawerChainResolver);
            ReinitializeResolver(_attributeResolver);
            ReinitializeResolver(_groupResolver);

            if (Children != null)
            {
                Children.Clear();
            }
        }

        /// <summary>
        /// Gets the drawer chain for this property, which determines how the property is drawn in the inspector.
        /// </summary>
        /// <returns>The drawer chain for this property</returns>
        /// <exception cref="InvalidOperationException">Thrown when the drawer chain resolver is null</exception>
        public DrawerChain GetDrawerChain()
        {
            if (DrawerChainResolver == null)
            {
                throw new InvalidOperationException($"DrawerChainResolver of property '{Path}' cannot be null.");
            }

            return DrawerChainResolver.GetDrawerChain();
        }

        /// <summary>
        /// Gets all attributes associated with this property.
        /// </summary>
        /// <returns>An array of all attributes on this property</returns>
        public Attribute[] GetAttributes()
        {
            return AttributeResolver.GetAttributes();
        }

        /// <summary>
        /// Gets all properties that belong to the same group as this property.
        /// </summary>
        /// <param name="beginGroupAttributeType">The type of the group attribute to search for</param>
        /// <returns>An array of properties in the same group</returns>
        public InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType)
        {
            return GroupResolver.GetGroupProperties(beginGroupAttributeType);
        }

        /// <summary>
        /// Gets a specific attribute of type T from this property.
        /// </summary>
        /// <typeparam name="T">The type of attribute to retrieve</typeparam>
        /// <returns>The attribute instance, or null if not found</returns>
        public T GetAttribute<T>()
            where T : Attribute
        {
            foreach (var attribute in GetAttributes())
            {
                if (attribute is T attr)
                {
                    return attr;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the source information for a specific attribute on this property.
        /// </summary>
        /// <param name="attribute">The attribute to get source information for</param>
        /// <returns>Information about where the attribute was defined</returns>
        public AttributeSource GetAttributeSource(Attribute attribute)
        {
            return AttributeResolver.GetAttributeSource(attribute);
        }

        /// <summary>
        /// Draws the property in the Unity inspector using the default label.
        /// This method handles property updates, skip logic, and drawer chain execution.
        /// </summary>
        public void Draw()
        {
            Draw(Label);
        }

        /// <summary>
        /// Draws the property in the Unity inspector using a custom label.
        /// This method handles property updates, skip logic, and drawer chain execution.
        /// </summary>
        /// <param name="label">The custom label to use for drawing</param>
        public void Draw(GUIContent label)
        {
            Update();

            if (SkipDrawCount > 0)
            {
                SkipDrawCount--;
                return;
            }

            var chain = GetDrawerChain();
            chain.Reset();

            if (chain.MoveNext())
            {
                EditorGUI.BeginDisabledGroup(IsSelfReadOnly);
                chain.Current.DrawProperty(label);
                EditorGUI.EndDisabledGroup();
            }
        }

        private void ReinitializeResolver(IInitializableResolver resolver)
        {
            if (resolver != null)
            {
                if (resolver.IsInitialized)
                {
                    resolver.Deinitialize();
                }

                resolver.Property = this;
                resolver.Initialize();
            }
        }

        /// <summary>
        /// Determines whether this property can have child properties by checking various conditions:
        /// - Logic root properties always allow children
        /// - Properties with value types that support children (complex types, collections)
        /// - Properties with specific attributes that indicate inspector expansion
        /// </summary>
        /// <returns>True if this property can have children, false otherwise</returns>
        private bool IsAllowChildrenImpl()
        {
            if (Info.IsLogicRoot)
            {
                return true;
            }

            // if (Info.TryGetMemberInfo(out var memberInfo))
            // {
            //     if (memberInfo is FieldInfo fieldInfo)
            //     {
            //         return InspectorPropertyInfoUtility.IsAllowChildrenField(fieldInfo);
            //     }
            //     else if (memberInfo is PropertyInfo propertyInfo)
            //     {
            //         throw new NotImplementedException();
            //     }
            //     else
            //     {
            //         return false;
            //     }
            // }

            if (ValueEntry != null)
            {
                var valueType = ValueEntry.ValueType;
                if (InspectorPropertyInfoUtility.IsAllowChildrenTypeLeniently(valueType))
                {
                    return true;
                }

                // if (valueType.IsDefined<SerializableAttribute>() ||
                //     valueType.IsDefined<ShowInInspectorAttribute>(true))
                // {
                //     return true;
                // }
                //
                // if (Parent.ChildrenResolver is ICollectionResolver)
                // {
                //     return true;
                // }
            }

            return false;
        }

        public override string ToString()
        {
            return $"{Path}";
        }

        /// <summary>
        /// Disposes the property and all associated resources, including resolvers, value entries, and children.
        /// This method should be called when the property is no longer needed to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            if (_childrenResolver?.IsInitialized == true)
            {
                _childrenResolver.Deinitialize();
                _childrenResolver = null;
            }

            if (_drawerChainResolver?.IsInitialized == true)
            {
                _drawerChainResolver.Deinitialize();
                _drawerChainResolver = null;
            }

            if (_attributeResolver?.IsInitialized == true)
            {
                _attributeResolver.Deinitialize();
                _attributeResolver = null;
            }

            if (_groupResolver?.IsInitialized == true)
            {
                _groupResolver.Deinitialize();
                _groupResolver = null;
            }

            BaseValueEntry?.Dispose();
            if (ValueEntry != BaseValueEntry)
            {
                ValueEntry?.Dispose();
                ValueEntry = null;
            }

            Children?.Dispose();
            BaseValueEntry = null;
            Children = null;
            Info = null;
            Tree = null;
            Parent = null;
        }
    }
}
