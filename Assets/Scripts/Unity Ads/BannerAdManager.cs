using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// 배너 광고를 제어하는 클래스입니다
/// </summary>
public class BannerAdManager : MonoBehaviour
{
    public static BannerAdManager Instance;

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.TOP_CENTER;
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.
    bool _isBannerLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif 
    }

    /// <summary>
    /// 배너를 로드하는 메서드입니다. ShowBannerAd 이전에 반드시 성공해야합니다.
    /// </summary>
    public void LoadBanner()
    {
        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);

        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        _isBannerLoaded = true;        
    }

    private void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }

    /// <summary>
    /// 배너를 보여주는 메서드입니다. LoadSceneManager에서 씬 종류에 따라 호출합니다. 
    /// </summary>
    public void ShowBannerAd()
    {
        if (!_isBannerLoaded)
        {
            Debug.LogError("배너가 로드되지 않았습니다.");
        }
        else
            Debug.Log("배너를 표시합니다");

        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(_adUnitId, options);
    }

    /// <summary>
    /// 배너를 숨겨주는 메서드입니다. LoadSceneManager에서 씬 종류에 따라 호출합니다. 
    /// </summary>
    public void HideBannerAd()
    {
        Debug.Log("배너를 숨깁니다");

        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    private void OnBannerClicked()
    {
        Debug.Log("배너가 클릭되었습니다.");
    }

    private void OnBannerShown()
    {
        Debug.Log("배너가 표시되었습니다.");
    }

    private void OnBannerHidden()
    {
        Debug.Log("배너가 숨겨졌습니다.");
    }
}