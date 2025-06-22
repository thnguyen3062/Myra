using GIKCore;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventBannerProps
{
    public int index;
    public SystemEvent systemEvent;
    public bool isOpen;
}
public class EventBanner : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_ImageBanner;

    // Values
    private EventBannerProps props;
    private ICallback.CallFunc2<int> onClickCloseCB = null;
    private ICallback.CallFunc onClickEventCB = null;
    IEnumerator coroutine = null;

    // Methods
    public EventBanner SetOnClickCloseCB(ICallback.CallFunc2<int> func) { onClickCloseCB = func; return this; }
    public EventBanner SetOnClickEventCB(ICallback.CallFunc func) { onClickEventCB = func; return this; }
    public void DoClickClose()
    {
        if (onClickCloseCB != null) { onClickCloseCB(props.index); }
    }
    public void DoClickEvent()
    {
        if (onClickEventCB != null) { onClickEventCB(); }
    }
    public EventBanner SetData(EventBannerProps data)
    {
        gameObject.SetActive(data.isOpen);
        props = data;
        if (props.systemEvent.spriteBanner != null) SetAvatarSprite(props.systemEvent.spriteBanner);
        else LoadHttpAvatar(props.systemEvent.banner, (s) => { props.systemEvent.spriteBanner = s; });
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
        m_ImageBanner.sprite = sprite;
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
