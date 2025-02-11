using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// ��� ���� �����ϴ� Ŭ�����Դϴ�
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
    /// ��ʸ� �ε��ϴ� �޼����Դϴ�. ShowBannerAd ������ �ݵ�� �����ؾ��մϴ�.
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
    /// ��ʸ� �����ִ� �޼����Դϴ�. LoadSceneManager���� �� ������ ���� ȣ���մϴ�. 
    /// </summary>
    public void ShowBannerAd()
    {
        if (!_isBannerLoaded)
        {
            Debug.LogError("��ʰ� �ε���� �ʾҽ��ϴ�.");
        }
        else
            Debug.Log("��ʸ� ǥ���մϴ�");

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
    /// ��ʸ� �����ִ� �޼����Դϴ�. LoadSceneManager���� �� ������ ���� ȣ���մϴ�. 
    /// </summary>
    public void HideBannerAd()
    {
        Debug.Log("��ʸ� ����ϴ�");

        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    private void OnBannerClicked()
    {
        Debug.Log("��ʰ� Ŭ���Ǿ����ϴ�.");
    }

    private void OnBannerShown()
    {
        Debug.Log("��ʰ� ǥ�õǾ����ϴ�.");
    }

    private void OnBannerHidden()
    {
        Debug.Log("��ʰ� ���������ϴ�.");
    }
}