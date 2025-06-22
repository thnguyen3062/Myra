using GIKCore.Utilities;
using UnityEngine;

public class AnimationParentCallback : MonoBehaviour
{
    [SerializeField] private AnimationCallback m_AnimationCallback;
    private ICallback.CallFunc onStartAnim;
    private ICallback.CallFunc onEndAnim;
    private ICallback.CallFunc onCustomAnim;

    public AnimationParentCallback SetOnStartAnim(ICallback.CallFunc func) { onStartAnim = func; return this; }
    public AnimationParentCallback SetOnEndAnim(ICallback.CallFunc func) { onEndAnim = func; return this; }
    public AnimationParentCallback SetOnCustomAnim(ICallback.CallFunc func) { onCustomAnim = func; return this; }

    public AnimationParentCallback StartAnim()
    {
        m_AnimationCallback.SetOnStartAnim(onStartAnim);
        return this;
    }

    public AnimationParentCallback EndAnim()
    {
        Debug.Log("here");
        m_AnimationCallback.SetOnEndAnim(onEndAnim);
        return this;
    }

    public AnimationParentCallback CustomAnim()
    {
        m_AnimationCallback.SetOnCustomAnim(onCustomAnim);
        return this;
    }
}
