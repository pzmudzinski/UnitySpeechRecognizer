//
//  RecognitionOptions.h
//  SpeechRecognizer
//
//  Created by Piotr Żmudziński on 15/11/2020.
//  Copyright © 2020 kokosoft. All rights reserved.
//

#ifndef RecognitionOptions_h
#define RecognitionOptions_h
typedef struct {
    BOOL shouldCollectPartialResults;
    BOOL requiresOnDeviceRecognition;
} RecognitionOptions;

#endif /* RecognitionOptions_h */
