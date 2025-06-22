using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : MonoBehaviour
{
    private ICallback.CallFunc onStartAnim;
    private ICallback.CallFunc onEndAnim;
    private ICallback.CallFunc onCustomAnim;

    public AnimationCallback SetOnStartAnim(ICallback.CallFunc func) { onStartAnim = func; return this; }
    public AnimationCallback SetOnEndAnim(ICallback.CallFunc func) { onEndAnim = func; return this; }
    public AnimationCallback SetOnCustomAnim(ICallback.CallFunc func) { onCustomAnim = func;return this; }

    public AnimationCallback StartAnim()
    {
        onStartAnim?.Invoke();
        return this;
    }

    public AnimationCallback EndAnim()
    {
        Debug.Log("here");
        onEndAnim?.Invoke();
        return this;
    }

    public AnimationCallback CustomAnim()
    {
        onCustomAnim?.Invoke();
        return this;
    }
}
