using GIKCore.Utilities;
using PathologicalGames;
using UnityEngine;

public class ParticleEffectParentCallback : MonoBehaviour
{
    [SerializeField] private ParticleEffectCallback m_ParticleCallback;
    private ICallback.CallFunc onComplete;

    public ParticleEffectParentCallback SetOnComplete(ICallback.CallFunc func) { onComplete = func; return this; }

    public ParticleEffectParentCallback SetOnPlay()
    {
        m_ParticleCallback.SetPlayEffect()
            .SetOnComplete(() =>
            {
                onComplete?.Invoke();
                PoolManager.Pools["Effect"].Despawn(transform);
            });

        return this;
    }
    public ParticleEffectParentCallback SetCallbackAfterPlay()
    {
        m_ParticleCallback.SetPlayEffect().SetOnComplete(() => { onComplete?.Invoke(); });
        return this;
        
    }    
}
