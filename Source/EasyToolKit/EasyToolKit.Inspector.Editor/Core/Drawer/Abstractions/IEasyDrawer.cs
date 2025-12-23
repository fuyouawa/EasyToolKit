using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public interface IEasyDrawer : IHandler
    {
        /// <summary>
        /// Gets or sets the <see cref="IElement"/> that this drawer is associated with.
        /// </summary>
        IElement Element { get; set; }

        bool SkipWhenDrawing { get; set; }

        void Draw(GUIContent label);
    }
}
