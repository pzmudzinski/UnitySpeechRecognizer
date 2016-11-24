using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using KKSpeech;

namespace KKSpeech {
	public class SpeechRecognitionLanguageDropdown : MonoBehaviour {

		private Dropdown dropdown;
		List<LanguageOption> languageOptions;
		// Use this for initialization
		void Start () {
			dropdown = GetComponent<Dropdown>();
			dropdown.onValueChanged.AddListener(onDropdownValueChanged);

			List<LanguageOption> languages = SpeechRecognizer.SupportedLanguages();

			dropdown.ClearOptions();

			List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();

			foreach (LanguageOption langOption in languages) {
				dropdownOptions.Add(new Dropdown.OptionData(langOption.displayName));
			}

			dropdown.AddOptions(dropdownOptions);

			languageOptions = languages;
		}

		public void GoToRecordingScene() {
			SceneManager.LoadScene("ExampleScene");
		}

		void onDropdownValueChanged(int index) {
			LanguageOption languageOption = languageOptions[index];

			SpeechRecognizer.SetDetectionLanguage(languageOption.id);
		}

	}
}

