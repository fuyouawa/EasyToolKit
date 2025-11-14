using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public struct DrawingToolContext
    {
        public TileWorldDesigner Designer;
        public Vector3 HitPoint;
        public Vector3 HitTilePosition;
        public TerrainDefinition TerrainDefinition;
    }

    public interface IDrawingTool
    {
        void OnSceneGUI(DrawingToolContext context);
        void OnSwitchTool(TileWorldDesigner designer, DrawMode currentDrawMode);
        void OnLeaveSceneView(TileWorldDesigner designer);
    }
}
