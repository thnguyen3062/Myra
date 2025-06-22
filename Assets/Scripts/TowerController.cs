using GIKCore.Sound;
using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using EZCameraShake;
using DG.Tweening;

public class TowerController : MonoBehaviour
{
    public int pos;
    public int id;
    #region Serialized Fields
    [HideInInspector] public long towerHealth;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private Transform highlighEffect;
    [SerializeField] private GameObject[] m_CrystalForm;
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private Texture[] m_TowerState;
    [SerializeField] private GameObject m_DestroyEffect;
    [SerializeField] private SC_HP_Tower towerStateController;
    [SerializeField] private GameObject m_GroundHole;
    #endregion
    private VisualEffect highlightVisualEffect;
    private bool canSelect = false;
    private VisualEffect healingVisualEffect;
    [SerializeField] private Transform healingEffect;
    [SerializeField] private SkinnedMeshRenderer[] materialsCrystal;
    [SerializeField] private SkinnedMeshRenderer[] materialCrystal;
    [SerializeField] private MeshRenderer[] meshMaterialCrytal;
    [SerializeField] private Sc_TriggerSlowmo m_slow;
    public ICallback.CallFunc2<TowerController> onAddToListSkill;
    //public ICallback.CallFunc2<TowerController> onRemoveFromListSkill;
    public ICallback.CallFunc2<TowerController> onEndSkillActive;

