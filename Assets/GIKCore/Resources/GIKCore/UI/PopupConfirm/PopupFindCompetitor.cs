using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GIKCore;
using GIKCore.Utilities;
using GIKCore.Lang;

public class PopupFindCompetitor : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_TxtTitle, m_TxtContent;
    [SerializeField] private ITween m_TweenFade;

    // Values    
  
    // Methods
    public void ButtonCancelClick()
    {
        Game.main.socket.GameBattleLeave();
        PlayTweenFade();
    }

    public void DoDestroy() { Destroy(gameObject); }
    public void PlayTweenFade() { m_TweenFade.Play(); }
   
    public static void Show(Transform parent = null)
    {
        if (parent == null)
            parent = Game.main.canvas.panelPopup;
        string assetName = "PopupFindCompetitor";

        GameObject go = IUtil.LoadPrefabWithParent("Prefabs/Gamb/" + assetName, parent);
        go.name = assetName;
        PopupFindCompetitor script = go.GetComponent<PopupFindCompetitor>();
       }
}
