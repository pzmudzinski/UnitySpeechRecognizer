package kokosoft.unity.speechrecognition;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.text.TextUtils;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * Created by piotr on 25/11/16.
 */

public class LanguageDetailsChecker extends BroadcastReceiver
{
    public interface LanguageDetailsListener {
        void onLanguagesFetched(List<LanguageOption> supportedLanguages);
    }

    private LanguageDetailsListener mListener;

    public LanguageDetailsChecker(LanguageDetailsListener listener) {
        mListener = listener;
    }

    @Override
    public void onReceive(Context context, Intent intent)
    {
        Bundle results = getResultExtras(true);
        if (results.containsKey(RecognizerIntent.EXTRA_SUPPORTED_LANGUAGES))
        {
            if (mListener != null) {
                List<String> ids = results.getStringArrayList(
                        RecognizerIntent.EXTRA_SUPPORTED_LANGUAGES);
                java.util.Collections.sort(ids);
                List<LanguageOption> options = new ArrayList<>(ids.size());

                Locale defaultLocale ;
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N)
                {
                    defaultLocale = UnityPlayer.currentActivity.getResources().getConfiguration().getLocales().get(0);
                }else
                {
                    defaultLocale = UnityPlayer.currentActivity.getResources().getConfiguration().locale;
                }

                String displayName = "";
                Locale parsedLocale;

                for (String id : ids) {
                    parsedLocale = parseLocale(id);
                    if (parsedLocale != null) {
                        displayName = parsedLocale.getDisplayName(defaultLocale);
                    }
                    options.add(new LanguageOption(id, displayName ));
                }

                mListener.onLanguagesFetched(options);
            }
        }
    }

    private static final Pattern localeMatcher = Pattern.compile
            ("^([^_]*)(_([^_]*)(_#(.*))?)?$");

    public static Locale parseLocale(String value) {
        Matcher matcher = localeMatcher.matcher(value.replace('-', '_'));
        return matcher.find()
                ? TextUtils.isEmpty(matcher.group(5))
                ? TextUtils.isEmpty(matcher.group(3))
                ? TextUtils.isEmpty(matcher.group(1))
                ? null
                : new Locale(matcher.group(1))
                : new Locale(matcher.group(1), matcher.group(3))
                : new Locale(matcher.group(1), matcher.group(3),
                matcher.group(5))
                : null;
    }

}