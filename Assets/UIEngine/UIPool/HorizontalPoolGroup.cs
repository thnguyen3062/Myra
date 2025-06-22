using UnityEngine;
using UIEngine.Extensions.Attribute;

namespace UIEngine.UIPool
{
    public class HorizontalPoolGroup : HorizontalOrVerticalPoolGroup
    {
        public enum Alignment { UpperLeft, MiddleLeft, LowerLeft, UpperCenter, MiddleCenter, LowerCenter, UpperRight, MiddleRight, LowerRight }

        // Fields
        [Help("- UpperCenter, MiddleCenter, LowerCenter, UpperRight, MiddleRight, LowerRight just available in case of content width less than or equals viewport width.\n-In other case:\nUpperCenter -> UpperLeft; MiddleCenter -> MiddleLeft; LowerCenter -> LowerLeft\nUpperRight -> UpperLeft; MiddleRight -> MiddleLeft; LowerRight -> LowerLeft", Type.Info)]
        [SerializeField] private Alignment m_ChildAlignment = Alignment.UpperLeft;
        [SerializeField] private float m_PaddingLeft = 0f;
        [SerializeField] private float m_PaddingRight = 0f;

        // Method
        protected override void CalculateSizeDelta()
        {
            base.CalculateSizeDelta();

            int numElement = adapter.Count;
            float width = viewport.rect.width;
            float height = viewport.rect.height;

            //calculate content size
            float wConstraint = m_PaddingLeft + m_PaddingRight;
            for (int i = 0; i < numElement; i++)
            {
                Vector2 elmSize = GetElementSize(i);
                AddToListCellSize(elmSize);
                wConstraint += (elmSize.x + m_Spacing.x);
            }

            float xTo = m_PaddingLeft;
            if (wConstraint <= width)
            {
                if (m_ChildAlignment == Alignment.UpperCenter || m_ChildAlignment == Alignment.MiddleCenter || m_ChildAlignment == Alignment.LowerCenter)
                {
                    xTo += (width * 0.5f - wConstraint * 0.5f);
                    wConstraint = width;
                }
                else if (m_ChildAlignment == Alignment.UpperRight || m_ChildAlignment == Alignment.MiddleRight || m_ChildAlignment == Alignment.LowerRight)
                {
                    xTo += (width - wConstraint);
                }
            }

            //set content size delta
            content.sizeDelta = new Vector2(wConstraint, height);

            //calculate init local position of each cell in group.
            //anchors min, max, pivot is at(0, 1)
            float yTo = 0f;

            for (int i = 0; i < numElement; i++)
            {
                Vector2 cellSize = GetCellSize(i);

                if (m_ChildAlignment == Alignment.UpperLeft || m_ChildAlignment == Alignment.UpperCenter || m_ChildAlignment == Alignment.UpperRight)
                    yTo = 0f;
                else if (m_ChildAlignment == Alignment.MiddleLeft || m_ChildAlignment == Alignment.MiddleCenter || m_ChildAlignment == Alignment.MiddleRight)
                    yTo = cellSize.y * 0.5f - height * 0.5f;
                else if (m_ChildAlignment == Alignment.LowerLeft || m_ChildAlignment == Alignment.LowerCenter || m_ChildAlignment == Alignment.LowerRight)
                    yTo = cellSize.y - height;

                AddToListCellPos(new Vector2(xTo, yTo + m_Spacing.y));
                xTo += (cellSize.x + m_Spacing.x);
            }
        }
        protected override void UpdateData()
        {
            base.UpdateData();

            //Calculate distance between current pivot's position and init pivot's position of layput group
            float offsetX = content.anchoredPosition.x;//init posX is at x = 0 (local)

            //check pool, inactive object if it's out of bound
            foreach (PoolObject po in listPool)
            {
                if (!po.isAvailable)
                {
                    float xLeft = GetCellPos(po.index).x + offsetX;
                    float xRight = xLeft + GetCellSize(po.index).x;
                    if (xRight < 0 || xLeft > viewport.rect.width)
                        po.RecycleObject();
                }
            }

            //data
            int numElement = adapter.Count;
            for (int i = 0; i < numElement; i++)
            {
                float xLeft = GetCellPos(i).x + offsetX;
                float xRight = xLeft + GetCellSize(i).x;
                if (xRight < 0 || xLeft > viewport.rect.width || IsCellVisible(i))
                {
                    continue;
                }

                //add cell
                GetPooledObject(i);
            }
        }
    }
}