using System;
using System.Collections.Generic;

using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class AdManager : MonoBehaviour
{
    [SerializeField] private bool showBanner = false;

    private static BannerView bannerView = null;
    private static InterstitialAd interstitialAd = null;
    private static RewardedAd rewardedAd = null;
    private List<GameObject> rewardedAdCoins;
    private List<GameObject> rewardedAdHints;

    #region UNITY MONOBEHAVIOR METHODS

    private void Awake()
    {
        bannerIsLoaded = false;
        bannerIsShowing = false;
    }

    public void Start()
    {
        Messenger.AddListener(GameEvent.ON_DISABLE_ADS, DestroyAds);
        Messenger<BannerInfo>.AddListener(GameEvent.ON_BANNER_SWITCH, (bannerInfo) => { });
        Messenger<bool>.AddListener(GameEvent.ON_REWARDED_AD, (loaded) => { });

        MobileAds.SetiOSAppPauseOnBackground(true);

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        rewardedAdCoins = new List<GameObject>();
        rewardedAdHints = new List<GameObject>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].CompareTag("RewardedAdCoins"))
            {
                rewardedAdCoins.Add(objs[i].gameObject);
            }
            if (objs[i].CompareTag("RewardedAdHints"))
            {
                rewardedAdHints.Add(objs[i].gameObject);
            }
        }

        if (rewardedAd != null)
        {
            AddRewardedAdHandlers();
            if (rewardedAd.IsLoaded())
            {
                foreach (var obj in rewardedAdCoins) obj.SetActive(true);
                foreach (var obj in rewardedAdHints) obj.SetActive(true);
            }
            Messenger<bool>.Broadcast(GameEvent.ON_REWARDED_AD, true);
        }
        if (interstitialAd != null) AddInterstitialAdHandlers();

        // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
            deviceIds.Add("08C241677009ABAFB0B021E7377CA4A8");
