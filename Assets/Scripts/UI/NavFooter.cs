using GIKCore;
using GIKCore.Lang;
using GIKCore.Sound;
using GIKCore.UI;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavFooter : MonoBehaviour
{
    public static NavFooter instance;
    public ICallback.CallFunc onChangeState;

    private void Awake()
    {
        instance = this;
    }
    // Fields

    // Values

    // Methods
    public NavFooter SetActive()
    {
        return this;
    }
    public void DoClickCollection()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (ProgressionController.instance != null)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return; 
        }
        Game.main.socket.GetUserPack();
        Game.main.socket.GetUserDeck();
        Game.main.socket.ViewUpgrade();
        Game.main.socket.FirstDeck();
        Game.main.LoadScene("CollectionScene", onLoadSceneSuccess: () =>
        {
            CollectionScene.instance.InitDataContent(0);
        }, delay: 0.3f, curtain: true);
        onChangeState?.Invoke();
    }
    public void DoClickQuests()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (ProgressionController.instance != null)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
             return;
        }    
        Game.main.socket.GetQuests();
        Game.main.LoadScene("QuestScene", delay: 0.3f, curtain: true);
        onChangeState?.Invoke();
        return;
    }
    public void DoClickShop()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (ProgressionController.instance != null)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return;
        }    
        Game.main.socket.GetShopItem();
        Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
        onChangeState?.Invoke();
    }
    public void DoClickRewards()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (ProgressionController.instance != null)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return;
        }
        //onChangeState?.Invoke();

        return;
    }
    public void DoClickPlay()
    {
        //Game.main.socket.GetUserDeck();
        //Game.main.socket.GetRank();
        //Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        if (ProgressionController.instance != null)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return;
        }    
        if (GameData.main.userHasReachedLevel6)
        {
            Game.main.socket.GetMode();
            Game.main.socket.GetRank();
            Game.main.socket.GetUserDeck();
        }
        else
        {
            Game.main.socket.GetUserDeck();
            Game.main.socket.GetRank();
            Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
        }

    }
    public void DoClickDisableButton()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
        onChangeState?.Invoke();
    }
}
