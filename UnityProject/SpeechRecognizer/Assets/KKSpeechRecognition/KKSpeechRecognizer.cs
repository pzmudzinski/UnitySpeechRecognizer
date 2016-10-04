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

	public class SpeechRecognizer : System.Object {

		public static bool ExistsOnDevice() {
			return iOSSpeechRecognizer._EngineExists();
		}

		public static void RequestAccess() {
			iOSSpeechRecognizer._RequestAccess();
		}

		public static bool IsRecording() {
			return iOSSpeechRecognizer._IsRecording();
		}

		public static AuthorizationStatus GetAuthorizationStatus() {
			return (AuthorizationStatus)iOSSpeechRecognizer._AuthorizationStatus();
		}

		public static void StopIfRecording() {
			Debug.Log("StopRecording...");
			iOSSpeechRecognizer._StopIfRecording();
		}

		public static void StartRecording(bool shouldCollectionPartialResults) {
			Debug.Log("StartRecording...");
			iOSSpeechRecognizer._StartRecording(shouldCollectionPartialResults);
		}


		private class iOSSpeechRecognizer {
			[DllImport ("__Internal")]
			internal static extern void _InitWithLocale(string localeID);

			[DllImport ("__Internal")]
			internal static extern void _RequestAccess();

			[DllImport ("__Internal")]
			internal static extern bool _IsRecording();

			[DllImport ("__Internal")]
			internal static extern bool _EngineExists();

			[DllImport ("__Internal")]
			internal static extern int _AuthorizationStatus();

			[DllImport ("__Internal")]
			internal static extern int _StopIfRecording();

			[DllImport ("__Internal")]
			internal static extern int _StartRecording(bool shouldCollectPartialResults);

		}
	}

}



