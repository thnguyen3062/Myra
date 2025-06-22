using GIKCore.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStopCallbackOnly : MonoBehaviour
{
    public static ParticleStopCallbackOnly Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public event ICallback.CallFunc complete;
    public void OnParticleSystemStopped()
    {
        complete?.Invoke();
    }
}
