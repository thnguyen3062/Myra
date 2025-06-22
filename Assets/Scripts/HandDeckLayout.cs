using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum HandDeckStatus
{
    Normal,
    Expand
}

public class HandDeckLayout : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int indexDeck;
    [SerializeField] private float boundSizeZ = 2;
    [SerializeField] private float boundSizeY = 1;
    [SerializeField] private MeshRenderer handCardMesh;
    [SerializeField] private Transform normalPosition;
    [SerializeField] private Transform expandPosition;
    #endregion

    private List<HandCard> lstCard = new List<HandCard>();
    public HandDeckStatus status;
    protected Bounds bounds;
    private bool canRebuildDeck = false;

    #region Unity Methods
    private void Start()
    {
        float sizeX = 8;
        
        Vector3 center = transform.position;
        Vector3 size = new Vector3(sizeX, boundSizeY, boundSizeZ);
        bounds.size = size;
        canRebuildDeck = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
    #endregion

    public Bounds GetDeckBound()
    {
        return bounds;
    }

    public List<HandCard> GetListCard
    {
        get
        {
            return lstCard;
        }
    }

    public void AddNewCard(HandCard card)
    {
        lstCard.Add(card);
    }

    public void RemoveCard(HandCard card, int index = 0)
    {
        lstCard.Remove(card);
        ReBuildDeck(index);
    }

    //#if UNITY_ANDROID || UNITY_IOS
    //    private void Update()
    //    {

    //        if (!canRebuildDeck)
    //            return;

    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            RaycastHit hit;
    //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //            if (GetDeckBound().IntersectRay(ray))
    //            {
    //                if (Physics.Raycast(ray, out hit))
    //                {
    //                    if (hit.collider.GetComponent<Card>() == null)
    //                    {
    //                        if (status == HandDeckStatus.Normal)
    //                        {
    //                            status = HandDeckStatus.Expand;
    //                        }
    //                        else
    //                        {
    //                            status = HandDeckStatus.Normal;
    //                        }
    //                        ReBuildDeck();
    //                    }
    //                }
    //            }
    //        }
    //    }
    //#endif

    public void ReBuildDeck(int index = 0, ICallback.CallFunc callback = null)
    {
        //#if UNITY_ANDROID || UNITY_IOS
        //        float[] layout = BuildLayoutCard(lstCard.Count);
        //        float xTo = layout[0];
        //        float wCard = layout[1];
        //        float hCard = layout[2];
        //        float overlap = layout[3];
        //        int currentIndex = lstCard.Count-1;
        //        //for (int i = lstCard.Count - 1; i >= 0; i--)
        //        //{
        //        //    Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.9f : xTo, index == 0 ? expandPosition.position.y + 0.003f * i : transform.position.y, index == 0 ? expandPosition.position.z + 0.001f * i : transform.position.z);
        //        //    Debug.Log(lstCard[i].gameObject.name + "________________");
        //        //    lstCard[i].MoveTo(newPosition, 0.2f, () =>
        //        //    {
        //        //        Debug.Log(lstCard[i].gameObject.name);
        //        //        lstCard[i].UpdatePosition();
        //        //        //if (i == 0)
        //        //        //    canRebuildDeck = true;
        //        //    });

        //        //    if (index == 0)
        //        //        lstCard[i].transform.rotation = transform.rotation;
        //        //    else
        //        //        lstCard[i].transform.rotation = Quaternion.Euler(new Vector3(0, 180, 180));

        //        //    xTo += wCard - overlap;
        //        //}
        //        foreach (HandCard card in lstCard)
        //        {
        //            Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.9f : xTo, index == 0 ? expandPosition.position.y + 0.01f * currentIndex : transform.position.y, index == 0 ? expandPosition.position.z + 0.001f * currentIndex : transform.position.z);

        //            card.MoveTo(newPosition, 0.2f, () =>
        //            {
        //                card.UpdatePosition();
        //                if (index == 0)
        //                {
        //                    card.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //                    //card.transform.localRotation = transform.localRotation;
        //                }
        //                else
        //                {
        //                    card.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
        //                    //x.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        //                }
        //            });

        //            xTo += wCard - overlap;
        //            currentIndex -= 1;
        //        }
        //        callback?.Invoke();
        //#elif UNITY_STANDALONE || UNITY_EDITOR
        float[] layout = BuildLayoutCard(lstCard.Count);
        float xTo = layout[0];
        float wCard = layout[1];
        float hCard = layout[2];
        float overlap = layout[3];

        int currentIndex = 0;

        //lstCard.ForEach(x =>
        //{
        //    Vector3 newPosition = new Vector3(xTo, index == 0 ? transform.position.y - 0.01f * currentIndex : transform.position.y, index == 0 ? transform.position.z - 0.001f * currentIndex : transform.position.z);

        //    x.MoveTo(newPosition, 0.2f, () =>
        //    {
        //        x.UpdatePosition();
        //        if (index == 0)
        //            x.transform.localRotation = transform.localRotation;
        //        else
        //        {
        //            x.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(180, 0, 0));
        //            //x.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        //        }    
        //    });

        //    xTo += wCard - overlap;
        //    currentIndex += 1;
        //});
        for(int i = lstCard.Count - 1; i >= 0; i--)
        {
            //Vector3 newPosition = new Vector3(xTo, index == 0 ? transform.position.y + 0.02f * currentIndex : transform.position.y, index == 0 ? transform.position.z - 0.002f * currentIndex : transform.position.z);
            // new position for index 0 : 0.15 , 0.001
            Vector3 newPosition0 = new Vector3(
                xTo,
                transform.position.y + 0.08f * i,
                transform.position.z + 0.006f * i);

            // new position for index 1
            Vector3 newPosition1 = new Vector3(
                xTo-2,
                transform.position.y + 0.01f * currentIndex,
                transform.position.z);
            HandCard hc = lstCard[i];
            hc.MoveTo(index == 0 ? newPosition0 : newPosition1, 0.2f, () =>
            {
                if (index == 0)
                {
                    hc.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(20, 0, 0));
                    //hc.transform.localRotation = transform.localRotation;
                }
                else
                {
                    hc.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(160, 0, 0));
                }
                 hc.UpdatePosition();
               //lstCard[i].UpdatePosition();
            });

            xTo += wCard - overlap;
            currentIndex += 1;
        }
        callback?.Invoke();
        //#endif
    }


    //public void ReBuildDeck(int index = 0, ICallback.CallFunc callback = null)
    //{
    //    //if (!canRebuildDeck)
    //    //    return;

    //    //#if UNITY_ANDROID || UNITY_IOS
    //    //        switch (status)
    //    //        {
    //    //            case HandDeckStatus.Normal:
    //    //                {
    //    //                    float[] layout = BuildLayoutCard(lstCard.Count);
    //    //                    float xTo = layout[0];
    //    //                    float wCard = layout[1];
    //    //                    float hCard = layout[2];
    //    //                    float overlap = layout[3];

    //    //                    int currentIndex = 0;

    //    //                    float totalTwist = 15;
    //    //                    int numberOfCard = lstCard.Count;
    //    //                    float twistPerCard = totalTwist / numberOfCard;
    //    //                    float startTwist = 0;

    //    //                    lstCard.ForEach(x =>
    //    //                    {
    //    //                        float twistForThisCard = startTwist + ((currentIndex) * twistPerCard);
    //    //                        float nudgeForThisCard = Mathf.Abs(twistForThisCard);
    //    //                        nudgeForThisCard = currentIndex < 5 ? 0 * currentIndex : 0.001f * currentIndex;
    //    //                        Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.957f : xTo, index == 0 ? normalPosition.position.y - 0.01f * currentIndex : transform.position.y, index == 0 ? normalPosition.position.z - nudgeForThisCard : transform.position.z);
    //    //                        x.MoveTo(newPosition, 0.2f, () =>
    //    //                        {
    //    //                            x.UpdatePosition();
    //    //                        });


    //    //                        if (index == 0)
    //    //                            x.transform.rotation = transform.rotation;
    //    //                        else
    //    //                            x.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 180));

    //    //                        x.transform.rotation = Quaternion.Euler(Vector3.zero);
    //    //                        x.transform.Rotate(0, -twistForThisCard, 0);

    //    //                        xTo += wCard - overlap;
    //    //                        currentIndex += 1;
    //    //                    });
    //    //                    break;
    //    //                }
    //    //            case HandDeckStatus.Expand:
    //    //                {
    //    //                    float[] layout = BuildLayoutCard(lstCard.Count);
    //    //                    float xTo = layout[0];
    //    //                    float wCard = layout[1];
    //    //                    float hCard = layout[2];
    //    //                    float overlap = layout[3];

    //    //                    int currentIndex = 0;

    //    //                    float totalTwist = 15;
    //    //                    int numberOfCard = lstCard.Count;
    //    //                    float twistPerCard = totalTwist / numberOfCard;
    //    //                    float startTwist = -1f * (totalTwist / 2f); ;
    //    //                    float scalingFactor = 0.01f;

    //    //                    lstCard.ForEach(x =>
    //    //                    {
    //    //                        float twistForThisCard = startTwist + ((currentIndex) * twistPerCard);
    //    //                        float nudgeForThisCard = Mathf.Abs(twistForThisCard);
    //    //                        nudgeForThisCard *= scalingFactor;
    //    //                        Vector3 newPosition = new Vector3(xTo, index == 0 ? expandPosition.position.y - 0.01f * currentIndex : transform.position.y, index == 0 ? expandPosition.position.z + nudgeForThisCard - 0.3f : transform.position.z);
    //    //                        x.MoveTo(newPosition, 0.2f, () =>
    //    //                        {
    //    //                            x.UpdatePosition();
    //    //                        });


    //    //                        if (index == 0)
    //    //                            x.transform.rotation = transform.rotation;
    //    //                        else
    //    //                            x.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 180));

    //    //                        x.transform.rotation = Quaternion.Euler(Vector3.zero);
    //    //                        x.transform.Rotate(0, -twistForThisCard, 0);

    //    //                        xTo += wCard - overlap;
    //    //                        currentIndex += 1;
    //    //                    });
    //    //                    callback?.Invoke();
    //    //                    break;
    //    //                }
    //    //        }
    //    //#else
    //    float[] layout = BuildLayoutCard(lstCard.Count);
    //    float xTo = layout[0];
    //    float wCard = layout[1];
    //    float hCard = layout[2];
    //    float overlap = layout[3];

    //    for (int i = lstCard.Count - 1; i >= 0; i--)
    //    {
    //        Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.9f : xTo, index == 0 ? expandPosition.position.y + 0.003f * i : transform.position.y, index == 0 ? expandPosition.position.z + 0.001f * i : transform.position.z);
    //        lstCard[i].MoveTo(newPosition, 0.2f, () =>
    //        {
    //            lstCard[i].UpdatePosition();
    //            //if (i == 0)
    //            //    canRebuildDeck = true;
    //        });

    //        if (index == 0)
    //            lstCard[i].transform.rotation = transform.rotation;
    //        else
    //            lstCard[i].transform.rotation = Quaternion.Euler(new Vector3(0, 180, 180));

    //        xTo += wCard - overlap;
    //    }

    //    callback?.Invoke();
    //    //#endif
    //}

    public void RebuildDeckOnDrawDeck(int index = 0, Transform screenPoint = null, ICallback.CallFunc callback = null)
    {
        StartCoroutine(RebuildDeckOnDrawDeckRoutine(index, screenPoint, callback));
        canRebuildDeck = false;
    }

    private IEnumerator RebuildDeckOnDrawDeckRoutine(int index = 0, Transform screenPoint = null, ICallback.CallFunc callback = null)
    {
        yield return new WaitForSeconds(0);

        //#if UNITY_ANDROID || UNITY_IOS

        //        float[] layout = BuildLayoutCard(lstCard.Count);
        //        float xTo = layout[0];
        //        float wCard = layout[1];
        //        float hCard = layout[2];
        //        float overlap = layout[3];

        //        int currentIndex = 0;

        //        float totalTwist = 15;
        //        int numberOfCard = lstCard.Count;
        //        float twistPerCard = totalTwist / numberOfCard;
        //        float startTwist = 0;

        //        for (int i = lstCard.Count - 1; i >= 0; i--)
        //        {
        //            float twistForThisCard = startTwist + (currentIndex * twistPerCard);
        //            float nudgeForThisCard = Mathf.Abs(twistForThisCard);
        //            nudgeForThisCard = currentIndex < 5 ? 0 * currentIndex : 0.001f * currentIndex;
        //            Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.957f : xTo, index == 0 ? normalPosition.position.y - 0.01f * currentIndex : transform.position.y, index == 0 ? normalPosition.position.z - nudgeForThisCard : transform.position.z);
        //            lstCard[i].DrawTo(newPosition, screenPoint, twistForThisCard, () =>
        //            {
        //                lstCard[i].UpdatePosition();
        //                if (i == 0)
        //                {
        //                    canRebuildDeck = true;
        //                }
        //            });


        //            currentIndex++;
        //            xTo += wCard - overlap;
        //            yield return new WaitForSeconds(2);
        //        }
        //#else
        float[] layout = BuildLayoutCard(lstCard.Count);
        float xTo = layout[0];
        float wCard = layout[1];
        float hCard = layout[2];
        float overlap = layout[3];
        for (int i = lstCard.Count - 1; i >= 0; i--)
        {
            Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.9f : xTo, index == 0 ? expandPosition.position.y + 0.006f * i : transform.position.y, index == 0 ? expandPosition.position.z + 0.001f * i : transform.position.z);
            lstCard[i].DrawTo(newPosition, screenPoint, 0, () =>
            {
                lstCard[i].UpdatePosition();
            });
            yield return new WaitForSeconds(0.6f);
            

            xTo += wCard - overlap;
        }
        //#endif

        callback?.Invoke();
    }

    public void RebuildDeckOnAddCard(int index = 0, Transform screenPoint = null, Card card = null, ICallback.CallFunc callback = null)
    {
        StartCoroutine(RebuildDeckOnAddCardRoutine(index, screenPoint, card, callback));
        canRebuildDeck = false;
    }

    private IEnumerator RebuildDeckOnAddCardRoutine(int index = 0, Transform screenPoint = null, Card card = null, ICallback.CallFunc callback = null)
    {
        yield return new WaitForSeconds(0);

        //#if UNITY_ANDROID || UNITY_IOS
        //        switch (status)
        //        {
        //            case HandDeckStatus.Normal:
        //                {
        //                    float[] layout = BuildLayoutCard(lstCard.Count);
        //                    float xTo = layout[0];
        //                    float wCard = layout[1];
        //                    float hCard = layout[2];
        //                    float overlap = layout[3];

        //                    int currentIndex = 0;

        //                    float totalTwist = 15;
        //                    int numberOfCard = lstCard.Count;
        //                    float twistPerCard = totalTwist / numberOfCard;
        //                    float startTwist = 0;
        //                    for (int i = lstCard.Count - 1; i >= 0; i--)
        //                    {
        //                        float twistForThisCard = startTwist + (currentIndex * twistPerCard);
        //                        float nudgeForThisCard = Mathf.Abs(twistForThisCard);
        //                        nudgeForThisCard = currentIndex < 5 ? 0 * currentIndex : 0.001f * currentIndex;
        //                        Debug.Log(twistForThisCard);
        //                        Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.957f : xTo, index == 0 ? normalPosition.position.y - 0.01f * currentIndex : transform.position.y, index == 0 ? normalPosition.position.z - nudgeForThisCard : transform.position.z);
        //                        if (lstCard[i] == card)
        //                        {
        //                            lstCard[i].DrawTo(newPosition, screenPoint, twistForThisCard, () =>
        //                            {
        //                                canRebuildDeck = true;
        //                                lstCard[i].UpdatePosition();
        //                            });
        //                        }
        //                        else
        //                        {
        //                            lstCard[i].MoveTo(newPosition, 0.1f, () =>
        //                            {
        //                                lstCard[i].UpdatePosition();
        //                            });
        //                            lstCard[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
        //                            lstCard[i].transform.Rotate(0, -twistForThisCard, 0);
        //                        }

        //                        xTo += wCard - overlap;
        //                        currentIndex++;
        //                    }
        //                    break;
        //                }
        //            case HandDeckStatus.Expand:
        //                {
        //                    float[] layout = BuildLayoutCard(lstCard.Count);
        //                    float xTo = layout[0];
        //                    float wCard = layout[1];
        //                    float hCard = layout[2];
        //                    float overlap = layout[3];

        //                    int currentIndex = 0;

        //                    float totalTwist = 15;
        //                    int numberOfCard = lstCard.Count;
        //                    float twistPerCard = totalTwist / numberOfCard;
        //                    float startTwist = -1f * (totalTwist / 2f); ;
        //                    float scalingFactor = 0.01f;

        //                    for (int i = lstCard.Count - 1; i >= 0; i--)
        //                    {
        //                        float twistForThisCard = startTwist + (currentIndex * twistPerCard);
        //                        float nudgeForThisCard = Mathf.Abs(twistForThisCard);
        //                        nudgeForThisCard *= scalingFactor;
        //                        Vector3 newPosition = new Vector3(xTo, index == 0 ? expandPosition.position.y - 0.01f * currentIndex : transform.position.y, index == 0 ? expandPosition.position.z + nudgeForThisCard - 0.3f : transform.position.z);
        //                        if (lstCard[i] == card)
        //                        {
        //                            lstCard[i].DrawTo(newPosition, screenPoint, twistForThisCard, () =>
        //                            {
        //                                canRebuildDeck = true;
        //                                lstCard[i].UpdatePosition();
        //                            });
        //                        }
        //                        else
        //                        {
        //                            lstCard[i].MoveTo(newPosition, 0.1f, () =>
        //                            {
        //                                lstCard[i].UpdatePosition();
        //                            });
        //                            lstCard[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
        //                            lstCard[i].transform.Rotate(0, -twistForThisCard, 0);
        //                        }

        //                        xTo += wCard - overlap;
        //                        currentIndex += 1;
        //                    }
        //                    break;
        //                }
        //        }
        //#else
        float[] layout = BuildLayoutCard(lstCard.Count);
        float xTo = layout[0];
        float wCard = layout[1];
        float hCard = layout[2];
        float overlap = layout[3];
        for (int i = lstCard.Count - 1; i >= 0; i--)
        {
            Vector3 newPosition = new Vector3(index == 0 ? xTo - 0.9f : xTo, index == 0 ? expandPosition.position.y + 0.003f * i : transform.position.y, index == 0 ? expandPosition.position.z + 0.001f * i : transform.position.z);
            if (lstCard[i] == card)
            {
                lstCard[i].DrawTo(newPosition, screenPoint, 0, () =>
                {
                    lstCard[i].UpdatePosition();
                });
            }
            else
            {
                lstCard[i].MoveTo(newPosition, 0.2f, () =>
                {
                    lstCard[i].UpdatePosition();
                });
            }

            xTo += wCard - overlap;
        }
        //#endif
        callback?.Invoke();
    }

    private float[] BuildLayoutCard(int numCard)
    {
        //#if UNITY_IOS || UNITY_ANDROID
        //        float spaceLeft = 1.1f;
        //#else
        float spaceLeft = (indexDeck==0 ? (numCard == 0 ? 2.5f :( 1 + numCard * 2 / (numCard +1))) : 1.5f);
        //#endif
        float wCard = /*BattleSceneTutorial.instance != null ? */ handCardMesh.bounds.size.x  /*: 5*/;
        float hCard = handCardMesh.bounds.size.y;

        //wCard = 5;        
        float overlap = (numCard > 1) ? /*(wCard - (spaceLeft - wCard) / (numCard - 1)) - 0.1f*/ (indexDeck == 0 ? 0.2f : 0.7f) : 0f;

        //if (overlap < wCard * 0.1f && numCard > 1) overlap = wCard * 0.1f;
        //if (overlap > wCard * 0.5f) overlap = wCard * 0.5f;

        float wAll = (wCard - overlap) * (numCard - 1) + wCard;

        //pivot of hand card at (0.5, 0.5) and (0, 0) is in center of screen
        float xTo = -wAll * 0.5f + wCard * 0.5f   ;
        //float yTo = (yStart != null) ? yStart(canvas.height, hCard) : (-canvas.height * 0.5f + offset.y - hCard * 0.25f);
        //check point in top left of card
        if (xTo - wCard * 0.5f < -3.5f/*-Screen.width * 0.5f + paddingL*/)
            xTo = /*-Screen.width * 0.5f + paddingL*/wCard * 0.5f;

        return new float[] { xTo, wCard, hCard, overlap };
    }
}
