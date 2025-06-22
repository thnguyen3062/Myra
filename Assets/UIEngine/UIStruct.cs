using UnityEngine;

namespace UIEngine
{
    public struct RectStruct
    {
        public float xRight;
        public float xLeft;
        public float xMiddle;

        public float yTop;
        public float yBottom;
        public float yMiddle;

        public static implicit operator RectStruct(RectTransform value)
        {
            RectStruct tmp = new RectStruct();

            if (value != null)
            {
                tmp.xRight = value.anchoredPosition.x + value.rect.width * (1f - value.pivot.x);
                tmp.xLeft = value.anchoredPosition.x - value.rect.width * value.pivot.x;
                tmp.xMiddle = (tmp.xRight + tmp.xLeft) * 0.5f;

                tmp.yTop = value.anchoredPosition.y + value.rect.height * (1f - value.pivot.y);
                tmp.yBottom = value.anchoredPosition.y - value.rect.height * value.pivot.y;
                tmp.yMiddle = (tmp.yTop + tmp.yBottom) * 0.5f;
            }

            return tmp;
        }
    }
    public struct BoundStruct
    {
        public float xLeft, xRight, yTop, yBot;
    }
}
