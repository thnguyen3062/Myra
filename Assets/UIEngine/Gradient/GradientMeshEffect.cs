using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Note that class work perfectly with UI Text.
/// Remember that the colors are applied per-vertex so if you have multiple points on your gradient where the color changes and there aren't enough vertices, you won't see all of the colors.
/// </summary>
namespace UIEngine.CustomGradient
{
    public class GradientMeshEffect : BaseMeshEffect
    {
        public enum Type { Horizontal, Vertical }
        public enum Blend { Override, Add, Multiply }

        // Fields
        [SerializeField] private Type m_GradientType;
        [SerializeField] private Blend m_BlendMode = Blend.Multiply;
        [SerializeField] [Range(-1, 1)] private float m_Offset = 0f;
        [SerializeField] private UnityEngine.Gradient m_GradientEffect = new UnityEngine.Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };

        // Methods
        public Type GradientType
        {
            get { return m_GradientType; }
            set { m_GradientType = value; }
        }
        public Blend BlendMode
        {
            get { return m_BlendMode; }
            set { m_BlendMode = value; }
        }
        public float Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }
        public UnityEngine.Gradient GradientEffect
        {
            get { return m_GradientEffect; }
            set { m_GradientEffect = value; }
        }

        private Color BlendColor(Color colorA, Color colorB)
        {
            switch (BlendMode)
            {
                case Blend.Add: return colorA + colorB;
                case Blend.Multiply: return colorA * colorB;
                default: return colorB;
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0)
                return;
            List<UIVertex> _vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(_vertexList);
            int nCount = _vertexList.Count;
            switch (GradientType)
            {
                case Type.Horizontal:
                    {
                        float left = _vertexList[0].position.x;
                        float right = _vertexList[0].position.x;
                        float x = 0f;

                        for (int i = nCount - 1; i >= 1; --i)
                        {
                            x = _vertexList[i].position.x;

                            if (x > right) right = x;
                            else if (x < left) left = x;
                        }

                        float width = (right != left) ? (right - left) : 1f;
                        UIVertex vertex = new UIVertex();

                        for (int i = 0; i < vh.currentVertCount; i++)
                        {
                            vh.PopulateUIVertex(ref vertex, i);
                            vertex.color = BlendColor(vertex.color, GradientEffect.Evaluate((vertex.position.x - left) / width - Offset));
                            vh.SetUIVertex(vertex, i);
                        }
                        break;
                    }
                case Type.Vertical:
                    {
                        float bottom = _vertexList[0].position.y;
                        float top = _vertexList[0].position.y;
                        float y = 0f;

                        for (int i = nCount - 1; i >= 1; --i)
                        {
                            y = _vertexList[i].position.y;

                            if (y > top) top = y;
                            else if (y < bottom) bottom = y;
                        }

                        float height = (top != bottom) ? (top - bottom) : 1f;
                        UIVertex vertex = new UIVertex();

                        for (int i = 0; i < vh.currentVertCount; i++)
                        {
                            vh.PopulateUIVertex(ref vertex, i);
                            vertex.color = BlendColor(vertex.color, GradientEffect.Evaluate((vertex.position.y - bottom) / height - Offset));
                            vh.SetUIVertex(vertex, i);
                        }
                        break;
                    }
            }
        }
    }
}
