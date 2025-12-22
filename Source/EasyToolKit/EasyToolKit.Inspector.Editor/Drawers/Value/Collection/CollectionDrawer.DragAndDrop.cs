using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public partial class CollectionDrawer<T>
    {
        private DropZoneHandle _dropZone;
        private bool _isDraggable;
        private bool _isAboutToDroppingUnityObjects;
        private bool _isDroppingUnityObjects;

        private void InitializeDragAndDrop()
        {
        }

        private void UpdateDragAndDrop()
        {
            _isDraggable = !_isReadOnly &&
                           (_listDrawerSettings?.IsDefinedDraggable == true
                               ? _listDrawerSettings.Draggable
                               : CollectionDrawerStyles.DefaultDraggable);
        }

        private DropZoneHandle BeginDropZone()
        {
            // Check for the new ordered collection operation interface
            if (_orderedCollectionAccessor == null) return null;

            var dropZone = DragAndDropManager.BeginDropZone(Property.Tree.GetHashCode() + "-" + Property.Path,
                _collectionStructureResolver.ItemType, true);

            if (Event.current.type == EventType.Repaint && DragAndDropManager.IsDragInProgress)
            {
                var rect = dropZone.Rect;
                dropZone.Rect = rect;
            }

            dropZone.Enabled = _isReadOnly == false;
            _dropZone = dropZone;
            return dropZone;
        }


        private void EndDropZone()
        {
            // Check for the new ordered collection operation interface
            if (_orderedCollectionOperation == null) return;

            if (_dropZone.IsReadyToClaim)
            {
                CollectionDrawerStaticContext.CurrentDraggingPropertyInfo = null;
                CollectionDrawerStaticContext.CurrentDroppingPropertyInfo = Property;
                object droppedObject = _dropZone.ClaimObject();

                if (_dropZone.IsCrossWindowDrag)
                {
                    // If it's a cross-window drag, the changes will for some reason be lost if we don't do this.
                    EasyGUIHelper.RequestRepaint();
                    EditorApplication.delayCall += () =>
                    {
                        DoInsertElement(Mathf.Clamp(_insertAt.Value, 0, Property.Children.Count), droppedObject);
                    };
                }
                else
                {
                    DoInsertElement(Mathf.Clamp(_insertAt.Value, 0, Property.Children.Count), droppedObject);
                }
            }
            else if (_isDraggable)
            {
                var droppedObjects = HandleUnityObjectsDrop();
                if (droppedObjects != null)
                {
                    foreach (var obj in droppedObjects)
                    {
                        object[] values = new object[Property.Tree.Targets.Count];

                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = obj;
                        }

                        DoInsertElement(Mathf.Clamp(_insertAt.Value, 0, Property.Children.Count), values);
                    }
                }
            }

            DragAndDropManager.EndDropZone();
        }

        private UnityEngine.Object[] HandleUnityObjectsDrop()
        {
            if (_isReadOnly) return null;

            var eventType = Event.current.type;
            if (eventType == EventType.Layout)
            {
                _isAboutToDroppingUnityObjects = false;
            }

            if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform) &&
                _dropZone.Rect.Contains(Event.current.mousePosition))
            {
                UnityEngine.Object[] objReferences = null;

                if (DragAndDrop.objectReferences.Any(n =>
                        n != null && _collectionStructureResolver.ItemType.IsAssignableFrom(n.GetType())))
                {
                    objReferences = DragAndDrop.objectReferences
                        .Where(x => x != null && _collectionStructureResolver.ItemType.IsAssignableFrom(x.GetType()))
                        .Reverse().ToArray();
                }
                else if (_collectionStructureResolver.ItemType.IsInheritsFrom(typeof(Component)))
                {
                    objReferences = DragAndDrop.objectReferences.OfType<GameObject>()
                        .Select(x => x.GetComponent(_collectionStructureResolver.ItemType)).Where(x => x != null).Reverse()
                        .ToArray();
                }
                else if (_collectionStructureResolver.ItemType.IsInheritsFrom(typeof(Sprite)) &&
                         DragAndDrop.objectReferences.Any(n => n is Texture2D && AssetDatabase.Contains(n)))
                {
                    objReferences = DragAndDrop.objectReferences.OfType<Texture2D>().Select(x =>
                    {
                        var path = AssetDatabase.GetAssetPath(x);
                        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    }).Where(x => x != null).Reverse().ToArray();
                }

                bool acceptsDrag = objReferences != null && objReferences.Length > 0;

                if (acceptsDrag)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                    _isAboutToDroppingUnityObjects = true;
                    _isDroppingUnityObjects = _isAboutToDroppingUnityObjects;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        return objReferences;
                    }
                }
            }

            if (eventType == EventType.Repaint)
            {
                _isDroppingUnityObjects = _isAboutToDroppingUnityObjects;
            }

            return null;
        }

        private void EndDragHandle(int index)
        {
            var handle = DragAndDropManager.EndDragHandle();

            if (handle.IsDragging)
            {
                Property.Tree.QueueCallback(() =>
                {
                    if (DragAndDropManager.CurrentDraggingHandle != null)
                    {
                        CollectionDrawerStaticContext.DelayedGUIDrawer.Draw(Event.current.mousePosition -
                                                                            DragAndDropManager.CurrentDraggingHandle
                                                                                .MouseDownPostionOffset);
                    }
                });
            }
        }

        private DragHandle BeginDragHandle(int index)
        {
            var child = Property.Children[index];
            var dragHandle = DragAndDropManager.BeginDragHandle(child, child.ValueEntry.WeakSmartValue,
                _isReadOnly ? DragAndDropMethods.Reference : DragAndDropMethods.Move);
            dragHandle.Enabled = _isDraggable;

            if (dragHandle.OnDragStarted)
            {
                CollectionDrawerStaticContext.CurrentDroppingPropertyInfo = null;
                CollectionDrawerStaticContext.CurrentDraggingPropertyInfo = Property.Children[index];
                dragHandle.OnDragFinnished = dropEvent =>
                {
                    if (dropEvent == DropEvents.Moved)
                    {
                        if (dragHandle.IsCrossWindowDrag ||
                            (CollectionDrawerStaticContext.CurrentDroppingPropertyInfo != null &&
                             CollectionDrawerStaticContext.CurrentDroppingPropertyInfo.Tree != Property.Tree))
                        {
                            // Make sure drop happens a bit later, as deserialization and other things sometimes
                            // can override the change.
                            EasyGUIHelper.RequestRepaint();
                            EditorApplication.delayCall += () => { DoRemoveElementAt(index); };
                        }
                        else
                        {
                            DoRemoveElementAt(index);
                        }
                    }

                    CollectionDrawerStaticContext.CurrentDraggingPropertyInfo = null;
                };
            }

            return dragHandle;
        }
    }
}
