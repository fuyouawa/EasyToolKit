using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{

    /// <summary>
    /// <para>This class is due to undergo refactoring. Use the new DragAndDropUtilities instead.</para>
    /// </summary>
    /// <seealso cref="DragAndDropUtilities"/>
    public static class DragAndDropManager
    {
        private static readonly GUIContextCache<(object, object), DropZoneHandle> DropZoneHandlesCache = new GUIContextCache<(object, object), DropZoneHandle>();
        private static readonly GUIContextCache<(object, object), DragHandle> DragHandlesCache = new GUIContextCache<(object, object), DragHandle>();

        private static GUIScopeStack<DragHandle> dragHandles = new GUIScopeStack<DragHandle>();

        private static GUIScopeStack<DropZoneHandle> dropZoneHandles = new GUIScopeStack<DropZoneHandle>();

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DragHandle CurrentDraggingHandle { get; internal set; }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DropZoneHandle CurrentHoveringDropZone { get; internal set; }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static bool IsDragInProgress
        {
            get { return CurrentDraggingHandle != null; }
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static bool IsHoveringDropZone
        {
            get { return CurrentHoveringDropZone != null; }
        }

        internal static bool WasDragPerformed = false;

        private static object dropZoneKey = new object();

        private static object draggableKey = new object();

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static bool AllowDrop = true;

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DropZoneHandle BeginDropZone<T>(object key) where T : struct
        {
            return BeginDropZone(key, typeof(T), false);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DropZoneHandle BeginDropZone<T>(object key, bool canAcceptMove) where T : class
        {
            return BeginDropZone(key, typeof(T), canAcceptMove);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DropZoneHandle BeginDropZone(object key, Type type, bool canAcceptMove)
        {
            Update();
            GUILayout.BeginVertical();
            var rect = EasyGUIHelper.GetCurrentLayoutRect();
            var cacheKey = (key, type);
            var dropZoneHandle = DropZoneHandlesCache.GetOrCreate(cacheKey);

            dropZoneHandle.Type = type;
            dropZoneHandle.CanAcceptMove = canAcceptMove;
            dropZoneHandle.LayoutDepth = dropZoneHandles.Count;
            dropZoneHandles.Push(dropZoneHandle);
            dropZoneHandle.Update(EventType.Layout);
            dropZoneHandle.SourceWindow = EasyGUIHelper.CurrentWindow;

            if (Event.current.type == EventType.Repaint)
            {
                dropZoneHandle.Rect = rect;
            }

            return dropZoneHandle;
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DropZoneHandle EndDropZone()
        {
            var dropZoneHandle = dropZoneHandles.Pop();
            GUILayout.EndVertical();
            dropZoneHandle.Update(EventType.Repaint);
            return dropZoneHandle;
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DragHandle BeginDragHandle(object key, object dragObject, DragAndDropMethods defaultMethod = DragAndDropMethods.Move)
        {
            return BeginDragHandle(key, dragObject, false, defaultMethod);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DragHandle BeginDragHandle(object key, object dragObject, bool isVirtualDragHandle, DragAndDropMethods defaultMethod = DragAndDropMethods.Move)
        {
            Update();

            if (Event.current.type == EventType.Repaint)
            {
                EasyGUIHelper.BeginLayoutMeasuring();
            }

            var dragObjectType = dragObject?.GetType();
            var cacheKey = (key, dragObjectType);
            var dragHandle = DragHandlesCache.GetOrCreate(cacheKey);

            dragHandle.Object = dragObject;
            dragHandle.DragAndDropMethod = defaultMethod;
            dragHandle.LayoutDepth = dragHandles.Count;
            dragHandles.Push(dragHandle);

            dragHandle.SourceWindow = EasyGUIHelper.CurrentWindow;
            return dragHandle;
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static DragHandle EndDragHandle()
        {
            var dragHandle = dragHandles.Pop();

            if (Event.current.type == EventType.Repaint)
            {
                var rect = EasyGUIHelper.EndLayoutMeasuring();
                if (dragHandle.IsDragging == false)
                {
                    dragHandle.TempRect = rect;
                }
            }

            dragHandle.Update();
            dragHandle.Rect = dragHandle.TempRect;

            return dragHandle;
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        private static GUIFrameCounter guiState = new GUIFrameCounter();

        private static void Update()
        {
            var frameInfo = guiState.Update();
            if (frameInfo.IsNewFrame)
            {
                // 在新帧开始时清理过期的缓存
                DropZoneHandlesCache.ClearExpired();
                DragHandlesCache.ClearExpired();

                if (IsDragInProgress)
                {
                    AllowDrop = true;
                    EasyGUIHelper.RequestRepaint();
                }
            }

            if (IsDragInProgress)
            {
                // Ensure drop event!
                if (WasDragPerformed == false)
                {
                    if (Event.current.type == EventType.DragPerform ||
                        Event.current.type == EventType.MouseMove ||
                        Event.current.type == EventType.MouseUp)
                    {
                        //Debug.Log(Event.current.type + " - " + Event.current.rawType);
                        WasDragPerformed = true;
                        if (Event.current.type == EventType.DragPerform && IsHoveringDropZone)
                        {
                            Event.current.Use();
                        }
                        EasyGUIHelper.RequestRepaint();
                    }
                }

                if (IsHoveringDropZone && EasyGUIHelper.CurrentWindowHasFocus)
                {
                    if (CurrentHoveringDropZone.IsAccepted == false)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    else if (CurrentDraggingHandle.CurrentMethod == DragAndDropMethods.Move)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    else if (CurrentDraggingHandle.CurrentMethod == DragAndDropMethods.Reference)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    else if (CurrentDraggingHandle.CurrentMethod == DragAndDropMethods.Copy)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                }
            }
            else
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    // TODO Start virtual drag.
                }
            }
        }
    }
}
