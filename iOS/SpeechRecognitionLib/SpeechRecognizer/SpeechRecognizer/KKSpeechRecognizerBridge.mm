//
//  KKSpeechRecognizer.m
//  SpeechRecognizer
//
//  Created by Piotr on 03/10/16.
//  Copyright © 2016 kokosoft. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "KKSpeechRecognizer.h"
#import "UnitySpeechRecognizerDelegate.h"

extern "C" {
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

char *stringToChar(NSString *string) {
    return cStringCopy([string UTF8String]);
}


static NSString *GameObjectName = @"KKSpeechRecognizerListener";
static KKSpeechRecognizer *speechRecognizer = nil;
static UnitySpeechRecognizerDelegate *speechDelegate = [[UnitySpeechRecognizerDelegate alloc] initWithGameObject:GameObjectName];

void InitSpeechRecognizerIfDoesntExist(char *languageID) {
    if ([SFSpeechRecognizer class] == nil) {
        return;
    }
    
    BOOL wantsSpecificLanguage = languageID != NULL;
    
    if (wantsSpecificLanguage) {
        NSString *localeID = [NSString stringWithUTF8String:languageID];
        NSLocale *locale = [NSLocale localeWithLocaleIdentifier:localeID];
        
        if (speechRecognizer == nil) {
            speechRecognizer = [[KKSpeechRecognizer alloc] initWithLocale:locale];
            speechRecognizer.delegate = speechDelegate;
        } else {
            // check if current recognizer uses the same language
            NSLocale *currentLocale = speechRecognizer.locale;
            BOOL isUsingTheSameLanguage = [currentLocale.localeIdentifier isEqualToString:locale.localeIdentifier];
            
            if (!isUsingTheSameLanguage) {
                speechRecognizer = [[KKSpeechRecognizer alloc] initWithLocale:locale];
                speechRecognizer.delegate = speechDelegate;
            }
        }
        
    } else {
        if (speechRecognizer == nil) {
            speechRecognizer = [[KKSpeechRecognizer alloc] init];
            speechRecognizer.delegate = speechDelegate;
        } else {
            NSLocale *locale = [NSLocale localeWithLocaleIdentifier:[[NSLocale preferredLanguages] objectAtIndex:0] ];
            NSLocale *currentLocale = speechRecognizer.locale;
            if (![currentLocale.localeIdentifier isEqualToString:locale.localeIdentifier]) {
                speechRecognizer = [[KKSpeechRecognizer alloc] init];
                speechRecognizer.delegate = speechDelegate;
            }
        }
    }
}

KKSpeechRecognizer* GetSpeechRecognizer() {
    
    if ([SFSpeechRecognizer class] == nil) {
        return nil;
    }
    
    if (speechRecognizer == nil) {
        speechRecognizer = [[KKSpeechRecognizer alloc] init];
        speechRecognizer.delegate = speechDelegate;
    }


    return speechRecognizer;
}

NSString *StringFromKKSpeechRecognitionAuthorizationStatus(KKSpeechRecognitionAuthorizationStatus status) {
    switch (status) {
        case KKSpeechRecognitionAuthorizationStatusDenied:
            return @"denied";
        case KKSpeechRecognitionAuthorizationStatusAuthorized:
            return @"authorized";
        case KKSpeechRecognitionAuthorizationStatusRestricted:
            return @"restricted";
        case KKSpeechRecognitionAuthorizationStatusNotDetermined:
            return @"notDetermined";
    }
}

extern "C" {
    
    void _SetDetectionLanguage(char *langID) {
        InitSpeechRecognizerIfDoesntExist(langID);
    }
    
    char* _SupportedLanguages() {
        NSArray<NSLocale*> *locales = [KKSpeechRecognizer supportedLocales].allObjects;
        NSMutableArray<NSString *> *ids = [NSMutableArray arrayWithCapacity:locales.count];
        NSLocale *currentLocale = [NSLocale currentLocale];
        
        [locales enumerateObjectsUsingBlock:^(NSLocale * _Nonnull obj, NSUInteger idx, BOOL * _Nonnull stop) {
            NSString *languageCode = [obj objectForKey:NSLocaleIdentifier];
            NSString *displayName = [currentLocale displayNameForKey:NSLocaleIdentifier value:languageCode];
            [ids addObject:[NSString stringWithFormat:@"%@^%@", languageCode, displayName]];
        }];
        
        NSString *components = [[ids sortedArrayUsingSelector:@selector(localizedCaseInsensitiveCompare:)] componentsJoinedByString:@"|"];
        
        return stringToChar(components);
    }
    
    void _RequestAccess() {
        [KKSpeechRecognizer requestAuthorization:^(KKSpeechRecognitionAuthorizationStatus status) {
            UnitySendMessage([GameObjectName UTF8String], [@"AuthorizationStatusFetched" UTF8String], [StringFromKKSpeechRecognitionAuthorizationStatus(status) UTF8String]);
        }];
    }
    
    BOOL _IsRecording() {
        return GetSpeechRecognizer().isRecording;
    }
    
    BOOL _isAvailable() {
        return GetSpeechRecognizer().isAvailable;
    }
    
    BOOL _EngineExists() {
        return [KKSpeechRecognizer engineExists];
    }
    
    int _AuthorizationStatus() {
        if (_EngineExists()) {
            return [KKSpeechRecognizer authorizationStatus];
        } else {
            return 3; // restricted
        }
    }
    
    void _StopIfRecording() {
        dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0ul);
        dispatch_async(queue, ^{
            [GetSpeechRecognizer() stopIfRecording];
        });
    }
    
    void _StartRecording(RecognitionOptions options) {
        dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0ul);
        dispatch_async(queue, ^{
             [GetSpeechRecognizer() startRecording:options];
        });
       
    }
}

