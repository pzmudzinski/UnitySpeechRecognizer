//
//  UnitySpeechRecognizerDelegate.h
//  SpeechRecognizer
//
//  Created by Piotr on 03/10/16.
//  Copyright © 2016 kokosoft. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "KKSpeechRecognizer.h"

@interface UnitySpeechRecognizerDelegate : NSObject<KKSpeechRecognizerDelegate>

@property (nonatomic, strong) NSString *gameObjectName;

- (instancetype)initWithGameObject:(NSString *)name;
@end
