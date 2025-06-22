using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectCallback : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;
    private ICallback.CallFunc onComplete;

    public ParticleEffectCallback SetOnComplete(ICallback.CallFunc func) { onComplete = func; return this; }

    public ParticleEffectCallback SetPlayEffect()
    {
        if (m_ParticleSystem == null)
            m_ParticleSystem = GetComponent<ParticleSystem>();
        m_ParticleSystem.Play();
        return this;
    }

    public void OnParticleSystemStopped()
    {
        onComplete?.Invoke();
    }
}
