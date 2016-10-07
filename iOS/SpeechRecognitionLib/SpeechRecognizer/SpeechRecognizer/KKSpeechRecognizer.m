//
//  SpeechRecognizer.m
//  SpeechRecognizer
//
//  Created by Piotr on 03/10/16.
//  Copyright © 2016 kokosoft. All rights reserved.
//

#import "KKSpeechRecognizer.h"

KKSpeechRecognitionAuthorizationStatus KKSpeechRecognitionAuthorizationStatusFromSF(SFSpeechRecognizerAuthorizationStatus sfStatus) {    
    switch (sfStatus) {
        case SFSpeechRecognizerAuthorizationStatusDenied:
            return KKSpeechRecognitionAuthorizationStatusDenied;
        case SFSpeechRecognizerAuthorizationStatusAuthorized:
            return KKSpeechRecognitionAuthorizationStatusAuthorized;
        case SFSpeechRecognizerAuthorizationStatusRestricted:
            return KKSpeechRecognitionAuthorizationStatusRestricted;
        case SFSpeechRecognizerAuthorizationStatusNotDetermined:
            return KKSpeechRecognitionAuthorizationStatusNotDetermined;
    }
}

@interface KKSpeechRecognizer() {
    SFSpeechRecognizer *_internalRecognizer;
    SFSpeechAudioBufferRecognitionRequest *_recognitionRequest;
    SFSpeechRecognitionTask *_recognitionTask;
    AVAudioEngine *_audioEngine;
}

- (void)sendStartRecordingErrorMessage:(NSString *)message;
@end

@implementation KKSpeechRecognizer

+ (NSSet*)supportedLocales {
    return [SFSpeechRecognizer supportedLocales];
}

+ (KKSpeechRecognitionAuthorizationStatus)authorizationStatus {
    return KKSpeechRecognitionAuthorizationStatusFromSF([SFSpeechRecognizer authorizationStatus]);
}

+ (BOOL)engineExists {
    return [SFSpeechRecognizer class] != nil;
}

- (id)init {
    if (self = [super init]) {
        _internalRecognizer = [SFSpeechRecognizer new];
        _internalRecognizer.delegate = self;
        _audioEngine = [AVAudioEngine new];
    }
    return self;
}

- (id)initWithLocale:(NSLocale *)locale {
    if (self = [super init]) {
        _internalRecognizer = [[SFSpeechRecognizer alloc] initWithLocale:locale];
        _internalRecognizer.delegate = self;
        _audioEngine = [AVAudioEngine new];
    }
    return self;
}

- (BOOL)isRecording {
    return [_audioEngine isRunning];
}

- (BOOL)isAvailable {
    return [_internalRecognizer isAvailable];
}

- (void)requestAuthorization:(AuthCallback)callback {
    [SFSpeechRecognizer requestAuthorization:^(SFSpeechRecognizerAuthorizationStatus status) {
        KKSpeechRecognitionAuthorizationStatus wrapperStatus = KKSpeechRecognitionAuthorizationStatusFromSF(status);
        dispatch_async(dispatch_get_main_queue(), ^{
            callback(wrapperStatus);
        });
    }];
}

- (void)startRecording:(BOOL)collectPartialResults {
    if (_recognitionTask != nil) {
        [_recognitionTask cancel];
        _recognitionTask = nil;
    }
    
    NSError *error;
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    [audioSession setCategory:AVAudioSessionCategoryRecord error:&error];
    [audioSession setMode:AVAudioSessionModeMeasurement error:&error];
    [audioSession setActive:YES error:&error];
    
    if (error != nil) {
        NSLog(@"KKSpeechRecognizer: AVAudioSession couldn't be created");
        [self sendStartRecordingErrorMessage:[NSString stringWithFormat:@"%@", error.userInfo]];
        return;
    }
    
    AVAudioInputNode *inputNode = _audioEngine.inputNode;
    if (inputNode == nil) {
        [self sendStartRecordingErrorMessage:@"AVAudioInputNode couldn't be created"];
        return;
    }
    
    _recognitionRequest = [SFSpeechAudioBufferRecognitionRequest new];
    if (_recognitionRequest == nil) {
        [self sendStartRecordingErrorMessage:@"AudioBufferRecognitionRequest couldn't be created"];
        return;
    }
    
    _recognitionRequest.shouldReportPartialResults = collectPartialResults;
    
    _recognitionTask = [_internalRecognizer recognitionTaskWithRequest:_recognitionRequest resultHandler:^(SFSpeechRecognitionResult * _Nullable result, NSError * _Nullable error) {
        
        if (result != nil) {
            if (result.isFinal) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    [_delegate speechRecognizer:self gotFinalResult:result.bestTranscription.formattedString];
                });
            } else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    [_delegate speechRecognizer:self gotPartialResult:result.bestTranscription.formattedString];
                });
            }
        }
        
        if (error != nil) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [_delegate speechRecognizer:self failedDuringRecordingWithReason:[NSString stringWithFormat:@"%@", error.userInfo]];
            });
            [self stopRecording];
        }
    }];
    
    AVAudioFormat *format = [inputNode outputFormatForBus:0];
    
    @try {
        [inputNode installTapOnBus:0 bufferSize:1024 format:format block:^(AVAudioPCMBuffer * _Nonnull buffer, AVAudioTime * _Nonnull when) {
            [_recognitionRequest appendAudioPCMBuffer:buffer];
        }];
    }
    @catch (NSException *exception) {
        NSLog(@"%@", exception.userInfo);
        [self sendStartRecordingErrorMessage:[NSString stringWithFormat:@"%@", exception.userInfo]];
        return;
    }
    
    [_audioEngine prepare];
    
    NSError *startError;
    [_audioEngine startAndReturnError:&startError];
    
    if (startError != nil) {
        [self sendStartRecordingErrorMessage:[NSString stringWithFormat:@"%@", startError.userInfo]];
    }
}

- (void)stopRecording {
    [_audioEngine stop];
    [_audioEngine.inputNode removeTapOnBus:0];
    
    [_recognitionRequest endAudio];
    _recognitionRequest = nil;
    _recognitionTask = nil;
}

- (void)stopIfRecording {
    if (_audioEngine.isRunning) {
        [self stopRecording];
    }
}

- (void)sendStartRecordingErrorMessage:(NSString *)message {
    NSLog(@"KKSpeechRecognizer: error while trying to start recording: %@", message);
    
    dispatch_async(dispatch_get_main_queue(), ^{
        [_delegate speechRecognizer:self failedToStartRecordingWithReason:message];
    });
}

- (void)speechRecognizer:(SFSpeechRecognizer *)speechRecognizer availabilityDidChange:(BOOL)available {
    dispatch_async(dispatch_get_main_queue(), ^{
        [_delegate speechRecognizer:self availabilityDidChange:available];
    });
}
@end
