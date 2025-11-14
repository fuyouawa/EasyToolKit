using System;
using System.Reflection;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorProperty : IDisposable
    {
        private string _niceName;
        private PropertyState _state;
        private bool _isSelfReadOnly;
        private long _lastSelfReadOnlyUpdateId;
        private bool _isAllowChildren;
        private long _lastAllowChildrenUpdateId;
        private string _unityPath;

        private IPropertyResolver _childrenResolver;
        private IGroupResolver _groupResolver;
        private IDrawerChainResolver _drawerChainResolver;
        private IAttributeResolver _attributeResolver;
        private long _lastUpdateId;

        public InspectorProperty Parent { get; private set; }
        public PropertyTree Tree { get; private set; }

        [CanBeNull] public PropertyChildren Children { get; private set; }
        public InspectorPropertyInfo Info { get; private set; }

        [CanBeNull] public IPropertyValueEntry BaseValueEntry { get; private set; }
        [CanBeNull] public IPropertyValueEntry ValueEntry { get; private set; }
        public string Path { get; private set; }

        public GUIContent Label { get; set; }

        public int Index { get; private set; }
        public int SkipDrawCount { get; set; }

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

        public string Name => Info.PropertyName;

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

        [CanBeNull]
        public IPropertyResolver ChildrenResolver
        {
            get => _childrenResolver;
            set
            {
                _childrenResolver = value;
                Refresh();
            }
        }

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
                        (!(ValueEntry is IPropertyValueEntryWrapper) &&
                         ValueEntry.RuntimeValueType != ValueEntry.BaseValueType))
                    {
                        if (ValueEntry != null)
                        {
                            ValueEntry.Dispose();
                        }

                        var wrapperType =
                            typeof(PropertyValueEntryWrapper<,>).MakeGenericType(valueType,
                                BaseValueEntry.BaseValueType);
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

        public DrawerChain GetDrawerChain()
        {
            if (DrawerChainResolver == null)
            {
                throw new InvalidOperationException($"DrawerChainResolver of property '{Path}' cannot be null.");
            }

            return DrawerChainResolver.GetDrawerChain();
        }

        public Attribute[] GetAttributes()
        {
            return AttributeResolver.GetAttributes();
        }

        public InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType)
        {
            return GroupResolver.GetGroupProperties(beginGroupAttributeType);
        }

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

        public AttributeSource GetAttributeSource(Attribute attribute)
        {
            return AttributeResolver.GetAttributeSource(attribute);
        }

        public void Draw()
        {
            Draw(Label);
        }

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
