//
//  UnitySpeechRecognizerDelegate.m
//  SpeechRecognizer
//
//  Created by Piotr on 03/10/16.
//  Copyright © 2016 kokosoft. All rights reserved.
//

#import "UnitySpeechRecognizerDelegate.h"

extern void UnitySendMessage(const char *, const char *, const char *);

@implementation UnitySpeechRecognizerDelegate

- (instancetype)initWithGameObject:(NSString *)name {
    if (self = [super init]){
        self.gameObjectName = name;
    }
    return self;
}

- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer availabilityDidChange:(BOOL)available {
    [self callUnityMethod:@"AvailabilityDidChange" withParam:available ? @"1":@"0"];
}

- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer failedDuringRecordingWithReason:(NSString *)reason {
    [self callUnityMethod:@"FailedDuringRecording" withParam:reason];
}

- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer gotPartialResult:(NSString *)result {
    [self callUnityMethod:@"GotPartialResult" withParam:result];
}

- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer gotFinalResult:(NSString *)result {
    [self callUnityMethod:@"GotFinalResult" withParam:result];
}

- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer failedToStartRecordingWithReason:(NSString *)reason {
    [self callUnityMethod:@"FailedToStartRecording" withParam:reason];
}

- (void)callUnityMethod:(NSString *)methodName withParam:(NSString *)param {
    UnitySendMessage([_gameObjectName UTF8String], [methodName UTF8String], [param UTF8String]);
}

@end
