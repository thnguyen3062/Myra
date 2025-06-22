using UnityEngine;
using UIEngine.Extensions.Attribute;

namespace UIEngine.UIPool
{    
    public class VerticalPoolGroup : HorizontalOrVerticalPoolGroup
    {
        public enum Alignment { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }

        // Fields
        [Help("- MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight just available in case of content height less than or equals viewport height.\n- In other case:\nMiddleLeft -> UpperLeft; MiddleCenter -> UpperCenter; MiddleRight -> UpperRight;\nLowerLeft -> UpperLeft; LowerCenter -> UpperCenter; LowerRight -> UpperRight", Type.Info)]
        [SerializeField] private Alignment m_ChildAlignment = Alignment.UpperLeft;
        [SerializeField] private float m_PaddingTop = 0f;
        [SerializeField] private float m_PaddingBot = 0f;

        // Method
        protected override void CalculateSizeDelta()
        {
            base.CalculateSizeDelta();

            int numElement = adapter.Count;
            float width = viewport.rect.width;
            float height = viewport.rect.height;

            //calculate content size
            float hConstraint = m_PaddingTop + m_PaddingBot;
            for (int i = 0; i < numElement; i++)
            {
                Vector2 elmSize = GetElementSize(i);
                AddToListCellSize(elmSize);
                hConstraint += (elmSize.y + m_Spacing.y);
            }
            hConstraint = Mathf.Max(0, hConstraint - m_Spacing.y);

            float yTo = -m_PaddingTop;
            if (hConstraint <= height)
            {
                if (m_ChildAlignment == Alignment.MiddleLeft || m_ChildAlignment == Alignment.MiddleCenter || m_ChildAlignment == Alignment.MiddleRight)
                {
                    yTo -= (height * 0.5f - hConstraint * 0.5f);
                    hConstraint = height;
                }
                else if (m_ChildAlignment == Alignment.LowerLeft || m_ChildAlignment == Alignment.LowerCenter || m_ChildAlignment == Alignment.LowerRight)
                {
                    yTo -= (height - hConstraint);
                    hConstraint = height;
                }
            }

            //set content size delta
            content.sizeDelta = new Vector2(width, hConstraint);

            //calculate init local position of each cell in group.
            //anchors min, max, pivot is at(0, 1)
            float xTo = 0f;
           
            for (int i = 0; i < numElement; i++)
            {
                Vector2 cellSize = GetCellSize(i);

                if (m_ChildAlignment == Alignment.UpperLeft || m_ChildAlignment == Alignment.MiddleLeft || m_ChildAlignment == Alignment.LowerLeft)
                    xTo = 0f;
                else if (m_ChildAlignment == Alignment.UpperCenter || m_ChildAlignment == Alignment.MiddleCenter || m_ChildAlignment == Alignment.LowerCenter)
                    xTo = width * 0.5f - cellSize.x * 0.5f;
                else if (m_ChildAlignment == Alignment.UpperRight || m_ChildAlignment == Alignment.MiddleRight || m_ChildAlignment == Alignment.LowerRight)
                    xTo = width - cellSize.x;

                AddToListCellPos(new Vector2(xTo + m_Spacing.x, yTo));
                yTo -= (cellSize.y + m_Spacing.y);
            }
        }
        protected override void UpdateData()
        {
            base.UpdateData();

            //Calculate distance between current pivot's position and init pivot's position of layput group
            float offsetY = content.anchoredPosition.y; //init posY is at y = 0 (local)

            //check pool, inactive object if it's out of bound
            foreach (PoolObject po in listPool)
            {
                if (!po.isAvailable)
                {
                    float yTop = GetCellPos(po.index).y + offsetY;
                    float yBot = yTop - GetCellSize(po.index).y;
                    if (yBot > 0 || yTop < -viewport.rect.height)
                        po.RecycleObject();
                }
            }

            //data
            int numElement = adapter.Count;
            for (int i = 0; i < numElement; i++)
            {
                float yTop = GetCellPos(i).y + offsetY;
                float yBot = yTop - GetCellSize(i).y;
                if (yBot > 0 || yTop < -viewport.rect.height || IsCellVisible(i))
                    continue;

                //add cell
                GetPooledObject(i);
            }
        }       
    }
}