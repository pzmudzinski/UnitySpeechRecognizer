using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

namespace KKSpeech {
	
	public class SetSpeechRecognitionPermissionsOniOS {

		public static bool shouldRun = true;
		/* check readme.pdf for explanation on those keys */
		public static string microphoneUsageDescription = "Microphone will be used in Speech Recognition.";
		public static string speechRecognitionUsageDescription = "Speech Recognition will be used to let you talk to the app.";

		#if UNITY_IOS
		private static string nameOfPlist = "Info.plist";
		private static string keyForMicrophoneUsage = "NSMicrophoneUsageDescription";
		private static string keyForSpeechRecognitionUsage = "NSSpeechRecognitionUsageDescription";
		#endif

		[PostProcessBuild]
		public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {
			#if UNITY_IOS
			if (shouldRun && buildTarget == BuildTarget.iOS) {

				// Get plist
				string plistPath = pathToBuiltProject + "/" + nameOfPlist;
				PlistDocument plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath));

				// Get root
				PlistElementDict rootDict = plist.root;

				rootDict.SetString(keyForMicrophoneUsage,microphoneUsageDescription);
				rootDict.SetString(keyForSpeechRecognitionUsage, speechRecognitionUsageDescription);

				// Write to file
				File.WriteAllText(plistPath, plist.WriteToString());
			}
			#endif
		}
	}
}
