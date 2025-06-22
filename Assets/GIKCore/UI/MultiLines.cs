using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Utilities;

namespace GIKCore.UI
{
    public class MultiLines : MonoBehaviour
    {
        // Fields
        [SerializeField] private Transform m_Panel;
        [SerializeField] private GameObject m_GoLine;
        [SerializeField] private Color m_Color = Color.white;
        [SerializeField] [Min(0.1f)] private float m_Width = 5f;
        [SerializeField] private List<Vector2> m_Points = new List<Vector2>();
        [SerializeField] private bool m_DrawOnAwake = true;

        // Methods
        private Vector2 lastPoint;
        private bool init = false;

        public void SetWidth(float w)
        {
            if (w <= 0) w = 1f;
            m_Width = w;
        }
        public void SetColor(Color c) { m_Color = c; }
        public void SetPoints(List<Vector2> points)
        {
            foreach (Transform tr in m_Panel)
                tr.gameObject.SetActive(false);
            DrawLine(points);
        }
        public void AddPoints(List<Vector2> points)
        {
            List<Vector2> clone = new List<Vector2>();
            if (init) clone.Add(lastPoint);
            clone.AddRange(points);

            DrawLine(clone);
        }

        private void DrawLine(List<Vector2> points)
        {
            int count = points.Count;
            if (count > 1)
            {
                init = true;
                lastPoint = points[count - 1];

                for (int i = 1; i < points.Count; i++)
                {
                    Vector2 from = points[i - 1];
                    Vector2 to = points[i];

                    AddLine(from, to);
                }
            }
        }
        private void AddLine(Vector2 from, Vector2 to)
        {
            Line go = GetLineFree();
            if (go == null)
            {
                GameObject tmp = IUtil.PrefabClone(m_GoLine, m_Panel);
                go = tmp.GetComponent<Line>();
            }

            if (go != null)
            {
                go.SetActive(true);
                go.SetWidth(m_Width);
                go.SetColor(m_Color);
                go.SetPoint(from, to);
            }
        }
        private Line GetLineFree()
        {
            foreach (Transform tr in m_Panel)
            {
                if (!tr.gameObject.activeSelf)
                    return tr.GetComponent<Line>();
            }
            return null;
        }

        void Awake()
        {
            if (m_DrawOnAwake && m_Points.Count > 1)
                SetPoints(m_Points);
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}