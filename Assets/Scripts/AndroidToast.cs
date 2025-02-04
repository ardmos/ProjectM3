using UnityEngine;

public static class AndroidToast
{
    public static void ShowToast(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject nativeToast = new AndroidJavaObject("com.unity3d.player.NativeToast");
        nativeToast.Call("ShowToast", message);
    }
}

