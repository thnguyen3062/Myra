using GIKCore.Utilities;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPointEffectHandle : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    public void SetupEffect(Vector3 start, Vector3 end, ICallback.CallFunc callback)
    {
        startPoint.position = start;
        endPoint.position = end;
        callback?.Invoke();
    }
}
