using GIKCore;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;
    //[SerializeField] private TextMeshProUGUI m_Status;
    //[SerializeField] private Button m_ShowInters;
    //[SerializeField] private Button m_ShowReward;

    BannerView _bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private const string bannerID = "ca-app-pub-3940256099942544/6300978111";
    private const string intersID = "ca-app-pub-3940256099942544/1033173712";
    private const string rewardID = "ca-app-pub-3940256099942544/5224354917";

    #region callback
    public delegate void OnRewardFail();
    public event OnRewardFail onRewardFail;
    public delegate void OnRewardSuccess();
    public event OnRewardSuccess onRewardSuccess;
    #endregion


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        //m_ShowInters.onClick.AddListener(ShowIntersAds);
        //m_ShowReward.onClick.AddListener(() =>
        //{
        //    ShowRewardedAd(() =>
        //    {

        //    }, () =>
        //    {

        //    });
        //});
        MobileAds.Initialize(initStatus =>
        {
            //m_Status.text = "Init success";
            Debug.Log("Init success");
        });
        //LoadBanner();
        LoadInterstitialAd();
        LoadRewardedAd();
    }
    #region Banner
    private void CreateBannerView()
    {
        //m_Status.text = ("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.Bottom);
        ListenToBannerAdEvents();
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBanner()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();

        // send the request to load the ad.
        //m_Status.text = ("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            //m_Status.text = ("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void ListenToBannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            //m_Status.text = ("Banner view loaded an ad with response : "
            //    + _bannerView.GetResponseInfo());
            _bannerView.Show();
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            //m_Status.text = ("Banner view failed to load an ad with error : "
            //    + error);
            LoadBanner();
        };
    }
    #endregion

    #region Inters
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

       // m_Status.text = ("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        // send the request to load the ad.
        InterstitialAd.Load(intersID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    //m_Status.text = ("interstitial ad failed to load an ad " +
                                  // "with error : " + error);
                    LoadInterstitialAd();
                    return;
                }

                //m_Status.text = ("Interstitial ad loaded with response : "
                //          + ad.GetResponseInfo());

                interstitialAd = ad;
                RegisterEventHandlers(interstitialAd);
            });
    }

    public void ShowIntersAds()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            //m_Status.text = ("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            //m_Status.text = ("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            //m_Status.text = ("Interstitial ad full screen content closed.");
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            //m_Status.text = ("Interstitial ad failed to open full screen content " +
            //              "with error : " + error);
            LoadInterstitialAd();
        };
    }
    #endregion

    #region Reward
    public bool HaveReward()
    {
        return rewardedAd.CanShowAd();
    }
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        //m_Status.text = ("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder().Build();

        // send the request to load the ad.
        RewardedAd.Load(rewardID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    //m_Status.text = ("Rewarded ad failed to load an ad " +
                      //             "with error : " + error);
                    LoadRewardedAd();
                    return;
                }

                //m_Status.text = ("Rewarded ad loaded with response : "
                  //        + ad.GetResponseInfo());

                rewardedAd = ad;
                RegisterEventHandlers(rewardedAd);
            });
    }

    public void ShowRewardedAd(OnRewardFail callbackFail, OnRewardSuccess callbackSuccess)
    {
        onRewardFail = callbackFail;
        onRewardSuccess = callbackSuccess;
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                //m_Status.text = (string.Format(rewardMsg, reward.Type, reward.Amount));
                LoadRewardedAd();
                //m_Status.text += "\n <size=50>Reward granted</size>";
                onRewardSuccess?.Invoke();
            });
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            //m_Status.text = ("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            //m_Status.text = ("Rewarded ad failed to open full screen content " +
              //             "with error : " + error);
            onRewardFail?.Invoke();
            LoadRewardedAd();
        };
    }
    #endregion
}