#endif

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Initialization complete");

            if (PlayerData.Ads)
            {
                if (showBanner)
                {
                    RequestBannerAd();
                }
                RequestAndLoadInterstitialAd();
            }

            RequestAndLoadRewardedAd();
        });
    }

    private void OnDestroy()
    {
        if (rewardedAd != null) RemoveRewardedAdHandlers();
        if (interstitialAd != null) RemoveInterstitialAdHandlers();
        if (bannerView != null) bannerView.Destroy();
        Messenger.RemoveListener(GameEvent.ON_DISABLE_ADS, DestroyAds);
        Messenger<BannerInfo>.RemoveListener(GameEvent.ON_BANNER_SWITCH, (bannerInfo) => { });
        Messenger<bool>.RemoveListener(GameEvent.ON_REWARDED_AD, (loaded) => { });
    }

    public void DestroyAds()
    {
        DestroyInterstitialAd();
        DestroyBannerAd();
    }

    #endregion

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }

    #endregion

    #region BANNER ADS

    private static bool bannerIsLoaded = false;
    private static bool bannerIsShowing = false;

    public void RequestBannerAd()
    {
        Debug.Log("Requesting Banner Ad.");

        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-6508972782939335/8385218568";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Add Event Handlers
        bannerView.OnAdLoaded += OnBannerAdLoaded;

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    private void OnBannerAdLoaded(object sender, EventArgs args)
    {
        bannerIsLoaded = true;
        bannerIsShowing = true;
        Messenger<BannerInfo>.Broadcast(GameEvent.ON_BANNER_SWITCH,
            new BannerInfo(bannerView.GetHeightInPixels(), true));
    }

    public static void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            Messenger<BannerInfo>.Broadcast(GameEvent.ON_BANNER_SWITCH, new BannerInfo(0f, false));
        }
    }

    public static void ShowBannerAd()
    {
        if (bannerIsLoaded && !bannerIsShowing && bannerView != null && PlayerData.Ads)
        {
            bannerIsShowing = true;
            bannerView.Show();
        }
    }

    public static void HideBannerAd()
    {
        if (bannerIsLoaded && bannerIsShowing && bannerView != null)
        {
            bannerIsShowing = false;
            bannerView.Hide();
        }
    }

    public struct BannerInfo
    {
        public float height { get; set; }
        public bool isShowing { get; set; }

        public BannerInfo(float height, bool isShowing)
        {
            this.height = height;
            this.isShowing = isShowing;
        }

        public static BannerInfo zero
        {
            get
            {
                return new BannerInfo(0f, false);
            }
        }
    }

    #endregion

    #region INTERSTITIAL ADS

    public void RequestAndLoadInterstitialAd()
    {

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-6508972782939335/5539563372";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial before using it
        if (interstitialAd == null || !interstitialAd.IsLoaded())
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }

            Debug.Log("Requesting Interstitial Ad.");
            interstitialAd = new InterstitialAd(adUnitId);

            // Add Event Handlers
            AddInterstitialAdHandlers();

            // Load an interstitial ad
            interstitialAd.LoadAd(CreateAdRequest());
        }
    }

    private void OnInterstitialAdClosed(object sender, EventArgs args)
    {
        RequestAndLoadInterstitialAd();
    }
    private void AddInterstitialAdHandlers()
    {
        interstitialAd.OnAdClosed += OnInterstitialAdClosed;
    }
    private void RemoveInterstitialAdHandlers()
    {
        interstitialAd.OnAdClosed += OnInterstitialAdClosed;
    }

    public static void ShowInterstitialAd()
    {
        if (PlayerData.Ads)
        {
            if (interstitialAd != null && interstitialAd.IsLoaded())
            {
                Debug.Log("Show Interstitial ad");
                interstitialAd.Show();
            }
            else
            {
                Debug.Log("Interstitial ad is not ready yet");
            }
        }
    }    

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    #endregion

    #region REWARDED ADS

    private static GameItem rewardType = GameItem.Coins;
    private static int rewardAmount = 0;

    public void RequestAndLoadRewardedAd()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-6508972782939335/6815974260";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif
        if (rewardedAd == null || !rewardedAd.IsLoaded())
        {
            Debug.Log("Requesting Rewarded Ad.");

            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }

            // create new rewarded ad instance
            rewardedAd = new RewardedAd(adUnitId);

            // Add Event Handlers
            AddRewardedAdHandlers();

            // Create empty ad request
            rewardedAd.LoadAd(CreateAdRequest());
        }
    }

    private void OnRewardedAdLoaded(object sender, EventArgs args)
    {
        for (int i = 0; i < rewardedAdCoins.Count; i++)
        {
            rewardedAdCoins[i].SetActive(true);
        }
        for (int i = 0; i < rewardedAdHints.Count; i++)
        {
            rewardedAdHints[i].SetActive(true);
        }
        Messenger<bool>.Broadcast(GameEvent.ON_REWARDED_AD, true);
    }

    private void OnRewardedAdOpening(object sender, EventArgs args)
    {
        if (rewardType == GameItem.Coins)
        {
            for (int i = 0; i < rewardedAdCoins.Count; i++)
            {
                rewardedAdCoins[i].SetActive(false);
            }
        }
        else if (rewardType == GameItem.Hints)
        {
            for (int i = 0; i < rewardedAdHints.Count; i++)
            {
                rewardedAdHints[i].SetActive(false);
            }
        }
        Messenger<bool>.Broadcast(GameEvent.ON_REWARDED_AD, false);
    }

    private void OnRewardedAdClosed(object sender, EventArgs args)
    {
        RequestAndLoadRewardedAd();
    }

    private void OnUserEarnedReward(object sender, Reward reward)
    {
        if (rewardType == GameItem.Coins)
        {
            PlayerData.Coins += rewardAmount;
        }
        else if (rewardType == GameItem.Hints)
        {
            PlayerData.Hints += rewardAmount;
        }
    }

    private void AddRewardedAdHandlers()
    {
        rewardedAd.OnAdLoaded += OnRewardedAdLoaded;
        rewardedAd.OnAdOpening += OnRewardedAdOpening;
        rewardedAd.OnAdClosed += OnRewardedAdClosed;
        rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
    }

    private void RemoveRewardedAdHandlers()
    {
        rewardedAd.OnAdLoaded -= OnRewardedAdLoaded;
        rewardedAd.OnAdOpening -= OnRewardedAdOpening;
        rewardedAd.OnAdClosed -= OnRewardedAdClosed;
        rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;
    }

    public static void ShowRewardedAd(RewardedAdData rewardedAdData)
    {
        if (rewardedAd != null)
        {
            rewardType = rewardedAdData.rewardType;
            rewardAmount = rewardedAdData.rewardAmount;
            rewardedAd.Show();
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    #endregion

    #region AD INSPECTOR

    public void OpenAdInspector()
    {
        Debug.Log("Open Ad Inspector.");

        MobileAds.OpenAdInspector((error) =>
        {
            if (error != null)
            {
                string errorMessage = error.GetMessage();
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    Debug.Log("Ad Inspector failed to open, error: " + errorMessage);
                });
            }
            else
            {
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    Debug.Log("Ad Inspector closed.");
                });
            }
        });
    }

    #endregion
}