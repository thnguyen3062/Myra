using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent]    
    public class Gradient : BaseMeshEffect
    {
        // Field
        [SerializeField]
        private Color32 m_StartColor = Color.white, m_EndColor = Color.white;

        [SerializeField]
        private GradientType m_GradientType = GradientType.Vertical;

        [SerializeField]
        [Range(-1.5f, 1.5f)]
        private float m_Offset = 0f;

        // Enum        
        public enum GradientType
        {
            Horizontal,
            Vertical
        }

        // Methods
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;
            List<UIVertex> lstUIVertex = new List<UIVertex>();
            vh.GetUIVertexStream(lstUIVertex);
            ModifyVertices(lstUIVertex);
            vh.Clear();
            vh.AddUIVertexTriangleStream(lstUIVertex);
        }

        private void ModifyVertices(List<UIVertex> lstUIVertex)
        {
            if (!IsActive() || lstUIVertex == null)
                return;

            int num = lstUIVertex.Count;
            if (num <= 0)
                return;
            switch (m_GradientType)
            {
                case GradientType.Vertical:
                    {
                        float yBot = lstUIVertex[0].position.y;
                        float yTop = lstUIVertex[0].position.y;
                        float yPos = 0f;

                        for (int i = 1; i < num; i++)
                        {
                            yPos = lstUIVertex[i].position.y;
                            if (yPos > yTop)
                                yTop = yPos;
                            else if (yPos < yBot)
                                yBot = yPos;
                        }

                        float uiElmHeight = (yTop != yBot) ? 1f / (yTop - yBot) : 1f;
                        for (int i = 0; i < num; i++)
                        {
                            UIVertex uiVertex = lstUIVertex[i];
                            uiVertex.color = Color32.Lerp(m_EndColor, m_StartColor, (uiVertex.position.y - yBot) * uiElmHeight - m_Offset);
                            lstUIVertex[i] = uiVertex;
                        }

                        break;
                    }
                case GradientType.Horizontal:
                    {
                        float xLeft = lstUIVertex[0].position.x;
                        float xRight = lstUIVertex[0].position.x;
                        float xPos = 0f;

                        for (int i = 1; i < num; i++)
                        {
                            xPos = lstUIVertex[i].position.x;
                            if (xPos > xRight)
                                xRight = xPos;
                            else if (xPos < xLeft)
                                xLeft = xPos;
                        }

                        float uiElmWidth = (xRight != xLeft) ? 1f / (xRight - xLeft) : 1f;
                        for (int i = 0; i < num; i++)
                        {
                            UIVertex uiVertex = lstUIVertex[i];
                            uiVertex.color = Color32.Lerp(m_StartColor, m_EndColor, (uiVertex.position.x - xLeft) * uiElmWidth - m_Offset);
                            lstUIVertex[i] = uiVertex;
                        }

                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
