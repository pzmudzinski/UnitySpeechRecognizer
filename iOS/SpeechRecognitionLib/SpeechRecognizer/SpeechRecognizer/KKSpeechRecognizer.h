//
//  SpeechRecognizer.h
//  SpeechRecognizer
//
//  Created by Piotr on 03/10/16.
//  Copyright © 2016 kokosoft. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Speech/Speech.h>
#import "RecognitionOptions.h"

@class KKSpeechRecognizer;
@protocol KKSpeechRecognizerDelegate<NSObject>

- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer availabilityDidChange:(BOOL)available;
- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer failedToStartRecordingWithReason:(NSString*)reason;
- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer failedDuringRecordingWithReason:(NSString*)reason;
- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer gotPartialResult:(NSString*)result;
- (void)speechRecognizer:(KKSpeechRecognizer *)recognizer gotFinalResult:(NSString*)result;

@end

typedef NS_ENUM(NSInteger, KKSpeechRecognitionAuthorizationStatus) {
    KKSpeechRecognitionAuthorizationStatusAuthorized,
    KKSpeechRecognitionAuthorizationStatusDenied,
    KKSpeechRecognitionAuthorizationStatusNotDetermined,
    KKSpeechRecognitionAuthorizationStatusRestricted
};


typedef void (^AuthCallback)(KKSpeechRecognitionAuthorizationStatus);

@interface KKSpeechRecognizer : NSObject<SFSpeechRecognizerDelegate>

@property (nonatomic, readonly) BOOL isRecording;
@property (nonatomic, readonly) BOOL isAvailable;
@property (nonatomic, readonly) NSLocale *locale;

@property (nonatomic, assign) id<KKSpeechRecognizerDelegate> delegate;

+ (NSSet<NSLocale *> *)supportedLocales;
+ (KKSpeechRecognitionAuthorizationStatus)authorizationStatus;
+ (BOOL)engineExists;

- (instancetype)init;
- (instancetype)initWithLocale:(NSLocale*)locale;

+ (void)requestAuthorization:(AuthCallback)callback;
- (void)startRecording:(RecognitionOptions)options;
- (void)stopIfRecording;


@end

