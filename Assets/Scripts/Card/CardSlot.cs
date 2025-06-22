using PathologicalGames;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
using Spine.Unity;
using GIKCore.Utilities;

public class CardSlot : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Transform effectContainer;
    [SerializeField] private GameObject slotEffectPrefab;
    [SerializeField] private GameObject slotSelectEffectPrefab;
    private GameObject slotEffect,selectSlotEffect;
    
    #endregion
    public long xPos;
    public long yPos;
    /*[HideInInspector]*/
    public Card currentCard;
    [HideInInspector] public SlotState state = SlotState.Empty;
    public SlotType type;
    private bool canSelect = false;

    [SerializeField] private Transform summonEffect;
    private SkeletonAnimation summonSkeleton;

    public event ICallback.CallFunc2<CardSlot> onAddToListSkill;
    //public event ICallback.CallFunc2<CardSlot> onRemoveFromListSkill;
    public event ICallback.CallFunc2<CardSlot> onEndSkillActive;

    private void Start()
    {
        if(GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            GameBattleScene.instance.onFinishChooseOneTarget += OnEndSkillActive;
        }   
            effectContainer = transform.GetChild(0);
    }

    public void ChangeSlotState(SlotState targetState, BoardCard card)
    {
        state = targetState;
        currentCard = card;
    }

    public void HighLightSlot()
    {
        if (slotEffect == null)
        {
            slotEffect = PoolManager.Pools["Effect"].Spawn(slotEffectPrefab).gameObject;
            slotEffect.transform.parent = effectContainer;
            slotEffect.transform.localPosition = Vector3.zero;
        }
        slotEffect.SetActive(true);
        canSelect = true;
    }
    public void HighlightSelectedSlot()
    {
        if(slotEffect!=null&& canSelect)
        {
            selectSlotEffect = PoolManager.Pools["Effect"].Spawn(slotSelectEffectPrefab).gameObject;
            selectSlotEffect.transform.parent = effectContainer;
            selectSlotEffect.transform.localPosition = Vector3.zero;
            selectSlotEffect.SetActive(true);
            slotEffect.SetActive(false);
        }
           
    }    
    public void UnHighlightSelectedSlot()
    {
        if (selectSlotEffect != null)
            selectSlotEffect.SetActive(false);
        if(slotEffect!=null && canSelect)
            slotEffect.SetActive(true);
    }    
    public void UnHighLightSlot()
    {
        canSelect = false;
        if (slotEffect != null)
            slotEffect.SetActive(false);
        if (selectSlotEffect != null)
            selectSlotEffect.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (GameBattleScene.instance == null)
            return;
        if (GameBattleScene.instance.skillState == SkillState.None)
                return;
            if (GameBattleScene.instance.selectedCardSlot == null)
            {
                if (canSelect)
                    onAddToListSkill?.Invoke(this);
            }
        //else
            //onRemoveFromListSkill?.Invoke(this);
    }

    private void OnEndSkillActive()
    {
        UnHighLightSlot();
        onEndSkillActive?.Invoke(this);
    }
}
