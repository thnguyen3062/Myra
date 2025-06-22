using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Attribute;

namespace GIKCore.UI
{
    public class ButtonTransitionScale : ButtonTransition
    {
        // Fields
        [SerializeField]
        [Help("When user press the target, the target will zoom to a scale. The final scale of the target = (target's original scale * zoomScale)")]
        private float m_ZoomScale = 1.2f;

        [SerializeField]
        [Min(0)]
        private float m_Duration = 0.1f;

        [SerializeField] private bool m_AllowHoverEffect = false;

        // Values
        private Vector3 originalScale = new Vector3();
        private Vector3 fromScale = new Vector3();
        private Vector3 toScale = new Vector3();
        private float timeRun = 0f;
        private bool transitionFinished = true;

        // Methods
        public override void TriggerDisable() { ZoomReset(); }
        public override void TriggerHighlighted()
        {
            if (m_AllowHoverEffect)
                ZoomUp();
        }
        public override void TriggerNormal() { ZoomReset(); }
        public override void TriggerPressed() { ZoomUp(); }
        public override void TriggerSelected() { ZoomBack(); }

        private void ZoomReset()
        {
            m_Target.localScale = originalScale;

            timeRun = 0;
            transitionFinished = true;
        }
        private void ZoomUp()
        {
            fromScale = m_Target.localScale;// originalScale
            toScale = originalScale * m_ZoomScale;

            timeRun = 0;
            transitionFinished = false;
        }
        private void ZoomBack()
        {
            fromScale = m_Target.localScale;
            toScale = originalScale;

            timeRun = 0;
            transitionFinished = false;
        }

        protected override void Awake()
        {
            base.Awake();
            originalScale = fromScale = toScale = m_Target.localScale;
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (!transitionFinished)
            {
                timeRun += Time.deltaTime;
                float ratio = 1f;
                if (m_Duration > 0) ratio = timeRun / m_Duration;

                if (ratio >= 1) ratio = 1;

                m_Target.localScale = Vector3.Lerp(fromScale, toScale, ratio);

                if (ratio == 1) transitionFinished = true;
            }
        }
    }
}
