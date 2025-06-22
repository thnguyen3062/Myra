using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GIKCore;
using GIKCore.Utilities;

public class Sc_NotiDoTween : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    [SerializeField] private Image rewardImage;
    public float flyDuration = 0.5f;
    private Vector3 startPoint;
    private Vector3 endPoint;

    private string defaultImg = "https://files.mytheria.io/file/WebEvent/Newbie-DefaultReward.png";
    private void Start()
    {
        Init();
        startPoint = this.transform.position + new Vector3(20, 0, 0); ;
        endPoint = this.transform.position;
        NotiFlyIn();
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
    public void NotiFlyIn()
    {
        // Move the card from the start point to the end point
        transform.position = startPoint;

        Transform tickObject = FindChildByName(transform, "Tick");
        Sequence sequence2 = DOTween.Sequence();

        if (tickObject != null)
        {
            sequence2.Append(tickObject.transform.DOPunchScale(new Vector3(2, 2, 2), 0.5f, 10, 1f));
        }
        else
        {
            Debug.LogError("Child object 'Tick' not found.");
        }
        effect.SetActive(GameData.main.statusNewbie);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(endPoint, flyDuration).SetEase(Ease.OutBack));
        sequence.AppendInterval(6f)
            .OnComplete(() =>
            {
                this.transform.parent.gameObject.SetActive(false);
            });
        sequence.Play();
    }
    public void DoClickNoti()
    {
        this.transform.localScale = Vector3.one;
        this.transform.DOPunchScale(new Vector3(2, 2, 2), 0.5f, 10, 1f).onComplete += delegate
        {
            Game.main.LoadScene("HomeSceneNew");
        };
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        // Search for the child object by name
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }

            Transform foundChild = FindChildByName(child, name);
            if (foundChild != null)
            {
                return foundChild;
            }
        }

        return null; // Child object not found
    }
}