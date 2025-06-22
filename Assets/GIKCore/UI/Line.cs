using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.UI
{
    public class Line : MonoBehaviour
    {
        // Fields    
        [SerializeField] private Image m_Image;

        // Values
        private float width = 1f;

        // Methods
        public bool activeSelf { get { return gameObject.activeSelf; } }
        public void SetActive(bool active) { gameObject.SetActive(active); }
        public void Clear(bool isRemove = false)
        {
            SetActive(false);
            if (isRemove) Destroy(gameObject);
        }
        public void SetColor(Color c)
        {
            m_Image.color = c;
        }
        public void SetWidth(float w)
        {
            if (w <= 0f) w = 1f;
            width = w;
        }
        public void SetPoint(Vector2 from, Vector2 to)
        {
            GetComponent<RectTransform>().anchoredPosition = from;

            Vector2 sizeDelta = m_Image.rectTransform.sizeDelta;
            sizeDelta.x = Vector2.Distance(from, to);
            sizeDelta.y = width;
            m_Image.rectTransform.sizeDelta = sizeDelta;
            m_Image.rectTransform.rotation = Quaternion.Euler(0f, 0f, AngleBetweenVector2(from, to));
        }

        private float AngleBetweenVector2(Vector2 v1, Vector2 v2)
        {
            Vector2 diference = v2 - v1;
            float sign = (v2.y < v1.y) ? -1.0f : 1.0f;
            return Vector2.Angle(Vector2.right, diference) * sign;
        }

        void Awake()
        {
            width = m_Image.rectTransform.sizeDelta.y;//init
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}