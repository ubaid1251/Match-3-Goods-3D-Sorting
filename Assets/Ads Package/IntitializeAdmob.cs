using GoogleMobileAds.Api;
using System;
using UnityEngine;
using GoogleMobileAds.Common;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.Events;


[System.Serializable]
public class BannerAdData
{
    [Header("Banner Ad ID")]
    public string bannerAdUnitIds=null;
    [Header("Banner Ad Position")]
    public AdPosition BadsPosition;
    [Header("Banner Ad Type")]
    public BannerType bannerType = BannerType.Simple_Banner;
    [HideInInspector]
    public BannerView bannerView;

    [Header("Big Banner Ad ID")]
    public string BigBannerAdUnitIds=null;
    [Header("Banner Ad Position")]
    public AdPosition BigBadsPosition;
    [HideInInspector]
    public BannerView BigBannerView;
}

[System.Serializable]
public class InterstialAdData
{
    [Header("Interstitial Ad ID")] public string admobInterstialID=null;
    [Header("static Interstitial Ad ID")] public string admobStaticInterstialID=null;
    [Header("Rewarded Interstitial Ad ID")] public string admobRewardedInterstialID=null;
}

public class IntitializeAdmob : MonoBehaviour
{
    public bool showLog = true;
    public static IntitializeAdmob Instance;
    public static bool showMeAd;
    [FormerlySerializedAs("AdmobTestIds")][Header("For Test Ads")] public bool admobTestIds = false;
    [Header("App ID")] public string admobAppID;
    public BannerAdData BannerData;
    public InterstialAdData InterData;

    [Header("Delay timer for Inter")] public float Delay_Time;
    public float get_touch = 0;
    public bool IsPlay_Count = true;
    public bool play_ad = false;

    [Header("App Open Ad ID")] public string AppOpenID=null;

    // App open ads can be preloaded for up to 4 hours.
    private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    private DateTime _expireTime;
    private AppOpenAd _appOpenAd;

    [Header("Rewarded Video ID")] public string RewardedID=null;
    public RewardedAd _rewardedAd;

    int BannerFailed = 0, InterFailed = 0, staticInterFailed = 0, BigBannerFailed = 0, RewardedFailed = 0, RewardedInterFailed = 0,
        appOpenFailed = 0;

    [HideInInspector] public InterstitialAd interstitialAd;
    [HideInInspector] public InterstitialAd interstitialStaticAd;
    [HideInInspector] public RewardedInterstitialAd rewardedinterstitialAd;


    void Update()
    {
        if (IsPlay_Count /*&& PlayerPrefs.GetInt("RemoveAds") == 0*/)
        {
            get_touch += Time.deltaTime;
            if (get_touch >= Delay_Time)
            {
                play_ad = true;
                IsPlay_Count = false;
            }
        }
    }

