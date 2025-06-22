//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using PathologicalGames;
//using UnityEngine.EventSystems;
//using GIKCore.Sound;
//using GIKCore.UI;
//using GIKCore.Utilities;
//using System.Linq;
//using GIKCore.Lang;

//public class DragAndDrop : MonoBehaviour
//{
//    //public bool isDragging;
//    //public LayerMask layerMask;
//    //[HideInInspector] public CardSlot currentCardSlot;
//    private CardSlot currentSelectedCardSlot;
//    private Card card;
//    //public GameObject cardCloneMinion, cardCloneSpell;
//    //long lastRow = -1, lastCol = -1, currentRow = -1, currentCol = -1;
//    //float currentClickTime = 0;
//    //float currentTime;
//    //public ICallback.CallFunc onClickInfo;
//    //private bool isTouch;
//    //private Cursor cursor;
//    //[HideInInspector] public GameObject newCardClone;

//    //private void Start()
//    //{
//    //    GameBattleScene.instance.onGameConfirmStartBattle += OnGameConfirmStartBattle;
//    //}

//    //private void OnEnable()
//    //{
//    //    card = GetComponent<Card>();
//    //}

//    //bool IsPointerOverUIObject()
//    //{
//    //    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
//    //    eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//    //    List<RaycastResult> results = new List<RaycastResult>();
//    //    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
//    //    return results.Count > 0;
//    //}

//    //void OnSpawned()
//    //{
//    //    card = GetComponent<Card>();
//    //    //if (card != null)
//    //    //{
//    //    //    card.isMulliganSelected = false;
//    //    //}
//    //}

//    //private void OnGameConfirmStartBattle()
//    //{
//    //    if (!isDragging)
//    //        return;

//    //    if (card.isSummoned)
//    //        card.MoveBack();
//    //    else
//    //        card.MoveBackToHand();
//    //    isDragging = false;
//    //    if (cursor != null)
//    //    {
//    //        cursor.gameObject.SetActive(false);
//    //        cursor = null;
//    //        GameBattleScene.instance.gameCursor = null;
//    //    }
//    //}

//    //public void ResetSlot()
//    //{
//    //    lastRow = -1;
//    //    lastCol = -1;
//    //    currentRow = -1;
//    //    currentCol = -1;
//    //}

//    //void OnDespawned()
//    //{
//    //    lastRow = -1;
//    //    lastCol = -1;
//    //    currentRow = -1;
//    //    currentCol = -1;
//    //}

//    //void OnMouseOver()
//    //{
//    //    if (card.cardOwner == CardOwner.Player)
//    //    {
//    //        if (IsPointerOverUIObject())
//    //            return;

//    //        if (!card.isSummoned)
//    //        {
//    //            if (!card.isInteracted && !isDragging && card.handObject != null)
//    //            {
//    //                card.handObject.transform.DOMove(card.transform.position - card.transform.forward * 0.5f, 0.05f);
//    //                card.Interested(true);
//    //                SoundHandler.main.PlaySFX("SelectCard", "sounds");
//    //            }
//    //        }
//    //    }
//    //}

//    //void OnMouseExit()
//    //{
//    //    if (card.cardOwner == CardOwner.Player)
//    //    {
//    //        if (!card.isSummoned)
//    //        {
//    //            if (card.isInteracted && !isDragging && card.handObject != null)
//    //            {
//    //                card.handObject.transform.DOLocalMove(Vector3.zero, 0.05f);
//    //                card.Interested(false);
//    //            }
//    //        }
//    //        else
//    //        {
//    //            UIManager.instance.ClosePreviewShortCard();
//    //        }
//    //        currentHoverTime = 0;
//    //    }
//    //}

//    private void OnMouseDown()
//    {
//        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC)
//        {
//            if (card.currentSlot != null && card.currentSlot.type != SlotType.Enemy)
//            {
//                if (card.isSummoned)
//                {
//                    return;
//                }
//            }
//        }

