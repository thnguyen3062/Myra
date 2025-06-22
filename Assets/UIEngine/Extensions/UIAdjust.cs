using UnityEngine;

namespace UIEngine.Extensions
{
    public class UIAdjust
    {
        /// <summary>
        /// convert a given point in world space to two points in canvas space with [0] = anchor min and [1] = anchor max corresponding.
        /// </summary>
        /// <returns>The adjust point.</returns>
        public static Vector2[] PixelAdjustPoint(Vector3 worldPoint, Vector2 anchorMin, Vector2 anchorMax, Canvas canvas)
        {
            //first you need the RectTransform of your canvas
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            //then you calculate the position of the UI element
            //(0, 0) for the canvas is at the anchor, whereas 
            //WorldToViewPortPoint treats the lower left corner as (0, 0). 
            //Because of this, you need to subtract the height / width of the canvas * anchor to get the correct position.

            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPoint);

            Vector2 canvasPointMin = new Vector2(
                                         (viewportPoint.x * canvasRect.rect.width) - canvasRect.rect.width * anchorMin.x,
                                         (viewportPoint.y * canvasRect.rect.height) - canvasRect.rect.height * anchorMin.y);
            Vector2 canvasPointMax = new Vector2(
                                         (viewportPoint.x * canvasRect.rect.width) - canvasRect.rect.width * anchorMax.x,
                                         (viewportPoint.y * canvasRect.rect.height) - canvasRect.rect.height * anchorMax.y);

            //so if anchorMin # anchorMax => you can not set SizeDelta, you set offsetMin, offsetMax instead, 
            //in example, you will keep clone UI element is same position and size with source UI element: 
            //rectTrans.offsetMin = new Vector2 (canvasPoints [0].x - width * 0.5f, canvasPoints [0].y - height * 0.5f);
            //rectTrans.offsetMax = new Vector2 (canvasPoints [1].x + width * 0.5f, canvasPoints [1].y + height * 0.5f);

            //but if anchorMin = anchorMax => canvasPoint is distance from pivot of UI element to anchor,
            //in example, you will get xRight, xLeft, yTop, yBotom of UI element base on canvasPoint:
            //xRight = canvasPoint.x + width * (1 - pivot.x);
            //xLeft = canvasPoint.x - width * pivot.x;
            //yTop = canvasPoint.y + height * (1 - pivot.y);
            //yBottom = canvaspoint.y - height * pivot.y;

            return new Vector2[] { canvasPointMin, canvasPointMax };
        }
    }
}