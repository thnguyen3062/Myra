using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GIKCore.Utilities;
using TMPro;

namespace GIKCore.UI
{
    public class BoxPage : MonoBehaviour
    {
        // Fields
        [SerializeField] private TextMeshProUGUI m_Label;
        [SerializeField] private GameObject m_GoPrev, m_GoPrevBlur, m_GoNext, m_GoNextBlur;

        // Values
        private int _page = 0;
        private ICallback.CallFunc onPrev, onNext;

        // Methods
        public void DoClickPrev()
        {
            if (onPrev != null) onPrev();
        }
        public void DoClickNext()
        {
            if (onNext != null) onNext();
        }

        public int now { get { return _page; } }
        public BoxPage Set(int page, bool next, int pageFrom = 0, string label = "")
        {
            _page = page;
            m_Label.text = !string.IsNullOrEmpty(label) ? label : (page - pageFrom + 1) + "";

            bool prev = (page > pageFrom);
            m_GoPrev.SetActive(prev);
            m_GoPrevBlur.SetActive(!prev);

            m_GoNext.SetActive(next);
            m_GoNextBlur.SetActive(!next);
            return this;
        }
        public BoxPage SetActive(bool active)
        {
            gameObject.SetActive(active);
            return this;
        }
        public BoxPage SetOnPrev(ICallback.CallFunc func)
        {
            onPrev = func;
            return this;
        }
        public BoxPage SetOnNext(ICallback.CallFunc func)
        {
            onNext = func;
            return this;
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
