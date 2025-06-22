using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIKCore.UI
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public abstract class ButtonTransition : MonoBehaviour
    {
        // Fields
        [SerializeField] protected Transform m_Target;

        // Methods
        public virtual void TriggerDisable() { }
        public virtual void TriggerHighlighted() { }
        public virtual void TriggerNormal() { }
        public virtual void TriggerPressed() { }
        public virtual void TriggerSelected() { }

        protected virtual void Awake()
        {
            if (m_Target == null) m_Target = transform;
            Animator animator = GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController == null)
            {
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("GIKCore/UI/Animation/ButtonTransition");
            }
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
