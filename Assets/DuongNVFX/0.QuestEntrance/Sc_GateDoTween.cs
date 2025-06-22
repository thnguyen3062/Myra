using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GIKCore.Utilities;
using GIKCore;
using GIKCore.Net;
using pbdson;

public class Sc_GateDoTween : GameListener
{
    [SerializeField] private GameObject effect;
    [SerializeField] private Image rewardImage;
    private string m_URL = "https://newbie.mytheria.io/";
    private Vector3 startPoint;
    private Vector3 endPoint;
    public float flyDuration = 0.5f;
    private string defaultImg = "https://files.mytheria.io/file/WebEvent/Newbie-DefaultReward.png";
    
    private void  Start()
    {
        Init();
        startPoint = this.transform.position + new Vector3(10, 0, 0);
        endPoint = this.transform.position;
        GateFlyIn();
    }
    private void Init()
    {
        if (string.IsNullOrEmpty(GameData.main.imgRewardNewbie))
        {
            LoadHttpRankImg(defaultImg);
        }
        else
        {
            LoadHttpRankImg(GameData.main.imgRewardNewbie);
        }
    }
    IEnumerator coroutine = null;
    public void LoadHttpRankImg(string url, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;
        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            rewardImage.sprite = sprite;

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    public void GateFlyIn()
    {
        // Move the card from the start point to the end point
        transform.position = startPoint;
        effect.SetActive(GameData.main.statusNewbie);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(endPoint, flyDuration).SetEase(Ease.OutBack));
        sequence.Play();

    }
    public void OnClickReward()
    {
        //IUtil.OpenURL(m_URL);
        Game.main.socket.GetAccessTokenWebview();

    }
    private void OnDisable()
    {
        this.transform.position = startPoint;
    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.GET_ACCESSTOKEN:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    IUtil.OpenURL(m_URL + "?token=" + cv.aString[0]);
                    break;
                }
        }
        return false;
    }
}