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
        public static IElement CurrentDraggingPropertyInfo;
        public static IElement CurrentDroppingPropertyInfo;
        public static DelayedGUIDrawer DelayedGUIDrawer = new DelayedGUIDrawer();
        [CanBeNull] public static Func<ValueDropdownList> NextElementDropdownListGetter;
    }

    [DrawerPriority(DrawerPriorityLevel.Value + 9)]
    public partial class CollectionDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawElement(IValueElement element)
        {
            return element.Definition.Roles.IsCollection();
        }

        public new ICollectionElement Element => base.Element as ICollectionElement;
        public new ICollectionEntry ValueEntry => base.ValueEntry as ICollectionEntry;

        private IOrderedCollectionAccessor _orderedCollectionAccessor;
        private Type _listDrawerTargetType;
        [CanBeNull] private ListDrawerSettingsAttribute _listDrawerSettings;
        [CanBeNull] private Func<ValueDropdownList> _elementDropdownListGetter;

        private bool _isReadOnly;
        private int _count;

        private string _error;

        protected override void Initialize()
        {
            _orderedCollectionAccessor = ValueEntry as IOrderedCollectionAccessor;

            _listDrawerSettings = Element.GetAttribute<MetroListDrawerSettingsAttribute>();
            if (_listDrawerSettings == null)
            {
                _listDrawerSettings = Element.GetAttribute<ListDrawerSettingsAttribute>();
            }

            if (_listDrawerSettings != null)
            {
                _listDrawerTargetType = ElementUtility.GetOwnerTypeWithAttribute(Element, _listDrawerSettings);
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

        protected override void Draw(GUIContent label)
        {
            _isReadOnly = ValueEntry.IsReadOnly || _listDrawerSettings?.IsReadOnly == true;
            _elementDropdownListGetter = CollectionDrawerStaticContext.NextElementDropdownListGetter;
            CollectionDrawerStaticContext.NextElementDropdownListGetter = null;
            UpdateDragAndDrop();
            UpdateDraw(label);
            UpdateLogic();
        }
    }
}
