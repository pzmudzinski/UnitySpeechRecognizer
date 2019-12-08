using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace KKSpeech {
	
	public class AddSpeechFrameworkOniOS {

		public static bool shouldRun = true;

		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			#if UNITY_IOS
			if (shouldRun && target == BuildTarget.iOS)
			{
				// Get target for Xcode project
				string projPath = PBXProject.GetPBXProjectPath(path);

				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));

				// Add dependencies
				Debug.Log("KKSpeechRecognizer Unity: Adding Speech Framework");

				#if UNITY_2019_3_OR_NEWER
					string targetName = proj.GetUnityFrameworkTargetGuid();
					proj.AddFrameworkToProject(targetName, "Speech.framework", true);
				#else
					string targetName = PBXProject.GetUnityTargetName();
					string projectTarget = proj.TargetGuidByName(targetName);
					proj.AddFrameworkToProject(projectTarget, "Speech.framework", true);
				#endif

				File.WriteAllText(projPath, proj.WriteToString());
			}
			#endif
		}
	}
}
