using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GIKCore.UI
{
    public class TextOverflowEffect : MonoBehaviour
    {
        public enum Direction
        {
            None,
            Horizontal,
            Vertical
        }

        // Fields
        [SerializeField] private RectTransform m_RectMask;
        [SerializeField] private Text m_TextPreSign, m_TextOriginal, m_TextHorizontal, m_TextVertical;
        [SerializeField] private Direction m_Direction = Direction.None;
        [SerializeField] private float m_TimeRest = 3f;
        [SerializeField] [Min(1)] private float m_Speed = 30f;

        // Methods
        public string text
        {
            get { return m_TextPreSign.text; }
            set
            {
                m_TextPreSign.text = value;
                float wText = m_TextPreSign.preferredWidth;
                float hText = m_TextPreSign.preferredHeight;

                float wMask = m_RectMask.rect.width;
                float hMask = m_RectMask.rect.height;

                DoReset();
                if (m_Direction == Direction.None)
                {
                    NoEffect(value);
                }
                else if (m_Direction == Direction.Horizontal)
                {
                    if (wText > wMask) HorizontalEffect(value, wText, wMask);
                    else NoEffect(value);
                }
                else if (m_Direction == Direction.Vertical)
                {
                    if (hText > hMask) VerticalEffect(value, hText, hMask);
                    else NoEffect(value);
                }
            }
        }
        public Color color
        {
            set
            {
                m_TextPreSign.color = m_TextOriginal.color = m_TextHorizontal.color = m_TextVertical.color = value;
            }
        }
        private void NoEffect(string s)
        {
            m_TextOriginal.text = s;
            m_TextOriginal.gameObject.SetActive(true);
        }
        private void HorizontalEffect(string s, float wText, float wMask)
        {
            m_TextHorizontal.text = s;
            m_TextHorizontal.gameObject.SetActive(true);

            float offset = wText - wMask;
            float duration = offset / m_Speed;

            float xTarget = -offset;
            Sequence seq = DOTween.Sequence();
            seq.Append(m_TextHorizontal.rectTransform.DOAnchorPosX(xTarget, duration).SetDelay(m_TimeRest).SetEase(Ease.Linear))
               .Append(m_TextHorizontal.rectTransform.DOAnchorPosX(0, duration).SetDelay(m_TimeRest).SetEase(Ease.Linear))
               .SetLoops(-1);
        }
        private void VerticalEffect(string s, float hText, float hMask)
        {
            m_TextVertical.text = s;
            m_TextVertical.gameObject.SetActive(true);

            float offset = hText - hMask;
            float duration = offset / m_Speed;

            float yTarget = offset;
            Sequence seq = DOTween.Sequence();
            seq.Append(m_TextHorizontal.rectTransform.DOAnchorPosY(yTarget, duration).SetDelay(m_TimeRest).SetEase(Ease.Linear))
               .Append(m_TextHorizontal.rectTransform.DOAnchorPosY(0, duration).SetDelay(m_TimeRest).SetEase(Ease.Linear))
               .SetLoops(-1);
        }
        private void DoReset()
        {
            m_TextOriginal.gameObject.SetActive(false);

            m_TextHorizontal.DOKill();
            m_TextHorizontal.rectTransform.anchoredPosition = Vector2.zero;
            m_TextHorizontal.gameObject.SetActive(false);

            m_TextVertical.DOKill();
            m_TextVertical.rectTransform.anchoredPosition = Vector2.zero;
            m_TextVertical.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}