using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 100)]
    public class TileCoordinateDrawer : EasyValueDrawer<TileCoordinate>
    {
        private InspectorProperty _coordinateProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _coordinateProperty = Property.Children["_coordinate"];
        }

        protected override void DrawProperty(GUIContent label)
        {
            _coordinateProperty.Draw(label);
        }
    }
}
