package kokosoft.unity.speechrecognition;

import android.Manifest;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.speech.RecognizerIntent;
import android.text.TextUtils;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.Collections;
import java.util.Comparator;
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
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N)
            {
                SpeechRecognizerBridge.sendImplicitOrderedBroadcast(UnityPlayer.currentActivity, detailsIntent, null, languageDetailsChecker, null, Activity.RESULT_OK, null, null);
            } else {
                UnityPlayer.currentActivity.sendOrderedBroadcast(detailsIntent, null, languageDetailsChecker, null, Activity.RESULT_OK, null, null);
            }

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

    private static void sendImplicitOrderedBroadcast(Context context, Intent i, String receiverPermission,
                                              BroadcastReceiver resultReceiver,
                                              Handler scheduler, int initialCode,
                                              String initialData,
                                              Bundle initialExtras) {
        PackageManager pm= context.getPackageManager();
        List<ResolveInfo> matches=pm.queryBroadcastReceivers(i, 0);

        Collections.sort(matches, new Comparator<ResolveInfo>() {
            @Override
            public int compare(ResolveInfo resolveInfo, ResolveInfo t1) {
                return t1.filter.getPriority() - resolveInfo.filter.getPriority();
            }
        });

        for (ResolveInfo resolveInfo : matches) {
            Intent explicit=new Intent(i);
            ComponentName cn=
                    new ComponentName(resolveInfo.activityInfo.applicationInfo.packageName,
                            resolveInfo.activityInfo.name);

            explicit.setComponent(cn);
            context.sendOrderedBroadcast(explicit, receiverPermission, resultReceiver,
                    scheduler, initialCode, initialData, initialExtras);
        }
    }

}
