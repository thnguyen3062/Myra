using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;

public class CardSkillHandler : MonoBehaviour
{
    public void CastSkill(long skillID, long effectID, Transform cardSkill, ICallback.CallFunc callback, GameObject target = null,bool waitToImpact =true)
    {
        ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillID && x.effectID == effectID);
        if (info != null)
        {
            Projectile projectile = ProjectileData.Instance.projectile.FirstOrDefault(x => x.type == info.type);
            if (projectile != null)
            {
                if (projectile.type == ProjectileType.Lightning)
                {  
                    StartCoroutine(OnAttackLightningEffect(projectile.projectile, target, callback));
                }
                else if (!info.requireProjectile)
                {
                    if (projectile.impact == null)
                        StartCoroutine(OnAttackWithoutProjectileNoTarget(cardSkill, projectile.muzzle, callback));
                    else
                    {
                        // neu la skill fight cua ares thì attack on the same time 
                        if(!waitToImpact)
                            StartCoroutine(OnAttackWithoutProjectileOnTheSameTime(cardSkill, projectile.muzzle, projectile.impact, target, callback));
                        else
                            StartCoroutine(OnAttackWithoutProjectile(cardSkill, projectile.muzzle, projectile.impact, target, callback));

                    }    
                }
                else
                {
                    StartCoroutine(OnAttackWithProjectile(cardSkill, projectile.muzzle, projectile.projectile, projectile.impact, target, callback));
                }
            }
        }
    }

    public IEnumerator OnAttackWithProjectile(Transform buffCard, Transform muzzle, Transform projectileObject, Transform impactObject, GameObject target, ICallback.CallFunc callback)
    {
        if (muzzle != null)
        {
            Transform start = PoolManager.Pools["Effect"].Spawn(muzzle);
            start.position = buffCard.position + new Vector3(0, 0.1f, 0);
            if(start.GetComponent<ParticleEffectParentCallback>() != null)
                start.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
            else if(start.GetComponent<AnimationParentCallback>() != null)
                start.GetComponent<AnimationParentCallback>()
                .SetOnEndAnim(() =>
                {
                    PoolManager.Pools["Effect"].Despawn(start);
                }).EndAnim();
            yield return new WaitForSeconds(0.4f);
        }

        Transform trans = PoolManager.Pools["Effect"].Spawn(projectileObject);

        trans.position = transform.position + new Vector3(0, 0.1f, 0);
        trans.GetComponent<ShootProjectile>().ShootObject(trans.position, target.transform.position, () =>
        {
            Transform transImpact = PoolManager.Pools["Effect"].Spawn(impactObject);
            transImpact.position = target.transform.position + new Vector3(0, 0.1f, 0);
            transImpact.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
        });
        yield return new WaitForSeconds(0.6f);
        callback?.Invoke();
        PoolManager.Pools["Effect"].Despawn(trans);

    }

    public IEnumerator OnAttackWithoutProjectile(Transform buffCard, Transform muzzle, Transform impactObject, GameObject target, ICallback.CallFunc callback)
    {

        if (muzzle != null)
        {
            Transform start = PoolManager.Pools["Effect"].Spawn(muzzle);
            start.position = buffCard.position + new Vector3(0, 0.1f, 0);
            if (start.GetComponent<ParticleEffectParentCallback>() != null)
                start.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
            else if (start.GetComponent<AnimationParentCallback>() != null)
            {
                start.GetComponent<AnimationParentCallback>()
                .SetOnEndAnim(() =>
                {
                    PoolManager.Pools["Effect"].Despawn(start);
                }).EndAnim();
            }
            yield return new WaitForSeconds(0.4f);
        }

        Transform trans = PoolManager.Pools["Effect"].Spawn(impactObject);

        trans.position = target.transform.position + new Vector3(0, 0.1f, 0);
        if (trans.GetComponent<ParticleEffectParentCallback>() != null)
            trans.GetComponent<ParticleEffectParentCallback>()
            .SetOnPlay();
        else if (trans.GetComponent<AnimationParentCallback>() != null)
            trans.GetComponent<AnimationParentCallback>()
            .SetOnEndAnim(() =>
            {
                PoolManager.Pools["Effect"].Despawn(trans);
            }).EndAnim();

        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }
    public IEnumerator OnAttackWithoutProjectileOnTheSameTime(Transform buffCard, Transform muzzle, Transform impactObject, GameObject target, ICallback.CallFunc callback)
    {

        if (muzzle != null)
        {
            Transform start = PoolManager.Pools["Effect"].Spawn(muzzle);
            start.position = buffCard.position + new Vector3(0, 0.1f, 0);
            if (start.GetComponent<ParticleEffectParentCallback>() != null)
                start.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
            else if (start.GetComponent<AnimationParentCallback>() != null)
            {
                start.GetComponent<AnimationParentCallback>()
                .SetOnEndAnim(() =>
                {
                    PoolManager.Pools["Effect"].Despawn(start);
                }).EndAnim();
            }
        }
        if (impactObject != null)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(impactObject);

            trans.position = target.transform.position + new Vector3(0, 0.1f, 0);
            if (trans.GetComponent<ParticleEffectParentCallback>() != null)
                trans.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
            else if (trans.GetComponent<AnimationParentCallback>() != null)
                trans.GetComponent<AnimationParentCallback>()
                .SetOnEndAnim(() =>
                {
                    PoolManager.Pools["Effect"].Despawn(trans);
                }).EndAnim();
        } 

        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }
    public IEnumerator OnAttackWithoutProjectileNoTarget(Transform buffCard, Transform muzzle, ICallback.CallFunc callback)
    {
        if (muzzle != null)
        {
            Transform start = PoolManager.Pools["Effect"].Spawn(muzzle);
            start.position = buffCard.position + new Vector3(0, 0.1f, 0);
            if (start.GetComponent<ParticleEffectParentCallback>() != null)
                start.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
            else if (start.GetComponent<AnimationParentCallback>() != null)
            {
                start.GetComponent<AnimationParentCallback>()
                .SetOnEndAnim(() =>
                {
                    PoolManager.Pools["Effect"].Despawn(start);
                }).EndAnim();
            }
            yield return new WaitForSeconds(0.4f);
        }
        callback?.Invoke();
    }
    public IEnumerator OnAttackLightningEffect(Transform projectileObject, GameObject target, ICallback.CallFunc callback)
    {
        Transform trans = PoolManager.Pools["Effect"].Spawn(projectileObject);
        trans.position = target.transform.position + new Vector3(0, 4.5f, 0);
        trans.GetComponent<VisualEffect>().Play();
        yield return new WaitForSeconds(0.4f);
        callback?.Invoke();
        yield return new WaitForSeconds(1f);
        trans.GetComponent<VisualEffect>().Stop();
        PoolManager.Pools["Effect"].Despawn(trans);
    }
}