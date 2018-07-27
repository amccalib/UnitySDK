using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlayFab
{
    public enum WebRequestType
    {
        UnityWww, // High compatability Unity api calls
        HttpWebRequest, // High performance multi-threaded api calls
#if UNITY_2017_2_OR_NEWER
        UnityWebRequest, // Modern unity HTTP component
#endif
        CustomHttp //If this is used, you must set the Http to an IPlayFabHttp object.
    }

    [Flags]
    public enum PlayFabLogLevel
    {
        None = 0,
        Debug = 1 << 0,
        Info = 1 << 1,
        Warning = 1 << 2,
        Error = 1 << 3,
        All = Debug | Info | Warning | Error,
    }

    public static partial class PlayFabSettings
    {
        static PlayFabSettings() { }

        private static PlayFabSharedSettings _playFabShared = null;
        private static PlayFabSharedSettings PlayFabSharedPrivate { get { if (_playFabShared == null) _playFabShared = GetSharedSettingsObjectPrivate(); return _playFabShared; } }
        [Obsolete("This field will become private after Mar 1, 2017", false)]
        public static PlayFabSharedSettings PlayFabShared { get { if (_playFabShared == null) _playFabShared = GetSharedSettingsObjectPrivate(); return _playFabShared; } }
        public const string SdkVersion = "2.47.180716";
        public const string BuildIdentifier = "jbuild_unitysdk_sdk-unity-2-slave_0";
        public const string VersionString = "UnitySDK-2.47.180716";
        private const string DefaultPlayFabApiUrlPrivate = ".playfabapi.com";

        public static readonly Dictionary<string, string> RequestGetParams = new Dictionary<string, string> {
            { "SDKname", "UnitySdk" },
            { "SDKversion", SdkVersion },
        };

        [Obsolete("This field will become private after Mar 1, 2017", false)]
        public static string DefaultPlayFabApiUrl { get { return DefaultPlayFabApiUrlPrivate; } }

        private static PlayFabSharedSettings GetSharedSettingsObjectPrivate()
        {
            var settingsList = Resources.LoadAll<PlayFabSharedSettings>("PlayFabSharedSettings");
            if (settingsList.Length != 1)
            {
                throw new Exception("The number of PlayFabSharedSettings objects should be 1: " + settingsList.Length);
            }
            return settingsList[0];
        }
        [Obsolete("This field will become private after Mar 1, 2017", false)]
        public static PlayFabSharedSettings GetSharedSettingsObject()
        {
            return GetSharedSettingsObjectPrivate();
        }

#if ENABLE_PLAYFABSERVER_API || ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABMATCHMAKER_API || UNITY_EDITOR
        public static string DeveloperSecretKey
        {
            set { PlayFabSharedPrivate.DeveloperSecretKey = value; }
            internal get { return PlayFabSharedPrivate.DeveloperSecretKey; }
        }
#endif

        public static string DeviceUniqueIdentifier
        {
            get
            {
                var deviceId = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
                deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
                return deviceId;
            }
        }


        private static string ProductionEnvironmentUrlPrivate
        {
            get { return !string.IsNullOrEmpty(PlayFabSharedPrivate.ProductionEnvironmentUrl) ? PlayFabSharedPrivate.ProductionEnvironmentUrl : DefaultPlayFabApiUrlPrivate; }
            set { PlayFabSharedPrivate.ProductionEnvironmentUrl = value; }
        }

        // You must set this value for PlayFabSdk to work properly (Found in the Game Manager for your title, at the PlayFab Website)
        public static string TitleId
        {
            get { return PlayFabSharedPrivate.TitleId; }
            set { PlayFabSharedPrivate.TitleId = value; }
        }

        public static PlayFabLogLevel LogLevel
        {
            get { return PlayFabSharedPrivate.LogLevel; }
            set { PlayFabSharedPrivate.LogLevel = value; }
        }

        public static WebRequestType RequestType
        {
            get { return PlayFabSharedPrivate.RequestType; }
            set { PlayFabSharedPrivate.RequestType = value; }
        }

        public static int RequestTimeout
        {
            get { return PlayFabSharedPrivate.RequestTimeout; }
            set { PlayFabSharedPrivate.RequestTimeout = value; }

        }

        public static bool RequestKeepAlive
        {
            get { return PlayFabSharedPrivate.RequestKeepAlive; }
            set { PlayFabSharedPrivate.RequestKeepAlive = value; }
        }

        public static bool CompressApiData
        {
            get { return PlayFabSharedPrivate.CompressApiData; }
            set { PlayFabSharedPrivate.CompressApiData = value; }
        }

        public static string LoggerHost
        {
            get { return PlayFabSharedPrivate.LoggerHost; }
            set { PlayFabSharedPrivate.LoggerHost = value; }

        }

        public static int LoggerPort
        {
            get { return PlayFabSharedPrivate.LoggerPort; }
            set { PlayFabSharedPrivate.LoggerPort = value; }
        }

        public static bool EnableRealTimeLogging
        {
            get { return PlayFabSharedPrivate.EnableRealTimeLogging; }
            set { PlayFabSharedPrivate.EnableRealTimeLogging = value; }
        }

        public static int LogCapLimit
        {
            get { return PlayFabSharedPrivate.LogCapLimit; }
            set { PlayFabSharedPrivate.LogCapLimit = value; }
        }

        public static string GetFullUrl(string apiCall, Dictionary<string, string> getParams)
        {
            StringBuilder getSB = new StringBuilder(1000);
            if (getParams != null)
            {
                foreach (var eachParamPair in getParams)
                {
                    if (getSB.Length == 0)
                        getSB.Append("?");
                    else
                        getSB.Append("&");
                    getSB.Append(eachParamPair.Key).Append("=").Append(eachParamPair.Value);
                }
            }

            string output;
            var baseUrl = ProductionEnvironmentUrlPrivate;
            if (baseUrl.StartsWith("http"))
                output = baseUrl + apiCall;
            else
                output = "https://" + TitleId + baseUrl + apiCall;

            if (getSB.Length > 0)
                output += getSB.ToString();

            return output; // todo final urlSB.tostring() ideally
        }
    }
}
