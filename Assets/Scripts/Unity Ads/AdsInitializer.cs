using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// 배너광고를 위한 Unity Ads 서비스 이니셜라이저입니다
/// Unity Ads 서비스 초기화를 성공하면 BannerAdManager를 통해 배너광고를 미리 로드합니다.
/// </summary>
public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    // Unity Ads 서비스 초기화를 성공시 BannerAdManager를 통해 배너광고를 미리 로드
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        BannerAdManager.Instance.LoadBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}