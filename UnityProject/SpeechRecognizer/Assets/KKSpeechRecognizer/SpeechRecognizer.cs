using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Events;

namespace KKSpeech {
	
	public enum AuthorizationStatus {
		Authorized,
		Denied,
		NotDetermined,
		Restricted
	}

	/*
	 * Please check README file before using this class
	 */ 
	public class SpeechRecognizer : System.Object {

		public static bool ExistsOnDevice() {
			#if UNITY_IOS && !UNITY_EDITOR
			return iOSSpeechRecognizer._EngineExists();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return AndroidSpeechRecognizer.EngineExists();
			#endif
			return false; // sorry, no support besides Android and iOS :-(
		}

		public static void RequestAccess() {
			#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer._RequestAccess();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.RequestAccess();
			#endif
		}

		public static bool IsRecording() {
			#if UNITY_IOS && !UNITY_EDITOR
			return iOSSpeechRecognizer._IsRecording();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return AndroidSpeechRecognizer.IsRecording();
			#endif
			return false;
		}

		public static AuthorizationStatus GetAuthorizationStatus() {
			#if UNITY_IOS && !UNITY_EDITOR
			return (AuthorizationStatus)iOSSpeechRecognizer._AuthorizationStatus();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return (AuthorizationStatus)AndroidSpeechRecognizer.AuthorizationStatus();
			#endif
			return AuthorizationStatus.NotDetermined;
		}

		public static void StopIfRecording() {
			Debug.Log("StopRecording...");
			#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer._StopIfRecording();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StopIfRecording();
			#endif
		}

		public static void StartRecording(bool shouldCollectionPartialResults) {
			Debug.Log("StartRecording...");
			#if UNITY_IOS && !UNITY_EDITOR
			iOSSpeechRecognizer._StartRecording(shouldCollectionPartialResults);
			#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidSpeechRecognizer.StartRecording(shouldCollectionPartialResults);
			#endif
		}


		private class iOSSpeechRecognizer {

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
			internal static extern void _StartRecording(bool shouldCollectPartialResults);

		}

		private class AndroidSpeechRecognizer {

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
				GetAndroidBridge().CallStatic("StartRecording", shouldCollectPartialResults);
			}

			private static AndroidJavaObject GetAndroidBridge() {
				var bridge = new AndroidJavaClass("kokosoft.unity.speechrecognition.SpeechRecognizerBridge");
				return bridge;
			}
		}
	}

}



