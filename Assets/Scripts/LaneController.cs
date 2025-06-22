using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LaneController : MonoBehaviour
{
    public long id;
    [SerializeField] private Transform effectContainer;
    [SerializeField] private Transform highlighEffect;
    private Transform highlightLaneEffect;
    private bool canSelect = false;

    public event ICallback.CallFunc2<LaneController> onAddToListSkill;
    //public event ICallback.CallFunc2<LaneController> onRemoveFromListSkill;
    public event ICallback.CallFunc2<LaneController> onEndSkillActive;

    private void Start()
    {
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            GameBattleScene.instance.onFinishChooseOneTarget += OnEndSkillActive;
        }
    }

    public void HighlightLane()
    {
        if (highlightLaneEffect == null)
        {
            Transform eff = PoolManager.Pools["Effect"].Spawn(highlighEffect.transform, effectContainer);
            eff.transform.localPosition = Vector3.zero;
            eff.transform.localScale = Vector3.one;
            highlightLaneEffect = eff;
            highlightLaneEffect.GetComponent<Animation>().Play("HighlightBlue");
        }
        transform.GetComponent<BoxCollider>().enabled = true;
        canSelect = true;
    }

    public void UnHighlightLane()
    {
        canSelect = false;
        if (highlightLaneEffect != null)
        {
            transform.GetComponent<BoxCollider>().enabled = false;
            PoolManager.Pools["Effect"].Despawn(highlightLaneEffect);
            highlightLaneEffect = null;
        }
    }

    private void OnMouseDown()
    {
        if (GameBattleScene.instance.skillState == SkillState.None)
            return;
        if (GameBattleScene.instance.selectedLane == null)
        {
            if (canSelect)
                onAddToListSkill?.Invoke(this);
        }
        if(canSelect)
        {
            if (highlightLaneEffect != null)
                highlightLaneEffect.GetComponent<Animation>().Play("HighlightBlue_Click");
        }    
        //else
        //    onRemoveFromListSkill?.Invoke(this);
    }

    private void OnEndSkillActive()
    {
        UnHighlightLane();
        onEndSkillActive?.Invoke(this);
    }
}
