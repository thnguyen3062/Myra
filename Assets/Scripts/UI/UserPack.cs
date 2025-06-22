using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UIEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GIKCore;

public class UserPack : MonoBehaviour
{
    [SerializeField] private Image packImg;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private TextMeshProUGUI m_name;
    [SerializeField] private UIDragDrop m_UIDragDrop;
    [SerializeField] private GameObject packClone;
    private Vector3 initPosition;
    private PackInfo data;

    private ICallback.CallFunc2<PackInfo> onEndDragCB = null;
    public void SetOnEndDragCallback(ICallback.CallFunc2<PackInfo> func) { onEndDragCB = func; }
    public long CurrentPackID
    {
        get;
        private set;
    }
    public UIDragDrop Event
    {
        get
        {
            return m_UIDragDrop;
        }
    }
    public bool allowDrag
    {
        get
        {
            return m_UIDragDrop.allowDrag;
        }
        set
        {
            m_UIDragDrop.allowDrag = value;
        }
    }
    void Start()
    {
        Event
            .SetOnBeginDragCallback((go) => { OnBeginDragDeckCard(); })
            .SetOnEndDragCallback((go) => { OnDropCard(go); });
    }
    public void InitData(PackInfo data)
    {
        this.data = data;
        CurrentPackID = data.packId;
        number.text = data.quantity.ToString();
        m_name.text = data.packName.ToString();
        initPosition = transform.position;
    }
    private void OnBeginDragDeckCard()
    {
        GameObject go = Instantiate(packClone, transform.GetChild(1).localPosition, Quaternion.identity, Game.main.canvas.panelPopup);
        
        CanvasGroup cvGroup = go.GetComponent<CanvasGroup>();
        cvGroup.blocksRaycasts = false;
        
        Event.rectTransform = go.GetComponent<RectTransform>();
        Event.rectTransform.sizeDelta = new Vector2(236, 205);
        initPosition = Event.rectTransform.position;
    }
    private void OnDropCard(GameObject go)
    {
        //tao tween 
        CanvasGroup cvGroup = go.GetComponent<CanvasGroup>();
        cvGroup.blocksRaycasts = true;
        if (onEndDragCB != null)
        {
            onEndDragCB(data);
        }
           
            Destroy(go);
        Event.rectTransform = this.GetComponent<RectTransform>();
    }
}
