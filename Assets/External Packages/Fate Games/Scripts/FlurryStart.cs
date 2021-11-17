using FlurrySDK;
using UnityEngine;

public class FlurryStart : MonoBehaviour
{

#if UNITY_ANDROID
    private string FLURRY_API_KEY = "T2C2RMG9HQFMSXSRHZQS";
#elif UNITY_IPHONE
    private string FLURRY_API_KEY = "IOS_API_KEY";
#else
    private string FLURRY_API_KEY = null;
#endif

    void Start()
    {
        // Initialize Flurry.
        new Flurry.Builder()
                  .WithCrashReporting(true)
                  .WithLogEnabled(true)
                  .WithLogLevel(Flurry.LogLevel.VERBOSE)
                  .WithMessaging(true)
                  .Build(FLURRY_API_KEY);
    }
}