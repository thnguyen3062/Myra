using UnityEngine;

namespace GIKCore.Utilities
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
    public struct PairStruct<T1, T2>
    {
        public T1 value1;
        public T2 value2;
    }

    [System.Serializable]
    public struct Size
    {
        public float width;
        public float height;

        /// <summary>
        /// Constructor a size with specified values.
        /// </summary>
        /// <param name="width">width of the Size, default value is 0.</param>
        /// <param name="height">height of the Size, default value is 0.</param>
        public Size(float width = 0f, float height = 0f)
        {
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// Constructor a size from another one.
        /// </summary>
        /// <param name="other">Specified Size.</param>
        public Size(Size other)
        {
            width = other.width;
            height = other.height;
        }

        /// <summary>
        /// clone the current Size.
        /// </summary>
        /// <returns></returns>
        public Size Clone()
        {
            return new Size(width, height);
        }
        /// <summary>
        /// Set the value of each component of the current Size.
        /// </summary>
        /// <param name="width">Specified width</param>
        /// <param name="height">Specified height</param>
        /// <returns>this</returns>
        public Size Set(float width, float height)
        {
            this.width = width;
            this.height = height;
            return this;
        }
        /// <summary>
        /// Set values with another Size.
        /// </summary>
        /// <param name="other">Specified Size</param>
        /// <returns>this</returns>
        public Size Set(Size other)
        {
            Set(other.width, other.height);
            return this;
        }
        /// <summary>
        /// Add the value of each component of the current Size.
        /// </summary>
        /// <param name="width">Specified width</param>
        /// <param name="height">Specified height</param>
        /// <returns>this</returns>
        public Size Add(float width, float height)
        {
            this.width += width;
            this.height += height;
            return this;
        }
        /// <summary>
        /// Add values with another Size.
        /// </summary>
        /// <param name="other">Specified Size</param>
        /// <returns>this</returns>
        public Size Add(Size other)
        {
            Add(other.width, other.height);
            return this;
        }
        /// <summary>
        /// Sub the value of each component of the current Size.
        /// </summary>
        /// <param name="width">Specified width</param>
        /// <param name="height">Specified width</param>
        /// <returns>this</returns>
        public Size Sub(float width, float height)
        {
            Add(-width, -height);
            return this;
        }
        /// <summary>
        /// Add values with another Size.
        /// </summary>
        /// <param name="other">Specified Size</param>
        /// <returns>this</returns>
        public Size Sub(Size other)
        {
            Sub(other.width, other.height);
            return this;
        }
        /// <summary>
        /// Check whether the current Size equals another one.
        /// </summary>
        /// <param name="other">Specified Size</param>
        /// <returns>Returns true when both dimensions are equal in width and height; otherwise returns false.</returns>
        public bool Equals(Size other)
        {
            return width == other.width && height == other.height;
        }
        /// <summary>
        ///  Return the information of the current size in string json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IUtil.StringFormat("{\"width\":{0},\"height\":{1}}", width, height);
        }

        public static Size One
        {
            get { return new Size(1f, 1f); }
        }
        public static Size Zero
        {
            get { return new Size(0f, 0f); }
        }
    }
}
