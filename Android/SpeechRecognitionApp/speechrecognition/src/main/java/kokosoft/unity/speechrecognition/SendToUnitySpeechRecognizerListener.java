package kokosoft.unity.speechrecognition;

import com.unity3d.player.UnityPlayer;

/**
 * Created by piotr on 04/10/16.
 */

public class SendToUnitySpeechRecognizerListener implements KKSpeechRecognizer.KKSpeechRecognizerListener{

    private String mGameObjectName;

    public SendToUnitySpeechRecognizerListener(String gameObjectName) {
        super();
        mGameObjectName = gameObjectName;
    }

    @Override
    public void onFailedToStartRecordingWithReason(String reason) {
        sendToUnity("FailedToStartRecording", reason);
    }

    @Override
    public void onFailedDuringRecordingWithReason(String reason) {
        sendToUnity("FailedDuringRecording", reason);
    }

    @Override
    public void onReadyForSpeech() {
        sendToUnity("OnReadyForSpeech", "");
    }

    @Override
    public void gotPartialResult(String result) {
        sendToUnity("GotPartialResult", result);
    }

    @Override
    public void gotFinalResult(String result) {
        sendToUnity("GotFinalResult", result);
    }

    @Override
    public void onEndOfSpeech() {
        sendToUnity("OnEndOfSpeech", "");
    }

    private void sendToUnity(String methodName, String param) {
        UnityPlayer.UnitySendMessage(mGameObjectName, methodName, param);
    }
}