//        if (card.currentSlot != null && card.currentSlot.type == SlotType.Enemy)
//        {
//            if (GameBattleScene.instance.skillState == SkillState.TWO_ANY_ENEMIES)
//            {
//                if (/*card.canSelect && */card.skillState == CardSkillState.None)
//                {
//                    if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 2)
//                    {
//                        card.skillState = CardSkillState.TWO_ANY_ENEMIES;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                }
//                else
//                {
//                    card.skillState = CardSkillState.None;
//                    GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                    GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                }
//                GameBattleScene.instance.CheckAvailableSkillCondition();
//            }
//            if (GameBattleScene.instance.skillState == SkillState.ANY_ENEMY_UNIT)
//            {
//                if (card.skillState == CardSkillState.None)
//                {
//                    if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                    {
//                        card.skillState = CardSkillState.ANY_ENEMY_UNIT;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                }
//                else
//                {
//                    card.skillState = CardSkillState.None;
//                    GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                    GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                }
//            }
//            if (GameBattleScene.instance.skillState == SkillState.ANY_UNIT)
//            {
//                if (card.skillState == CardSkillState.None)
//                {
//                    if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                    {
//                        card.skillState = CardSkillState.ANY_UNIT;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                }
//                else
//                {
//                    card.skillState = CardSkillState.None;
//                    GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                    GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                }
//            }

//            if (GameBattleScene.instance.skillState == SkillState.ANY_TARGET)
//            {
//                if (card.skillState == CardSkillState.None)
//                {
//                    if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                    {
//                        card.skillState = CardSkillState.ANY_TARGET;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                }
//                else
//                {
//                    card.skillState = CardSkillState.None;
//                    GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                    GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                }
//            }
//            if (GameBattleScene.instance.skillState == SkillState.ANY_ALL_UNIT)
//            {
//                if (GameBattleScene.instance.selectedTower == null)
//                {
//                    if (card.skillState == CardSkillState.None)
//                    {
//                        if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                        {
//                            card.skillState = CardSkillState.ANY_ALL_UNIT;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform);
//                        }
//                        else
//                        {
//                            card.skillState = CardSkillState.None;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                        }
//                    }
//                }
//            }

//            if (GameBattleScene.instance.skillState == SkillState.ANY_MOTAL)
//            {
//                if (card.skillState == CardSkillState.None)
//                {
//                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
//                    {
//                        if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                        {
//                            card.skillState = CardSkillState.ANY_MOTAL;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform);
//                        }
//                        else
//                        {
//                            card.skillState = CardSkillState.None;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                        }
//                    }
//                }
//            }
//            if (GameBattleScene.instance.skillState == SkillState.ANY_COL_ENEMY)
//            {
//                if (card.skillState == CardSkillState.None)
//                {
//                    if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                    {
//                        card.skillState = CardSkillState.ANY_COL_ENEMY;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                }
//                else
//                {
//                    card.skillState = CardSkillState.None;
//                    GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                    GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                }
//            }
//            if (GameBattleScene.instance.skillState == SkillState.ANY_LANE_ENEMY)
//            {
//                if (card.canSelect)
//                {
//                    if (card.skillState == CardSkillState.None)
//                    {
//                        if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                        {
//                            card.skillState = CardSkillState.ANY_COL_ENEMY;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform);
//                        }
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                    }
//                }
//            }

