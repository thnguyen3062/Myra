using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GIKCore;
using GIKCore.Utilities;
using GIKCore.Lang;

public class PopupConfirm : MonoBehaviour
{
    // Fields
    [SerializeField] private GameObject m_GoClose;
    [SerializeField] private TextMeshProUGUI m_TxtTitle, m_TxtContent;
    [SerializeField] private Transform[] m_GroupAction;
    [SerializeField] private ITween m_TweenFade;

    // Values    
    List<ICallback.CallFunc2<GameObject>> lstCallback = new List<ICallback.CallFunc2<GameObject>>();
    private ICallback.CallFunc2<GameObject> btCloseCallback = null;
    private bool destroyByHand = false;

    // Methods
    public void DoDestroy() { Destroy(gameObject); }
    public void PlayTweenFade() { m_TweenFade.Play(); }
    public void DoClickAction(int index)
    {
        if (index >= 0 && index < lstCallback.Count)
        {
            ICallback.CallFunc2<GameObject> callback = lstCallback[index];
            if (callback != null) callback(gameObject);
        }

        if (!destroyByHand) PlayTweenFade();
    }
    public void DoClickClose()
    {
        if (btCloseCallback != null) btCloseCallback(gameObject);
        PlayTweenFade();
    }

    private void SetActionCallback(params ICallback.CallFunc2<GameObject>[] values)
    {
        lstCallback.Clear();
        foreach (ICallback.CallFunc2<GameObject> func in values)
            lstCallback.Add(func);
    }
    private void SetBtCloseCallback(ICallback.CallFunc2<GameObject> func) { btCloseCallback = func; }
    public void FillData(string content, string title = "default", string action1 = "default", string action2 = "", bool activeBtnClose = false,TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        if (title.Equals("default")) title = "NOTICE";
        if (action1.Equals("default")) action1 = "CONFIRM";

        m_TxtTitle.text = title;
        m_TxtContent.text = content;
        m_TxtContent.alignment = align;
        m_GoClose.SetActive(activeBtnClose);
        m_GroupAction[0].GetComponentInChildren<TextMeshProUGUI>().text = action1;
        m_GroupAction[1].GetComponentInChildren<TextMeshProUGUI>().text = action2;

        if (string.IsNullOrEmpty(action1) && string.IsNullOrEmpty(action2))
        {
            m_GroupAction[0].parent.gameObject.SetActive(false);
            return;
        }
        if (string.IsNullOrEmpty(action1)) m_GroupAction[0].gameObject.SetActive(false);
        if (string.IsNullOrEmpty(action2)) m_GroupAction[1].gameObject.SetActive(false);
    }

    // Use this for initialization
    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    public static void Show(string content, string title = "default",
        string action1 = "default", string action2 = "", bool activeBtnClose = false,
        ICallback.CallFunc2<GameObject> action1Callback = null,
        ICallback.CallFunc2<GameObject> action2Callback = null,
        ICallback.CallFunc2<GameObject> btCloseCallback = null,
        bool destroyByHand = false,
        Transform parent = null,
        TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        if (parent == null)
            parent = Game.main.canvas.panelPopup;
        string assetName = "PopupConfirm";

        GameObject go = IUtil.LoadPrefabWithParent("GIKCore/UI/PopupConfirm/" + assetName, parent);
        go.name = assetName;

        PopupConfirm script = go.GetComponent<PopupConfirm>();
        script.FillData(content, title, action1, action2, activeBtnClose,align);
        script.SetActionCallback(action1Callback, action2Callback);
        script.SetBtCloseCallback(btCloseCallback);
        script.destroyByHand = destroyByHand;
    }
}
