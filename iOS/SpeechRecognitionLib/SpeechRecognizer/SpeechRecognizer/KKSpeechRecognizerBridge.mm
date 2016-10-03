//
//  KKSpeechRecognizer.m
//  SpeechRecognizer
//
//  Created by Piotr on 03/10/16.
//  Copyright © 2016 kokosoft. All rights reserved.
//

#import <Foundation/Foundation.h>

extern void UnitySendMessage(const char *, const char *, const char *);

static NSString *GameObjectName = @"KKSpeechRecognizerListener";


extern "C" {
    
    void _RequestAccess() {
        
    }

    
    void _SendJSMessage(char *message) {

    }
    
}

