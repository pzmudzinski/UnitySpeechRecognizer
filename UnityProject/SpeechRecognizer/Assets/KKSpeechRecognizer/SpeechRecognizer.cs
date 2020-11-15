using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System.Text;
using System.Collections.Generic;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace KKSpeech
{

  public enum AuthorizationStatus
  {
    Authorized,
    Denied,
    NotDetermined,
    Restricted
  }

  public struct SpeechRecognitionOptions
  {
    public bool shouldCollectPartialResults;
    // see: https://developer.android.com/reference/android/speech/RecognizerIntent#EXTRA_SPEECH_INPUT_COMPLETE_SILENCE_LENGTH_MILLIS
    public int? completeSilenceLengthMillis;
    // see: https://developer.android.com/reference/android/speech/RecognizerIntent#EXTRA_SPEECH_INPUT_POSSIBLY_COMPLETE_SILENCE_LENGTH_MILLIS
    public int? possiblyCompleteSilenceLengthMillis;
    // see: https://developer.apple.com/documentation/speech/sfspeechrecognitionrequest/3152603-requiresondevicerecognition?language=objc
    public bool requiresOnDeviceRecognition;
  }

  public struct LanguageOption
  {
    public readonly string id;
    public readonly string displayName;

    public LanguageOption(string id, string displayName)
    {
      this.id = id;
      this.displayName = displayName;
    }
  }

  /*
	 * check readme.pdf before using!
	 */

  public class SpeechRecognizer : System.Object
  {


    [StructLayout(LayoutKind.Sequential)]
    private struct iOSSpeechRecognitionOptions
    {
      [MarshalAs(UnmanagedType.U1)]
      public bool shouldCollectPartialResults;
      [MarshalAs(UnmanagedType.U1)]
      public bool requiresOnDeviceRecognition;
    }

#pragma warning disable CS0162
    public static bool ExistsOnDevice()
    {
#if UNITY_IOS && !UNITY_EDITOR
			return iOSSpeechRecognizer._EngineExists();
#elif UNITY_ANDROID && !UNITY_EDITOR
			return AndroidSpeechRecognizer.EngineExists();
#endif
      return false; // sorry, no support besides Android and iOS :-(
    }

    public static void RequestAccess()
    {
#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer._RequestAccess();
#elif UNITY_ANDROID && !UNITY_EDITOR
  		if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
  		{
     		Permission.RequestUserPermission(Permission.Microphone);
  		}
#endif
    }

    public static bool IsRecording()
    {
#if UNITY_IOS && !UNITY_EDITOR
			return iOSSpeechRecognizer._IsRecording();
#elif UNITY_ANDROID && !UNITY_EDITOR
			return AndroidSpeechRecognizer.IsRecording();
#endif
      return false;
    }

    public static AuthorizationStatus GetAuthorizationStatus()
    {
#if UNITY_IOS && !UNITY_EDITOR
			return (AuthorizationStatus)iOSSpeechRecognizer._AuthorizationStatus();
#elif UNITY_ANDROID && !UNITY_EDITOR
			return (AuthorizationStatus)AndroidSpeechRecognizer.AuthorizationStatus();
#endif
      return AuthorizationStatus.NotDetermined;
    }

    public static void StopIfRecording()
    {
      Debug.Log("StopRecording...");
#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer._StopIfRecording();
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StopIfRecording();
#endif
    }

    public static void StartRecording(SpeechRecognitionOptions options)
    {
#if UNITY_IOS && !UNITY_EDITOR
      iOSSpeechRecognitionOptions iosOptions = new iOSSpeechRecognitionOptions();
      iosOptions.requiresOnDeviceRecognition = options.requiresOnDeviceRecognition;
      iosOptions.shouldCollectPartialResults = options.shouldCollectPartialResults;
      iOSSpeechRecognizer._StartRecording(iosOptions);
#elif UNITY_ANDROID && !UNITY_EDITOR
      AndroidSpeechRecognizer.StartRecording(options);
#endif
    }

    public static void StartRecording(bool shouldCollectPartialResults)
    {
      Debug.Log("StartRecording...");
#if UNITY_IOS && !UNITY_EDITOR
      iOSSpeechRecognitionOptions iosOptions = new iOSSpeechRecognitionOptions();
      iosOptions.shouldCollectPartialResults = shouldCollectPartialResults;
      iOSSpeechRecognizer._StartRecording(iosOptions);
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StartRecording(shouldCollectPartialResults);
#endif
    }

    public static void GetSupportedLanguages()
    {
#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer.SupportedLanguages();
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.GetSupportedLanguages();
#endif
    }

    public static void SetDetectionLanguage(string languageID)
    {
#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer._SetDetectionLanguage(languageID);
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.SetDetectionLanguage(languageID);
#endif
    }
#pragma warning restore CS0162

#if UNITY_IOS && !UNITY_EDITOR
		private class iOSSpeechRecognizer {

			[DllImport ("__Internal")]
			internal static extern void _SetDetectionLanguage(string languageID);

			[DllImport ("__Internal")]
			internal static extern string _SupportedLanguages();

			[DllImport ("__Internal")]
			internal static extern void _RequestAccess();

			[DllImport ("__Internal")]
			internal static extern bool _IsRecording();

			[DllImport ("__Internal")]
			internal static extern bool _EngineExists();

			[DllImport ("__Internal")]
			internal static extern int _AuthorizationStatus();

			[DllImport ("__Internal")]
			internal static extern void _StopIfRecording();

			[DllImport ("__Internal")]
			internal static extern void _StartRecording(iOSSpeechRecognitionOptions options);

			public static void SupportedLanguages() {
				string formattedLangs = _SupportedLanguages();
				var listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
				if (listener != null) {
					listener.SupportedLanguagesFetched(formattedLangs);
				}
			}
		}
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
		private class AndroidSpeechRecognizer {

			private static string DETECTION_LANGUAGE = null;

			internal static void GetSupportedLanguages() {
				GetAndroidBridge().CallStatic("GetSupportedLanguages");
			}

			internal static void SetDetectionLanguage(string languageID) {
				AndroidSpeechRecognizer.DETECTION_LANGUAGE = languageID;
			}

			internal static void RequestAccess() {
				GetAndroidBridge().CallStatic("RequestAccess");
			}
				
			internal static bool IsRecording() {
				return GetAndroidBridge().CallStatic<bool>("IsRecording");
			}

			internal static bool EngineExists() {
				return GetAndroidBridge().CallStatic<bool>("EngineExists");
			}

			internal static int AuthorizationStatus() {
				return GetAndroidBridge().CallStatic<int>("AuthorizationStatus");
			}

			internal static void StopIfRecording() {
				GetAndroidBridge().CallStatic("StopIfRecording");
			}

			internal static void StartRecording(bool shouldCollectPartialResults) {
				var options = new SpeechRecognitionOptions();
				options.shouldCollectPartialResults = shouldCollectPartialResults;
				StartRecording(options);
			}

			internal static void StartRecording(SpeechRecognitionOptions options) {
				GetAndroidBridge().CallStatic("StartRecording", CreateJavaRecognitionOptionsFrom(options));
			}

			private static AndroidJavaObject CreateJavaRecognitionOptionsFrom(SpeechRecognitionOptions options) {
				var javaOptions = new AndroidJavaObject("kokosoft.unity.speechrecognition.SpeechRecognitionOptions");
				javaOptions.Set<bool>("shouldCollectPartialResults", options.shouldCollectPartialResults);
				javaOptions.Set<string>("languageID", DETECTION_LANGUAGE);

				if (options.possiblyCompleteSilenceLengthMillis.HasValue) {
					javaOptions.Set<int>("possiblyCompleteSilenceLengthMillis", options.possiblyCompleteSilenceLengthMillis.Value);
				}

				if (options.completeSilenceLengthMillis.HasValue) {
					javaOptions.Set<int>("completeSilenceLengthMillis", options.completeSilenceLengthMillis.Value);
				}

				return javaOptions;
			}

			private static AndroidJavaObject GetAndroidBridge() {
				var bridge = new AndroidJavaClass("kokosoft.unity.speechrecognition.SpeechRecognizerBridge");
				return bridge;
			}
		}
#endif
  }

}



