using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GodInfoHandler : MonoBehaviour, IPointerClickHandler
{
    #region Serialized Fields
    [SerializeField] private Button activeSkillLButton;
    [SerializeField] private Button activeSkillRButton;
    [SerializeField] private Button activeSkillUButton;
    [SerializeField] private Image skillLImage;
    [SerializeField] private Image skillRImage;
    [SerializeField] private Image skillUImage;
    [SerializeField] private Image godImage;
    [SerializeField] private Image godColor;
    #endregion
    private DBHeroSkill skillL;
    private DBHeroSkill skillR;
    private DBHeroSkill skillU;
    private Card currentCard;

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.ShowPreviewHandCard(currentCard, currentCard.heroInfoTmp, currentCard.frameC);
    }

    public void SetGodInfoData(Card card)
    {
        if (currentCard == null)
            currentCard = card;
        else
            if (currentCard == card)
                return;
        else
            currentCard = card;

        godImage.sprite = CardData.Instance.GetOnBoardSprite(card.heroInfo.id);
        godColor.sprite = CardData.Instance.GetCardColorSprite("Border_" + card.heroInfo.color);
        activeSkillUButton.gameObject.SetActive(false);
        activeSkillLButton.gameObject.SetActive(false);
        activeSkillRButton.gameObject.SetActive(false);
        card.lstSkill.ForEach(x =>
        {
            if (x.skill_type == 1)
            {
                if (x.isUltiType)
                {
                    skillU = x;
                    activeSkillUButton.gameObject.SetActive(true);
                    Sprite ultiSprite = CardData.Instance.GetUltiSprite(x.id.ToString());
                    if (ultiSprite != null)
                        skillUImage.sprite = ultiSprite;
                }
                else
                {
                    if (CardData.Instance.GetCardSkillDataInfo(card.heroInfo.id).skillIds.Contains(x.id))
                    {
                        if (!activeSkillLButton.gameObject.activeSelf)
                        {
                            skillL = x;
                            activeSkillLButton.gameObject.SetActive(true);
                            Sprite activeSkillSprite = CardData.Instance.GetUltiSprite(x.id.ToString());
                            if (activeSkillSprite != null)
                                skillLImage.sprite = activeSkillSprite;
                        }
                        else
                        {
                            skillR = x;
                            activeSkillRButton.gameObject.SetActive(true);
                            Sprite activeSkillSprite = CardData.Instance.GetUltiSprite(x.id.ToString());
                            if (activeSkillSprite != null)
                                skillRImage.sprite = activeSkillSprite;
                        }
                    }
                }
            }
            //else
            //{
            //    if (CardData.Instance.GetCardSkillDataInfo(card.heroInfo.id).skillIds.Contains(x.id))
            //    {
            //        if (activeSkillLButton.gameObject.activeSelf)
            //        {
            //            skillR = x;
            //            activeSkillRButton.gameObject.SetActive(true);
            //            Sprite activeSkillSprite = CardData.Instance.GetUltiSprite(x.id.ToString());
            //            if (activeSkillSprite != null)
            //                skillRImage.sprite = activeSkillSprite;
            //        }
            //        else
            //        {
            //            skillL = x;
            //            activeSkillLButton.gameObject.SetActive(true);
            //            Sprite activeSkillSprite = CardData.Instance.GetUltiSprite(x.id.ToString());
            //            if (activeSkillSprite != null)
            //                skillLImage.sprite = activeSkillSprite;
            //        }
            //    }
            //}
        });
        activeSkillLButton.onClick.AddListener(delegate
        {
            card.OnActiveSkill(skillL);
        });
        activeSkillRButton.onClick.AddListener(delegate
        {
            card.OnActiveSkill(skillR);
        });
        activeSkillUButton.onClick.AddListener(delegate
        {
            card.OnActiveSkill(skillU);
            
        });
    }

    
}
