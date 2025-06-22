using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.DB;
using GIKCore.Utilities;
using pbdson;
using UIEngine.UIPool;
using UIEngine.Extensions;

public class BattleLoadingScene : GameListener
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_TxtName;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private HorizontalPoolGroup m_Pool;

    // Methods

    public void DoBack()
    {
        Game.main.LoadScene("HomeScene");
    }

    public void FindCompetitor()
    {
        Game.main.socket.GameBattleJoin();
    }

    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.GET_USER_BATTLE_DECK:
                {
                    List<DBHero> adapter = GameData.main.battleDeck.GetDatabases();
                    m_Pool.SetAdapter(adapter);
                    break;
                }

            case IService.GAME_BATTLE_JOIN:
                {
                    ListCommonVector lstCommonVector = ISocket.Parse<ListCommonVector>(data);
                    PlayerBattleJoin(lstCommonVector);
                    break;
                }

            case IService.GAME_BATTLE_LEAVE:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    PlayerBattleLeave(commonVector);
                    break;
                }

            case IService.GAME_START:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    GameStart(commonVector);
                    break;
                }

        }
        return false;
    }

    private void GameStart(CommonVector commonVector)
    {
        LogWriterHandle.WriteLog("GAME START=" + IUtil.ConvertListLongToString(commonVector.aLong));
        LogWriterHandle.WriteLog("GAME START=" + IUtil.ConvertListStringToString(commonVector.aString));
        long idFirst = commonVector.aLong[0];
        string usernameFirst = commonVector.aString[0];

        foreach (BattlePlayer player in GameData.main.mLstBattlePlayer)
            if (player.id == idFirst)
                player.isFirst = true;
        
        Game.main.LoadScene("BattleScene");
    }

    private void PlayerBattleLeave(CommonVector commonVector)
    {
     
        long position = commonVector.aLong[0];
        long id = commonVector.aLong[1];
        foreach (BattlePlayer player in GameData.main.mLstBattlePlayer)
            if (player.id == id)
            {
                GameData.main.mLstBattlePlayer.Remove(player);
                break;
            }
    
    }

    private void PlayerBattleJoin(ListCommonVector lstCommonVector)
    {
        //if (lstCommonVector.aVector.Count > 0)
        foreach (CommonVector cv in lstCommonVector.aVector)

        {
            //CommonVector cv = lstCommonVector.aVector[0];
            LogWriterHandle.WriteLog("GameJoin =" + IUtil.ConvertListLongToString(cv.aLong));
            LogWriterHandle.WriteLog("GameJoin =" + IUtil.ConvertListStringToString(cv.aString));

            BattlePlayer player = new BattlePlayer();
            player.position = cv.aLong[0];
            player.id = cv.aLong[1];
            player.username = cv.aString[0];
            player.screenname = cv.aString[1];

            GameData.main.mLstBattlePlayer.Add(player);
            Debug.Log(GameData.main.mLstBattlePlayer.Count +"");

            if (GameData.main.profile.userID == cv.aLong[1])
                PopupFindCompetitor.Show();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_Pool.SetCellDataCallback((GameObject go, DBHero data, int index) =>
        {
            string s = string.Format("{0}\n{1}\nMana: {2}\nATK: {3}\nHP: {4}", UIText.StringBoldface(CardData.Instance.GetCardDataInfo(data.id).name), data.GetTypeName(), data.mana, data.atk, data.hp);
            TextMeshProUGUI label = go.GetComponentInChildren<TextMeshProUGUI>();
            label.text = s;
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        m_TxtName.text = GameData.main.profile.displayName;
        Game.main.socket.GetUserBattleDeck();
        GameData.main.mLstBattlePlayer = new List<BattlePlayer>();
    }

    // Update is called once per frame
    //void Update() { }
}
