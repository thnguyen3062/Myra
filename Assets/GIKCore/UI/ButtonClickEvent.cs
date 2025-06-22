using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GIKCore.Utilities;
using GIKCore.Sound;

namespace GIKCore.UI
{
    [DisallowMultipleComponent]
    public class ButtonClickEvent : MonoBehaviour
    {
        // Fields
        [SerializeField] private UnityEvent m_ClickEvents;

        // Values
        private ICallback.CallFunc onClickCB = null;

        // Methods
        public void DoClick()
        {
            SoundHandler.main.PlaySFX("900_click_4", "sounds");
            if (onClickCB != null) onClickCB();
            IUtil.InvokeEvent(m_ClickEvents);
        }

        public ButtonClickEvent SetOnClickCallback(ICallback.CallFunc func) { onClickCB = func; return this; }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
