using GIKCore;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeMapLevelProps
{
    public int level;
    public int state; // 0 => not pass, 1 => current level, 2 => pass
    public string image;
    public bool isHard;
    public bool isUserLevel;
}
public class HomeMapLevel : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_ListState;
    [SerializeField] private GameObject m_LabelHard;
    [SerializeField] private GameObject m_LabelReward;
    [SerializeField] private Image m_Image;
    [SerializeField] private TextMeshProUGUI m_TxtLevel;
    [SerializeField] public Transform m_AvatarPosition;
    public HomeMapLevel SetData(HomeMapLevelProps props)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < m_ListState.Count; i++)
            m_ListState[i].SetActive(i == props.state);

        m_LabelHard.SetActive(props.isHard);
        m_LabelReward.SetActive(!props.isUserLevel && (props.state == 1 || props.state == 0));
        m_TxtLevel.text = props.level + "";
        m_TxtLevel.gameObject.SetActive(props.state == 1 || props.state == 0);
        IEnumerator coroutine = IUtil.LoadTexture2DFromUrl(props.image, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            if (m_Image != null && sprite != null)
                m_Image.sprite = sprite;
        });
        Game.main.StartCoroutine(coroutine);  
        return this;
    }
    public void DoClick()
    {
        HandleNetData.QueueNetData(NetData.DO_CLICK_OPEN_HOME_LEVEL);
    }
}
