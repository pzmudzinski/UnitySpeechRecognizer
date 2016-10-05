using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class KKSetSpeechRecognitionPermissionsOniOS {

	public static bool shouldRun = true;
	public static string microphoneUsageDescription = "Pujt something here about microphone usage";
	public static string speechRecognitionUsageDescription = "Put something here about speech recognition usage";

	private static string nameOfPlist = "Info.plist";
	private static string keyForMicrophoneUsage = "NSMicrophoneUsageDescription";
	private static string keyForSpeechRecognitionUsage = "NSSpeechRecognitionUsageDescription";

	[PostProcessBuild]
	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

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
	}
}