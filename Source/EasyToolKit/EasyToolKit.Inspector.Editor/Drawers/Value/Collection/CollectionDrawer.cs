using System;
using System.Collections;
using System.Reflection;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class CollectionDrawerStaticContext
    {
        public static InspectorProperty CurrentDraggingPropertyInfo;
        public static InspectorProperty CurrentDroppingPropertyInfo;
        public static DelayedGUIDrawer DelayedGUIDrawer = new DelayedGUIDrawer();
        [CanBeNull] public static Func<ValueDropdownList> NextElementDropdownListGetter;
    }

    [DrawerPriority(DrawerPriorityLevel.Value + 9)]
    public partial class CollectionDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.ChildrenResolver is ICollectionStructureResolver;
        }

        private ICollectionStructureResolver _collectionStructureResolver;
        private IChangeManager _changeManager;
        private ICollectionOperationResolver _collectionOperationResolver;
        [CanBeNull] private IOrderedCollectionOperationResolver _orderedCollectionOperationResolver;
        [CanBeNull] private ListDrawerSettingsAttribute _listDrawerSettings;
        [CanBeNull] private Type _listDrawerTargetType;
        private bool _isListDrawerClassAttribute;
        [CanBeNull] private Func<ValueDropdownList> _elementDropdownListGetter;

        private bool _isReadOnly;
        private int _count;

        private string _error;

        protected override void Initialize()
        {
            _collectionStructureResolver = (ICollectionStructureResolver)Property.ChildrenResolver;
            _changeManager = _collectionStructureResolver.ChangeManager;
            _collectionOperationResolver = _collectionStructureResolver.OperationResolver;
            _orderedCollectionOperationResolver = _collectionOperationResolver as IOrderedCollectionOperationResolver;

            _listDrawerSettings = Property.GetAttribute<MetroListDrawerSettingsAttribute>();
            if (_listDrawerSettings == null)
            {
                _listDrawerSettings = Property.GetAttribute<ListDrawerSettingsAttribute>();
            }

            if (_listDrawerSettings != null)
            {
                _isListDrawerClassAttribute = Property.GetAttributeSource(_listDrawerSettings) == AttributeSource.Type;
                _listDrawerTargetType = _isListDrawerClassAttribute
                    ? Property.ValueEntry.ValueType
                    : Property.Parent.ValueEntry.ValueType;
            }

            try
            {
                InitializeLogic();
                InitializeDraw();
                InitializeDragAndDrop();
            }
            catch (Exception e)
            {
                _error = e.Message;
            }
        }

        protected override void DrawProperty(GUIContent label)
        {
            _isReadOnly = _collectionStructureResolver.IsReadOnly || _listDrawerSettings?.IsReadOnly == true;
            _elementDropdownListGetter = CollectionDrawerStaticContext.NextElementDropdownListGetter;
            CollectionDrawerStaticContext.NextElementDropdownListGetter = null;
            UpdateDragAndDrop();
            UpdateDraw(label);
            UpdateLogic();
        }
    }
}
