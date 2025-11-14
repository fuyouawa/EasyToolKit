using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public abstract class DraggableDrawingTool : IDrawingTool
    {
        private List<TileCoordinate> _dragTileCoordinatePath = new List<TileCoordinate>();
        private bool _isDragging = false;

        public void OnSceneGUI(DrawingToolContext context)
        {
            var tileSize = context.Designer.TileWorldAsset.TileSize;

            if (!IsInRange(context.Designer, context.HitTilePosition))
            {
                return;
            }

            var tileCoordinate = context.Designer.StartPoint.PositionToTileCoordinate(context.HitTilePosition, tileSize);

            var newTilePosition = AdjustTilePosition(context, _dragTileCoordinatePath);
            if (newTilePosition != context.HitTilePosition)
            {
                context.HitTilePosition = newTilePosition;
                if (!IsInRange(context.Designer, context.HitTilePosition))
                {
                    return;
                }

                tileCoordinate = context.Designer.StartPoint.PositionToTileCoordinate(context.HitTilePosition, tileSize);
            }

            if (!FilterHitTile(context, tileCoordinate))
            {
                return;
            }

            var drawingTileCoordinates = GetDrawingTileCoordinates(context, _dragTileCoordinatePath);

            // Handle mouse interactions
            if (IsMouseDown())
            {
                // Start a new drag operation
                _dragTileCoordinatePath.Clear();
                _dragTileCoordinatePath.Add(tileCoordinate);
                _isDragging = true;
                FinishMouse();
            }
            else if (IsMouseDrag() && _isDragging)
            {
                // Continue the drag operation
                if (!_dragTileCoordinatePath.Contains(tileCoordinate))
                {
                    _dragTileCoordinatePath.Add(tileCoordinate);
                }
                FinishMouse();
            }
            else if (IsMouseUp() && _isDragging)
            {
                // End the drag operation - now apply all changes at once
                if (_dragTileCoordinatePath.Count > 0)
                {
                    // Record a single undo operation for all changes
                    Undo.RecordObject(context.Designer.TileWorldAsset, $"Draw tiles in {context.Designer.TileWorldAsset.name}");

                    DoTiles(context, drawingTileCoordinates);

                    // Mark the asset as dirty once for all changes
                    EasyEditorUtility.SetUnityObjectDirty(context.Designer.TileWorldAsset);
                }

                _isDragging = false;
                _dragTileCoordinatePath.Clear();
                FinishMouse();
            }

            var hitColor = GetHitColor(context);
            DrawCursorCube(context, tileCoordinate, hitColor);

            // Draw all tiles in the drag operation
            foreach (var dragTileCoordinate in drawingTileCoordinates)
            {
                DrawHitCube(context, dragTileCoordinate, hitColor);
            }
        }

        public virtual void OnSwitchTool(TileWorldDesigner designer, DrawMode currentDrawMode)
        {
            Clear(designer);
        }

        public virtual void OnLeaveSceneView(TileWorldDesigner designer)
        {
            Clear(designer);
        }

        protected virtual void Clear(TileWorldDesigner designer)
        {
            _dragTileCoordinatePath.Clear();
        }

        protected virtual void DrawCursorCube(DrawingToolContext context, TileCoordinate tileCoordinate, Color hitColor)
        {
            var tileSize = context.Designer.TileWorldAsset.TileSize;
            var surroundingColor = Color.white.SetA(0.2f);
            TileWorldHandles.DrawHitCube(
                tileCoordinate,
                tileSize,
                context.Designer.Settings.FillCursorDebugCube,
                hitColor,
                surroundingColor);
        }

        protected virtual void DrawHitCube(DrawingToolContext context, TileCoordinate tileCoordinate, Color hitColor)
        {
            var tileSize = context.Designer.TileWorldAsset.TileSize;
            var dragWorldPosition = context.Designer.StartPoint.TileCoordinateToPosition(tileCoordinate, tileSize);
            TileWorldHandles.DrawHitCube(
                dragWorldPosition,
                tileSize,
                context.Designer.Settings.FillCursorDebugCube,
                hitColor.MulA(0.7f));
        }

        protected virtual Vector3 AdjustTilePosition(
            DrawingToolContext context,
            IReadOnlyList<TileCoordinate> dragTileCoordinatePath)
        {
            return context.HitTilePosition;
        }

        protected virtual IReadOnlyList<TileCoordinate> GetDrawingTileCoordinates(
            DrawingToolContext context,
            IReadOnlyList<TileCoordinate> dragTileCoordinatePath)
        {
            return dragTileCoordinatePath;
        }

        protected abstract Color GetHitColor(DrawingToolContext context);

        protected abstract void DoTiles(DrawingToolContext context, IReadOnlyList<TileCoordinate> tileCoordinates);

        protected virtual bool FilterHitTile(DrawingToolContext context, TileCoordinate tileCoordinate)
        {
            return true;
        }

        private static bool IsMouseDown()
        {
            var e = Event.current;
            return e.type == EventType.MouseDown && e.button == 0;
        }

        private static bool IsMouseDrag()
        {
            var e = Event.current;
            return e.type == EventType.MouseDrag && e.button == 0;
        }

        private static bool IsMouseUp()
        {
            var e = Event.current;
            return e.type == EventType.MouseUp && e.button == 0;
        }

        private static void FinishMouse()
        {
            Event.current.Use();
        }

        private static bool IsInRange(TileWorldDesigner target, Vector3 position)
        {
            var tileSize = target.TileWorldAsset.TileSize;
            var range = target.TileWorldAsset.BaseRange;

            var tilePosition = target.StartPoint.PositionToTileCoordinate(position, tileSize);

            if (tilePosition.X < 0 || tilePosition.X >= range.x ||
                tilePosition.Z < 0 || tilePosition.Z >= range.y ||
                tilePosition.Y < 0)
                return false;

            return true;
        }
    }
}