    private void Start()
    {   
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            GameBattleScene.instance.onTowerDestroyed += OnTowerDestroyed;
            GameBattleScene.instance.onInitPlayer += OnInitHealth;

            //towerHealth = 15;
            towerHealth = 200;
            healthText.text = towerHealth.ToString();
        }
        if (m_DestroyEffect != null)
        {
            m_DestroyEffect.GetComponent<ParticleEffectCallback>().SetOnComplete(() =>
            {
                m_DestroyEffect.SetActive(false);
            });
        }
        if (materialsCrystal != null)
        {
            foreach (SkinnedMeshRenderer mesh in materialsCrystal)
            {
                foreach (Material m in mesh.materials)
                {
                    Texture mat = CardData.Instance.GetRemandBlueCrystals();
                    m.SetTexture("_BaseMap", mat);
                    m.SetTexture("_EmissionMap", mat);
                }
            }
        }
        if (materialCrystal != null)
        {
            foreach (SkinnedMeshRenderer mesh in materialCrystal)
            {
                Texture mat = CardData.Instance.GetRemandBlueCrystals();
                mesh.material.SetTexture("_BaseMap", mat);
                mesh.material.SetTexture("_EmissionMap", mat);
            }
        }
        if (meshMaterialCrytal != null)
        {
            foreach (MeshRenderer mesh in meshMaterialCrytal)
            {
                Texture mat = CardData.Instance.GetRemandBlueCrystals();
                mesh.material.SetTexture("_BaseMap", mat);
                mesh.material.SetTexture("_EmissionMap", mat);
            }
        }
    }
    private void OnInitHealth(bool pos6H, string screenName, long towerHeath)
    {
        int posTower = (pos6H == true) ? 0 : 1;
        if (pos == posTower)
            UpdateHealth(towerHeath);
        else
            return;
    }    
    public void OnDamaged(long damage, long health, bool isDestroyed)
    {
        DamagePopup.Create(transform.GetChild(0).position, damage, PopupType.Damage);
        UpdateHealth(health);
        towerStateController.UpdateTowerState (health, pos == 0 ? true : false);
        m_MeshRenderer.material.SetFloat("_Hp", ((float)health/10));
        if (isDestroyed)
        {
            // chì còn tr? chính , n? là thua 
            m_slow.Shoot();
            CameraShaker.Instance.ShakeOnce(2, 6, .1f, 3f);
            SoundHandler.main.PlaySFX("Nexus Explosion", "sounds");
            foreach (GameObject obj in m_CrystalForm)
                obj.SetActive(false);
            m_GroundHole.SetActive(true);
        }
    }

    /// <summary>
    /// N?u 2 pos khác nhau -> return
    /// N?u id = nhau && id != 1 (không ph?i tower gi?a) -> chuy?n tr?ng thái sang t??ng v?
    /// ??ng th?i chuy?n tr?ng thái t??ng gi?a v? tr?ng thái v? t??ng ?ng (break trái -> tower state[1], ph?i -> tower state[2])
    /// N?u id == nhau && id == 1 -> chuy?n tr?ng thái t??ng v? sang v? 2 bên (tower state[0]) và chuy?n tr?ng thái tr?
    /// </summary>
    /// <param name="towerPos"></param>
    /// <param name="towerID"></param>

    private void OnTowerDestroyed(int towerPos, int towerID, bool isResume)
    {
        if (towerPos != pos)
            return;

        StartCoroutine(OnTowerDestroyedRoutine(towerID, isResume));
    }

    IEnumerator OnTowerDestroyedRoutine(int towerID, bool isResume)
    {
        float waitTime = 0f;
        if (!isResume && m_DestroyEffect != null)
        {
            m_DestroyEffect.SetActive(true);
            m_DestroyEffect.GetComponent<ParticleEffectCallback>().SetPlayEffect();
            waitTime = 0.5f;
        }
        yield return new WaitForSeconds(waitTime);
        // tru giua ve id ==0
        if (towerID == id)
        {
            if (id == 0)
            {
                foreach(GameObject obj in m_CrystalForm)
                    obj.SetActive(false);
                m_GroundHole.SetActive(true);
            }
        }
    }

    public void OnHealing(long health, long value)
    {
        UpdateHealth(health);
        towerStateController.UpdateTowerState(health, pos == 0 ? true : false);  
        m_MeshRenderer.material.SetFloat("_Hp", ((float)health/10));
        if (healingVisualEffect == null)
        {
            Transform effect = PoolManager.Pools["Effect"].Spawn(healingEffect);
            effect.position = transform.position;
            effect.parent = transform;
            healingVisualEffect = effect.GetComponent<VisualEffect>();
        }
        if (healingVisualEffect != null)
            healingVisualEffect.Play();
        DamagePopup.Create(transform.position, value, PopupType.Bonus, () =>
        {
            healingVisualEffect.Stop();
        });
    }

    public void UpdateHealth(long health)
    {
        towerHealth = health;
        healthText.text = towerHealth.ToString();
    }

    public void TweenHP(long hp, float duration ,ICallback.CallFunc callback)
    {
        m_CrystalForm[0].SetActive(true);
        Sequence s = DOTween.Sequence();
        s.Append(healthText.DOCounter((int)towerHealth, (int)hp, duration / 2).SetEase(Ease.InOutSine))
          .Join(transform.DOScale(new Vector3(.3f, .3f, 0f), duration).SetLoops(2, LoopType.Yoyo))
          .OnComplete( () =>
          {
              if (callback != null)
                  callback?.Invoke();
          });

    }
    public void OnBeingAttacked()
    {
        LogWriterHandle.WriteLog("Tower " + id + " Hit");
    }

    public void HighLightTower()
    {
        if (highlightVisualEffect == null)
        {
            Transform eff = PoolManager.Pools["Effect"].Spawn(highlighEffect.transform);
            eff.transform.position = transform.position;
            eff.transform.parent = transform;
            eff.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            highlightVisualEffect = eff.GetComponent<VisualEffect>();
        }
        highlightVisualEffect.Play();
        canSelect = true;
    }

    public void UnHighlightTower()
    {
        canSelect = false;
        if (highlightVisualEffect != null)
        {
            highlightVisualEffect.Stop();
        }
    }

    private void OnEndSkillActive()
    {
        UnHighlightTower();
        onEndSkillActive?.Invoke(this);
    }

    private void OnMouseDown()
    {
        if (GameBattleScene.instance == null)
            return;
        if (GameBattleScene.instance.skillState == SkillState.None)
            return;

        if (GameBattleScene.instance.selectedTower == null)
        {
            if (canSelect)
                onAddToListSkill?.Invoke(this);
        }
    }
}
