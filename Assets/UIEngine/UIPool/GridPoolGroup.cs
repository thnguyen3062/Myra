using UnityEngine;
using System.Collections.Generic;

namespace UIEngine.UIPool
{    
    public class GridPoolGroup : BasePoolGroup
    {
        public enum StartAxis
        {
            //fill all column(x) before fill new row(y)
            Horizontal,
            //fill all row(y) before fill new column(x)
            Vertical
        }
        public enum Alignment { UpperLeft, UpperCenter, UpperRight, MiddleLeft, LowerLeft }

        // Fields
        [SerializeField] private Alignment m_ChildAlignment = Alignment.UpperLeft;
        [SerializeField] private StartAxis m_StartAxis = StartAxis.Horizontal;
        [SerializeField] [Min(1)] private int m_ConstraintCount = 1; //fixed column or row count                

        // Method
        public override BasePoolGroup SetAdapter<T>(List<T> adapter, bool resetPosition = true)
        {
            base.SetAdapter(adapter, resetPosition);
            CalculateSizeDelta();
            ResetPool();
            UpdateData();
            if (resetPosition) ScrollToFirst(0);
            return this;
        }
        protected override void CalculateSizeDelta()
        {
            base.CalculateSizeDelta();

            int numOfGroup = 0;
            int numElement = adapter.Count;
            Vector2 _cellSize = GetCellSize();

            //calculate number of group cell
            for (int i = 0; i < numElement; i += m_ConstraintCount)
            {
                numOfGroup++;
            }

            //add cell size
            for (int i = 0; i < numElement; i++)
            {
                AddToListCellSize(GetCellSize());
            }

            float sizeX = viewport.rect.width;
            float sizeY = viewport.rect.height;
            float sizeConstraintX = 0f;
            float sizeConstaintY = 0f;

            //calculate content size
            if (m_StartAxis == StartAxis.Horizontal)
            {
                sizeConstraintX = (_cellSize.x + m_Spacing.x) * m_ConstraintCount - m_Spacing.x;
                sizeY = (_cellSize.y + m_Spacing.y) * numOfGroup - m_Spacing.y;
            }
            else if (m_StartAxis == StartAxis.Vertical)
            {
                sizeX = (_cellSize.x + m_Spacing.x) * numOfGroup - m_Spacing.x;
                sizeConstaintY = (_cellSize.y + m_Spacing.y) * m_ConstraintCount - m_Spacing.y;
            }

            //set size delta
            content.sizeDelta = new Vector2(Mathf.Max(0f, sizeX), Mathf.Max(0f, sizeY));            

            /*
             * calculate init local position of each cell in group.		 
             * anchors min, max, pivot is at (0, 1).
             * start corner is always upper left.
             */
            int index = -1;

            // fill all column with constraint count before break to new row 
            if (m_StartAxis == StartAxis.Horizontal)
            {
                for (int i = 0; i < numOfGroup; i++)
                {
                    float xTo = 0f;
                    float yTo = -i * (_cellSize.y + m_Spacing.y);

                    if (m_ChildAlignment == Alignment.UpperLeft)
                        xTo = 0f;
                    else if (m_ChildAlignment == Alignment.UpperCenter)
                        xTo = sizeX * 0.5f - sizeConstraintX * 0.5f;
                    else if (m_ChildAlignment == Alignment.UpperRight)
                        xTo = sizeX - sizeConstraintX;

                    for (int j = 0; j < m_ConstraintCount; j++)
                    {
                        index++;
                        if (index > (numElement - 1))
                            break;
                        AddToListCellPos(new Vector2(xTo, yTo));
                        xTo = xTo + (_cellSize.x + m_Spacing.x);
                    }
                }
            }
            // fill all row with constraint count before break to new column
            else if (m_StartAxis == StartAxis.Vertical)
            {
                for (int i = 0; i < numOfGroup; i++)
                {
                    float xTo = i * (_cellSize.x + m_Spacing.x);
                    float yTo = 0f;

                    if (m_ChildAlignment == Alignment.UpperLeft)
                        yTo = 0f;
                    else if (m_ChildAlignment == Alignment.MiddleLeft)
                        yTo = sizeConstaintY * 0.5f - sizeY * 0.5f;
                    else if (m_ChildAlignment == Alignment.LowerLeft)
                        yTo = sizeConstaintY - sizeY;

                    for (int j = 0; j < m_ConstraintCount; j++)
                    {
                        index++;
                        if (index > (numElement - 1))
                            break;
                        AddToListCellPos(new Vector2(xTo, yTo));
                        yTo = yTo - (_cellSize.y + m_Spacing.y);
                    }
                }
            }
        }
        protected override void UpdateData()
        {
            base.UpdateData();

            //Calculate distance between current pivot's position and init pivot's position of layput group
            //init pos is at (0, 0) (local)
            float offsetX = content.anchoredPosition.x;
            float offsetY = content.anchoredPosition.y;

            //check pool, inactive object if it's out of bound
            foreach (PoolObject po in listPool)
            {
                if (!po.isAvailable)
                {
                    Vector2 _cellSize = GetCellSize(po.index);
                    Vector2 _cellPos = GetCellPos(po.index);

                    float xLeft = _cellPos.x + offsetX;
                    float xRight = xLeft + _cellSize.x;

                    float yTop = _cellPos.y + offsetY;
                    float yBot = yTop - _cellSize.y;

                    if (xRight < 0 || xLeft > viewport.rect.width
                        || yBot > 0 || yTop < -viewport.rect.height)
                        po.RecycleObject();
                }
            }

            //data
            int numElement = adapter.Count;
            for (int i = 0; i < numElement; i++)
            {
                Vector2 _cellSize = GetCellSize(i);
                Vector2 _cellPos = GetCellPos(i);

                float xLeft = _cellPos.x + offsetX;
                float xRight = xLeft + _cellSize.x;

                float yTop = _cellPos.y + offsetY;
                float yBot = yTop - _cellSize.y;

                if (xRight < 0 || xLeft > viewport.rect.width
                    || yBot > 0 || yTop < -viewport.rect.height
                    || IsCellVisible(i))
                    continue;

                //add cell
                GetPooledObject(i);
            }
        }       
    }
}