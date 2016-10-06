package kokosoft.unity.speechrecognition;

import android.Manifest;
import android.content.pm.PackageManager;

import com.unity3d.player.UnityPlayer;

/**
 * Created by piotr on 04/10/16.
 */
@SuppressWarnings("unused")
public class SpeechRecognizerBridge {

    static private final String GAME_OBJECT_NAME = "KKSpeechRecognizerListener";

    static private KKSpeechRecognizer speechRecognizer;

    static private SendToUnitySpeechRecognizerListener listener = new SendToUnitySpeechRecognizerListener(GAME_OBJECT_NAME);

    public static boolean EngineExists() {
        return KKSpeechRecognizer.isRecognitionAvailable(UnityPlayer.currentActivity);
    }

    public static boolean IsRecording() {
        if (speechRecognizer == null) {
            return false;
        } else {
            return speechRecognizer.isRecording();
        }
    }

    public static boolean IsAvailable() {
        return true;
    }

    public static int AuthorizationStatus() {
        int permissionCheck = PermissionStatus();
        if (permissionCheck == PackageManager.PERMISSION_GRANTED) {
            return 0; // authorized
        } else {
            return 1; // denied
        }
    }

    public static void StopIfRecording() {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (speechRecognizer != null ) {
                    speechRecognizer.stopIfRecording();
                }
            }
        });
    }

    public static void StartRecording(final SpeechRecognitionOptions options) {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (speechRecognizer == null) {
                    speechRecognizer = new KKSpeechRecognizer(UnityPlayer.currentActivity);
                    speechRecognizer.setListener(listener);
                }

                speechRecognizer.startRecording(options);
            }
        });
    }

    public static void RequestAccess() {
        int permissionCheck = PermissionStatus();

        String status;
        if (permissionCheck == PackageManager.PERMISSION_GRANTED) {
            status = "authorized";
        } else {
            status = "denied";
        }

        UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "AuthorizationStatusFetched", status);
    }

    private static int PermissionStatus() {
        PackageManager pm = UnityPlayer.currentActivity.getPackageManager();
        int permissionStatus = pm.checkPermission(
                Manifest.permission.RECORD_AUDIO,
                UnityPlayer.currentActivity.getPackageName());
        return permissionStatus;
    }
}