//            return;
//        }
//        if (GameBattleScene.instance.skillState == SkillState.None)
//        {
//            if ((GameBattleScene.instance.isGameStarted || card.currentSlot != null) && GameBattleScene.instance.IsYourTurn)
//            {
//                isTouch = true;
//            }
//        }
//        else
//        {
//            switch (GameBattleScene.instance.skillState)
//            {
//                case SkillState.ANY_ALLY_UNIT:
//                    if (/*card.canSelect && */card.skillState == CardSkillState.None && card.isSummoned)
//                    {
//                        if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                        {
//                            card.skillState = CardSkillState.ANY_ALLY_UNIT;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform);
//                        }
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                    }
//                    break;
//                case SkillState.ANY_UNIT_BUT_SELF:
//                    if (/*card.canSelect && */card.skillState == CardSkillState.None && card.isSummoned)
//                    {
//                        if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
//                        {
//                            card.skillState = CardSkillState.ANY_UNIT_BUT_SELF;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform);
//                        }
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                    }
//                    break;
//                case SkillState.TWO_ANY_ALLIES:
//                    if (/*card.canSelect && */card.skillState == CardSkillState.None && card.isSummoned)
//                    {
//                        if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 2)
//                        {
//                            card.skillState = CardSkillState.TWO_ANY_ALLIES;
//                            GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                            GameBattleScene.instance.UpdateSpellTarget(transform);
//                        }
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                    }
//                    break;
//                case SkillState.MY_HAND_CARD:
//                    if (card.skillState == CardSkillState.None && !card.isSummoned)
//                    {
//                        card.skillState = CardSkillState.MY_HAND_CARD;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                        card.handObject.transform.DOMove(card.transform.position - card.transform.forward * 0.3f, 0.05f);
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                        card.handObject.transform.DOLocalMove(Vector3.zero, 0.05f);
//                    }
//                    break;
//                case SkillState.ANY_UNIT:
//                    if (card.skillState == CardSkillState.None && card.isSummoned)
//                    {
//                        card.skillState = CardSkillState.ANY_UNIT;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                    }
//                    break;

//                case SkillState.ANY_TARGET:
//                    if (card.skillState == CardSkillState.None && card.isSummoned)
//                    {
//                        card.skillState = CardSkillState.ANY_TARGET;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Add(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform);
//                    }
//                    else
//                    {
//                        card.skillState = CardSkillState.None;
//                        GameBattleScene.instance.lstSelectedSkillBoardCard.Remove(card);
//                        GameBattleScene.instance.UpdateSpellTarget(transform, true);
//                    }
//                    break;
//            }
//            GameBattleScene.instance.CheckAvailableSkillCondition();
//        }
//    }

//    private void OnMouseUp()
//    {
//        if (currentTime - currentClickTime < 0.2f)
//        {
//            onClickInfo?.Invoke();
//            return;
//        }

//        if (isTouch)
//        {
//            OnMouseUpAction();
//            isTouch = false;
//        }
//        else
//        {
//            Toast.Show(LangHandler.Get("tut-5", "YOU COULD NOT PLAY CARD IN OPPONENT'S TURN"));
//        }
//    }

//    private void OnMouseDrag()
//    {
//        if (GameBattleScene.instance.battleState == BATTLE_STATE.WAIT_COMFIRM)
//            return;

//        if (isTouch)
//        {
//            if ((GameBattleScene.instance.isGameStarted || card.currentSlot != null) && GameBattleScene.instance.IsYourTurn)
//            {
//                if (currentTime - currentClickTime < 0.2f)
//                {
//                    return;
//                }

//                if (!isDragging)
//                {
//                    BeginDrag();
//                    if (card.currentSlot != null)
//                    {
//                        lastRow = card.currentSlot.xPos;
//                        lastCol = card.currentSlot.yPos;
//                        currentSelectedCardSlot = card.currentSlot;
//                    }
//                    else
//                    {
//                        SoundHandler.main.PlaySFX("dealcard", "sounds");
//                    }
//                }

//            }

//        }
//    }

//    public void OnMouseUpAction()
//    {
//        if (isDragging)
//        {
//            if (GameBattleScene.instance.isGameStarted || card.currentSlot != null)
//            {
//                EndDrag();
//            }
//        }
//    }

