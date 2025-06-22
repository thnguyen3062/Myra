using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShootProjectile : MonoBehaviour
{
    public void ShootObject(Vector3 start, Vector3 end, ICallback.CallFunc callback)
    {
        transform.position = start;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(1f, 0.2f))
            .Insert(0.2f, transform.DOMoveY(0.1f, 0.2f))
            .Insert(0, transform.DOMoveX(end.x, 0.4f))
            .Insert(0, transform.DOMoveZ(end.z, 0.4f))
            .OnComplete(() => callback?.Invoke());
    }
    public void ShootObjectFast(Vector3 start, Vector3 end, ICallback.CallFunc callback)
    {
        transform.position = start;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(1f, 0.1f))
            .Insert(0.2f, transform.DOMoveY(0.1f, 0.1f))
            .Insert(0, transform.DOMoveX(end.x, 0.2f))
            .Insert(0, transform.DOMoveZ(end.z, 0.2f))
            .OnComplete(() => callback?.Invoke());
    }
}
