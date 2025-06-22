using GIKCore;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellQuest : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_Icon;
    [SerializeField] private GameObject m_Expire;
    [SerializeField] private TextMeshProUGUI m_QuestName;

    // Values
    IEnumerator coroutine = null;

    // Methods
    public void SetData(QuestProps data)
    {
        m_Expire.SetActive(false);
        m_QuestName.text = data.name;

        if (data.sprite != null) SetAvatarSprite(data.sprite);
        else LoadHttpAvatar(data.icon, (s) => { data.sprite = s; });
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
        if(m_Icon!= null)
         m_Icon.sprite = sprite;
    }
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}
}