    void Awake()
    {
        Debug.unityLogger.logEnabled = showLog;
        // This is used to launch the loaded ad when we open the app.
        if (!string.IsNullOrEmpty(AppOpenID))
        {
            print(AppOpenID);
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }
        try
        {
            Instance = this;
            IsPlay_Count = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            DontDestroyOnLoad(gameObject);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void Start()
    {
        _isOtherAdShowing = false;
        try
        {
            CallAds();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log("Ads package not Initialise");
        }
    }


    #region AdsInitialization

    [Obsolete]
    public void CallAds()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
        deviceIds.Add("080f7973-1604-496a-a155-10ca1ad1abe1");
        MobileAds.SetRequestConfiguration(new RequestConfiguration()
        {
            TestDeviceIds = deviceIds
        });

        MobileAds.Initialize(initStatus =>
        {
            if (PlayerPrefs.GetInt("RemoveAds") == 0)
            {

                if (!string.IsNullOrEmpty(AppOpenID))
                    LoadAppOpenAd();
                RequestAdmobInterstitial();
                if (!string.IsNullOrEmpty(InterData.admobStaticInterstialID))
                    RequestStaticAdmobInterstitial();
                RequestBannerHigh();
                if (!string.IsNullOrEmpty(BannerData.BigBannerAdUnitIds))
                    RequestBigBanner();
            }

            if (!string.IsNullOrEmpty(RewardedID))
                LoadRewardedAd();
            if (!string.IsNullOrEmpty(InterData.admobRewardedInterstialID))
                LoadRewardedInterstitialAd();
        });
        
    }

    #endregion

    #region InterstitalAd

    [Obsolete]
    public void RequestAdmobInterstitial()
    {
        try
        {
            string interId = null;
            if (admobTestIds)
            {
                print("In test ID");
                interId = "ca-app-pub-3940256099942544/1033173712";
                InterData.admobInterstialID = interId;
            }

            // Clean up the old ad before loading a new one.
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            Debug.Log("Loading the interstitial ad.");
            // create our request used to load the ad.
            var adRequest = new AdRequest();
            // send the request to load the ad.
            InterstitialAd.Load(InterData.admobInterstialID, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.Log("interstitial ad failed to load an ad " + "with error : " + error);
                        //InitializeFirebase_CB._Instance.LogEvent("InterstitialAd_error");
                        return;
                    }
                    adRequest.Extras.Add("npa", "1");
                    //Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                    interstitialAd = ad;
                    RegisterEventHandlers(interstitialAd);
                });
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void ShowInterstitialAd()
    {
        if (IsInterAvailable())
        {
            ShowInterstitial();
        }
        else
        {
            resetLoaderTimer();
        }
    }

    private void ShowInterstitial()
    {
        if (play_ad)
        {
            // Debug.Log("Showing ShowInterstitial");
            if (interstitialAd != null)
            {
                if (interstitialAd.CanShowAd())
                {
                    _isOtherAdShowing = true;
                    resetLoaderTimer();
                    HideBanner();
                    // Debug.Log("Showing interstitial ad.");
                    interstitialAd.Show();
                }
            }
            else
            {
                RequestAdmobInterstitial();
                resetLoaderTimer();
            }
        }
    }

    public void resetLoaderTimer()
    {
        play_ad = false;
        showMeAd = false;
        get_touch = 0;
        IsPlay_Count = true;
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            _isOtherAdShowing = false;
            Debug.Log("Interstitial ad full screen content closed.");
            //interstitialAd.Destroy();
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Invoke(nameof(RequestAdmobInterstitial), 3);
                //CheckInternetConnection(AdmobAdInterstitialRequest);
            }
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("Interstitial ad failed to open full screen content " + "with error : " + error);
            InterFailed++;
            if (InterFailed < 5)
            {
                Invoke(nameof(RequestAdmobInterstitial), 3);
            }
        };
    }

    public bool IsInterAvailable()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
        {
            return false;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        if (interstitialAd == null)
        {
            print("sadad");
            return false;
        }

        if (interstitialAd != null)
        {
            if (!interstitialAd.CanShowAd())
            {
                print("sadad");
                return false;
            }
        }

        return play_ad;
    }

    #endregion

    #region StaticInterstitalAd

    [Obsolete]
    public void RequestStaticAdmobInterstitial()
    {
        try
        {
            string interId = null;
            if (admobTestIds)
            {
                print("In test ID");
                interId = "ca-app-pub-3940256099942544/1033173712";
                InterData.admobStaticInterstialID = interId;
            }

            // Clean up the old ad before loading a new one.
            if (interstitialStaticAd != null)
            {
                interstitialStaticAd.Destroy();
                interstitialStaticAd = null;
            }

            Debug.Log("Loading the interstitial static ad.");
            // create our request used to load the ad.
            var adRequest = new AdRequest();
            // send the request to load the ad.
            InterstitialAd.Load(InterData.admobStaticInterstialID, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.Log("interstitial ad failed to load an ad " + "with error : " + error);
                        //InitializeFirebase_CB._Instance.LogEvent("InterstitialAd_error");
                        return;
                    }
                    adRequest.Extras.Add("npa", "1");
                    //Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                    interstitialStaticAd = ad;
                    RegisterStaticEventHandlers(interstitialStaticAd);
                });
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    private void RegisterStaticEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            _isOtherAdShowing = false;
            Debug.Log("Interstitial ad full screen content closed.");
            //interstitialAd.Destroy();
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Invoke(nameof(RequestStaticAdmobInterstitial), 3);
                //CheckInternetConnection(AdmobAdInterstitialRequest);
            }
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("Interstitial ad failed to open full screen content " + "with error : " + error);
            staticInterFailed++;
            if (staticInterFailed < 5)
            {
                Invoke(nameof(RequestStaticAdmobInterstitial), 3);
            }
        };
    }
    public void ShowStaticInterstitialAd()
    {
        if (IsStaticInterAvailable())
        {
            ShowStaticInterstitial();
        }
        else
        {
            resetLoaderTimer();
        }
    }

    private void ShowStaticInterstitial()
    {
        if (play_ad)
        {
            // Debug.Log("Showing ShowInterstitial");
            if (interstitialStaticAd != null)
            {
                if (interstitialStaticAd.CanShowAd())
                {
                    _isOtherAdShowing = true;
                    resetLoaderTimer();
                    HideBanner();
                    // Debug.Log("Showing interstitial ad.");
                    interstitialStaticAd.Show();
                }
            }
            else
            {
                RequestStaticAdmobInterstitial();
                resetLoaderTimer();
            }
        }
    }

    public bool IsStaticInterAvailable()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
        {
            return false;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        if (interstitialStaticAd == null)
        {
            print("sadad");
            return false;
        }

        if (interstitialStaticAd != null)
        {
            if (!interstitialStaticAd.CanShowAd())
            {
                print("sadad");
                return false;
            }
        }

        return play_ad;
    }

    #endregion

    #region BannerAd

    public void RequestBannerHigh()
    {
        if (BannerData.bannerView != null)
        {
            DestroyBannerView();
        }

        string bannerId = null;
        if (admobTestIds)
        {
            bannerId = "ca-app-pub-3940256099942544/6300978111";
            BannerData.bannerAdUnitIds = bannerId;
        }
        else
        {
            bannerId = BannerData.bannerAdUnitIds;
        }

        if (BannerData.bannerType == BannerType.Simple_Banner)
        {
            BannerData.bannerView = new BannerView(BannerData.bannerAdUnitIds, AdSize.Banner, BannerData.BadsPosition);
        }
        else if (BannerData.bannerType == BannerType.Adaptive_Banner)
        {
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            BannerData.bannerView = new BannerView(BannerData.bannerAdUnitIds, adaptiveSize, BannerData.BadsPosition);
        }
        else
        {
            BannerData.bannerView = new BannerView(BannerData.bannerAdUnitIds, AdSize.SmartBanner, BannerData.BadsPosition);
        }

        // Create an ad request with the highest priority
        AdRequest bannerAdRequest = new AdRequest();

        // Load the banner ad with the ad request
        BannerData.bannerView.LoadAd(bannerAdRequest);
        // Debug.Log("Hiding banner view.");
        BannerData.bannerView.Hide();
        ListenToAdEvents();
    }

    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        BannerData.bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : " + BannerData.bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        BannerData.bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            BannerFailed++;
            if (BannerFailed < 5)
            {
                RequestBannerHigh();
            }

            Debug.Log("Banner view failed to load an ad with error : " + error);
        };
        // Raised when the ad is estimated to have earned money.
        //BannerData.bannerView.OnAdPaid += (AdValue adValue) =>
        //{
        //    Debug.Log(String.Format("Banner view paid {0} {1}.",
        //        adValue.Value,
        //        adValue.CurrencyCode));
        //};
        // Raised when an impression is recorded for an ad.
        BannerData.bannerView.OnAdImpressionRecorded += () => { Debug.Log("Banner view recorded an impression."); };
        // Raised when a click is recorded for an ad.
        BannerData.bannerView.OnAdClicked += () => { Debug.Log("Banner view was clicked."); };
        // Raised when an ad opened full screen content.
        BannerData.bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        BannerData.bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }


    public void ShowBanner()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            if (BannerData.bannerView != null) // && !BannerExsist())
            {
                Debug.Log("Showing banner view.");
                BannerData.bannerView.Show();
            }
            else
            {
                RequestBannerHigh();
            }
        }
    }

    public void HideBanner()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            if (BannerData.bannerView != null) // && !BannerExsist())
            {
                Debug.Log("Hiding banner view.");
                BannerData.bannerView.Hide();
            }
        }
    }

    public void DestroyBannerView()
    {
        if (BannerData.bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            BannerData.bannerView.Destroy();
            BannerData.bannerView = null;
        }
    }

    #endregion

    #region BigBannerAd
    public void RequestBigBanner()
    {
        if (BannerData.BigBannerView != null)
        {
            DestroyBigBannerView();
        }
        string bannerId = null;
        if (admobTestIds)
        {
            bannerId = "ca-app-pub-3940256099942544/6300978111";
            BannerData.BigBannerAdUnitIds = bannerId;
        }

        BannerData.BigBannerView = new BannerView(BannerData.BigBannerAdUnitIds, AdSize.MediumRectangle, BannerData.BigBadsPosition);

        // Create an ad request with the highest priority
        AdRequest bannerAdRequest = new AdRequest();

        // Load the banner ad with the ad request
        BannerData.BigBannerView.LoadAd(bannerAdRequest);
        Debug.Log("Hiding banner view.");
        HideBigBanner();
        ListenToBigAdEvents();
    }
    private void ListenToBigAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        BannerData.BigBannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : " + BannerData.BigBannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        BannerData.BigBannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            BigBannerFailed++;
            if (BigBannerFailed < 5)
            {
                RequestBigBanner();
            }
            Debug.Log("Banner view failed to load an ad with error : " + error);
        };
        // Raised when the ad is estimated to have earned money.
        BannerData.BigBannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        BannerData.BigBannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        BannerData.BigBannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        BannerData.BigBannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        BannerData.BigBannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    public bool isBigBannerShowing = false;

    public void ShowBigBanner()
    {
        if (!isBigBannerShowing)
        {
            isBigBannerShowing = true;
            if (PlayerPrefs.GetInt("RemoveAds") == 0)
            {
                if (BannerData.BigBannerView != null)// && !BannerExsist())
                {
                    Debug.Log("Showing banner view.");
                    BannerData.BigBannerView.Show();
                }
            }
        }
    }
    public void HideBigBanner()
    {
        isBigBannerShowing = false;
        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            if (BannerData.BigBannerView != null)// && !BannerExsist())
            {
                Debug.Log("Hiding banner view.");
                BannerData.BigBannerView.Hide();
            }
        }
    }
    public void DestroyBigBannerView()
    {
        if (BannerData.BigBannerView != null)
        {
            Debug.Log("Destroying banner view.");
            BannerData.BigBannerView.Destroy();
            BannerData.BigBannerView = null;
        }
    }

    #endregion

    #region AppOpen

    private void LoadAppOpenAd()
    {
        // Clean up the old ad before loading a new one.
        if (_appOpenAd != null)
        {
            DestroyAppOpenAd();
        }
        string appOpenId = null;
        if (admobTestIds)
        {
            appOpenId = "ca-app-pub-3940256099942544/9257395921";
            AppOpenID = appOpenId;
        }
        else
        {
            appOpenId = AppOpenID;
        }
        Debug.Log("Loading app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        AppOpenAd.Load(appOpenId, adRequest, (AppOpenAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.Log("App open ad failed to load an ad with error : "
                          + error);
                return;
            }

            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.Log("Unexpected error: App open ad load event fired with " +
                          " null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
            _appOpenAd = ad;

            // App open ads can be preloaded for up to 4 hours.
            _expireTime = DateTime.Now + TIMEOUT;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);
        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAppOpenAd()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 1)
            return;
        // App open ads can be preloaded for up to 4 hours.
        //print(_appOpenAd != null);
        //print(_appOpenAd.CanShowAd());
        //print(DateTime.Now < _expireTime);
        //print(_isOtherAdShowing==false);
        if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime && _isOtherAdShowing == false)
        {
            Debug.Log("Showing app open ad.");
            _appOpenAd.Show();
        }
        else
        {
            Debug.Log("App open ad is not ready yet.");
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAppOpenAd()
    {
        if (_appOpenAd != null)
        {
            Debug.Log("Destroying app open ad.");
            _appOpenAd.Destroy();
            _appOpenAd = null;
        }
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (_appOpenAd != null)
        {
            var responseInfo = _appOpenAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    public bool _isOtherAdShowing = false;
    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("App State changed to : " + state);

        // If the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            ShowAppOpenAd();
        }
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");

        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");
            appOpenFailed++;
            if (appOpenFailed < 5)
            {
                LoadAppOpenAd();
            }
            // It may be useful to load a new ad when the current one is complete.

        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            appOpenFailed++;
            if (appOpenFailed < 5)
            {
                LoadAppOpenAd();
            }
            Debug.Log("App open ad failed to open full screen content with error : "
                            + error);
        };
    }

    #endregion

    #region RewardedVideoAd

    public void LoadRewardedAd()
    {

        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyRewardedAd();
        }

        Debug.Log("Loading rewarded ad.");
        string idVideo = null;
        if (admobTestIds)
        {
            idVideo = "ca-app-pub-3940256099942544/5224354917";
            RewardedID = idVideo;
        }
        else
        {
            idVideo = RewardedID;
        }
        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(idVideo, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.Log("Rewarded ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.Log("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }



            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;
            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);
        });
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    /// 
    public bool IsVideoAdAvailable()
    {
        // if (PlayerPrefs.GetInt("RemoveAds") == 1)
        //     return false;

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _isOtherAdShowing = true;
            Debug.Log("Showing rewarded ad.");
            return true;
        }
        else
        {
            LoadRewardedAd();
            ToastMessage.Instance.ShowToastMessage("Video is not available.");
            return false;
        }
    }

    //public UnityAction reward;
    public void ShowRewarded(UnityAction reward)
    {
        if (IsVideoAdAvailable())
        {
            _rewardedAd.Show(userRewardEarnedCallback =>
            {
                reward?.Invoke();
            });
        }
    }
    public void DestroyRewardedAd()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseRewardedAdInfo()
    {
        if (_rewardedAd != null)
        {
            var responseInfo = _rewardedAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // LogResponseRewardedAdInfo();
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            _isOtherAdShowing = false;
            Invoke(nameof(LoadRewardedAd), 2);
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {

            RewardedFailed++;
            if (RewardedFailed < 5)
            {
                Invoke(nameof(LoadRewardedAd), 2);
            }
            Debug.Log("Rewarded ad failed to open full screen content with error : "
                + error);
        };
    }

    #endregion

    #region RewardedInterstitial
    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedinterstitialAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading rewarded interstitial ad.");
        string idInterVideo = null;
        if (admobTestIds)
        {
            idInterVideo = "ca-app-pub-3940256099942544/5354046379";
            InterData.admobRewardedInterstialID = idInterVideo;
        }
        else
        {
            idInterVideo = InterData.admobRewardedInterstialID;
        }
        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedInterstitialAd.Load(idInterVideo, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.Log("Rewarded interstitial ad failed to load an ad with error : "
                                    + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpexted error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.Log("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Rewarded interstitial ad loaded with response : "
                    + ad.GetResponseInfo());
                rewardedinterstitialAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
            });
    }

    public void ShowAd()
    {
        if (rewardedinterstitialAd != null && rewardedinterstitialAd.CanShowAd())
        {
            rewardedinterstitialAd.Show((Reward reward) =>
            {
                Debug.Log("Rewarded interstitial ad rewarded : " + reward.Amount);
            });
        }
        else
        {
            Debug.Log("Rewarded interstitial ad is not ready yet.");
        }
    }

    public void DestroyAd()
    {
        if (rewardedinterstitialAd != null)
        {
            Debug.Log("Destroying rewarded interstitial ad.");
            rewardedinterstitialAd.Destroy();
            rewardedinterstitialAd = null;
        }
    }

    public bool IsInterVideoAdAvailable()
    {
        // if (PlayerPrefs.GetInt("RemoveAds") == 1)
        //     return false;

        if (rewardedinterstitialAd != null && rewardedinterstitialAd.CanShowAd())
        {
            _isOtherAdShowing = true;
            Debug.Log("Showing rewarded ad.");
            return true;
        }
        else
        {
            LoadRewardedInterstitialAd();
            Debug.Log("Rewarded ad is not ready yet.");
            return false;
        }
    }
    public void LogRewardedResponseInfo()
    {
        if (rewardedinterstitialAd != null)
        {
            var responseInfo = rewardedinterstitialAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    protected void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            RewardedInterFailed++;
            if (RewardedInterFailed < 5)
            {
                Invoke(nameof(LoadRewardedInterstitialAd), 2);
            }
            Debug.Log("Rewarded interstitial ad failed to open full screen content" +
                           " with error : " + error);
        };
    }

    #endregion

    private void OnDestroy()
    {
        // Always unlisten to events when complete.
        if (!string.IsNullOrEmpty(AppOpenID))
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }
}
public enum BannerType
{
    Simple_Banner = 1,
    Smart_Banner = 2,
    Adaptive_Banner = 3
}