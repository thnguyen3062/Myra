using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIKCore.UI
{
    public class RotateInfinity : MonoBehaviour
    {
        // Fields    
        [SerializeField] private Transform m_Target;
        [SerializeField] private float m_RotateSpeed = 200f;
        [SerializeField] private bool m_ClockWise = true;

        // Use this for initialization
        private void Awake()
        {
            if (m_Target == null) m_Target = transform;
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float z = m_RotateSpeed * Time.deltaTime;
            if (m_ClockWise) z *= -1f;
            m_Target.Rotate(0f, 0f, z);
        }
    }
}