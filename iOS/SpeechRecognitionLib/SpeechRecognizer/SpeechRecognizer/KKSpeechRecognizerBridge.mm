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

static NSString *GameObjectName = @"KKSpeechRecognizerListener";
static KKSpeechRecognizer *speechRecognizer = nil;
static UnitySpeechRecognizerDelegate *speechDelegate = [[UnitySpeechRecognizerDelegate alloc] initWithGameObject:GameObjectName];

extern "C" {
    
    void _InitWithLocale(char *localeID) {
        NSString *string = [NSString stringWithUTF8String:localeID];
        NSLocale *locale = [NSLocale localeWithLocaleIdentifier:string];
        if (locale != nil) {
            speechRecognizer = [[KKSpeechRecognizer alloc] initWithLocale:locale];
            speechRecognizer.delegate = speechDelegate;
        } else {
            NSLog(@"KKSpeechRecognizer error: no %@ language ID found", string);
        }
    }
    
    void _RequestAccess() {
        if (speechRecognizer == nil) {
            speechRecognizer = [[KKSpeechRecognizer alloc] init];
            speechRecognizer.delegate = speechDelegate;
        }
        
        [speechRecognizer requestAuthorization:^(KKSpeechRecognitionAuthorizationStatus status) {
            UnitySendMessage([GameObjectName UTF8String], [@"AuthorizationStatusFetched" UTF8String], [StringFromKKSpeechRecognitionAuthorizationStatus(status) UTF8String]);
        }];
    }
    
    BOOL _IsRecording() {
        return speechRecognizer.isRecording;
    }
    
    BOOL _isAvailable() {
        return speechRecognizer.isAvailable;
    }
    
    int _AuthorizationStatus() {
        return [KKSpeechRecognizer authorizationStatus];
    }
    
    void _StopIfRecording() {
        [speechRecognizer stopIfRecording];
    }
    
    void _StartRecording(BOOL shouldCollectPartialResults) {
        [speechRecognizer startRecording:shouldCollectPartialResults];
    }
    
    BOOL _EngineExists() {
        return [KKSpeechRecognizer engineExists];
    }
}

