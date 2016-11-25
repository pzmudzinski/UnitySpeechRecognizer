package kokosoft.unity.speechrecognition;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.speech.RecognizerIntent;
import android.text.TextUtils;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.List;

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

    static private LanguageDetailsChecker languageDetailsChecker = new LanguageDetailsChecker(new LanguageDetailsChecker.LanguageDetailsListener() {
        @Override
        public void onLanguagesFetched(List<LanguageOption> supportedLanguages) {
            String[] optionStrings = new String[supportedLanguages.size()];

            LanguageOption option;
            for (int i = 0 ; i < supportedLanguages.size() ; i++) {
                option = supportedLanguages.get(i);
                optionStrings[i] = String.format("%s^%s", option.id, option.displayName);
            }

            String wholeString = TextUtils.join("|", optionStrings);
            UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "SupportedLanguagesFetched", wholeString);
        }
    });

    public static void GetSupportedLanguages() {
        try {
            Intent detailsIntent = new Intent(RecognizerIntent.ACTION_GET_LANGUAGE_DETAILS);
            UnityPlayer.currentActivity.sendOrderedBroadcast(detailsIntent, null, languageDetailsChecker, null, Activity.RESULT_OK, null, null);
        }
        catch (Exception e){
            Log.e("Unity", Log.getStackTraceString(e));
        }
    }

    private static int PermissionStatus() {
        PackageManager pm = UnityPlayer.currentActivity.getPackageManager();
        int permissionStatus = pm.checkPermission(
                Manifest.permission.RECORD_AUDIO,
                UnityPlayer.currentActivity.getPackageName());
        return permissionStatus;
    }
}
