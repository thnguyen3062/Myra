using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillCard : GameListener
{
    private Card card;
    private DBHeroSkill skillA;
    [SerializeField] private GameObject[] skillState;
    private SkillGodActiveState state;
    public void InitData(BoardCard godCard,DBHeroSkill skill)
    {
        card = godCard;
        skillA = skill;
            state = SkillGodActiveState.Lock;
            skillState[2].SetActive(true);

        CheckState();
    }
    private void Start()
    {
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onUpdateMana += OnUpdateMana;
            //GameBattleScene.instance.onUpdateShard += OnUpdateShard;
            GameBattleScene.instance.onGameBattleSimulation += GameBattleSimulation;
            GameBattleScene.instance.onActiveSkillActive += OnSkillActive;

            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            foreach (GameObject sk in skillState)
                sk.SetActive(false);
        }
        else
        {
            foreach (GameObject sk in skillState)
                sk.SetActive(false);
            skillState[0].SetActive(true);
        }    
    }
    public override bool ProcessNetData(int id, object data)
    {
        if (base.ProcessNetData(id, data)) return true;
        switch (id)
        {
            case NetData.CARD_UPDATE_MATRIX:
                {
                    if (GameBattleScene.instance == null)
                        break;
                    CardUpdateHeroMatrix cuhm = (CardUpdateHeroMatrix)data;
                    if (cuhm.battleId == card.battleID)
                    {
                        CheckState();
                    }
                    break;
                }
        }
        return false;
    }
    private void OnUpdateMana(int index, long mana, ManaState state, long usedMana)
    {
        CheckState();
    }
    //private void OnUpdateShard(int index, long shard)
    //{
    //    if (index == 0)
    //    {
    //        CheckState();
    //    }
    //}
    public void GameBattleSimulation(bool isPlayer, long roundCount ,long turnMana)
    {
        if (isPlayer)
        {
            CheckState();
        }
        else
        {
            
        }
    }
    public void OnEndSkillActive()
    {
        CheckState();
    } 
    private void OnSkillActive(long skillId, long battleId )
    {
        if(skillA.id== skillId && card.battleID== battleId)
        {
            state = SkillGodActiveState.Activating;
            if (!skillState[1].activeSelf)
            {
                foreach (GameObject sk in skillState)
                    sk.SetActive(false);
                skillState[1].SetActive(true);
                StartCoroutine(LockSkill());
            }
        }    
    }
    IEnumerator LockSkill()
    {
        yield return new WaitForSeconds(0.2f);
        state = SkillGodActiveState.Lock;
        if (!skillState[2].activeSelf)
        {
            foreach (GameObject sk in skillState)
                sk.SetActive(false);
            skillState[2].SetActive(true);
        }
    }
    private void OnMouseDown()
    {
        if (state == SkillGodActiveState.Activating|| state== SkillGodActiveState.Unlock)
            return;
        if (GameBattleScene.instance.IsYourTurn && GameBattleScene.instance.isGameStarted)
        {
            if (card != null)
            {
                card.OnActiveSkill(skillA);
            }
        }
    }
    private void CheckState()
    {  
        if(CheckHeroActiveSkill())
        {
            if(state != SkillGodActiveState.Activable)
            {
                state = SkillGodActiveState.Unlock;
                if (!skillState[3].activeInHierarchy)
                {
                    foreach (GameObject sk in skillState)
                        sk.SetActive(false);
                    skillState[3].SetActive(true);
                    StartCoroutine(UnlockSkill());
                }

            }   
            //state = SkillGodActiveState.Activable;
        }    
        else
        {
            state = SkillGodActiveState.Lock;
            if(!skillState[2].activeSelf)
            {
                foreach (GameObject sk in skillState)
                    sk.SetActive(false);
                skillState[2].SetActive(true);
            }    
        }    
    }
    IEnumerator UnlockSkill()
    {
        yield return new WaitForSeconds(0.2f);
        state = SkillGodActiveState.Activable;
        if (!skillState[0].activeInHierarchy)
        {
            foreach (GameObject sk in skillState)
                sk.SetActive(false);
            skillState[0].SetActive(true);
        }
    }    
    private bool CheckHeroActiveSkill()
    {
        //change mode skill can active
        if (skillA != null)
        {
            //bool shardOk = CheckShard();
            //if (!shardOk)
            //{
            //    return false;
            //}
            if (skillA.isUltiType && !GameData.main.isUsedUlti)
            {
                return true;
            }
            else if (skillA.isUltiType && GameData.main.isUsedUlti)
            {
                //@Tony
                //card.SetSkillState(false);
                Toast.Show(LangHandler.Get("82", "Ultimate already activated for this match"));
            }
            if (!skillA.isUltiType && card.countDoActiveSkill == 0)
            {
                BoardCard bCard = card.GetComponent<BoardCard>();
                if (bCard != null)
                {
                    if (bCard.isTired)
                        Toast.Show(LangHandler.Get("145"," You can't use active skill when god is tired."));
                    else
                        return true;
                }
                //return true;
            }
            else if (!skillA.isUltiType && card.countDoActiveSkill != 0)
            {
                Toast.Show(LangHandler.Get("146", " You can use active skill once per turn."));
            }
        }
        else
        {
        }
        return false;
    }
    private bool CheckShard()
    {

        bool isOk = false;
        if (card != null)
        {
            //if (card.countShardAddded >= skillA.min_shard)
            //{
            //    if (skillA.max_shard == -1)
            //        isOk = true;
            //    else if (card.countShardAddded <= skillA.max_shard)
            //        isOk = true;
            //    foreach (ConditionSkill cond in skillA.lstConditionSkill)
            //    {
            //        if (cond.type == 19)
            //        {
            //            if (cond.number <= GameBattleScene.instance.currentShard)
            //                isOk = true;
            //            else
            //                isOk = false;
            //        }
            //    }
            //}
        }
        else isOk = false;
                return isOk;
    }
}
    
