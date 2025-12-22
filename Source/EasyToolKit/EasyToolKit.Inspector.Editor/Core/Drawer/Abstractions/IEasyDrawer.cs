using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public interface IEasyDrawer : IHandler
    {
        bool SkipWhenDrawing { get; set; }

        void Draw(GUIContent label);
    }
}
