using UnityEngine;

namespace GIKCore.UI
{
    /// <summary>
    ///  Attach to Template GameObject of Dropdown
    /// </summary>
    public class DropdownSolution : MonoBehaviour
    {
        //Fields
        [SerializeField] private Vector2 m_SizeOffset = Vector2.zero;
        [SerializeField] private Vector2 m_SizeMin = Vector2.zero;

        void Start()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + m_SizeOffset.x, rectTransform.sizeDelta.y + m_SizeOffset.y);
            if (m_SizeMin.x > 0 || m_SizeMin.y > 0)
            {
                Vector2 v = rectTransform.sizeDelta;
                if (v.x < m_SizeMin.x) v.x = m_SizeMin.x;
                if (v.y < m_SizeMin.y) v.y = m_SizeMin.y;

                rectTransform.sizeDelta = v;
            }
        }
    }
}