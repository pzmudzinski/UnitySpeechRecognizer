using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using KKSpeech;

namespace KKSpeech {
	public class SpeechRecognitionLanguageDropdown : MonoBehaviour {

		private Dropdown dropdown;
		private List<LanguageOption> languageOptions;

		void Start () {
			dropdown = GetComponent<Dropdown>();
			dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
			dropdown.ClearOptions();

			GameObject.FindObjectOfType<SpeechRecognizerListener>().
				onSupportedLanguagesFetched.
				AddListener(OnSupportedLanguagesFetched);

			SpeechRecognizer.GetSupportedLanguages();
		}

		// remember to add ExampleScene to Build Settings!
		public void GoToRecordingScene() {
			SceneManager.LoadScene("ExampleScene");
		}

		void OnDropdownValueChanged(int index) {
			LanguageOption languageOption = languageOptions[index];

			SpeechRecognizer.SetDetectionLanguage(languageOption.id);
		}

		void OnSupportedLanguagesFetched(List<LanguageOption> languages) {
			List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();

			foreach (LanguageOption langOption in languages) {
				dropdownOptions.Add(new Dropdown.OptionData(langOption.displayName));
			}

			dropdown.AddOptions(dropdownOptions);

			languageOptions = languages;
		} 

	}
}

