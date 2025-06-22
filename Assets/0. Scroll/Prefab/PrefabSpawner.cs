using EZCameraShake;
using GIKCore;
using GIKCore.Sound;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPrefabPair
    {
        public Button button;
        public GameObject prefab;
        public GameObject parentObject;
        public float destructionDelay; // Thoi gian de tat prefab
    }

    public List<ButtonPrefabPair> buttonPrefabPairs;
    [SerializeField] private GameObject m_BidIdleUI;
    private long curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid;
    private GameObject currentSpawnedObj;
    void Start()
    {
        foreach (var pair in buttonPrefabPairs)
        {
            pair.button.onClick.AddListener(() => SpawnPrefab(pair));
        }
    }

    void SpawnPrefab(ButtonPrefabPair pair)
    {
        if (pair.prefab != null)
        {
            GameObject instantiatedPrefab;
            if (pair.parentObject != null)
            {
                instantiatedPrefab = Instantiate(pair.prefab, pair.parentObject.transform);
            }
            else
            {
                instantiatedPrefab = Instantiate(pair.prefab, Vector3.zero, Quaternion.identity);
            }
            currentSpawnedObj = instantiatedPrefab;
            if (pair.destructionDelay > 0)
            {
                Destroy(instantiatedPrefab, pair.destructionDelay);
            }
        }
        else
        {
            Debug.LogWarning("Khong co prefab o nut: " + pair.button.name);
        }

    }
    public void OnStartBid(long pShard, long eShard, long pBid, long eBid,long timeRemain = -1)
    {
        curPlayerBid = pBid;
        curEnemyBid = eBid;
        curPlayerShard = pShard;
        curEnemyShard = eShard;
        m_BidIdleUI.SetActive(true);
        m_BidIdleUI.GetComponent<BidStateInfo>().SetData(pShard, eShard, pBid, eBid, true, timeRemain);

        SoundHandler.main.PlaySFX("Coinflip to Idle", "sounds");
    }
    public void OnNoBidRound()
    {
        SpawnPrefab(buttonPrefabPairs[2]);
        if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
            currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
        SoundHandler.main.PlaySFX("NoBids", "sounds");
    }
    public void OnUpBidYou(long curBid, long curBalance)
    {
        curPlayerBid = curBid;
        curPlayerShard = curBalance;
        SpawnPrefab(buttonPrefabPairs[1]);
        SoundHandler.main.PlaySFX("Bid_You", "sounds");
        if (currentSpawnedObj.GetComponent<SetTextProgression>() != null)
            currentSpawnedObj.GetComponent<SetTextProgression>().SetData(curPlayerBid.ToString());
        m_BidIdleUI.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
    }
    public void OnUpBidEnemy(long curBid, long curBalance)
    {
        curEnemyBid = curBid;
        curEnemyShard = curBalance;
        SpawnPrefab(buttonPrefabPairs[0]);
        SoundHandler.main.PlaySFX("Bid_Opp", "sounds");
        if (currentSpawnedObj.GetComponent<SetTextProgression>() != null)
            currentSpawnedObj.GetComponent<SetTextProgression>().SetData(curEnemyBid.ToString());
        m_BidIdleUI.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
    }
    public void OnEndBid(BidState state, bool isMeWinBid, out float timeEff, bool isPrepareEndSkill = false)
    {
        switch (state)
        {
            case BidState.Lose:
                {
                    // opp win
                    if (isPrepareEndSkill)
                    {
                        // effect thang ->eff prepare
                        StartCoroutine(DoSerialSpawn(() =>
                        {
                            SpawnPrefab(buttonPrefabPairs[6]);
                            SoundHandler.main.PlaySFX("BidToEffect-Opp", "sounds");
                            if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                        },
                        3.5f,
                        () => { SpawnPrefab(buttonPrefabPairs[12]); }));
                        timeEff = 6;
                    }
                    else
                    {
                        StartCoroutine(DoSerialSpawn(() =>
                        {
                            SpawnPrefab(buttonPrefabPairs[4]);
                            SoundHandler.main.PlaySFX("BidToEffect-Opp", "sounds");
                            if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                        },
                                                             3.5f,
                                                             () =>
                                                             {
                                                                 SoundHandler.main.PlaySFX("EffectOpp", "sounds");
                                                                 SpawnPrefab(buttonPrefabPairs[10]);
                                                                 CameraShaker.Instance.ShakeOnce(1f, 2, .1f, .6f);
                                                             }));
                        timeEff = 6;
                    }
                    m_BidIdleUI.SetActive(false);
                    break;
                }

            case BidState.Random:
                {
                    // hoa
                    if (!isMeWinBid)
                    {
                        
                        if (isPrepareEndSkill)
                        {
                            StartCoroutine(DoSerialSpawn(() => 
                                                        { 
                                                            SpawnPrefab(buttonPrefabPairs[8]);
                                                            SoundHandler.main.PlaySFX("Coinflip_Lose", "sounds");
                                                        },
                                                        3.5f,
                                                        () =>
                                                        {
                                                            SpawnPrefab(buttonPrefabPairs[6]);
                                                            if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                                                currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                                                            m_BidIdleUI.SetActive(false);
                                                        },
                                                        3.5f,
                                                        () => { SpawnPrefab(buttonPrefabPairs[12]); }));
                            timeEff = 9;
                        }
                        else
                        {
                            StartCoroutine(DoSerialSpawn(() =>
                                                        { 
                                                            SpawnPrefab(buttonPrefabPairs[8]);
                                                            SoundHandler.main.PlaySFX("Coinflip_Lose", "sounds");
                                                        },
                                                        3.5f,
                                                        () =>
                                                        {
                                                            SpawnPrefab(buttonPrefabPairs[4]);
                                                            if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                                                currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                                                            m_BidIdleUI.SetActive(false);
                                                        },
                                                        3.5f,
                                                        () =>
                                                        {
                                                            SoundHandler.main.PlaySFX("EffectOpp", "sounds");
                                                            SpawnPrefab(buttonPrefabPairs[10]);
                                                            CameraShaker.Instance.ShakeOnce(1f, 2, .1f, .6f);
                                                        }));
                            timeEff = 9;
                        }
                    }
                    else
                    {
                        
                        if (isPrepareEndSkill)
                        {
                            StartCoroutine(DoSerialSpawn(() => 
                                                         { 
                                                             SpawnPrefab(buttonPrefabPairs[7]);
                                                             SoundHandler.main.PlaySFX("Coinflip-You", "sounds");
                                                         },
                                                         3.5f,
                                                         () =>
                                                         {
                                                             SpawnPrefab(buttonPrefabPairs[5]);
                                                             if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                                                 currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                                                             m_BidIdleUI.SetActive(false);
                                                         },
                                                         3.5f,
                                                         () => { SpawnPrefab(buttonPrefabPairs[11]); }));
                            timeEff = 9;
                        }
                        else
                        {
                            StartCoroutine(DoSerialSpawn(() => 
                                                         {
                                                            SpawnPrefab(buttonPrefabPairs[7]);
                                                             SoundHandler.main.PlaySFX("Coinflip-You", "sounds");
                                                         },
                                                         3.5f,
                                                         () =>
                                                         {
                                                             SpawnPrefab(buttonPrefabPairs[3]);
                                                             if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                                                 currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                                                             m_BidIdleUI.SetActive(false);
                                                         },
                                                         3.5f,
                                                         () =>
                                                         {
                                                             SoundHandler.main.PlaySFX("EffectYou", "sounds");
                                                             SpawnPrefab(buttonPrefabPairs[9]);
                                                             CameraShaker.Instance.ShakeOnce(1f, 2, .1f, .6f);
                                                         }));
                            timeEff = 9;
                        }
                    }
                    break;
                }
            case BidState.Win:
                {
                    // you win
                    if (isPrepareEndSkill)
                    {
                        StartCoroutine(DoSerialSpawn(() =>
                        {
                            SpawnPrefab(buttonPrefabPairs[5]);
                            SoundHandler.main.PlaySFX("BidToEffect-You", "sounds");
                            if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                        },
                                                     3.5f,
                                                     () => { SpawnPrefab(buttonPrefabPairs[11]); }));
                        timeEff = 6;
                    }
                    else
                    {
                        StartCoroutine(DoSerialSpawn(() =>
                        {
                            SpawnPrefab(buttonPrefabPairs[3]);
                            SoundHandler.main.PlaySFX("BidToEffect-You", "sounds");
                            if (currentSpawnedObj != null && currentSpawnedObj.GetComponent<BidStateInfo>() != null)
                                currentSpawnedObj.GetComponent<BidStateInfo>().SetData(curPlayerShard, curEnemyShard, curPlayerBid, curEnemyBid);
                        },
                                                     3.5f,
                                                     () =>
                                                     {
                                                         SoundHandler.main.PlaySFX("EffectYou", "sounds");
                                                         SpawnPrefab(buttonPrefabPairs[9]);
                                                         CameraShaker.Instance.ShakeOnce(1f, 2, .1f, .6f);
                                                     }));
                        timeEff = 6;
                    }
                    m_BidIdleUI.SetActive(false);
                    break;
                }
            default:
                timeEff = 6;
                break;
        }

    }
    IEnumerator DoSerialSpawn(ICallback.CallFunc cb1, float timeWait, ICallback.CallFunc cb2, float time2 = 0, ICallback.CallFunc cb3 = null)
    {
        if (cb1 != null)
            cb1();
        yield return new WaitForSeconds(timeWait);
        if (cb2 != null)
            cb2();
        yield return new WaitForSeconds(time2);
        if (cb3 != null)
            cb3();
        yield return new WaitForSeconds(3f);
        Game.main.socket.ConfirmBidState();


    }
}
