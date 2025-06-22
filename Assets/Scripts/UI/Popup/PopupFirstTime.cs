using GIKCore.Utilities;
using GIKCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore.Lang;

public class PopupFirstTime : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_Content;

    // Values

    // Methods
    public PopupFirstTime SetData(string key)
    {
        string content = Database.GetContentFirstTime(key);
        m_Content.text = LangHandler.Get(content);
        return this;
    }
    public void DoClickClose()
    {
        Destroy(gameObject);
    }
    public static void Show(string key)
    {
        string assetName = "PopupFirstTime";
        GameObject go = IUtil.LoadPrefabWithParent("Prefabs/Home/" + assetName, Game.main.canvas.panelPopup);
        PopupFirstTime script = go.GetComponent<PopupFirstTime>();
        script.SetData(key);
    }
}
