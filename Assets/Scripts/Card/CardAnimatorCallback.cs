using EZCameraShake;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardAnimatorCallback : MonoBehaviour
{
    [SerializeField] private BoardCard card;
    [SerializeField] private Transform landingEffect;
    [SerializeField] private Transform godLinkEffect;
    Transform landing;
    Transform linkToTower;

    public void OnAttackTrigger()
    {
        card.OnAttackTriggerCallback();
    }

    public void OnEndDeadAnim()
    {
        linkToTower.GetComponent<SC_GodLinkColor>().OnGodDead();
        card.OnEndDeadAnim();
    }

    public void OnStartSummon()
    {
        landing = PoolManager.Pools["Effect"].Spawn(landingEffect);
        landing.position = card.GetComponent<BoardCard>().slot.transform.position;
        landing.GetComponent<ParticleSystem>().Play();
    }
    public void OnLandingSummon()
    {
        if (card.heroInfo.type == DBHero.TYPE_GOD)
        {
            CameraShaker.Instance.ShakeOnce(1, 4, .1f, .6f);
            linkToTower = PoolManager.Pools["Effect"].Spawn(godLinkEffect);
            TowerController tower;
            if (GameBattleScene.instance!= null)
            {
                tower = GameBattleScene.instance.lstTowerInBattle.FirstOrDefault(x => x.pos == ((card.cardOwner == CardOwner.Player) ? 0 : 1));
                if (tower != null)
                {
                    linkToTower.GetComponent<SC_GodLinkColor>().SetPosition(card.GetComponent<BoardCard>().slot.transform, tower.transform);
                    card.godLinkEffect = linkToTower.GetComponent<SC_GodLinkColor>();
                }
            }
            else
            {
                tower = BattleSceneTutorial.instance.lstTowerInBattle.FirstOrDefault(x => x.pos == ((card.cardOwner == CardOwner.Player) ? 0 : 1));
                if (tower != null)
                {
                    linkToTower.GetComponent<SC_GodLinkColor>().SetPosition(card.GetComponent<BoardCard>().slot.transform, tower.transform);
                    card.godLinkEffect = linkToTower.GetComponent<SC_GodLinkColor>();
                }
            }    
        }
    }    
    public void OnEndAttack()
    {
        card.OnEndAttackCallback();
        card.ShowHideShadow(true);
        if(linkToTower!= null)
            linkToTower.gameObject.SetActive(true);
    }

    public void OnStartAnim()
    {
        card.ShowHideShadow(false);
        if(linkToTower!= null)
            linkToTower.gameObject.SetActive(false);
    }

    public void OnEndAnim()
    {
        card.ShowHideShadow(true);
    }
}
