using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Utilities;

public class PopupLoading : MonoBehaviour
{
    private const float TIMEOUT_SEC = 4f;

    // Fields
    [SerializeField] private TweenAlphaCanvasGroup m_TweenFade;

    // Values
    private float timeout = 0f;

    // Methods
    public void DoDestroy() { Destroy(gameObject); }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeout += Time.deltaTime;
        if (timeout >= TIMEOUT_SEC)
        {
            timeout = 0f;
            m_TweenFade.Play();
        }
    }

    public static void Show()
    {
        string assetName = "PopupLoading";
        Transform parent = Game.main.canvas.panelPopup;
        Transform target = parent.Find(assetName);

        if (target == null)
        {
            GameObject go = IUtil.LoadPrefabWithParent("Prefabs/Common/" + assetName, parent);
            go.name = assetName;
            target = go.transform;
        }

        target.SetAsLastSibling();
    }
}
