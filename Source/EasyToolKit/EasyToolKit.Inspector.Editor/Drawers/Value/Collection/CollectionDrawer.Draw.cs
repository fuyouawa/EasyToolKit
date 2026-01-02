using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class CollectionDrawerStyles
    {
        private static GUIStyle s_metroHeaderLabelStyle;
        private static GUIStyle s_metroHeaderFoldoutStyle;
        private static GUIStyle s_listItemStyle;

        public static GUIStyle MetroHeaderLabelStyle
        {
            get
            {
                if (s_metroHeaderLabelStyle == null)
                {
                    s_metroHeaderLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_metroHeaderLabelStyle.fontSize += 1;
                }

                return s_metroHeaderLabelStyle;
            }
        }

        public static GUIStyle MetroHeaderFoldoutStyle
        {
            get
            {
                if (s_metroHeaderFoldoutStyle == null)
                {
                    s_metroHeaderFoldoutStyle = new GUIStyle(EasyGUIStyles.Foldout)
                    {
                    };
                    s_metroHeaderFoldoutStyle.margin.top = 7;
                    s_metroHeaderFoldoutStyle.fontSize += 1;
                }

                return s_metroHeaderFoldoutStyle;
            }
        }

        public static readonly Color ListSelectionBorderColor = new Color(0.24f, 0.59f, 0.9f);

        public static readonly Color MetroHeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
        public static readonly Color MetroItemBackgroundColor = new Color(0.9f, 0.9f, 0.9f);

        public static GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(25, 20, 3, 3)
        };

        public static GUIStyle MetroListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(30, 37, 6, 6)
        };

        public static readonly bool DefaultShowIndexLabel = true;
        public static readonly bool DefaultShowFoldoutHeader = true;
        public static readonly bool DefaultDraggable = true;
    }

    class CollectionItemContext
    {
        public Rect RemoveBtnRect;
        public Rect DragHandleRect;
    }

    public partial class CollectionDrawer<T>
    {
        [CanBeNull] private IExpressionEvaluator<Texture> _iconTextureGetterEvaluator;
        [CanBeNull] private Func<object, int, string> _customIndexLabelFunction;

        private bool _showIndexLabel;
        private bool _hideRemoveButton;
        private bool _hideAddButton;
        private Vector2 _layoutMousePosition;
        private bool _showFoldoutHeader;

        private void InitializeDraw()
        {
            _showIndexLabel = _listDrawerSettings?.IsDefinedShowIndexLabel == true
                ? _listDrawerSettings.ShowIndexLabel
                : CollectionDrawerStyles.DefaultShowIndexLabel;
            if (_listDrawerSettings != null)
            {
                if (_listDrawerSettings is MetroListDrawerSettingsAttribute metroListDrawerSettings)
                {
                    if (metroListDrawerSettings.IconTextureGetter.IsNotNullOrEmpty())
                    {
                        _iconTextureGetterEvaluator = ExpressionEvaluatorFactory
                            .Evaluate<Texture>(metroListDrawerSettings.IconTextureGetter, _listDrawerTargetType)
                            .Build();
                    }
                }

                if (_showIndexLabel && _listDrawerSettings.CustomIndexLabelFunction.IsNotNullOrEmpty())
                {
                    var customIndexLabelFunction = _listDrawerTargetType.GetMethodEx(
                        _listDrawerSettings.CustomIndexLabelFunction,
                        BindingFlagsHelper.All, typeof(int)) ?? throw new Exception(
                        $"Cannot find method '{_listDrawerSettings.CustomIndexLabelFunction}' in '{_listDrawerTargetType}'");
                    _customIndexLabelFunction = (instance, index) =>
                    {
                        return (string)customIndexLabelFunction.Invoke(instance, new object[] { index });
                    };
                }

                if (_listDrawerSettings.CustomRemoveIndexFunction.IsNotNullOrEmpty())
                {
                    var customRemoveIndexFunction = _listDrawerTargetType.GetMethodEx(
                        _listDrawerSettings.CustomRemoveIndexFunction,
                        BindingFlagsHelper.All, typeof(int)) ?? throw new Exception(
                        $"Cannot find method '{_listDrawerSettings.CustomRemoveIndexFunction}' in '{_listDrawerTargetType}'");
                    _customRemoveIndexFunction = (instance, index) =>
                    {
                        customRemoveIndexFunction.Invoke(instance, new object[] { index });
                    };
                }
            }
        }

        private void UpdateDraw(GUIContent label)
        {
            _hideRemoveButton = _listDrawerSettings?.HideRemoveButton == true || _isReadOnly;
            _hideAddButton = _listDrawerSettings?.HideAddButton == true || _isReadOnly;
            _showFoldoutHeader = _listDrawerSettings?.IsDefinedShowFoldoutHeader == true
                ? _listDrawerSettings.ShowFoldoutHeader
                : CollectionDrawerStyles.DefaultShowFoldoutHeader;
            _showFoldoutHeader = _showFoldoutHeader && Element.LogicalChildren.Count > 0;

            CollectionDrawerStyles.ListItemStyle.padding.left = _isDraggable ? 25 : 7;
            CollectionDrawerStyles.ListItemStyle.padding.right = _hideRemoveButton ? 4 : 20;

            if (_error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(_error, MessageType.Error);
                return;
            }

            if (_iconTextureGetterEvaluator != null && _iconTextureGetterEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                _count = Element.LogicalChildren.Count;
            }
            else
            {
                var newCount = Element.LogicalChildren.Count;
                if (_count > newCount)
                {
                    _count = newCount;
                }
            }

            var rect = EasyEditorGUI.BeginIndentedVertical(EasyGUIStyles.PropertyPadding);
            BeginDropZone();
            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                DrawMetroHeader(label);
            }
            else
            {
                DrawHeader(label);
            }

            if (!_showFoldoutHeader || Element.State.Expanded)
            {
                DrawItems();
            }
            EndDropZone();
            EasyEditorGUI.EndIndentedVertical();
        }

        private void DrawHeader(GUIContent label)
        {
            EasyEditorGUI.BeginHorizontalToolbar();

            if (_showFoldoutHeader)
            {
                Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, label ?? GUIContent.none);
            }
            else
            {
                GUILayout.Label(label ?? GUIContent.none);
            }

            GUILayout.FlexibleSpace();

            if (!_hideAddButton)
            {
                var buttonRect = GUILayoutUtility.GetRect(22, 22, GUILayout.ExpandWidth(false));
                if (EasyEditorGUI.ToolbarButton(buttonRect, EasyEditorIcons.Plus))
                {
                    DoAddItem(buttonRect);
                }
            }

            EasyEditorGUI.EndHorizontalToolbar();
        }

        private void DrawMetroHeader(GUIContent label)
        {
            EasyGUIHelper.PushColor(CollectionDrawerStyles.MetroHeaderBackgroundColor);
            EasyEditorGUI.BeginHorizontalToolbar(30);
            EasyGUIHelper.PopColor();

            if (_iconTextureGetterEvaluator != null)
            {
                var iconTexture = _iconTextureGetterEvaluator.Evaluate(ElementUtility.GetOwnerWithAttribute(Element, _listDrawerSettings));
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }


            if (_showFoldoutHeader)
            {
                var rect = EditorGUILayout.GetControlRect(false, 30);
                Element.State.Expanded = EasyEditorGUI.Foldout(
                    rect,
                    Element.State.Expanded,
                    label ?? GUIContent.none,
                    CollectionDrawerStyles.MetroHeaderFoldoutStyle);
            }
            else
            {
                GUILayout.Label(label ?? GUIContent.none, CollectionDrawerStyles.MetroHeaderLabelStyle, GUILayout.Height(30));
            }

            GUILayout.FlexibleSpace();

            if (!_hideAddButton)
            {
                var btnRect = GUILayoutUtility.GetRect(
                    EasyEditorIcons.Plus.HighlightedContent,
                    "Button",
                    GUILayout.ExpandWidth(false),
                    GUILayout.Width(30),
                    GUILayout.Height(30));

                if (GUI.Button(btnRect, GUIContent.none, "Button"))
                {
                    EasyGUIHelper.RemoveFocusControl();
                    DoAddItem(btnRect);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    EasyEditorIcons.Plus.Draw(btnRect.AlignCenter(25, 25));
                }
            }

            EasyEditorGUI.EndHorizontalToolbar();
        }

        private void DrawItems()
        {
            int fromIndex = 0;
            int toIndex = _count;
            var drawEmptySpace = _dropZone != null && _dropZone.IsBeingHovered || _isDroppingUnityObjects;
            float height = drawEmptySpace
                ? _isDroppingUnityObjects ? 16 : (DragAndDropManager.CurrentDraggingHandle.Rect.height - 3)
                : 0;

            var rect = EasyEditorGUI.BeginVerticalList();

            int index = 0;
            // the logical sense index, including dragging items
            int logicIndex = fromIndex;
            // the actual index, excluding dragging items
            int actualIndex = fromIndex;
            for (; logicIndex < toIndex; index++, logicIndex++)
            {
                var dragHandle = BeginDragHandle(logicIndex);
                {
                    if (drawEmptySpace)
                    {
                        var topHalf = dragHandle.Rect;
                        topHalf.height /= 2;
                        if (topHalf.Contains(_layoutMousePosition) || topHalf.y > _layoutMousePosition.y && index == 0)
                        {
                            GUILayout.Space(height);
                            drawEmptySpace = false;
                            _insertAt = actualIndex;
                        }
                    }

                    if (dragHandle.IsDragging == false)
                    {
                        actualIndex++;
                        DrawItem(Element.LogicalChildren[logicIndex], dragHandle, logicIndex, Element.Children[logicIndex]);
                    }
                    else
                    {
                        GUILayout.Space(3);
                        CollectionDrawerStaticContext.DelayedGUIDrawer.Begin(dragHandle.Rect.width,
                            dragHandle.Rect.height, dragHandle.CurrentMethod != DragAndDropMethods.Move);
                        DragAndDropManager.AllowDrop = false;
                        DrawItem(Element.LogicalChildren[logicIndex], dragHandle, logicIndex, Element.Children[logicIndex]);
                        DragAndDropManager.AllowDrop = true;
                        CollectionDrawerStaticContext.DelayedGUIDrawer.End();
                        if (dragHandle.CurrentMethod != DragAndDropMethods.Move)
                        {
                            GUILayout.Space(3);
                        }
                    }

                    if (drawEmptySpace)
                    {
                        var bottomHalf = dragHandle.Rect;
                        bottomHalf.height /= 2;
                        bottomHalf.y += bottomHalf.height;

                        if (bottomHalf.Contains(_layoutMousePosition) ||
                            bottomHalf.yMax < _layoutMousePosition.y && logicIndex + 1 == toIndex)
                        {
                            GUILayout.Space(height);
                            drawEmptySpace = false;
                            _insertAt = Mathf.Min(actualIndex, toIndex);
                        }
                    }
                }
                EndDragHandle(logicIndex);
            }

            if (drawEmptySpace)
            {
                GUILayout.Space(height);
                _insertAt = Event.current.mousePosition.y > rect.center.y ? toIndex : fromIndex;
            }

            if (toIndex == Element.LogicalChildren.Count && Element.ValueEntry.State == ValueEntryState.Mixed)
            {
                EasyEditorGUI.BeginListItem(false);
                GUILayout.Label(EditorHelper.TempContent("------"), EditorStyles.centeredGreyMiniLabel);
                EasyEditorGUI.EndListItem();
            }

            EasyEditorGUI.EndVerticalList();

            if (Event.current.type == EventType.Repaint)
            {
                _layoutMousePosition = Event.current.mousePosition;
            }
        }

        private void DrawItem(ICollectionItemElement itemElement, DragHandle dragHandle, int index, IElement drawnElement)
        {
            var itemContext = itemElement.GetPersistentContext("ItemContext", new CollectionItemContext()).Value;
            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                EasyGUIHelper.PushColor(CollectionDrawerStyles.MetroItemBackgroundColor);
            }

            var rect = BeginDrawItem();

            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                EasyGUIHelper.PopColor();
            }

            {
                if (Event.current.type == EventType.Repaint && !_isReadOnly)
                {
                    DrawItemDragHandle(rect, dragHandle, itemContext);
                }

                EasyGUIHelper.PushHierarchyMode(false);

                DrawItemElement(itemElement, index, drawnElement);

                EasyGUIHelper.PopHierarchyMode();

                if (!_hideRemoveButton)
                {
                    if (DrawItemRemoveButton(itemContext.RemoveBtnRect))
                    {
                        if (_orderedCollectionAccessor != null)
                        {
                            if (index >= 0)
                            {
                                _removeAt = index;
                            }
                        }
                        else
                        {
                            var values = new object[itemElement.ValueEntry.TargetCount];

                            for (int i = 0; i < values.Length; i++)
                            {
                                values[i] = itemElement.ValueEntry.GetWeakValue(i);
                            }

                            _removeValues = values;
                        }
                    }
                }
            }
            EndDrawItem();
        }

        private bool DrawItemRemoveButton(Rect rect)
        {
            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                if (GUI.Button(rect, GUIContent.none, "Button"))
                {
                    EasyGUIHelper.RemoveFocusControl();
                    return true;
                }

                if (Event.current.type == EventType.Repaint)
                {
                    EasyEditorIcons.X.Draw(rect.AlignCenter(25, 25));
                }

                return false;
            }
            else
            {
                return EasyEditorGUI.IconButton(rect, EasyEditorIcons.X);
            }
        }

        private Rect BeginDrawItem()
        {
            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                return EasyEditorGUI.BeginListItem(false, CollectionDrawerStyles.MetroListItemStyle,
                    GUILayout.MinHeight(30),
                    GUILayout.ExpandWidth(true));
            }
            else
            {
                return EasyEditorGUI.BeginListItem(false, CollectionDrawerStyles.ListItemStyle, GUILayout.MinHeight(25),
                    GUILayout.ExpandWidth(true));
            }
        }

        private void EndDrawItem()
        {
            EasyEditorGUI.EndListItem();
        }

        private void DrawItemDragHandle(Rect rect, DragHandle dragHandle, CollectionItemContext itemContext)
        {
            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                dragHandle.DragHandleRect = new Rect(rect.x + 4, rect.y, 23, rect.height);
                itemContext.DragHandleRect = new Rect(
                    rect.x + 4,
                    rect.y + ((int)rect.height - 23) / 2, 23, 23);
                itemContext.RemoveBtnRect = new Rect(
                    itemContext.DragHandleRect.x + rect.width - 37,
                    itemContext.DragHandleRect.y - 3, 30, 30);
            }
            else
            {
                dragHandle.DragHandleRect = new Rect(rect.x + 4, rect.y, 20, rect.height);
                itemContext.DragHandleRect = new Rect(
                    rect.x + 4,
                    rect.y + 2 + ((int)rect.height - 23) / 2, 20, 20);
                itemContext.RemoveBtnRect = new Rect(
                    itemContext.DragHandleRect.x + rect.width - 22,
                    itemContext.DragHandleRect.y + 1, 14, 14);
            }

            if (_isDraggable)
            {
                GUI.Label(itemContext.DragHandleRect, EasyEditorIcons.List.InactiveTexture, GUIStyle.none);
            }
        }

        protected virtual void DrawItemElement(ICollectionItemElement itemElement, int index, IElement drawnElement)
        {
            GUIContent label = null;

            if (_showIndexLabel)
            {
                label = EditorHelper.TempContent($"{index} : ");
                if (_customIndexLabelFunction != null)
                {
                    var target = ElementUtility.GetOwnerWithAttribute(Element, _listDrawerSettings);
                    label.text = _customIndexLabelFunction(target, index);
                }
            }

            if (label != null)
            {
                if (drawnElement.Children != null)
                {
                    drawnElement.State.Expanded = EasyEditorGUI.Foldout(drawnElement.State.Expanded, label);
                    if (drawnElement.State.Expanded)
                    {
                        EditorGUI.indentLevel++;
                        drawnElement.Draw();
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    drawnElement.Draw(label);
                }
            }
            else
            {
                drawnElement.Draw(null);
            }
        }
    }
}
