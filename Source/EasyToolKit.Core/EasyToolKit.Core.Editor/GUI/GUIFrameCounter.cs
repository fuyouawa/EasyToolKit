using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    /// <summary>
    /// A utility class for properly counting frames and helps determine when a frame has started in an editor window.
    /// </summary>
    //TODO 版权问题
    public class GUIFrameCounter
    {
        private int frameCount;
        private bool isNewFrame = true;
        private bool nextEventIsNew = true;

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        public int FrameCount
        {
            get { return this.frameCount; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new frame.
        /// </summary>
        public bool IsNewFrame
        {
            get { return this.isNewFrame; }
        }

        /// <summary>
        /// Updates the frame counter and returns itself.
        /// </summary>
        public GUIFrameCounter Update()
        {
            if (Event.current == null)
            {
                return this;
            }

            var e = Event.current.type;

            if (e == EventType.Repaint)
            {
                this.nextEventIsNew = true;
                this.isNewFrame = false;
                return this;
            }

            if (this.nextEventIsNew && e != EventType.Repaint)
            {
                this.frameCount++;
                this.nextEventIsNew = false;
                this.isNewFrame = true;
                return this;
            }

            this.isNewFrame = false;

            return this;
        }
    }
}
