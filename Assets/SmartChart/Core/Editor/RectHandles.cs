using UnityEngine;
using UnityEditor;

namespace ToucanSystems
{
    public class RectHandles
    {
        public static void ResizeRect(RectTransform rt, Handles.CapFunction capFunction, Color capColor, float capSize, float snap)
        {
            rt.localRotation = Quaternion.Euler(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);

            Vector3 halfRectSize = new Vector3(rt.sizeDelta.x * 0.5f, rt.sizeDelta.y * 0.5f, 0);

            Vector3[] rectCorners = new Vector3[4];
            rt.GetWorldCorners(rectCorners);

            Vector3[] handlePoints =
            {
            new Vector3(rectCorners[0].x, rectCorners[0].y + halfRectSize.y, 0),   // Left
            new Vector3(rectCorners[0].x + rt.sizeDelta.x, rectCorners[0].y +  + halfRectSize.y, 0),   // Right
            new Vector3(rectCorners[0].x + halfRectSize.x, rectCorners[0].y + rt.sizeDelta.y, 0),   // Top
            new Vector3(rectCorners[0].x + halfRectSize.x, rectCorners[0].y, 0),   // Bottom 
            new Vector3(rectCorners[0].x + halfRectSize.x, rectCorners[0].y + halfRectSize.y, 0), // Center
            };

            Handles.color = capColor;

            Vector2 newSize = rt.sizeDelta;
            Vector3 newPosition = rt.localPosition;

            float leftHandle = Handles.Slider(handlePoints[0], -Vector3.right, capSize, capFunction, snap).x - handlePoints[0].x;
            float rightHandle = Handles.Slider(handlePoints[1], Vector3.right, capSize, capFunction, snap).x - handlePoints[1].x;
            float topHandle = Handles.Slider(handlePoints[2], Vector3.up, capSize, capFunction, snap).y - handlePoints[2].y;
            float bottomHandle = Handles.Slider(handlePoints[3], -Vector3.up, capSize, capFunction, snap).y - handlePoints[3].y;
            Vector3 positionHandle = Handles.FreeMoveHandle(handlePoints[4], Quaternion.identity, capSize * 1.2f, Vector3.one, Handles.SphereHandleCap);
            Vector3 bottomLeftHandle = Handles.FreeMoveHandle(rectCorners[0], Quaternion.identity, capSize, Vector3.one, capFunction);
            Vector3 bottomRightHandle = Handles.FreeMoveHandle(rectCorners[3], Quaternion.identity, capSize, Vector3.one, capFunction);
            Vector3 topLeftHandle = Handles.FreeMoveHandle(rectCorners[1], Quaternion.identity, capSize, Vector3.one, capFunction);
            Vector3 topRightHandle = Handles.FreeMoveHandle(rectCorners[2], Quaternion.identity, capSize, Vector3.one, capFunction);

            float sizeXFactor = (bottomRightHandle.x - newSize.x - rectCorners[0].x) + (topRightHandle.x - newSize.x - rectCorners[0].x) - (bottomLeftHandle.x - rectCorners[0].x) - (topLeftHandle.x - rectCorners[0].x);
            float sizeYFactor = (topLeftHandle.y - newSize.y - rectCorners[0].y) + (topRightHandle.y - newSize.y - rectCorners[0].y) - (bottomLeftHandle.y - rectCorners[0].y) - (bottomRightHandle.y - rectCorners[3].y);

            newSize = new Vector2(Mathf.Max(2.1f, newSize.x - leftHandle + rightHandle + sizeXFactor), Mathf.Max(2.1f, newSize.y + topHandle - bottomHandle + sizeYFactor));

            Vector3 positionFactor = (positionHandle - handlePoints[4]) + (bottomLeftHandle - rectCorners[0]) + new Vector3(topLeftHandle.x - rectCorners[1].x, 0, 0) + new Vector3(0, bottomRightHandle.y - rectCorners[3].y, 0);
            newPosition = new Vector2(newPosition.x + leftHandle + positionFactor.x, newPosition.y + bottomHandle + positionFactor.y);

            rt.sizeDelta = newSize;
            rt.localPosition = newPosition;
        }
    }
}