using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 100)]
    public class TileCoordinateDrawer : EasyValueDrawer<TileCoordinate>
    {
        private IElement _coordinateElement;

        protected override void Initialize()
        {
            base.Initialize();
            _coordinateElement = Element.Children!["_coordinate"];
        }

        protected override void Draw(GUIContent label)
        {
            _coordinateElement.Draw(label);
        }
    }
}
