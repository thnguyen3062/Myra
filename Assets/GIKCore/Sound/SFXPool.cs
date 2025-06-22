using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIKCore.Sound
{
    public class SFXPool : MonoBehaviour
    {
        // Fields
        [SerializeField]
        private AudioSource m_AudioSource;

        // Methods
        public AudioSource audioSource { get { return m_AudioSource; } }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (m_AudioSource.clip != null && !m_AudioSource.isPlaying)
                gameObject.SetActive(false);
        }
    }
}
