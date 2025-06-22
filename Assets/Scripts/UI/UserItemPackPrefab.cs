using GIKCore;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserItemPackPrefab : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_PackName, m_PackCount;
    [SerializeField] private Image m_PackIcon;

    // Values
    IEnumerator coroutine = null;

    // Methods
    public void SetData(UserItem data)
    {
        m_PackName.text = data.name;
        m_PackCount.text = "x" + data.quantity;
        m_PackIcon.GetComponent<CanvasGroup>().alpha = 0.3f;

        if (data.sprite != null)
        {            
            SetAvatarSprite(data.sprite);
        }
        else
        {
            LoadHttpAvatar(data.image, (s) => { data.sprite = s; });
        }
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
        m_PackIcon.sprite = sprite;
        m_PackIcon.GetComponent<CanvasGroup>().alpha = 1f;
    }
}
