using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace KKSpeech {
	
	public class AddSpeechFrameworkOniOS {

		public static bool shouldRun = true;

		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			if (shouldRun && target == BuildTarget.iOS)
			{
				// Get target for Xcode project
				string projPath = PBXProject.GetPBXProjectPath(path);

				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));

				string targetName = PBXProject.GetUnityTargetName();
				string projectTarget = proj.TargetGuidByName(targetName);

				// Add dependencies
				Debug.Log("KKSpeechRecognizer Unity: Adding Speech Framework");

				proj.AddFrameworkToProject(projectTarget, "Speech.framework", true);

				File.WriteAllText(projPath, proj.WriteToString());

			}
		}
	}
}
