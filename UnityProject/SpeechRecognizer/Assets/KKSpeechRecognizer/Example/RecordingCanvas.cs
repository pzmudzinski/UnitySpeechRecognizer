using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour {

	public Button startRecordingButton;
	public Text resultText;

	void Start() {
		if (SpeechRecognizer.ExistsOnDevice()) {
			KKSpeechRecognizerListener listener = GameObject.FindObjectOfType<KKSpeechRecognizerListener>();
			listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
			listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
			listener.onErrorDuringRecording.AddListener(OnError);
			listener.onErrorOnStartRecording.AddListener(OnError);
			listener.onFinalResults.AddListener(OnFinalResult);
			listener.onPartialResults.AddListener(OnPartialResult);
			listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
			startRecordingButton.enabled = false;
			SpeechRecognizer.RequestAccess();
		} else {
			resultText.text = "Sorry, but this device doesn't support speech recognition";
			startRecordingButton.enabled = false;
		}

	}

	public void OnFinalResult(string result) {
		resultText.text = result;
	}

	public void OnPartialResult(string result) {
		resultText.text = result;
	}

	public void OnAvailabilityChange(bool available) {
		startRecordingButton.enabled = available;
	}

	public void OnAuthorizationStatusFetched(AuthorizationStatus status) {
		switch (status) {
		case AuthorizationStatus.Authorized:
			startRecordingButton.enabled = true;
			break;
		default:
			startRecordingButton.enabled = false;
			break;
		}
	}

	public void OnEndOfSpeech() {
		startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
	}

	public void OnError(string result) {
		Debug.LogError(result);
	}

	public void OnStartRecordingPressed() {
		if (SpeechRecognizer.IsRecording()) {
			SpeechRecognizer.StopIfRecording();
			startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
		} else {
			SpeechRecognizer.StartRecording(true);
			startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
		}
	}
}
