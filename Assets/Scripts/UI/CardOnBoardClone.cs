using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnBoardClone : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshPro healthOnBoardText;
    [SerializeField] private TextMeshPro damageOnBoardText;
    [SerializeField] private MeshRenderer spellBoardMesh;
    [SerializeField] private GameObject[] boardMesh;
    [SerializeField] private MeshRenderer outline;
    [SerializeField] private Transform premiumEffects;
    [SerializeField] private Material normalMatFrameMinion;
    [SerializeField] private Material normalMatPrintMinion;
    [SerializeField] private Material normalMatFrameHighGod, normalMatFrameLowGod;
    [SerializeField] private Material normalMatPrintGod;
    private bool canSelect;
    private DBHero heroInfo;
    public long cloneBattleId;
    public long cloneHeroID;
    public long cloneFrame;
    
    [HideInInspector] public CardSlot cloneSlot;

    public event ICallback.CallFunc2<CardOnBoardClone> onAddToListSkill;
    //public event ICallback.CallFunc2<CardOnBoardClone> onRemoveFromListSkill;
    public event ICallback.CallFunc2<CardOnBoardClone> onEndSkillActive;

    
    private void Start()
    {
        if (GameBattleScene.instance!=null)
            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
    }

    public void InitData(DBHero hero,long battleId,long frame)
    {
        heroInfo = hero;
        cloneBattleId = battleId;
        cloneFrame = frame;
        cloneHeroID = hero.id;
        Texture cardTexture = null;
        if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC ||heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
        {
            cardTexture = CardData.Instance.GetOnBoardTexture(heroInfo.id);
            if (spellBoardMesh != null)
                spellBoardMesh.material.SetTexture("_print", cardTexture);
        }
        else
        {
            cardTexture = CardData.Instance.GetOnBoardTexture(heroInfo.id);
            if (boardMesh != null)
            {
                if (heroInfo.type == DBHero.TYPE_GOD)
                {
                    Texture cardFrame = CardData.Instance.GetCardFrameTexture("GodB_" + frame + "_" + heroInfo.rarity + "_" + heroInfo.color);
                    Texture frameMask = CardData.Instance.GetCardFrameMaskTexture("MGodB_" + heroInfo.rarity);
                    Texture flareMask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
                    foreach (GameObject go in boardMesh)
                        go.SetActive(false);
                    GameObject f = boardMesh[heroInfo.rarity - 1];
                    f.SetActive(true);
                    if (heroInfo.rarity > 3)
                    {
                        var mat = f.GetComponent<SkinnedMeshRenderer>().materials;
                        mat[0] = normalMatPrintGod;
                        mat[1] = normalMatFrameLowGod;
                        mat[1].SetTexture("_print_img", cardFrame);
                        mat[1].SetTexture("FrameMask", frameMask);
                        mat[1].SetTexture("_FlareMask", flareMask);
                        f.GetComponent<SkinnedMeshRenderer>().materials = mat;
                        if (frame >= 3)
                        {
                            var mats = f.GetComponent<SkinnedMeshRenderer>().materials;
                            mats[0] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroInfo.id +"_3d")) as Material;
                            mats[0].shader = Shader.Find("ReadyPrefab/Shader3.14");
                            mats[0].SetVector("_TileOff", new Vector3(0.68f, 0.93f, 0.17f));
                            //set frame
                            Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frame)) as Material;
                            mats[1] = matframe;
                            Texture main = CardData.Instance.GetCardFrameTexture("GodB_" + frame + "_" + heroInfo.rarity + "_" + heroInfo.color);
                            //mats[1].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                            mats[1].SetTexture("_MainTex", main);
                            Texture mask = CardData.Instance.GetCardFrameMaskTexture("MGodB_" + heroInfo.rarity);
                            //mats[1].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                            mats[1].SetTexture("_FrameMask", mask);
                            mats[1].SetTexture("_FlareMask", flareMask);
                            f.GetComponent<SkinnedMeshRenderer>().materials = mats;

                            //f.GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 2);
                            premiumEffects.GetChild((int)frame - 3).gameObject.SetActive(true);
                            premiumEffects.GetChild((int)frame - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
                        }
                        else
                        {
                            f.GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 0);
                            f.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_print_img", cardTexture);
                        }
                    }
                    else
                    {
                        var mat = f.GetComponent<SkinnedMeshRenderer>().materials;
                        mat[0] = normalMatFrameLowGod;
                        mat[1] = normalMatPrintGod;
                        mat[0].SetTexture("_print_img", cardFrame);
                        mat[0].SetTexture("FrameMask", frameMask);
                        mat[0].SetTexture("_FlareMask", flareMask);
                        f.GetComponent<SkinnedMeshRenderer>().materials = mat;

                        if (frame >= 3)
                        {
                            var mats = f.GetComponent<SkinnedMeshRenderer>().materials;
                            mats[1] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroInfo.id+"_3d")) as Material;
                            mats[1].shader = Shader.Find("ReadyPrefab/Shader3.14");
                            mats[1].SetVector("_TileOff", new Vector3(0.68f, 0.93f, 0.17f));
                            //set frame
                            Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frame)) as Material;
                            mats[0] = matframe;
                            Texture main = CardData.Instance.GetCardFrameTexture("GodB_" + frame + "_" + heroInfo.rarity + "_" + heroInfo.color);
                            //mats[0].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                            mats[0].SetTexture("_MainTex", main);
                            Texture mask = CardData.Instance.GetCardFrameMaskTexture("MGodB_" + heroInfo.rarity);
                            //mats[0].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);

                            mats[0].SetTexture("_FrameMask", mask);

                            mats[0].SetTexture("_FlareMask", flareMask);
                            f.GetComponent<SkinnedMeshRenderer>().materials = mats;
                            //f.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 1);
                            premiumEffects.GetChild((int)frame - 3).gameObject.SetActive(true);
                            premiumEffects.GetChild((int)frame - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
                        }
                        else
                        {
                            f.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 0);
                            f.GetComponent<SkinnedMeshRenderer>().materials[1].SetTexture("_print_img", cardTexture);
                        }
                    }
                }
                else
                {
                    Texture cardFrame = CardData.Instance.GetCardFrameTexture("MortalB_" + frame + "_" + /*hero.rarity*/1);
                    Texture frameMask = CardData.Instance.GetCardFrameMaskTexture("MMortalB_" + /*hero.rarity*/1);
                    Texture flareMask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");

                    if (heroInfo.rarity > 3)
                    {
                        boardMesh[1].SetActive(true);
                        var mat = boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials;
                        mat[0] = normalMatPrintMinion;
                        mat[1] = normalMatFrameMinion;

                        mat[1].SetTexture("_print_img", cardFrame);
                        mat[1].SetTexture("FrameMask", frameMask);
                        mat[1].SetTexture("_FlareMask", flareMask);
                        boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials = mat;

                        boardMesh[0].SetActive(false);
                        if (frame >= 3)
                        {
                            var mats = boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials;
                            mats[0] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroInfo.id+ "_3d")) as Material;
                            mats[0].shader = Shader.Find("ReadyPrefab/Shader3.14");
                            mats[0].SetVector("_TileOff", new Vector4(0.6f, 0.9f, 0.15f, 0.15f));
                            //set frame
                            Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frame)) as Material;
                            mats[1] = matframe;
                            Texture main = CardData.Instance.GetCardFrameTexture("MortalB_" + frame + "_" + /*hero.rarity*/1);
                            //mats[1].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                            mats[1].SetTexture("_MainTex", main);
                            Texture mask = CardData.Instance.GetCardFrameMaskTexture("MMortalB_" + /*hero.rarity*/1);
                            //mats[1].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                            mats[1].SetTexture("_FrameMask", mask);

                            mats[1].SetTexture("_FlareMask", flareMask);
                            boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials = mats;

                            //boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 1);
                            premiumEffects.GetChild((int)frame - 3).gameObject.SetActive(true);
                            premiumEffects.GetChild((int)frame - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
                        }
                        else
                        {
                            boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 0);
                            boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_print_img", cardTexture);
                        }
                    }
                    else
                    { // thu tu material bi dao nguoc thu tu giua low va high frame
                        boardMesh[1].SetActive(false);
                        boardMesh[0].SetActive(true);
                        var mat = boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials;
                        mat[1] = normalMatPrintMinion;
                        mat[0] = normalMatFrameMinion;

                        mat[0].SetTexture("_print_img", cardFrame);
                        mat[0].SetTexture("FrameMask", frameMask);
                        mat[0].SetTexture("_FlareMask", flareMask);
                        boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials = mat;
                        if (frame >= 3)
                        {
                            var mats = boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials;
                            mats[1] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroInfo.id +"_3d")) as Material;
                            mats[1].shader = Shader.Find("ReadyPrefab/Shader3.14");
                            mats[1].SetVector("_TileOff", new Vector4(0.6f, 0.9f, 0.15f, 0.15f));
                            //set frame
                            Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frame)) as Material;
                            mats[0] = matframe;
                            Texture main = CardData.Instance.GetCardFrameTexture("MortalB_" + frame + "_" + /*hero.rarity*/1);
                            //mats[0].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                            mats[0].SetTexture("_MainTex", main);
                            Texture mask = CardData.Instance.GetCardFrameMaskTexture("MMortalB_" + /*hero.rarity*/1);
                            //mats[0].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                            mats[0].SetTexture("_FrameMask", mask);
                            mats[0].SetTexture("_FlareMask", flareMask);
                            boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials = mats;

                            //boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 1);
                            premiumEffects.GetChild((int)frame - 3).gameObject.SetActive(true);
                            premiumEffects.GetChild((int)frame - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
                        }
                        else
                        {
                            boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 0);
                            boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[1].SetTexture("_print_img", cardTexture);
                        }
                    }
                }    
                
            }
        }
        if (healthOnBoardText != null && damageOnBoardText != null)
        {
            healthOnBoardText.text = heroInfo.hp.ToString();
            damageOnBoardText.text = heroInfo.atk.ToString();
        }
    }

    private void OnMouseDown()
    {
        if (GameBattleScene.instance.skillState != SkillState.None)
        {
            if (GameBattleScene.instance.onBoardClone == null)
            {
                if (canSelect)
                    onAddToListSkill?.Invoke(this);
            }
            //else
            //    onRemoveFromListSkill?.Invoke(this);
        }
    }

    public void HighlightUnit()
    {
        if (outline != null)
            outline.gameObject.SetActive(true);
        canSelect = true;
    }

    public void UnHighlightUnit()
    {
        if (outline != null)
            outline.gameObject.SetActive(false);
        canSelect = false;
    }

    private void OnEndSkillActive()
    {
        UnHighlightUnit();
        onEndSkillActive?.Invoke(this);
    }

    public void SetPosition(Vector3 target, float blend)
    {
        transform.position = Vector3.Lerp(transform.position, target, 6 * Time.deltaTime);
        animator.SetFloat("Blend", blend);
    }
}
