using UnityEngine;

namespace Script.Manager.Events
{
    public class CameraRectEventData
    {
        public Rect cameraRect { get; private set; }

        public CameraRectEventData(Rect cameraRect)
        {
            this.cameraRect = cameraRect;
        }
    }
}