//    //private void FixedUpdate()
//    //{
//    //    currentTime = Time.fixedTime;
//    //    if (isDragging)
//    //    {
//    //        RaycastHit hit;
//    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//    //        if (card.heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
//    //        {
//    //            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
//    //            {
//    //                if (hit.collider != null)
//    //                {
//    //                    if (hit.collider.GetComponent<CardSlot>() != null)
//    //                    {
//    //                        if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy)
//    //                        {
//    //                            CardSlot slot = hit.collider.GetComponent<CardSlot>();
//    //                            if (currentSelectedCardSlot == null)
//    //                            {
//    //                                currentSelectedCardSlot = slot;
//    //                                currentSelectedCardSlot.HighLightSlot();
//    //                            }
//    //                            else
//    //                            {
//    //                                if (slot != currentSelectedCardSlot)
//    //                                {
//    //                                    currentSelectedCardSlot.UnHighLightSlot();
//    //                                    currentSelectedCardSlot = slot;
//    //                                    currentSelectedCardSlot.HighLightSlot();
//    //                                }
//    //                            }
//    //                        }
//    //                    }
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    //public void BeginDrag()
//    //{
//    //    isDragging = true;

//    //    if (GameBattleScene.instance.gameCursor == null)
//    //    {
//    //        Transform trans = PoolManager.Pools["Card"].Spawn(GameBattleScene.instance.cursorObject);
//    //        GameBattleScene.instance.gameCursor = trans.GetComponent<Cursor>();
//    //    }
//    //    if (cursor == null)
//    //        cursor = GameBattleScene.instance.gameCursor;

//    //    cursor.gameObject.SetActive(true);
//    //    cursor.InitCursor(transform.position, card);
//    //    //?
//    //    if (card.cardState == CardState.OnBoard)
//    //    {
//    //        if (card.heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
//    //            card.Selected(true, card.currentSlot == null);
//    //    }

//    //    //if (isCheckingSkill)
//    //    //    return;
//    //    //if (card.heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
//    //    //    return;
//    //    //GameBattleScene.instance.CheckHeroSkill(0, card, -1, -1);
//    //    //isCheckingSkill = true;
//    //}

//    public void EndDrag()
//    {
//        isDragging = false;
//        if (currentSelectedCardSlot != null)
//        {
//            currentSelectedCardSlot.UnHighLightSlot();
//            currentSelectedCardSlot = null;
//        }
//        DropCard();
//        //if (card.heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
//        //    return;
//        //isCheckingSkill = false;
//    }

//    void DropCard()
//    {
//        cursor.gameObject.SetActive(false);
//        //cursor = null;
//        //GameBattleScene.instance.gameCursor = null;
//        RaycastHit hit;
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC)
//        {
//            if (!GameBattleScene.instance.Decks[0].GetDeckBound().IntersectRay(ray))
//            {
//                GameBattleScene.instance.SummonCardInBattlePhase(card, currentRow, currentCol);
//            }
//        }
//        else
//        {
//            if (!GameBattleScene.instance.Decks[0].GetDeckBound().IntersectRay(ray))
//            {
//                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
//                {
//                    if (hit.collider != null)
//                    {
//                        if (hit.collider.GetComponent<CardSlot>() != null)
//                        {
//                            if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy)
//                            {
//                                currentCardSlot = hit.collider.GetComponent<CardSlot>();
//                                currentRow = currentCardSlot.xPos;
//                                currentCol = currentCardSlot.yPos;

//                                if (!card.isTired)
//                                {
//                                    if (!GameBattleScene.instance.isGameStarted)
//                                    {
//                                        GameBattleScene.instance.MoveGodInReadyPhase(card, lastRow, lastCol, currentRow, currentCol);
//                                    }
//                                    // hand
//                                    else if (card.currentSlot == null)
//                                    {
//                                        GameBattleScene.instance.SummonCardInBattlePhase(card, currentRow, currentCol);
//                                    }
//                                    else if (card.currentSlot != null)
//                                    {
//                                        GameBattleScene.instance.MoveCardInBattlePhase(card, lastRow, lastCol, currentRow, currentCol);
//                                    }
//                                }
//                            }
//                        }
//                        else if (card.currentSlot == null)
//                        {
//                            card.MoveBackToHand();
//                        }
//                        else
//                        {
//                            card.MoveBack();
//                            return;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}