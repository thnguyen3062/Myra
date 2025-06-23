using DG.Tweening;
using GIKCore;
using GIKCore.Sound;
using GIKCore.Utilities;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialController instance;
    public long m_TutorialID ;
    [SerializeField] private GodCardHandler godCardHandler;
    [SerializeField] private GameObject cardPreview;
    [SerializeField] private CardPreviewInfo godCardPreview;
    [SerializeField] private CardPreviewInfo minionCardPreview;
    [SerializeField] private CardPreviewInfo spellCardPreview;
    [SerializeField] private GameObject cardPreviewShort;
    [SerializeField] private GameObject showCardPrefab;
    [SerializeField] private Transform showCard1, showCard2;
    [SerializeField] private CardPreviewInfo godCardPreviewShort;
    [SerializeField] private CardPreviewInfo minionCardPreviewShort;
    [SerializeField] private CardPreviewInfo spellCardPreviewShort;
    [SerializeField] private Sprite[] gamePhaseSprite;
    [SerializeField] private GameObject startupEnd;
    [SerializeField] private GameObject[] lstActionPhase;
    [SerializeField] private GameObject buttonSkip;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Highlight")]
    [SerializeField] private GameObject hlETower;
    [SerializeField] private GameObject hlPManas;
    [SerializeField] private GameObject hlPOneMana;
    [SerializeField] private GameObject hlShardButton;
    [SerializeField] public GameObject hlPass;
    [SerializeField] private GameObject hlLanePlayerRight;
    [SerializeField] private GameObject hlLanePlayerLeft;
    [SerializeField] private GameObject hlLaneLeft;
    [SerializeField] private GameObject hlLaneRight;
    [SerializeField] private GameObject hlColumn;
    [SerializeField] private GameObject[] m_GuidingArrowPlayer;
    [SerializeField] private GameObject[] m_GuidingArrowEnemy;

    [SerializeField] private GameObject m_SmokeEffect;
    public bool allowPass = false;
    public GameObject shardObject;
    public int index = 0;
    bool canPause = true;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        BattleSceneTutorial.instance.onGameDealCard += GameDealCard;
        BattleSceneTutorial.instance.onGameBattleSimulation += GameBattleSimulation;
        BattleSceneTutorial.instance.onGameChooseWayRequest += GameChooseWayRequest;
        BattleSceneTutorial.instance.onGameBattleChangeTurn += OnGameBattleChangeTurn;
        //update mana button orb
        ButtonOrbTutorial.instance.UpdateMana(0, 1, ManaState.StartTurn, 0);
        ButtonOrbTutorial.instance.UpdateMana(1, 1, ManaState.StartTurn, 0);
        if (m_TutorialID == 0)
            BattleSceneTutorial.instance.lstTowerInBattle.ForEach(t => t.UpdateHealth(7));
        else
            BattleSceneTutorial.instance.lstTowerInBattle.ForEach(t => t.UpdateHealth(7));
        foreach (GameObject go in m_GuidingArrowEnemy)
            go.SetActive(false);
        foreach(GameObject go in m_GuidingArrowPlayer)
            go.SetActive(false);
        godCardHandler.gameObject.SetActive(false);
        StartCoroutine(PlayTutorial());
    }

    private float timeToPause = 10f;
    bool counting = false;
    public void PassTut(int ind)
    {
        if(ind == 0)
        {
            BattleSceneTutorial.instance.RewardTut1(); 
            //update progression 
            Game.main.socket.UpdateProgression(0);

            GameData.main.userProgressionState = 1;
            if (!PlayerPrefs.HasKey("TRACK_END_TUT1"))
            {
                PlayerPrefs.SetInt("TRACK_END_TUT1", 1);
                ITrackingParameter pr = new ITrackingParameter() { name = "finish_tutorial_1", value = "true" };
                ITracking.LogEventFirebase(ITracking.TRACK_END_TUT1, pr);
            }
        }  
        else
        {
            BattleSceneTutorial.instance.RewardTut2();
            Game.main.socket.UpdateProgression(5);
            GameData.main.userProgressionState = 6;
            if (!PlayerPrefs.HasKey("TRACK_END_TUT2"))
            {
                PlayerPrefs.SetInt("TRACK_END_TUT2", 1);
                ITrackingParameter pr = new ITrackingParameter() { name = "finish_tutorial_2", value = "true" };
                ITracking.LogEventFirebase(ITracking.TRACK_END_TUT2, pr);
            }
        }    
    }    
    private IEnumerator PlayTutorial()
    {
        if (m_TutorialID == 0)
        {
            BattleSceneTutorial.instance.Phase00();

            // voice ares
            yield return new WaitForSeconds(1f);
            tutorialPanel.SetActive(true);
            lstActionPhase[0].SetActive(true);
            SoundHandler.main.PlaySFX("Tut1_Ares_1", "soundtutorial");
            yield return new WaitForSeconds(1f);
            buttonSkip.SetActive(true);
        }
        else if (m_TutorialID == 1)
        {
            yield return new WaitForSeconds(0f);
            lstActionPhase[0].SetActive(true);
            buttonSkip.SetActive(true);
        }
    }
    public void HideTutBox()
    {
        if (lstActionPhase[index].activeInHierarchy)
        {
            tutorialPanel.SetActive(false);
            lstActionPhase[index].SetActive(false);
        }
    }
    private IEnumerator AllowSkip(float wait, bool openPanelTutorial, int index, bool allowSkip = true, float waitDisplay = 0f, ICallback.CallFunc callback = null)
    {
        yield return new WaitForSeconds(waitDisplay);
        tutorialPanel.SetActive(openPanelTutorial);
        lstActionPhase[index].SetActive(true);
        callback?.Invoke();
        yield return new WaitForSeconds(wait);
        buttonSkip.SetActive(allowSkip);

    }
    public void Skip()
    {
        if (m_TutorialID == 0)
        {
            tutorialPanel.SetActive(false);
            buttonSkip.SetActive(false);

            switch (index)
            {
                case 0:
                    {
                        BattleSceneTutorial.instance.Phase01();
                        m_GuidingArrowPlayer[1].SetActive(true);
                        break;
                    }
                case 2:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.battleID == 2)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        BattleSceneTutorial.instance.Phase11();
                        break;
                    }
                case 3:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.battleID == 2)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        BattleSceneTutorial.instance.Phase12();
                        break;
                    }

                case 6:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.battleID == 1)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        break;
                    }
                case 7:
                    {
                        // ?n m?i tên 
                        break;
                    }
                case 9:
                    {
                        BattleSceneTutorial.instance.Phase13();
                        break;
                    }
                case 10:
                    {
                        BattleSceneTutorial.instance.Phase21();
                        break;
                    }
                case 12:
                    {
                        foreach (CardSlot slot in BattleSceneTutorial.instance.playerSlotContainer)
                            slot.UnHighLightSlot();

                        m_GuidingArrowEnemy[0].SetActive(false);
                        break;
                    }
                case 14:
                    {
                        break;
                    }
                case 15:
                    {
                        BattleSceneTutorial.instance.Phase22();
                        break;
                    }
                case 16:
                    {
                        BattleSceneTutorial.instance.Phase31();
                        break;
                    }
                case 17:
                    {
                        BattleSceneTutorial.instance.Phase32();
                        break;
                    }
                case 18:
                    {
                        BattleSceneTutorial.instance.Phase33();
                        break;
                    }
                case 19:
                    {
                        foreach (HandCard card in BattleSceneTutorial.instance.Decks[0].GetListCard)
                        {
                            card.ScaleCard(false);
                        }
                        break;
                    }
                case 21:
                    {
                        BattleSceneTutorial.instance.Phase34();
                        //update progression 
                        Game.main.socket.UpdateProgression(0);

                        GameData.main.userProgressionState = 1;
                        if (!PlayerPrefs.HasKey("TRACK_END_TUT1"))
                        {
                            PlayerPrefs.SetInt("TRACK_END_TUT1", 1);
                            ITrackingParameter pr = new ITrackingParameter() { name = "finish_tutorial_1", value = "true" };
                            ITracking.LogEventFirebase(ITracking.TRACK_END_TUT1, pr);
                        }
                        break;
                    }



            }

            foreach (GameObject phase in lstActionPhase)
                phase.SetActive(false);
            index++;
            switch (index)
            {
                case 1:
                    {
                        //2
                        StartCoroutine(AllowSkip(2f, true, index));
                        break;
                    }
                case 2:
                    {
                        BattleSceneTutorial.instance.Phase04();
                        m_GuidingArrowEnemy[0].SetActive(true);
                        StartCoroutine(AllowSkip(2.5f, false, index,true,1.5f));
                        break;
                    }
                case 3:
                    {
                        StartCoroutine(DelayCallback(9f, () =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                                if (card.heroID == 10008)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);
                                    //voice this wwill be fun
                                }
                            SoundHandler.main.PlaySFX("Tut1_Athena_2", "soundtutorial");
                        }));
                        StartCoroutine(AllowSkip(2f, false, index, true, 9f));

                        break;
                    }
                case 4:
                    {

                        //voice : check thest out 
                        //atk
                        StartCoroutine(AllowSkip(2f, false, index, true, 3f, () =>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Check", "soundtutorial");
                        }));
                        break;
                    }
                case 5:
                    {
                        //hp
                        StartCoroutine(AllowSkip(2f, false, index));
                        break;
                    }
                case 6:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.HighlightUnit();
                                card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);
                                //voice :We need more cards to attack enemy base.
                                SoundHandler.main.PlaySFX("Tut1_Ares_2", "soundtutorial");

                            }

                        StartCoroutine(AllowSkip(1f, false, index));
                        break;
                    }
                case 7:
                    {
                        // voice drag to summon 
                        SoundHandler.main.PlaySFX("Tut1_Drag", "soundtutorial");
                        // mui ten drag card
                        HandCard card = BattleSceneTutorial.instance.Decks[0].GetListCard.FirstOrDefault(x => x.heroID == 107);
                        if (card != null)
                            card.HighlighCard();
                        CardSlot slot = BattleSceneTutorial.instance.playerSlotContainer.FirstOrDefault(x => x.xPos == 0 && x.yPos == 1);
                        slot.HighLightSlot();
                        StartCoroutine(AllowSkip(.5f, false, index, false));
                        break;
                    }
                case 8:
                    {
                        // voice 
                        StartCoroutine(AllowSkip(2f, false, index,true,0,() =>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Ares_3", "soundtutorial");
                        }));
                        break;
                    }
                case 9:
                    {
                        //end round 1 voice ?

                        StartCoroutine(AllowSkip(0f, false, index, false));
                        break;
                    }
                case 10:
                    {
                        //befor end combat
                        StartCoroutine(AllowSkip(2f, false, index, true, 8f));
                        break;
                    }
                case 11:
                    {
                        //round2 
                        StartCoroutine(AllowSkip(2f, false, index, true, 5.5f,()=>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Ares_4", "soundtutorial");
                        }));
                        //
                        break;
                    }
                case 12:
                    {
                        // drag to summon poseidon's soldier
                        HandCard card = BattleSceneTutorial.instance.Decks[0].GetListCard.FirstOrDefault(x => x.heroID == 107);
                        if (card != null)
                            card.HighlighCard();
                        foreach (CardSlot slot in BattleSceneTutorial.instance.playerSlotContainer)
                            if (slot.yPos == 0)
                                slot.HighLightSlot();
                        StartCoroutine(AllowSkip(2f, false, index, false));
                        break;
                    }
                case 13:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true,1f,()=>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Ares_5", "soundtutorial");
                        }));
                        break;
                    }
                case 14:
                    {
                        //endturn
                        StartCoroutine(AllowSkip(0f, false, index, false, 0f));
                        break;
                    }
                case 15:
                    {
                        //enemy turn
                        StartCoroutine(AllowSkip(2f, false, index, true, 2.5f, () =>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Athena_3", "soundtutorial");
                        }));
                        break;
                    }
                case 16:
                    {
                        // berfore  round 3
                        StartCoroutine(AllowSkip(2f, false, index, true, 14.5f,()=>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Ares_6", "soundtutorial");
                        }));
                        break;
                    }
                case 17:
                    {
                        //round3 
                        StartCoroutine(AllowSkip(2f, false, index, true, 8.5f,()=>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Athena_4", "soundtutorial");
                        }));

                        break;
                    }
                case 18:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true, 4f,()=>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Ares_7", "soundtutorial");
                        }));
                        break;
                    }
                case 19:
                    {
                        // can summon quan siege
                           
                        StartCoroutine(AllowSkip(2f, false, index, true, 4f,() =>
                        {
                            SoundHandler.main.PlaySFX("Tut1_Ares_9", "soundtutorial");
                            HandCard card = BattleSceneTutorial.instance.Decks[0].GetListCard.FirstOrDefault(x => x.heroID == 116);
                            if (card != null)
                            {
                                card.HighlighCard();
                                card.ScaleCard(true);
                            }
                        }));
                        break;
                    }
                case 20:
                    {
                        HandCard card = BattleSceneTutorial.instance.Decks[0].GetListCard.FirstOrDefault(x => x.heroID == 116);
                        if (card != null)
                        {
                            card.HighlighCard();
                        }
                        StartCoroutine(AllowSkip(1f, false, index, false));
                        break;
                    }
                case 21:
                    {
                        // voice ares "great move"
                        SoundHandler.main.PlaySFX("Tut1_Ares_8", "soundtutorial");
                        StartCoroutine(AllowSkip(1f, false, index, true));
                        break;
                    }
            }
        }
        else if (m_TutorialID == 1)
        {
            tutorialPanel.SetActive(false);
            buttonSkip.SetActive(false);
            switch (index)
            {
                case 0:
                    {
                        BattleSceneTutorial.instance.Phase00Tut2();
                        break;
                    }
                case 1:
                    {
                        //glow tru + tween mau tru 7-16
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.heroID == 10008)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        BattleSceneTutorial.instance.lstTowerInBattle[1].TweenHP(16, 1.5f, () =>
                          {
                              BattleSceneTutorial.instance.lstTowerInBattle[0].TweenHP(16, 1.5f, null);
                          });

                        break;
                    }
                case 2:
                    {
                        // m? effect che lane ph?i. callback start round 1 
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        BattleSceneTutorial.instance.Phase11Tut2();
                        break;
                    }

                case 4:
                    {
                        //? t?t popup 2 l?n
                        ClosePreviewHandCard();
                        break;
                    }
                case 6:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 107)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);
                            }
                        break;
                    }
                case 7:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);

                            }
                        ClosePreviewHandCard();
                        break;
                    }
                case 8:
                    {  
                        BattleSceneTutorial.instance.Phase12Tut2();
                        break;
                    }
                case 9:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.heroID == 10008)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);

                            }
                        BattleSceneTutorial.instance.Phase13Tut2();
                        break;
                    }
                case 11:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);

                            }
                        BattleSceneTutorial.instance.Phase21Tut2();
                        break;
                    }

                case 12:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.heroID == 10008)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);

                            }
                        BattleSceneTutorial.instance.Phase22Tut2();
                        break;
                    }
                case 13:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        break;
                    }
                case 16:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        break;
                    }
                case 17:
                    {
                        BattleSceneTutorial.instance.Phase23Tut2();
                        break;
                    }
                case 19:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        break;
                    }
                case 22:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.heroID == 10008)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);

                            }
                        BattleSceneTutorial.instance.Phase31Tut2();
                        break;
                    }
                case 23:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        BattleSceneTutorial.instance.Phase32Tut2();
                        break;
                    }
                case 24:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.heroID == 10008)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);

                            }
                        // summon 2 quân 10010 vs 10005
                        BattleSceneTutorial.instance.Phase41Tut2();
                        break;
                    }
                case 25:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.UnHighlightUnit();
                                card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);
                            }
                        break;
                    }
                case 26:
                    {
                        foreach (HandCard card in BattleSceneTutorial.instance.Decks[0].GetListCard)
                        {
                            card.ScaleCard(false);
                        }
                        break;
                    }
                case 28:
                    {
                        BattleSceneTutorial.instance.Phase42Tut2();
                        Game.main.socket.UpdateProgression(5);
                        GameData.main.userProgressionState = 6;
                        if (!PlayerPrefs.HasKey("TRACK_END_TUT2"))
                        {
                            PlayerPrefs.SetInt("TRACK_END_TUT2", 1);
                            ITrackingParameter pr = new ITrackingParameter() { name = "finish_tutorial_2", value = "true" };
                            ITracking.LogEventFirebase(ITracking.TRACK_END_TUT2, pr);
                        }
                        break;
                    }
                
            }

            foreach (GameObject phase in lstActionPhase)
                phase.SetActive(false);
            lstActionPhase[index].SetActive(false);
            index++;
            switch (index)
            {
                case 1:
                    {
                        StartCoroutine(DelayCallback(4f, () =>
                        {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                            if (card.heroID == 10008)
                            {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);
                                //voice :Ready to clash? Face me with full 16 health
                                SoundHandler.main.PlaySFX("Tut2_Athena_1", "soundtutorial");

                            }
                        }));
                        StartCoroutine(AllowSkip(2f, false, index, true, 4f));

                        break;
                    }
                case 2:
                    {
                        m_SmokeEffect.GetComponent<MeshRenderer>().material.DOFade(0,5f).onComplete += delegate { m_SmokeEffect.SetActive(false); };
                        StartCoroutine(DelayCallback(7f, () =>
                         {
                             foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                 if (card.heroID == 10000)
                                 {
                                     card.HighlightUnit();
                                     card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);
                                     //voice : A new path! I wonder if we can play cards there.
                                     SoundHandler.main.PlaySFX("Tut2_Ares_1", "soundtutorial");
                                     // chay anim smoke fade out  (smoke de trong boardobject)
                                     
                                     //m_SmokeEffect.gameObject.SetActive(false);
                                 }
                         }));
                        StartCoroutine(AllowSkip(2f, false, index, true, 7f));
                        break;
                    }

                case 3:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true, 4.5f));
                        break;
                    }
                case 4:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true, 0f));
                        DBHero hero = new DBHero();
                        hero.id = 107;
                        hero.type = 1;
                        hero.color = 0;
                        hero.name = "Poseidon's Soldier";
                        hero.atk = 2;
                        hero.hp = 1;
                        hero.mana = 1;
                        hero.rarity = 1;
                        hero.speciesId = 1;
                        hero.speciesName = "Aquatic";
                        ShowPreviewHandCard(null, hero, 1);
                        break;
                    }
                case 5:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, false, 0));
                        HandCard card = BattleSceneTutorial.instance.Decks[0].GetListCard.FirstOrDefault(x => x.heroID == 107);
                        if (card != null)
                            card.HighlighCard();
                        CardSlot slot = BattleSceneTutorial.instance.playerSlotContainer.FirstOrDefault(x => x.xPos == 1 && x.yPos == 0);
                        break;
                    }
                case 6:
                    {
                        //  voice : Thanks to your upgrade, I have 2 attack now.
                        StartCoroutine(AllowSkip(2f, false, index, true, 2f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 107)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);
                                    TurnOnOffGuidingArrow(true, 0, true);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Soldier", "soundtutorial");
                        }));
                        break;
                    }
                case 7:
                    {
                        //voice : By the way, I have a special effect when I kill a card.
                        StartCoroutine(AllowSkip(6.5f, false, index, true, 1f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 10000)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Ares_2", "soundtutorial");

                        }));
                        StartCoroutine(DelayCallback(5.5f, () =>
                         {
                             BoardCard card = BattleSceneTutorial.instance.GetListPlayerCardInBattle().FirstOrDefault(x => x.heroID == 10000);
                             ShowPreviewHandCard(card, card.heroInfoTmp, 1);
                             TurnOffPhase();
                         }));
                        break;
                    }
                case 8:
                    {
                        //pass turn 1
                        StartCoroutine(AllowSkip(0f, false, index, false, 0f));
                        
                        break;
                    }
                case 9:
                    {
                        // voice delay 2f :I drew a Supreme Leader card on my hand. Your doom is near, hehe.
                        StartCoroutine(AllowSkip(2f, false, index, true, 7f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                                if (card.heroID == 10008)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Athena_2", "soundtutorial");
                        }));
                        break;
                    }
                case 10:
                    {
                        // round 2
                        StartCoroutine(AllowSkip(2f, false, index, true, 15f));
                        break;
                    }
                case 11:
                    {
                        // voice ares
                        StartCoroutine(AllowSkip(2f, false, index, true, 1.5f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 10000)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Ares_2_2", "soundtutorial");
                        }));
                        break;
                    }
                case 12:
                    {
                        StartCoroutine(AllowSkip(1.5f, false, index, true, 1f, () =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                                if (card.heroID == 10008)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);
                                    TurnOnOffGuidingArrow(true, 0, false);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Athena_3", "soundtutorial");
                        }));
                        break;
                    }

                case 13:
                    {
                        //voice ares delay 1s
                        StartCoroutine(AllowSkip(1.5f, false, index, true, 4f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 10000)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Ares_3", "soundtutorial");

                        }));
                        break;
                    }
                case 14:
                    {
                        foreach (CardSlot slot in BattleSceneTutorial.instance.playerSlotContainer)
                            if (slot.yPos==2)
                            {
                                slot.HighLightSlot();
                                //voice : A new path! I wonder if we can play cards there.

                            }
                        StartCoroutine(AllowSkip(.5f, false, index, true, 0f));
                        break;
                    }

                case 15:
                    {
                        //drag to summon hero 30 
                        foreach (HandCard card in BattleSceneTutorial.instance.Decks[0].GetListCard)
                            card.HighlighCard();
                        StartCoroutine(AllowSkip(.5f, false, index, false));
                        break;
                    }
                case 16:
                    {
                        //voice tut2 ares 4

                        StartCoroutine(AllowSkip(2f, false, index, true,3f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 10000)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Ares_4", "soundtutorial");
                        }));
                        break;
                    }
                case 17:
                    {
                        //pass turn 2
                        StartCoroutine(AllowSkip(0f, false, index, false, 0f));
                        break;
                    }
                case 18:
                    {
                        StartCoroutine(AllowSkip(0f, false, index, false,14f));
                        StartCoroutine(DelayCallback(14f, () =>
                        {
                            //highlight c? 2 lá bài trên tay 
                            foreach (HandCard card in BattleSceneTutorial.instance.Decks[0].GetListCard)
                                card.HighlighCard();
                            foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                if (slot.yPos == 2)
                                {
                                    slot.HighLightSlot();
                                }
                        }));
                        break;
                    }
                case 19:
                    {
                        foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                            if (card.heroID == 10000)
                            {
                                card.HighlightUnit();
                                card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                            }
                        SoundHandler.main.PlaySFX("Tut2_Ares_5", "soundtutorial");
                        StartCoroutine(AllowSkip(2f, false, index, true, 0f));
                        break;
                    }
                case 20:
                    {
                        //drag 2nd card 
                        StartCoroutine(AllowSkip(.5f, false, index, false, 0f));
                        foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                            if (slot.yPos == 2)
                            {
                                slot.HighLightSlot();

                            }
                        break;
                    }
                case 21:
                    {
                        //pass turn 
                        StartCoroutine(AllowSkip(0f, false, index, false, 0f));
                        break;
                    }
                case 22:
                    {
                        StartCoroutine(AllowSkip(2, false, index, true, 3.5f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                                if (card.heroID == 10008)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Athena_4", "soundtutorial");
                        }));
                        break;
                    }
                case 23:
                    {
                        StartCoroutine(AllowSkip(2.5f, false, index, true, 5f ,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 10000)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Ares_5_2", "soundtutorial");
                        }));
                        break;
                    }
                case 24:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true, 18f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                                if (card.heroID == 10008)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Athena_5", "soundtutorial");
                        }));
                        break;
                    }
                case 25:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true, 6f,() =>
                        {
                            foreach (BoardCard card in BattleSceneTutorial.instance.GetListPlayerCardInBattle())
                                if (card.heroID == 10000)
                                {
                                    card.HighlightUnit();
                                    card.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

                                }
                            SoundHandler.main.PlaySFX("Tut2_Ares_6", "soundtutorial");
                        }));
                        break;
                    }
                case 26:
                    {
                        StartCoroutine(AllowSkip(2f, false, index, true, 1f, () =>
                        {
                            foreach (HandCard card in BattleSceneTutorial.instance.Decks[0].GetListCard)
                            { 
                                card.HighlighCard();
                                card.ScaleCard(true);
                            }
                            SoundHandler.main.PlaySFX("Tut2_Murder_1", "soundtutorial");
                        }));
                        break;
                    }
                case 27:
                    {
                        foreach (HandCard card in BattleSceneTutorial.instance.Decks[0].GetListCard)
                            card.HighlighCard();
                        StartCoroutine(AllowSkip(1f, false, index, false, 0f));
                        break;
                    }
                case 28:
                    {
                        //pass turn:
                        StartCoroutine(AllowSkip(0f, false, index, false, 0f));
                        break;
                    }


            }
        }
    }
    public void TurnOnOffGuidingArrow(bool isPlayer, int pos, bool isActive)
    {
        if (isPlayer)
            m_GuidingArrowPlayer[pos].SetActive(isActive);
        else
            m_GuidingArrowEnemy[pos].SetActive(isActive);
    }
    IEnumerator DelayCallback(float waitTime, ICallback.CallFunc callback)
    {
        yield return new WaitForSeconds(waitTime);
        if (callback != null)
        {
            callback?.Invoke();
        }
    }

    public void TurnOffPhase()
    {
        foreach (GameObject tut in lstActionPhase)
        {
            tut.SetActive(false);
        }
    }
    IEnumerator Pause(float wait)
    {
        if (canPause)
        {
            yield return new WaitForSeconds(wait);
            if (index == 17 || index == 18 || index == 19)
            {
                if (index == 17)
                {
                    foreach (BoardCard card in BattleSceneTutorial.instance.GetListEnemyCardInBattle())
                        if (card.heroID == 10006)
                        {
                            card.HighlightUnit();
                        }
                }
                Time.timeScale = 0;
            }
        }

    }
    public void GameDealCard(List<long> godBattleID1, List<DBHero> godHero1, List<long> frame1, List<long> atk1, List<long> hp1, List<long> mana1, List<long> godBattleID2, List<DBHero> godHero2, List<long> frame2, List<long> atk2, List<long> hp2, List<long> mana2)
    {
        godCardHandler.InitGodUI(godBattleID1, godHero1, frame1,atk1,hp1,mana1, godBattleID2, godHero2, frame2, atk2, hp2, mana2);
        var groupGodPlayer = godHero1.GroupBy(g => g.heroNumber).Select(grp => grp.ToList()).ToList();
        groupGodPlayer.ForEach(x =>
        {
            GameObject go = Instantiate(showCardPrefab, Vector3.zero, Quaternion.identity, showCard1);
            ShowCardUI card = go.GetComponent<ShowCardUI>();
            card.InitData(x[0].heroNumber);
        });
        var groupGodEnemy = godHero2.GroupBy(g => g.heroNumber).Select(grp => grp.ToList()).ToList();
        groupGodEnemy.ForEach(x =>
        {
            GameObject go = Instantiate(showCardPrefab, Vector3.zero, Quaternion.identity, showCard2);
            ShowCardUI card = go.GetComponent<ShowCardUI>();
            card.InitData(x[0].heroNumber);
        });

    }
    private void GameBattleSimulation(bool isPlayer, string username, long roundCount)
    {
        //SetStartup(isPlayer, roundCount);
        // StartCoroutine(CloseStartupText());
    }
    public void ShowPreviewHandCard(Card card, DBHero hero, long frame, ICallback.CallFunc complete = null)
    {
        if (cardPreview.activeSelf)
            return;
        cardPreview.SetActive(true);
        if (hero.type == DBHero.TYPE_GOD)
        {
            godCardPreview.gameObject.SetActive(true);
            godCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
        {
            spellCardPreview.gameObject.SetActive(true);
            spellCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            minionCardPreview.gameObject.SetActive(true);
            minionCardPreview.SetCardPreview(hero, frame, true);
        }

        complete?.Invoke();
    }
    public void ClosePreviewHandCard()
    {
        if (cardPreview.gameObject.activeSelf)
        {
            godCardPreview.gameObject.SetActive(false);
            minionCardPreview.gameObject.SetActive(false);
            cardPreview.gameObject.SetActive(false);
            spellCardPreview.gameObject.SetActive(false);
        }
    }
    public void ShowPreviewShortCard(Card card, long id, long frame)
    {
        if (cardPreviewShort.activeSelf)
            return;

        cardPreviewShort.SetActive(true);
        DBHero hero = Database.GetHero(id);
        if (hero.type == DBHero.TYPE_GOD)
        {
            godCardPreviewShort.gameObject.SetActive(true);
            godCardPreviewShort.SetCardPreview(hero, frame, false);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
        {
            spellCardPreviewShort.gameObject.SetActive(true);
            spellCardPreviewShort.SetCardPreview(hero, frame, false);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            minionCardPreviewShort.gameObject.SetActive(true);
            minionCardPreviewShort.SetCardPreview(hero, frame, false);
        }
    }
    public void ClosePreviewShortCard()
    {
        if (cardPreviewShort.gameObject.activeSelf)
        {
            godCardPreviewShort.gameObject.SetActive(false);
            minionCardPreviewShort.gameObject.SetActive(false);
            cardPreviewShort.gameObject.SetActive(false);
            spellCardPreviewShort.gameObject.SetActive(false);
        }
    }
    //chuyen turn
    public void SetStartup(bool isPlayer, long roundCount)
    {
        startupEnd.gameObject.SetActive(true);
        if (isPlayer)
            startupEnd.GetComponent<Image>().sprite = (roundCount == 0) ? gamePhaseSprite[0] : gamePhaseSprite[2];
        else
            startupEnd.GetComponent<Image>().sprite = (roundCount == 0) ? gamePhaseSprite[3] : gamePhaseSprite[1];
    }
    private void OnGameBattleChangeTurn(long index)
    {
        if (index == -1)
        {
            //startupEnd.gameObject.SetActive(true);
            //startupEnd.GetComponent<Image>().sprite = gamePhaseSprite[5];
        }
    }
    private void GameChooseWayRequest()
    {
        //startupEnd.gameObject.SetActive(true);
        //startupEnd.GetComponent<Image>().sprite = gamePhaseSprite[4];
        //StartCoroutine(CloseStartupText());
    }
    public IEnumerator CloseStartupText()
    {
        yield return new WaitForSeconds(2);
        startupEnd.gameObject.SetActive(false);
    }

}
