using GIKCore;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSlider : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_ImageSlider;

    // Values
    private ICallback.CallFunc onClickCB;
    private SystemEvent systemEventData;
    IEnumerator coroutine = null;

    // Methods
    public EventSlider SetOnClckCB(ICallback.CallFunc func) { onClickCB = func; return this; }
    public void DoClick()
    {
        if (onClickCB != null) { onClickCB(); }
    }
    public EventSlider Setdata(SystemEvent data)
    {
        systemEventData = data;
        if (systemEventData.spriteSlider != null) SetAvatarSprite(systemEventData.spriteSlider);
        else LoadHttpAvatar(systemEventData.slide, (s) => { systemEventData.spriteSlider = s; });
        return this;
    }

    public void LoadHttpAvatar(string url, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;

        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            SetAvatarSprite(sprite);

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    private void SetAvatarSprite(Sprite sprite)
    {
        m_ImageSlider.sprite = sprite;
    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
